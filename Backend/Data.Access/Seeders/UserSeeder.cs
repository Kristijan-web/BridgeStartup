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


            // napravi user-a sa mojim email-om i sifrom koji je validiran

            List<User> users = userFaker.Generate(10);

            // treba da napravim user objekat i da ga dodam na users
            // - Mora da ACtivatedAt ima datum
            // - ACtivation code da bude null
            User adminUser = new User
            {
                Username = "Kiki",
                Password = "$2y$10$yR3YZbC/dYTxTGUlFANOFOs8GubWG0N.SAZBXgIIff3YY1iu5ZVPi",
                Email = "kristijankiki884@gmail.com",
                RoleId = 2,
                ActivatedAt = DateTime.Now

            };

            users.Add(adminUser);

            _context.AddRange(users);
            _context.SaveChanges();




        }
    }
}
