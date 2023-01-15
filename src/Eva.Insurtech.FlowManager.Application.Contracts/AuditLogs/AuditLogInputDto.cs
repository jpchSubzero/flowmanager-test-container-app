using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Eva.Insurtech.AuditLogEva.AuditLog
{
    public class AuditLogInputDto 
    {
        [Required(ErrorMessage = "Campo TrackingId es obligatorio")]
        public Guid TrackingId { get; set; }
        [StringLength(255, MinimumLength = 4,ErrorMessage ="Longuitud del campo action debe ser mínimo 4 y máximo 255 caracteres")]
        [Required(ErrorMessage = "Campo Action es obligatorio")]
        public string Action { get; set; }  

    }
}
