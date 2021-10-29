using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
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
    public class ProductTypeController : Controller
    {
        private readonly IProductTypeRepository _productTypeRepository;
        private readonly IMapper _mapper;

        public ProductTypeController(IProductTypeRepository productTypeRepository, IMapper mapper)
        {
            _productTypeRepository = productTypeRepository;
            _mapper = mapper;
        }

   
        public async Task<IActionResult> Index()
        {
            var productTypes = await _productTypeRepository.GetAllAsync();
            var result = _mapper.Map<IList<ProductTypeVM>>(productTypes);
            return View(result);
        }

     
        public async Task<IActionResult> Upsert(int? id)
        {
            var result = new ProductTypeVM();

            if (id == null) // Creating
                return View(result);


            var productType = await _productTypeRepository.FindAsync(id.GetValueOrDefault());
            result = _mapper.Map<ProductTypeVM>(productType);
            return productType == null ? NotFound() : View(result);
        }

        [HttpPost]
        public async Task<IActionResult> Upsert(ProductTypeVM productTypeVM)
        {
            if (!ModelState.IsValid)
                return View(productTypeVM);

            var result = _mapper.Map<ProductType>(productTypeVM);

            if (result.Id == 0)
                await _productTypeRepository.AddAsync(result);
            else
                _productTypeRepository.Update(result);

            await _productTypeRepository.SaveAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productTypeRepository.FindAsync(id);
            return result == null ? NotFound() : View(result);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var result = await _productTypeRepository.FindAsync(id);
            _productTypeRepository.Remove(result);
            await _productTypeRepository.SaveAsync();
            return RedirectToAction("Index");
        }
    }
}