using Bogus;
using Domain;

namespace Data.Access.Seeders
{
    public class ContactSeeder
    {

        // Koje kolojne ima Contact
        // - Subject
        // - Message
        // - UserId

        public ContactSeeder(ApplicationDbContext _context)
        {

            // Za seeder mogu da koristim Bogus

            List<User> usersDb = _context.Users.ToList();

            Faker<Contact> ContactFaker = new Faker<Contact>();

            ContactFaker.RuleFor(x => x.Subject, f => f.Lorem.Sentence());
            ContactFaker.RuleFor(x => x.Message, f => f.Lorem.Paragraphs(2));
            ContactFaker.RuleFor(x => x.User, f => f.PickRandom(usersDb));

            List<Contact> FakeContacts = ContactFaker.Generate(10);

            _context.AddRange(FakeContacts);
            _context.SaveChanges();



        }
    }
}
