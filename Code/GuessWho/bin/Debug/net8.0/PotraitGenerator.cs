using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GuessWho
{
    /// <summary>
    /// Génère dynamiquement des portraits uniques avec des attributs aléatoires et pondérés.
    /// </summary>
    public class PortraitGenerator
    {
        #region Propriétés
        private readonly Random random = new();
        #endregion

        #region Recuperation textures aléatoires
        /// <summary>
        /// Génère une liste de portraits uniques, dupliqués pour deux joueurs.
        /// </summary>
        /// <param name="count">Nombre de portraits de base (avant duplication).</param>
        /// <param name="maxSimilarAttributes">Nombre max d’attributs identiques autorisés entre deux portraits.</param>
        public Portrait[] GeneratePortraits(int count, int maxSimilarAttributes)
        {
            List<Portrait> portraits = new();
            int attempts = 0;

            while (portraits.Count < count)
            {
                Portrait newPortrait = CreateRandomPortrait(portraits.Count.ToString());

                if (maxSimilarAttributes > 1 && maxSimilarAttributes <= 10)
                {
                    bool isUnique = portraits.All(existing =>
                        !newPortrait.IsSimilarTo(existing, maxSimilarAttributes));

                    if (isUnique)
                        portraits.Add(newPortrait);
                    else
                        Console.WriteLine($"Portrait rejeté - similarité trop élevée (tentative #{++attempts})");
                }
            }

            // Dupliquer les portraits pour les deux joueurs
            List<Portrait> duplicates = portraits.Select(p => p.Clone()).ToList();
            portraits.AddRange(duplicates);

            return portraits.ToArray();
        }

        /// <summary>
        /// Crée un portrait aléatoire avec un identifiant et un nom uniques.
        /// </summary>
        private Portrait CreateRandomPortrait(string id)
        {
            List<string> selectedNames = new();

            string clothesAsset = GetRandomAsset("clothes");
            string clothesFileName = Path.GetFileName(clothesAsset);

            // Pas de logo si vêtement rare ou légendaire
            string logoAsset = (clothesFileName.StartsWith("rare_") || clothesFileName.StartsWith("legendary_"))
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
                Id = $"{name}_{id}",
                Skin = GetRandomAsset("base"),
                Clothes = clothesAsset,
                Logo = logoAsset,
                Eyebrows = GetRandomAsset("eyebrows"),
                Eyes = GetRandomAsset("eyes"),
                Beard = beardAsset,
                Glasses = GetRandomAsset("glasses"),
                Hair = GetRandomAsset($"hair\\{gender}hair"),
                Mouth = GetRandomAsset("mouth"),
                Gender = genderAsset,
                Name = name
            };
        }

        #region Système de rareté
        /// <summary>
        /// Sélectionne un asset d’un dossier selon une distribution de rareté pondérée.
        /// </summary>
        private string GetRandomAsset(string category)
        {
            string path = Path.Combine("assets", "portrait", category);
            string[] files = Directory.GetFiles(path, "*.png");

            if (files.Length == 0)
                throw new FileNotFoundException($"Aucun asset trouvé dans : {path}");

            // Classement par rareté
            var filesByRarity = new Dictionary<string, List<string>>
            {
                { "common", new() },
                { "uncommon", new() },
                { "rare", new() },
                { "legendary", new() }
            };

            foreach (string file in files)
            {
                string f = file.ToLower();
                if (f.Contains("legendary")) filesByRarity["legendary"].Add(file);
                else if (f.Contains("rare")) filesByRarity["rare"].Add(file);
                else if (f.Contains("uncommon")) filesByRarity["uncommon"].Add(file);
                else filesByRarity["common"].Add(file);
            }

            // Pondérations par rareté
            var rarityWeights = new Dictionary<string, double>
            {
                { "common", 0.5 },
                { "uncommon", 0.3 },
                { "rare", 0.19999 },
                { "legendary", 0.01 }
            };

            var available = filesByRarity
                .Where(kv => kv.Value.Any())
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            double totalWeight = available.Keys.Sum(k => rarityWeights[k]);
            var normalized = available.ToDictionary(
                kv => kv.Key,
                kv => rarityWeights[kv.Key] / totalWeight
            );

            // Sélection pondérée
            double roll = random.NextDouble();
            double cumulative = 0;
            string selectedRarity = "common";

            foreach (var kv in normalized)
            {
                cumulative += kv.Value;
                if (roll < cumulative)
                {
                    selectedRarity = kv.Key;
                    break;
                }
            }

            List<string> pool = available[selectedRarity];
            return pool[random.Next(pool.Count)];
        }
        #endregion
        #endregion

        #region Récupération noms aléatoires
        /// <summary>
        /// Sélectionne un nom aléatoire unique depuis une liste spécifique au genre.
        /// </summary>
        public static string GetRandomName(string gender, List<string> alreadySelected)
        {
            string filename = gender switch
            {
                "female" => "femalenames.txt",
                "male" => "malenames.txt",
                _ => throw new ArgumentException("Genre invalide : 'male' ou 'female' attendu.")
            };

            string path = Path.Combine(Directory.GetCurrentDirectory(), "assets", filename);
            var names = LoadNames(path);

            if (names.Count < 48)
                throw new Exception($"'{filename}' doit contenir au moins 48 noms uniques.");

            var available = names.Except(alreadySelected).ToList();

            if (!available.Any())
                throw new InvalidOperationException("Tous les noms uniques ont été utilisés.");

            return available[new Random().Next(available.Count)];
        }

        /// <summary>
        /// Charge les noms depuis un fichier texte (séparés par virgules).
        /// </summary>
        public static List<string> LoadNames(string path)
        {
            try
            {
                string content = File.ReadAllText(path);
                return content
                    .Split(',')
                    .Select(n => n.Trim())
                    .Where(n => !string.IsNullOrWhiteSpace(n))
                    .Distinct()
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur de lecture ({path}) : {ex.Message}");
                return new List<string>();
            }
        }
        #endregion
    }
}
