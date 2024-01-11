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
    public class USUARIOSController : Controller
    {
        private RECSALUDEntities db = new RECSALUDEntities();
		[AuthorizeUser(idOperacion: 1)]
		// GET: USUARIOS
		[HttpPost]
		public JsonResult GetUsuario(int documento)
		{
			var response = "";
			USUARIOS Usu = db.USUARIOS.Find(documento);
			if (Usu != null)
			{
				 response = "1";
				
			}
			else
			{
				 response = "";
				
			}
			return Json(response);
		}






		public ActionResult Index()
        {
            var uSUARIOS = db.USUARIOS.Include(u => u.PROFESIONES).Include(u => u.ROLES);
            return View(uSUARIOS.ToList());
        }

        // GET: USUARIOS/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            USUARIOS uSUARIOS = db.USUARIOS.Find(id);
            if (uSUARIOS == null)
            {
                return HttpNotFound();
            }
            return View(uSUARIOS);
        }
		[AuthorizeUser(idOperacion: 1)]
		// GET: USUARIOS/Create
		public ActionResult Create()
        {
            ViewBag.profesion = new SelectList(db.PROFESIONES, "id", "profesion");
            ViewBag.rol = new SelectList(db.ROLES, "id", "rol");
            return View();
        }

        // POST: USUARIOS/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "documento,nombres,apellidos,rol,profesion,simultnaneos,correo,contrasena,activo")] USUARIOS uSUARIOS)
        {
            if (ModelState.IsValid)
            {
				uSUARIOS.activo = 1;
				uSUARIOS.nombres=uSUARIOS.nombres.ToUpper();
				uSUARIOS.apellidos=uSUARIOS.apellidos.ToUpper();
				db.USUARIOS.Add(uSUARIOS);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.profesion = new SelectList(db.PROFESIONES, "id", "profesion", uSUARIOS.profesion);
            ViewBag.rol = new SelectList(db.ROLES, "id", "rol", uSUARIOS.rol);
            return View(uSUARIOS);
        }
		[AuthorizeUser(idOperacion: 1)]
		// GET: USUARIOS/Edit/5
		public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            USUARIOS uSUARIOS = db.USUARIOS.Find(id);
            if (uSUARIOS == null)
            {
                return HttpNotFound();
            }
            ViewBag.profesion = new SelectList(db.PROFESIONES, "id", "profesion", uSUARIOS.profesion);
            ViewBag.rol = new SelectList(db.ROLES, "id", "rol", uSUARIOS.rol);
            return View(uSUARIOS);
        }

        // POST: USUARIOS/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "documento,nombres,apellidos,rol,profesion,simultnaneos,correo,contrasena,activo")] USUARIOS uSUARIOS)
        {
            if (ModelState.IsValid)
            {
                db.Entry(uSUARIOS).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.profesion = new SelectList(db.PROFESIONES, "id", "profesion", uSUARIOS.profesion);
            ViewBag.rol = new SelectList(db.ROLES, "id", "rol", uSUARIOS.rol);
            return View(uSUARIOS);
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
