using Application.Users.Dtos;
using Domain.Entities;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;

namespace Infrastructure.Data.LocalDb;

internal class UserTableQueryHelper
{
    private const string TABLE_NAME = "USERS";
    private const string INSERT_QUERY = @$"
            INSERT INTO {TABLE_NAME} (
                first_name,
                last_name,
                age,
                date,
                country,
                province,
                city
            )";

    public static bool IsTableCreated(string tableName, IDbConnection connection)
    {
        var query = $"SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = '{tableName}'";
        using (IDbCommand command = new SqlCommand(query))
        {
            command.Connection = connection;
            if (connection.State == ConnectionState.Closed) connection.Open();

            var firstColumn = command.ExecuteScalar();

            return firstColumn is not null;
        }
    }

    public static bool CreateTable(IDbConnection connection)
    {
        var query = @$"
            CREATE TABLE {TABLE_NAME} (
                id INT PRIMARY KEY IDENTITY,
                first_name VARCHAR(50),
                last_name VARCHAR(50),
                age INT,
                date DATETIME2,
                country VARCHAR(50),
                province VARCHAR(50),
                city VARCHAR(50)
            );
            CREATE INDEX {TABLE_NAME}_age_country ON {TABLE_NAME} (age, country);
        ";
        using (IDbCommand command = new SqlCommand(query))
        {
            command.Connection = connection;
            if (connection.State == ConnectionState.Closed) connection.Open();

            command.ExecuteNonQuery();
        }

        return true;
    }

    public static User InsertUser(User user, IDbConnection connection)
    {
        var query = @$"
            {INSERT_QUERY}
            OUTPUT Inserted.ID
            VALUES (
                '{user.FirstName}',
                '{user.LastName}',
                {user.Age},
                '{user.Date}',
                '{user.Country}',
                '{user.Province}',
                '{user.City}'
            );
        ";
        using (IDbCommand command = new SqlCommand(query))
        {
            command.Connection = connection;
            if (connection.State == ConnectionState.Closed) connection.Open();

            var lastInsertedId = command.ExecuteScalar();
            user.Id = (int)lastInsertedId;
        }

        return user;
    }
    
    public static bool BulkUserDataFrom(string absoluteFileTempDir, IDbConnection connection)
    {
        var query = @$"
            BULK INSERT {TABLE_NAME}
            FROM '{absoluteFileTempDir}'
            WITH (FIELDTERMINATOR = ',', ROWTERMINATOR = '\n', FIRSTROW = 2)
        ";

        using (IDbCommand command = new SqlCommand(query))
        {
            command.Connection = connection;
            if (connection.State == ConnectionState.Closed) connection.Open();

            var rowsAffected = command.ExecuteNonQuery();

            return rowsAffected > 0;
        }
    }

    public static StringBuilder PrepareQueryToBulkLoadUserInfoFromStream(Stream fileStream)
    {
        var streamReader = new StreamReader(fileStream);
        var stringBuilder = new StringBuilder($"{INSERT_QUERY} VALUES ");
        string? row;

        while ((row = streamReader.ReadLine()) != null)
        {
            var rowValues = row.Split(',')
                .Select(value => int.TryParse(value, out _) ? value : $"'{value.Replace("'", "''")}'");

            stringBuilder.Append($"({string.Join(",", rowValues)}){(streamReader.EndOfStream ? "" : ",")}");
        }

        return stringBuilder;
    }
    
    public static string QueryUserData(UserFilter filter)
    {
        var whereCondictions = new List<string>();
        if (filter.Age.HasValue)
        {
            whereCondictions.Add($"age = {filter.Age.Value}");
        }
        if (!string.IsNullOrEmpty(filter.Country))
        {
            whereCondictions.Add($"country = '{filter.Country}'");
        }
        if (!string.IsNullOrEmpty(filter.Province))
        {
            whereCondictions.Add($"province = '{filter.Province}'");
        }
        if (!string.IsNullOrEmpty(filter.City))
        {
            whereCondictions.Add($"city = '{filter.City}'");
        }
        if (!string.IsNullOrEmpty(filter.FirstName))
        {
            whereCondictions.Add($"first_name = '{filter.FirstName}'");
        }
        if (!string.IsNullOrEmpty(filter.LastName))
        {
            whereCondictions.Add($"last_name = '{filter.LastName}'");
        }

        var where = whereCondictions.Count > 0 ? $"WHERE {string.Join(" AND ", whereCondictions)}" : "";

        var query = $@"
            DECLARE @TotalItems AS INT
            SELECT @TotalItems = COUNT(*) FROM {TABLE_NAME} {where}

            SELECT id,first_name,last_name,age,date,country,province,city, @TotalItems as totalItems
            FROM {TABLE_NAME}
            {where}
            ORDER BY first_name, last_name
            /*OFFSET {(filter.PageIndex - 1) * filter.PageSize} ROWS
            FETCH NEXT {filter.PageSize} ROWS ONLY*/
        ";

        return query;
    }
}
