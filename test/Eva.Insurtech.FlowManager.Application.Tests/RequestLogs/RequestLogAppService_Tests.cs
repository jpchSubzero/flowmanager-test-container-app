using Eva.Insurtech.FlowManagers.PreTrackings.PreTrackingSteps;
using Eva.Insurtech.FlowManagers.RequestLogs.Dtos;
using Eva.Insurtech.FlowManagers.RequestLogs.Exceptions;
using Eva.Insurtech.FlowManagers.RequestLogs.Requests.Dtos;
using Eva.Insurtech.FlowManagers.RequestLogs.Requests.Inputs;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Eva.Insurtech.FlowManagers.RequestLogs
{
    [ExcludeFromCodeCoverage]

    public class RequestLogAppService_Tests : FlowManagerApplicationTestBase
    {
        private readonly IRequestLogAppService _requestLogAppService;

        public RequestLogAppService_Tests()
        {
            _requestLogAppService = GetRequiredService<IRequestLogAppService>();
        }

        [Fact]
        public async Task GetRequestLog_ById_ReturnRequestLog()
        {
            var requestLog = await _requestLogAppService.GetByService("service-test");

            var result = await _requestLogAppService.GetById(requestLog.Result.Id);

            Assert.Equal(requestLog.Result.Service, result.Result.Service);
        }

        [Fact]
        public async Task GetRequestLog_ByNonExistsId_ReturnNull()
        {
            var result = await _requestLogAppService.GetById(Guid.NewGuid());

            Assert.NotNull(result);
            Assert.Null(result.Result);
        }

        [Fact]
        public async Task CreateRequestLog_WithNullData_ReturnNullException()
        {
            try
            {
                await _requestLogAppService.InsertAsync(null);
            }
            catch (Exception ex)
            {
                Assert.Equal("Method arguments are not valid! See ValidationErrors for details.", ex.Message);
            }
        }

        [Fact]
        public async Task CreateRequestLog_WithData_ReturnRequestLog()
        {
            var requestLog = GetCreate();

            var result = await _requestLogAppService.InsertAsync(requestLog);

            Assert.NotNull(result);
            Assert.Equal(result.Result.Service, requestLog.Service);
        }

        [Fact]
        public async Task CreateRequestLogStep_WithData_ReturnRequestLog()
        {
            var requestLog = await _requestLogAppService.GetByService("service-test");

            var input = GetRequest();

            var result = await _requestLogAppService.AddRequestLogRequest(input, requestLog.Result.Id);

            Assert.NotNull(result.Result.Requests);
            Assert.Equal(result.Result.Requests.FirstOrDefault().TransactionReference, input.TransactionReference);
            Assert.Equal(result.Result.Requests.FirstOrDefault().Body, input.Body);
            Assert.Equal(result.Result.Requests.FirstOrDefault().Service, input.Service);
            Assert.True(result.Result.Requests.FirstOrDefault().Observations.IsNullOrEmpty());
        }

        [Fact]
        public async Task CreateRequestLogStep_WithNonExistsRequestLog_ReturnException()
        {
            var input = GetRequest();

            var result = await _requestLogAppService.AddRequestLogRequest(input, Guid.NewGuid());

            Assert.NotNull(result.Error);
            Assert.Equal(result.Error.Code, ErrorConsts.ERROR_NOT_FOUND_BY_ID);
        }



        #region Private Methods


        private static CreateRequestLogDto GetCreate(string suffix = "")
        {
            var requestLog = new CreateRequestLogDto() {
                Service = "service-test" + suffix,
                Observations = ""
            };
            return requestLog;
        }

        private static CreateRequestDto GetRequest()
        {
            var requestLogStep = new CreateRequestDto() 
            {
                TransactionReference = "transaction-reference-test",
                Service = "service-test",
                Body = "body-test",
                Observations = ""
            };
            return requestLogStep;
        }


        #endregion    
    }
}
