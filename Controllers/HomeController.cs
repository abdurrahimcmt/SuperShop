
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SuperShop.Data;
using SuperShop.Models;
using SuperShop.Models.ViewModels;
using SuperShop.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SuperShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public readonly ApplicationDbContext _db;
        public HomeController(ILogger<HomeController> logger,ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM()
            {
                products=_db.Product.Include(u=> u.Category).Include(u=> u.ApplicationType),
                Categories=_db.Category
            };
            return View(homeVM);
        }

        public IActionResult Details(int ? id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            DetailsVM detailsVM = new DetailsVM()
            {
                product = _db.Product.Include(u => u.Category).Include(u => u.ApplicationType)
                .Where(u => u.Id == id).FirstOrDefault(),
                ExistsInCart = false

            };

            foreach (var item in shoppingCartList)
            {
                if (item.ProductId == id)
                {
                    detailsVM.ExistsInCart = true;
                }
            }

            return View(detailsVM);
        }

        [HttpPost,ActionName("Details")]
        public IActionResult DetailsPost(int id)
        {
            // retrieve shopping card to know how many products in Shopping Cart
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count()>0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }
            shoppingCartList.Add(new ShoppingCart { ProductId = id });
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Index));
        }


        public IActionResult RemoveFromCart(int id)
        {
            // retrieve shopping card to know how many products in Shopping Cart
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            var itemtoRemove = shoppingCartList.SingleOrDefault(r => r.ProductId == id);

            if (itemtoRemove!=null)
            {
                shoppingCartList.Remove(itemtoRemove);
            }
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
