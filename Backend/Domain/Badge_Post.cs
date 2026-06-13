using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Badge_Post: Entity
    {
        // Ovde ce biti kompozitni kljuc

        // Koga referencira Badge_Post? -> Navigation Prop
        // - Badge
        // - Post

        public long BadgeId { get; set; }
        public virtual Badge Badge{ get; set; }

        public long PostId { get; set; }
        public virtual Post Post { get; set; }

        // Ko referencira Badge_Post?
        // - Niko
    }
}
