using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using eLaptop.Models;
using eLaptop.Repositories;
using eLaptop.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eLaptop.Controllers
{
    [Authorize(Roles = WebConstance.AdminRole)]
    public class CompanyController : Controller
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;

        public CompanyController(ICompanyRepository companyRepository, IMapper mapper)
        {
            _companyRepository = companyRepository;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var companies = await _companyRepository.GetAllAsync();
            var result = _mapper.Map<IList<CompanyVM>>(companies);
            return View(result);
        }


        public async Task<IActionResult> Upsert(int? id)
        {
            var result = new CompanyVM();

            if (id == null) // Creating
                return View(result);


            var company = await _companyRepository.FindAsync(id.GetValueOrDefault());
            result = _mapper.Map<CompanyVM>(company);
            return company == null ? NotFound() : View(result);
        }


        [HttpPost]
        public async Task<IActionResult> Upsert(CompanyVM companyVM)
        {
            if (!ModelState.IsValid)
                return View(companyVM);

            var result = _mapper.Map<Company>(companyVM);

            if (result.Id == 0)
                await _companyRepository.AddAsync(result);
            else
                _companyRepository.Update(result);

            await _companyRepository.SaveAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var company = await _companyRepository.FindAsync(id);
            return company == null ? NotFound() : View(company);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var company = await _companyRepository.FindAsync(id);
            _companyRepository.Remove(company);
            await _companyRepository.SaveAsync();
            return RedirectToAction("Index");
        }
    }
}