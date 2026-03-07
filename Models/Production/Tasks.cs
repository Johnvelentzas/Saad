using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Production
{
    public class Tasks : IEntity
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Processes")]
        public required int ProcessId { get; set; }
        [ForeignKey("Products")]
        public required int ProductId { get; set; }
        public required bool IsCompleted { get; set; }
    }
}
