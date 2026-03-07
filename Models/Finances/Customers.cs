using System.ComponentModel.DataAnnotations;

namespace Models.Finances
{
    public class Customers : IEntity
    {
        [Key]
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public required string LastName { get; set; }
        public string? Email { get; set; }
        public string? Telephone { get; set; }
        public string? TaxNumber { get; set; }
        //TODO public required enum('Retail','Whoesale') CustomerType {  get; set; }
        [DataType(DataType.Date)]
        public required DateTime CreatedDate { get; set; }
    }
}
