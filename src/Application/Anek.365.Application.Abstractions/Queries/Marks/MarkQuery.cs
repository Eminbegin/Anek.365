using SourceKit.Generators.Builder.Annotations;

namespace Anek._365.Application.Abstractions.Marks;

[GenerateBuilder]
public partial record MarkQuery(
    int AnekId,
    int UserId,
    int Value);