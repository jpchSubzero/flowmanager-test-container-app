using Eva.Insurtech.FlowManagers.PreTrackings.Dtos;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Eva.Insurtech.FlowManagers.PreTrackings
{
    [ExcludeFromCodeCoverage]

    public class PreTrackingAppService_Tests : FlowManagerApplicationTestBase
    {
        private readonly IPreTrackingAppService _preTrackingAppService;

        public PreTrackingAppService_Tests()
        {
            _preTrackingAppService = GetRequiredService<IPreTrackingAppService>();
        }

        [Fact]
        public async Task GetPreTracking_ById_ReturnPreTracking()
        {
            var preTracking = await _preTrackingAppService.GetByTransactionReference("pre-tracking-test");

            var result = await _preTrackingAppService.GetById(preTracking.Result.Id);

            Assert.Equal(preTracking.Result.TransactionReference, result.Result.TransactionReference);
        }

        [Fact]
        public async Task GetPreTracking_ByNonExistsId_ReturnNull()
        {
            var result = await _preTrackingAppService.GetById(Guid.NewGuid());

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Null(result.Result);
            Assert.Null(result.Error);
        }

        [Fact]
        public async Task CreatePreTracking_WithNullData_ReturnNullException()
        {
            CreatePreTrackingDto preTracking = null;

            try
            {
                await _preTrackingAppService.InsertAsync(preTracking);
            }
            catch (Exception ex)
            {
                Assert.Equal("Method arguments are not valid! See ValidationErrors for details.", ex.Message);
            }
        }

        [Fact]
        public async Task CreatePreTracking_WithData_ReturnPreTracking()
        {
            var preTracking = GetCreate();

            var result = await _preTrackingAppService.InsertAsync(preTracking);

            Assert.NotNull(result);
            Assert.Equal(result.Result.TransactionReference, preTracking.TransactionReference);
            Assert.Equal(result.Result.Identification, preTracking.Identification);
            Assert.Equal(result.Result.FullName, preTracking.FullName);
            Assert.Equal(result.Result.CellPhone, preTracking.CellPhone);
            Assert.Equal(result.Result.Email, preTracking.Email);
        }

        [Fact]
        public async Task CreatePreTrackingStep_WithData_ReturnPreTracking()
        {
            var preTracking = await _preTrackingAppService.GetByTransactionReference("pre-tracking-test");

            var input = GetPreTrackingStep();

            var result = await _preTrackingAppService.AddPreTrackingStep(input, preTracking.Result.Id);

            Assert.NotNull(result.Result.PreTrackingSteps);
            Assert.Equal(result.Result.PreTrackingSteps.FirstOrDefault().Container, input.Container);
            Assert.Equal(result.Result.PreTrackingSteps.FirstOrDefault().Component, input.Component);
            Assert.Equal(result.Result.PreTrackingSteps.FirstOrDefault().Method, input.Method);
            Assert.Equal(result.Result.PreTrackingSteps.FirstOrDefault().Body, input.Body);
            Assert.True(result.Result.PreTrackingSteps.FirstOrDefault().Observations.IsNullOrEmpty());
        }

        [Fact]
        public async Task CreatePreTrackingStep_WithNonExistsPreTracking_ReturnException()
        {
            var input = GetPreTrackingStep();

            var result = await _preTrackingAppService.AddPreTrackingStep(input, Guid.NewGuid());

            Assert.NotNull(result.Error);
            Assert.Equal(result.Error.Code, ErrorConsts.ERROR_NOT_FOUND_BY_ID);
        }



        #region Private Methods


        private static CreatePreTrackingDto GetCreate()
        {
            var preTracking = new CreatePreTrackingDto() {
                TransactionReference = "54fgsdg6s165dg55g1fg1f5",
                Identification = "1123659863",
                FullName = "Juan José Ramírez Rodríguez",
                CellPhone = "0952148520",
                Email = "email@email.com"
            };
            return preTracking;
        }

        private static CreatePreTrackingStepDto GetPreTrackingStep()
        {
            var preTrackingStep = new CreatePreTrackingStepDto()
            {
                Container = "container-test",
                Component = "component-test",
                Method = "method-test",
                Body = "body-test",
                Observations = ""
            };
            return preTrackingStep;
        }


        #endregion    
    }
}
