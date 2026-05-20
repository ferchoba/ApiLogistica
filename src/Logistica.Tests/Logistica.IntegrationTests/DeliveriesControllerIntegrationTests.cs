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
       
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();

        var csvContent = "OrderId,Customer,Address,DeliveryDate,Weight\nORD-001,Test Customer,Calle 1,2099-01-01,5.0";
        using var content = new MultipartFormDataContent();
        
        var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes(csvContent));
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("text/csv");
        
        content.Add(fileContent, "file", "test.csv");

        
        var response = await client.PostAsync("/api/deliveries/upload/csv", content);

        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UploadEndpoint_WithCrossFormatMismatch_Returns400BadRequest()
    {
        
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();

        var csvContent = "OrderId,Customer,Address,DeliveryDate,Weight\nORD-001,Test Customer,Calle 1,2099-01-01,5.0";
        using var content = new MultipartFormDataContent();
        
        var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes(csvContent));
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("text/csv");
        
        content.Add(fileContent, "file", "test.csv");

        
        var response = await client.PostAsync("/api/deliveries/upload/json", content);

       
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Contains("Inconsistencia de formato", responseString);
    }
}
