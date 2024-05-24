using Anek._365.Application.Abstractions.Queries.Aneks;
using Anek._365.Application.Abstractions.Queries.Tags;
using Anek._365.Application.Abstractions.Repositories;
using Anek._365.Application.Contracts.Commands.Aneks;
using Anek._365.Application.Contracts.Services.Aneks;
using Anek._365.Application.Models;
using Anek._365.Application.Services;
using Anek._365.Infrastructure.DataAccess;
using Anek._365.Tests.UnitTests.Fakers;
using Bogus;
using FluentAssertions;
using Moq;
using Xunit;

namespace Anek._365.Tests.UnitTests.Seriveces;

public class AneksServiceTests
{
    private readonly Mock<IAneksRepository> _aneksRepositoryFake = new(MockBehavior.Loose);
    private readonly Mock<IUsersRepository> _usersRepositoryFake = new(MockBehavior.Loose);
    private readonly Mock<ITagsRepository> _tagsRepositoryFake = new(MockBehavior.Loose);
    private readonly Mock<IMarksRepository> _marksRepositoryFake = new(MockBehavior.Loose);

    private readonly IAneksService _aneksService;

    private readonly Faker _faker;

    public AneksServiceTests()
    {
        _faker = new Faker();
        _aneksService = new AneksService(new PersistenceContext(
            _aneksRepositoryFake.Object,
            _usersRepositoryFake.Object,
            _tagsRepositoryFake.Object,
            _marksRepositoryFake.Object));
    }

    [Fact]
    public async Task CreateAnekAsync_Success()
    {
        // Arrange
        _aneksRepositoryFake.Setup(r
                => r.AddAnek(It.IsAny<CreateAnekQuery>(), default))
            .ReturnsAsync(1);

        _tagsRepositoryFake.Setup(r
            => r.AddAnek(It.IsAny<AddAnekQuery>(), default));

        var request = new CreateAnekCommand.Request(1, "1", "1", [1, 2]);

        // Act
        CreateAnekCommand.Response result = await _aneksService.CreateAnekAsync(request, default);

        // Assert
        if (result is CreateAnekCommand.Response.Success success)
        {
            success.AnekId.Should().Be(1);
        }
        else
        {
            Assert.Fail("Be be be, be be be");
        }
    }

    [Fact]
    public async Task CreateAnekAsync_Failure()
    {
        // Arrange
        _aneksRepositoryFake.Setup(r
                => r.AddAnek(It.IsAny<CreateAnekQuery>(), default))
            .ReturnsAsync((int?)null);

        var request = new CreateAnekCommand.Request(1, "1", "1", [1, 2]);

        // Act
        CreateAnekCommand.Response result = await _aneksService.CreateAnekAsync(request, default);

        // Assert
        result.Should().BeOfType<CreateAnekCommand.Response.Failure>();
    }

    [Fact]
    public async Task GetAneksAsync()
    {
        // Arrange
        AnekDot[] anekDotSeed = [_faker.AnekDot(), _faker.AnekDot()];

        _aneksRepositoryFake.Setup(r
                => r.QueryAsync(It.IsAny<AneksQuery>(), default))
            .Returns(anekDotSeed.ToAsyncEnumerable());

        var request = new GetAnekListCommand.Request(anekDotSeed.Select(x => x.Id).ToArray());

        // Act
        GetAnekListCommand.Response result = await _aneksService.GetAneksAsync(request, default);

        // Assert
        result.Aneks.Should()
            .HaveCount(2)
            .And.Contain(anekDotSeed[0])
            .And.Contain(anekDotSeed[1]);
    }

    [Fact]
    public async Task AddViewAsync()
    {
        // Arrange
        _aneksRepositoryFake.Setup(r
                => r.AddViewing(It.IsAny<int>(), default))
            .Returns(Task.CompletedTask);

        var request = new AddViewCommand.Request(1);

        // Act
        await _aneksService.AddViewAsync(request, default);

        // Assert
        Assert.True(true);
    }

