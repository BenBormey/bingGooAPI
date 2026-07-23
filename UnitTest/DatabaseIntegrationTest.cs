using Dapper;
using JuJuBiAPI.Queries;
using Microsoft.Data.SqlClient;

namespace UnitTest
{
    // Runs the app's real SQL against the real database schema. Mocked
    // controller tests can never catch "invalid object name" bugs after a
    // table is renamed or dropped — these tests can. Every test silently
    // passes when the database is unreachable, so the suite stays green on
    // machines without SQL Server (e.g. CI).
    public class DatabaseIntegrationTest
    {
        private const string DefaultConnection =
            "Server=DESKTOP-1ULGF16\\JUJUBO;Database=DBJuJuBi;Trusted_Connection=True;TrustServerCertificate=True;Connect Timeout=3;";

        private static SqlConnection? TryOpen()
        {
            try
            {
                var conn = new SqlConnection(
                    Environment.GetEnvironmentVariable("TEST_DB_CONNECTION") ?? DefaultConnection);
                conn.Open();
                return conn;
            }
            catch (SqlException)
            {
                return null; // no DB on this machine — tests no-op
            }
        }

        [Fact]
        public async Task Report_PnL_RunsOnRealSchema()
        {
            using var conn = TryOpen();
            if (conn == null) return;

            var rows = await conn.QueryAsync(
                ReportQueries.PnL,
                new { From = DateTime.Today.AddYears(-1), To = DateTime.Today });

            Assert.NotNull(rows);
        }

        [Fact]
        public async Task Report_BalanceSheetInventory_RunsOnRealSchema()
        {
            using var conn = TryOpen();
            if (conn == null) return;

            var total = await conn.ExecuteScalarAsync<decimal>(
                ReportQueries.BalanceSheetInventory);

            Assert.True(total >= 0);
        }

        [Fact]
        public async Task Report_BalanceSheetCash_RunsOnRealSchema()
        {
            using var conn = TryOpen();
            if (conn == null) return;

            await conn.ExecuteScalarAsync<decimal>(
                ReportQueries.BalanceSheetCash,
                new { AsOfDate = DateTime.Today });
        }

        [Fact]
        public async Task Report_RetainedEarnings_RunsOnRealSchema()
        {
            using var conn = TryOpen();
            if (conn == null) return;

            await conn.ExecuteScalarAsync<decimal>(
                ReportQueries.BalanceSheetRetainedEarnings,
                new { AsOfDate = DateTime.Today });
        }

        [Fact]
        public async Task PurchaseOrder_StockQueries_RunOnRealSchema()
        {
            using var conn = TryOpen();
            if (conn == null) return;

            var exists = await conn.ExecuteScalarAsync<int>(
                PurchaseOrderQueries.OutletStockExists,
                new { ProNumY = "ZZ-TEST-NONE", OutletId = -1 });

            Assert.Equal(0, exists);

            // UPDATE keyed to a row that cannot exist: validates the table and
            // column names without touching any data.
            var affected = await conn.ExecuteAsync(
                PurchaseOrderQueries.AddProductStock,
                new { ReceivedQty = 1m, ProNumY = "ZZ-TEST-NONE", OutletId = -1 });

            Assert.Equal(0, affected);

            // INSERT needs real FK targets; run only when they exist and roll
            // the row back either way.
            var outletId = await conn.ExecuteScalarAsync<int?>(
                "SELECT TOP 1 Id FROM Outlet");
            var proNumY = await conn.ExecuteScalarAsync<string?>(
                "SELECT TOP 1 ProNumY FROM TPRProducts");

            if (outletId == null || proNumY == null) return;

            using var tx = conn.BeginTransaction();
            await conn.ExecuteAsync(
                PurchaseOrderQueries.InsertProductStock,
                new { OutletId = outletId.Value, ProNumY = proNumY, StockQty = 1m },
                tx);
            tx.Rollback();
        }

        [Fact]
        public async Task OutletOrder_WarehouseStockQueries_RunOnRealSchema()
        {
            using var conn = TryOpen();
            if (conn == null) return;

            await conn.ExecuteScalarAsync<int?>(
                OutletOrderQueries.GetWarehouseOutletId);

            await conn.QueryFirstOrDefaultAsync<decimal?>(
                OutletOrderQueries.GetWarehouseStock,
                new { ProNumY = "ZZ-TEST-NONE", OutletId = -1 });
        }

        [Fact]
        public async Task TransferOrder_SourceStockQuery_RunsOnRealSchema()
        {
            using var conn = TryOpen();
            if (conn == null) return;

            await conn.QueryFirstOrDefaultAsync<decimal?>(
                TransferOrderQueries.GetSourceStock,
                new { ProNumY = "ZZ-TEST-NONE", OutletId = -1 });
        }

        [Fact]
        public async Task Order_OutletStockQuery_RunsOnRealSchema()
        {
            using var conn = TryOpen();
            if (conn == null) return;

            await conn.QuerySingleOrDefaultAsync<decimal?>(
                OrderQueries.GetOutletStock,
                new { ProNumY = "ZZ-TEST-NONE", OutletId = -1 });
        }
    }
}
