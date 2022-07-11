using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rocky_DataAccess.Data;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;
using Rocky_Utility;
using System.Collections.Generic;

namespace Rocky.Controllers
{
    [Authorize(Roles=WC.AdminRole)]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _catRepo;

        public CategoryController(ICategoryRepository catRepo)
        {
            _catRepo = catRepo;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> objList = _catRepo.GetAll();
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
            if(ModelState.IsValid)
            {
                _catRepo.Add(obj);
                _catRepo.Save();
                return RedirectToAction("Index"); // перенапрявляем исполнение кода в метод Index
            }
            return View(obj);
        }

        //Get - Edit;  GET-запросы, это те запросы которые возвращают View
        public IActionResult Edit(int? id)
        {
            if (id==null || id==0)
            {
                return NotFound();
            }

            var obj = _catRepo.Find(id.GetValueOrDefault());

            if(obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] //встроенный механизм для форм ввода, в который добавляется специальный токен защиты от взлома и в пост происходит проверка,
                                   //что токен действителен и безопасность данных сохранена
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _catRepo.Update(obj);
                _catRepo.Save();
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

            var obj = _catRepo.Find(id.GetValueOrDefault());

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
            var obj = _catRepo.Find(id.GetValueOrDefault());

            if (obj==null)
            {
                return NotFound();
            }
            _catRepo.Remove(obj);
            _catRepo.Save();
                return RedirectToAction("Index"); // перенапрявляем исполнение кода в метод Index
            
        }
    }
}
