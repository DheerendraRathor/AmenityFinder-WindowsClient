using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AmenityFinder.models
{
    public class Location: AbstractModel
    {

        public int Id { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string Name { get; set; }
        public bool IsFree { get; set; }
        public float Rating { get; set; }
        public User User { get; set; }
        public bool Male { get; set; }
        public bool Female { get; set; }
        public int FlagCount { get; set; }

    }

    public class NewLocation: AbstractModel
    {
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string Name { get; set; }
        public bool IsFree { get; set; } = true;
        public bool Male { get; set; } = true;
        public bool Female { get; set; } = true;
        public bool IsAnonymous { get; set; } = true;

    }

    public class SearchByBBox : AbstractModel
    {
        public float LatMin { get; set; }
        public float LongMin { get; set; }
        public float LatMax { get; set; }
        public float LongMax { get; set; }
    }

}
