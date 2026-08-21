using Application;
using Application.DTO.Post;
using Application.DTO.User;
using Application.Queries;
using Data.Access;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Implementation.Queries.Users
{
    public class EfGetUserPosts : IGetUserPostsQuery
    {
        public string Id => "get-user-posts"; // -> odem u bazu i napravim usecase sa OVIM imenom

        public string Name => "getting all posts for this user";

        private ApplicationDbContext _context;

        private IApplicationUser _user;
        public EfGetUserPosts(ApplicationDbContext context, IApplicationUser user)
        {
            _context = context;
            _user = user;
        }

        public GetUserPostsDTO Execute(int dto)
        {
            // Sta treba da uradim?
            // - Da dohvatim post-ove za prosledjen id korisnika

            GetUserPostsDTO userPosts = _context.Users
          .Where(x => x.Id == dto)
          .Select(x => new GetUserPostsDTO
          {
              UserId = x.Id,
              Email = x.Email,
              Username = x.Username,

              Posts = x.Posts.Select(y => new PostsDTO
              {
                  Title = y.Title,

              }).ToList()
          })
          .FirstOrDefault();

            return userPosts;




        }
    }
}
