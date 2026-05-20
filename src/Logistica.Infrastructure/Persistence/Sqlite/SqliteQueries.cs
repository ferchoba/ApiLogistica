namespace Logistica.Infrastructure.Persistence.Sqlite;

internal static class SqliteQueries
{
    internal static class Schema
    {
        internal const string CreateDeliveryOrdersTable = @"
            CREATE TABLE IF NOT EXISTS DeliveryOrders (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                OrderId TEXT NOT NULL,
                Customer TEXT NOT NULL,
                Address TEXT NOT NULL,
                DeliveryDate TEXT NOT NULL,
                Weight REAL NOT NULL
            );";

        internal const string CreateOrderIdIndex = @"
            CREATE INDEX IF NOT EXISTS IX_DeliveryOrders_OrderId ON DeliveryOrders(OrderId);";
    }

    internal static class DeliveryOrder
    {
        internal const string BulkInsert = @"
            INSERT INTO DeliveryOrders (OrderId, Customer, Address, DeliveryDate, Weight)
            VALUES ($orderId, $customer, $address, $deliveryDate, $weight);";
    }
}
