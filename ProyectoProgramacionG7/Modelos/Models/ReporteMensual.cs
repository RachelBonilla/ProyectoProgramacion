using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Modelos.Models
{
    public class ReporteMensual
    {
        [Key]
        public int IdReporte { get; set; }

        [Required]
        [ForeignKey("Comercio")]
        public int IdComercio { get; set; }

        public Comercio Comercio { get; set; }

        [Required]
        public int CantidadDeCajas { get; set; }

        [Required]
        public decimal MontoTotalRecaudado { get; set; }

        [Required]
        public int CantidadDeSINPES { get; set; }

        [Required]
        public decimal MontoTotalComision { get; set; }

        [Required]
        public DateTime FechaDelReporte { get; set; }
    }
}