using Microsoft.AspNetCore.Mvc;
using Rocky.Data;
using Rocky.Models;
using System.Collections.Generic;

namespace Rocky.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> objList = _db.Category;
            return View(objList);
        }


        //Get - Create;  GET-запросы, это те запросы которые возвращают View
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] //встроенный механизм для форм ввода, в который добавляется специальный токен защиты от взлома и в пост происходит проверка, что токен действителен и безопасность данных сохранена
        public IActionResult Create(Category obj)
        {
            _db.Category.Add(obj);
            _db.SaveChanges();
            return RedirectToAction("Index"); // перенапрявляем исполнение кода в метод Index
        }
    }
}
