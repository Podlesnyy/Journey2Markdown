namespace Convertor.JourneyJsons;

public class JourneyRecord
{
    public string id { get; set; }
    public long date_modified { get; set; }
    public long date_journal { get; set; }
    public string timezone { get; set; }
    public string text { get; set; }
    public string preview_text { get; set; }
    public int mood { get; set; }
    public float lat { get; set; }
    public float lon { get; set; }
    public string address { get; set; }
    public string label { get; set; }
    public string folder { get; set; }
    public int sentiment { get; set; }
    public bool favourite { get; set; }
    public string music_title { get; set; }
    public string music_artist { get; set; }
    public object[] photos { get; set; }
    public Weather weather { get; set; }
    public string[] tags { get; set; }
    public string type { get; set; }
}