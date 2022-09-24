using MessagePack;

namespace CarAppFinder.Models
{
    public class Car : BaseModel
    { 
        public long Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }

        public string TrackerId { get; set; }
        public List<Coordinates> Coordinates { get; set; }
    }
}
