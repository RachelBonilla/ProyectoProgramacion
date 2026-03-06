using System;
using System.ComponentModel.DataAnnotations;

namespace Modelos.Models
{
    public class BitacoraEvento
    {
        [Key]
        public int IdEvento { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Tabla")]
        public string TablaDeEvento { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Tipo de Evento")]
        public string TipoDeEvento { get; set; }

        [Required]
        [Display(Name = "Fecha")]
        public DateTime FechaDeEvento { get; set; }

        [Required]
        [Display(Name = "Descripción")]
        public string DescripcionDeEvento { get; set; }

        [Required]
        [Display(Name = "StackTrace")]
        public string StackTrace { get; set; }

        [Display(Name = "Datos Anteriores")]
        public string DatosAnteriores { get; set; }

        [Display(Name = "Datos Posteriores")]
        public string DatosPosteriores { get; set; }
    }
}