using Practy2;
using System.Text.RegularExpressions;

var html = await Load("https://example.com");

var cleanHtml = new Regex("\\s+").Replace(html, " ");
var htmlLines = new Regex("<(.*?)>").Split(cleanHtml).Where(s => s.Trim().Length > 0);

var root = BuildTree(htmlLines);

var selector = Selector.Parse("html body p");
var results = root.Find(selector);

Console.WriteLine($"Found {results.Count} elements:");
foreach (var element in results)
    Console.WriteLine($"<{element.Name}> id={element.Id} classes={string.Join(",", element.Classes)}");

async Task<string> Load(string url)
{
    using HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    return await response.Content.ReadAsStringAsync();
}

static HtmlElement BuildTree(IEnumerable<string> htmlLines)
{
    var helper = HtmlHelper.Instance;
    var root = new HtmlElement();
    var current = root;

    foreach (var line in htmlLines)
    {
        var text = line.Trim();
        var firstWord = text.Split(' ')[0];

        if (firstWord == "/html")
            break;

        if (firstWord.StartsWith("/"))
        {
            if (current.Parent != null && helper.AllTags.Contains(firstWord.Substring(1)))
                current = current.Parent;
            continue;
        }

        if (!helper.AllTags.Contains(firstWord))
        {
            current.InnerHtml = text;
            continue;
        }

        var element = new HtmlElement { Name = firstWord, Parent = current };
        current.Children.Add(element);

        var attributes = new Regex("([^\\s]*?)=\"(.*?)\"").Matches(text.Substring(firstWord.Length));
        foreach (Match attribute in attributes)
            element.Attributes[attribute.Groups[1].Value] = attribute.Groups[2].Value;

        if (element.Attributes.TryGetValue("id", out var id))
            element.Id = id;

        if (element.Attributes.TryGetValue("class", out var classNames))
            element.Classes = classNames.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

        bool isSelfClosing = text.EndsWith("/") || helper.SelfClosingTags.Contains(firstWord);
        if (!isSelfClosing)
            current = element;
    }

    return root;
}
