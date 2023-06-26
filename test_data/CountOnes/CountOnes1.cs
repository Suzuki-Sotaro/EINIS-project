using System;

public class Program
{
    public static int CountOnes(int i)
    {
			int oneCount = 0;
			
			char[] binary = Convert.ToString(i, 2).ToCharArray();
			
			foreach (char number in binary)
			{
				if (number == '1')
				{
					oneCount++;
				}
			}
			
			return oneCount;
			
    }
}