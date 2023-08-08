using Application.Users.Dtos;
using Domain.Entities;
using Microsoft.Data.SqlClient;
using System.Data;
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

    public static string IsTableCreatedQuery()
    {
        var query = @$"SELECT * FROM INFORMATION_SCHEMA.TABLES
        WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = '{TABLE_NAME}'";

        return query;
    }

    public static string CreateTableQuery()
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

        return query;
    }

    public static string PrepareInsertUserQuery(User user)
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

        return query;
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
    
    public static string UserDataQuery(UserFilter? filter)
    {
        var whereCondictions = new List<string>();
        if (filter != null && filter.Age.HasValue)
        {
            whereCondictions.Add($"age = {filter.Age.Value}");
        }
        if (filter != null && !string.IsNullOrEmpty(filter.Country))
        {
            whereCondictions.Add($"country = '{filter.Country}'");
        }
        if (filter != null && !string.IsNullOrEmpty(filter.Province))
        {
            whereCondictions.Add($"province = '{filter.Province}'");
        }
        if (filter != null && !string.IsNullOrEmpty(filter.City))
        {
            whereCondictions.Add($"city = '{filter.City}'");
        }
        if (filter != null && !string.IsNullOrEmpty(filter.FirstName))
        {
            whereCondictions.Add($"first_name = '{filter.FirstName}'");
        }
        if (filter != null && !string.IsNullOrEmpty(filter.LastName))
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
            {(filter == null || whereCondictions.Count > 0 ? ""
               : $"OFFSET {(filter.PageIndex - 1) * filter.PageSize} ROWS FETCH NEXT {filter.PageSize} ROWS ONLY")}
        ";

        return query;
    }
}
