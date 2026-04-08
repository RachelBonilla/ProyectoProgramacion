using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Modelos.Models
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [ForeignKey("Comercio")]
        public int? IdComercio { get; set; }
        public Comercio Comercio { get; set; }

        public Guid? IdNetUser { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombres { get; set; }

        [Required]
        [StringLength(100)]
        public string PrimerApellido { get; set; }

        [Required]
        [StringLength(100)]
        public string SegundoApellido { get; set; }

        [Required]
        [StringLength(10)]
        public string Identificacion { get; set; }

        [Required]
        [StringLength(200)]
        public string CorreoElectronico { get; set; }

        [Required]
        public DateTime FechaDeRegistro { get; set; }

        public DateTime? FechaDeModificacion { get; set; }

        [Required]
        public bool Estado { get; set; }
    }
}