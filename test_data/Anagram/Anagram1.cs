public class Program
{
    public static bool AnagramStrStr(string needle, string haystack)
    {
			bool started = false;
            foreach(var c in haystack)
            {
                var pos = needle.IndexOf(c);
                if(pos > -1)
                {
									started = true;
                    needle = needle.Remove(pos, 1);
									  if(needle.Length == 0)
                    {
                        return true;
                    }
                }
                else if (started)
                {
                    return false;
                }
            }   
			return false;
    }
}