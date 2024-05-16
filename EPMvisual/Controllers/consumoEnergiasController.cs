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
    public class consumoEnergiasController : Controller
    {
        private EPMEntities db = new EPMEntities();

        // GET: consumoEnergias
        public ActionResult Index()
        {
            var consumoEnergia = db.consumoEnergia.Include(c => c.cliente);
            return View(consumoEnergia.ToList());
        }

        // GET: consumoEnergias/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            consumoEnergia consumoEnergia = db.consumoEnergia.Find(id);
            if (consumoEnergia == null)
            {
                return HttpNotFound();
            }
            return View(consumoEnergia);
        }

        // GET: consumoEnergias/Create
        public ActionResult Create()
        {
            ViewBag.cedulaCliente = new SelectList(db.cliente, "cedulaCliente", "nombreCliente");
            return View();
        }

        // POST: consumoEnergias/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idConsumoEnergia,cedulaCliente,metaAhorroEnergia,consumoActualEnergia")] consumoEnergia consumoEnergia)
        {
            if (ModelState.IsValid)
            {
                db.consumoEnergia.Add(consumoEnergia);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.cedulaCliente = new SelectList(db.cliente, "cedulaCliente", "nombreCliente", consumoEnergia.cedulaCliente);
            return View(consumoEnergia);
        }

        // GET: consumoEnergias/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            consumoEnergia consumoEnergia = db.consumoEnergia.Find(id);
            if (consumoEnergia == null)
            {
                return HttpNotFound();
            }
            ViewBag.cedulaCliente = new SelectList(db.cliente, "cedulaCliente", "nombreCliente", consumoEnergia.cedulaCliente);
            return View(consumoEnergia);
        }

        // POST: consumoEnergias/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idConsumoEnergia,cedulaCliente,metaAhorroEnergia,consumoActualEnergia")] consumoEnergia consumoEnergia)
        {
            if (ModelState.IsValid)
            {
                db.Entry(consumoEnergia).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.cedulaCliente = new SelectList(db.cliente, "cedulaCliente", "nombreCliente", consumoEnergia.cedulaCliente);
            return View(consumoEnergia);
        }

        // GET: consumoEnergias/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            consumoEnergia consumoEnergia = db.consumoEnergia.Find(id);
            if (consumoEnergia == null)
            {
                return HttpNotFound();
            }
            return View(consumoEnergia);
        }

        // POST: consumoEnergias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            consumoEnergia consumoEnergia = db.consumoEnergia.Find(id);
            db.consumoEnergia.Remove(consumoEnergia);
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
