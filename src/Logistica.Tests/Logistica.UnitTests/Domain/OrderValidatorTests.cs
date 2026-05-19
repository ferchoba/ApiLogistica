using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logistica.Domain.Entities;
using Logistica.Domain.Rules;

namespace Logistica.UnitTests.Domain
{
    public class OrderValidatorTests
    {
        [Fact]
        public void Validate_ValidOrder_ReturnsNoErrors()
        {
            // Arrange (Preparar)
            var validOrder = new DeliveryOrder(
                OrderId: "ORD-001",
                Customer: "Carlos Ruiz",
                Address: "Calle 10 #20-30",
                DeliveryDate: DateTime.UtcNow.AddDays(2), // Fecha futura
                Weight: 2.5m // Peso válido
            );

            // Act (Actuar)
            var errors = OrderValidator.Validate(validOrder, rowNumber: 1).ToList();

            // Assert (Afirmar)
            Assert.Empty(errors); // No debe haber errores
        }

        [Theory]
        [InlineData("", "MISSING_IDENTITY")]
        [InlineData("   ", "MISSING_IDENTITY")]
        [InlineData(null, "MISSING_IDENTITY")]
        public void Validate_MissingOrderId_ReturnsIdentityError(string? invalidOrderId, string expectedErrorCode)
        {
            // Arrange
            // FIX CS8604: El operador '!' indica que pasamos null intencionalmente para probar la validación.
            // La anotación nullable se mantiene en la firma para documentar el intent del test.
            var order = new DeliveryOrder(invalidOrderId!, "Juan", "Calle 1", DateTime.UtcNow, 1m);

            // Act
            var errors = OrderValidator.Validate(order, rowNumber: 2).ToList();

            // Assert
            Assert.Single(errors);
            Assert.Equal(expectedErrorCode, errors[0].ErrorCode); // FIX S6608: [0] en vez de .First()
        }

        [Theory]
        [InlineData("", "Calle 123")]
        [InlineData("Ana", "")]
        public void Validate_MissingDestination_ReturnsDestinationError(string customer, string address)
        {
            // Arrange
            var order = new DeliveryOrder("ORD-002", customer, address, DateTime.UtcNow, 1m);

            // Act
            var errors = OrderValidator.Validate(order, rowNumber: 3).ToList();

            // Assert
            Assert.Single(errors);
            Assert.Equal("INVALID_DESTINATION", errors[0].ErrorCode); // FIX S6608: [0] en vez de .First()
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1.5)]
        [InlineData(-100)]
        public void Validate_InvalidWeight_ReturnsWeightError(decimal invalidWeight)
        {
            // Arrange
            var order = new DeliveryOrder("ORD-003", "Maria", "Av 10", DateTime.UtcNow, invalidWeight);

            // Act
            var errors = OrderValidator.Validate(order, rowNumber: 4).ToList();

            // Assert
            Assert.Single(errors);
            Assert.Equal("INVALID_WEIGHT", errors[0].ErrorCode); // FIX S6608: [0] en vez de .First()
        }

        [Fact]
        public void Validate_PastDeliveryDate_ReturnsDateError()
        {
            // Arrange: Simulamos una fecha de ayer
            var pastDate = DateTime.UtcNow.AddDays(-1);
            var order = new DeliveryOrder("ORD-004", "Pedro", "Diagonal 4", pastDate, 5m);

            // Act
            var errors = OrderValidator.Validate(order, rowNumber: 5).ToList();

            // Assert
            Assert.Single(errors);
            Assert.Equal("INVALID_DATE", errors[0].ErrorCode); // FIX S6608: [0] en vez de .First()
        }

        [Fact]
        public void Validate_MultipleViolations_ReturnsAllErrors()
        {
            // Arrange: Una orden que rompe múltiples reglas a la vez
            var completelyInvalidOrder = new DeliveryOrder(
                OrderId: "", // Falla: Identidad
                Customer: "", // Falla: Destino
                Address: "Calle 1",
                DeliveryDate: DateTime.UtcNow.AddDays(-5), // Falla: Fecha
                Weight: -2m // Falla: Peso
            );

            // Act
            var errors = OrderValidator.Validate(completelyInvalidOrder, rowNumber: 6).ToList();

            // Assert
            Assert.Equal(4, errors.Count); // Debe haber capturado las 4 validaciones fallidas
            Assert.Contains(errors, e => e.ErrorCode == "MISSING_IDENTITY");
            Assert.Contains(errors, e => e.ErrorCode == "INVALID_DESTINATION");
            Assert.Contains(errors, e => e.ErrorCode == "INVALID_DATE");
            Assert.Contains(errors, e => e.ErrorCode == "INVALID_WEIGHT");
        }
    }
}
