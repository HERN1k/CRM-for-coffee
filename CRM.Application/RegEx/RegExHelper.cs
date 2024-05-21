
using System.Text.RegularExpressions;

namespace CRM.Application.RegEx
{
  public static class RegExHelper
  {
    public static bool ChackString(string input, string pattern)
    {
      return Regex.IsMatch(input, pattern);
    }
  }
}
