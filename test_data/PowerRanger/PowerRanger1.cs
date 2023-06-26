using System;
public class Program 
{
    public static int PowerRanger(int power, int min, int max) 
    {
			int db=0;
			
			int i=1;
			
			while (Math.Pow((double) i, power)<=max) {
				if (Math.Pow((double) i, power)>=min) db++;
				i++;
			}
			return db;
    }
}