using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Production
{
    public class Orders : IEntity
    {
        [Key]
        public int Id { get; set; }
        public bool IsDraft { get; set; } = false;
        [ForeignKey("Customers")]
        public required int CustomerId { get; set; }
        public required bool IsCompleted { get; set; }
        public SaleChannel? SaleChannel { get; set; }
        public string? Comments { get; set; }
    }

    public enum SaleChannel
    {
        InStore,
        Phone,
        Online,
        Email,
        SocialMedia
    }
}
