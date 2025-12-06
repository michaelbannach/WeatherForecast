using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using WeatherForecast.Application.Interfaces;
using WeatherForecast.Application.Services;
using WeatherForecast.Domain.Models;
using Xunit;

namespace WeatherForecast.Application.Tests.Services;

public class FavoriteServiceTests
{
    private readonly Mock<IFavoriteRepository> _favoriteRepoMock = new();
    private readonly Mock<IUserService> _userServiceMock = new();
    private readonly Mock<ILogger<FavoriteService>> _loggerMock = new();

    private FavoriteService CreateService()
        => new FavoriteService(_favoriteRepoMock.Object, _userServiceMock.Object, _loggerMock.Object);

    [Fact]
    public async Task GetFavoritesAsync_throws_if_userId_empty()
    {
        var sut = CreateService();

        await Assert.ThrowsAsync<ArgumentException>(() => sut.GetFavoritesAsync(""));
    }

    [Fact]
    public async Task GetFavoritesAsync_throws_if_user_not_found()
    {
        var sut = CreateService();
        _userServiceMock
            .Setup(s => s.GetByApplicationUserIdAsync("app-1"))
            .ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<ArgumentException>(() => sut.GetFavoritesAsync("app-1"));
    }

    [Fact]
    public async Task GetFavoritesAsync_returns_favorites_for_user()
    {
        var sut = CreateService();

        var domainUser = new User { Id = Guid.NewGuid(), ApplicationUserId = "app-1" };
        _userServiceMock
            .Setup(s => s.GetByApplicationUserIdAsync("app-1"))
            .ReturnsAsync(domainUser);

        var favorites = new List<Favorite>
        {
            new Favorite { Id = 1, City = "Berlin", Country = "DE", UserId = domainUser.Id }
        };

        _favoriteRepoMock
            .Setup(r => r.GetFavoritesAsync(domainUser.Id))
            .ReturnsAsync(favorites);

        var result = await sut.GetFavoritesAsync("app-1");

        Assert.Single(result);
        Assert.Equal("Berlin", result[0].City);
    }

    [Fact]
    public async Task AddFavoriteAsync_returns_error_if_applicationUserId_empty()
    {
        var sut = CreateService();

        var (added, error) = await sut.AddFavoriteAsync("", new Favorite());

        Assert.False(added);
        Assert.Equal("Unknown User", error);
    }

    [Fact]
    public async Task AddFavoriteAsync_returns_error_if_user_not_found()
    {
        var sut = CreateService();
        _userServiceMock
            .Setup(s => s.GetByApplicationUserIdAsync("app-1"))
            .ReturnsAsync((User?)null);

        var (added, error) = await sut.AddFavoriteAsync("app-1", new Favorite());

        Assert.False(added);
        Assert.Equal("User not found", error);
    }

    [Fact]
    public async Task AddFavoriteAsync_returns_error_if_city_or_country_empty()
    {
        var sut = CreateService();
        var user = new User { Id = Guid.NewGuid(), ApplicationUserId = "app-1" };

        _userServiceMock
            .Setup(s => s.GetByApplicationUserIdAsync("app-1"))
            .ReturnsAsync(user);

        var favorite = new Favorite { City = "", Country = "" };

        var (added, error) = await sut.AddFavoriteAsync("app-1", favorite);

        Assert.False(added);
        Assert.Equal("City and Country must not be empty", error);
    }

    [Fact]
    public async Task AddFavoriteAsync_returns_error_if_already_exists()
    {
        var sut = CreateService();
        var user = new User { Id = Guid.NewGuid(), ApplicationUserId = "app-1" };

        _userServiceMock
            .Setup(s => s.GetByApplicationUserIdAsync("app-1"))
            .ReturnsAsync(user);

        var favorite = new Favorite { City = "Berlin", Country = "DE" };

        _favoriteRepoMock
            .Setup(r => r.AlreadyExistsAsync(user.Id, "Berlin", "DE"))
            .ReturnsAsync(true);

        var (added, error) = await sut.AddFavoriteAsync("app-1", favorite);

        Assert.False(added);
        Assert.Equal("City already exists", error);
    }

    [Fact]
    public async Task AddFavoriteAsync_returns_error_if_more_than_5()
    {
        var sut = CreateService();
        var user = new User { Id = Guid.NewGuid(), ApplicationUserId = "app-1" };

        _userServiceMock
            .Setup(s => s.GetByApplicationUserIdAsync("app-1"))
            .ReturnsAsync(user);

        var favorite = new Favorite { City = "Berlin", Country = "DE" };

        _favoriteRepoMock
            .Setup(r => r.AlreadyExistsAsync(user.Id, "Berlin", "DE"))
            .ReturnsAsync(false);

        _favoriteRepoMock
            .Setup(r => r.CountFavoritesAsync(user.Id))
            .ReturnsAsync(5);

        var (added, error) = await sut.AddFavoriteAsync("app-1", favorite);

        Assert.False(added);
        Assert.Equal("Maximal 5 allowed", error);
    }

