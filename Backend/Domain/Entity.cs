using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Entity
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

    }
}
