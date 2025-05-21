using System;
using System.Linq;

namespace GuessWho
{
    /// <summary>
    /// Représente un portrait généré pour le jeu avec tous ses attributs distinctifs.
    /// </summary>
    public class Portrait
    {
        // Identifiant unique du portrait
        public string Id;

        // Attributs principaux (visuels et logiques)
        public required string Name;
        public required string Skin;
        public required string Clothes;
        public required string Logo;
        public required string Eyebrows;
        public required string Eyes;
        public required string Beard;
        public required string Glasses;
        public required string Hair;
        public required string Mouth;
        public required string Gender;

        // États liés à l'interface utilisateur et à la logique de jeu
        public float HoverOffset = 0f;       // Décalage visuel au survol
        public bool IsEliminated = false;    // Portrait éliminé de la grille
        public bool isTarget = false;        // Portrait à deviner
        public bool CanAppear = true;        // Portrait visible dans la génération

        /// <summary>
        /// Retourne le profil ADN sous forme de tableau d’attributs clés.
        /// </summary>
        public string[] GetDNA() => new[]
        {
            Skin, Clothes, Logo, Eyebrows, Eyes,
            Beard, Glasses, Hair, Mouth, Gender
        };

        /// <summary>
        /// Compare l’ADN avec un autre portrait pour évaluer leur similarité.
        /// </summary>
        /// <param name="other">Autre portrait à comparer.</param>
        /// <param name="maxSimilarAttributes">Seuil maximal de similarités tolérées.</param>
        /// <returns>True si le nombre de similarités dépasse le seuil.</returns>
        public bool IsSimilarTo(Portrait other, int maxSimilarAttributes)
        {
            int similarCount = GetDNA()
                .Zip(other.GetDNA(), (a, b) => a == b)
                .Count(match => match);

            return similarCount > maxSimilarAttributes;
        }

        /// <summary>
        /// Clone ce portrait avec un nouvel identifiant unique.
        /// </summary>
        public Portrait Clone()
        {
            return new Portrait
            {
                Id = Guid.NewGuid().ToString(),
                Name = this.Name,
                Skin = this.Skin,
                Clothes = this.Clothes,
                Logo = this.Logo,
                Eyebrows = this.Eyebrows,
                Eyes = this.Eyes,
                Beard = this.Beard,
                Glasses = this.Glasses,
                Hair = this.Hair,
                Mouth = this.Mouth,
                Gender = this.Gender,

                // Copie des états
                HoverOffset = this.HoverOffset,
                IsEliminated = this.IsEliminated,
                isTarget = this.isTarget,
                CanAppear = this.CanAppear
            };
        }
    }
}
