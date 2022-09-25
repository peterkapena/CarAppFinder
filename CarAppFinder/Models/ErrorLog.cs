using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarAppFinder.Models.Pub
{
    public partial class Pub
    {
        public enum StatusCode
        {
            InternalServerError = 500,
            InvalidData = 400
        }
        public class ErrorLog
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public long ErrorID { get; set; }
#nullable enable
            public string? StackTrace { get; set; }
            public string? Source { get; set; }
#nullable disable
            public string Message { get; set; }
            public string InnerException { get; set; }
            public int HResult { get; set; }
#nullable enable
            public string? HelpLink { get; set; }
            public DateTime? Date { get; set; }
            public string? UserName { get; set; }
            public string? UserId { get; set; }
#nullable disable
        }
    }
}
