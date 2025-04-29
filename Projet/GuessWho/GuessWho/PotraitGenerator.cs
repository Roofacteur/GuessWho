using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GuessWho
{
    public class PortraitGenerator
    {
        private const int MaxSimilarAttributes = 3;
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
            string clothesAsset = GetRandomAsset("clothes");

            string clothesFileName = Path.GetFileName(clothesAsset);

            string logoAsset = clothesFileName.StartsWith("Special_") ? "logoNone.png" : GetRandomAsset("logos");

            return new Portrait
            {
                Id = id,
                Name = $"Portrait_{id}",
                Skin = GetRandomAsset("base"),
                Clothes = clothesAsset,
                Logo = logoAsset,
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
            // Utilisation de Path.Combine pour générer le chemin de manière sûre
            string directoryPath = Path.Combine("assets", "portrait", category);
            string[] files = Directory.GetFiles(directoryPath, "*.png");
            return files[random.Next(files.Length)];
        }
        
    }


}
