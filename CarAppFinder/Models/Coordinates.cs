namespace CarAppFinder.Models
{
    public class Coordinates : BaseModel
    {
        public string Coords { get; set; }
        public DateTime Time { get; set; }
        public string CarTrackerSerialNumber { get; set; }
        public Car Car { get; set; }
    }
}
