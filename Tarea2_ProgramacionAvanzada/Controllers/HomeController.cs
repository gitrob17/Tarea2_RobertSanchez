using Antlr.Runtime.Misc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tarea2_ProgramacionAvanzada.Models;

namespace Tarea2_ProgramacionAvanzada.Controllers
{
    public class HomeController : Controller
    {
        private string archivoDestinos = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/destinos.json");

        private static List<string> paises = new List<string> {"Costa Rica", "Estados Unidos", "España", "Argentina", "Brasil"};

        private static List<string> roles = new List<string> {"Turista", "Viajero de negocios", "Estudiante", "Investigador", "Otro"};

        public ActionResult Index()
        {
            var destinos = GetDestinos();

            // Ordenar por Clasificación descendente
            var topDestinos = destinos
                .OrderByDescending(d => d.Clasificacion)
                .Take(20)
                .ToList();

            // Calcular posición y diferencia
            for (int i = 0; i < topDestinos.Count; i++)
            {
                topDestinos[i].Posicion = i + 1;

                if (i == 0)
                    topDestinos[i].DiferenciaPorcentual = "0%";
                else
                {
                    double diferencia = Math.Round(topDestinos[i - 1].Clasificacion - topDestinos[i].Clasificacion, 2);
                    string signo = diferencia >= 0 ? "+" : "-";
                    topDestinos[i].DiferenciaPorcentual = $"{signo}{Math.Abs(diferencia)}%";
                }
            }
            return View(topDestinos);
        }

        private List<DestinoCalificacion> GetDestinos()
        {
            if (Session["ListaDestinos"] is List<DestinoCalificacion> listaSesion)
                return listaSesion;

            List<DestinoCalificacion> lista;

            var json = System.IO.File.ReadAllText(archivoDestinos);
            lista = JsonConvert.DeserializeObject<List<DestinoCalificacion>>(json)
                    ?? new List<DestinoCalificacion>();

            Session["ListaDestinos"] = lista;

            return lista;
        }

        public ActionResult Agregar()
        {
            return RedirectToAction("Crear");
        }

        public ActionResult Crear()
        {
            ViewBag.Paises = new SelectList(paises);
            ViewBag.Roles = new SelectList(roles);

            var destinos = Session["ListaDestinos"] as List<DestinoCalificacion>;
            if (destinos != null)
            {
                var nombresDestinos = destinos.Select(d => d.Nombre).OrderBy(n => n).ToList();
                ViewBag.Destinos = new SelectList(nombresDestinos);
            }
            else
            {
                List<string> lista = new List<string>();
                ViewBag.Destinos = new SelectList(lista);
            }

            return View(new Encuesta());
        }


        [HttpPost]
        public JsonResult Crear(Encuesta model)
        {
            var destinos = Session["ListaDestinos"] as List<DestinoCalificacion>;

            var destino1 = destinos.FirstOrDefault(d => d.Nombre == model.DestinoPrimario);
            if (destino1 != null) destino1.Clasificacion += 1;

            var destino2 = destinos.FirstOrDefault(d => d.Nombre == model.DestinoSecundario);
            if (destino2 != null) destino2.Clasificacion += 0.5;

            double total = destinos.Sum(d => d.Clasificacion);
            foreach (var d in destinos)
            {
                d.Clasificacion = total == 0 ? 0 : Math.Round((d.Clasificacion / total) * 100, 2);
            }

            System.IO.File.WriteAllText(archivoDestinos, JsonConvert.SerializeObject(destinos, Formatting.Indented));

            return Json(new { success = true });
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}