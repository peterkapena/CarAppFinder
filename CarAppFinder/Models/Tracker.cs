namespace CarAppFinder.Models
{
    public class Tracker : BaseModel
    {
        public string Id { get; set; }
        public string Position { get; set; }
        public Car Car { get; set; }
    }
}
