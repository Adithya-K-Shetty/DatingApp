namespace API.SignalR
{
    public class PresenceTracker
    {
        //first parameter is the username
        //second parameter is the list of connection id's (one id for each of the device)
        private static readonly Dictionary<string,List<string>> OnlineUsers = new Dictionary<string, List<string>>();

        public Task UserConnected(string username, string connectionId)
        {
            //dictionary is not thread safe
            //lock user when accessing the dictionary
            lock(OnlineUsers){
                if(OnlineUsers.ContainsKey(username))
                {
                    OnlineUsers[username].Add(connectionId);
                }
                else{
                    OnlineUsers.Add(username,new List<string>{connectionId});   
                }
            }
            return Task.CompletedTask;
        }  
        public Task UserDisconnected(string username, string connectionId)
        {
            lock(OnlineUsers){
                if(!OnlineUsers.ContainsKey(username)) return Task.CompletedTask;

                OnlineUsers[username].Remove(connectionId);

                if(OnlineUsers[username].Count == 0)
                {
                    OnlineUsers.Remove(username);
                }
            }
            return Task.CompletedTask;
        }
        //method to notify logged users to the current logged in user
        public Task<string[]> GetOnlineUsers(){
            string[] onlineUsers;
            lock(OnlineUsers)
            {
                onlineUsers = OnlineUsers.OrderBy(k => k.Key).Select(k =>k.Key).ToArray();
            }
            return Task.FromResult(onlineUsers);
        }
    }
}