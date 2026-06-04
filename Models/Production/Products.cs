using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Production
{
    public class Products : IEntity
    {
        [Key]
        public int Id { get; set; }
        public bool IsDraft { get; set; } = false;
        public int FromId { get; set; } = 0;
        [ForeignKey("Orders")]
        public required int OrderId { get; set; }
        [ForeignKey("ProductCategories")]
        public required int CategoryId { get; set; }
        [ForeignKey("Models")]
        public int ModelId { get; set; }
        public required bool IsCompleted { get; set; }
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [DataType(DataType.Date)]
        public required DateTime? ExpectedStartDate { get; set; }
        [DataType(DataType.Date)]
        public required DateTime? ExpectedFinishDate { get; set; }
        public string? Comments { get; set; }
    }
}
