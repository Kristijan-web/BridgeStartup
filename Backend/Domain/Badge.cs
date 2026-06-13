using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Badge: Entity
    {
        public string Name { get; set; }


        // Ko referencira Badge? -> HashSet
        // - Badge_Post

        public virtual HashSet<Badge_Post> BadgePosts{ get; set; }


        // Koga referencira Badge?
        // - Nikoga
    }
}
