namespace JuJuBiAPI.Entities
{
    public class Currency : BaseEntity
    {
        public string CurrencyNo { get; set; } = string.Empty;      // CUR00001 (លេខបង្ហាញលើ UI)
        public string CurrencyCode { get; set; } = string.Empty;    // USD, KHM, THB
        public string CurrencyName { get; set; } = string.Empty;    // United State Dollar
        public decimal BuyRate { get; set; }
        public decimal SellRate { get; set; }
        public int? SupplierId { get; set; }        // nullable → NULL = all suppliers
        public bool IsBase { get; set; }
        public DateTime? UpdatedAt { get; set; }     // nullable → កែពេលណាទើបមានតម្លៃ
    }
}