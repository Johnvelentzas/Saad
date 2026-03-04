

using System.ComponentModel.DataAnnotations;

namespace Models.Attributes
{
    public class ProductCategories
    {
        [Key]
        public int Id { get; set; }
        public required string CategoryName { get; set; }
    }
}
