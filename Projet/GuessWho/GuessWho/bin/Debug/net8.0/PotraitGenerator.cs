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
            int numberOfTries = 0;

            // Étape 1 : Générer les portraits originaux
            while (portraits.Count < count)
            {
                Portrait newPortrait = CreateRandomPortrait(portraits.Count.ToString());

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

            // Étape 2 : Dupliquer les portraits dans le même ordre
            List<Portrait> duplicates = portraits
                .Select(p => p.Clone())
                .ToList();

            portraits.AddRange(duplicates);

            return portraits.ToArray();
        }
        private Portrait CreateRandomPortrait(string id)
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

            // Définir les taux de rareté
            // Plus le chiffre est proche de 1.0 plus il est rare
            const double legendaryThreshold = 0.999;  // 0.1% chance pour "legendary"
            const double rareThreshold = 0.80;       // 20% chance pour "rare"
            const double uncommonThreshold = 0.50;   // 50% chance pour "uncommon"

            double rarityRoll = random.NextDouble();

            string[] selectedFiles;

            if (rarityRoll < uncommonThreshold) // Entre 0.00 et 0.50
            {
                // 50 % de chances — fichiers "communs"
                selectedFiles = FilterFiles(allFiles, file => !file.Contains("uncommon", StringComparison.OrdinalIgnoreCase) &&
                                                            !file.Contains("rare", StringComparison.OrdinalIgnoreCase) &&
                                                            !file.Contains("legendary", StringComparison.OrdinalIgnoreCase));
            }
            else if (rarityRoll < rareThreshold) // Entre 0.50 et 0.80
            {
                // 20 % de chances — fichiers "rare"
                selectedFiles = FilterFiles(allFiles, file => file.Contains("uncommon", StringComparison.OrdinalIgnoreCase));
            }
            else if (rarityRoll < legendaryThreshold) // Entre 0.80 et 0.99
            {
                // 20 % de chances — fichiers "rare"
                selectedFiles = FilterFiles(allFiles, file => file.Contains("rare", StringComparison.OrdinalIgnoreCase));
            }
            else // Entre 0.99 et 1.00
            {
                // 0.1 % de chances — fichiers "legendary"
                selectedFiles = FilterFiles(allFiles, file => file.Contains("legendary", StringComparison.OrdinalIgnoreCase));
            }

            return selectedFiles.Length > 0 ? selectedFiles[random.Next(selectedFiles.Length)] : allFiles[random.Next(allFiles.Length)];
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

        // Méthode générique pour filtrer les fichiers selon un prédicat donné
        private string[] FilterFiles(string[] allFiles, Func<string, bool> predicate)
        {
            List<string> selectedFiles = new List<string>();

            foreach (var file in allFiles)
            {
                if (predicate(file))
                {
                    selectedFiles.Add(file);
                }
            }

            return selectedFiles.ToArray();
        }
    }
}
