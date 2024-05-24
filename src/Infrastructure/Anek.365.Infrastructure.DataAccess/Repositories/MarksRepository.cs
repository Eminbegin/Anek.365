using Anek._365.Application.Abstractions.Marks;
using Anek._365.Application.Models;
using Itmo.Dev.Platform.Postgres.Connection;
using Itmo.Dev.Platform.Postgres.Extensions;
using Npgsql;

namespace Anek._365.Application.Abstractions.Repositories;

public class MarksRepository : IMarksRepository
{
    private readonly IPostgresConnectionProvider _connectionProvider;

    public MarksRepository(IPostgresConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task AddMark(MarkQuery query, CancellationToken cancellationToken)
    {
        const string sql = """
        insert into marks(user_id, anek_id, mark)
        values (:user_id, :anek_id, :mark)
        on conflict on constraint marks_pkey
        do update set user_id = excluded.user_id,
                      anek_id = excluded.anek_id,
                      mark = excluded.mark;
        """;

        NpgsqlConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using NpgsqlCommand command = new NpgsqlCommand(sql, connection)
            .AddParameter("anek_id", query.AnekId)
            .AddParameter("user_id", query.UserId)
            .AddParameter("mark", query.Value);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task AddView(MarkQuery query, CancellationToken cancellationToken)
    {
        const string sql = """
        insert into marks(user_id, anek_id, mark)
        values (:user_id, :anek_id, :mark)
        on conflict on constraint marks_pkey do nothing;
        """;

        NpgsqlConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using NpgsqlCommand command = new NpgsqlCommand(sql, connection)
            .AddParameter("anek_id", query.AnekId)
            .AddParameter("user_id", query.UserId)
            .AddParameter("mark", query.Value);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<Mark?> GetMark(int anekId, int userId, CancellationToken cancellationToken)
    {
        const string sql = """
        select m.mark               as mark
          from marks as m
         where m.anek_id = :anek_id
           and m.user_id = :user_id
        """;

        NpgsqlConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using NpgsqlCommand command = new NpgsqlCommand(sql, connection)
            .AddParameter("anek_id", anekId)
            .AddParameter("user_id", userId);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        int mark = reader.GetOrdinal("mark");

        while (await reader.ReadAsync(cancellationToken))
        {
            return new Mark(anekId, userId, reader.GetInt32(mark));
        }

        return null;
    }
}