using System;
public class Program 
{
    public static int PowerRanger(int power, int min, int max) 
    {
			int a=0;;
			for(int i=1;Math.Pow(i,power)<=max;i++)
			{	if(Math.Pow(i,power)<=max&&Math.Pow(i,power)>=min)
				a++;}
			return a;
    }
}