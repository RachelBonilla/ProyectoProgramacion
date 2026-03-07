using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Modelos.Models
{
    public class Caja
    {
        [Key]
        public int IdCaja { get; set; }  // ID no nullable

        [ForeignKey("Comercio")]
        public int? IdComercio { get; set; } // nullable

        public Comercio? Comercio { get; set; } // nullable

        [StringLength(200)]
        public string? Nombre { get; set; } // nullable, se protege en la consulta

        [StringLength(50)]
        public string? Codigo { get; set; } // nullable, se protege en la consulta

        [StringLength(10)]
        public string? Telefono { get; set; }  // nullable

        [Required]
        public bool Estado { get; set; }

        [Required]
        public DateTime FechaDeRegistro { get; set; }

        public DateTime? FechaDeModificacion { get; set; }

        [StringLength(100)]
        public string? UsuarioRegistro { get; set; }

        [StringLength(100)]
        public string? UsuarioModificacion { get; set; }
    }
}