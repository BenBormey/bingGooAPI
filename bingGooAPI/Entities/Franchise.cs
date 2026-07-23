namespace JuJuBiAPI.Entities
{
    public class Franchise
    {
        public int FranchiseId { get; set; }

        public string Outlet { get; set; } = string.Empty;

        public string OutletName { get; set; } = string.Empty;

        public string FranchiseInformation { get; set; } = string.Empty;
        public int FranchiseTypeId { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public DateTime? AgreementDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public DateTime? AfterDate { get; set; }
    }
}
