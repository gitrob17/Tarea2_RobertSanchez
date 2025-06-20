using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tarea2_ProgramacionAvanzada.Models
{
    public class Encuesta
    {
        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellido { get; set; }

        [Required]
        [Display(Name = "País")]
        public string Pais { get; set; }

        [Required]
        public string Rol { get; set; }

        [Required]
        [Display(Name = "Destino Primario")]
        public string DestinoPrimario { get; set; }

        [Required]
        [Display(Name = "Destino Secundario")]
        public string DestinoSecundario { get; set; }
    }
}