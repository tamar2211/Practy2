using System.Text.RegularExpressions;

namespace Practy2;

public class Selector
{
    public string? TagName { get; set; }
    public string? Id { get; set; }
    public List<string> Classes { get; set; } = new List<string>();

    public Selector? Parent { get; set; }
    public Selector? Child { get; set; }

    public static Selector Parse(string query)
    {
        var levels = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var root = new Selector();
        var current = root;

        for (int i = 0; i < levels.Length; i++)
        {
            var parts = new Regex("(?=[#.])").Split(levels[i]).Where(p => p.Length > 0);

            foreach (var part in parts)
            {
                if (part.StartsWith("#"))
                    current.Id = part.Substring(1);
                else if (part.StartsWith("."))
                    current.Classes.Add(part.Substring(1));
                else if (HtmlHelper.Instance.AllTags.Contains(part))
                    current.TagName = part;
            }

            if (i == levels.Length - 1)
                break;

            var child = new Selector { Parent = current };
            current.Child = child;
            current = child;
        }

        return root;
    }

    public bool IsMatch(HtmlElement element)
    {
        if (TagName != null && TagName != element.Name)
            return false;
        if (Id != null && Id != element.Id)
            return false;
        return Classes.All(c => element.Classes.Contains(c));
    }
}
