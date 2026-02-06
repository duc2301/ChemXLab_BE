using Microsoft.AspNetCore.SignalR;

namespace ChemXLabWebAPI.Hubs
{
    public class PaymentHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst("UserId")?.Value;
            var connectionId = Context.ConnectionId;
            Console.WriteLine($"PaymentHub connected - UserId: '{userId}', ConnectionId: {connectionId}");
            return base.OnConnectedAsync();
        }
    }
}
