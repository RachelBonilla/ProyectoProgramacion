using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Modelos.Models
{
    public class Caja
    {
        [Key]
        public int IdCaja { get; set; }

        [ForeignKey("Comercio")]
        public int IdComercio { get; set; }

        public Comercio? Comercio { get; set; }

        public string? Nombre { get; set; }

        public string? Descripcion { get; set; }

        public string? TelefonoSINPE { get; set; }

        public DateTime? FechaDeRegistro { get; set; }

        public DateTime? FechaDeModificacion { get; set; }

        public bool Estado { get; set; }
    }
}