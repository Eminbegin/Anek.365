using FluentMigrator;
using Itmo.Dev.Platform.Postgres.Migrations;

namespace Anek._365.Infrastructure.DataAccess.Migrations;

#pragma warning disable SA1649

[Migration(1713965252, "Initial")]
public class Initial : SqlMigration
{
    protected override string GetUpSql(IServiceProvider serviceProvider)
    {
        return """
        create table users(
            user_id         serial                      primary key,
            name            text                        not null,
            email           text                        not null,
            standard_name   text                        not null
        );
        
        create table aneks
        (
            anek_id         serial                      primary key,
            user_id         int                         not null,
            title           text                        not null,
            content         text                        not null,
            created_at      timestamp with time zone    not null,
            views           int                         not null,
            foreign key (user_id) references users(user_id)
        );

        create table tags(
            tag_id          serial                      primary key,
            name            text                        not null,
            standard_name   text                        not null
        );
        
        create table marks(
            anek_id         int                         not null,
            user_id         int                         not null,
            mark            int                         not null,
            primary key (user_id, anek_id),
            foreign key (user_id) references users(user_id),
            foreign key (anek_id) references aneks(anek_id)
        );

        create table tags_aneks(
            tag_id          int                         not null,
            anek_id         int                         not null,
            primary key (tag_id, anek_id),
            foreign key (anek_id) references aneks(anek_id),
            foreign key (tag_id) references tags(tag_id)
        );
        """;
    }

    protected override string GetDownSql(IServiceProvider serviceProvider)
    {
        return """
        drop table tags_aneks;
        drop table marks;
        drop table tags;
        drop table aneks;
        drop table users
        """;
    }
}