namespace JuJuBiAPI.Queries
{
    public static class FranchiseQueries
    {
        public const string GetAll = @"
         SELECT
     f.FranchiseId,
     f.Outlet,
     f.OutletName,
     f.FranchiseInformation,
     f.FranchiseTypeId,
	 ft.TypeName,
        f.AgreementDate,
        f.ExpirationDate,
        f.AfterDate
 FROM Franchise f inner join franchise_type ft
 on ft.Id  = f.FranchiseTypeId
 ORDER BY f.FranchiseId DESC
;

            ";

        public const string GetById = @"
 SELECT
     f.FranchiseId,
     f.Outlet,
     f.OutletName,
     f.FranchiseInformation,
     f.FranchiseTypeId,
	 ft.TypeName,
     f.AgreementDate,
        f.ExpirationDate,
        f.AfterDate
 FROM Franchise f inner join franchise_type ft
 on ft.Id  = f.FranchiseTypeId
 WHERE FranchiseId = @Id
 ORDER BY f.FranchiseId DESC
;

            ";

        public const string Insert = @"
                INSERT INTO Franchise
                    (Outlet, OutletName, FranchiseInformation, FranchiseTypeId, AgreementDate, ExpirationDate, AfterDate)
                VALUES
                    (@Outlet, @OutletName, @FranchiseInformation, @FranchiseTypeId, @AgreementDate, @ExpirationDate, @AfterDate)
            ";

        public const string Update = @"
                UPDATE Franchise
                SET
                    Outlet = @Outlet,
                    OutletName = @OutletName,
                    FranchiseInformation = @FranchiseInformation,
                    FranchiseTypeId = @FranchiseTypeId,
                    AgreementDate = @AgreementDate,
                    ExpirationDate = @ExpirationDate,
                    AfterDate = @AfterDate
                WHERE FranchiseId = @FranchiseId
            ";

        public const string Delete = @"
        DELETE FROM Franchise
        WHERE FranchiseId = @Id
    ";
    }
}
