using Microsoft.AspNetCore.Mvc;
using Rocky_DataAccess.Repository;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Controllers
{
    public class InquiryController : Controller   //код для вывода заголовка запроса
    {
        private readonly IInquiryHeaderRepository _inqHRepo;
        private readonly IInquiryDetailRepository _inqDRepo;

        [BindProperty]
        public InquiryVM InquiryVM { get; set; }


        public InquiryController(IInquiryHeaderRepository inqHRepo, IInquiryDetailRepository inqDRepo)
        {
            _inqHRepo = inqHRepo;
            _inqDRepo = inqDRepo;
        }

        public IActionResult Index()
        {
            return View();
        }

        //настраиваем таблицу с деталями запроса
        [HttpGet]
        public IActionResult Details(int id)
        {
            InquiryVM = new InquiryVM()
            {
                InquiryHeader = _inqHRepo.FirstOrDefault(u=>u.Id==id),
                InquiryDetail = _inqDRepo.GetAll(u=>u.InquiryHeaderId==id, includeProperties:"Product")
            };

            return View(InquiryVM);
        }


        #region API CALLS (API для получени всех запросов и представления их в ответ на вызов API)

        [HttpGet]
        public IActionResult GetInquiryList()
        {
            return Json(new { data = _inqHRepo.GetAll() });  //получаем запросы клиентов из БД и передаём их из этого метода в виде JSON объекта
        }

        #endregion
    }
}
