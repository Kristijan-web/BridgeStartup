using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.Post
{
    public class ApplyToPostDTO
    {
        
        
            public long UserId { get; set; }
            public long PostId { get; set; }

            // Ispod su polja izvucena iz fajla
            public string FileName { get; set; }
            public string ContentType { get; set; }
            public long FileLength { get; set; }
            public Stream FileStream { get; set; } // sadrzaj fajla
        
    }
}
