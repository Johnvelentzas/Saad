
using System.ComponentModel.DataAnnotations;

namespace Models.Production
{
    public class Users : IEntity
    {
        [Key]
        public int Id { get; set; }
        public bool IsDraft { get; set; } = false;
        public int FromId { get; set; } = 0;
        public required string Name { get; set; }
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        //TODO : Add image
    }
}
