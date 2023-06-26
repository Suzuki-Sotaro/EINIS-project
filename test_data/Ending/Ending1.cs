public class Program
{
    public static string LongestCommonEnding(string str1, string str2)
    {
			string s = str1.Length > str2.Length ? str2 : str1;
			string b = str1.Length > str2.Length ? str1 : str2;
			string common = string.Empty;
			for (int i = s.Length - 1, j = 0; i >= 0; i--, j++)
			{
				if (s[i] == b[b.Length - 1 - j])
					common = s[i] + common;
				else 
					break;
			}
			return common;
    }
}