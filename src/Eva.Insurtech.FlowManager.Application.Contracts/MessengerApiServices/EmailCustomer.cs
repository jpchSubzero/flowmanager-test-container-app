using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eva.Insurtech.FlowManagers.MessengerApiServices
{
    public class EmailCustomer
    {
        [DataType(DataType.EmailAddress)]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "Campo EmailCustomer debe tener una longuitud entre 4 y 100")]
        [Required(ErrorMessage = "Campo EmailCustomer es obligatorio")]
        public string Email { get; set; }
        public string Name { get; set; }
    }
}
