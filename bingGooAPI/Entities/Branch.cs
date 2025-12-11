namespace bingGooAPI.Entities
{
    public class Branch : BaseEntity
    {
        public string BranchCode { get; set; }
        public string BranchName { get; set; }

        public string Phone { get; set; }
        public string Province { get; set; }

        public string Address { get; set; }
        public string Remark { get; set; }

        public bool MainBranch { get; set; }
    }
}
