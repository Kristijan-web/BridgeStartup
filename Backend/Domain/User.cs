using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class User: Entity   
    {
        // Koje kolone ce imati user?
        // - Username
        // - Password
        // - Email

        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

        // Koga referencira User? Navigation Prop
        // - Role

        public long RoleId { get; set; }
        public virtual Role Role { get; set; }

        // Ko referencira usera? Hashset
        // - Post
        public virtual HashSet<Post> Posts { get; set; }



    }
}
