using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmenityFinder.utils;
using Newtonsoft.Json;

namespace AmenityFinder.models
{
    public abstract class AbstractModel
    {

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings {ContractResolver = new SnakeCasePropertyNamesContractResolver()});
        }

        public static T Deserialize<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data,
                new JsonSerializerSettings {ContractResolver = new SnakeCasePropertyNamesContractResolver()});
        }

        public bool IsUserLoggedIn => Core.IsUserLoggedIn();

        public int UserId => Core.GetUid();
    }

    public abstract class AbstractPaginated<T> : AbstractModel
    {
        public string Next { get; set; }
        public string Previous { get; set; }
        public List<T> Results { get; set; } 
    }
}
