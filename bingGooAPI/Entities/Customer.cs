namespace JuJuBiAPI.Entities
{
    public class Customer
    {
        public int CustomerID { get; set; }

        public string? CustomerCode { get; set; }
        public string? CustomerName { get; set; }

        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }

        public int Points { get; set; }
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
