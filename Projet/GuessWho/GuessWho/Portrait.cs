using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GuessWho
{
    public class Portrait
    {
        public int Id;
        required public string Name;
        required public string Skin;
        required public string Clothes;
        required public string Logo;
        required public string Eyebrows;
        required public string Eyes;
        required public string Beard;
        required public string Glasses;
        required public string Hair;
        required public string Mouth;

        public bool IsEliminated = false;

        public string[] GetDNA()
        {
            return new[] { Skin, Clothes, Logo, Eyebrows, Eyes, Beard, Glasses, Hair, Mouth };
        }

        public bool IsSimilarTo(Portrait other, int maxSimilarAttributes)
        {
            int similarCount = GetDNA().Zip(other.GetDNA(), (a, b) => a == b).Count(match => match);
            return similarCount > maxSimilarAttributes;
        }
    }

}
