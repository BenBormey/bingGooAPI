namespace JuJuBiAPI.Queries
{
    public static class VatSettingQueries
    {
        // Ensure the single row exists (Id = 1), then return it. Keeps the
        // read self-healing so a fresh DB never returns null.
        // NOTE: Percent is a T-SQL keyword, so the column is always bracketed.
        public const string Get = @"
    IF NOT EXISTS (SELECT 1 FROM VatSetting WHERE Id = 1)
        INSERT INTO VatSetting (Id, [Percent], UpdatedAt) VALUES (1, 0, GETDATE());

    SELECT Id, [Percent] AS [Percent], UpdatedBy, UpdatedAt
    FROM VatSetting
    WHERE Id = 1;";

        public const string Update = @"
    IF NOT EXISTS (SELECT 1 FROM VatSetting WHERE Id = 1)
        INSERT INTO VatSetting (Id, [Percent], UpdatedBy, UpdatedAt)
        VALUES (1, @Percent, @UpdatedBy, GETDATE());
    ELSE
        UPDATE VatSetting
        SET [Percent] = @Percent,
            UpdatedBy = @UpdatedBy,
            UpdatedAt = GETDATE()
        WHERE Id = 1;";
    }
}