    [Fact]
    public async Task GetNewAneksAsync()
    {
        // Arrange
        AnekForViewing[] anekSeed = [_faker.AnekForViewing()];

        _aneksRepositoryFake.Setup(r
                => r.GetNewAneks(It.IsAny<AneksNewQuery>(), default))
            .Returns(anekSeed.ToAsyncEnumerable);

        _aneksRepositoryFake.Setup(r
                => r.CountInPeriod(It.IsAny<DateTimeOffset>(), default))
            .ReturnsAsync(1);

        var request = new GetAneksCommand.Request.News(1, 10);

        // Act
        GetAneksCommand.Response result = await _aneksService.GetNewAneksAsync(request, default);

        // Assert
        if (result is GetAneksCommand.Response.Success success)
        {
            success.Count.Should().Be(1);
            success.Name.Should().Be(string.Empty);
            success.AnekDots.Should().HaveCount(1);
        }
    }

    [Fact]
    public async Task GetPopularAneksAsync()
    {
        // Arrange
        AnekForViewing[] anekSeed = [_faker.AnekForViewing()];

        _aneksRepositoryFake.Setup(r
                => r.GetPopularAneks(It.IsAny<AneksPeriodedQuery>(), default))
            .Returns(anekSeed.ToAsyncEnumerable);

        _aneksRepositoryFake.Setup(r
                => r.CountInPeriod(It.IsAny<DateTimeOffset>(), default))
            .ReturnsAsync(1);

        var request = new GetAneksCommand.Request.Popular(1, 10, Period.None);

        // Act
        GetAneksCommand.Response result = await _aneksService.GetPopularAneksAsync(request, default);

        // Assert
        if (result is GetAneksCommand.Response.Success success)
        {
            success.Count.Should().Be(1);
            success.Name.Should().Be(string.Empty);
            success.AnekDots.Should().HaveCount(1);
        }
    }

    [Fact]
    public async Task GetMoreViewedAneksAsync()
    {
        // Arrange
        AnekForViewing[] anekSeed = [_faker.AnekForViewing()];

        _aneksRepositoryFake.Setup(r
                => r.GetMoreViewedAneks(It.IsAny<AneksPeriodedQuery>(), default))
            .Returns(anekSeed.ToAsyncEnumerable);

        _aneksRepositoryFake.Setup(r
                => r.CountInPeriod(It.IsAny<DateTimeOffset>(), default))
            .ReturnsAsync(1);

        var request = new GetAneksCommand.Request.MoreViewed(1, 10, Period.None);

        // Act
        GetAneksCommand.Response result = await _aneksService.GetMoreViewedAneksAsync(request, default);

        // Assert
        if (result is GetAneksCommand.Response.Success success)
        {
            success.Count.Should().Be(1);
            success.Name.Should().Be(string.Empty);
            success.AnekDots.Should().HaveCount(1);
        }
    }

    [Fact]
    public async Task GetAnekAsync_Success()
    {
        // Arrange
        int id = 1;
        AnekDot anekDotSeed = _faker.AnekDot(id);
        User userSeed = _faker.User(anekDotSeed.UserId);
        Tag[] tagsSeed = [_faker.Tag()];

        _aneksRepositoryFake.Setup(r
                => r.QueryAsync(It.IsAny<AneksQuery>(), default))
            .Returns(new[] { anekDotSeed }.ToAsyncEnumerable);

        _usersRepositoryFake.Setup(r
                => r.GetById(anekDotSeed.UserId, default))
            .ReturnsAsync(userSeed);

        _tagsRepositoryFake.Setup(r
                => r.GetByAnekId(id, default))
            .Returns(tagsSeed.ToAsyncEnumerable);

        var request = new GetOneAnekCommand.Request(id);

        // Act
        GetOneAnekCommand.Response result = await _aneksService.GetAnekAsync(request, default);

        // Assert
        if (result is GetOneAnekCommand.Response.Success success)
        {
            success.AnekDot.Should().Be(anekDotSeed);
            success.User.Should().Be(userSeed);
            success.Tags.Should().HaveCount(tagsSeed.Length);
        }
        else
        {
            Assert.Fail("bbb");
        }
    }

