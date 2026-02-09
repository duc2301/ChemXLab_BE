using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChemXLabWebAPI.Hubs
{
    public class PaymentHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            var connectionId = Context.ConnectionId;
            Console.WriteLine($"PaymentHub connected - UserId: '{userId}', ConnectionId: {connectionId}");

            foreach (var claim in Context.User?.Claims ?? Enumerable.Empty<Claim>())
            {
                Console.WriteLine($"Claim: {claim.Type} = {claim.Value}");
            }

            return base.OnConnectedAsync();
        }
    }
}
