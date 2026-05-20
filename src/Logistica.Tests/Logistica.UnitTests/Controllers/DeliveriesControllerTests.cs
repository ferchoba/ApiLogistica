using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Logistica.API.Controllers;
using Logistica.Application.Dtos;
using Logistica.Application.Services;
using Logistica.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Logistica.UnitTests.Controllers;

public class DeliveriesControllerTests
{
    [Fact]
    public async Task UploadFile_WithCrossFormatMismatch_ReturnsBadRequest()
    {
       
        var mockOrchestrator = new Mock<INormalizationOrchestrator>();
        var mockParser = new Mock<IDeliveryParser>();
        mockParser.Setup(p => p.FormatId).Returns("JSON");
        
        var mockParserFactory = new Mock<IDeliveryParserFactory>();
        mockParserFactory.Setup(f => f.GetParser("JSON")).Returns(mockParser.Object);
        
        var controller = new DeliveriesController(mockOrchestrator.Object, mockParserFactory.Object);

        
        var fileContent = "OrderId,Customer,Address,DeliveryDate,Weight\nORD-001,Test,Calle 1,2099-01-01,5.0";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
        
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns("manifiesto.csv"); 
        mockFile.Setup(f => f.Length).Returns(stream.Length);
        mockFile.Setup(f => f.ContentType).Returns("text/csv"); 
        mockFile.Setup(f => f.OpenReadStream()).Returns(stream);

       
        var result = await controller.UploadFile("JSON", mockFile.Object, CancellationToken.None);

        
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        
        
        var errorValue = badRequestResult.Value;
        Assert.NotNull(errorValue);
        
        var errorProperty = errorValue.GetType().GetProperty("Error");
        Assert.NotNull(errorProperty);
        
        var errorMessage = errorProperty.GetValue(errorValue) as string;
        Assert.NotNull(errorMessage);
        Assert.Contains("Inconsistencia de formato", errorMessage);

        
        mockOrchestrator.Verify(
            o => o.ProcessFileAsync(It.IsAny<Stream>(), It.IsAny<IDeliveryParser>(), It.IsAny<CancellationToken>()), 
            Times.Never);
    }
}
