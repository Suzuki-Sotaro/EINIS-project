using System;
public class Program
{
    public static string LongestCommonEnding(string str1, string str2)
    {
        string answer = "";

        int MinLen = Math.Min(str1.Length, str2.Length)+1;

        for (int i = 1; i < MinLen; i++)
        {
            if (str1[str1.Length - i] == str2[str2.Length - i])
                answer = str1[str1.Length - i] + answer;
            else
                return answer;
        }
        return answer;
    }		
}