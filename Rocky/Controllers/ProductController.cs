using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rocky.Data;
using Rocky.Models;
using System.Collections.Generic;
using System.Linq;

namespace Rocky.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ProductController (ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> objList = _db.Product;

            foreach (var obj in objList)
            {
                obj.Category = _db.Category.FirstOrDefault(u => u.Id == obj.CategoryId);
            }

            return View(objList);
        }


        //Get - Upsert;  GET-запросы, это те запросы которые возвращают View
        public IActionResult Upsert(int? id)
        {
            IEnumerable<SelectListItem> CategoryDropDown = _db.Category.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });

            ViewBag.CategoryDropDown = CategoryDropDown;//временные данные, которые нужно передать от контроллера к представлению

            Product product = new Product();
            if(id == 0 || id == null)
            {
                //code for Create
                return View(product);
            }
            else
            {
                product = _db.Product.Find(id);
                if(product==null)
                {
                    return NotFound();
                }
                return View(product);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken] //встроенный механизм для форм ввода, в который добавляется специальный токен защиты от взлома и в пост происходит проверка, что токен действителен и безопасность данных сохранена
        public IActionResult Upsert(Category obj)
        {
            if(ModelState.IsValid)
            {
                _db.Category.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index"); // перенапрявляем исполнение кода в метод Index
            }
            return View(obj);
        }


        //Get - Delete;  GET-запросы, это те запросы которые возвращают View
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var obj = _db.Category.Find(id);

            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] //встроенный механизм для форм ввода, в который добавляется специальный токен защиты от взлома и в пост происходит проверка,
                                   //что токен действителен и безопасность данных сохранена
        public IActionResult DeletePost(int? id)
        {
            var obj = _db.Category.Find(id);

            if (obj==null)
            {
                return NotFound();
            } 
                _db.Category.Remove(obj);
                _db.SaveChanges();
                return RedirectToAction("Index"); // перенапрявляем исполнение кода в метод Index
            
        }
    }
}
