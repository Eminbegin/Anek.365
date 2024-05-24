using Anek._365.Application.Models;
using Bogus;

namespace Anek._365.Tests.UnitTests.Fakers;

public static class FakerExtension
{
    public static AnekDot AnekDot(this Faker faker, int? id = null)
    {
        return new AnekDot(
            id ?? faker.Random.Int(0),
            faker.Random.String2(10),
            faker.Random.String2(10),
            new DateTimeOffset(faker.Date.Past()),
            faker.Random.Int(),
            faker.Random.Int(0),
            faker.Random.Int(0));
    }

    public static AnekForViewing AnekForViewing(this Faker faker)
    {
        return new AnekForViewing(
            faker.Random.Int(0),
            faker.Random.String2(10),
            faker.Random.String2(10),
            faker.Random.Int(),
            faker.Random.Int(0));
    }

    public static Tag Tag(this Faker faker, string? standardName = null)
    {
        return new Tag(
            faker.Random.Int(0),
            faker.Random.String2(10),
            standardName ?? faker.Random.String2(10));
    }

    public static User User(this Faker faker, int? id = null, string? standardName = null)
    {
        return new User(
            id ?? faker.Random.Int(0),
            faker.Random.String2(10),
            faker.Random.String2(10),
            standardName ?? faker.Random.String2(10));
    }

    public static UserWithCount UserWithCount(this Faker faker, int? id = null, string? standardName = null)
    {
        return new UserWithCount(
            id ?? faker.Random.Int(0),
            faker.Random.String2(10),
            id ?? faker.Random.Int(0),
            standardName ?? faker.Random.String2(10));
    }
}