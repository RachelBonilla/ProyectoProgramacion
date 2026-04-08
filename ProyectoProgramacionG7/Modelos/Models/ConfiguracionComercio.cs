using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Modelos.Models
{
    public class ConfiguracionComercio
    {
        [Key]
        public int IdConfiguracion { get; set; }

        [Required]
        [ForeignKey("Comercio")]
        public int IdComercio { get; set; }

        public Comercio Comercio { get; set; }

        [Required]
        public int TipoConfiguracion { get; set; }
        // 1 Plataforma, 2 Externa, 3 Ambas

        [Required]
        public int Comision { get; set; }

        [Required]
        public DateTime FechaDeRegistro { get; set; }

        public DateTime? FechaDeModificacion { get; set; }

        [Required]
        public bool Estado { get; set; }
    }
}