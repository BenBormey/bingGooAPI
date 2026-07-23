namespace JuJuBiAPI.Models.Branch
{
    public class CreateBranch
    {
        public string BranchCode { get; set; } = string.Empty;

        public string BranchName { get; set; } = string.Empty;

        public bool IsMainBranch { get; set; }

        public bool Active { get; set; }
        public string? Remark { get; set; }
    }
}
