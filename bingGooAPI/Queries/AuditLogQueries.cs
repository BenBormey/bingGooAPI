namespace JuJuBiAPI.Queries
{
    public static class AuditLogQueries
    {
        public const string Insert = @"
    INSERT INTO AuditLogs
        (UserId, UserName, Action, Module, TableName, RecordId,
         OldValue, NewValue, IPAddress, DeviceName, Remark, CreatedAt)
    VALUES
        (@UserId, @UserName, @Action, @Module, @TableName, @RecordId,
         @OldValue, @NewValue, @IPAddress, @DeviceName, @Remark, GETDATE());";

        // Newest first; optional filters fall through when null/empty.
        public const string GetRecent = @"
    SELECT TOP (@Take) *
    FROM AuditLogs
    WHERE (@Action = '' OR Action = @Action)
      AND (@Module = '' OR Module = @Module)
      AND (@UserName = '' OR UserName = @UserName)
    ORDER BY AuditLogId DESC;";
    }
}
