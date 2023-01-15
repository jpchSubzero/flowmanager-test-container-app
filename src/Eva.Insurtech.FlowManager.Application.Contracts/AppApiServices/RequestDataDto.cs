using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eva.Insurtech.AppApiServices
{
    public class RequestDataDto
    {
        public string UrlBase { get; set; }
        public string Body { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public CredentialsDto Credentials { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public string InLineParameters { get; set; }
    }
}
