using Xunit;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using WeatherForecast.Application.Interfaces;
using WeatherForecast.Application.Services;
using WeatherForecast.Domain.Models;
using WeatherForecast.Infrastructure.Models;

public class AuthServiceTests
{
    private Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        return new Mock<UserManager<ApplicationUser>>(
            store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
    }

    private Mock<SignInManager<ApplicationUser>> CreateSignInManagerMock(UserManager<ApplicationUser> userManager)
    {
        var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();

        return new Mock<SignInManager<ApplicationUser>>(
            userManager,
            contextAccessor.Object,
            claimsFactory.Object,
            null!, null!, null!, null!);
    }

    private Mock<RoleManager<IdentityRole>> CreateRoleManagerMock()
    {
        var store = new Mock<IRoleStore<IdentityRole>>();
        return new Mock<RoleManager<IdentityRole>>(
            store.Object, null!, null!, null!, null!);
    }

    private (AuthService sut,
             Mock<SignInManager<ApplicationUser>> signInMock,
             Mock<UserManager<ApplicationUser>> userManagerMock,
             Mock<RoleManager<IdentityRole>> roleManagerMock,
             Mock<IUserService> userServiceMock) CreateService()
    {
        var userManagerMock = CreateUserManagerMock();
        var signInMock = CreateSignInManagerMock(userManagerMock.Object);
        var roleManagerMock = CreateRoleManagerMock();
        var userServiceMock = new Mock<IUserService>();
        var loggerMock = new Mock<ILogger<AuthService>>();

        var sut = new AuthService(
            signInMock.Object,
            userManagerMock.Object,
            roleManagerMock.Object,
            userServiceMock.Object,
            loggerMock.Object);

        return (sut, signInMock, userManagerMock, roleManagerMock, userServiceMock);
    }

    [Fact]
    public async Task LoginAsync_success_returns_true()
    {
        var (sut, signInMock, _, _, _) = CreateService();

        signInMock
            .Setup(s => s.PasswordSignInAsync("test@test.de", "Pass!123", false, false))
            .ReturnsAsync(SignInResult.Success);

        var (success, error) = await sut.LoginAsync("test@test.de", "Pass!123");

        Assert.True(success);
        Assert.Null(error);
    }

    [Fact]
    public async Task LoginAsync_invalid_credentials_returns_false_with_error()
    {
        var (sut, signInMock, _, _, _) = CreateService();

        signInMock
            .Setup(s => s.PasswordSignInAsync("test@test.de", "wrong", false, false))
            .ReturnsAsync(SignInResult.Failed);

        var (success, error) = await sut.LoginAsync("test@test.de", "wrong");

        Assert.False(success);
        Assert.Equal("Ungültige Anmeldedaten", error);
    }

    [Fact]
    public async Task RegisterAsync_creates_identity_and_domain_user()
    {
        var (sut, _, userManagerMock, roleManagerMock, userServiceMock) = CreateService();

        userManagerMock
            .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), "Pass!123"))
            .ReturnsAsync(IdentityResult.Success);

        userServiceMock
            .Setup(s => s.CreateUserAsync(It.IsAny<User>()))
            .ReturnsAsync((true, (string?)null, new User { Id = Guid.NewGuid() }));

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
        Assert.NotNull(domainUserId);
    }
}
