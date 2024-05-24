using Anek._365.Application.Abstractions.Queries.Tags;
using Anek._365.Application.Models;
using Itmo.Dev.Platform.Postgres.Connection;
using Itmo.Dev.Platform.Postgres.Extensions;
using Npgsql;
using System.Runtime.CompilerServices;

namespace Anek._365.Application.Abstractions.Repositories;

public class TagsRepository : ITagsRepository
{
    private readonly IPostgresConnectionProvider _connectionProvider;

    public TagsRepository(IPostgresConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async IAsyncEnumerable<TagWithCount> GetAllWithViews(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
            select t.tag_id                 as tag_id
                 , t.name                   as name
                 , t.standard_name          as standard_name
                 , count(*)                 as count
              from tags as t
        inner join tags_aneks as ta
                on ta.tag_id = t.tag_id
          group by t.tag_id, t.name
            having count(*) != 0
          order by t.name
        """;

        NpgsqlConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using var command = new NpgsqlCommand(sql, connection);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        int tagId = reader.GetOrdinal("tag_id");
        int name = reader.GetOrdinal("name");
        int count = reader.GetOrdinal("count");
        int standardName = reader.GetOrdinal("standard_name");

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new TagWithCount(
                reader.GetInt32(tagId),
                reader.GetString(name),
                reader.GetInt32(count),
                reader.GetString(standardName));
        }
    }

    public async IAsyncEnumerable<Tag> GetAll(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
          select t.tag_id                     as tag_id
               , t.name                       as name
               , t.standard_name              as standard_name
            from tags as t
        order by t.name
        """;

        NpgsqlConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using var command = new NpgsqlCommand(sql, connection);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        int tagId = reader.GetOrdinal("tag_id");
        int name = reader.GetOrdinal("name");
        int standardName = reader.GetOrdinal("standard_name");

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new Tag(
                reader.GetInt32(tagId),
                reader.GetString(name),
                reader.GetString(standardName));
        }
    }

    public async Task<Tag?> GetByStandardName(string standardName, CancellationToken cancellationToken)
    {
        const string sql = """
          select t.tag_id                     as tag_id
               , t.name                       as name
            from tags as t
            where t.standard_name =:standard_name
        """;

        NpgsqlConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using NpgsqlCommand command = new NpgsqlCommand(sql, connection)
            .AddParameter("standard_name", standardName);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        int tagId = reader.GetOrdinal("tag_id");
        int name = reader.GetOrdinal("name");

        while (await reader.ReadAsync(cancellationToken))
        {
            return new Tag(
                reader.GetInt32(tagId),
                reader.GetString(name),
                standardName);
        }

        return null;
    }

    public async IAsyncEnumerable<Tag> GetByAnekId(
        int anekId,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
            select t.tag_id                     as tag_id
                 , t.name                       as name
                 , t.standard_name              as standard_name
              from tags as t
        inner join tags_aneks as ta
                on ta.tag_id = t.tag_id
               and ta.anek_id = :anek_id
          order by t.name
        """;

        NpgsqlConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using NpgsqlCommand command = new NpgsqlCommand(sql, connection)
            .AddParameter("anek_id", anekId);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        int tagId = reader.GetOrdinal("tag_id");
        int name = reader.GetOrdinal("name");
        int standardName = reader.GetOrdinal("standard_name");

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new Tag(
                reader.GetInt32(tagId),
                reader.GetString(name),
                reader.GetString(standardName));
        }
    }

    public async Task AddAnek(AddAnekQuery query, CancellationToken cancellationToken)
    {
        const string sql = """
        insert into tags_aneks(tag_id, anek_id)
        select * from unnest(:tag_ids, :anek_ids)
        """;

        NpgsqlConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using NpgsqlCommand command = new NpgsqlCommand(sql, connection)
            .AddParameter("tag_ids", query.TagIds)
            .AddParameter("anek_ids", Enumerable.Repeat(query.AnekId, query.TagIds.Length));

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}