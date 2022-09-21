using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarAppFinder.Services.Setting
{
    public class Setting
    {
        public JwtSetting JwtSetting { get; set; }
        public string ConnectionString { get; set; }
        public bool IsProduction { get; set; }

        [NotMapped]
        public JsonSerializerSettings JsonSerializerSettings
        {
            get
            {
                return new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
            }
        }
    }
}
