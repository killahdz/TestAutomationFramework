using System;

namespace Core.Library.Helpers
{
    public class WordGenerator
    {
        public static string GenerateSentence(int length)
        {
            var rnd = new Random(DateTime.Now.Millisecond);
            var str = "";
            while (str.Length < length) str = $"{str} {GenerateWord(rnd, rnd.Next(10))}";
            return str.Substring(0, length);
        }

        private static string GenerateWord(Random rnd, int requestedLength)
        {
            string[] consonants =
            {
                "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "y", "z"
            };
            string[] vowels = {"a", "e", "i", "o", "u"};

            var word = "";

            if (requestedLength == 1)
            {
                word = GetRandomLetter(rnd, vowels);
            }
            else
            {
                for (var i = 0; i < requestedLength; i += 2)
                    word += GetRandomLetter(rnd, consonants) + GetRandomLetter(rnd, vowels);

                word = word.Replace("q", "qu")
                    .Substring(0,
                        requestedLength); // We may generate a string longer than requested length, but it doesn't matter if cut off the excess.
            }

            return word;
        }

        private static string GetRandomLetter(Random rnd, string[] letters)
        {
            return letters[rnd.Next(0, letters.Length - 1)];
        }
    }
}