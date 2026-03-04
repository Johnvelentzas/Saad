using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Production
{
    public class TaskDependencies
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Tasks")]
        public required int TaskId { get; set; }
        [ForeignKey("Tasks")]
        public required int DependsOnTaskId { get; set; }
    }
}
