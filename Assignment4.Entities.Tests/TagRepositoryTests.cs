using System;
using System.Collections.Generic;
using Assignment4.Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Assignment4.Entities.Tests
{
    public class TagRepositoryTests : IDisposable
    {
        private readonly KanbanContext _context;
        private readonly TagRepository _repo;

        public TagRepositoryTests()
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

            context.Tags.Add(new Tag { Name = "Bug" });
            context.Tags.Add(new Tag { Name = "Feature" });
            context.Tags.Add(new Tag { Name = "On hold" });
            context.Tags.Add(new Tag { Name = "Frontend" });
            context.Tags.Add(new Tag { Name = "Backend" });
            context.SaveChanges();

            _context = context;
            _repo = new TagRepository(_context);
        }

        [Fact]
        public void Create_given_Tag_returns_tag_with_Id()
        {
            var tag = new TagCreateDTO { Name = "Urgent" };
            var created = _repo.Create(tag);

            Assert.Equal((Response.Created, 7), created);
        }

        [Fact]
        public void Read_given_non_existing_id_returns_null()
        {
            var tag = _repo.Read(42);
            Assert.Null(tag);
        }

        [Fact]
        public void Read_given_existing_id_returns_tag()
        {
            var tag = _repo.Read(1);
            Assert.Equal(new TagDTO(1, "Bug"), tag);
        }

        [Fact]
        public void ReadAll_returns_all_tags()
        {
            var allTags = _repo.ReadAll();

            Assert.Collection(allTags,
                c => Assert.Equal(new TagDTO(1, "Bug"), c),
                c => Assert.Equal(new TagDTO(2, "Feature"), c),
                c => Assert.Equal(new TagDTO(3, "On hold"), c),
                c => Assert.Equal(new TagDTO(4, "Frontend"), c),
                c => Assert.Equal(new TagDTO(5, "Backend"), c)
            );
        }

        [Fact]
        public void Trying_to_delete_unassigned_without_force_returns_deleted()
        {
            var response = _repo.Delete(2, false);
            Assert.Equal(Response.Deleted, response);
        }

        [Fact]
        public void Trying_to_delete_unassigned_with_force_returns_deleted()
        {
            var response = _repo.Delete(2, true);
            Assert.Equal(Response.Deleted, response);
        }

        [Fact]
        public void Trying_to_delete_assigned_without_force_returns_conflict()
        {
            var response = _repo.Delete(1, false);
            Assert.Equal(Response.Conflict, response);
        }

        [Fact]
        public void Trying_to_delete_assigned_with_force_returns_deleted()
        {
            var response = _repo.Delete(1, true);
            Assert.Equal(Response.Deleted, response);
        }

        [Fact]
        public void update()
        {

        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
