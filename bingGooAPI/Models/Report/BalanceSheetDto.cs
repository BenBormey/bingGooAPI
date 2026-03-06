namespace bingGooAPI.Models.Report
{
    public class BalanceSheetDto
    {
   
        public DateTime AsOfDate { get; set; }


        public List<BalanceSheetItemDto> Assets { get; set; } = new();
        public List<BalanceSheetItemDto> Liabilities { get; set; } = new();
        public List<BalanceSheetItemDto> Equity { get; set; } = new();

        public decimal TotalAssets => Assets.Sum(x => x.Amount);
        public decimal TotalLiabilities => Liabilities.Sum(x => x.Amount);
        public decimal TotalEquity => Equity.Sum(x => x.Amount);


        public bool IsBalanced =>
            TotalAssets == (TotalLiabilities + TotalEquity);
    }
}
