using CtiHub.WebApi.Controllers;
using Microsoft.AspNetCore.Authorization;
using Xunit;

namespace CtiHub.Tests.Controllers;

public class ScanControllerAuthorizationTests
{
    [Fact]
    public void ScanController_ShouldRequireAuthorization()
    {
        var controllerType = typeof(ScanController);

        var hasAuthorize = controllerType
            .GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true)
            .Any();

        Assert.True(hasAuthorize);
    }

    [Fact]
    public void ScanController_Actions_ShouldNotAllowAnonymous()
    {
        var controllerType = typeof(ScanController);

        var hasAllowAnonymousOnActions = controllerType
            .GetMethods()
            .Where(m => m.DeclaringType == controllerType)
            .Any(m => m.GetCustomAttributes(typeof(AllowAnonymousAttribute), inherit: true).Any());

        Assert.False(hasAllowAnonymousOnActions);
    }
}
