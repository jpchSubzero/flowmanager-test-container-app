using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eva.Insurtech.FlowManagers.MessengerApiServices
{
    public  class MessengerEvaDto
    {
        [Required(ErrorMessage = "Campo EmailCustomer es obligatorio")]
        public List<EmailCustomer> EmailsCustomer { get; set; }
        [Required(ErrorMessage = "Campo CodeTemplate es obligatorio")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Campo CodeTemplate debe tener una longuitud entre 4 y 50")]
        public string CodeTemplate { get; set; }
        public List<TextData> TextData { get; set; }
        public List<AttachedStream> AttachedFiles { get; set; }
    }
    public class TextData
    {
        public string Text { get; set; }
    }
    public class AttachedStream
    {
        [Required(ErrorMessage = "Campo Stream es obligatorio.")]
        public byte[] Stream { get; set; }
        [Required(ErrorMessage = "Campo AttachedStreamName es obligatorio.")]
        public string AttachedStreamName { get; set; }
    }
}
