using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Rocky_DataAccess.Data;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;
using Rocky_Models.ViewModels;
using Rocky_Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Rocky.Controllers
{
    [Authorize]
    public class CartController : Controller
    {       
        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly IEmailSender _emailSender;

        private readonly IApplicationUserRepository _userRepo;

        private readonly IProductRepository _prodRepo;

        private readonly IInquiryHeaderRepository _inqHRepo;

        private readonly IInquiryDetailRepository _inqDRepo;

        [BindProperty]                                   //привяжем свойство для POST запросов; и теперь нам не нужно указывать это свойство в качестве параметра ЯВНО
        public ProductUserVM ProductUserVM { get; set; }

        public CartController(IWebHostEnvironment webHostEnvironment, IEmailSender emailSender,
            IApplicationUserRepository userRepo, IProductRepository prodRepo, IInquiryHeaderRepository inqHRepo, IInquiryDetailRepository inqDDRepo)
        {
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
            _userRepo = userRepo;
            _prodRepo = prodRepo;
            _inqHRepo = inqHRepo;
            _inqDRepo = inqDDRepo;
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

            IEnumerable<Product> prodList = _prodRepo.GetAll(u => prodInCart.Contains(u.Id)); //извлекаем товары, сравнивая Id c Id из prodInCart

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

            IEnumerable<Product> prodList = _prodRepo.GetAll(u => prodInCart.Contains(u.Id)); //извлекаем товары, сравнивая Id c Id из prodInCart

            ProductUserVM = new ProductUserVM()
            {
                ApplicationUser = _userRepo.FirstOrDefault(u => u.Id == claim.Value),
                ProductList = prodList.ToList()
            };

            return View(ProductUserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(ProductUserVM ProductUserVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var PathToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()   //Path.DirectorySeparatorChar.ToString() - наклонная черта
                + "templates" + Path.DirectorySeparatorChar.ToString() +
                "Inquiry.html";

            var subject = "New Inquiry";
            string HtmlBody = "";

            using (StreamReader sr = System.IO.File.OpenText(PathToTemplate))
            {
                HtmlBody = sr.ReadToEnd(); //прочитаем и сохраним шаблон в объект HtmlBody
            }

            //Name: {0}
            //Email: {1}
            //Phone: {2}
            //Products: {3}

            StringBuilder productListSB = new StringBuilder();
            foreach (var prod in ProductUserVM.ProductList)
            {
                productListSB.Append($" - Name: {prod.Name} <span style='font-size:14px;'>(ID: {prod.Id})</span><br />");
            }

            string messageBody = string.Format(HtmlBody,
                ProductUserVM.ApplicationUser.FullName,
                ProductUserVM.ApplicationUser.Email,
                ProductUserVM.ApplicationUser.PhoneNumber,
                productListSB.ToString()
                );

            await _emailSender.SendEmailAsync(WC.EmailAdmin, subject, messageBody);

            InquiryHeader inquiryHeader = new InquiryHeader()           //заполняем поля для заголовка запроса
            {
                ApplicationUserId = claim.Value,
                FullName = ProductUserVM.ApplicationUser.FullName,
                Email = ProductUserVM.ApplicationUser.Email,
                PhoneNumber = ProductUserVM.ApplicationUser.PhoneNumber,
                InquiryDate = DateTime.Now
             };

            _inqHRepo.Add(inquiryHeader);
            _inqHRepo.Save();

            foreach (var prod in ProductUserVM.ProductList)
            {
                InquiryDetail inquiryDetail = new InquiryDetail()
                {
                    InquiryHeaderId = inquiryHeader.Id,
                    ProductId = prod.Id
                };
                _inqDRepo.Add(inquiryDetail);
            }

            _inqDRepo.Save();  //сохранение внутри цикла foreach может быть не эффективно для нескольки элементов, т.к.
                               //сохраняться будут несколько элементов поочерёдно и получим несколько запросов на сохранение
                               //Если Save вне цикла, то можем сохранить несколько элементов одним запросом

            return RedirectToAction(nameof(InquiryConfirmation));
        }


        public IActionResult InquiryConfirmation()
        {
            HttpContext.Session.Clear(); //очистим данные текущей сессии. Для текущей сессии все товары, которые интересовали клиента, уже попали в запрос
            return View();
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
