namespace JuJuBiAPI.Models.Product
{
    public class ProductOutletDto
    {
        public int OutletID { get; set; }

        public string? OutletName { get; set; }

        public int StockQty { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
