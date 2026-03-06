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
        public string TablaDeEvento { get; set; }

        [Required]
        [StringLength(20)]
        public string TipoDeEvento { get; set; }

        [Required]
        public DateTime FechaDeEvento { get; set; }

        public string DescripcionDeEvento { get; set; }

        public string StackTrace { get; set; }

        public string DatosAnteriores { get; set; }

        public string DatosPosteriores { get; set; }
    }
}