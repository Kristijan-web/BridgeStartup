using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Role : Entity
    {
        // Koja polja ce da ima role?
        // - Samo Name

        public string Name { get; set; }

        // Koga referencira Role?
        // - Nikoga

        // Ko referencira Role? HashSet
        // - User

        public virtual HashSet<User> Users{ get; set; }


    }
}
