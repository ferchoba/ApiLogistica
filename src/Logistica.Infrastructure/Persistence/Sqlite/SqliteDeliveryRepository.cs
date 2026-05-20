using Logistica.Domain.Entities;
using Logistica.Domain.Interfaces;
using Logistica.Infrastructure.Resources;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Logistica.Infrastructure.Persistence.Sqlite;

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
            throw new InvalidOperationException(InfraMessages.Repo_MissingConnectionString);
        }

        _connectionString = connectionString;
        EnsureDatabaseInitialized();
    }

    private void EnsureDatabaseInitialized()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = SqliteQueries.Schema.CreateDeliveryOrdersTable + "\n" + SqliteQueries.Schema.CreateOrderIdIndex;
        command.ExecuteNonQuery();
    }

    public async Task BulkInsertAsync(IReadOnlyList<DeliveryOrder> orders, CancellationToken cancellationToken = default)
    {
        if (orders == null || orders.Count == 0) return;

        var orderCount = orders.Count;
        _logger.LogInformation(InfraMessages.Repo_BulkInsertStart, orderCount);

        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        using var transaction = (SqliteTransaction)await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            using var command = connection.CreateCommand();
            command.Transaction = transaction;

            command.CommandText = SqliteQueries.DeliveryOrder.BulkInsert;

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
            _logger.LogInformation(InfraMessages.Repo_BulkInsertSuccess, orderCount);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);

            throw new InvalidOperationException(
                string.Format(InfraMessages.Repo_BulkInsertFailed, orders.Count),
                ex);
        }
    }
}
