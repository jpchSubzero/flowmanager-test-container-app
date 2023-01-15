using Eva.Framework.Utility.Option.Contracts;
using Eva.Framework.Utility.Response.Models;
using Eva.Insurtech.FlowManagers.ApiServices;
using Eva.Insurtech.FlowManagers.Products.Dtos;
using Eva.Insurtech.FlowManagers.Products.Exceptions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace Eva.Insurtech.FlowManagers.Products
{
    public class ProductManager : DomainService
    {
        private readonly IProductRepository _productRepository;
        private readonly ApiServiceManager _apiServiceManager;
        private readonly IAppConfigurationManager _appConfigurationManager;

        public ProductManager(
            IProductRepository productRepository,
            ApiServiceManager apiServiceManager,
            IAppConfigurationManager appConfigurationManager

        )
        {
            _productRepository = productRepository;
            _apiServiceManager = apiServiceManager;
            _appConfigurationManager = appConfigurationManager;

    }

        public async Task<Product> InsertAsync(Product product, bool autoSave = true)
        {
            return await _productRepository.InsertAsync(product, autoSave);
        }

        public async Task<Product> UpdateAsync(Product product, bool autoSave = true)
        {
            return await _productRepository.UpdateAsync(product, autoSave);
        }

        public async Task<Product> GetByExternalIdAsync(Guid id)
        {
            return await _productRepository.GetByExternalIdAsync(id);
        }

        public async Task<Product> GetByIdAsync(Guid id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<Product> GetByCodeAsync(string code)
        {
            return await _productRepository.GetByCodeAsync(code);
        }

        public async Task<ICollection<Product>> GetAllAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task UpdateFromProductContext()
        {
            var products = await GetProducts();
            foreach (var product in products)
            {
                await InsertAsync(new Product(
                    product.Id,
                    product.Code,
                    product.ExternalCode,
                    product.Name
                ));
            }
        }

        public async Task ValidateIfChannelExistInProduct(string channelCode)
        {
            if (channelCode == null)
            {
                throw new ProductNotFoundException(ProductErrorCodes.GetErrorNullCode(nameof(channelCode)));
            }
            if (await GetByChannelCodeFromProducts(channelCode) == null)
            {
                throw new ChannelNotFoundException(ProductErrorCodes.GetErrorNotFoundChannelByCode(channelCode));
            }
        }

        public async Task ValidateIfExistInProduct(Guid productId)
        {
            if (await _productRepository.GetByExternalIdAsync(productId) == null) { 

                var product = await GetByCodeFromProduct(productId);
                var products = await _productRepository.GetAllAsync();
                if (!products.Any(x => product.Id.Equals(x.ProductId)))
                {
                    await InsertAsync(new Product(
                        product.Id,
                        product.Code,
                        product.ExternalCode,
                        product.Name
                    ));
                } else { 
                    throw new ProductNotFoundException(ProductErrorCodes.GetErrorNotFoundById());
                }
            }
        }


        #region Private Methods


        private async Task<ProductDto> GetByCodeFromProduct(Guid productId)
        {
            string requestUrl = GetUrlRequestOfProducts(productId);
            try
            {
                var response = await QueryAsync<ProductDto>(requestUrl);
                return response.Result;
            }
            catch (Exception)
            {
                throw new ProductNotFoundException(ProductErrorCodes.GetErrorNotFoundById());
            }
        }

        private async Task<ICollection<ProductDto>> GetProducts()
        {
            string requestUrl = GetUrlRequestOfAllProducts();
            try
            {
                var response = await QueryAsync<ICollection<ProductDto>>(requestUrl);
                return response.Result;
            }
            catch (Exception)
            {
                throw new ProductNotFoundException(ProductErrorCodes.GetErrorNotFoundById());
            }
        }

        private async Task<ProductDto> GetByChannelCodeFromProducts(string code)
        {
            string requestUrl = GetUrlRequestOfChannels(code);
            try
            {
                var response = await QueryAsync<ProductDto>(requestUrl);
                return response.Result;
            }
            catch (GeneralException)
            {
                throw new ChannelNotFoundException(ProductErrorCodes.GetErrorNotFoundChannelByCode());
            }
        }


        private async Task<Response<T>> QueryAsync<T>(string requestUrl)
        {
            Dictionary<string, string> headers = new();
            RequestDataDto requestData = new()
            {
                UrlBase = requestUrl,
                Headers = headers
            };
            var result = await _apiServiceManager.QueryAsync(requestData);
            var response = result.ToObject<Response<T>>();

            if (!response.Success)
                throw new ProductNotFoundException(ProductErrorCodes.GetErrorNotFoundById());
            return response;
        }

        private string GetUrlRequestOfProducts(Guid productId)
        {
            string controller = GetController(ProductConsts.Product);
            var method = _appConfigurationManager.GetVariableByTypeName(ProductConsts.Products, ProductConsts.GetProducts);

            return $"{controller}{method}{productId}";
        }

        private string GetUrlRequestOfAllProducts()
        {
            string controller = GetController(ProductConsts.Product);
            var method = _appConfigurationManager.GetVariableByTypeName(ProductConsts.Products, ProductConsts.GetProducts);

            return $"{controller}{method}";
        }

        private string GetUrlRequestOfChannels(string code)
        {
            string controller = GetController(ProductConsts.Product);
            var method = _appConfigurationManager.GetVariableByTypeName(ProductConsts.Products, ProductConsts.GetChannelByCode);
            return string.Format($"{controller}{method}", code);
        }


        private string GetUrlContext()
        {
            return _appConfigurationManager.GetServiceUrlBySystemControllerCapacity(ProductConsts.ExternalApi, ProductConsts.ExternalApis, ProductConsts.ProductBaseUrl);
        }

        private string GetController(string controller)
        {
            return $"{GetUrlContext()}{_appConfigurationManager.GetVariableByTypeName(ProductConsts.Catalogs, controller)}";
        }

        #endregion
    }
}
