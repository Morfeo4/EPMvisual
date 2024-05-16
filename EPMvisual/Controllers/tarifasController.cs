using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EPMvisual;

namespace EPMvisual.Controllers
{
    public class tarifasController : Controller
    {
        private EPMEntities db = new EPMEntities();

        // GET: tarifas
        public ActionResult Index()
        {
            return View(db.tarifa.ToList());
        }

        // GET: tarifas/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tarifa tarifa = db.tarifa.Find(id);
            if (tarifa == null)
            {
                return HttpNotFound();
            }
            return View(tarifa);
        }

        // GET: tarifas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: tarifas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idTarifa,tipo,valor")] tarifa tarifa)
        {
            if (ModelState.IsValid)
            {
                db.tarifa.Add(tarifa);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tarifa);
        }

        // GET: tarifas/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tarifa tarifa = db.tarifa.Find(id);
            if (tarifa == null)
            {
                return HttpNotFound();
            }
            return View(tarifa);
        }

        // POST: tarifas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idTarifa,tipo,valor")] tarifa tarifa)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tarifa).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tarifa);
        }

        // GET: tarifas/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tarifa tarifa = db.tarifa.Find(id);
            if (tarifa == null)
            {
                return HttpNotFound();
            }
            return View(tarifa);
        }

        // POST: tarifas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            tarifa tarifa = db.tarifa.Find(id);
            db.tarifa.Remove(tarifa);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
