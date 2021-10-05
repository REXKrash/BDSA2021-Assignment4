using System;
using System.IO;
using Assignment4.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
<<<<<<< HEAD:Assignment4/KanbanContextFactory.cs
using Assignment4.Core;
using System.Collections.Generic;
=======
using Assignment4.Entities;
>>>>>>> 34bf0bcdd10a041627c2fd3de5207aa176a78b7a:Assignment4/KanbanContextFactory

namespace Assignment4
{
    public class KanbanContextFactory : IDesignTimeDbContextFactory<KanbanContext>
    {
        public KanbanContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddUserSecrets<Program>()
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("Kanban");

            var optionsBuilder = new DbContextOptionsBuilder<KanbanContext>()
                .UseSqlServer(connectionString);

            return new KanbanContext(optionsBuilder.Options);
        }

        public static void Seed(KanbanContext context)
        {
            context.Database.ExecuteSqlRaw("DELETE dbo.Tasks");
            context.Database.ExecuteSqlRaw("DELETE dbo.TasksTags");
            context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('dbo.Tasks', RESEED, 0)");
            context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('dbo.TasksTags', RESEED, 0)");

            var task1 = new TaskDTO {
                Id = 1,
                Title = "Code",
                Description = "Code some things",
                AssignedToId = 1,
                Tags = new List() {"Urgent", "C#"}.AsReadOnly(),
                State = State.New
            };
            context.Tasks.AddRange(
                task1
            );

            context.SaveChanges();
        }
    }
}
