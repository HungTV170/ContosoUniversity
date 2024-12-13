using Microsoft.AspNetCore.SignalR;

namespace ContosoUniversity.Hubs{

    public interface IChatClient{
        Task ReceiveMessage(string user, string messagemessage);
        Task<string> GetMessage();

        Task HelloClient(string message);
    }
    public class ChatHub : Hub<IChatClient>{
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.ReceiveMessage(user, message);
        }

        public async Task SendMessageToCaller(string user, string message){
            await Clients.Caller.ReceiveMessage(user, message);
        }

        public async Task SendMessageToGroup(string user, string message){
            await Clients.Group("SignalR Users").ReceiveMessage(user, message);
        }

        public async Task<string> WaitForMessage(string connectionId)
        {
            string message = await Clients.Client(connectionId).GetMessage();
            await Clients.Client(connectionId).HelloClient(message);
            return message;
        }

        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).ReceiveMessage("Send", $"{Context.ConnectionId} has joined the group {groupName}.");
        }

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).ReceiveMessage("Send", $"{Context.ConnectionId} has left the group {groupName}.");
        }
    }
}