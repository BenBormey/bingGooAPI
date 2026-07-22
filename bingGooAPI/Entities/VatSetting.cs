namespace JuJuBiAPI.Entities
{
    // Single-row table: the one outlet-wide VAT percentage the POS applies to
    // every order. Managed from the management app's VAT settings form.
    public class VatSetting
    {
        public int Id { get; set; }
        public decimal Percent { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
