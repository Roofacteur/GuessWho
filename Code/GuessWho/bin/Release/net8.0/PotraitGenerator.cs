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

            // Filtrage des fichiers par rareté présente
            Dictionary<string, List<string>> rarityFiles = new()
            {
                { "common", new List<string>() },
                { "uncommon", new List<string>() },
                { "rare", new List<string>() },
                { "legendary", new List<string>() }
            };

            // Définir les poids de rareté standard (modifiable selon votre logique)
            Dictionary<string, double> rarityWeights = new()
            {
                { "common", 0.5 },
                { "uncommon", 0.3 },
                { "rare", 0.19999 },
                { "legendary", 0.01 }
            };

            foreach (var file in allFiles)
            {
                string f = file.ToLower();
                if (f.Contains("legendary"))
                    rarityFiles["legendary"].Add(file);
                else if (f.Contains("rare"))
                    rarityFiles["rare"].Add(file);
                else if (f.Contains("uncommon"))
                    rarityFiles["uncommon"].Add(file);
                else
                    rarityFiles["common"].Add(file);
            }

            // Supprimer les catégories vides
            var availableRarities = rarityFiles
                .Where(kv => kv.Value.Count > 0)
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            // Normaliser les poids pour les raretés disponibles uniquement
            var totalWeight = availableRarities.Keys.Sum(k => rarityWeights.ContainsKey(k) ? rarityWeights[k] : 0);
            var normalizedWeights = availableRarities.ToDictionary(
                kv => kv.Key,
                kv => rarityWeights[kv.Key] / totalWeight
            );

            // Tirage selon la distribution normalisée
            double roll = random.NextDouble();
            double cumulative = 0;
            string selectedRarity = "common"; // par défaut

            foreach (var kv in normalizedWeights)
            {
                cumulative += kv.Value;
                if (roll < cumulative)
                {
                    selectedRarity = kv.Key;
                    break;
                }
            }

            // Sélection finale
            var pool = availableRarities[selectedRarity];
            return pool[random.Next(pool.Count)];

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
