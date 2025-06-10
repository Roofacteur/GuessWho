using System;

namespace GuessWho
{
    /// <summary>
    /// Représente la grille de portraits d’un joueur, avec logique d’élimination par attribut.
    /// </summary>
    public class Board
    {
        #region Propriétés
        public Portrait[] Portraits;
        #endregion

        #region Constructeur
        public Board(Portrait[] portraits)
        {
            Portraits = portraits ?? throw new ArgumentNullException(nameof(portraits), "Le tableau de portraits ne peut pas être nul.");
        }
        #endregion

        #region Méthodes
        /// <summary>
        /// Élimine les portraits qui ne correspondent pas à l'attribut et à la valeur spécifiés.
        /// </summary>
        /// <param name="attribute">Le nom de l’attribut à tester (ex : "Hair").</param>
        /// <param name="value">La valeur à comparer (ex : "blonde.png").</param>
        public void EliminatePortraitsByQuestion(string attribute, string value)
        {
            if (string.IsNullOrWhiteSpace(attribute))
                throw new ArgumentException("L'attribut ne peut pas être vide.", nameof(attribute));

            foreach (var portrait in Portraits)
            {
                bool match = attribute switch
                {
                    "Skin" => portrait.Skin == value,
                    "Clothes" => portrait.Clothes == value,
                    "Logo" => portrait.Logo == value,
                    "Eyebrows" => portrait.Eyebrows == value,
                    "Eyes" => portrait.Eyes == value,
                    "Beard" => portrait.Beard == value,
                    "Glasses" => portrait.Glasses == value,
                    "Hair" => portrait.Hair == value,
                    "Mouth" => portrait.Mouth == value,
                    "Gender" => portrait.Gender == value,
                    _ => throw new ArgumentException($"Attribut inconnu : {attribute}")
                };

                if (!match)
                    portrait.IsEliminated = true;
            }
        }

        /// <summary>
        /// Réinitialise l’état d’élimination de tous les portraits.
        /// </summary>
        public void Reset()
        {
            foreach (var portrait in Portraits)
            {
                portrait.IsEliminated = false;
            }
        }
        #endregion
    }
}
