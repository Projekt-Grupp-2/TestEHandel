using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace TestEHandel.Service
{
    public class MockAuthenticationStateProvider : AuthenticationStateProvider
    {


        private readonly ClaimsPrincipal _user;


        public MockAuthenticationStateProvider()
        {
            // Skapa en fejkad användare med UserId och andra claims
            var identity = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.Name, "FakeUser"),
            new Claim(ClaimTypes.Role, "User"), // Lägger till roll
            new Claim(ClaimTypes.Email, "fakeuser@example.com"),
            new Claim(ClaimTypes.NameIdentifier, "00000000-0000-0000-0000-000000000008"), // Här kan du sätta UserId   //useridGuid: d2b4f793-8c17-49a9-86fd-7a3f5e446e4e //00000000-0000-0000-0000-000000000000
            
        }, "mock");

            _user = new ClaimsPrincipal(identity);
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return Task.FromResult(new AuthenticationState(_user));
        }






    }
}
