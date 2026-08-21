using Application.DTO.Post;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.User
{
    public class GetUserPostsDTO
    {
        // user id
        public long UserId { get; set; }
        // Kako ce izgledati struktura DTO-a?
        // Koje su mi potrebne kolone?

        // imacu user objekat sa nizom post-ova

        public string Email { get; set; }
        public string Username { get; set; }

        public IEnumerable<PostsDTO> Posts { get; set; } = new List<PostsDTO>();
    }
}
