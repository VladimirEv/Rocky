using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Controllers
{
    public class InquiryController : Controller   //код для вывода заголовка запроса
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
