namespace CosmosDbDemo.Models
{
    public class GavUser
    {
        public string id { get; set; }
        public string userId { get; set; } // Partition key
        public string username { get; set; }
        public List<Engagement> engagement { get; set; }
        public UserDetails userdetails { get; set; }
    }
}
