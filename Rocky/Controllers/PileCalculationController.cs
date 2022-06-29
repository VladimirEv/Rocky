using Microsoft.AspNetCore.Mvc;
using Rocky.Data;
using Rocky.Models;
using System.Collections.Generic;

namespace Rocky.Controllers
{
    public class PileCalculationController : Controller
    {
        private readonly ApplicationDbContext _db;

        public PileCalculationController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View(); 
        }

        public IActionResult SoilTable()
        {
            IEnumerable<SoilProperties> soilList = _db.SoilProperties;
            return View(soilList);
        }
    }
}