    [Fact]
    public async Task AddFavoriteAsync_returns_error_if_repo_cannot_save()
    {
        var sut = CreateService();
        var user = new User { Id = Guid.NewGuid(), ApplicationUserId = "app-1" };

        _userServiceMock
            .Setup(s => s.GetByApplicationUserIdAsync("app-1"))
            .ReturnsAsync(user);

        _favoriteRepoMock
            .Setup(r => r.AlreadyExistsAsync(user.Id, "Berlin", "DE"))
            .ReturnsAsync(false);

        _favoriteRepoMock
            .Setup(r => r.CountFavoritesAsync(user.Id))
            .ReturnsAsync(0);

        _favoriteRepoMock
            .Setup(r => r.AddFavoriteAsync(It.IsAny<Favorite>()))
            .ReturnsAsync(false);

        var favorite = new Favorite { City = "Berlin", Country = "DE" };

        var (added, error) = await sut.AddFavoriteAsync("app-1", favorite);

        Assert.False(added);
        Assert.Equal("Saving not possible", error);
    }

    [Fact]
    public async Task AddFavoriteAsync_success_creates_favorite()
    {
        var sut = CreateService();
        var user = new User { Id = Guid.NewGuid(), ApplicationUserId = "app-1" };

        _userServiceMock
            .Setup(s => s.GetByApplicationUserIdAsync("app-1"))
            .ReturnsAsync(user);

        _favoriteRepoMock
            .Setup(r => r.AlreadyExistsAsync(user.Id, "Berlin", "DE"))
            .ReturnsAsync(false);

        _favoriteRepoMock
            .Setup(r => r.CountFavoritesAsync(user.Id))
            .ReturnsAsync(0);

        _favoriteRepoMock
            .Setup(r => r.AddFavoriteAsync(It.IsAny<Favorite>()))
            .ReturnsAsync(true);

        var favorite = new Favorite { City = " Berlin ", Country = "de" };

        var (added, error) = await sut.AddFavoriteAsync("app-1", favorite);

        Assert.True(added);
        Assert.Null(error);

        _favoriteRepoMock.Verify(r => r.AddFavoriteAsync(
            It.Is<Favorite>(f => f.City == "Berlin" && f.Country == "DE")), Times.Once);
    }

    [Fact]
    public async Task DeleteByIdAsync_returns_error_if_userId_empty()
    {
        var sut = CreateService();

        var (success, error) = await sut.DeleteByIdAsync("", 42);

        Assert.False(success);
        Assert.Equal("User must not be empty", error);
    }

    [Fact]
    public async Task DeleteByIdAsync_returns_error_if_user_not_found()
    {
        var sut = CreateService();

        _userServiceMock
            .Setup(s => s.GetByApplicationUserIdAsync("app-1"))
            .ReturnsAsync((User?)null);

        var (success, error) = await sut.DeleteByIdAsync("app-1", 42);

        Assert.False(success);
        Assert.Equal("User not found", error);
    }

    [Fact]
    public async Task DeleteByIdAsync_returns_error_if_repo_returns_false()
    {
        var sut = CreateService();
        var user = new User { Id = Guid.NewGuid(), ApplicationUserId = "app-1" };

        _userServiceMock
            .Setup(s => s.GetByApplicationUserIdAsync("app-1"))
            .ReturnsAsync(user);

        _favoriteRepoMock
            .Setup(r => r.DeleteByIdAsync(user.Id, 42))
            .ReturnsAsync(false);

        var (success, error) = await sut.DeleteByIdAsync("app-1", 42);

        Assert.False(success);
        Assert.Equal("Favorite not found or deleted", error);
    }

    [Fact]
    public async Task DeleteByIdAsync_success_deletes_favorite()
    {
        var sut = CreateService();
        var user = new User { Id = Guid.NewGuid(), ApplicationUserId = "app-1" };

        _userServiceMock
            .Setup(s => s.GetByApplicationUserIdAsync("app-1"))
            .ReturnsAsync(user);

        _favoriteRepoMock
            .Setup(r => r.DeleteByIdAsync(user.Id, 42))
            .ReturnsAsync(true);

        var (success, error) = await sut.DeleteByIdAsync("app-1", 42);

        Assert.True(success);
        Assert.Null(error);

        _favoriteRepoMock.Verify(r => r.DeleteByIdAsync(user.Id, 42), Times.Once);
    }
}
