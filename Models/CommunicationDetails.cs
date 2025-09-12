namespace CosmosDbDemo.Models
{
    public class CommunicationDetails
    {
        public string id { get; set; }
        public string chatId { get; set; }
        public string GroupEngagementId { get; set; }
        public string ComponentEngagementId { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<Participant> Participants { get; set; }
        public List<Message> Messages { get; set; }
    }

public class Participant
{
    public string UserId { get; set; }
    public string Role { get; set; } // group_auditor, component_auditor, etc.
    public DateTime? JoinedAt { get; set; }
}

public class Message
{
    public string MessageId { get; set; }
    public string SenderId { get; set; }
    public DateTime Timestamp { get; set; }
    public string Type { get; set; } // text, task_assignment, etc.
    public string Content { get; set; }
    //public string TaskId { get; set; }
    public string Status { get; set; } // accepted, rejected (optional)
    public List<string> VisibleTo { get; set; }
    //public MessageData Data { get; set; }
}

//public class MessageData
//{
//    public string Summary { get; set; }
//    public string FileUrl { get; set; }
//}
 
}
