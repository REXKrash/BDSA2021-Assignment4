using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Assignment4.Core;

namespace Assignment4.Entities
{
    public class TaskRepository : ITaskRepository
    {
        private readonly KanbanContext _context;

        public TaskRepository(KanbanContext context)
        {
            _context = context;
        }

        public (Response Response, int TaskId) Create(TaskCreateDTO task)
        {
            var user = _context.Users.Find(task.AssignedToId);
            if (user == null)
            {
                return (Response.BadRequest, -1);
            }

            var entity = new Task
            {
                Title = task.Title,
                Description = task.Description,
                AssignedTo = user,
                Tags = task.Tags.Select(t => new Tag { Name = t }).ToList(),
                State = State.New,
                Created = DateTime.Now,
                StatusUpdated = DateTime.Now
            };

            _context.Tasks.Add(entity);
            _context.SaveChanges();

            return (Response.Created, entity.Id);
        }

        public Response Delete(int taskId)
        {
            var entity = _context.Tasks.Find(taskId);

            if (entity == null)
            {
                return Response.NotFound;
            }
            if (entity.State == State.New)
            {
                _context.Tasks.Remove(entity);
                _context.SaveChanges();

                return Response.Deleted;
            }
            else if (entity.State == State.Active)
            {
                entity.State = State.Removed;
                _context.SaveChanges();
                return Response.Updated;
            }
            else
            {
                return Response.Conflict;
            }
        }

        public TaskDetailsDTO Read(int taskId)
        {
            var tasks = from t in _context.Tasks
                        where t.Id == taskId
                        select new TaskDetailsDTO(
                            t.Id,
                            t.Title,
                            t.Description,
                            t.Created,
                            t.AssignedTo.Name,
                            t.Tags.Select(t => t.Name).ToList().AsReadOnly(),
                            t.State,
                            t.StatusUpdated);

            return tasks.FirstOrDefault();
        }

        public IReadOnlyCollection<TaskDTO> ReadAll()
        {
            return _context.Tasks.Select(t => new TaskDTO(
                t.Id,
                t.Title,
                t.AssignedTo.Name,
                t.Tags.Select(t => t.Name).ToList().AsReadOnly(),
                t.State)).ToList().AsReadOnly();
        }

        public IReadOnlyCollection<TaskDTO> ReadAllByState(State state)
        {
            return _context.Tasks.Where(t => t.State == state).Select(t => new TaskDTO(
                t.Id,
                t.Title,
                t.AssignedTo.Name,
                t.Tags.Select(t => t.Name).ToList().AsReadOnly(),
                t.State)).ToList().AsReadOnly();
        }

        public IReadOnlyCollection<TaskDTO> ReadAllByTag(string tag)
        {
            return _context.Tasks.Where(t => t.Tags.Select(ta => ta.Name).ToList().Contains(tag)).Select(t => new TaskDTO(
                t.Id,
                t.Title,
                t.AssignedTo.Name,
                t.Tags.Select(t => t.Name).ToList().AsReadOnly(),
                t.State)).ToList().AsReadOnly();
        }

        public IReadOnlyCollection<TaskDTO> ReadAllByUser(int userId)
        {
            return _context.Tasks.Where(t => t.AssignedTo.Id == userId).Select(t => new TaskDTO(
                t.Id,
                t.Title,
                t.AssignedTo.Name,
                t.Tags.Select(t => t.Name).ToList().AsReadOnly(),
                t.State)).ToList().AsReadOnly();
        }

        public IReadOnlyCollection<TaskDTO> ReadAllRemoved()
        {
            return ReadAllByState(State.Removed);
        }

        public Response Update(TaskUpdateDTO task)
        {
            var entity = _context.Tasks.Find(task.Id);

            if (entity == null)
            {
                return Response.NotFound;
            }
            entity.Id = task.Id;
            entity.State = task.State;
            entity.Tags = task.Tags.Select(t => new Tag { Name = t }).ToList();
            entity.StatusUpdated = DateTime.Now;

            _context.SaveChanges();

            return Response.Updated;
        }
    }
}
