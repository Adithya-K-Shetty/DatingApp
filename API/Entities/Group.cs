using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
   
    public class Group
    {
        public Group()
        {
            
        }

        public Group(string name)
        {
            Name = name;
        }


        [Key] //we are representing that Name is the primary key
        //Name is the name of the group
        //name of the group has to be unique
        public string Name { get; set; }

        public ICollection<Connection> Connections {get;set;} = new List<Connection>();
    }
}