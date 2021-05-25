using System.Collections.Generic;
using System.Security.Claims;

namespace CoreSnip.Tests.IntegrationTests
{
    public interface IAuthenticatedUserInfo
    {
        string AuthenticationScheme { get; set; }
        IEnumerable<Claim> Claims { get; set; }
    }

    public class AuthenticatedUserInfo : IAuthenticatedUserInfo
    {
        public string AuthenticationScheme { get; set; }
        public IEnumerable<Claim> Claims { get; set; }
    }
}