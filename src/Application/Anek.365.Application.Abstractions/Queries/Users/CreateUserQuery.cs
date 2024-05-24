using SourceKit.Generators.Builder.Annotations;

namespace Anek._365.Application.Abstractions.Queries.Users;

[GenerateBuilder]
public partial record CreateUserQuery(
    string Name,
    string Email);