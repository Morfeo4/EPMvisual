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
    public class consumoAguasController : Controller
    {
        private EPMEntities db = new EPMEntities();

        // GET: consumoAguas
        public ActionResult Index()
        {
            var consumoAgua = db.consumoAgua.Include(c => c.cliente);
            return View(consumoAgua.ToList());
        }

        // GET: consumoAguas/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            consumoAgua consumoAgua = db.consumoAgua.Find(id);
            if (consumoAgua == null)
            {
                return HttpNotFound();
            }
            return View(consumoAgua);
        }

        // GET: consumoAguas/Create
        public ActionResult Create()
        {
            ViewBag.cedulaCliente = new SelectList(db.cliente, "cedulaCliente", "nombreCliente");
            return View();
        }

        // POST: consumoAguas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idConsumoAgua,cedulaCliente,promedioConsumoAgua,consumoAguaActual")] consumoAgua consumoAgua)
        {
            if (ModelState.IsValid)
            {
                db.consumoAgua.Add(consumoAgua);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.cedulaCliente = new SelectList(db.cliente, "cedulaCliente", "nombreCliente", consumoAgua.cedulaCliente);
            return View(consumoAgua);
        }

        // GET: consumoAguas/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            consumoAgua consumoAgua = db.consumoAgua.Find(id);
            if (consumoAgua == null)
            {
                return HttpNotFound();
            }
            ViewBag.cedulaCliente = new SelectList(db.cliente, "cedulaCliente", "nombreCliente", consumoAgua.cedulaCliente);
            return View(consumoAgua);
        }

        // POST: consumoAguas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idConsumoAgua,cedulaCliente,promedioConsumoAgua,consumoAguaActual")] consumoAgua consumoAgua)
        {
            if (ModelState.IsValid)
            {
                db.Entry(consumoAgua).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.cedulaCliente = new SelectList(db.cliente, "cedulaCliente", "nombreCliente", consumoAgua.cedulaCliente);
            return View(consumoAgua);
        }

        // GET: consumoAguas/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            consumoAgua consumoAgua = db.consumoAgua.Find(id);
            if (consumoAgua == null)
            {
                return HttpNotFound();
            }
            return View(consumoAgua);
        }

        // POST: consumoAguas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            consumoAgua consumoAgua = db.consumoAgua.Find(id);
            db.consumoAgua.Remove(consumoAgua);
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
