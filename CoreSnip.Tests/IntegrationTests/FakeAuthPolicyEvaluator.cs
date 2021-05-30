using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoreSnip.Tests.IntegrationTests
{

    public class FakeAuthPolicyEvaluator : IPolicyEvaluator
    {
        private readonly IAuthenticatedUserInfo authenticatedUser;

        public FakeAuthPolicyEvaluator(IAuthenticatedUserInfo authenticatedUser)
        {
            this.authenticatedUser = authenticatedUser;
        }

        public virtual async Task<AuthenticateResult> AuthenticateAsync(AuthorizationPolicy policy, HttpContext context)
        {
            var principal = new ClaimsPrincipal();
            principal.AddIdentity(new ClaimsIdentity(authenticatedUser.Claims, authenticatedUser.AuthenticationScheme));

            return await Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, new AuthenticationProperties(), authenticatedUser.AuthenticationScheme)));
        }

        public virtual async Task<PolicyAuthorizationResult> AuthorizeAsync(AuthorizationPolicy policy, AuthenticateResult authenticationResult, HttpContext context, object resource)
            => await Task.FromResult(PolicyAuthorizationResult.Success());
    }
}
