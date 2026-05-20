using Logistica.Application.Services;
using Logistica.Domain.Entities;
using Logistica.Domain.Interfaces;
using Moq;

namespace Logistica.UnitTests.Services
{
    public class NormalizationOrchestratorServiceTests
    {
        private readonly Mock<IDeliveryRepository> _repositoryMock;
        private readonly Mock<ILogger<NormalizationOrchestratorService>> _loggerMock;
        private readonly Mock<IDeliveryParser> _parserMock;
        private readonly NormalizationOrchestratorService _sut;

        public NormalizationOrchestratorServiceTests()
        {
            _repositoryMock = new Mock<IDeliveryRepository>();
            _loggerMock = new Mock<ILogger<NormalizationOrchestratorService>>();
            _parserMock = new Mock<IDeliveryParser>();
            
            _sut = new NormalizationOrchestratorService(
                _repositoryMock.Object, 
                _loggerMock.Object);
        }

        [Fact]
        public async Task ProcessFileAsync_WithValidAndDuplicateOrders_DetectsDuplicatesAndConsolidates()
        {
            
            using var dummyStream = new MemoryStream();
            var order1 = new DeliveryOrder("ORD-001", "Cliente A", "Dir 1", DateTime.Now, 10m);
            var order2 = new DeliveryOrder("ORD-001", "Cliente B", "Dir 2", DateTime.Now, 20m);

            _parserMock.Setup(p => p.FormatId).Returns("MOCK_PARSER");
            
            async IAsyncEnumerable<(DeliveryOrder? Order, DeliveryError? Error)> GetMockedDataAsync()
            {
                yield return (order1, null);
                yield return (order2, null);
                await Task.CompletedTask;
            }

            _parserMock.Setup(p => p.ParseAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                       .Returns(GetMockedDataAsync());

           
            var response = await _sut.ProcessFileAsync(dummyStream, _parserMock.Object);

            
            Assert.NotNull(response);
            Assert.Equal(2, response.Summary.TotalProcessed);
            
            Assert.Equal(1, response.Summary.TotalSuccessful);
            Assert.Equal(1, response.Summary.TotalFailed);

            Assert.Single(response.Errors);
            Assert.Equal("DUPLICATE_ORDER", response.Errors.First().ErrorCode);

            _repositoryMock.Verify(r => r.BulkInsertAsync(It.Is<IReadOnlyList<DeliveryOrder>>(b => b.Count == 1),It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
