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
        private readonly SqlConnection _connection;

        public TaskRepository(SqlConnection connection)
        {
            _connection = connection;
        }

        public IReadOnlyCollection<TaskDTO> All()
        {
            var cmdText = @"SELECT t.Id, t.Title, t.AssignedToid, t.Description, t.State
                            FROM dbo.Tasks AS t";
            using var command = new SqlCommand(cmdText, _connection);

            OpenConnection();

            using var reader = command.ExecuteReader();
            var list = new List<TaskDTO>();
            while (reader.Read())
            {
                list.Add(new TaskDTO
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    AssignedToId = reader.GetInt32(2),
                    Description = reader.GetString(3),
                    State = Enum.Parse<State>(reader.GetString(4), true),
                }
                );
            }
            CloseConnection();
            return new ReadOnlyCollection<TaskDTO>(list);
        }

        public int Create(TaskDTO task)
        {
            var cmdText = @"INSERT dbo.Tasks (Title, Description, AssignedToId, State)
                            VALUES (@Title, @Description, @AssignedToId, @State);
                            SELECT SCOPE_IDENTITY()";

            using var command = new SqlCommand(cmdText, _connection);

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
            var cmdText = @"DELETE dbo.Tasks WHERE Id = @Id";

            using var command = new SqlCommand(cmdText, _connection);

            command.Parameters.AddWithValue("@Id", taskId);

            OpenConnection();

            command.ExecuteNonQuery();

            CloseConnection();
        }

        public TaskDetailsDTO FindById(int id)
        {
            var cmdText = @"SELECT t.Id, t.Title, t.AssignedToid, t.Description, t.State, u.name, u.email
                            FROM dbo.Tasks AS t
                            JOIN dbo.Users AS u ON t.AssignedToid = u.id
                            WHERE t.Id = @Id";
            using var command = new SqlCommand(cmdText, _connection);

            command.Parameters.AddWithValue("@Id", id);
            OpenConnection();

            using var reader = command.ExecuteReader();
            var task = reader.Read() ? new TaskDetailsDTO
            {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1),
                AssignedToId = reader.GetInt32(2),
                Description = reader.GetString(3),
                State = Enum.Parse<State>(reader.GetString(4), true),
                AssignedToName = reader.GetString(5),
                AssignedToEmail = reader.GetString(6)

            } : null;
            CloseConnection();

            return task;
        }

        public void Update(TaskDTO task)
        {
            var cmdText = @"UPDATE dbo.Tasks SET
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
