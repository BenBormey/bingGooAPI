namespace bingGooAPI.Models.Branch
{
    public class CreateBranch
    {
        public string BranchCode { get; set; }

        public string BranchName { get; set; }

        public bool IsMainBranch { get; set; }

        public bool Active { get; set; }
        public string? Remark { get; set; }
    }
}
