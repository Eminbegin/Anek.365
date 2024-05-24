using SourceKit.Generators.Builder.Annotations;

namespace Anek._365.Application.Abstractions.Queries.Tags;

[GenerateBuilder]
public partial record AddAnekQuery(
    int AnekId,
    int[] TagIds);