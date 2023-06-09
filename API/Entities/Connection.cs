namespace API.Entities
{
    public class Connection
    {
        //reason for creating empty constructor
        //is when EF creates the schema it doesnt want the connectionID,username to be passed
        public Connection()
        {
            
        }
        public Connection(string connectionId, string username)
        {
            ConnectionId = connectionId;
            Username = username;
        }

        public string ConnectionId { get; set; }
        public string Username { get; set; }
    }
}