using Bogus;
using Domain;

namespace Data.Access.Seeders
{
    public class UserSeeder
    {
        public UserSeeder(ApplicationDbContext _context)
        {
            List<Role> roles = _context.Roles.ToList();



            Faker<User> userFaker = new Faker<User>();

            userFaker.RuleFor(x => x.Username, f => f.Internet.UserName());
            userFaker.RuleFor(x => x.Password, f => f.Internet.Password());
            userFaker.RuleFor(x => x.Email, f => f.Internet.Email());
            userFaker.RuleFor(x => x.Role, f => f.PickRandom(roles));
            userFaker.RuleFor(x => x.ActivatedAt, f => f.Date.Recent(30));
            userFaker.RuleFor(x => x.ActivationCode, f => f.Random.Guid().ToString());

            List<User> users = userFaker.Generate(10);

            _context.AddRange(users);
            _context.SaveChanges();




        }
    }
}
