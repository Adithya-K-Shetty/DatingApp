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
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public MessageHub(IMessageRepository messageRepository,IUserRepository userRepository,IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _messageRepository = messageRepository;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext(); //getting http context
            var otherUser = httpContext.Request.Query["user"]; //getting data from query params
            var groupName = GetGroupName(Context.User.GetUsername(),otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId,groupName); //users added to the group

            var messages = await _messageRepository.GetMessageThread(Context.User.GetUsername(),otherUser);

             await Clients.Group(groupName).SendAsync("ReceiveMessageThread",messages);
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(CreateMessageDto createMessageDto){
            var username = Context.User.GetUsername();

            if(username == createMessageDto.RecipientUserName.ToLower())
                throw new HubException("You cannot send messages to yourself");
            
            var sender =  await _userRepository.GetUserByUsernameAsync(username);

            var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUserName);

            if(recipient == null) throw new HubException("Not found user");

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUserName = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };
            _messageRepository.AddMessage(message); //now entity frame work starts tracking the messages

            if(await _messageRepository.SaveAllAsync()){
                var group = GetGroupName(sender.UserName,recipient.UserName);
                await Clients.Group(group).SendAsync("NewMessage",_mapper.Map<MessageDto>(message));
            }
        }
        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller,other) < 0;

            return stringCompare ? $"{caller} - {other}" : $"{other}-{caller}";
        }
    }
}