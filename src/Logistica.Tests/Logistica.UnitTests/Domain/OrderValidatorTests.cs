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
            
            var validOrder = new DeliveryOrder(
                OrderId: "ORD-001",
                Customer: "Carlos Ruiz",
                Address: "Calle 10 #20-30",
                DeliveryDate: DateTime.UtcNow.AddDays(2), 
                Weight: 2.5m 
            );

            
            var errors = OrderValidator.Validate(validOrder, rowNumber: 1).ToList();

            
            Assert.Empty(errors); 
        }

        [Theory]
        [InlineData("", "MISSING_IDENTITY")]
        [InlineData("   ", "MISSING_IDENTITY")]
        [InlineData(null, "MISSING_IDENTITY")]
        public void Validate_MissingOrderId_ReturnsIdentityError(string? invalidOrderId, string expectedErrorCode)
        {
            
            var order = new DeliveryOrder(invalidOrderId!, "Juan", "Calle 1", DateTime.UtcNow, 1m);

            
            var errors = OrderValidator.Validate(order, rowNumber: 2).ToList();

           
            Assert.Single(errors);
            Assert.Equal(expectedErrorCode, errors[0].ErrorCode); // FIX S6608: [0] en vez de .First()
        }

        [Theory]
        [InlineData("", "Calle 123")]
        [InlineData("Ana", "")]
        public void Validate_MissingDestination_ReturnsDestinationError(string customer, string address)
        {
            
            var order = new DeliveryOrder("ORD-002", customer, address, DateTime.UtcNow, 1m);

           
            var errors = OrderValidator.Validate(order, rowNumber: 3).ToList();

            
            Assert.Single(errors);
            Assert.Equal("INVALID_DESTINATION", errors[0].ErrorCode); // FIX S6608: [0] en vez de .First()
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1.5)]
        [InlineData(-100)]
        public void Validate_InvalidWeight_ReturnsWeightError(decimal invalidWeight)
        {
            
            var order = new DeliveryOrder("ORD-003", "Maria", "Av 10", DateTime.UtcNow, invalidWeight);

            
            var errors = OrderValidator.Validate(order, rowNumber: 4).ToList();

            
            Assert.Single(errors);
            Assert.Equal("INVALID_WEIGHT", errors[0].ErrorCode); // FIX S6608: [0] en vez de .First()
        }

        [Fact]
        public void Validate_PastDeliveryDate_ReturnsDateError()
        {
           
            var pastDate = DateTime.UtcNow.AddDays(-1);
            var order = new DeliveryOrder("ORD-004", "Pedro", "Diagonal 4", pastDate, 5m);

           
            var errors = OrderValidator.Validate(order, rowNumber: 5).ToList();

            
            Assert.Single(errors);
            Assert.Equal("INVALID_DATE", errors[0].ErrorCode); // FIX S6608: [0] en vez de .First()
        }

        [Fact]
        public void Validate_MultipleViolations_ReturnsAllErrors()
        {
            
            var completelyInvalidOrder = new DeliveryOrder(
                OrderId: "", 
                Customer: "", 
                Address: "Calle 1",
                DeliveryDate: DateTime.UtcNow.AddDays(-5), 
                Weight: -2m 
            );

            
            var errors = OrderValidator.Validate(completelyInvalidOrder, rowNumber: 6).ToList();

            
            Assert.Equal(4, errors.Count); 
            Assert.Contains(errors, e => e.ErrorCode == "MISSING_IDENTITY");
            Assert.Contains(errors, e => e.ErrorCode == "INVALID_DESTINATION");
            Assert.Contains(errors, e => e.ErrorCode == "INVALID_DATE");
            Assert.Contains(errors, e => e.ErrorCode == "INVALID_WEIGHT");
        }
    }
}