    [Fact]
    public async Task GetAnekAsync_Failure()
    {
        // Arrange
        int id = 1;
        AnekDot anekDotSeed = _faker.AnekDot(id);

        _aneksRepositoryFake.Setup(r
                => r.QueryAsync(It.IsAny<AneksQuery>(), default))
            .Returns(new[] { anekDotSeed }.ToAsyncEnumerable);

        _usersRepositoryFake.Setup(r
                => r.GetById(anekDotSeed.UserId, default))
            .ReturnsAsync((User?)null);

        var request = new GetOneAnekCommand.Request(id);

        // Act
        GetOneAnekCommand.Response result = await _aneksService.GetAnekAsync(request, default);

        // Assert
        result.Should().BeOfType<GetOneAnekCommand.Response.Failure>();
    }

    [Fact]
    public async Task GetNewAneksByTagAsync_Success()
    {
        // Arrange
        AnekForViewing[] anekSeed = [_faker.AnekForViewing()];
        string name = "Popa";
        Tag tagSeed = _faker.Tag(name);

        _tagsRepositoryFake.Setup(r
                => r.GetByStandardName(name, default))
            .ReturnsAsync(tagSeed);

        _aneksRepositoryFake.Setup(r
                => r.CountByTagId(tagSeed.Id, default))
            .ReturnsAsync(1);

        _aneksRepositoryFake.Setup(r
                => r.GetNewWithTagIdAneks(It.IsAny<AneksWithTagIdQuery>(), default))
            .Returns(anekSeed.ToAsyncEnumerable);

        var request = new GetAneksCommand.Request.NewByTag(name, 1, 10);

        // Act
        GetAneksCommand.Response result = await _aneksService.GetNewAneksByTagAsync(request, default);

        // Assert
        if (result is GetAneksCommand.Response.Success success)
        {
            success.Count.Should().Be(1);
            success.Name.Should().Be(tagSeed.Name);
            success.AnekDots.Should().HaveCount(1);
        }
        else
        {
            Assert.Fail(string.Empty);
        }
    }

    [Fact]
    public async Task GetNewAneksByTagAsync_Failure()
    {
        // Arrange
        string name = "Popa";
        _tagsRepositoryFake.Setup(r
                => r.GetByStandardName(name, default))
            .ReturnsAsync((Tag?)null);

        var request = new GetAneksCommand.Request.NewByTag(name, 1, 10);

        // Act
        GetAneksCommand.Response result = await _aneksService.GetNewAneksByTagAsync(request, default);

        // Assert
        result.Should().BeOfType<GetAneksCommand.Response.Failure>();
    }

    [Fact]
    public async Task GetPopularAneksByTagAsync_Success()
    {
        // Arrange
        AnekForViewing[] anekSeed = [_faker.AnekForViewing()];
        string name = "Popa";
        Tag tagSeed = _faker.Tag(name);

        _tagsRepositoryFake.Setup(r
                => r.GetByStandardName(name, default))
            .ReturnsAsync(tagSeed);

        _aneksRepositoryFake.Setup(r
                => r.CountByTagId(tagSeed.Id, default))
            .ReturnsAsync(1);

        _aneksRepositoryFake.Setup(r
                => r.GetPopularWithTagIdAneks(It.IsAny<AneksWithTagIdQuery>(), default))
            .Returns(anekSeed.ToAsyncEnumerable);

        var request = new GetAneksCommand.Request.PopularByTag(name, 1, 10);

        // Act
        GetAneksCommand.Response result = await _aneksService.GetPopularAneksByTagAsync(request, default);

        // Assert
        if (result is GetAneksCommand.Response.Success success)
        {
            success.Count.Should().Be(1);
            success.Name.Should().Be(tagSeed.Name);
            success.AnekDots.Should().HaveCount(1);
        }
        else
        {
            Assert.Fail(string.Empty);
        }
    }

