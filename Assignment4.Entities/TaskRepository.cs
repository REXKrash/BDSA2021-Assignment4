using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using Assignment4.Core;
using Microsoft.Data.SqlClient;

namespace Assignment4.Entities
{
    public class TaskRepository : ITaskRepository
    {
        public (Response Response, int TaskId) Create(TaskCreateDTO task)
        {
            throw new NotImplementedException();
        }

        public Response Delete(int taskId)
        {
            throw new NotImplementedException();
        }

        public TaskDetailsDTO Read(int taskId)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<TaskDTO> ReadAll()
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<TaskDTO> ReadAllByState(State state)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<TaskDTO> ReadAllByTag(string tag)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<TaskDTO> ReadAllByUser(int userId)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<TaskDTO> ReadAllRemoved()
        {
            throw new NotImplementedException();
        }

        public Response Update(TaskUpdateDTO task)
        {
            throw new NotImplementedException();
        }
    }
}
