namespace bingGooAPI.Entities
{
    public class Branch : BaseEntity
    {
        public string? BranchCode { get; set; }
        public string BranchName { get; set; }
        public string Remark { get; set; }
        public bool MainBranch { get; set; }
    }
}
