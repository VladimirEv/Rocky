using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rocky.Data;
using Rocky.Models;
using Rocky.Models.ViewModels;
using Rocky.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Rocky.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        [BindProperty]                                   //привяжем свойство для POST запросов; и теперь нам не нужно указывать это свойство в качестве параметра ЯВНО
        public ProductUserVM ProductUserVM { get; set; }

        public CartController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            List<ShoppingCart> ShoppingCartList = new List<ShoppingCart>(); //создадим новый экземпляр типа ShoppingCart

            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart)!=null 
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0) //проверим существует ли сессия
            {

                ShoppingCartList = HttpContext.Session.Get <List<ShoppingCart>>(WC.SessionCart); // получаем все товары (извлекаем объекты и сессии)
            }

            List<int> prodInCart = ShoppingCartList.Select(i => i.ProductId).ToList(); //получаем все товары из карзины

            IEnumerable<Product> prodList = _db.Product.Where(u => prodInCart.Contains(u.Id)); //извлекаем товары, сравнивая Id c Id из prodInCart

            return View(prodList);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        { 
            return RedirectToAction(nameof(Summary));
        }


        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            //Объекты claim представляют некоторую информацию о пользователе, которую мы можем использовать для авторизации в приложении.
            //у пользователя может быть определенный возраст, город, страна проживания...
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier); //если пользователь заходил в систему, то объект будет получен

            //var userId = User.FindFirstValue(ClaimTypes.Name); // второй способ

            //доступ к корзине покупок
            List<ShoppingCart> ShoppingCartList = new List<ShoppingCart>(); //создадим новый экземпляр типа ShoppingCart

            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0) //проверим существует ли сессия
            {

                ShoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart); // получаем все товары (извлекаем объекты и сессии)
            }

            List<int> prodInCart = ShoppingCartList.Select(i => i.ProductId).ToList(); //получаем все товары из карзины

            IEnumerable<Product> prodList = _db.Product.Where(u => prodInCart.Contains(u.Id)); //извлекаем товары, сравнивая Id c Id из prodInCart

            ProductUserVM = new ProductUserVM()
            {
                ApplicationUser = _db.ApplicationUser.FirstOrDefault(u => u.Id == claim.Value),
                ProductList = prodList
            };

            return View(ProductUserVM);
        }


        public IActionResult Remove (int id)
        {
            List<ShoppingCart> ShoppingCartList = new List<ShoppingCart>(); //создадим новый экземпляр типа ShoppingCart

            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0) //проверим существует ли сессия
            {

                ShoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart); // получаем все товары (извлекаем объекты и сессии)
            }

            ShoppingCartList.Remove(ShoppingCartList.FirstOrDefault(u=>u.ProductId==id));

            HttpContext.Session.Set(WC.SessionCart, ShoppingCartList);

            return RedirectToAction(nameof(Index));

        }
    }
}
