namespace bingGooAPI.Models.Branch
{
    public class UpdateBranch
    {
        public int Id { get; set; }
        public bool Active { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? BranchCode { get; set; }
        public string BranchName { get; set; }
        public string Remark { get; set; }
        public bool MainBranch { get; set; }
    }
}
