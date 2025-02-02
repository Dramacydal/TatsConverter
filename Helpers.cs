using System.Text.RegularExpressions;

namespace TatsConverter;

public static class Helpers
{
    public static string NormalizeId(string id)
    {
        return Regex.Replace(id, "[^A-Z0-9_]", "_", RegexOptions.IgnoreCase);
    }
}
