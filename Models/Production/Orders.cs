using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Production
{
    public class Orders : IEntity
    {
        [Key]
        public int Id { get; set; }
        public bool IsDraft { get; set; } = false;
        public int FromId { get; set; } = 0;
        [ForeignKey("Customers")]
        public required int CustomerId { get; set; }
        public required bool IsCompleted { get; set; }
        public SaleChannel? SaleChannel { get; set; }
        public string? Comments { get; set; }
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
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
