﻿using SourceKit.Generators.Builder.Annotations;

namespace Anek._365.Application.Abstractions.Queries.Aneks;

[GenerateBuilder]
public partial record CreateAnekQuery(
    int UserId,
    string Title,
    string Content,
    DateTimeOffset CreatedAt);