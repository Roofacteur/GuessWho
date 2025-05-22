using Raylib_cs;
using static Raylib_cs.Raylib;

namespace GuessWho
{
    /// <summary>
    /// Contrôleur central du jeu « Guess Who ».
    /// Gère les états du jeu, les interactions entre joueurs, la logique de sélection des portraits
    /// ainsi que les transitions entre les différentes phases (menu, jeu, règles, création...).
    /// Sert de point d'entrée pour coordonner l'ensemble des composants du gameplay.
    /// </summary>
    public class GameManager
    {
        #region Configuration et états globaux

        public bool isMusicMuted = false;
        public bool isSfxMuted = false;
        public bool gameStarted = false;
        public bool portraitsGenerated = false;
        public bool generatedExample = false;
        public bool userHasDualScreen = true;

        public GameState CurrentState = GameState.None;
        public bool StateSelectingPortrait = true;

        public enum GameState
        {
            None,
            Menu,
            Rules,
            Generation,
            Settings,
            Creating,
            InGame,
            Guessing,
            Victory
        }

        #endregion

        #region Données de jeu

        public Portrait[] allPortraits;
        public Player player1;
        public Player player2;

        public int currentPlayerTurn = 1;
        public int userMaxAttributesInput = 4;

        #endregion

        #region Gestionnaires

        public PortraitCreator creator;
        public PortraitGenerator generator;
        public PortraitRenderer renderer = new();
        public UIManager uIManager;
        public SoundManager soundManager;

        #endregion

        #region Gestion des tours

        public int GetCurrentPlayer() => currentPlayerTurn;
        public void NextTurn() => currentPlayerTurn = (currentPlayerTurn == 1) ? 2 : 1;
        public void ResetTurn() => currentPlayerTurn = 1;

        #endregion

        #region Initialisation

        /// <summary>
        /// Initialise les gestionnaires principaux et place l’état initial sur le menu.
        /// </summary>
        public void Initialize()
        {
            uIManager = new UIManager();
            soundManager = new SoundManager();
            ResetTurn();
            CurrentState = GameState.Menu;
        }

        /// <summary>
        /// Initialise l’environnement pour le créateur de portraits.
        /// </summary>
        public void InitializeCreator()
        {
            uIManager = new UIManager();
            creator = new PortraitCreator();
        }

        #endregion

        #region Boucle de jeu principale

        /// <summary>
        /// Met à jour le comportement du jeu selon l’état courant.
        /// </summary>
        public void Update(GameManager gameManager)
        {
            switch (CurrentState)
            {
                case GameState.Menu:
                    StateSelectingPortrait = true;
                    gameStarted = false;
                    portraitsGenerated = false;
                    LoadUIAndSounds(gameManager);
                    uIManager.UpdateMenu(gameManager);
                    break;

                case GameState.InGame:
                    LoadUIAndSounds(gameManager);
                    if (StateSelectingPortrait)
                    {
                        Generate();
                        if (player1.TargetPortrait == null || player2.TargetPortrait == null)
                            uIManager.DrawSelectingPortraits(gameManager);
                        else
                            StateSelectingPortrait = false;
                    }
                    else
                    {
                        if (!gameStarted)
                        {
                            ResetTurn();
                            gameStarted = true;
                        }
                        CheckVictory(player1, player2, this);
                        if (CurrentState == GameState.Victory)
                            return;
                        soundManager.SoundsLoader(this);
                        uIManager.DrawGame(gameManager);
                    }
                    
                    break;

                case GameState.Settings:
                    LoadUIAndSounds(gameManager);
                    uIManager.DrawOptions(gameManager);
                    break;

                case GameState.Rules:
                    LoadUIAndSounds(gameManager);
                    uIManager.DrawRules(gameManager);
                    break;

                case GameState.Creating:
                    generatedExample = false;
                    LoadUIAndSounds(gameManager);
                    uIManager.DrawCreator(gameManager);
                    break;

                case GameState.Generation:
                    LoadUIAndSounds(gameManager);
                    if (!generatedExample)
                    {
                        GenerateExample();
                        generatedExample = true;
                    }
                    uIManager.DrawGeneration(gameManager);
                    break;

                case GameState.Guessing:
                    // État de devinette à implémenter
                    break;

                case GameState.Victory:
                    LoadUIAndSounds(gameManager);
                    uIManager.DrawWinScreen(gameManager, renderer);
                    break;
            }
        }

        #endregion

