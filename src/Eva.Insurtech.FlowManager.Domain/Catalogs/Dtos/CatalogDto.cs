using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace Eva.Insurtech.FlowManagers.Catalogs.Dtos
{
    public class CatalogDto
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsExtending { get; set; }

        public bool IsActived { get; set; }

        public object CatalogParentCode { get; set; }

        public Guid CountryId { get; set; }

        public ICollection<CatalogItem> CatalogItems { get; set; }

        public ICollection<object> CustomCatalogs { get; set; }

        public Guid Id { get; set; }
    }

    public partial class CatalogItem
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public long Order { get; set; }

        public bool IsComplexObject { get; set; }

        public string AssociatedValue { get; set; }

        public bool IsActived { get; set; }

        public Guid CatalogId { get; set; }

        public Guid Id { get; set; }
    }
}

