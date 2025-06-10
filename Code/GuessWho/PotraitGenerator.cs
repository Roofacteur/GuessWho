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
        private readonly HashSet<string> selectedNames = new();
        private const int MaxAttempts = 1000;
        #endregion

        #region Recuperation textures aléatoires
        /// <summary>
        /// Génère une liste de portraits uniques, dupliqués pour deux joueurs.
        /// Limite le nombre d’essais pour éviter boucle infinie.
        /// </summary>
        /// <param name="count">Nombre de portraits de base (avant duplication).</param>
        /// <param name="maxSimilarAttributes">Nombre max d’attributs identiques autorisés entre deux portraits.</param>
        public Portrait[] GeneratePortraits(int count, int maxSimilarAttributes)
        {
            if (maxSimilarAttributes < 1 || maxSimilarAttributes > 10)
                throw new ArgumentOutOfRangeException(nameof(maxSimilarAttributes), "Doit être entre 1 et 10.");

            List<Portrait> portraits = new();
            int attempts = 0;

            while (portraits.Count < count)
            {
                if (attempts++ > MaxAttempts)
                    throw new InvalidOperationException("Nombre maximum d’essais atteint, impossible de générer plus de portraits uniques.");

                Portrait newPortrait = CreateRandomPortrait(portraits.Count.ToString());

                bool isUnique = portraits.All(existing => !newPortrait.IsSimilarTo(existing, maxSimilarAttributes));

                if (isUnique)
                {
                    portraits.Add(newPortrait);
                }
                else
                {
                    Console.WriteLine($"Portrait rejeté - similarité trop élevée (tentative #{attempts})");
                }
            }

            // Dupliquer les portraits pour les deux joueurs (assure deep copy)
            List<Portrait> duplicates = portraits.Select(p => p.Clone()).ToList();
            portraits.AddRange(duplicates);

            return portraits.ToArray();
        }

        /// <summary>
        /// Crée un portrait aléatoire avec un identifiant et un nom uniques.
        /// </summary>
        private Portrait CreateRandomPortrait(string id)
        {
            string clothesAsset = GetRandomAsset("clothes");
            string clothesFileName = Path.GetFileName(clothesAsset);

            // Pas de logo si vêtement rare ou légendaire
            string logoAsset = (clothesFileName.StartsWith("rare_") || clothesFileName.StartsWith("legendary_"))
                ? Path.Combine("assets", "portrait", "logos", "logoNone.png")
                : GetRandomAsset("logos");

            string genderAsset = GetRandomAsset("gender");
            string gender = Path.GetFileNameWithoutExtension(genderAsset).ToLower();

            string name = GetRandomName(gender);
            selectedNames.Add(name);

            string beardAsset = gender == "female"
                ? Path.Combine("assets", "portrait", "beard", "noBeard.png")
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
                Hair = GetRandomAsset(Path.Combine("hair", $"{gender}hair")),
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

            var filesByRarity = new Dictionary<string, List<string>>
            {
                { "common", new List<string>() },
                { "uncommon", new List<string>() },
                { "rare", new List<string>() },
                { "legendary", new List<string>() }
            };

            foreach (string file in files)
            {
                string lower = file.ToLowerInvariant();
                if (lower.Contains("legendary")) filesByRarity["legendary"].Add(file);
                else if (lower.Contains("rare")) filesByRarity["rare"].Add(file);
                else if (lower.Contains("uncommon")) filesByRarity["uncommon"].Add(file);
                else filesByRarity["common"].Add(file);
            }

            // Pondérations par rareté
            var rarityWeights = new Dictionary<string, double>
            {
                { "common", 0.5 },
                { "uncommon", 0.3 },
                { "rare", 0.19 },
                { "legendary", 0.01 }
            };

            // Filtrer les catégories disponibles
            var available = filesByRarity.Where(kv => kv.Value.Count > 0).ToDictionary(kv => kv.Key, kv => kv.Value);

            double totalWeight = available.Keys.Sum(k => rarityWeights[k]);
            var normalizedWeights = available.ToDictionary(
                kv => kv.Key,
                kv => rarityWeights[kv.Key] / totalWeight);

            // Tirage pondéré
            double roll = random.NextDouble();
            double cumulative = 0.0;
            string selectedRarity = "common";

            foreach (var kv in normalizedWeights)
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

        #region Récupération noms aléatoires
        /// <summary>
        /// Sélectionne un nom aléatoire unique depuis une liste spécifique au genre.
        /// </summary>
        public string GetRandomName(string gender)
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
                throw new Exception($"Le fichier '{filename}' doit contenir au moins 48 noms uniques.");

            var available = names.Except(selectedNames).ToList();

            if (available.Count == 0)
                throw new InvalidOperationException("Tous les noms uniques ont été utilisés.");

            return available[random.Next(available.Count)];
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
#endregion