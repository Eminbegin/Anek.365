using FluentMigrator;
using Itmo.Dev.Platform.Postgres.Migrations;

namespace Anek._365.Infrastructure.DataAccess.Migrations;

#pragma warning disable SA1649

[Migration(1714135252, "Initial")]
public class AddTags : SqlMigration
{
    protected override string GetUpSql(IServiceProvider serviceProvider)
    {
        return """
        insert INTO tags (name, standard_name)
        select * from unnest(array['негры','каламабуры','про евреев','про армян','классика','бородатые','7-8','жопа','про бизнес','РОССИЯ','про говно','для первого свидания','туалетные','чёрный юмор','коричневый юмор','ИТМО','IT','пидорас','измены','ГАИ', 'Император туалетов', 'гои', 'хохлы', 'про политику', 'спорт', 'студенты', 'большие сиськи', 'мамки', 'про грузин'],              
                             array['niggers','puns',         'jews', 'armenians', 'classics', 'bearded', '7-8', 'ass', 'business', 'RUSSIA',    'shit','first_date',         'toilet',    'black_humor','brown_humor',    'ITMO','IT','pidoras','cheating','GAI','toilet_emperor',    'goi', 'hohol', 'politics',     'sport', 'students', 'big tits', 'moms', 'georgians']);                      
        """;
    }

    protected override string GetDownSql(IServiceProvider serviceProvider)
    {
        return """
        truncate table tags;
        truncate table tags_aneks;
        """;
    }
}