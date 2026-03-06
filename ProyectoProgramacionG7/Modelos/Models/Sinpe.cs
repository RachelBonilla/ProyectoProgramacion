using System;
using System.ComponentModel.DataAnnotations;

namespace Modelos.Models
{
    public class Sinpe
    {
        [Key]
        public int IdSinpe { get; set; }

        [Required(ErrorMessage = "El teléfono origen es obligatorio")]
        [StringLength(10)]
        [Display(Name = "Teléfono Origen")]
        public string TelefonoOrigen { get; set; }

        [Required(ErrorMessage = "El nombre origen es obligatorio")]
        [StringLength(200)]
        [Display(Name = "Nombre Origen")]
        public string NombreOrigen { get; set; }

        [Required(ErrorMessage = "El teléfono destinatario es obligatorio")]
        [StringLength(10)]
        [Display(Name = "Teléfono Destinatario")]
        public string TelefonoDestinatario { get; set; }

        [Required(ErrorMessage = "El nombre destinatario es obligatorio")]
        [StringLength(200)]
        [Display(Name = "Nombre Destinatario")]
        public string NombreDestinatario { get; set; }

        [Required(ErrorMessage = "El monto es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        [Display(Name = "Monto")]
        public decimal Monto { get; set; }

        [Required]
        [Display(Name = "Fecha de Registro")]
        public DateTime FechaDeRegistro { get; set; }

        [StringLength(50)]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Required]
        [Display(Name = "Estado")]
        public bool Estado { get; set; } 
    }
}