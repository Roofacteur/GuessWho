using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessWho
{
    public class PortraitGenerator
    {
        private const int MaxSimilarAttributes = 2;
        private Random random = new Random();

        public Portrait[] GeneratePortraits(int count)
        {
            List<Portrait> portraits = new List<Portrait>();

            while (portraits.Count < count)
            {
                Portrait newPortrait = CreateRandomPortrait(portraits.Count);

                if (portraits.All(existing => !newPortrait.IsSimilarTo(existing, MaxSimilarAttributes)))
                {
                    portraits.Add(newPortrait);
                }
            }

            return portraits.ToArray();
        }

        private Portrait CreateRandomPortrait(int id)
        {
            return new Portrait
            {
                Id = id,
                Name = $"Portrait_{id}",
                Skin = GetRandomAsset("base"),
                Clothes = GetRandomAsset("clothes"),
                Logo = GetRandomAsset("clothes/logos"),
                Eyebrows = GetRandomAsset("eyebrows"),
                Eyes = GetRandomAsset("eyes"),
                Beard = GetRandomAsset("beard"),
                Glasses = GetRandomAsset("glasses"),
                Hair = GetRandomAsset("hair"),
                Mouth = GetRandomAsset("mouth"),
            };
        }

        private string GetRandomAsset(string category)
        {
            string[] files = Directory.GetFiles($"assets/portrait/{category}", "*.png");
            return files[random.Next(files.Length)];
        }
    }

}
