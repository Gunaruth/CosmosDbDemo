namespace CosmosDbDemo.Models
{
    public class ComponentEngagement
    {
        public string id { get; set; }
        public string engagementId { get; set; } // Partition key
        public string engagementType { get; set; }
        public string createdBy { get; set; }
        public DateTime createdAt { get; set; }
        public string status { get; set; }
        public string opinionId { get; set; }
        public Location location { get; set; }
        public List<ComponentLinked> groupLinked { get; set; }
        public List<OpinionOption> opinionOptions { get; set; }
        public List<TeamMember> team { get; set; }
    }
    public class OpinionOption
    {
        public string engagementId { get; set; } // Partition key
        public string componentId { get; set; }
        public string opinionId { get; set; }
        public string name { get; set; }
        public string status { get; set; }
    }
}
