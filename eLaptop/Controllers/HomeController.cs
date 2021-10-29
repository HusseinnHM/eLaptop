#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using eLaptop.Models;
using eLaptop.Repositories;
using eLaptop.ViewModels;
using eLaptop.ViewModels.Home;
using Microsoft.AspNetCore.Http;

namespace eLaptop.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IProductTypeRepository _productTypeRepository;
        private readonly ShoppingCart _shoppingCart;
        private readonly IMapper _mapper;

        public HomeController(
            IProductRepository productRepository,
            ICompanyRepository companyRepository,
            IProductTypeRepository productTypeRepository,
            ShoppingCart shoppingCart,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _companyRepository = companyRepository;
            _productTypeRepository = productTypeRepository;
            _shoppingCart = shoppingCart;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync(includeProperties: "Company,ProductType");
            var companies = await _companyRepository.GetAllAsync();
            var productTypes = await _productTypeRepository.GetAllAsync();
            var result = new HomeVM()
            {
                Products = _mapper.Map<IList<HomeProductVM>>(products),
                Companies = _mapper.Map<IList<CompanyVM>>(companies),
                ProductTypes = _mapper.Map<IList<ProductTypeVM>>(productTypes),
            };
            return View(result);
        }


        public async Task<IActionResult> Details(int id)
        {
            var product =
                await _productRepository.GetOneAsync(p => p.ProductId == id, includeProperties: "Company,ProductType");
            var result = _mapper.Map<HomeDetailsVM>(product);

            if (result == null)
                return NotFound();

            return View(result);
        }


        public IActionResult Error()
        {
            return View();
        }

     

        [HttpPost]
        public async Task<IActionResult> SearchByName(string? searchString)
        {
            var products = (!string.IsNullOrEmpty(searchString))
                ? await _productRepository.GetAllAsync(u => u.Name.ToLower().Contains(searchString.ToLower()))
                : null;

            var companies = await _companyRepository.GetAllAsync();
            var productTypes = await _productTypeRepository.GetAllAsync();

            var result = new HomeVM()
            {
                Products = _mapper.Map<IList<HomeProductVM>>(products),
                Companies = _mapper.Map<IList<CompanyVM>>(companies),
                ProductTypes = _mapper.Map<IList<ProductTypeVM>>(productTypes),
            };


            return View("Index", result);
        }

        public async Task<IActionResult> AddItemToShoppingCart(int id)
        {
            var item = await _productRepository.FindAsync(id);
            if (!(item != null && _shoppingCart.AddItemToCart(item)))
                TempData[WebConstance.Error] = "You ordered all available amount.";
            else
                TempData[WebConstance.Success] = "a item added from cart";
            
            return RedirectToAction("Index");
        }
    }
}