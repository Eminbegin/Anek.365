using SourceKit.Generators.Builder.Annotations;

namespace Anek._365.Application.Abstractions.Queries.Users;

[GenerateBuilder]
public partial record UsersQuery(
    int PageNumber = 1,
    int PageSize = 10);