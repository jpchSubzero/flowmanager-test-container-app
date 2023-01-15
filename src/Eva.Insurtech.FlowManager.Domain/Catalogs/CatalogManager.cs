using Eva.Framework.Utility.Option.Contracts;
using Eva.Framework.Utility.Response.Models;
using Eva.Insurtech.FlowManagers.ApiServices;
using Eva.Insurtech.FlowManagers.Catalogs.Dtos;
using Eva.Insurtech.FlowManagers.Catalogs.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace Eva.Insurtech.FlowManagers.Catalogs
{
    public class CatalogManager : DomainService
    {
        private readonly ICatalogRepository _catalogRepository;
        private readonly ApiServiceManager _apiServiceManager;
        private readonly IAppConfigurationManager _appConfigurationManager;


        public CatalogManager(
            ICatalogRepository catalogRepository,
            ApiServiceManager apiServiceManager,
            IAppConfigurationManager appConfigurationManager
        )
        {
            _catalogRepository = catalogRepository;
            _apiServiceManager = apiServiceManager;
            _appConfigurationManager = appConfigurationManager;
        }

        public async Task<Catalog> InsertAsync(Catalog catalog, bool autoSave = true)
        {
            return await _catalogRepository.InsertAsync(catalog, autoSave);
        }

        public async Task<Catalog> UpdateAsync(Catalog catalog, bool autoSave = true)
        {
            return await _catalogRepository.UpdateAsync(catalog, autoSave);
        }

        public async Task<Catalog> GetByExternalIdAsync(Guid id)
        {
            return await _catalogRepository.GetByExternalIdAsync(id);
        }

        public async Task<Catalog> GetByCodeAsync(string code, string catalogCode)
        {
            var catalog = await _catalogRepository.GetByCodeAsync(code);
            if (catalog == null)
            {
                await GetFromConfigurationContextByCatalogCode(catalogCode);
                catalog = await _catalogRepository.GetByCodeAsync(code);
            }
            return catalog;
        }

        public async Task<ICollection<Catalog>> GetAllAsync()
        {
            return await _catalogRepository.GetAllAsync();
        }

        public Task UpdateFromCatalogContext(Catalog catalog)
        {
            throw new NotImplementedException();
        }

        public async Task ValidateIfExistInCatalog(string code, Guid catalogId)
        {
            if (await _catalogRepository.GetByExternalIdAsync(catalogId) == null)
            {
                await FindOnConfigurationContextByCatalogId(code, catalogId);
            }
        }

        public async Task<CatalogItem> GetByCodeAndItemCode(string code, string itemCode)
        {
            string requestUrl = GetUrlRequestOfCatalogsAndItemCode(code, itemCode);
            try
            {
                var response = await QueryAsync<CatalogItem>(requestUrl);
                return response.Result;
            }
            catch (GeneralException ex)
            {
                throw new ExternalServiceException(new Framework.Utility.Response.Models.Error(ex));
            }
        }



        #region Private Methods

        private async Task FindOnConfigurationContextByCatalogId(string code, Guid catalogId)
        {
            var catalog = await GetByCodeFromCatalog(code);
            var catalogsItems = catalog.CatalogItems;

            if (catalogsItems != null && catalogsItems.Any(x => catalogId.Equals(x.Id)))
            {
                var catalogs = await _catalogRepository.GetAllAsync();
                foreach (var catalogsItem in catalogsItems)
                {
                    if (!catalogs.Any(x => catalogsItem.Id.Equals(x.CatalogId)))
                    {
                        try
                        {
                            await InsertAsync(new Catalog(
                                catalogsItem.Id,
                                catalogsItem.Code,
                                catalogsItem.Name
                            ));

                        }
                        catch (Exception)
                        {
                            throw new CatalogNotFoundException(CatalogErrorCodes.GetErrorGeneral());
                        }
                    }
                }
            }
            else
            {
                throw new CatalogNotFoundException(CatalogErrorCodes.GetErrorNotFoundById());
            }
        }

        public async Task GetFromConfigurationContextByCatalogCode(string code)
        {
            var catalog = await GetByCodeFromCatalog(code);
            var catalogsItems = catalog.CatalogItems;

            if (catalogsItems != null)
            {
                foreach (var catalogsItem in catalogsItems)
                {
                    try
                    {
                        var catalogContext = await _catalogRepository.GetByCodeAsync(catalogsItem.Code);
                        if (catalogContext == null)
                        {
                            await InsertAsync(new Catalog(
                                catalogsItem.Id,
                                catalogsItem.Code,
                                catalogsItem.Name
                            ));
                        } else
                        {
                            catalogContext.SetCatalogId(catalogsItem.Id);
                            catalogContext.SetCode(catalogsItem.Code);
                            catalogContext.SetName(catalogsItem.Name);
                            await UpdateAsync(catalogContext);
                        }
                    }
                    catch (Exception)
                    {
                        throw new CatalogNotFoundException(CatalogErrorCodes.GetErrorGeneral());
                    }
                }
            }
            else
            {
                throw new CatalogNotFoundException(CatalogErrorCodes.GetErrorNotFoundById());
            }
        }

        private async Task<CatalogDto> GetByCodeFromCatalog(string code)
        {
            string requestUrl = GetUrlRequestOfCatalogs(code);
            try
            {
                var response = await QueryAsync<CatalogDto>(requestUrl);
                return response.Result;
            }
            catch (GeneralException ex)
            {
                throw new ExternalServiceException(new Framework.Utility.Response.Models.Error(ex));
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
                throw new CatalogNotFoundException(CatalogErrorCodes.GetErrorNotFoundById());
            return response;
        }

        private string GetUrlRequestOfCatalogs(string code)
        {
            string controller = GetController(CatalogConsts.Catalog);
            var method = _appConfigurationManager.GetVariableByTypeName(CatalogConsts.Catalogs, CatalogConsts.GetCatalogByCode);
            return $"{controller}{method}?code={code}&includeItems=true&isActived=true";
        }

        private string GetUrlRequestOfCatalogsAndItemCode(string code, string itemCode)
        {
            string controller = GetController(CatalogConsts.Catalog);
            var method = _appConfigurationManager.GetVariableByTypeName(CatalogConsts.Catalogs, CatalogConsts.GetCatalogByCodeAndItemCode);
            return $"{controller}{string.Format(method, code, itemCode)}";
        }


        private string GetUrlContext()
        {
            return _appConfigurationManager.GetServiceUrlBySystemControllerCapacity(CatalogConsts.ExternalApi, CatalogConsts.ExternalApis, CatalogConsts.ConfigurationBaseUrl);
        }

        private string GetController(string controller)
        {
            return $"{GetUrlContext()}{_appConfigurationManager.GetVariableByTypeName(CatalogConsts.Catalogs, controller)}";
        }

        #endregion
    }
}
