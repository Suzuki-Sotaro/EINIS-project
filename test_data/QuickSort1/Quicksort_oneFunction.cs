using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quick_Sort
{
    class Program
    {
        private static void Quick_Sort(int[] arr, int left, int right) 
        {
            if (left < right)
            {
                int p;
                int pivot = arr[left];
                while (true)
                {

                    while (arr[left] < pivot)
                    {
                        left++;
                    }

                    while (arr[right] > pivot)
                    {
                        right--;
                    }

                    if (left < right)
                    {
                        if (arr[left] == arr[right]) p= right;

                        int temp = arr[left];
                        arr[left] = arr[right];
                        arr[right] = temp;


                    }
                    else
                    {
                        p= right;
                    }
                }


                if (p > 1) {
                    Quick_Sort(arr, left, p - 1);
                }
                if (p + 1 < right) {
                    Quick_Sort(arr, p + 1, right);
                }
            }
        
        }

        
        static void Main(string[] args)
        {
            int[] arr = new int[] { 2, 5, -4, 11, 0, 18, 22, 67, 51, 6 };

            Console.WriteLine("Original array : ");
            foreach (var item in arr)
            {
                Console.Write(" " + item);    
            }
            Console.WriteLine();

            Quick_Sort(arr, 0, arr.Length-1);
            
            Console.WriteLine();
            Console.WriteLine("Sorted array : ");
           
		   foreach (var item in arr)
            {
                Console.Write(" " + item);
            }
            Console.WriteLine();
                    }
    }
}
