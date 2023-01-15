using Eva.Insurtech.FlowManagers.PreTrackings.Exceptions;
using Eva.Insurtech.FlowManagers.PreTrackings.Inputs;
using Eva.Insurtech.FlowManagers.PreTrackings.PreTrackingSteps;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Eva.Insurtech.FlowManagers.PreTrackings
{
    [ExcludeFromCodeCoverage]
    public class PreTrackingManager_Tests : FlowManagerDomainTestBase
    {
        private readonly PreTrackingManager _preTrackingManager;

        public PreTrackingManager_Tests()
        {
            _preTrackingManager = GetRequiredService<PreTrackingManager>();
        }

        [Fact]
        public async Task GetPreTracking_ById_ReturnPreTracking()
        {
            var preTracking = await _preTrackingManager.GetByTransactionReference("pre-tracking-test");

            var result = await _preTrackingManager.GetById(preTracking.Id);

            Assert.Equal(preTracking.TransactionReference, result.TransactionReference);
        }

        [Fact]
        public async Task GetPreTracking_ByNonExistsId_ReturnNull()
        {
            var result = await _preTrackingManager.GetById(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task CreatePreTracking_WithNullData_ReturnNullException()
        {
            PreTracking preTracking = null;
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _preTrackingManager.InsertAsync(preTracking);
            });
        }

        [Fact]
        public async Task CreatePreTracking_WithData_ReturnPreTracking()
        {
            var preTracking = GetCreate();

            var result = await _preTrackingManager.InsertAsync(preTracking);

            Assert.NotNull(result);
            Assert.Equal(result.TransactionReference, preTracking.TransactionReference);
            Assert.Equal(result.Identification, preTracking.Identification);
            Assert.Equal(result.FullName, preTracking.FullName);
            Assert.Equal(result.CellPhone, preTracking.CellPhone);
            Assert.Equal(result.Email, preTracking.Email);
        }

        [Fact]
        public async Task CreatePreTrackingStep_WithData_ReturnPreTracking()
        {
            var preTracking = await _preTrackingManager.GetByTransactionReference("pre-tracking-test");

            var input = GetPreTrackingStep(preTracking.Id);

            var result = await _preTrackingManager.AddPreTrackingStep(input);

            Assert.NotNull(result.PreTrackingSteps);
            Assert.Equal(result.PreTrackingSteps.FirstOrDefault().Container, input.Container);
            Assert.Equal(result.PreTrackingSteps.FirstOrDefault().Component, input.Component);
            Assert.Equal(result.PreTrackingSteps.FirstOrDefault().Method, input.Method);
            Assert.Equal(result.PreTrackingSteps.FirstOrDefault().Body, input.Body);
            Assert.True(result.PreTrackingSteps.FirstOrDefault().Observations.IsNullOrEmpty());
        }

        [Fact]
        public async Task CreatePreTrackingStep_WithNonExistsPreTracking_ReturnException()
        {
            var input = GetPreTrackingStep(Guid.NewGuid());

            await Assert.ThrowsAsync<PreTrackingNotFoundException>(async () =>
            {
                await _preTrackingManager.AddPreTrackingStep(input);
            });
        }



        #region Private Methods


        private static PreTracking GetCreate()
        {
            var preTracking = new PreTracking(
                "54fgsdg6s165dg55g1fg1f5",
                "1123659863",
                "Juan José Ramírez Rodríguez",
                "0952148520",
                "email@email.com"
            );
            return preTracking;
        }

        private static PreTrackingStepInput GetPreTrackingStep(Guid preTrackingId)
        {
            var preTrackingStep = new PreTrackingStepInput() {
                PreTrackingId = preTrackingId,
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
