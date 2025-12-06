using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using WeatherForecast.Application.Interfaces;
using WeatherForecast.Application.Services;
using WeatherForecast.Domain.Models;
using WeatherForecast.Infrastructure.Models;
using Xunit;

namespace WeatherForecast.Application.Tests.Services;

public class AuthServiceTests
{
    private static Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        return new Mock<UserManager<ApplicationUser>>(
            store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
    }

    private static Mock<SignInManager<ApplicationUser>> CreateSignInManagerMock(UserManager<ApplicationUser> userManager)
    {
        var contextAccessor = new Mock<IHttpContextAccessor>();
        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();

        return new Mock<SignInManager<ApplicationUser>>(
            userManager,
            contextAccessor.Object,
            claimsFactory.Object,
            null!, null!, null!, null!);
    }

    private static Mock<RoleManager<IdentityRole>> CreateRoleManagerMock()
    {
        var store = new Mock<IRoleStore<IdentityRole>>();
        return new Mock<RoleManager<IdentityRole>>(
            store.Object, null!, null!, null!, null!);
    }

    private (AuthService sut,
        Mock<SignInManager<ApplicationUser>> signInMock,
        Mock<UserManager<ApplicationUser>> userManagerMock,
        Mock<RoleManager<IdentityRole>> roleManagerMock,
        Mock<IUserService> userServiceMock)
        CreateService()
    {
        var userManagerMock = CreateUserManagerMock();
        var signInMock = CreateSignInManagerMock(userManagerMock.Object);
        var roleManagerMock = CreateRoleManagerMock();
        var userServiceMock = new Mock<IUserService>();
        var loggerMock = new Mock<ILogger<AuthService>>();

        var inMemorySettings = new Dictionary<string, string?>
        {
            ["Jwt:Key"] = "SuperSecretTestKeySuperSecretTestKey",
            ["Jwt:Issuer"] = "TestIssuer",
            ["Jwt:Audience"] = "TestAudience",
            ["Jwt:ExpiresMinutes"] = "60"
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        var sut = new AuthService(
            signInMock.Object,
            userManagerMock.Object,
            roleManagerMock.Object,
            userServiceMock.Object,
            loggerMock.Object,
            configuration);

        return (sut, signInMock, userManagerMock, roleManagerMock, userServiceMock);
    }

    [Fact]
    public async Task LoginAsync_success_returns_true()
    {
        var (sut, _, userManagerMock, _, _) = CreateService();

        var user = new ApplicationUser
        {
            Id = "user-1",
            Email = "test@test.de",
            UserName = "test@test.de"
        };

        userManagerMock
            .Setup(m => m.FindByEmailAsync("test@test.de"))
            .ReturnsAsync(user);

        userManagerMock
            .Setup(m => m.CheckPasswordAsync(user, "Pass!123"))
            .ReturnsAsync(true);

        userManagerMock
            .Setup(m => m.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "User" });

        var (success, error, token) = await sut.LoginAsync("test@test.de", "Pass!123");

        Assert.True(success);
        Assert.Null(error);
        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    [Fact]
    public async Task LoginAsync_invalid_credentials_returns_error()
    {
        var (sut, _, userManagerMock, _, _) = CreateService();

        var user = new ApplicationUser
        {
            Id = "user-1",
            Email = "test@test.de",
            UserName = "test@test.de"
        };

        userManagerMock
            .Setup(m => m.FindByEmailAsync("test@test.de"))
            .ReturnsAsync(user);

        userManagerMock
            .Setup(m => m.CheckPasswordAsync(user, "wrong"))
            .ReturnsAsync(false);

        var (success, error, token) = await sut.LoginAsync("test@test.de", "wrong");

        Assert.False(success);
        Assert.Equal("LoginAsync, CheckPassword: Invalid Login", error);
        Assert.Null(token);
    }


    [Fact]
    public async Task LoginAsync_returns_internal_error_if_exception_thrown()
    {
        var (sut, _, userManagerMock, _, _) = CreateService();

        userManagerMock
            .Setup(m => m.FindByEmailAsync("test@test.de"))
            .ThrowsAsync(new Exception("boom"));

        var (success, error, token) = await sut.LoginAsync("test@test.de", "Pass!123");

        Assert.False(success);
        Assert.Equal("LoginAsync:Internal Server error", error);
        Assert.Null(token);
    }


    [Fact]
    public async Task RegisterAsync_returns_error_if_identity_creation_fails()
    {
        var (sut, _, userManagerMock, _, _) = CreateService();

        userManagerMock
            .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), "Pass!123"))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Invalid" }));

        var (success, error, appUserId, domainUserId) =
            await sut.RegisterAsync("test@test.de", "Pass!123", "Max", "Mustermann");

        Assert.False(success);
        Assert.Contains("Invalid", error);
        Assert.Null(appUserId);
        Assert.Null(domainUserId);
    }

    [Fact]
    public async Task RegisterAsync_returns_error_if_domain_user_creation_fails()
    {
        var (sut, _, userManagerMock, _, userServiceMock) = CreateService();

        userManagerMock
            .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), "Pass!123"))
            .ReturnsAsync(IdentityResult.Success);

        userServiceMock
            .Setup(s => s.CreateUserAsync(It.IsAny<User>()))
            .ReturnsAsync((false, "DomainError", (User?)null));

        userManagerMock
            .Setup(m => m.DeleteAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Success);

        var (success, error, appUserId, domainUserId) =
            await sut.RegisterAsync("test@test.de", "Pass!123", "Max", "Mustermann");

        Assert.False(success);
        Assert.Equal("DomainError", error);
        Assert.Null(appUserId);
        Assert.Null(domainUserId);

        userManagerMock.Verify(m => m.DeleteAsync(It.IsAny<ApplicationUser>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_success_creates_user_and_assigns_role()
    {
        var (sut, _, userManagerMock, roleManagerMock, userServiceMock) = CreateService();

        userManagerMock
            .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), "Pass!123"))
            .ReturnsAsync(IdentityResult.Success);

        var createdDomainUser = new User { Id = Guid.NewGuid() };

        userServiceMock
            .Setup(s => s.CreateUserAsync(It.IsAny<User>()))
            .ReturnsAsync((true, (string?)null, createdDomainUser));

        roleManagerMock
            .Setup(r => r.RoleExistsAsync("User"))
            .ReturnsAsync(true);

        userManagerMock
            .Setup(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), "User"))
            .ReturnsAsync(IdentityResult.Success);

        var (success, error, appUserId, domainUserId) =
            await sut.RegisterAsync("test@test.de", "Pass!123", "Max", "Mustermann");

        Assert.True(success);
        Assert.Null(error);
        Assert.False(string.IsNullOrWhiteSpace(appUserId));
        Assert.Equal(createdDomainUser.Id, domainUserId);
    }
}
