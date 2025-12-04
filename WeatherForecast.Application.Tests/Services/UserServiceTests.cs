using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using WeatherForecast.Application.Interfaces;
using WeatherForecast.Application.Services;
using WeatherForecast.Domain.Models;
using Xunit;

namespace WeatherForecast.Application.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<ILogger<UserService>> _loggerMock = new();

    private UserService CreateService()
        => new UserService(_userRepoMock.Object, _loggerMock.Object);

    [Fact]
    public async Task GetByApplicationUserIdAsync_returns_null_if_id_invalid()
    {
        var sut = CreateService();

        var result = await sut.GetByApplicationUserIdAsync("");

        Assert.Null(result);
        _userRepoMock.Verify(r => r.GetByApplicationUserIdAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetByApplicationUserIdAsync_returns_user_when_found()
    {
        var sut = CreateService();
        var user = new User { Id = Guid.NewGuid(), ApplicationUserId = "app-1" };

        _userRepoMock
            .Setup(r => r.GetByApplicationUserIdAsync("app-1"))
            .ReturnsAsync(user);

        var result = await sut.GetByApplicationUserIdAsync("app-1");

        Assert.NotNull(result);
        Assert.Equal("app-1", result!.ApplicationUserId);
    }

    [Fact]
    public async Task GetByApplicationUserIdAsync_returns_null_if_repo_throws()
    {
        var sut = CreateService();

        _userRepoMock
            .Setup(r => r.GetByApplicationUserIdAsync("app-1"))
            .ThrowsAsync(new Exception("db error"));

        var result = await sut.GetByApplicationUserIdAsync("app-1");

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateUserAsync_returns_error_if_first_or_lastname_missing()
    {
        var sut = CreateService();
        var user = new User
        {
            ApplicationUserId = "app-1",
            FirstName = "",
            LastName = ""
        };

        var (success, error, created) = await sut.CreateUserAsync(user);

        Assert.False(success);
        Assert.Equal("Vor- und Nachname erforderlich", error);
        Assert.Null(created);
        _userRepoMock.Verify(r => r.GetByApplicationUserIdAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task CreateUserAsync_returns_error_if_user_already_exists()
    {
        var sut = CreateService();
        var user = new User
        {
            ApplicationUserId = "app-1",
            FirstName = "Max",
            LastName = "Mustermann"
        };

        _userRepoMock
            .Setup(r => r.GetByApplicationUserIdAsync("app-1"))
            .ReturnsAsync(new User());

        var (success, error, created) = await sut.CreateUserAsync(user);

        Assert.False(success);
        Assert.Equal("User existiert bereits", error);
        Assert.Null(created);
        _userRepoMock.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task CreateUserAsync_success_creates_user_with_new_guid()
    {
        var sut = CreateService();
        var user = new User
        {
            ApplicationUserId = "app-1",
            FirstName = "Max",
            LastName = "Mustermann"
        };

        _userRepoMock
            .Setup(r => r.GetByApplicationUserIdAsync("app-1"))
            .ReturnsAsync((User?)null);

        _userRepoMock
            .Setup(r => r.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => u);

        var (success, error, created) = await sut.CreateUserAsync(user);

        Assert.True(success);
        Assert.Null(error);
        Assert.NotNull(created);
        Assert.NotEqual(Guid.Empty, created!.Id);
    }

    [Fact]
    public async Task CreateUserAsync_returns_error_if_repo_throws()
    {
        var sut = CreateService();
        var user = new User
        {
            ApplicationUserId = "app-1",
            FirstName = "Max",
            LastName = "Mustermann"
        };

        _userRepoMock
            .Setup(r => r.GetByApplicationUserIdAsync("app-1"))
            .ReturnsAsync((User?)null);

        _userRepoMock
            .Setup(r => r.CreateAsync(It.IsAny<User>()))
            .ThrowsAsync(new Exception("db error"));

        var (success, error, created) = await sut.CreateUserAsync(user);

        Assert.False(success);
        Assert.Equal("Error creating user", error);
        Assert.Null(created);
    }
}
