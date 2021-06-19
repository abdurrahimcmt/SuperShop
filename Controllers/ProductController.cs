using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SuperShop.Data;
using SuperShop.Models;
using SuperShop.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ProductController(ApplicationDbContext db)
        {
            _db = db;
        }
        // GET PRODUCT LIST
        public IActionResult Index()
        {
            IEnumerable<Product> objList = _db.Product;
            foreach (var obj in objList)
            {
                obj.Category = _db.Category.FirstOrDefault(u => u.Id == obj.CategoryId);
            }
            return View(objList);
        }

        // GET UPSERT
        public IActionResult Upsert(int ? Id)
        {
            /*IEnumerable<SelectListItem> CategoryDropDown = _db.Category.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            ViewData["CategoryDropDown"] = CategoryDropDown;
            Product product = new Product();*/
            ProductVM ProductVM = new ProductVM
            {
                Product = new Product(),
                CategorySelectList = _db.Category.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };
            if (Id==null)
            {
                return View(ProductVM);
            }
            else
            {
                ProductVM.Product = _db.Product.Find();
                if (ProductVM.Product == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(ProductVM);
                }
            }
        }
    }
}
