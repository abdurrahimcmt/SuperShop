using Microsoft.AspNetCore.Mvc;
using SuperShop.Data;
using SuperShop.Models;
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
            Product product = new Product();
            if (Id==null)
            {
                return View(product);
            }
            else
            {
                product = _db.Product.Find();
                if (product==null)
                {
                    return NotFound();
                }
                else
                {
                    return View(product);
                }
            }
        }
    }
}
