namespace Practy2;

public static class HtmlElementExtensions
{
    public static HashSet<HtmlElement> Find(this HtmlElement element, Selector selector)
    {
        var result = new HashSet<HtmlElement>();
        FindRecursive(element, selector, result);
        return result;
    }

    private static void FindRecursive(HtmlElement element, Selector selector, HashSet<HtmlElement> result)
    {
        var matches = element.Descendants().Skip(1).Where(selector.IsMatch);

        if (selector.Child == null)
        {
            result.UnionWith(matches);
            return;
        }

        foreach (var match in matches)
            FindRecursive(match, selector.Child, result);
    }
}
