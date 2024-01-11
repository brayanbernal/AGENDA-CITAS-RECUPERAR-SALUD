using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RECSALUD.Filters;
using RECSALUD.Models;

namespace RECSALUD.Controllers
{
    public class DATOSPACIENTEsController : Controller
    {
        private RECSALUDEntities db = new RECSALUDEntities();
		[AuthorizeUser(idOperacion: 1)]
		// GET: DATOSPACIENTEs
		public ActionResult Index()
        {
            var dATOSPACIENTE = db.DATOSPACIENTE.Include(d => d.TIPO_DOCUMENTO);
            return View(dATOSPACIENTE.ToList());
        }

        // GET: DATOSPACIENTEs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DATOSPACIENTE dATOSPACIENTE = db.DATOSPACIENTE.Find(id);
            if (dATOSPACIENTE == null)
            {
                return HttpNotFound();
            }
            return View(dATOSPACIENTE);
        }
		[AuthorizeUser(idOperacion: 1)]

		// GET: DATOSPACIENTEs/Edit/5
		public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DATOSPACIENTE dATOSPACIENTE = db.DATOSPACIENTE.Find(id);
            if (dATOSPACIENTE == null)
            {
                return HttpNotFound();
            }
            ViewBag.tipodoc = new SelectList(db.TIPO_DOCUMENTO, "id", "tipo_docu", dATOSPACIENTE.tipodoc);
            return View(dATOSPACIENTE);
        }

        // POST: DATOSPACIENTEs/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "documentop,tipodoc,nombre,apellido,telefono1,telefono2,direccion,entidad,sesiones,correo")] DATOSPACIENTE dATOSPACIENTE)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dATOSPACIENTE).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.tipodoc = new SelectList(db.TIPO_DOCUMENTO, "id", "tipo_docu", dATOSPACIENTE.tipodoc);
            return View(dATOSPACIENTE);
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
