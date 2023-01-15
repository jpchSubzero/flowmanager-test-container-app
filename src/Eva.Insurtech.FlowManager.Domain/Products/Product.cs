using Eva.Insurtech.FlowManagers.Products.Inputs;
using JetBrains.Annotations;
using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;

namespace Eva.Insurtech.FlowManagers.Products
{
    public class Product : AggregateRoot<Guid>
    {
        /// <summary>
        /// Id de catálogo que viene del contexto de Configuration
        /// </summary>
        public virtual Guid ProductId { get; private set; }
        /// <summary>
        /// Codigo de producto
        /// </summary>
        public virtual string Code { get; private set; }
        /// <summary>
        /// Codigo de producto en caso de tener un código externo
        /// </summary>
        public virtual string ExternalCode { get; private set; }
        /// <summary>
        /// Nombre de producto
        /// </summary>
        public virtual string Name { get; private set; }

        protected Product()
        {
        }

        public Product(
            [NotNull][MaxLength(ProductConsts.NameMaxLength)] Guid productId,
            [NotNull][MaxLength(ProductConsts.NameMaxLength)] string code,
            [NotNull][MaxLength(ProductConsts.NameMaxLength)] string externalCode,
            [NotNull][MaxLength(ProductConsts.NameMaxLength)] string name
        )
        {
            ProductId = productId;
            Code = code;
            ExternalCode = externalCode;
            Name = name;
        }
       
    }
}
