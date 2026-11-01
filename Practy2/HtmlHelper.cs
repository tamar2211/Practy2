using System.Text.Json;

namespace Practy2;

public class HtmlHelper
{
    private static readonly HtmlHelper _instance = new HtmlHelper();
    public static HtmlHelper Instance => _instance;

    public string[] AllTags { get; set; }
    public string[] SelfClosingTags { get; set; }

    private HtmlHelper()
    {
        var allTagsJson = File.ReadAllText("HtmlTags.json");
        AllTags = JsonSerializer.Deserialize<string[]>(allTagsJson);

        var selfClosingJson = File.ReadAllText("HtmlVoidTags.json");
        SelfClosingTags = JsonSerializer.Deserialize<string[]>(selfClosingJson);
    }
}
