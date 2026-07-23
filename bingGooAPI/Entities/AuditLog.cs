namespace JuJuBiAPI.Entities
{
    // One audited action — who did what, to which record, from where.
    // Written by AuditLogger; read back by GET api/AuditLog.
    public class AuditLog
    {
        public long AuditLogId { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public string Action { get; set; } = string.Empty;      // LOGIN / CREATE / UPDATE / VOID / FULFILL / REJECT ...
        public string Module { get; set; } = string.Empty;      // POS / Setting / OutletOrder / Auth ...
        public string TableName { get; set; } = string.Empty;
        public string RecordId { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string? IPAddress { get; set; }
        public string? DeviceName { get; set; }
        public string? Remark { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
