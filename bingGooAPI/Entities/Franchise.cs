namespace JuJuBiAPI.Entities
{
    public class Franchise
    {
        public int FranchiseId { get; set; }

        public string Outlet { get; set; }

        public string OutletName { get; set; }

        public string FranchiseInformation { get; set; }
        public int FranchiseTypeId { get; set; }
        public string TypeName { get; set; }
        public DateTime? AgreementDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public DateTime? AfterDate { get; set; }
    }
}
