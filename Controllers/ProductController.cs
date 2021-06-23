using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SuperShop.Data;
using SuperShop.Models;
using SuperShop.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SuperShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ApplicationDbContext db,IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }
        // GET PRODUCT LIST
        public IActionResult Index()
        {
            IEnumerable<Product> objList = _db.Product.Include(u=> u.Category).Include(u=> u.ApplicationType);
            /*foreach (var obj in objList)
            {
                obj.Category = _db.Category.FirstOrDefault(u => u.Id == obj.CategoryId);
                obj.ApplicationType = _db.ApplicationType.FirstOrDefault(u => u.Id == obj.ApplicationTypeId);
            }*/
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
                }),

                ApplicationTypeSelectList = _db.ApplicationType.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };
            if (Id==null)
            {
                // This is for Create
                return View(ProductVM);
            }
            else
            {
                //This is for Update
                ProductVM.Product = _db.Product.Find(Id);
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
        // Post Upsert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM ProductVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;

                if (ProductVM.Product.Id == 0)
                {
                    // Creating 
                    string upload = webRootPath + WC.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    using (var fileStream= new FileStream(Path.Combine(upload,fileName+extension),FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    ProductVM.Product.Image = fileName + extension;
                    _db.Product.Add(ProductVM.Product);
                }
                else
                {
                    // Updating
                    var objFromDb = _db.Product.AsNoTracking().FirstOrDefault(u=> u.Id == ProductVM.Product.Id);
                    if (files.Count>0)
                    {
                        string upload = webRootPath + WC.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        var OldFile = Path.Combine(upload, objFromDb.Image);
                        if (System.IO.File.Exists(OldFile))
                        {
                            System.IO.File.Delete(OldFile);
                        }
                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }
                        ProductVM.Product.Image = fileName + extension;
                        
                    }
                    else
                    {
                        ProductVM.Product.Image = objFromDb.Image;
                    }
                    _db.Product.Update(ProductVM.Product);
                }
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ProductVM.CategorySelectList = _db.Category.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
                ProductVM.ApplicationTypeSelectList = _db.ApplicationType.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
                return View(ProductVM);
            }
            
        }
        // GET DELETE
        public IActionResult Delete(int ? id)
        {
            if (id==null && id==0)
            {
                return NotFound();
            }
            else
            {
                var obj = _db.Product.Include(u=> u.Category).Include(u => u.ApplicationType).FirstOrDefault(u=> u.Id==id);
                if (obj==null)
                {
                    return NotFound();
                }
                else
                {
                    return View(obj);
                }
            }
        }
        // GET POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        
        public IActionResult DeletePost(int ? id)
        {
            var obj = _db.Product.Find(id);
            if (obj==null)
            {
                return NotFound();
            }
            string upload = _webHostEnvironment.WebRootPath + WC.ImagePath;
            var OldFile = Path.Combine(upload, obj.Image);
            if (System.IO.File.Exists(OldFile))
            {
                System.IO.File.Delete(OldFile);
            }
            _db.Product.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
