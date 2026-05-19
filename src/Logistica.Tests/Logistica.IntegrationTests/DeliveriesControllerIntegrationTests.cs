using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Logistica.IntegrationTests;

public class DeliveriesControllerIntegrationTests
{
    [Fact]
    public async Task UploadEndpoint_WithValidCsvFile_Returns200OK()
    {
        // Arrange
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();

        var csvContent = "OrderId,Customer,Address,DeliveryDate,Weight\nORD-001,Test Customer,Calle 1,2099-01-01,5.0";
        using var content = new MultipartFormDataContent();
        
        var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes(csvContent));
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("text/csv");
        
        content.Add(fileContent, "file", "test.csv");

        // Act
        var response = await client.PostAsync("/api/deliveries/upload/csv", content);

        // Assert — FIX Extra 5: Aserción real que valida el contrato HTTP del endpoint
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
