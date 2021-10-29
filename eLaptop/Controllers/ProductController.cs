using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using eLaptop.Data;
using eLaptop.Models;
using eLaptop.Repositories;
using eLaptop.ViewModels;
using eLaptop.ViewModels.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace eLaptop.Controllers
{
    [Authorize(Roles = WebConstance.AdminRole)]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;

        private readonly IMapper _mapper;

        public ProductController(IProductRepository productRepository,
            IMapper mapper)
        {
            _productRepository = productRepository;

            _mapper = mapper;
        }

    
        public async Task<IActionResult> Index()
        {
            IList<Product> products = await _productRepository.GetAllAsync(includeProperties: "Company,ProductType");
            var result = _mapper.Map<IList<ProductIndexVM>>(products);
            return View(result);
        }

       
        public async Task<IActionResult> Upsert(int? id)
        {
            ProductUpsertVM result;
            if (id == null)
            {
                // Creating
                result = new ProductUpsertVM()
                {
                    CompaniesList = _productRepository.GetAllDropdownList(WebConstance.CompanyNames),
                    ProductTypesList = _productRepository.GetAllDropdownList(WebConstance.ProductTypeNames)
                };
                return View(result);
            }

            var product =
                await _productRepository.GetOneAsync(p => p.ProductId == id, includeProperties: "Company,ProductType");

            result = _mapper.Map<ProductUpsertVM>(product);
            result.CompaniesList = _productRepository.GetAllDropdownList(WebConstance.CompanyNames);
            result.ProductTypesList = _productRepository.GetAllDropdownList(WebConstance.ProductTypeNames);

            return (product != null)
                ? View(result)
                : NotFound();
        }



        [HttpPost]
        public async Task<IActionResult> Upsert(ProductUpsertVM productVM)
        {
            var result = _mapper.Map<Product>(productVM);
            if (!ModelState.IsValid)
            {
                productVM.CompaniesList = _productRepository.GetAllDropdownList(WebConstance.CompanyNames);
                productVM.ProductTypesList = _productRepository.GetAllDropdownList(WebConstance.ProductTypeNames);
                return View(productVM);
            }

            if (result.ProductId == 0)
                await _productRepository.AddAsync(result);
            else
                _productRepository.Update(result);

            await _productRepository.SaveAsync();

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Delete(int? id)
        {
            var product =
                await _productRepository.GetOneAsync(p => p.ProductId == id, includeProperties: "Company,ProductType");

            return product == null ? NotFound() : View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.FindAsync(id);
            _productRepository.Remove(product);
            await _productRepository.SaveAsync();
            return RedirectToAction("Index");
        }
    }
}