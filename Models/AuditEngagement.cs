namespace CosmosDbDemo.Models
{
    public class AuditEngagement
    {
        public string id { get; set; }
        public string engagementId { get; set; } // Partition key
        public string engagementType { get; set; }
        public string createdBy { get; set; }
        public DateTime createdAt { get; set; }
        public string status { get; set; }
        public bool isGroup { get; set; }
        public string opinionId { get; set; }
        public Location location { get; set; }
        public List<ComponentLinked> componentsLinked { get; set; }
        public List<TeamMember> team { get; set; }
        public string userId { get; set; }
    }

    public class Location
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class ComponentLinked
    {
        public string name { get; set; }
        public string status { get; set; }
    }

    public class TeamMember
    {
        public string user { get; set; }
        public string role { get; set; }
    }

    public class OpinionOption
    {
        public string engagement_id { get; set; } // Partition key
        public string component_id { get; set; }
        public string opinionid { get; set; }
        public string name { get; set; }
        public string status { get; set; }
    }

}
