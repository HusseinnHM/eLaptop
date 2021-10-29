#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Internal;
using eLaptop.Models;
using eLaptop.Repositories;
using eLaptop.ViewModels;
using eLaptop.ViewModels.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace eLaptop.Controllers
{
    public class OrderController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly ShoppingCart _shoppingCart;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;


        public OrderController(
            IProductRepository productRepository,
            IOrderRepository orderRepository,
            ShoppingCart shoppingCart,
            IMapper mapper, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _shoppingCart = shoppingCart;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);

            var result = new List<OrderVM>();

            foreach (var order in orders)
            {
                result.Add(new OrderVM
                {
                    OrderId = order.OrderId,
                    TotalPrice = order.TotalPrice,
                    ShoppingDate = order.ShoppingDate,
                    Area = order.Area,
                    City = order.City,
                    StreetAddress = order.StreetAddress,
                    OrderItems = _mapper.Map<IList<OrderItemVM>>(order.OrderItems),
                });
            }

            return View(result);
        }

        [Authorize(Roles = WebConstance.AdminRole)]
        public async Task<IActionResult> AllOrders()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            var result = new List<OrderAllVM>();

            foreach (var order in orders)
            {
                result.Add(new OrderAllVM()
                {
                    OrderId = order.OrderId,
                    TotalPrice = order.TotalPrice,
                    ShoppingDate = order.ShoppingDate,
                    Area = order.Area,
                    City = order.City,
                    FullName = order.ApplicationUser.FullName,
                    Email = order.ApplicationUser.Email,
                    StreetAddress = order.StreetAddress,
                    OrderItems = _mapper.Map<IList<OrderItemVM>>(order.OrderItems),
                });
            }

            return View(result);
        }


        public IActionResult ShoppingCart()
        {
            var items = _shoppingCart.GetShoppingCartItems();

            if (items == null)
                return RedirectToAction("Index", "Home");


            var result = new ShoppingCartVM()
            {
                ShoppingCartItems = _mapper.Map<IList<ShoppingCartItemVM>>(items),
                ShoppingCartTotal = _shoppingCart.GetShoppingCartTotal()
            };

            return View(result);
        }


        public async Task<IActionResult> AddItemToShoppingCart(int id)
        {
            var item = await _productRepository.FindAsync(id);

            if (!(item != null && _shoppingCart.AddItemToCart(item)))
            {
                TempData[WebConstance.Error] = "You ordered all available amount.";
            }
            
            TempData[WebConstance.Success] = "a item added to cart";

            return RedirectToAction("ShoppingCart");
        }

        public async Task<IActionResult> RemoveItemFromShoppingCart(int id)
        {
            var item = await _productRepository.FindAsync(id);

            if (item != null)
            {
                _shoppingCart.RemoveItemFromCart(id);
            }
            
            TempData[WebConstance.Success] = "a item removed from cart";

            return RedirectToAction("ShoppingCart");
        }

        public async Task<IActionResult> DeleteProductFromShoppingCart(int id)
        {
            var item = await _productRepository.FindAsync(id);

            if (item != null)
            {
                _shoppingCart.DeleteItemFromCart(id);
            }
            
            TempData[WebConstance.Success] = "a product removed from cart";

            return RedirectToAction("ShoppingCart");
        }

        [Authorize]
        public async Task<IActionResult> CompleteOrder()
        {
            var user = await _userManager.GetUserAsync(User);


            var result = new OrderCompleteVM()
            {
                ApplicationUserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                City = user.City,
                Area = user.Area,
                PhoneNumber = user.PhoneNumber,
                StreetAddress = user.StreetAddress,
                ShoppingDate = DateTime.Now,
                OrderItems = new List<OrderItemVM>()
            };
            var items = _shoppingCart.GetShoppingCartItems();
            foreach (var item in items)
            {
                var orderItem = new OrderItemVM()
                {
                    Amount = item.Amount,
                    Price = item.Price,

                    Name = item.Name,
                    ProductId = item.ProductId
                };

                result.TotalPrice += item.Price * item.Amount;

                result.OrderItems.Add(orderItem);
            }


            return View(result);
        }

        [HttpPost,ActionName("CompleteOrder")]
        [Authorize]
        public async Task<IActionResult> CompleteOrderPost(OrderCompleteVM orderVM)
        {
            var items = _shoppingCart.GetShoppingCartItems("Product");

            var invalidProducts = items
                .Where(p => p.Amount > p.Product.Quantity)
                .Select(p => p.Name)
                .ToList();

            if (invalidProducts.Any())
            {
                TempData[WebConstance.Error] =
                    $"Your order of {invalidProducts.Select(p => p + " ")} is more than available.";
                return RedirectToAction("ShoppingCart");
            }

            items.ForAll(p => p.Product.Quantity -= p.Amount);

            await _productRepository.SaveAsync();

            var orderItems = _mapper.Map<IList<OrderItem>>(items);
            var order = _mapper.Map<Order>(orderVM);

            await _orderRepository.StoreOrderAsync(order, orderItems);
            await _orderRepository.SaveAsync();
            await _shoppingCart.ClearShoppingCartAsync();
            
            TempData[WebConstance.Success] = "Your order has been completed successfully";

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> CancelOrder()
        {
            await _shoppingCart.ClearShoppingCartAsync();
            TempData[WebConstance.Success] = "Your order has been canceled";
            return RedirectToAction("Index", "Home");
        }
    }
}