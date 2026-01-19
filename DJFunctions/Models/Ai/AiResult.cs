public class AiResult
{
    public bool IsSuccess { get; set; }
    public bool IsConfident { get; set; }
    public string Reason { get; set; }
    public Dictionary<string, string> ExtractedData { get; set; }
}
