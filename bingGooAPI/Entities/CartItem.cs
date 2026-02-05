namespace bingGooAPI.Entities
{
    public class CartItem
    {
        public int CartItemID { get; set; }


        public int CartID { get; set; }

        public int StockID { get; set; }   


        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }



        public decimal DiscountPercent { get; set; }

        public decimal DiscountAmount { get; set; }



        public decimal TaxPercent { get; set; }

        public decimal TaxAmount { get; set; }



        public decimal SubTotal { get; set; }

        public decimal TotalPrice { get; set; }
        public int ProductID { get; set; }  




        public Cart Cart { get; set; }

        public ProductStock ProductStock { get; set; }
    }
}
