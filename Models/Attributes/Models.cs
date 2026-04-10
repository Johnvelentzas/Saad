using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Attributes
{
    public class Models : IEntity
    {
        [Key]
        public int Id { get; set; }
        public bool IsDraft { get; set; } = false;
        [ForeignKey("ProductCategories")]
        public int CategoryId { get; set; }
        public required string ModelName { get; set; }
    }
}
