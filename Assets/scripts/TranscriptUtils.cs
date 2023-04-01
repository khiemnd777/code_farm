using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class TranscriptUtils
{
  public static string ReplaceAllYieldFunc(string script, List<string> yieldFncList)
  {
    var result = script;

    if (yieldFncList != null && yieldFncList.Any())
    {
      for (var i = 0; i < yieldFncList.Count; i++)
      {
        result = ReplaceBuiltinYieldFunc(result, yieldFncList[i]);
      }
    }

    return result;
  }

  public static string RemoveBuiltinModule(string script, string module)
  {
    var result = Regex.Replace(script, $@"from\s+__builtins__\.{module}\s+import\s+.+", "", RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.Multiline);
    result = Regex.Replace(result, $@"import\s+__builtins__\.{module}", "", RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.Multiline);
    return Regex.Replace(script, $@"from\s+__builtins__\.{module}\s+import\s+.+", "", RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.Multiline);
  }

  public static string RemoveAsyncKeyword(string script)
  {
    return Regex.Replace(script, $@"async\s*", "", RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.Multiline);
  }

  public static string ReplaceBuiltinYieldFunc(string script, string builtinFn)
  {
    var result = Regex.Replace(script, $@"await\s{builtinFn}\s*\(", $"{builtinFn}(", RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.Multiline);
    result = Regex.Replace(result, $@"yield\s{builtinFn}\s*\(", $"{builtinFn}(", RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.Multiline);
    return Regex.Replace(result, $@"{builtinFn}\s*\(", $"yield {builtinFn}(", RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.Multiline);
  }

  public static string ReplaceAwaitKeywordToYield(string script)
  {
    return Regex.Replace(script, @"await", "yield", RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.Multiline);
  }
}