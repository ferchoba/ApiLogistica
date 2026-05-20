using System.Text;
using Logistica.Domain.Entities;
using Logistica.Infrastructure.Parsers;
using Moq;
using Xunit;

namespace Logistica.UnitTests.Parsers
{
    public class CsvParserTests
    {
        private readonly Mock<ILogger<CsvParser>> _loggerMock;
        private readonly CsvParser _sut;

        public CsvParserTests()
        {
            _loggerMock = new Mock<ILogger<CsvParser>>();
            _sut = new CsvParser(_loggerMock.Object);
        }

        [Fact]
        public async Task ParseAsync_WithValidLine_ReturnsDeliveryOrder()
        {
            
            var csvContent = "OrderId,Customer,Address,DeliveryDate,Weight\n" +
                             "ORD-001,Cliente A,Dirección 1,2023-10-15,10.5";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

            
            var results = new List<(DeliveryOrder? Order, DeliveryError? Error)>();
            await foreach (var item in _sut.ParseAsync(stream))
            {
                results.Add(item);
            }

            
            Assert.Single(results);
            Assert.NotNull(results[0].Order);
            Assert.Null(results[0].Error);
            Assert.Equal("ORD-001", results[0].Order!.OrderId);
        }

        [Fact]
        public async Task ParseAsync_WithInvalidFormat_ReturnsDeliveryError()
        {
            
            var csvContent = "OrderId,Customer,Address,DeliveryDate,Weight\n" +
                             "ORD-002,Cliente B"; // Registro incompleto (< 5 columnas)
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

            
            var results = new List<(DeliveryOrder? Order, DeliveryError? Error)>();
            await foreach (var item in _sut.ParseAsync(stream))
            {
                results.Add(item);
            }

            
            Assert.Single(results);
            Assert.Null(results[0].Order);
            Assert.NotNull(results[0].Error);
            Assert.Equal("INVALID_FORMAT", results[0].Error!.ErrorCode);
        }
    }
}
