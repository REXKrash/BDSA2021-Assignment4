using System;
using System.Collections.Generic;
using System.Data;
using Assignment4.Core;
using Microsoft.Data.SqlClient;

namespace Assignment4.Entities
{
    public class TaskRepository : ITaskRepository
    {
        private readonly SqlConnection _connection;

        public TaskRepository(SqlConnection connection)
        {
            _connection = connection;
        }

        public IReadOnlyCollection<TaskDTO> All()
        {
            


            throw new System.NotImplementedException();
        }

        public int Create(TaskDTO task)
        {
            var cmdText = @"INSERT Task (Id, Title, Description, AssignedToId, State)
                            VALUES (@Id, @Title, @Description, @AssignedToId, @State);
                            SELECT SCOPE_IDENTITY()";

            using var command = new SqlCommand(cmdText, _connection);

            command.Parameters.AddWithValue("@Id", task.Id);
            command.Parameters.AddWithValue("@Title", task.Title);
            command.Parameters.AddWithValue("@Description", task.Description);
            command.Parameters.AddWithValue("@AssignedToId", task.AssignedToId);
            command.Parameters.AddWithValue("@State", task.State.ToString());

            OpenConnection();
            var id = command.ExecuteScalar();
            CloseConnection();

            return (int)id;
        }

        public void Delete(int taskId)
        {
            var cmdText = @"DELETE Task WHERE Id = @Id";

            using var command = new SqlCommand(cmdText, _connection);

            command.Parameters.AddWithValue("@Id", taskId);

            OpenConnection();

            command.ExecuteNonQuery();

            CloseConnection();
        }

        public TaskDetailsDTO FindById(int id)
        {
            var cmdText = @"SELECT t.Id, t.Title, t.Description, t.AssignedToId, t.State
                            FROM Tasks t
                            WHERE t.Id = @Id";

            using var command = new SqlCommand(cmdText, _connection);

            command.Parameters.AddWithValue("@Id", id);

            OpenConnection();

            using var reader = command.ExecuteReader();

            var task = reader.Read()
                ? new TaskDetailsDTO
                {
                    Id = reader.GetInt32("Id"),
                    Title = reader.GetString("Title"),
                    Description = reader.GetString("Description"),
                    AssignedToId = reader.GetInt32("AssignedToId"),
                    State = Enum.Parse<State>(reader.GetString("State"))
                }
                : null;

            CloseConnection();
            return task;
        }

        public void Update(TaskDTO task)
        {
            var cmdText = @"UPDATE Tasks SET
                            Id = @Id,
                            Title = @Title,
                            Description = @Description,
                            AssignedToId = @AssignedToId,
                            State = @State
                            WHERE Id = @Id";

            using var command = new SqlCommand(cmdText, _connection);

            command.Parameters.AddWithValue("@Id", task.Id);
            command.Parameters.AddWithValue("@Title", task.Title);
            command.Parameters.AddWithValue("@Description", task.Description);
            command.Parameters.AddWithValue("@AssignedToId", task.AssignedToId);
            command.Parameters.AddWithValue("@State", task.State.ToString());

            OpenConnection();

            command.ExecuteNonQuery();

            CloseConnection();
        }

        private void OpenConnection()
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
        }

        private void CloseConnection()
        {
            if (_connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
