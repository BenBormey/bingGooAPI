namespace bingGooAPI.Entities
{
    public class Currency : BaseEntity
    {
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }

        public decimal BuyRate { get; set; }
        public decimal SellRate { get; set; }

        public bool IsBase { get; set; }
    }
}
