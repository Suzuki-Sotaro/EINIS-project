using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
public class Program 
{
        public static string TranslateWord(string word)
        {
					if (word == null || word == "")
            {
                return "";
            }
            bool sign = false;
            bool isUpper = false;
            bool firstSign = false;
            bool atLast = false;
            if (!char.IsLetter(word.Last()))
            {
                sign = true;
                if (!char.IsLetter(word[word.Length - 2]))
                {
                    atLast = true;
                }
            }
            if (!char.IsLetter(word.First()))
            {
                firstSign = true;
            }
            if (Char.IsUpper(word[0]) || (Char.IsUpper(word[1]) && firstSign == true))
            {
                isUpper = true;
            }
            char[] vowels = { 'a', 'e', 'i', 'o', 'u' };
            char x =  Char.ToLower(word[0]);
            foreach (char c in vowels)
            {
                if (c == x)
                { 
                    if (sign)
                    {
                        if (atLast)
                        {
                            if (isUpper)
                            {
                                string result = word.Substring(0, word.Length - 2) + "yay" + word[word.Length - 2] + word.Last();
                                return Char.ToUpper(result[0]) + result.Substring(1, result.Length - 1).ToLower();
                            }
                            else
                            {
                                return word.Substring(0, word.Length - 2) + "yay" + word[word.Length - 2] + word.Last();
                            }
                        }
                        else
                        {
                            if (isUpper)
                            {
                                string result = word.Substring(0, word.Length - 1) + "yay" + word.Last();
                                return Char.ToUpper(result[0]) + result.Substring(1, result.Length - 1).ToLower();
                            }
                            else
                            {
                                return word.Substring(0, word.Length - 1) + "yay" + word.Last();
                            }
                        }
                    }
                    else if (firstSign)
                    {
                        if (isUpper)
                        {
                            string result = word.First() + word.Substring(1, word.Length - 1) + "yay";
                            return result[0] + Char.ToUpper(result[1]) + result.Substring(2, result.Length - 2).ToLower();
                        }
                        else
                        {
                            return word.First() + word.Substring(1, word.Length - 1) + "yay";
                        }
                    }
                    else
                    {
                        if (isUpper)
                        {
                            string result = word + "yay";
                            return Char.ToUpper(result[0]) + result.Substring(1, result.Length - 1).ToLower();
                        }
                        else
                        {
                            return word + "yay";
                        }
                    }
                }
            }

            int i = 0;
            foreach (char z in word)
            {
                foreach (char c in vowels)
                {
                    if (c == z)
                    {
                        if (sign)
                        {
                            if (isUpper)
                            {
                                string result = word.Substring(i, word.Length - i - 1) + word.Substring(0, i) + "ay" + word.Last();
                                return Char.ToUpper(result[0]) + result.Substring(1, result.Length - 1).ToLower();
                            }
                            else
                            {
                                return word.Substring(i, word.Length - i - 1) + word.Substring(0, i) + "ay" + word.Last();
                            }
                        }
                        else if (firstSign)
                        {
                            if (isUpper)
                            {
                                string result = word.First() + word.Substring(i, word.Length - i) + word.Substring(1, i - 1)  + "ay";
                                string zxc = result[0] + "" + Char.ToUpper(result[1]) + result.Substring(2, result.Length - 2).ToLower();
                                return zxc;
                            }
                            else
                            {
                                return word.First() + word.Substring(i, word.Length - i) + word.Substring(1, i - 1) + "ay";
                            }
                        }
                        else
                        {
                            if (isUpper)
                            {
                                string result = word.Substring(i, word.Length - i) + word.Substring(0, i) + "ay";
                                return Char.ToUpper(result[0]) + result.Substring(1, result.Length - 1).ToLower();
                            }
                            else
                            {
                                return word.Substring(i, word.Length - i) + word.Substring(0, i) + "ay";
                            }
                        }
                    }
                }
                i++;
            }
            return word;
        }
        public static string TranslateSentence(string sentence)
        {
					if (sentence == null)
            {
                return "";
            }
            string[] words = sentence.Split(' ');
            string result = "";
            foreach (string s in words)
            {
                result += TranslateWord(s) + " ";
            }
            return result.Substring(0, result.Length - 1);
        }
}