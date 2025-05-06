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
        private Random random = new Random();
        private int numberOfTries;

        public Portrait[] GeneratePortraits(int count, int MaxSimilarAttributes)
        {
            List<Portrait> portraits = new List<Portrait>();

            while (portraits.Count < count)
            {
                Portrait newPortrait = CreateRandomPortrait(portraits.Count);

                if (MaxSimilarAttributes > 0 && MaxSimilarAttributes <= 10)
                {
                    if (portraits.All(existing => !newPortrait.IsSimilarTo(existing, MaxSimilarAttributes)))
                    {
                        portraits.Add(newPortrait);
                    }
                    else
                    {
                        numberOfTries++;
                        Console.WriteLine("Retried generation " + numberOfTries.ToString() + " times");
                    }
                }
                
            }
            
            return portraits.ToArray();
        }
        private Portrait CreateRandomPortrait(int id)
        {
            string clothesAsset = GetRandomAsset("clothes");
            string clothesFileName = Path.GetFileName(clothesAsset);
            string logoAsset = clothesFileName.StartsWith("rare_") || clothesFileName.StartsWith("legendary_")
                ? "assets\\portrait\\logos\\logoNone.png" : GetRandomAsset("logos");


            string genderAsset = GetRandomAsset("gender");
            string gender = Path.GetFileName(genderAsset); 

            return new Portrait
            {
                Id = id,
                Skin = GetRandomAsset("base"),
                Clothes = clothesAsset,
                Logo = logoAsset,
                Eyebrows = GetRandomAsset("eyebrows"),
                Eyes = GetRandomAsset("eyes"),
                Beard = GetRandomAsset("beard"),
                Glasses = GetRandomAsset("glasses"),
                Hair = GetRandomAsset("hair"),
                Mouth = GetRandomAsset("mouth"),
                Gender = genderAsset,
                Name = GetRandomName(gender)
            };
        }

        private string GetRandomAsset(string category)
        {

            string directoryPath = Path.Combine("assets", "portrait", category);
            string[] allFiles = Directory.GetFiles(directoryPath, "*.png");

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

        private static string GetRandomName(string gender)
        {
            string genderFileName;

            if (gender == "female.png")
            {
                genderFileName = "femalenames.txt";
            }
            else if (gender == "male.png")
            {
                genderFileName = "malenames.txt";
            }
            else
            {
                throw new ArgumentException("Invalid gender value. Expected 'female.png' or 'male.png'");
            }

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "assets", genderFileName);
            List<string> names = LoadNames(filePath);

            if (names.Count < 48)
            {
                throw new Exception($"Critical error: '{genderFileName}' must contain at least 48 unique names!");
            }

            // Return a random name
            return names.OrderBy(str => Guid.NewGuid()).First();
        }

        public static List<string> LoadNames(string filePath)
        {
            try
            {
                string fileContent = File.ReadAllText(filePath);
                return fileContent
                    .Split(',')
                    .Select(name => name.Trim())
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .Distinct()
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading file: " + ex.Message);
                return new List<string>();
            }
        }


    }
}
