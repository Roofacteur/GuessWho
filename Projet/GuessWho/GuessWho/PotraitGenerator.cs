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
        public int MaxSimilarAttributes = 4;
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

            string logoAsset = clothesFileName.StartsWith("rare_") || clothesFileName.StartsWith("legendary_") ? "logoNone.png" : GetRandomAsset("logos");

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

            // Récupération du fichier
            string directoryPath = Path.Combine("assets", "portrait", category);
            string[] allFiles = Directory.GetFiles(directoryPath, "*.png");

            // Rareté
            if (allFiles.Length == 0)
                throw new FileNotFoundException($"Aucun fichier trouvé dans le répertoire : {directoryPath}");

            double rarityRoll = random.NextDouble();

            string[] FilterFilesByRarity(string rarityTag) =>
                allFiles.Where(f => f.Contains(rarityTag, StringComparison.OrdinalIgnoreCase)).ToArray();

            string[] selectedFiles = rarityRoll switch
            {
                < 0.60 => allFiles.Where(f => !f.Contains("uncommon", StringComparison.OrdinalIgnoreCase) &&
                                              !f.Contains("rare", StringComparison.OrdinalIgnoreCase) &&
                                              !f.Contains("legendary", StringComparison.OrdinalIgnoreCase)).ToArray(),
                < 0.85 => FilterFilesByRarity("uncommon"),
                < 0.95 => FilterFilesByRarity("rare"),
                _ => FilterFilesByRarity("legendary")
            };

            return selectedFiles.Length > 0
                ? selectedFiles[random.Next(selectedFiles.Length)]
                : allFiles[random.Next(allFiles.Length)];
        }


    }


}
