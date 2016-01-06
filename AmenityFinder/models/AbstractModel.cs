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
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings() {ContractResolver = new SnakeCasePropertyNamesContractResolver()});
        }

    }
}
