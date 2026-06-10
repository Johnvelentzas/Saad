using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Production
{
    public class Tasks : IEntity
    {
        [Key]
        public int Id { get; set; }
        public bool IsDraft { get; set; } = false;
        public int FromId { get; set; } = 0;
        [ForeignKey("Processes")]
        public required int ProcessId { get; set; }
        [ForeignKey("Products")]
        public required int ProductId { get; set; }
        [ForeignKey("Users")]
        public required int UserId { get; set; }
        public required bool IsCompleted { get; set; }
        public required bool IsCancelled { get; set; }
        [DataType(DataType.Date)]
        public DateTime FinishBy { get; set; } = DateTime.Now;
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string? Comments { get; set; }
    }
}
