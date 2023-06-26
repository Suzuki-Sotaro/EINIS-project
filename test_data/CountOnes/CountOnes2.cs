using System;
using System.Linq;
public class Program
{
    public static int CountOnes(int i)
    {
			string binary = Convert.ToString(i, 2);
            int count = 0;
            for(int x=0; x<binary.Length; x++)
            {
                if (binary[x] == '1') count++;
            }
            return count;
    }
}