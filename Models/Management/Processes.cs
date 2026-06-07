using System.ComponentModel.DataAnnotations;

namespace Models.Management
{
    public class Processes : IEntity
    {
        [Key]
        public int Id { get; set; }
        public bool IsDraft { get; set; } = false;
        public int FromId { get; set; } = 0;
        public required ProcessesType Type { get; set; }
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string? Comments { get; set; }

    }

    public enum ProcessesType
    {
        //Don't add more in the beggining. Only add at the end to avoid breaking existing data
        //Production
        Customers,
        Orders,
        Products,
        //Management
        Users,
        //Attributes
        ProductCategories,
        Brands,
        Models,
        Patterns,
        StitchTypes,
        YarnColors,
        Fabrics,
        //Appointments
        DropOffApt,
        TestTryApt,
        PickUpApt,
        //Foam Tasks
        FoamFix,
        FoamAdapt,
        FoamGel,
        FoamAnatomical,
        //Tasks
        CoverRemove,
        CustomPattern,
        Cut,
        Sew,
        Embroider,
        Bolt,
        Inspect,
        //Tabs
        Tasks,
        Foam,
        Calendar
        //Add more ONLY at the end to avoid breaking existing data
    }
}
