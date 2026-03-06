using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Modelos.Models
{
    public class Caja
    {
        [Key]
        public int IdCaja { get; set; }

        [Required(ErrorMessage = "El comercio es obligatorio")]
        [ForeignKey("Comercio")]
        public int IdComercio { get; set; }

        public Comercio Comercio { get; set; }

        [Required(ErrorMessage = "El nombre de la caja es obligatorio")]
        [StringLength(100)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(150)]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El teléfono SINPE es obligatorio")]
        [StringLength(10)]
        [Display(Name = "Teléfono SINPE")]
        public string TelefonoSINPE { get; set; }

        [Required]
        [Display(Name = "Estado")]
        public bool Estado { get; set; }

        [Required]
        [Display(Name = "Fecha de Registro")]
        public DateTime FechaDeRegistro { get; set; }

        [Display(Name = "Fecha de Modificación")]
        public DateTime? FechaDeModificacion { get; set; }
    }
}