namespace JuJuBiAPI.Entities
{
    public class Branch : BaseEntity
    {
        public string? BranchCode { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string Remark { get; set; } = string.Empty;
        public bool MainBranch { get; set; }
    }
}
