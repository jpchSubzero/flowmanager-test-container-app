using Eva.Insurtech.FlowManagers.Catalogs.Inputs;
using JetBrains.Annotations;
using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;

namespace Eva.Insurtech.FlowManagers.Catalogs
{
    public class Catalog : AggregateRoot<Guid>
    {
        /// <summary>
        /// Id de catálogo que viene del contexto de Configuration
        /// </summary>
        public virtual Guid CatalogId { get; private set; }
        /// <summary>
        /// Codigo de catalogo
        /// </summary>
        public virtual string Code { get; private set; }
        /// <summary>
        /// Nombre de catalogo
        /// </summary>
        public virtual string Name { get; private set; }

        protected Catalog()
        {
        }

        public Catalog(
            [NotNull][MaxLength(CatalogConsts.NameMaxLength)] Guid catalogId,
            [NotNull][MaxLength(CatalogConsts.NameMaxLength)] string code,
            [NotNull][MaxLength(CatalogConsts.NameMaxLength)] string name
        )
        {
            CatalogId = catalogId;
            Code = code;
            Name = name;
        }

        public void SetCatalogId(Guid id)
        {
            CatalogId = id;
        }

        public void SetCode(string code)
        {
            Code = code;
        }

        public void SetName(string name)
        {
            Name = name;
        }
        public void SetExtraProperties(ExtraPropertyDictionary extraProperties) 
        { 
            ExtraProperties = extraProperties;
        }
    }
}
