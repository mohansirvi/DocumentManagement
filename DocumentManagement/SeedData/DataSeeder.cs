using Bogus;
using DocumentManagement.Data;
using DocumentManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace DocumentManagement.SeedData
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (context.Users.Any() || context.Documents.Any())
                return;

            var roles = new[] { "admin", "editor", "viewer" };
            var passwordHash = "hashedpassword123";

            var userFaker = new Faker<User>()
                .RuleFor(u => u.Username, f => f.Internet.UserName())
                .RuleFor(u => u.PasswordHash, _ => passwordHash)
                .RuleFor(u => u.Role, f => f.PickRandom(roles));

            var users = userFaker.Generate(1000);
            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();

            var userIds = await context.Users.Select(u => u.Id).ToListAsync();

            var docFaker = new Faker<Document>()
                .RuleFor(d => d.Title, f => f.Lorem.Sentence())
                .RuleFor(d => d.Content, f => f.Lorem.Paragraph())
                .RuleFor(d => d.CreatedAt, f => f.Date.Past().ToUniversalTime())
                .RuleFor(d => d.UserId, f => f.PickRandom(userIds));

            var documents = docFaker.Generate(100000);
            await context.Documents.AddRangeAsync(documents);
            await context.SaveChangesAsync();
        }
    }
}
