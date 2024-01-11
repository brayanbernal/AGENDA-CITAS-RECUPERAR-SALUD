using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RECSALUD.Controllers
{
	public class ErrorController : Controller
	{
		[HttpGet]
		public ActionResult UnauthorizedOperation(String operacion, String modulo, String msjeErrorExcepcion)
		{
			//agregar los viebag del error 
			//Para mstrar nombre y decir que no tiene acceso 


			return View();
		}
	}
}