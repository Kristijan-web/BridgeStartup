using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Post: Entity
    {
        // Koje kolone ce imati post
        // - Title
        // - Descriptions
        // - Email, preko kojeg ce ljudi kontaktirati -> OPCIONO
        // -  Broj telefona preko kojeg ce ljudi kontaktirati -> OPCIONO
        // - UserId
        // 

        public string Title { get; set; }
        public string Description { get; set; }

        public string? Email { get; set; }
        public string? Phone { get; set; }

        // Ko referencira Post? Hashset
        // - Badge_Post

        public virtual HashSet<Badge_Post> BadgePosts{ get; set; }

        // Koga referencira Post?
        // - User

        public long UserId { get; set; }
        public virtual User User { get; set; }



    }
}
