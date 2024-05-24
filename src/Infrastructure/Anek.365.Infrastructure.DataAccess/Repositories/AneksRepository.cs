using Anek._365.Application.Abstractions.Queries.Aneks;
using Anek._365.Application.Models;
using Itmo.Dev.Platform.Postgres.Connection;
using Itmo.Dev.Platform.Postgres.Extensions;
using Npgsql;
using System.Runtime.CompilerServices;

namespace Anek._365.Application.Abstractions.Repositories;

public class AneksRepository : IAneksRepository
{
    private readonly IPostgresConnectionProvider _connectionProvider;

    public AneksRepository(IPostgresConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async IAsyncEnumerable<AnekForViewing> GetNewAneks(
        AneksNewQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
        with filtered as (
            select a.anek_id                as anek_id
                 , a.title                  as title
                 , a.content                as content
                 , a.views                  as views
                 , a.created_at             as created_at
            from aneks as a
            order by a.created_at desc
              offset :offset
               limit :limit
        )
           select a.anek_id                 as id
                , a.title                   as title
                , a.content                 as content
                , a.views                   as views
                , coalesce(sum(m.mark), 0)  as mark
             from filtered as a
        left join marks as m
               on m.anek_id = a.anek_id
         group by a.anek_id, a.title, a.content, a.views, a.created_at
         order by a.created_at desc;
        """;

        NpgsqlConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using NpgsqlCommand command = new NpgsqlCommand(sql, connection)
            .AddParameter("offset", query.PageSize * (query.PageNumber - 1))
            .AddParameter("limit", query.PageSize);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        int id = reader.GetOrdinal("id");
        int title = reader.GetOrdinal("title");
        int content = reader.GetOrdinal("content");
        int mark = reader.GetOrdinal("mark");
        int views = reader.GetOrdinal("views");

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new AnekForViewing(
                reader.GetInt32(id),
                reader.GetString(title),
                reader.GetString(content),
                reader.GetInt32(mark),
                reader.GetInt32(views));
        }
    }

    public async IAsyncEnumerable<AnekForViewing> GetPopularAneks(
        AneksPeriodedQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
        with filtered as (
        select a.anek_id                    as anek_id
             , a.title                      as title
             , a.content                    as content
             , a.views                      as views
          from aneks as a
         where a.created_at > :border
        ),
        
        choosing as (
           select a.anek_id                 as anek_id
                , a.title                   as title
                , a.content                 as content
                , a.views                   as views
                , coalesce(sum(mark), 0)    as mark
             from filtered as a
        left join marks as m on a.anek_id = m.anek_id
         group by a.anek_id, a.title, a.content, a.views
         order by mark desc
           offset :offset
            limit :limit
        )
          select a.anek_id                  as id
               , a.title                    as title
               , a.content                  as content
               , a.views                    as views
               , a.mark                     as mark
            from choosing as a
        order by mark desc;
        """;

        NpgsqlConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using NpgsqlCommand command = new NpgsqlCommand(sql, connection)
            .AddParameter("border", query.Border.ToUniversalTime())
            .AddParameter("offset", query.PageSize * (query.PageNumber - 1))
            .AddParameter("limit", query.PageSize);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        int id = reader.GetOrdinal("id");
        int title = reader.GetOrdinal("title");
        int content = reader.GetOrdinal("content");
        int mark = reader.GetOrdinal("mark");
        int views = reader.GetOrdinal("views");

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new AnekForViewing(
                reader.GetInt32(id),
                reader.GetString(title),
                reader.GetString(content),
                reader.GetInt32(mark),
                reader.GetInt32(views));
        }
    }

