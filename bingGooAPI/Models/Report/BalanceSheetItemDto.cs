namespace bingGooAPI.Models.Report
{
    public class BalanceSheetItemDto
    {
        public string Code { get; set; } = string.Empty;  
        public string Name { get; set; } = string.Empty; 
        public decimal Amount { get; set; }
    }
}