    [Fact]
    public async Task GetPopularAneksByTagAsync_Failure()
    {
        // Arrange
        string name = "Popa";
        _tagsRepositoryFake.Setup(r
                => r.GetByStandardName(name, default))
            .ReturnsAsync((Tag?)null);

        var request = new GetAneksCommand.Request.PopularByTag(name, 1, 10);

        // Act
        GetAneksCommand.Response result = await _aneksService.GetPopularAneksByTagAsync(request, default);

        // Assert
        result.Should().BeOfType<GetAneksCommand.Response.Failure>();
    }

    [Fact]
    public async Task GetMoreViewedAneksByTagAsync_Success()
    {
        // Arrange
        AnekForViewing[] anekSeed = [_faker.AnekForViewing()];
        string name = "Popa";
        Tag tagSeed = _faker.Tag(name);

        _tagsRepositoryFake.Setup(r
                => r.GetByStandardName(name, default))
            .ReturnsAsync(tagSeed);

        _aneksRepositoryFake.Setup(r
                => r.CountByTagId(tagSeed.Id, default))
            .ReturnsAsync(1);

        _aneksRepositoryFake.Setup(r
                => r.GetMoreViewedWithTagIdAneks(It.IsAny<AneksWithTagIdQuery>(), default))
            .Returns(anekSeed.ToAsyncEnumerable);

        var request = new GetAneksCommand.Request.MoreViewedByTag(name, 1, 10);

        // Act
        GetAneksCommand.Response result = await _aneksService.GetMoreViewedAneksByTagAsync(request, default);

        // Assert
        if (result is GetAneksCommand.Response.Success success)
        {
            success.Count.Should().Be(1);
            success.Name.Should().Be(tagSeed.Name);
            success.AnekDots.Should().HaveCount(1);
        }
        else
        {
            Assert.Fail(string.Empty);
        }
    }

    [Fact]
    public async Task GetMoreViewedAneksByTagAsync_Failure()
    {
        // Arrange
        string name = "Popa";
        _tagsRepositoryFake.Setup(r
                => r.GetByStandardName(name, default))
            .ReturnsAsync((Tag?)null);

        var request = new GetAneksCommand.Request.MoreViewedByTag(name, 1, 10);

        // Act
        GetAneksCommand.Response result = await _aneksService.GetMoreViewedAneksByTagAsync(request, default);

        // Assert
        result.Should().BeOfType<GetAneksCommand.Response.Failure>();
    }

    [Fact]
    public async Task GetNewAneksByUserAsync_Success()
    {
        // Arrange
        AnekForViewing[] anekSeed = [_faker.AnekForViewing()];
        string name = "Popa";
        User userSeed = _faker.User(null, name);

        _usersRepositoryFake.Setup(r
                => r.GetByStandardName(name, default))
            .ReturnsAsync(userSeed);

        _aneksRepositoryFake.Setup(r
                => r.CountByUserId(userSeed.Id, default))
            .ReturnsAsync(1);

        _aneksRepositoryFake.Setup(r
                => r.GetNewWithUserIdAneks(It.IsAny<AneksWithUserIdQuery>(), default))
            .Returns(anekSeed.ToAsyncEnumerable);

        var request = new GetAneksCommand.Request.NewByUser(name, 1, 10);

        // Act
        GetAneksCommand.Response result = await _aneksService.GetNewAneksByUserAsync(request, default);

        // Assert
        if (result is GetAneksCommand.Response.Success success)
        {
            success.Count.Should().Be(1);
            success.Name.Should().Be(userSeed.Name);
            success.AnekDots.Should().HaveCount(1);
        }
        else
        {
            Assert.Fail(string.Empty);
        }
    }

