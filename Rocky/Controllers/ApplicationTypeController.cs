using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rocky_DataAccess.Data;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;
using Rocky_Utility;
using System.Collections.Generic;

namespace Rocky.Controllers
{
    [Authorize(Roles =WC.AdminRole)]
    public class ApplicationTypeController : Controller
    {
        private readonly IApplicationTypeRepository _appTypeRepo;

        public ApplicationTypeController(IApplicationTypeRepository appTypeRepo)
        {
            _appTypeRepo = appTypeRepo;
        }

        public IActionResult Index()
        {
            IEnumerable<ApplicationType> objList = _appTypeRepo.GetAll();

            return View(objList);
        }

        //GET - for Create
        public IActionResult Create()
        {           
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ApplicationType obj)
        {
            if(ModelState.IsValid)
            {
                _appTypeRepo.Add(obj);
                _appTypeRepo.Save();
            return RedirectToAction("Index");
            }
            return View(obj);
        }

        //Get - Edit;  GET-запросы, это те запросы которые возвращают View
        public IActionResult Edit(int? id)
        {
            if(id==0 || id==null)
            {
                return NotFound();
            }

            var obj = _appTypeRepo.Find(id.GetValueOrDefault());

            if(obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] //встроенный механизм для форм ввода, в который добавляется специальный токен защиты от взлома и в пост происходит проверка, что токен действителен и безопасность данных сохранена                             
        public IActionResult Edit(ApplicationType obj)
        {
            if(ModelState.IsValid)
            {
                _appTypeRepo.Update(obj);
                _appTypeRepo.Save();
            return RedirectToAction("Index");
            }
            return View(obj);
        }

        //Get - Delete;  GET-запросы, это те запросы которые возвращают View
        public IActionResult Delete(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }
            var obj = _appTypeRepo.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }


        [HttpPost]
        [ValidateAntiForgeryToken] //встроенный механизм для форм ввода, в который добавляется специальный токен защиты от взлома и в пост происходит проверка, что токен действителен и безопасность данных сохранена                             
        public IActionResult DeletePost(int? id)
        {
            var obj = _appTypeRepo.Find(id.GetValueOrDefault());

          if(obj == null)
            {
                return NotFound();
            }

            _appTypeRepo.Remove(obj);
            _appTypeRepo.Save();
            return RedirectToAction("Index");
        }
    }
}
