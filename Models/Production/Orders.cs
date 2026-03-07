using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Production
{
    public class Orders : IEntity
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Customers")]
        public required int CustomerId { get; set; }
        public required bool IsCompleted { get; set; }
        public string? SaleChannel { get; set; }
        public string? Comments { get; set; }
    }
}
