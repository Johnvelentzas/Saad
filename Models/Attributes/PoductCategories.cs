

using System.ComponentModel.DataAnnotations;

namespace Models.Attributes
{
    internal class PoductCategories
    {
        [Key]
        public int Id { get; set; }
        public required string CategoryName { get; set; }
    }
}
