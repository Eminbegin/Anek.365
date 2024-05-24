using SourceKit.Generators.Builder.Annotations;

namespace Anek._365.Application.Abstractions.Queries.Aneks;

[GenerateBuilder]
public partial record AneksWithTagIdQuery(
    int Id,
    int PageNumber = 1,
    int PageSize = 10);