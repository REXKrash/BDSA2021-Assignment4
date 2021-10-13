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
            context.Tasks.Add(new Task { Title = "Setup project", AssignedTo = user, Tags = new List<Tag> { new Tag { Name = "Urgent" } } });
            context.Tasks.Add(new Task { Title = "Add controllers", AssignedTo = user, Tags = new List<Tag> { new Tag { Name = "Issue" } } });
            context.Tasks.Add(new Task { Title = "Fix major bugs", AssignedTo = user, Tags = new List<Tag> { new Tag { Name = "Bug" } } });

            context.SaveChanges();

            _context = context;
            _repo = new TaskRepository(_context);
        }

        [Fact]
        public void Create_given_Task_returns_task_with_Title()
        {
            var task = new TaskCreateDTO { Title = "Do some coding", Tags = new List<string> { "Bug" }, AssignedToId = 1 };
            var created = _repo.Create(task);

            Assert.Equal((Response.Created, 4), created);
        }

        [Fact]
        public void Create_task_returns_bad_request()
        {
            var task = new TaskCreateDTO { Title = "Do some coding", Tags = new List<string> { "Bug" }, AssignedToId = 11 };
            var created = _repo.Create(task);

            Assert.Equal((Response.BadRequest, -1), created);
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
            var task = new TaskCreateDTO { Title = "Do some coding", Tags = new List<string> { "Bug" }, AssignedToId = 1 };
            var created = _repo.Create(task);
            var expected = DateTime.Now;

            var readTask = _repo.Read(4);

            Assert.Equal(4, readTask.Id);
            Assert.Equal("Do some coding", readTask.Title);
            Assert.Equal("Paolo", readTask.AssignedToName);
            Assert.Equal(State.New, readTask.State);

            Assert.Equal(expected, readTask.Created, precision: TimeSpan.FromSeconds(2));
            Assert.Equal(expected, readTask.StateUpdated, precision: TimeSpan.FromSeconds(2));

            Assert.Null(readTask.Description);
        }

        [Fact]
        public void readAll_returns_all_tasks()
        {
            var allTasks = _repo.ReadAll();
            Assert.Equal(3, allTasks.Count);
        }

        [Fact]
        public void ReadAllByState_returns_collection_containing_all_tasks_with_given_state()
        {
            _repo.Update(new TaskUpdateDTO { Id = 1, State = State.Active, Tags = new List<string> { "Urgent" } });

            Assert.Equal(1, _repo.ReadAllByState(State.Active).Count);
            Assert.Equal(2, _repo.ReadAllByState(State.New).Count);
            Assert.Equal(0, _repo.ReadAllByState(State.Closed).Count);
            Assert.Equal(0, _repo.ReadAllByState(State.Removed).Count);
            Assert.Equal(0, _repo.ReadAllByState(State.Resolved).Count);
        }

        [Fact]
        public void ReadAllByTag()
        {
            Assert.Equal(1, _repo.ReadAllByTag("Urgent").Count);
            Assert.Equal(1, _repo.ReadAllByTag("Issue").Count);
            Assert.Equal(1, _repo.ReadAllByTag("Bug").Count);
            Assert.Equal(0, _repo.ReadAllByTag("Random").Count);
        }

        [Fact]
        public void ReadAllByUser_returns_collection_containing_all_tasks_with_given_userId()
        {
            Assert.Equal(3, _repo.ReadAllByUser(1).Count);
            Assert.Equal(0, _repo.ReadAllByUser(100).Count);
        }

        [Fact]
        public void ReadAllRemoved()
        {
            Assert.Equal(0, _repo.ReadAllRemoved().Count);

            _repo.Update(new TaskUpdateDTO { Id = 1, State = State.Active, Tags = new List<string> { "Urgent" } });
            _repo.Delete(1);

            Assert.Equal(1, _repo.ReadAllRemoved().Count);
        }

        [Fact]
        public void Delete_returns_not_found()
        {
            var response = _repo.Delete(100);
            Assert.Equal(Response.NotFound, response);
        }

        [Fact]
        public void Delete_returns_successfully_deleted()
        {
            var response = _repo.Delete(1);
            Assert.Equal(Response.Deleted, response);
        }

        [Fact]
        public void Delete_returns_updated()
        {
            _repo.Update(new TaskUpdateDTO { Id = 1, State = State.Active, Tags = new List<string> { "Urgent" } });
            var response = _repo.Delete(1);
            Assert.Equal(Response.Updated, response);
        }

        [Fact]
        public void Delete_returns_conflict()
        {
            _repo.Update(new TaskUpdateDTO { Id = 1, State = State.Closed, Tags = new List<string> { "Urgent" } });
            var response = _repo.Delete(1);
            Assert.Equal(Response.Conflict, response);
        }

        [Fact]
        public void Update_returns_successfully_updated()
        {
            var response = _repo.Update(new TaskUpdateDTO { Id = 1, State = State.Active, Tags = new List<string> { "Urgent" } });
            Assert.Equal(Response.Updated, response);
        }

        [Fact]
        public void Update_returns_not_found()
        {
            var response = _repo.Update(new TaskUpdateDTO { Id = 100, State = State.Active, Tags = new List<string> { "Urgent" } });
            Assert.Equal(Response.NotFound, response);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
