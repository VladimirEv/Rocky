using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rocky.Data;
using Rocky.Models;
using Rocky.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;

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
    }
}
