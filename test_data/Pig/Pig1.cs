using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
public class Program 
{
public static string TranslateWord(string word)
        {
	 if(string.IsNullOrWhiteSpace(word))
            {
                return string.Empty;
            }
            char[] vowel = new char[] { 'a', 'e', 'i', 'o', 'u' };

            var isUpperCase = Regex.IsMatch(word, "[A-Z].*");
            var matchPunctuation = Regex.Match(word, "\\W+");
            var startPunctuation = matchPunctuation.Success && matchPunctuation.Index == 0;
            var endPunctuation = matchPunctuation.Success && matchPunctuation.Index != 0;

            var wordWithoutPunctuation = word.Remove(matchPunctuation.Index, matchPunctuation.Length);

            if (!vowel.Any(v => v == word.ToLower()[0]))
            {
                var indexOfVowel = wordWithoutPunctuation.IndexOfAny(vowel);
                if(indexOfVowel > -1)
                {
                    wordWithoutPunctuation =$"{wordWithoutPunctuation.Substring(indexOfVowel)}{wordWithoutPunctuation.Substring(0, indexOfVowel)}ay";
                }
                else
                {
                    wordWithoutPunctuation =$"{wordWithoutPunctuation}ay";
                }
            }
            else
            {
                wordWithoutPunctuation = $"{wordWithoutPunctuation}yay";
            }

            if(isUpperCase)
            {
                wordWithoutPunctuation = $"{wordWithoutPunctuation.Substring(0, 1).ToUpper()}{wordWithoutPunctuation.Substring(1).ToLower()}";
            }

            var result = wordWithoutPunctuation;
            var punctuation = word.Substring(matchPunctuation.Index, matchPunctuation.Length);

            if (startPunctuation)
            {
                result = $"{punctuation}{wordWithoutPunctuation}";
            }
            else if(endPunctuation)
            {
                result = $"{wordWithoutPunctuation}{punctuation}";
            }

            return result;
        }

       public static string TranslateSentence(string sentence)
        {
            return string.IsNullOrWhiteSpace(sentence) ? string.Empty : string.Join(" ", sentence.Split(' ').Select(s => TranslateWord(s)));
        }
}