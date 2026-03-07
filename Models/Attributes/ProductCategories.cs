

using System.ComponentModel.DataAnnotations;

namespace Models.Attributes
{
    public class ProductCategories : IEntity
    {
        [Key]
        public int Id { get; set; }
        public required string CategoryName { get; set; }
    }
}
