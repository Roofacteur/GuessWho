using System;
using Raylib_cs;

namespace GuessWho
{
    /// <summary>
    /// Représente un joueur dans la partie, avec sa propre grille de portraits et un portrait cible à deviner.
    /// </summary>
    public class Player
    {
        #region Propriétés
        // Identifiant unique du joueur (ex. : 0 pour joueur 1, 1 pour joueur 2)
        public int Id { get; private set; }

        // Grille de jeu contenant les portraits visibles par le joueur
        public Board Board;

        // Zone d'affichage dans la fenêtre (UI)
        public Rectangle Zone;

        // Nom du joueur
        public string Name;

        // Portrait que l’adversaire doit deviner
        public Portrait TargetPortrait;

        // Portrait actuellement sélectionné comme devinette
        public Portrait SelectedGuess { get; set; }

        // Booléen définissant si le joueur est le gagnant
        public bool IsTheWinner;

        // Booléen définissant si le joueur est le perdant
        public bool IsTheLoser;
        #endregion

        #region Constructeur
        /// <summary>
        /// Initialise un joueur avec un ensemble de portraits et un identifiant unique.
        /// </summary>
        public Player(Portrait[] portraits, int id)
        {
            Id = id;
            Board = new Board(portraits);
            Name = "Player " + id;
        }
        #endregion

        #region Méthodes
        public Portrait GetRemainingPortrait()
        {
            List<Portrait> remaining = Board.Portraits.Where(p => !p.IsEliminated).ToList();

            if (remaining.Count == 1)
            {
                SelectedGuess = remaining[0];
                return remaining[0];
            }
            return null;
        }

        /// <summary>
        /// Déduis si le joueur n'a qu'un seul portrait qui n'est pas éliminé sur le plateau
        /// </summary>
        /// <returns>Booléen</returns>
        public bool HasOnlyOnePortraitLeft() => Board.Portraits.Count(p => !p.IsEliminated) == 1;

        /// <summary>
        /// Réinitialise l’état du plateau du joueur (tous les portraits sont réactivés).
        /// </summary>
        public void Reset()
        {
            Board.Reset();
            SelectedGuess = null;
        }

        /// <summary>
        /// Enregistre une tentative de devinette du portrait cible.
        /// </summary>
        /// <param name="guess">Le portrait sélectionné comme devinette.</param>
        /// <returns>True si la devinette est correcte, False sinon.</returns>
        public bool MakeGuess(Portrait guess)
        {
            SelectedGuess = guess;
            return TargetPortrait == guess;
        }
        #endregion
    }
}
