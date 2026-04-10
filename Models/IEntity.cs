

namespace Models
{
    public interface IEntity
    {
        int Id { get; set; }

        bool IsDraft { get; set; }
    }
}
