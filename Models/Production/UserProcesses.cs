

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Production
{
    public class UserProcesses : IEntity
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Users")]
        public required int UserId { get; set; }
        [ForeignKey("Processes")]
        public required int ProcessId { get; set; }
    }
}
