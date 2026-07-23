namespace JuJuBiAPI.Entities
{
    // One cashier shift: opens with a counted float, collects sales, closes
    // with a cash count so the drawer can be reconciled.
    public class Shift
    {
        public int ShiftID { get; set; }

        public int OutletId { get; set; }

        public int UserId { get; set; }

        public decimal OpeningFloat { get; set; }

        public DateTime OpenedAt { get; set; }

        public DateTime? ClosedAt { get; set; }

        public decimal? ExpectedCash { get; set; }

        public decimal? CountedCash { get; set; }

        public decimal? Variance { get; set; }

        public string Status { get; set; } = "Open";

        public string Notes { get; set; } = string.Empty;
    }

    // Totals for one shift, used for the close screen and the Z-report.
    public class ShiftSummary
    {
        public int ShiftID { get; set; }
        public decimal OpeningFloat { get; set; }
        public DateTime OpenedAt { get; set; }

        public int OrdersTotal { get; set; }
        public int OrdersVoided { get; set; }

        public decimal SalesCash { get; set; }
        public decimal SalesCard { get; set; }
        public decimal SalesWallet { get; set; }
        public decimal SalesTotal { get; set; }

        // OpeningFloat + SalesCash: what should be in the drawer.
        public decimal ExpectedCash { get; set; }
    }
}
