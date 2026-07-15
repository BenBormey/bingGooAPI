namespace JuJuBiAPI.Entities
{
    public class UOM
    {
        public int UOMId { get; set; }

        public string UOMCode { get; set; } = string.Empty;

        public string UOMName { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}