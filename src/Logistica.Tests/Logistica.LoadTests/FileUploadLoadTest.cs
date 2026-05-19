using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using NBomber.Contracts;
using NBomber.Contracts.Stats;
using NBomber.CSharp;
using Xunit;

namespace Logistica.LoadTests
{
    public class FileUploadLoadTest
    {
        [Fact]
        [Trait("Category", "LoadTest")]
        public void UploadFile_LoadTest_ConcurrentUsers()
        {
            // Arrange
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://localhost:5000");

            var scenario = Scenario.Create("file_upload_scenario", async context =>
            {
                // Generamos un archivo CSV sintético para la prueba
                // Simular un lote para no colapsar la memoria del runner local
                var csvPayload = new StringBuilder();
                csvPayload.AppendLine("OrderId,Customer,Address,DeliveryDate,Weight");
                for (int i = 0; i < 50; i++) // Lote de 50 registros por request
                {
                    var guidPrefix = Guid.NewGuid().ToString().AsSpan(0, 8);
                    csvPayload.Append("ORD-");
                    csvPayload.Append(guidPrefix);
                    csvPayload.Append(",User ");
                    csvPayload.Append(i);
                    csvPayload.Append(",Dir ");
                    csvPayload.Append(i);
                    csvPayload.AppendLine(",2025-01-01,10.5");
                }

                using var content = new MultipartFormDataContent();
                var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes(csvPayload.ToString()));
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("text/csv");
                content.Add(fileContent, "file", "test.csv");

                try
                {
                    // Act
                    var response = await httpClient.PostAsync("/api/deliveries/upload/csv", content, CancellationToken.None);
                    
                    // Assert de la respuesta de red
                    if (response.IsSuccessStatusCode)
                    {
                        // Se mide la latencia y se registra la transacción como Ok
                        return Response.Ok(statusCode: ((int)response.StatusCode).ToString(), sizeBytes: content.Headers.ContentLength ?? 0);
                    }
                    
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    return Response.Fail(statusCode: ((int)response.StatusCode).ToString(), message: errorMsg);
                }
                catch (Exception ex)
                {
                    return Response.Fail(statusCode: "500", message: ex.Message);
                }
            })
            .WithoutWarmUp()
            .WithLoadSimulations(
                // Inyectamos 10 requests concurrentes por segundo, sostenido por 10 segundos
                Simulation.Inject(rate: 10, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(10))
            );

            // Act - Correr NBomber
            var stats = NBomberRunner
                .RegisterScenarios(scenario)
                .WithReportFileName("logistica_load_report")
                .WithReportFolder("./nbomber_reports")
                .WithReportFormats(ReportFormat.Html, ReportFormat.Md)
                .Run();

            // Assert - Criterios de Aceptación (SLA)
            Assert.NotNull(stats);
            var scenarioStats = stats.ScenarioStats.Get("file_upload_scenario");
            
            // Requerimos al menos un 95% de éxito bajo carga
            var totalRequests = scenarioStats.Ok.Request.Count + scenarioStats.Fail.Request.Count;
            var okPercent = totalRequests > 0 ? (double)scenarioStats.Ok.Request.Count / totalRequests * 100 : 0;
            
            Assert.True(okPercent >= 95, $"SLA Fallido: Tasa de éxito muy baja ({okPercent}%)");
            
            // Requerimos que la latencia p95 sea menor a 2000 ms para este lote
            Assert.True(scenarioStats.Ok.Latency.Percent95 < 2000, $"SLA Fallido: Latencia p95 muy alta ({scenarioStats.Ok.Latency.Percent95} ms)");
        }
    }
}
