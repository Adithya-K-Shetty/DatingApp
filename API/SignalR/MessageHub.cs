using API.DTOs;
using API.Entities;
using API.Extensions;
using API.interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    [Authorize]
    public class MessageHub :Hub
    {
        
        private readonly IMapper _mapper;
          private readonly IHubContext<PresenceHub> _presenceHub;
        private readonly IUnitOfWork _uow;
        public MessageHub( IUnitOfWork uow,IMapper mapper,
        IHubContext<PresenceHub> presenceHub)
        {
            _uow = uow;
            _presenceHub = presenceHub;
            _mapper = mapper;
         
        }

        public override async Task OnConnectedAsync()
        {
            //http is used while setting up SignalR
            var httpContext = Context.GetHttpContext(); //getting http context
            var otherUser = httpContext.Request.Query["user"]; //getting data from query params
            var groupName = GetGroupName(Context.User.GetUsername(),otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId,groupName); //users added to the group
            await AddToGroup(groupName);

            var messages = await _uow.MessageRepository.GetMessageThread(Context.User.GetUsername(),otherUser);

            if(_uow.HasChanges()) await _uow.Complete();
            //adding connection to the group
             await Clients.Group(groupName).SendAsync("ReceiveMessageThread",messages);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await RemoveFromMessageGroup();
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(CreateMessageDto createMessageDto){
            var username = Context.User.GetUsername();

            if(username == createMessageDto.RecipientUserName.ToLower())
                throw new HubException("You cannot send messages to yourself");
            
            var sender =  await _uow.UserRepository.GetUserByUsernameAsync(username);

            var recipient = await _uow.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUserName);

            if(recipient == null) throw new HubException("Not found user");

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUserName = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };
            var groupName = GetGroupName(sender.UserName, recipient.UserName);

            var group = await _uow.MessageRepository.GetMessageGroup(groupName);

            if(group.Connections.Any(x => x.Username == recipient.UserName))
            {
                message.DateRead =  DateTime.UtcNow;
            }
            else{
                //GET USERS GROUP 
                var connections = await PresenceTracker.GetConnectionForUser(recipient.UserName);
                if(connections != null)
                {
                    await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived",
                        new {username = sender.UserName,KnowAs = sender.UserName});
                }
            }

            //as soon we add to the database we call signalr to send back the entered message for client to display
            _uow.MessageRepository.AddMessage(message); //now entity frame work starts tracking the messages

            if(await _uow.Complete()){
                // var group = GetGroupName(sender.UserName,recipient.UserName);
                await Clients.Group(groupName).SendAsync("NewMessage",_mapper.Map<MessageDto>(message));
            }
        }

        //get group name
        //basically depends on the person who first created the request
        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller,other) < 0;

            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }

        private async Task<bool> AddToGroup(string groupName)
        {
            var group = await _uow.MessageRepository.GetMessageGroup(groupName);

            var connection = new Connection(Context.ConnectionId,Context.User.GetUsername());   

            if(group == null)
            {
                 group = new Group(groupName);
                _uow.MessageRepository.AddGroup(group);        
            }

            group.Connections.Add(connection);
            return await _uow.Complete();
        }

        private async Task RemoveFromMessageGroup()
        {
            var connection = await _uow.MessageRepository.GetConnection(Context.ConnectionId);
            _uow.MessageRepository.RemoveConnection(connection);

            await _uow.Complete();
        }
    }
}