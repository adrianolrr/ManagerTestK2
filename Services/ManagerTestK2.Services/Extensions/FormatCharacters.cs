using System.Text.Json;
using System.Text.RegularExpressions;

namespace ManagerTestK2.Extensions
{
    public static class FormatCharacters
    {
        public static long RemoveNonNumber(string rgNumber)
        {
            var resultString = Regex.Match(rgNumber, @"\d+").Value;

            return long.Parse(resultString);
        }
    }
}