    [Fact]
    public async Task GetNewAneksByUserAsync_Failure()
    {
        // Arrange
        string name = "Popa";
        _usersRepositoryFake.Setup(r
                => r.GetByStandardName(name, default))
            .ReturnsAsync((User?)null);

        var request = new GetAneksCommand.Request.NewByUser(name, 1, 10);

        // Act
        GetAneksCommand.Response result = await _aneksService.GetNewAneksByUserAsync(request, default);

        // Assert
        result.Should().BeOfType<GetAneksCommand.Response.Failure>();
    }

    [Fact]
    public async Task GetPopularAneksByUserAsync_Success()
    {
        // Arrange
        AnekForViewing[] anekSeed = [_faker.AnekForViewing()];
        string name = "Popa";
        User userSeed = _faker.User(null, name);

        _usersRepositoryFake.Setup(r
                => r.GetByStandardName(name, default))
            .ReturnsAsync(userSeed);

        _aneksRepositoryFake.Setup(r
                => r.CountByUserId(userSeed.Id, default))
            .ReturnsAsync(1);

        _aneksRepositoryFake.Setup(r
                => r.GetPopularWithUserIdAneks(It.IsAny<AneksWithUserIdQuery>(), default))
            .Returns(anekSeed.ToAsyncEnumerable);

        var request = new GetAneksCommand.Request.PopularByUser(name, 1, 10);

        // Act
        GetAneksCommand.Response result = await _aneksService.GetPopularAneksByUserAsync(request, default);

        // Assert
        if (result is GetAneksCommand.Response.Success success)
        {
            success.Count.Should().Be(1);
            success.Name.Should().Be(userSeed.Name);
            success.AnekDots.Should().HaveCount(1);
        }
        else
        {
            Assert.Fail(string.Empty);
        }
    }

    [Fact]
    public async Task GetPopularAneksByUserAsync_Failure()
    {
        // Arrange
        string name = "Popa";
        _usersRepositoryFake.Setup(r
                => r.GetByStandardName(name, default))
            .ReturnsAsync((User?)null);

        var request = new GetAneksCommand.Request.PopularByUser(name, 1, 10);

        // Act
        GetAneksCommand.Response result = await _aneksService.GetPopularAneksByUserAsync(request, default);

        // Assert
        result.Should().BeOfType<GetAneksCommand.Response.Failure>();
    }

    [Fact]
    public async Task GetMoreViewedAneksByUserAsync_Success()
    {
        // Arrange
        AnekForViewing[] anekSeed = [_faker.AnekForViewing()];
        string name = "Popa";
        User userSeed = _faker.User(null, name);

        _usersRepositoryFake.Setup(r
                => r.GetByStandardName(name, default))
            .ReturnsAsync(userSeed);

        _aneksRepositoryFake.Setup(r
                => r.CountByUserId(userSeed.Id, default))
            .ReturnsAsync(1);

        _aneksRepositoryFake.Setup(r
                => r.GetMoreViewedWithUserIdAneks(It.IsAny<AneksWithUserIdQuery>(), default))
            .Returns(anekSeed.ToAsyncEnumerable);

        var request = new GetAneksCommand.Request.MoreViewedByUser(name, 1, 10);

        // Act
        GetAneksCommand.Response result = await _aneksService.GetMoreViewedAneksByUserAsync(request, default);

        // Assert
        if (result is GetAneksCommand.Response.Success success)
        {
            success.Count.Should().Be(1);
            success.Name.Should().Be(userSeed.Name);
            success.AnekDots.Should().HaveCount(1);
        }
        else
        {
            Assert.Fail(string.Empty);
        }
    }

    [Fact]
    public async Task GetMoreViewedAneksByUserAsync_Failure()
    {
        // Arrange
        string name = "Popa";
        _usersRepositoryFake.Setup(r
                => r.GetByStandardName(name, default))
            .ReturnsAsync((User?)null);

        var request = new GetAneksCommand.Request.MoreViewedByUser(name, 1, 10);

        // Act
        GetAneksCommand.Response result = await _aneksService.GetMoreViewedAneksByUserAsync(request, default);

        // Assert
        result.Should().BeOfType<GetAneksCommand.Response.Failure>();
    }
}