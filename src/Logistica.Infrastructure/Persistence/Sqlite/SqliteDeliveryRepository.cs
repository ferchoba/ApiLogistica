using Logistica.Domain.Entities;
using Logistica.Domain.Interfaces;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace Logistica.Infrastructure.Persistence.Sqlite
{
    public class SqliteDeliveryRepository : IDeliveryRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<SqliteDeliveryRepository> _logger;

        public SqliteDeliveryRepository(
            ILogger<SqliteDeliveryRepository> logger,
            IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var connectionString = configuration.GetConnectionString("Sqlite");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Fallo Crítico: La cadena de conexión 'Sqlite' no está configurada o es vacía en el archivo de entorno. Verifique la configuración de ConnectionStrings.");
            }

            _connectionString = connectionString;
            EnsureDatabaseInitialized();
        }

        
        private void EnsureDatabaseInitialized()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS DeliveryOrders (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    OrderId TEXT NOT NULL,
                    Customer TEXT NOT NULL,
                    Address TEXT NOT NULL,
                    DeliveryDate TEXT NOT NULL,
                    Weight REAL NOT NULL
                );
                
                CREATE INDEX IF NOT EXISTS IX_DeliveryOrders_OrderId ON DeliveryOrders(OrderId);
            ";
            command.ExecuteNonQuery();
        }

        
        public async Task BulkInsertAsync(IReadOnlyList<DeliveryOrder> orders, CancellationToken cancellationToken = default)
        {
            if (orders == null || orders.Count == 0) return;

            var orderCount = orders.Count;
            _logger.LogInformation("Iniciando BulkInsertAsync atómico para {OrderCount} registros.", orderCount);

            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            using var transaction = (SqliteTransaction)await connection.BeginTransactionAsync(cancellationToken);

            try
            {
                using var command = connection.CreateCommand();
                command.Transaction = transaction;

                command.CommandText = @"
                    INSERT INTO DeliveryOrders (OrderId, Customer, Address, DeliveryDate, Weight)
                    VALUES ($orderId, $customer, $address, $deliveryDate, $weight);
                ";

                var pOrderId = command.CreateParameter(); pOrderId.ParameterName = "$orderId"; command.Parameters.Add(pOrderId);
                var pCustomer = command.CreateParameter(); pCustomer.ParameterName = "$customer"; command.Parameters.Add(pCustomer);
                var pAddress = command.CreateParameter(); pAddress.ParameterName = "$address"; command.Parameters.Add(pAddress);
                var pDate = command.CreateParameter(); pDate.ParameterName = "$deliveryDate"; command.Parameters.Add(pDate);
                var pWeight = command.CreateParameter(); pWeight.ParameterName = "$weight"; command.Parameters.Add(pWeight);

                foreach (var order in orders)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    pOrderId.Value = order.OrderId;
                    pCustomer.Value = order.Customer;
                    pAddress.Value = order.Address;
                    pDate.Value = order.DeliveryDate.ToString("yyyy-MM-dd HH:mm:ss");
                    pWeight.Value = order.Weight;

                    await command.ExecuteNonQueryAsync(cancellationToken);
                }

                await transaction.CommitAsync(cancellationToken);
                _logger.LogInformation("Transacción atómica completada exitosamente. Se persistieron {OrderCount} registros.", orderCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fallo crítico durante el BulkInsert. Ejecutando ROLLBACK explícito para garantizar integridad.");
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
