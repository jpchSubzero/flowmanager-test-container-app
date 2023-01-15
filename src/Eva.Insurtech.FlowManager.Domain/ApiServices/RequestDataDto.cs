using System.Collections.Generic;

namespace Eva.Insurtech.FlowManagers.ApiServices
{
    public class RequestDataDto
    {
        public string UrlBase { get; set; }
        public string Body { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public CredentialsDto Credentials { get; set; }
        public Dictionary<string, string> Headers { get; set; }
    }
}