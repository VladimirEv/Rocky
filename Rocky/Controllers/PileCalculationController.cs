using Microsoft.AspNetCore.Mvc;
using Rocky.Data;
using Rocky.Models;
using System.Collections.Generic;

namespace Rocky.Controllers
{
    public class PileCalculationController : Controller
    {
        private readonly Data.ApplicationDbContext _db;

        

        public PileCalculationController(Data.ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View(); 
        }

        //Get
        public IActionResult SoilTable()
        {
            IEnumerable<SoilProperties> soilList = _db.SoilProperties;
            return View(soilList);
        }

        //Get
        public IActionResult AddSoilTable()
        {
            return View();
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult AddSoilTable(SoilProperties soil)
        {
           if(ModelState.IsValid)
            {
                _db.SoilProperties.Add(soil);
                _db.SaveChanges();
                return RedirectToAction("SoilTable");
            }
            return View(soil);
        }


        //Get
        public IActionResult Edit(int? id)
        {
            if(id==null || id==0)
            {
                return NotFound();
            }

            var soil = _db.SoilProperties.Find(id);

            if(soil==null)
            {
                return NotFound();
            }

            return View(soil);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Edit(SoilProperties soil)
        {
            if(ModelState.IsValid)
            {
                _db.SoilProperties.Update(soil);
                _db.SaveChanges();
                return RedirectToAction(nameof(SoilTable));
            }

            return View(soil);
        }

        //Get
        public IActionResult Delete(int? id)
        {
            if(id==null || id==0)
            {
                return NotFound();
            }

            var soil = _db.SoilProperties.Find(id);

            if(soil==null)
            {
                return NotFound();
            }

            return View(soil);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            if(id==0 || id==null)
            {
                return NotFound();
            }

            var soil = _db.SoilProperties.Find(id);

            _db.SoilProperties.Remove(soil);
            _db.SaveChanges();

            return RedirectToAction(nameof(SoilTable));
        }


    }
}
