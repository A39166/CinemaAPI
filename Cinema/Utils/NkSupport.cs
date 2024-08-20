using System.Text.RegularExpressions;

namespace CinemaAPI.Utils
{
    public class NkSupport
    {
        public static bool IsUsernameValid(string username)
        {
            // Regular expression to check for special characters
            string pattern = @"^[a-zA-Z0-9_]*$";

            // Check if the username matches the pattern
            return Regex.IsMatch(username, pattern);
        }
    }
}
