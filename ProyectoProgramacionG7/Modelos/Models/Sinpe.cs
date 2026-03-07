using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Modelos.Models
{
    public class Sinpe
    {
        [Key]
        public int IdSinpe { get; set; }

        [Required(ErrorMessage = "El Id de la caja es obligatorio")]
        [ForeignKey("Caja")]
        public int CajaId { get; set; }

        public Caja Caja { get; set; }

        [Required(ErrorMessage = "El teléfono de origen es obligatorio")]
        [StringLength(10)]
        public string TelefonoOrigen { get; set; }

        [Required(ErrorMessage = "El nombre de origen es obligatorio")]
        [StringLength(200)]
        public string NombreOrigen { get; set; }

        [Required(ErrorMessage = "El nombre del destinatario es obligatorio")]
        [StringLength(200)]
        public string NombreDestinatario { get; set; }

        [Required(ErrorMessage = "El teléfono del destinatario es obligatorio")]
        [StringLength(10)]
        public string TelefonoDestinatario { get; set; }

        [Required(ErrorMessage = "El monto es obligatorio")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Monto { get; set; }

        [StringLength(50)]
        public string Descripcion { get; set; }

        [Required]
        public DateTime FechaDeRegistro { get; set; }

        [Required]
        public bool Estado { get; set; } // 1=Sincronizado, 0=No sincronizado
    }
}