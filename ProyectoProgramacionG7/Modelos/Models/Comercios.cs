using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Modelos.Models
{

    public class Comercio
    {
        [Key]
        public int IdComercio { get; set; } // PK, Identity

        [Required(ErrorMessage = "La identificación es obligatoria")]
        [StringLength(30)]
        public string Identificacion { get; set; }

        [Required(ErrorMessage = "El tipo de identificación es obligatorio")]
        [Range(1, 2, ErrorMessage = "Tipo de identificación inválido")]
        public int TipoIdentificacion { get; set; }
        /*
         1 – Física
         2 – Jurídica
        */

        [Required(ErrorMessage = "El nombre del comercio es obligatorio")]
        [StringLength(200)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El tipo de comercio es obligatorio")]
        [Range(1, 4, ErrorMessage = "Tipo de comercio inválido")]
        public int TipoDeComercio { get; set; }
        /*
         1 – Restaurantes
         2 – Supermercados
         3 – Ferreterías
         4 – Otros
        */

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [StringLength(20)]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [StringLength(200)]
        [EmailAddress(ErrorMessage = "Correo electrónico inválido")]
        public string CorreoElectronico { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria")]
        [StringLength(500)]
        public string Direccion { get; set; }

        [Required]
        public DateTime FechaDeRegistro { get; set; }

        public DateTime? FechaDeModificacion { get; set; }

        [Required]
        public bool Estado { get; set; } // 1 Activo | 0 Inactivo
    }
}