    public async IAsyncEnumerable<AnekForViewing> GetMoreViewedAneks(
        AneksPeriodedQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
        with filtered as (
              select a.anek_id              as anek_id
                   , a.title                as title
                   , a.content              as content
                   , a.views                as views
                from aneks as a
               where a.created_at > :border
            order by a.views desc
              offset :offset
               limit :limit
        )
           select a.anek_id                 as id
                , a.title                   as title
                , a.content                 as content
                , a.views                   as views
                , coalesce(sum(m.mark), 0)  as mark
             from filtered as a
        left join marks as m
               on m.anek_id = a.anek_id
         group by a.anek_id, a.title, a.content, a.views
         order by a.views desc;
        """;

        NpgsqlConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using NpgsqlCommand command = new NpgsqlCommand(sql, connection)
            .AddParameter("border", query.Border.ToUniversalTime())
            .AddParameter("offset", query.PageSize * (query.PageNumber - 1))
            .AddParameter("limit", query.PageSize);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        int id = reader.GetOrdinal("id");
        int title = reader.GetOrdinal("title");
        int content = reader.GetOrdinal("content");
        int mark = reader.GetOrdinal("mark");
        int views = reader.GetOrdinal("views");

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new AnekForViewing(
                reader.GetInt32(id),
                reader.GetString(title),
                reader.GetString(content),
                reader.GetInt32(mark),
                reader.GetInt32(views));
        }
    }

    public async IAsyncEnumerable<AnekDot> QueryAsync(
        AneksQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
        with aaa as (select a.anek_id                   as anek_id
                          , a.title                     as title
                          , a.content                   as content
                          , a.created_at                as created_at
                          , a.views                     as views
                          , a.user_id                   as user_id
                       from aneks as a
                      where a.anek_id = any (:ids)
                      )
        
           select a.anek_id                             as anek_id
                , a.title                               as title
                , a.content                             as content
                , a.created_at                          as created_at
                , a.views                               as views
                , a.user_id                             as user_id
                , coalesce(sum(m.mark), 0)              as mark
             from aaa as a
        left join marks as m
               on a.anek_id = m.anek_id
         group by a.anek_id, a.title, a.content, a.created_at, a.views, a.user_id
        """;

        NpgsqlConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using NpgsqlCommand command = new NpgsqlCommand(sql, connection)
            .AddParameter("ids", query.Ids.ToArray());

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        int anekId = reader.GetOrdinal("anek_id");
        int title = reader.GetOrdinal("title");
        int content = reader.GetOrdinal("content");
        int createdAt = reader.GetOrdinal("created_at");
        int views = reader.GetOrdinal("views");
        int userId = reader.GetOrdinal("user_id");
        int mark = reader.GetOrdinal("mark");

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new AnekDot(
                reader.GetInt32(anekId),
                reader.GetString(title),
                reader.GetString(content),
                reader.GetFieldValue<DateTimeOffset>(createdAt),
                reader.GetInt32(mark),
                reader.GetInt32(views),
                reader.GetInt32(userId));
        }
    }

    public async Task<int?> AddAnek(CreateAnekQuery query, CancellationToken cancellationToken)
    {
        const string sql = """
        insert into aneks(user_id, title, content, created_at, views)
        values (:user_id, :title, :content, :created_at, :views)
        returning anek_id   ;
        """;
        NpgsqlConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using NpgsqlCommand command = new NpgsqlCommand(sql, connection)
            .AddParameter("user_id", query.UserId)
            .AddParameter("title", query.Title)
            .AddParameter("content", query.Content)
            .AddParameter("created_at", query.CreatedAt.ToUniversalTime())
            .AddParameter("views", 1);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        int anekId = reader.GetOrdinal("anek_id");

        while (await reader.ReadAsync(cancellationToken))
        {
            return reader.GetInt32(anekId);
        }

        return null;
    }

    public async IAsyncEnumerable<AnekForViewing> GetNewWithTagIdAneks(
        AneksWithTagIdQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
        with filtered as(
               select a.anek_id             as anek_id
                    , a.title               as title
                    , a.content             as content
                    , a.views               as views
                    , a.created_at          as created_at
                 from aneks as a
           inner join tags_aneks as ta
                   on ta.anek_id = a.anek_id
                  and ta.tag_id = :tag_id
             order by a.created_at desc
               offset :offset
                limit :limit
        )

           select a.anek_id                 as id
                , a.title                   as title
                , a.content                 as content
                , a.views                   as views
                , coalesce(sum(m.mark), 0)  as mark
             from filtered as a
        left join marks as m
               on m.anek_id = a.anek_id
         group by a.anek_id, a.title, a.content, a.views, a.created_at
         order by a.created_at desc;
        """;
        NpgsqlConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using NpgsqlCommand command = new NpgsqlCommand(sql, connection)
            .AddParameter("tag_id", query.Id)
            .AddParameter("offset", query.PageSize * (query.PageNumber - 1))
            .AddParameter("limit", query.PageSize);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        int id = reader.GetOrdinal("id");
        int title = reader.GetOrdinal("title");
        int content = reader.GetOrdinal("content");
        int mark = reader.GetOrdinal("mark");
        int views = reader.GetOrdinal("views");

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new AnekForViewing(
                reader.GetInt32(id),
                reader.GetString(title),
                reader.GetString(content),
                reader.GetInt32(mark),
                reader.GetInt32(views));
        }
    }

    public async IAsyncEnumerable<AnekForViewing> GetPopularWithTagIdAneks(
        AneksWithTagIdQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
            select a.anek_id                as anek_id
                 , a.title                  as title
                 , a.content                as content
                 , a.views                  as views
                 , coalesce(sum(mark), 0)   as mark
              from aneks as a
        inner join tags_aneks as ta
                on ta.anek_id = a.anek_id
               and ta.tag_id = :tag_id
         left join marks as m
                on a.anek_id = m.anek_id
          group by a.anek_id, a.title, a.content, a.views, a.created_at
          order by mark desc
            offset :offset
             limit :limit;
        """;

        NpgsqlConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using NpgsqlCommand command = new NpgsqlCommand(sql, connection)
            .AddParameter("tag_id", query.Id)
            .AddParameter("offset", query.PageSize * (query.PageNumber - 1))
            .AddParameter("limit", query.PageSize);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        int id = reader.GetOrdinal("anek_id");
        int title = reader.GetOrdinal("title");
        int content = reader.GetOrdinal("content");
        int mark = reader.GetOrdinal("mark");
        int views = reader.GetOrdinal("views");

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new AnekForViewing(
                reader.GetInt32(id),
                reader.GetString(title),
                reader.GetString(content),
                reader.GetInt32(mark),
                reader.GetInt32(views));
        }
    }

    public async IAsyncEnumerable<AnekForViewing> GetMoreViewedWithTagIdAneks(
        AneksWithTagIdQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
        with filtered as(
              select a.anek_id             as anek_id
                   , a.title               as title
                   , a.content             as content
                   , a.views               as views
                from aneks as a
          inner join tags_aneks as ta
                  on ta.anek_id = a.anek_id
                 and ta.tag_id = :tag_id
            order by a.views desc
              offset :offset
               limit :limit
        )

          select a.anek_id                 as id
               , a.title                   as title
               , a.content                 as content
               , a.views                   as views
               , coalesce(sum(m.mark), 0)  as mark
            from filtered as a
        left join marks as m
              on m.anek_id = a.anek_id
        group by a.anek_id, a.title, a.content, a.views
        order by a.views desc;
        """;
        NpgsqlConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using NpgsqlCommand command = new NpgsqlCommand(sql, connection)
            .AddParameter("tag_id", query.Id)
            .AddParameter("offset", query.PageSize * (query.PageNumber - 1))
            .AddParameter("limit", query.PageSize);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        int id = reader.GetOrdinal("id");
        int title = reader.GetOrdinal("title");
        int content = reader.GetOrdinal("content");
        int mark = reader.GetOrdinal("mark");
        int views = reader.GetOrdinal("views");

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new AnekForViewing(
                reader.GetInt32(id),
                reader.GetString(title),
                reader.GetString(content),
                reader.GetInt32(mark),
                reader.GetInt32(views));
        }
    }

    public async IAsyncEnumerable<AnekForViewing> GetNewWithUserIdAneks(
        AneksWithUserIdQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
        with filtered as (
               select a.anek_id             as anek_id
                    , a.title               as title
                    , a.content             as content
                    , a.views               as views
                    , a.created_at          as created_at
                 from aneks as a
                where a.user_id = :user_id
             order by a.created_at desc
               offset :offset
                limit :limit
        )

           select a.anek_id                 as id
                , a.title                   as title
                , a.content                 as content
                , a.views                   as views
                , coalesce(sum(m.mark), 0)  as mark
             from filtered as a
        left join marks as m
               on m.anek_id = a.anek_id
         group by a.anek_id, a.title, a.content, a.views, a.created_at
         order by a.created_at desc;
        """;

        NpgsqlConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using NpgsqlCommand command = new NpgsqlCommand(sql, connection)
            .AddParameter("user_id", query.Id)
            .AddParameter("offset", query.PageSize * (query.PageNumber - 1))
            .AddParameter("limit", query.PageSize);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        int id = reader.GetOrdinal("id");
        int title = reader.GetOrdinal("title");
        int content = reader.GetOrdinal("content");
        int mark = reader.GetOrdinal("mark");
        int views = reader.GetOrdinal("views");

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new AnekForViewing(
                reader.GetInt32(id),
                reader.GetString(title),
                reader.GetString(content),
                reader.GetInt32(mark),
                reader.GetInt32(views));
        }
    }

    public async IAsyncEnumerable<AnekForViewing> GetPopularWithUserIdAneks(
        AneksWithUserIdQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
            select a.anek_id                 as id
                 , a.title                   as title
                 , a.content                 as content
                 , a.views                   as views
                 , coalesce(sum(m.mark), 0)  as mark
              from aneks as a
         left join marks as m
                on m.anek_id = a.anek_id
               and a.user_id = :user_id
          group by a.anek_id, a.title, a.content, a.views, a.created_at
          order by mark desc
            offset :offset
             limit :limit;
        """;

        NpgsqlConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using NpgsqlCommand command = new NpgsqlCommand(sql, connection)
            .AddParameter("user_id", query.Id)
            .AddParameter("offset", query.PageSize * (query.PageNumber - 1))
            .AddParameter("limit", query.PageSize);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        int id = reader.GetOrdinal("id");
        int title = reader.GetOrdinal("title");
        int content = reader.GetOrdinal("content");
        int mark = reader.GetOrdinal("mark");
        int views = reader.GetOrdinal("views");

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new AnekForViewing(
                reader.GetInt32(id),
                reader.GetString(title),
                reader.GetString(content),
                reader.GetInt32(mark),
                reader.GetInt32(views));
        }
    }

    public async IAsyncEnumerable<AnekForViewing> GetMoreViewedWithUserIdAneks(
        AneksWithUserIdQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
        with filtered as (
              select a.anek_id             as anek_id
                   , a.title               as title
                   , a.content             as content
                   , a.views               as views
                from aneks as a
               where a.user_id = :user_id
            order by a.views desc
              offset :offset
               limit :limit
        )

          select a.anek_id                 as id
               , a.title                   as title
               , a.content                 as content
               , a.views                   as views
               , coalesce(sum(m.mark), 0)  as mark
            from filtered as a
        left join marks as m
              on m.anek_id = a.anek_id
        group by a.anek_id, a.title, a.content, a.views
        order by a.views desc;
        """;

        NpgsqlConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using NpgsqlCommand command = new NpgsqlCommand(sql, connection)
            .AddParameter("user_id", query.Id)
            .AddParameter("offset", query.PageSize * (query.PageNumber - 1))
            .AddParameter("limit", query.PageSize);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        int id = reader.GetOrdinal("id");
        int title = reader.GetOrdinal("title");
        int content = reader.GetOrdinal("content");
        int mark = reader.GetOrdinal("mark");
        int views = reader.GetOrdinal("views");

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new AnekForViewing(
                reader.GetInt32(id),
                reader.GetString(title),
                reader.GetString(content),
                reader.GetInt32(mark),
                reader.GetInt32(views));
        }
    }

    public async Task<int> CountInPeriod(DateTimeOffset border, CancellationToken cancellationToken)
    {
        const string sql = """
        select count(*) as count
        from aneks as a
        where a.created_at > :border;
        """;

        NpgsqlConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using NpgsqlCommand command = new NpgsqlCommand(sql, connection)
            .AddParameter("border", border.ToUniversalTime());

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        int count = reader.GetOrdinal("count");

        while (await reader.ReadAsync(cancellationToken))
        {
            return reader.GetInt32(count);
        }

        return 0;
    }

    public async Task<int> CountByTagId(int tagId, CancellationToken cancellationToken)
    {
        const string sql = """
        select count(*) as count
        from tags_aneks as ta
        where ta.tag_id = :tag_id;
        """;

        NpgsqlConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using NpgsqlCommand command = new NpgsqlCommand(sql, connection)
            .AddParameter("tag_id", tagId);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        int count = reader.GetOrdinal("count");

        while (await reader.ReadAsync(cancellationToken))
        {
            return reader.GetInt32(count);
        }

        return 0;
    }

    public async Task<int> CountByUserId(int userId, CancellationToken cancellationToken)
    {
        const string sql = """
        select count(*) as count
        from aneks as a
        where a.user_id = :user_id;
        """;

        NpgsqlConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using NpgsqlCommand command = new NpgsqlCommand(sql, connection)
            .AddParameter("user_id", userId);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        int count = reader.GetOrdinal("count");

        while (await reader.ReadAsync(cancellationToken))
        {
            return reader.GetInt32(count);
        }

        return 0;
    }

    public async Task AddViewing(int anekId, CancellationToken cancellationToken)
    {
        const string sql = """
        update aneks
        set views = views + 1
        where anek_id = :anek_id
        """;

        NpgsqlConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using NpgsqlCommand command = new NpgsqlCommand(sql, connection)
            .AddParameter("anek_id", anekId);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}