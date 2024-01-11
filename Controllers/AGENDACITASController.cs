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
using RECSALUD.Models.ViewModels;

namespace RECSALUD.Controllers
{
	public class paciente
	{
		public int tipodoc { get; set; }
		public string nombre { get; set; }
		public string apellido { get; set; }
		public string tel1 { get; set; }
		public string tel2 { get; set; }
		public string dir { get; set; }
		public string ent { get; set; }
		public int ses { get; set; }
		public string correo { get; set; }

	}
	public class AGENDACITASController : Controller
	{
		private RECSALUDEntities db = new RECSALUDEntities();

		public JsonResult GetHoras(int profesional)
		{
			try
			{
				
				USUARIOS Profesional = db.USUARIOS.Find(profesional);
				var simult = Profesional.simultnaneos;
				List<Llenar_calendario> llencal = new List<Llenar_calendario>();
				DateTime fechaActual = DateTime.Now;
				DateTime fechafutura = fechaActual.AddDays(180);
				while (fechaActual <= fechafutura)
				{
					var fecha1 = fechaActual.ToString("yyyy-MM-dd")+" 00:00:00";
					DateTime fecha = Convert.ToDateTime(fecha1);

					for (int i = 1; i <= 18; i++)
					{
						var conteocitas = (from a in db.AGENDACITAS
										   where a.fechacita == fecha && a.horacita == i && a.profesional == profesional
										   select a).Count();
						if (simult > conteocitas)
						{
							HORASATENCION ha = db.HORASATENCION.Find(i);
							llencal.Add(new Llenar_calendario() { fecha = fecha, value = ha.id, text = ha.hora, });
							//lst.Add(new SelectListItem() { Text = ha.hora, Value = Convert.ToString(ha.id),});
						}
					}
					fechaActual = fechaActual.AddDays(1);
				}				
				var ListcitasGetCalendarViewModel = llencal.Select(x => new Models.ViewModels.citasGetCalendarViewModel
				{
					Id = Convert.ToInt32(x.value),
					Title = x.text,
					Start = x.fecha.ToString("yyyy-MM-dd") + " " + x.text,
					AllDay = false,
					Color = "#ABB2B9",
					TextColor = "#ffffff"
				}).ToList();
				return Json(new
				{
					Data = ListcitasGetCalendarViewModel,
					IsSuccesful = true
				}, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{

				return Json(new Models.ViewModels.ResponseViewModel
				{
					IsSuccesful = false,
					Errors = new List<string> { ex.Message }
				}, JsonRequestBehavior.AllowGet);
			}

		}

		[HttpPost]
		public JsonResult getpaciente(int documento)
		{
			DATOSPACIENTE pacien = db.DATOSPACIENTE.Find(documento);
			if (pacien != null)
			{
				var response = new paciente() { tipodoc = Convert.ToInt32(pacien.tipodoc), nombre = pacien.nombre, apellido = pacien.apellido, tel1 = pacien.telefono1, tel2 = pacien.telefono2, dir = pacien.direccion, ent = pacien.entidad, ses = Convert.ToInt32(pacien.sesiones), correo = pacien.correo };
				return Json(response, JsonRequestBehavior.AllowGet);
			}
			else
			{
				var response = "";
				return Json(response);
			}
		}
		//CAMBIO ESTADO EN INDEX
			
		public class Observ
		{
			public DateTime fecha { get; set; }
			public string hora { get; set; }
			public int documento { get; set; }
			

		}

		[HttpPost]

		public JsonResult GetObserv(Observ ob)
		{
			var id_hora = (from a in db.HORASATENCION
						   where a.hora == ob.hora
						   select a).FirstOrDefault();

			var id_cita = (from a in db.AGENDACITAS
						   where a.horacita == id_hora.id && a.fechacita == ob.fecha && a.documentop == ob.documento
						   select a).FirstOrDefault();

			var resp = id_cita.observaciones;
			return Json(resp,JsonRequestBehavior.AllowGet);
		}


		#region Cambio_Estado
		public class C_est
		{
			public DateTime fecha { get; set; }
			public string hora { get; set; }
			public int documento { get; set; }
			public int estado { get; set; }

		}
		[HttpPost]

		public JsonResult cambioEstado(C_est c_est)
		{

			var id_hora = (from a in db.HORASATENCION
						   where a.hora == c_est.hora
						   select a).FirstOrDefault();

			var id_cita = (from a in db.AGENDACITAS
						   where a.horacita == id_hora.id && a.fechacita == c_est.fecha && a.documentop == c_est.documento
						   select a).FirstOrDefault();

			AGENDACITAS estado_nuevo = db.AGENDACITAS.Find(id_cita.id);
			// **** RESTAR O SUMAR SESIONES SEGUN EL ESTADO.
			DATOSPACIENTE dp = db.DATOSPACIENTE.Find(id_cita.documentop);
			var estadoant = id_cita.estado;
			if ((estadoant == 1 || estadoant == 2) && (c_est.estado == 3 || c_est.estado == 4))
			{
				dp.sesiones = dp.sesiones - 1;
				db.SaveChanges();
			}
			if ((estadoant == 3 || estadoant == 4) && (c_est.estado == 2))
			{
				dp.sesiones = dp.sesiones + 1;
			}

			estado_nuevo.estado = c_est.estado;
			db.SaveChanges();

		
			var resp = id_cita.documentop;
			return Json(resp);
		}

		#endregion
		[AuthorizeUser(idOperacion: 1)]
		public ActionResult Index2()
		{
			var agenda = db.AGENDACITAS.Include(d => d.ESTADOSPACIENTECITA).Include(d => d.DATOSPACIENTE);
			return View(agenda.ToList());

		}
		public ActionResult Index(int? profesional)
		{


			USUARIOS Profesional = db.USUARIOS.Find(profesional);
			// DOCUMENTO Y NOMBRE DEL PROFESIONAL DUEÑO DE AGENDA
			ViewBag.PROFESIONAL = Profesional.documento;
			ViewBag.PROFESIONALN = Profesional.nombres + " " + Profesional.apellidos;
			return View();
		}
		
			public ActionResult GetIndex(int? profesional)
		{
			try
			{
				//	

				var citas = (from a in db.AGENDACITAS
							 where a.profesional == profesional
							 select a).ToList();
				var ListcitasGetCalendarViewModel = citas.Select(x => new Models.ViewModels.citasGetCalendarViewModel
				{

					Id = Convert.ToInt32(x.documentop),
					Title = x.DATOSPACIENTE.nombre + " " + x.DATOSPACIENTE.apellido,
					Start = x.fechacita.Value.ToString("yyyy-MM-dd") + " " + x.HORASATENCION.hora,
					AllDay = false,
					Color = x.ESTADOSPACIENTECITA.color,
					TextColor = "#ffffff"
				}).ToList();
				return Json(new
				{
					Data = ListcitasGetCalendarViewModel,
					IsSuccesful = true
				}, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{

				return Json(new Models.ViewModels.ResponseViewModel
				{
					IsSuccesful = false,
					Errors = new List<string> { ex.Message }
				}, JsonRequestBehavior.AllowGet);
			}
			//return View(aGENDACITAS.ToList());
		}

		// GET: AGENDACITAS/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			AGENDACITAS aGENDACITAS = db.AGENDACITAS.Find(id);
			if (aGENDACITAS == null)
			{
				return HttpNotFound();
			}
			return View(aGENDACITAS);
		}
		[AuthorizeUser(idOperacion: 1)]
		// GET: AGENDACITAS/Create
		public ActionResult Create()
		{

		

			ViewBag.tipodoc = new SelectList(db.TIPO_DOCUMENTO, "id", "tipo_docu");
			ViewBag.estado = new SelectList(db.ESTADOSPACIENTECITA, "id", "estado");
			ViewBag.horacita = new SelectList(string.Empty, "Value", "Text");
			ViewBag.profesional = new SelectList(db.USUARIOS.Where(item=>item.profesion!=1), "documento", "nombres");
			return View();
		}

		// POST: AGENDACITAS/Create
		// Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
		// más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]

		public ActionResult Create(sesionesViewmodes model)
		{
			try
			{
				using (RECSALUDEntities db = new RECSALUDEntities())
				{
					DATOSPACIENTE datospaciente = db.DATOSPACIENTE.Find(model.documentop);

					if (datospaciente != null)
					{
						datospaciente.sesiones = model.sesiones;
						db.SaveChanges();

						foreach (var citas in model.citas)
						{
							AGENDACITAS agc = new AGENDACITAS();
							agc.fechacita = citas.fechacita;
							agc.horacita = citas.horacita;
							agc.documentop = model.documentop;
							agc.profesional = citas.profesional;
							agc.estado = 1;
							agc.observaciones = citas.observaciones;
							db.AGENDACITAS.Add(agc);
						}
						db.SaveChanges();

						return RedirectToAction("Index", "USUARIOS");
					}
					else 
					{
						// fecha citas simultaneas ya se encuentra validado en el gethoras asegurando que las citas simultaneassean segun lashoras y el profesional
						DATOSPACIENTE datosp = new DATOSPACIENTE();
						datosp.documentop = model.documentop;
						datosp.tipodoc = model.tipodoc;
						datosp.nombre = model.nombre.ToUpper();
						datosp.apellido = model.apellido.ToUpper();
						datosp.telefono1 = model.tel1;
						datosp.telefono2 = model.tel2;
						datosp.direccion = model.dir.ToUpper();
						datosp.entidad = model.ent.ToUpper();
						datosp.sesiones = model.sesiones;
						datosp.correo = model.correo;
						db.DATOSPACIENTE.Add(datosp);
						db.SaveChanges();

						foreach (var Citas in model.citas)
						{
							AGENDACITAS agc = new AGENDACITAS();
							agc.fechacita = Citas.fechacita;
							agc.horacita = Citas.horacita;
							agc.documentop = datosp.documentop;
							agc.profesional = Citas.profesional;
							agc.estado = 1;
							agc.observaciones = Citas.observaciones;
							db.AGENDACITAS.Add(agc);
							
						}
						db.SaveChanges();
						return RedirectToAction("Index", "USUARIOS");
					}
				}
			}
			catch (Exception ex)
			{
				ViewBag.tipodoc = new SelectList(db.TIPO_DOCUMENTO, "id", "tipo_docu");
				ViewBag.horacita = new SelectList(db.HORASATENCION, "id", "hora");
				ViewBag.profesional = new SelectList(db.USUARIOS.Where(item => item.profesion != 1), "documento", "nombres");
				return View();
			}

		}
		// GET: AGENDACITAS/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			AGENDACITAS aGENDACITAS = db.AGENDACITAS.Find(id);
			if (aGENDACITAS == null)
			{
				return HttpNotFound();
			}
			ViewBag.documentop = new SelectList(db.DATOSPACIENTE, "documentop", "nombre", aGENDACITAS.documentop);
			ViewBag.estado = new SelectList(db.ESTADOSPACIENTECITA, "id", "estado", aGENDACITAS.estado);
			ViewBag.horacita = new SelectList(db.HORASATENCION, "id", "hora", aGENDACITAS.horacita);
			ViewBag.profesional = new SelectList(db.USUARIOS, "documento", "nombres", aGENDACITAS.profesional);
			return View(aGENDACITAS);
		}

		// POST: AGENDACITAS/Edit/5
		// Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
		// más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "id,fechacita,horacita,documentop,profesional,estado,observaciones")] AGENDACITAS aGENDACITAS)
		{
			if (ModelState.IsValid)
			{
				db.Entry(aGENDACITAS).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index", "USUARIOS");
			}
			ViewBag.documentop = new SelectList(db.DATOSPACIENTE, "documentop", "nombre", aGENDACITAS.documentop);
			ViewBag.estado = new SelectList(db.ESTADOSPACIENTECITA, "id", "estado", aGENDACITAS.estado);
			ViewBag.horacita = new SelectList(db.HORASATENCION, "id", "hora", aGENDACITAS.horacita);
			ViewBag.profesional = new SelectList(db.USUARIOS, "documento", "nombres", aGENDACITAS.profesional);
			return View(aGENDACITAS);
		}
		[AuthorizeUser(idOperacion: 1)]
		// GET: AGENDACITAS/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			AGENDACITAS aGENDACITAS = db.AGENDACITAS.Find(id);
			if (aGENDACITAS == null)
			{
				return HttpNotFound();
			}
			return View(aGENDACITAS);
		}

		// POST: AGENDACITAS/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			AGENDACITAS aGENDACITAS = db.AGENDACITAS.Find(id);
			int EST = Convert.ToInt16(aGENDACITAS.estado);
			DATOSPACIENTE dp = db.DATOSPACIENTE.Find(aGENDACITAS.documentop);
			int ses = Convert.ToInt16(dp.sesiones);			
			
			if (EST == 1 || EST == 2)
			{
				if (ses > 0 )
				{
					dp.sesiones = dp.sesiones - 1;
					db.SaveChanges();
				}
			}
			db.AGENDACITAS.Remove(aGENDACITAS);
			db.SaveChanges();
			return RedirectToAction("Index", "USUARIOS");
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
