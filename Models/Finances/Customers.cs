using System.ComponentModel.DataAnnotations;

namespace Models.Finances
{
    public class Customers : IEntity
    {
        [Key]
        public int Id { get; set; }
        public bool IsDraft { get; set; } = false;
        public int FromId { get; set; } = 0;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Telephone { get; set; }
        public string? TaxNumber { get; set; }
        public CustomerType? Type { get; set; }
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }

    public enum CustomerType
    {
        Retail,
        Wholesale
    }
}
