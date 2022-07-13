using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;
using Rocky_Models.ViewModels;
using Rocky_Utility;
using System;
using System.Collections.Generic;
using System.IO;

namespace Rocky.Controllers
{
    [Authorize(Roles =WC.AdminRole)]
    public class ProductController : Controller
    {
        private readonly IProductRepository _prodRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController (IProductRepository prodRepo, IWebHostEnvironment webHostEnvironment)
        {
            _prodRepo = prodRepo;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> objList = _prodRepo.GetAll(includeProperties: "Category,ApplicationType"); 

            //foreach (var obj in objList)
            //{
            //    obj.Category = _db.Category.FirstOrDefault(u => u.Id == obj.CategoryId);
            //    obj.ApplicationType = _db.ApplicationType.FirstOrDefault(u => u.Id == obj.ApplicationTypeId);
            //}

            return View(objList);
        }


        //Get - Upsert;  GET-запросы, это те запросы которые возвращают View
        public IActionResult Upsert(int? id)
        {
            //IEnumerable<SelectListItem> CategoryDropDown = _db.Category.Select(i => new SelectListItem
            //{
            //    Text = i.Name,
            //    Value = i.Id.ToString()
            //});
            //
            ////ViewBag.CategoryDropDown = CategoryDropDown;//временные данные, которые нужно передать от контроллера к представлению
            //ViewData["CategoryDropDown"] = CategoryDropDown;
            //Product product = new Product();

            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategorySelectList = _prodRepo.GetAllDropdownList(WC.CategoryName),
                ApplicationTypeSelectList = _prodRepo.GetAllDropdownList(WC.ApplicationTypeName)
            };

            if (id == 0 || id == null)
            {
                //code for Create
                return View(productVM);
            }
            else
            {
                productVM.Product = _prodRepo.Find(id.GetValueOrDefault());
                if(productVM.Product==null)
                {
                    return NotFound();
                }
                return View(productVM);
            }

        }
  
  

        [HttpPost]
        [ValidateAntiForgeryToken] //встроенный механизм для форм ввода, в который добавляется специальный токен защиты от взлома и в пост происходит проверка,
                                   //что токен действителен и безопасность данных сохранена
        public IActionResult Upsert(ProductVM productVM)
        {
            if(ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files; //получаем новое изображение, если оно загружено;
                //HttpContext — это класс в C#, который содержит всю информацию о HTTP-запросе: авторизацию,
                //аутентификацию, запрос, ответ, сеанс, элементы, пользователей, параметры формы и т. д.
                //Каждый HTTP-запрос создает новый объект HttpContext с текущей информацией.

                string webRootPath = _webHostEnvironment.WebRootPath; //путь к папке WWWROOT; получили путь к папке, где будут храниться файлы с картинками
        
                if(productVM.Product.Id == 0)
                {
                    //Creating
                    string upload = webRootPath + WC.ImagePath; // WWWROOT + \images\product\, т.е. получим: j:\D_Programming\С_шарп\MyProjects\AspNetCoreMvc5\Part1\Rocky\Rocky\Rocky\wwwroot\images\product\
                    string fileName = Guid.NewGuid().ToString();              
                    string extension = Path.GetExtension(files[0].FileName); //получим расширение файла; 
                                                                             //Класс Path содержит статические методы для удобной работы с именами файлов

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream); //копируем файл в новое место, которое определяется значением upload
                    }

                    productVM.Product.Image = fileName + extension; //обновляем ссылку на Image внутри сущности продукт, указав новый путь для доступа

                    _prodRepo.Add(productVM.Product);

                    _prodRepo.Save();
                }
                else
                {
                    //Updating                    
                    var objFromDb = _prodRepo.FirstOrDefault(u => u.Id == productVM.Product.Id, isTracking:false); //извлекаем объект из БД, тослько чтобы получить ссылку на имя старого файла (фото)


                    if(files.Count>0)
                    { 
                         string upload = webRootPath + WC.ImagePath;
                         string fileName = Guid.NewGuid().ToString();
                         string extension = Path.GetExtension(files[0].FileName); //получим расширение файла; 
                                                                                  //Класс Path содержит статические методы для удобной работы с именами файлов
                        var oldFile = Path.Combine(upload, objFromDb.Image);//создадим ссылку на старое фото
                        
                        if(System.IO.File.Exists(oldFile)) //если фото существует, удаляем
                        {
                            System.IO.File.Delete(oldFile);
                        }
                         
                         using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                         {
                             files[0].CopyTo(fileStream); //копируем файл в новое место, которое определяется значением upload
                         }

                        productVM.Product.Image = fileName + extension;

                    }

                    else

                    {
                        productVM.Product.Image = objFromDb.Image; //если изображение не изменилось, останется первоначальное изображение
                    }

                    _prodRepo.Update(productVM.Product); // обновляться будет только этот объект
                }
                _prodRepo.Save();
                return RedirectToAction("Index");
        
            }


            productVM.CategorySelectList = _prodRepo.GetAllDropdownList(WC.CategoryName);
            productVM.ApplicationTypeSelectList =_prodRepo.GetAllDropdownList(WC.ApplicationTypeName);

            return View(productVM);
        }


        //Get - Delete;  GET-запросы, это те запросы которые возвращают View
        public IActionResult Delete(int? id)
        {
            if(id==0 || id==null)
            {
                return NotFound();
            }

            //var product = _db.Product.Find(id);  - вместо этого будем использовать жадную загрузку; Вариант 1
            //product.Category = _db.Category.Find(product.CategoryId); //Вариант 1

            //жадная загрузка - это способ сообщийть EF CORE, что когда загружаем Product, нужно модефицировать операцию JOIN в БД
            //и также загрузить соответствующую категорию, если эта запись будет найдена в БД;
            //такой запрос, если извлекается несколько сущностей: var product = _db.Product.Include(u => u.Category).Where(u => u.Id == id);
            var product = _prodRepo.FirstOrDefault(u=>u.Id==id, includeProperties: "Category,ApplicationType");

            if (product == null)
            {
                return NotFound();
            }
          
                return View(product);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken] //встроенный механизм для форм ввода, в который добавляется специальный токен защиты от взлома и в пост происходит проверка,                                  
        public IActionResult DeletePost(int? id)                 //что токен действителен и безопасность данных сохранена
        {
            var obj = _prodRepo.Find(id.GetValueOrDefault());

            if (obj == null)
            {
                return NotFound();
            }

            string upload = _webHostEnvironment.WebRootPath + WC.ImagePath;
            var oldFile = Path.Combine(upload, obj.Image);                  //создадим ссылку на старое фото

            if (System.IO.File.Exists(oldFile)) //если фото существует, удаляем
            {
                System.IO.File.Delete(oldFile);
            }

            _prodRepo.Remove(obj);
            _prodRepo.Save();
            return RedirectToAction("Index"); // перенапрявляем исполнение кода в метод Index

        }
    }
}
