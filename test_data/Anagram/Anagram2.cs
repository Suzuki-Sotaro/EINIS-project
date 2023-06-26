using System.Linq;
public class Program
{
public static bool AnagramStrStr(string needle, string haystack)
        {
            int ascii = needle.Select(x => (int)x).Sum();
            for (int i = 0; i < haystack.Length-needle.Length; i++)
            {
                int ascii2 = 0;
                for (int j = i; j < haystack.Length; j++)
                {
                    ascii2 += (int)haystack[j];
                    if (ascii == ascii2)
                        return true;
                }               
            }
            return false;
        }
}