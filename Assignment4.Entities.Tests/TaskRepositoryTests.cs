using System;
using Assignment4.Entities;
using Xunit;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Assignment4.Core;
using Assignment4;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace Assignment4.Entities.Tests
{
    public class TaskRepositoryTests
    {

        [Fact]
        public void test()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddUserSecrets<TaskRepositoryTests>()
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("Kanban");
            var repository = new TaskRepository(new SqlConnection(connectionString));
        
            var task1 = new TaskDTO {
                Id = 1,
                Title = "Code",
                Description = "Code some things",
                AssignedToId = 1,
                Tags = new List<string>() {"Urgent", "C#"}.AsReadOnly(),
                State = State.New
            };

            repository.Create(task1);

            var returnedTask = repository.FindById(1);

            Assert.Equal(task1.Id, returnedTask.Id);
        }
    }
}