        #region Chargement des ressources

        /// <summary>
        /// Charge les textures et les sons en fonction des préférences utilisateur.
        /// </summary>
        private void LoadUIAndSounds(GameManager gameManager)
        {
            uIManager.TextureLoader(gameManager);

            if (!isMusicMuted)
                soundManager.SoundsLoader(this);
            else
                soundManager.StopMusic();

            if (!isSfxMuted)
                soundManager.SoundsLoader(this);
            else
                soundManager.StopSFX();
        }

        #endregion

        #region Génération de portraits

        /// <summary>
        /// Génère les portraits de jeu pour les deux joueurs.
        /// </summary>
        public void Generate()
        {
            if (!portraitsGenerated)
            {
                renderer.UnloadAll();
                renderer = new PortraitRenderer();
                generator = new PortraitGenerator();
                allPortraits = generator.GeneratePortraits(24, userMaxAttributesInput);

                player1 = new Player(allPortraits.Take(24).ToArray(), 1);
                player2 = new Player(allPortraits.Skip(24).Take(24).ToArray(), 2);

                foreach (Portrait portrait in allPortraits)
                    renderer.LoadPortraitTextures(portrait);

                portraitsGenerated = true;
            }
        }

        /// <summary>
        /// Génère deux portraits à titre d'exemple dans l’éditeur.
        /// </summary>
        public void GenerateExample()
        {
            renderer = new PortraitRenderer();
            generator = new PortraitGenerator();
            allPortraits = generator.GeneratePortraits(2, userMaxAttributesInput);

            player1 = new Player(allPortraits.Take(2).ToArray(), 1);

            foreach (Portrait portrait in allPortraits)
                renderer.LoadPortraitTextures(portrait);
        }

        #endregion

        #region Logique de jeu

        /// <summary>
        /// Vérifie si un joueur a deviné correctement le portrait de l’adversaire.
        /// </summary>
        public void CheckVictory(Player player1, Player player2, GameManager gameManager)
        {
            // Récupère les portraits restants pour chaque joueur
            Portrait remainingForPlayer1 = player2.GetRemainingPortrait();
            Portrait remainingForPlayer2 = player1.GetRemainingPortrait();

            // Vérifie si les portraits restants correspondent aux cibles par nom (ou par Id si disponible)
            bool player1Wins = remainingForPlayer1 != null &&
                               player1.TargetPortrait != null &&
                               remainingForPlayer1.Name == player1.TargetPortrait.Name;

            bool player2Wins = remainingForPlayer2 != null &&
                               player2.TargetPortrait != null &&
                               remainingForPlayer2.Name == player2.TargetPortrait.Name;

            // Affichage de debug
            Console.WriteLine("P1 Guess: " + (remainingForPlayer1 != null ? remainingForPlayer1.Name : "null") +
                              " | P2 Target: " + (player1.TargetPortrait != null ? player1.TargetPortrait.Name : "null"));
            Console.WriteLine("P2 Guess: " + (remainingForPlayer2 != null ? remainingForPlayer2.Name : "null") +
                              " | P1 Target: " + (player2.TargetPortrait != null ? player2.TargetPortrait.Name : "null"));
            Console.WriteLine("Comparaison P1: " + player1Wins + " | Comparaison P2: " + player2Wins);

            // Résolution de victoire
            if (player1Wins && !player2Wins)
            {
                player1.IsTheWinner = true;
                player2.IsTheLoser = true;
                gameManager.CurrentState = GameState.Victory;
            }
            else if (player2Wins && !player1Wins)
            {
                player2.IsTheWinner = true;
                player1.IsTheLoser = true;
                gameManager.CurrentState = GameState.Victory;
            }
        }

        /// <summary>
        /// Assigne un portrait cible à un joueur.
        /// </summary>
        public void SelectedPortrait(Player player, Portrait portrait)
        {
            player.TargetPortrait = portrait;
            portrait.isTarget = true;
        }

        /// <summary>
        /// Réinitialise la partie avec les deux joueurs.
        /// </summary>
        public void Reset(Player p1, Player p2)
        {
            p1.Reset();
            p2.Reset();
            ResetTurn();
            StateSelectingPortrait = true;
            CurrentState = GameState.InGame;
        }

        /// <summary>
        /// Termine la partie et affiche l’écran de victoire.
        /// </summary>
        public void EndGame(bool player1Won)
        {
            CurrentState = GameState.Victory;
        }

        #endregion
    }
}
