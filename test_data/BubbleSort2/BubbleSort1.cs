using System; 
public class Bubble_Sort  
{  
   public static void Main(string[] args)
         { 
            int[] tab = { 3, 0, 2, 5, -1, 4, 1 }; 
			int t;
            foreach (int element in tab)                                          
            for (int p = 0; p <= tab.Length - 2; p++)
            {
                for (int i = 0; i <= tab.Length - 2; i++)
                {
                    if (tab[i] > tab[i + 1])
                    {
                        t = tab[i + 1];
                        tab[i + 1] = tab[i];
                        tab[i] = t;
                    }
                } 
            }                      

        }
}
