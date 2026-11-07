using System.Text.RegularExpressions;

namespace Gmc.Core;

/// <summary>
/// keepLocal patterns: files the patcher installs when missing but never overwrites or deletes.
/// Case-insensitive, "/" or "\" as separator; "*" and "?" match within one segment, "**" across segments.
/// A pattern ending in "/" matches everything below that folder.
/// </summary>
public sealed class KeepLocal
{
    readonly List<Regex> _patterns = new();

    public KeepLocal(IEnumerable<string> patterns)
    {
        foreach (string raw in patterns)
        {
            if (string.IsNullOrWhiteSpace(raw)) continue;
            string p = PathSafety.Normalize(raw.Trim());
            if (p.EndsWith('/')) p += "**";
            _patterns.Add(new Regex("^" + ToRegex(p) + "$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant));
        }
    }

    public bool Matches(string relPath)
    {
        string p = PathSafety.Normalize(relPath);
        foreach (var r in _patterns) if (r.IsMatch(p)) return true;
        return false;
    }

    static string ToRegex(string glob)
    {
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < glob.Length; i++)
        {
            char c = glob[i];
            if (c == '*' && i + 1 < glob.Length && glob[i + 1] == '*')
            {
                i++;
                if (i + 1 < glob.Length && glob[i + 1] == '/') { i++; sb.Append("(?:.*/)?"); }   // "**/" = any depth incl. none
                else sb.Append(".*");
            }
            else if (c == '*') sb.Append("[^/]*");
            else if (c == '?') sb.Append("[^/]");
            else sb.Append(Regex.Escape(c.ToString()));
        }
        return sb.ToString();
    }
}
