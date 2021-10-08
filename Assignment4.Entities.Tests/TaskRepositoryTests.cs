using System;
using System.Collections.Generic;
using Assignment4.Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Assignment4.Entities.Tests
{
    public class TaskRepositoryTests : IDisposable
    {
        private readonly KanbanContext _context;
        private readonly TaskRepository _repo;

        public TaskRepositoryTests()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            var builder = new DbContextOptionsBuilder<KanbanContext>();
            builder.UseSqlite(connection);
            var context = new KanbanContext(builder.Options);
            context.Database.EnsureCreated();

            var user = new User { Id = 1, Name = "Paolo", Email = "paolo@google.com" };
            context.Users.Add(user);
            context.Tasks.Add(new Task { Title = "Setup project", AssignedTo = user, Tags = new List<Tag> { new Tag { Name = "Bug" } } });
            context.SaveChanges();

            _context = context;
            _repo = new TaskRepository(_context);
        }

        [Fact]
        public void Create_given_Task_returns_task_with_Title()
        {
            var task = new TaskCreateDTO { Title = "Do some coding", Tags = new List<string> { "Bug" }, AssignedToId = 1 };
            var created = _repo.Create(task);

            Assert.Equal((Response.Created, 2), created);
        }

        [Fact]
        public void Read_given_non_existing_id_returns_null()
        {
            var task = _repo.Read(42);
            Assert.Null(task);
        }

        [Fact]
        public void Read_given_existing_id_returns_task()
        {
            //var task = _repo.Read(1);
            //Assert.Equal(new TaskDetailsDTO { Title = "Setup project" }, task);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
