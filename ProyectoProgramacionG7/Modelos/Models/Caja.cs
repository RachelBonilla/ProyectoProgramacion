using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Modelos.Models
{
    public class Caja
    {
        [Key]
        public int?  IdCaja { get; set; }

        [Required(ErrorMessage = "El comercio es obligatorio")]
        [ForeignKey("Comercio")]
        public int?  IdComercio { get; set; }

        public Comercio Comercio { get; set; }

        [Required(ErrorMessage = "El nombre de la caja es obligatorio")]
        [StringLength(200)]
        public string? Nombre { get; set; }

        [Required(ErrorMessage = "El código de la caja es obligatorio")]
        [StringLength(50)]
        public string? Codigo { get; set; }

        [Required]
        public bool?  Estado { get; set; }

        [Required]
        public DateTime?  FechaDeRegistro { get; set; }

        public DateTime?  FechaDeModificacion { get; set; }

        [StringLength(100)]
        public string? UsuarioRegistro { get; set; }

        [StringLength(100)]
        public string? UsuarioModificacion { get; set; }
    }
}