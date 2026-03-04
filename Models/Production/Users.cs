
using System.ComponentModel.DataAnnotations;

namespace Models.Production
{
    public class Users
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }  
        //TODO : Add image
    }
}
