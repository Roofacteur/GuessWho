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
            List<string> selectedNames = new List<string>();
            string clothesAsset = GetRandomAsset("clothes");
            string clothesFileName = Path.GetFileName(clothesAsset);

            string logoAsset = clothesFileName.StartsWith("rare_") || clothesFileName.StartsWith("legendary_")
                ? "assets\\portrait\\logos\\logoNone.png"
                : GetRandomAsset("logos");

            string genderAsset = GetRandomAsset("gender");
            string gender = Path.GetFileNameWithoutExtension(genderAsset).ToLower();

            string name = GetRandomName(gender, selectedNames);
            selectedNames.Add(name);

            string beardAsset = gender == "female"
                ? "assets\\portrait\\beard\\beardNone.png"
                : GetRandomAsset("beard");

            return new Portrait
            {
                Id = name + "_" + id,
                Skin = GetRandomAsset("base"),
                Clothes = clothesAsset,
                Logo = logoAsset,
                Eyebrows = GetRandomAsset("eyebrows"),
                Eyes = GetRandomAsset("eyes"),
                Beard = beardAsset,
                Glasses = GetRandomAsset("glasses"),
                Hair = GetRandomAsset("hair\\"+gender+"hair"),
                Mouth = GetRandomAsset("mouth"),
                Gender = genderAsset,
                Name = name
            };
        }


        private string GetRandomAsset(string category)
        {
            string directoryPath = Path.Combine("assets", "portrait", category);
            string[] allFiles = Directory.GetFiles(directoryPath, "*.png");

            if (allFiles.Length == 0)
                throw new FileNotFoundException($"Aucun fichier trouvé dans le répertoire : {directoryPath}");

            double rarityRoll = random.NextDouble();
            string[] selectedFiles;

            if (rarityRoll < 0.5) // 50% commun
            {
                selectedFiles = FilterFiles(allFiles, file =>
                    !file.Contains("uncommon", StringComparison.OrdinalIgnoreCase) &&
                    !file.Contains("rare", StringComparison.OrdinalIgnoreCase) &&
                    !file.Contains("legendary", StringComparison.OrdinalIgnoreCase));
            }
            else if (rarityRoll < 0.8) // 30% uncommon
            {
                selectedFiles = FilterFiles(allFiles, file =>
                    file.Contains("uncommon", StringComparison.OrdinalIgnoreCase));
            }
            else if (rarityRoll < 0.99999) // 19.999% rare
            {
                selectedFiles = FilterFiles(allFiles, file =>
                    file.Contains("rare", StringComparison.OrdinalIgnoreCase));
            }
            else // 0.001% legendary
            {
                selectedFiles = FilterFiles(allFiles, file =>
                    file.Contains("legendary", StringComparison.OrdinalIgnoreCase));
            }

            return selectedFiles.Length > 0
                ? selectedFiles[random.Next(selectedFiles.Length)]
                : allFiles[random.Next(allFiles.Length)];
        }


        public static string GetRandomName(string gender, List<string> alreadySelectedNames)
        {
            string genderFileName;

            if (gender == "female")
            {
                genderFileName = "femalenames.txt";
            }
            else if (gender == "male")
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

            var availableNames = names.Except(alreadySelectedNames).ToList();

            if (availableNames.Count == 0)
            {
                throw new InvalidOperationException("No more unique names available to select.");
            }

            var random = new Random();
            int index = random.Next(availableNames.Count);
            return availableNames[index];
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
