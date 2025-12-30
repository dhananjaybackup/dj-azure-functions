public class ActivityInput
{
    public string UserId { get; set; }
    public string UserName { get; set; }

    // Durable correlation
    public string InstanceId { get; set; }

    // Optional but powerful
    public string MessageId { get; set; }
}
