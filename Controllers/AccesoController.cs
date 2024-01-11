using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RECSALUD.Controllers
{
	public class AccesoController : Controller
	{
		// GET: Acceso
		public ActionResult Login()
		{
			Session["User"] = null;
			Session["nombre"] = null;
			Session["doc"] = null;
			Session["rol"] = null;
			return View();
		}
		[HttpPost]
		public ActionResult Login(int User, string Pass)
		{
			try
			{
				using (Models.RECSALUDEntities db = new Models.RECSALUDEntities())// CREAMOS CONEXION 
				{
					var oUser = (from d in db.USUARIOS
								 where d.documento == User && d.contrasena == Pass.Trim()
								 select d).FirstOrDefault();

					Session["nombre"] = oUser.nombres+" "+oUser.apellidos;
					Session["doc"] = oUser.documento;
					Session["rol"] = oUser.rol;
					int rol = Convert.ToInt32(oUser.rol);


					if (oUser == null)
					{
						ViewBag.Error = "Usuario o contraseña invalida";
						return View();
					}

					Session["User"] = oUser;
					if (rol == 1)
					{
						return RedirectToAction("Index", "USUARIOS");
					}else
					{
						return RedirectToAction("Index","AGENDACITAS", new { profesional = Session["doc"] });
					}
				}
				
				
				
			}
			catch(Exception ex)
			{
				ViewBag.Error = ex.Message;
				return View();
			}

		}
	}
}