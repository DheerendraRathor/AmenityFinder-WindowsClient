using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmenityFinder.models
{
    public class Post: AbstractModel
    {
        public int Id { get; set; }
        public int Location { get; set; }
        public string Comment { get; set; }
        public float Rating { get; set; }
        public User User { get; set; }
        public bool IsAnonymous { get; set; }
        public int Upvotes { get; set; }
        public int Downvotes { get; set; }
        public DateTime Created { get; set; }

    }

    public class NewPost : AbstractModel
    {
        public int Location { get; set; }
        public string Commnet { get; set; }
        public int Rating { get; set; }
        public bool IsAnonymous { get; set; } = true;
    }

}
