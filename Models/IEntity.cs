

namespace Models
{
    public interface IEntity
    {
        int Id { get; set; }

        bool IsDraft { get; set; }

        int FromId { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
