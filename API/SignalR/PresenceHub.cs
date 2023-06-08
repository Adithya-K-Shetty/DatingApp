using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    [Authorize]
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _tracker;

        public PresenceHub(PresenceTracker tracker){
            _tracker = tracker;
            
        }
        public override async Task OnConnectedAsync()
        {
            //invoking methods over on the 
            //clients connected to the hub

            //adding connected user connection id and username to the dictionary stored inside the server
            await _tracker.UserConnected(Context.User.GetUsername(),Context.ConnectionId);
            
            await Clients.Others.SendAsync("UserIsOnline",Context.User.GetUsername());

            //getting a list of connected users
            var currentUsers = await _tracker.GetOnlineUsers();
            //sending the data of all the online users to all the users
            await Clients.All.SendAsync("GetOnlineUsers",currentUsers);

        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
           await _tracker.UserDisconnected(Context.User.GetUsername(),Context.ConnectionId);
           await Clients.Others.SendAsync("UserIsOffline",Context.User.GetUsername());
           var currentUsers = await _tracker.GetOnlineUsers();
           await Clients.All.SendAsync("GetOnlineUsers",currentUsers);
           await base.OnDisconnectedAsync(exception);
        }
    }
}