using System;

namespace Eva.Insurtech.FlowManagers.Products.Dtos
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string ExternalCode { get; set; }
        public string Name { get; set; }
    }
}

