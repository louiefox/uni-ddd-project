using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace DDDProject
{
    public class SocietyChatHub : Hub
    {
        public const string HubUrl = "/chathub";

        public async Task Broadcast(string username, string message)
        {
            await Clients.All.SendAsync("Broadcast", username, message);
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"{Context.ConnectionId} connected");

            string username = Context.ConnectionId;
            Clients.Client(Context.ConnectionId).SendAsync("OnClientConnected", username);
            // Clients.All.SendAsync("UserConnected", username);
            
            return base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception e)
        {
            Console.WriteLine($"Disconnected {e?.Message} {Context.ConnectionId}");

            string username = Context.ConnectionId;
            // await Clients.All.SendAsync("UserDisconnected", username);

            await base.OnDisconnectedAsync(e);
        }
    }
}
