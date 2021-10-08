using System;
using Microsoft.EntityFrameworkCore;

namespace Assignment4.Entities
{
    public interface IKanbanContext : IDisposable
    {
        DbSet<Task> Tasks { get; }
        DbSet<Tag> Tags { get; }
        DbSet<User> Users { get; }
        int SaveChanges();
    }
}
