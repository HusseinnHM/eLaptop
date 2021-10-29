#nullable enable
using System;
using System.Threading.Tasks;
using AutoMapper;
using eLaptop.Models;
using eLaptop.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace eLaptop.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IMapper _mapper;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger,
            IMapper mapper,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _mapper = mapper;
            _roleManager = roleManager;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            
            var user = await _userManager.GetUserAsync(User);
            var result = _mapper.Map<UpdateInfoVM>(user);
            return View(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Index(UpdateInfoVM model)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(model.Id);
            if (!ModelState.IsValid)
                return View(model);


            user.FullName = model.FullName;
            user.City = model.City;
            user.Area = model.Area;
            user.StreetAddress = model.StreetAddress;


            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData[WebConstance.Success] = "Your profile has been updated successfully";
                return View(model);
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            
            if (!ModelState.IsValid)
                return View(model);

            var user = _mapper.Map<ApplicationUser>(model);
            user.PhoneNumber = !(user.PhoneNumber.StartsWith('0'))
                ? "00963" + user.PhoneNumber
                : "00963" + user.PhoneNumber.Remove(0);

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                //This code in if statement run just for the first time then delete it.
                if (!await _roleManager.RoleExistsAsync(WebConstance.AdminRole))
                {
                    await _roleManager.CreateAsync(new IdentityRole(WebConstance.AdminRole));
                    await _roleManager.CreateAsync(new IdentityRole(WebConstance.CustomerRole)); 
                    await _userManager.AddToRoleAsync(user, WebConstance.AdminRole);
                }
                else
                {

                    if (User.IsInRole(WebConstance.AdminRole))
                        await _userManager.AddToRoleAsync(user, WebConstance.AdminRole);
                    else
                        await _userManager.AddToRoleAsync(user, WebConstance.CustomerRole);
                }

                string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                string confirmationLink = Url.Action("ConfirmEmail", "Account",
                    new { userId = user.Id, token = token }, Request.Scheme);

                _logger.Log(LogLevel.Warning, confirmationLink);

                TempData[WebConstance.Success] = "Registration successful";


                return View("ToConfirmEmail", user.Email);
            }


            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }


            return View(model);
        }

        public async Task<IActionResult> ResendNewToken(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is { EmailConfirmed : true })
                return RedirectToAction("Index", "Home");

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            string confirmationLink = Url.Action("ConfirmEmail", "Account",
                new { userId = user.Id, token = token }, Request.Scheme);

            _logger.Log(LogLevel.Warning, confirmationLink);

            TempData[WebConstance.Success] = "New token has been resend";

            return View("ToConfirmEmail", email);
        }


        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginVM model, string? returnUrl)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("Email", "the email doesn't exist");
                return View(model);
            }

            if (!await _userManager.CheckPasswordAsync(user, model.Password))
            {
                ModelState.AddModelError("Password", "password is wrong");
                return View(model);
            }

            if (user is { EmailConfirmed: false })
            {
                TempData[WebConstance.Error] = $"Your {model.Email} email not confirmed yet";
                return RedirectToActionPermanent("ResendNewToken", new { email = model.Email });
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);

            if (result.Succeeded)
                return (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    ? Redirect(returnUrl)
                    : RedirectToAction("Index", "Home");

            if (result.IsLockedOut)
            {
                TempData[WebConstance.Error] = "Your account is locked out currently";
                return View();
            }

            ModelState.AddModelError(string.Empty, "Invalid Login Attempt");

            return View(model);
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return Json(true);

            return Json($"Email {email} is already in use.");
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token, string? newEmail)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return RedirectToAction("index", "home");

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                TempData[WebConstance.Error] = "The User ID is invalid";
                return NotFound();
            }

            if (!string.IsNullOrEmpty(newEmail))
            {
                var result = await _userManager.ChangeEmailAsync(user, newEmail, token);
                if (result.Succeeded)
                    return RedirectToAction("Index", "Home");
            }
            else
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, true);
                    return RedirectToAction("Index", "Home");
                }
            }


            TempData[WebConstance.Error] = "Email cannot be confirmed";
            return View("Error");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgetPasswordVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);


            if (user == null)
            {
                ModelState.AddModelError("Email", "Not match");
                return View(model);
            }

            if (user is { EmailConfirmed : false })
            {
                TempData[WebConstance.Error] = $"Your {model.Email} email not confirmed yet";
                return RedirectToAction("ResendNewToken", new { email = model.Email });
            }

            string token = await _userManager.GeneratePasswordResetTokenAsync(user);

            string passwordResetLink = Url.Action("ResetPassword", "Account",
                new { email = model.Email, token = token }, Request.Scheme);

            _logger.Log(LogLevel.Warning, passwordResetLink);

            return View("ForgotPasswordConfirmation", model.Email);
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
                ModelState.AddModelError("", "Invalid password reset token");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

                if (result.Succeeded)
                    return View("ResetPasswordConfirmation");

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);


                return View(model);
            }


            return View("ResetPasswordConfirmation");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var result = await _userManager.ChangePasswordAsync(user,
                model.CurrentPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user);
            TempData[WebConstance.Success] = "Your password has been changed";
            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> ChangeEmail(string? email)
        {
            return View(new ChangeEmailVM()
            {
                Email = email ?? await _userManager.GetEmailAsync(await _userManager.GetUserAsync(User))
            });
        }

        [HttpPost]
        public async Task<IActionResult> ChangeEmail(ChangeEmailVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("Email", "email doesn't excite");
                return View(model);
            }

            if (!await _userManager.CheckPasswordAsync(user, model.Password))
            {
                ModelState.AddModelError("Password", "Password is wrong");
                return View(model);
            }

            string token = await _userManager.GenerateChangeEmailTokenAsync(user, model.NewEmail);

            string changeEmailToken = Url.Action("ConfirmEmail", "Account",
                new { userId = user.Id, token = token, newEmail = model.NewEmail }, Request.Scheme);

            _logger.Log(LogLevel.Warning, changeEmailToken);

            return View("ToConfirmEmail", model.NewEmail);
        }
    }
}