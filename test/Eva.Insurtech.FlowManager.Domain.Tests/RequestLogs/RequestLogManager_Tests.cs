using Eva.Insurtech.FlowManagers.RequestLogs;
using Eva.Insurtech.FlowManagers.RequestLogs.Exceptions;
using Eva.Insurtech.FlowManagers.RequestLogs.Requests.Inputs;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Eva.Insurtech.FlowManagers.RequestLogs
{
    [ExcludeFromCodeCoverage]
    public class RequestLogManager_Tests : FlowManagerDomainTestBase
    {
        private readonly RequestLogManager _requestLogManager;

        public RequestLogManager_Tests()
        {
            _requestLogManager = GetRequiredService<RequestLogManager>();
        }

        [Fact]
        public async Task GetRequestLog_ById_ReturnRequestLog()
        {
            var requestLog = await _requestLogManager.GetByService("service-test");

            var result = await _requestLogManager.GetById(requestLog.Id);

            Assert.Equal(requestLog.Service, result.Service);
        }

        [Fact]
        public async Task GetRequestLog_ByNonExistsId_ReturnNull()
        {
            var result = await _requestLogManager.GetById(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task GetRequestLog_ByServiceToday_ReturnRequestLog()
        {
            var requestLog = GetCreate("today");

            var created = await _requestLogManager.InsertAsync(requestLog);

            var result = await _requestLogManager.GetByServiceToday(created.Service);

            Assert.Equal(requestLog.Service, result.Service);
        }

        [Fact]
        public async Task GetRequestLog_ByNonExistsServiceToday_ReturnNull()
        {
            var result = await _requestLogManager.GetByServiceToday("non-exists");

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateRequestLog_WithNullData_ReturnNullException()
        {
            RequestLog requestLog = null;
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _requestLogManager.InsertAsync(requestLog);
            });
        }

        [Fact]
        public async Task CreateRequestLog_WithData_ReturnRequestLog()
        {
            var requestLog = GetCreate();

            var result = await _requestLogManager.InsertAsync(requestLog);

            Assert.NotNull(result);
            Assert.Equal(result.Service, requestLog.Service);
            Assert.Equal(result.Iterations, requestLog.Iterations);
        }

        [Fact]
        public async Task CreateRequestLogStep_WithData_ReturnRequestLog()
        {
            var requestLog = await _requestLogManager.GetByService("service-test");

            var input = GetRequest(requestLog.Id);

            var result = await _requestLogManager.AddRequestLogRequest(input);

            Assert.NotNull(result.Requests);
            Assert.Equal(result.Requests.FirstOrDefault().TransactionReference, input.TransactionReference);
            Assert.Equal(result.Requests.FirstOrDefault().Body, input.Body);
            Assert.Equal(result.Requests.FirstOrDefault().Service, input.Service);
            Assert.True(result.Requests.FirstOrDefault().Observations.IsNullOrEmpty());
        }

        [Fact]
        public async Task CreateRequestLogStep_WithNonExistsRequestLog_ReturnException()
        {
            var input = GetRequest(Guid.NewGuid());

            await Assert.ThrowsAsync<RequestLogNotFoundException>(async () =>
            {
                await _requestLogManager.AddRequestLogRequest(input);
            });
        }



        #region Private Methods


        private static RequestLog GetCreate(string suffix = "")
        {
            var requestLog = new RequestLog(
                "service-test" + suffix,
                0,
                ""
            );
            return requestLog;
        }

        private static RequestInput GetRequest(Guid requestLogId)
        {
            var requestLogStep = new RequestInput()
            {
                RequestLogId = requestLogId,
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
