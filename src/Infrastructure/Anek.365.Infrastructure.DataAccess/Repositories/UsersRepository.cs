using Anek._365.Application.Abstractions.Queries.Users;
using Anek._365.Application.Models;
using Itmo.Dev.Platform.Postgres.Connection;
using Itmo.Dev.Platform.Postgres.Extensions;
using Npgsql;
using System.Runtime.CompilerServices;

namespace Anek._365.Application.Abstractions.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly IPostgresConnectionProvider _connectionProvider;

    public UsersRepository(IPostgresConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async IAsyncEnumerable<UserWithCount> GetWithViews(
        UsersQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
            select u.user_id            as user_id
                 , u.name               as name
                 , u.standard_name      as standard_name
                 , count(*)             as count
              from users as u
        inner join aneks as a
                on u.user_id = a.user_id
          group by u.user_id, u.name, u.email
            having count(*) != 0
          order by u
            offset :offset
             limit :limit
        """;

        NpgsqlConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using NpgsqlCommand command = new NpgsqlCommand(sql, connection)
            .AddParameter("offset", query.PageSize * (query.PageNumber - 1))
            .AddParameter("limit", query.PageSize);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        int tagId = reader.GetOrdinal("user_id");
        int name = reader.GetOrdinal("name");
        int standardName = reader.GetOrdinal("standard_name");
        int count = reader.GetOrdinal("count");

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new UserWithCount(
                reader.GetInt32(tagId),
                reader.GetString(name),
                reader.GetInt32(count),
                reader.GetString(standardName));
        }
    }

    public async Task<User?> GetById(int id, CancellationToken cancellationToken)
    {
        const string sql = """
        select u.user_id                as user_id
             , u.name                   as name
             , u.standard_name          as standard_name
             , u.email                  as email
          from users as u
         where user_id = :user_id
        """;

        NpgsqlConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using NpgsqlCommand command = new NpgsqlCommand(sql, connection)
            .AddParameter("user_id", id);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        int userId = reader.GetOrdinal("user_id");
        int name = reader.GetOrdinal("name");
        int standardName = reader.GetOrdinal("standard_name");
        int email = reader.GetOrdinal("email");

        while (await reader.ReadAsync(cancellationToken))
        {
            return new User(
                reader.GetInt32(userId),
                reader.GetString(name),
                reader.GetString(email),
                reader.GetString(standardName));
        }

        return null;
    }

    public async Task<User?> GetByEmail(string email, CancellationToken cancellationToken)
    {
        const string sql = """
        select u.user_id                as user_id
             , u.name                   as name
             , u.standard_name          as standard_name
          from users as u
         where u.email= :email
        """;

        NpgsqlConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using NpgsqlCommand command = new NpgsqlCommand(sql, connection)
            .AddParameter("email", email);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        int userId = reader.GetOrdinal("user_id");
        int name = reader.GetOrdinal("name");
        int standardName = reader.GetOrdinal("standard_name");

        while (await reader.ReadAsync(cancellationToken))
        {
            return new User(
                reader.GetInt32(userId),
                reader.GetString(name),
                email,
                reader.GetString(standardName));
        }

        return null;
    }

    public async Task<User?> GetByName(string name, CancellationToken cancellationToken)
    {
        const string sql = """
        select u.user_id                as user_id
             , u.email                  as email
             , u.standard_name          as standard_name
          from users as u
         where u.name= :name
        """;

        NpgsqlConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using NpgsqlCommand command = new NpgsqlCommand(sql, connection)
            .AddParameter("name", name);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        int userId = reader.GetOrdinal("user_id");
        int email = reader.GetOrdinal("email");
        int standardName = reader.GetOrdinal("standard_name");

        while (await reader.ReadAsync(cancellationToken))
        {
            return new User(
                reader.GetInt32(userId),
                name,
                reader.GetString(email),
                reader.GetString(standardName));
        }

        return null;
    }

    public async Task<User?> GetByStandardName(string name, CancellationToken cancellationToken)
    {
        const string sql = """
        select u.user_id                as user_id
             , u.email                  as email
             , u.name                   as name
          from users as u
         where u.standard_name= :standard_name
        """;

        NpgsqlConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using NpgsqlCommand command = new NpgsqlCommand(sql, connection)
            .AddParameter("standard_name", name.ToLowerInvariant());

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        int userId = reader.GetOrdinal("user_id");
        int email = reader.GetOrdinal("email");

        while (await reader.ReadAsync(cancellationToken))
        {
            return new User(
                reader.GetInt32(userId),
                name,
                reader.GetString(email),
                name.ToLowerInvariant());
        }

        return null;
    }

    public async Task<int?> AddUser(CreateUserQuery query, CancellationToken cancellationToken)
    {
        const string sql = """
        insert into users(name, email, standard_name)
        values (:name, :email, :standard_name)
        returning user_id;
        """;
        NpgsqlConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using NpgsqlCommand command = new NpgsqlCommand(sql, connection)
            .AddParameter("name", query.Name)
            .AddParameter("email", query.Email)
            .AddParameter("standard_name", query.Name.ToLowerInvariant());

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        int userId = reader.GetOrdinal("user_id");

        while (await reader.ReadAsync(cancellationToken))
        {
            return reader.GetInt32(userId);
        }

        return null;
    }

    public async Task<int> UsersCount(CancellationToken cancellationToken)
    {
        const string sql = """
        select count(*) as count
        from (select count(*)
              from aneks as a
              group by a.user_id) as ac;
        """;
        NpgsqlConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using var command = new NpgsqlCommand(sql, connection);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        int count = reader.GetOrdinal("count");

        while (await reader.ReadAsync(cancellationToken))
        {
            return reader.GetInt32(count);
        }

        return 0;
    }
}