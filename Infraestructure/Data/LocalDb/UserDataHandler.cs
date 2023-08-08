using Application.Common.Interfaces;
using Application.Users.Dtos;
using Core.Models;
using Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Infrastructure.Data.LocalDb;

public class UserDataHandler : IDataHandler<User>
{
    private readonly string _stringConnection;

    public UserDataHandler(IConfiguration configuration)
    {
        _stringConnection = configuration.GetConnectionString("ApiUserDataContext");
        CreateUserTableIfNotExists();
    }

    private void CreateUserTableIfNotExists()
    {
        using (var connection = new SqlConnection(_stringConnection))
        {
            var query = UserTableQueryHelper.IsTableCreatedQuery();
            using (IDbCommand command = new SqlCommand(query))
            {
                command.Connection = connection;
                if (connection.State == ConnectionState.Closed) connection.Open();

                var firstColumn = command.ExecuteScalar();
                if (firstColumn is null)
                {
                    command.CommandText = UserTableQueryHelper.CreateTableQuery();
                    command.ExecuteNonQuery();
                }
            }
        }
    }

    public DataResum<User> Get(UserFilter userFilter)
    {
        using (var connection = new SqlConnection(_stringConnection))
        {
            var users = new List<User>();
            int totalRows = 0;
            var query = UserTableQueryHelper.UserDataQuery(userFilter);
            using (var command = new SqlCommand(query))
            {
                command.Connection = connection;
                if (connection.State == ConnectionState.Closed) connection.Open();

                var dataReader = command.ExecuteReader();

                do
                {
                    while (dataReader.Read())
                    {
                        totalRows = dataReader.GetInt32(dataReader.GetOrdinal("totalItems"));
                        users.Add(new User
                        {
                            Id = dataReader.GetInt32(dataReader.GetOrdinal("id")),
                            Age = dataReader.GetInt32(dataReader.GetOrdinal("age")),
                            FirstName = dataReader.GetString(dataReader.GetOrdinal("first_name")),
                            LastName = dataReader.GetString(dataReader.GetOrdinal("last_name")),
                            Date = dataReader.GetDateTime(dataReader.GetOrdinal("date")),
                            Country = dataReader.GetString(dataReader.GetOrdinal("country")),
                            City = dataReader.GetString(dataReader.GetOrdinal("city")),
                            Province = dataReader.GetString(dataReader.GetOrdinal("province"))
                        });
                    }
                } while (dataReader.NextResult());

                return new DataResum<User>
                {
                    Items = users,
                    TotalItems = totalRows
                };
            }

            /*// var currentPage = (userFilter.PageIndex - 1) * userFilter.PageSize;
            using (var dataAdapter = new SqlDataAdapter(query, connection))
            {
                var dataset = new DataSet();
                dataAdapter.Fill(dataset*//*, currentPage, userFilter.PageSize, "USERS"*//*);
                var userTable = dataset.Tables[0];
                foreach (DataRow row in userTable.Rows)
                {
                    totalRows = row.Field<int>("totalItems");
                    users.Add(new User
                    {
                        Id = row.Field<int>("id"),
                        Age = row.Field<int>("age"),
                        FirstName = row.Field<string>("first_name"),
                        LastName = row.Field<string>("last_name"),
                        Date = row.Field<DateTime>("date"),
                        Country = row.Field<string>("country"),
                        City = row.Field<string>("city"),
                        Province = row.Field<string>("province")
                    });
                }
            }

            return new Tuple<int, IEnumerable<User>>(totalRows, users);*/
        }
    }

    public User Save(User data)
    {
        using (var connection = new SqlConnection(_stringConnection))
        {
            var query = UserTableQueryHelper.PrepareInsertUserQuery(data);

            using (IDbCommand command = new SqlCommand(query))
            {
                command.Connection = connection;
                if (connection.State == ConnectionState.Closed) connection.Open();

                var lastInsertedId = command.ExecuteScalar();
                data.Id = (int)lastInsertedId;
            }

            return data;
        }
    }

    public Task<bool> BulkUserDataFromFile(Stream fileStream)
    {
        return Task.Factory.StartNew(() =>
        {
            using (var connection = new SqlConnection(_stringConnection))
            {
                using (IDbCommand command = new SqlCommand())
                {
                    var queryBuilded = UserTableQueryHelper.PrepareQueryToBulkLoadUserInfoFromStream(fileStream);
                    command.Connection = connection;
                    command.CommandText = queryBuilded.ToString();
                    if (connection.State == ConnectionState.Closed) connection.Open();

                    queryBuilded.Clear();
                    var rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        });
    }
}
