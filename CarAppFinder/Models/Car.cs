namespace CarAppFinder.Models
{
    public class Car : BaseModel
    {
        public string TrackerSerialNumber { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public List<Coordinates> Coordinates { get; set; }
    }
}
