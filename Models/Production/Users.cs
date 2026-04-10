
using System.ComponentModel.DataAnnotations;

namespace Models.Production
{
    public class Users : IEntity
    {
        [Key]
        public int Id { get; set; }
        public bool IsDraft { get; set; } = false;
        public required string Name { get; set; }
        //TODO : Add image
    }
}
