using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Users.Dtos;
using Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Infrastructure.Data.LocalDb;

public class LocalDbDataManager : IDataHandler<User>
{
    private readonly string _stringConnection;

    public LocalDbDataManager(IConfiguration configuration)
    {
        _stringConnection = configuration.GetConnectionString("ApiUserDataContext");
        CreateUserTableIfNotExists();
    }

    private void CreateUserTableIfNotExists()
    {
        using (var connection = new SqlConnection(_stringConnection))
        {
            if (!UserTableQueryHelper.IsTableCreated("USERS", connection))
            {
                UserTableQueryHelper.CreateTable(connection);
            }
        }
    }

    public Tuple<int, IEnumerable<User>> Get(UserFilter userFilter)
    {
        using (var connection = new SqlConnection(_stringConnection))
        {
            IList<User> users = new List<User>();
            int totalRows = 0;
            var query = UserTableQueryHelper.QueryUserData(userFilter);
            var currentPage = (userFilter.PageIndex - 1) * userFilter.PageSize;
            using (var dataAdapter = new SqlDataAdapter(query, connection))
            {
                var dataset = new DataSet();
                dataAdapter.Fill(dataset, currentPage, userFilter.PageSize, "USERS");
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

            return new Tuple<int, IEnumerable<User>>(totalRows, users);
        }
    }

    public User Save(User data)
    {
        using (var connection = new SqlConnection(_stringConnection))
        {
            var user = UserTableQueryHelper.InsertUser(data, connection);

            return user;
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
