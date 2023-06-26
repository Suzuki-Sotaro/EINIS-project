using System; 
public class Bubble_Sort  
{  
   public static void Main(string[] args)
         { 
            int[] Table = { 3, 0, 2, 5, -1, 4, 1 }; 
			int temp;
            foreach (int item in Table)                                          
            for (int i = 0; i <= Table.Length - 2; i++)
            {
                for (int k = 0; k <= Table.Length - 2; k++)
                {
                    if (Table[k] > Table[k + 1])
                    {
                        temp = Table[k + 1];
                        Table[k + 1] = Table[k];
                        Table[k] = temp;
                    }
                } 
            }                   

        }
}
