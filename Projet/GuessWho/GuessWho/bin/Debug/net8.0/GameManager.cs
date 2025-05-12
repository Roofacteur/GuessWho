using Raylib_cs;
using System.Drawing;
using static Raylib_cs.Raylib;

namespace GuessWho
{
    public class GameManager
    {
        private Music backgroundMusic;

        bool gameStarted = false;
        public bool portraitsGenerated = false;
        public bool generatedExample = false;
        public bool userHasDualScreen = true;

        public enum GameState
        {
            None,
            Menu,
            Generation,
            Options,
            Creating,
            InGame,
            Guessing,
            Victory
        }
        public GameState CurrentState;
        public bool StateSelectingPortrait = true;
        public Portrait[] allPortraits;
        public Player player1;
        public Player player2;
        public PortraitCreator creator;
        public PortraitGenerator generator;
        public PortraitRenderer renderer = new PortraitRenderer();
        public UIManager uIManager;
        public SoundManager soundsManager;
        public int currentPlayerTurn = 1;
        public int userMaxAttributesInput = 4;
        public int GetCurrentPlayer() => currentPlayerTurn;
        public void NextTurn() => currentPlayerTurn = (currentPlayerTurn == 1) ? 2 : 1;
        public void ResetTurn() => currentPlayerTurn = 1;

        public void Update(GameManager gamemanager)
        {
            switch (CurrentState)
            {
                case GameState.Menu:

                    // Remettre les états à zéro
                    StateSelectingPortrait = true;
                    gameStarted = false;
                    portraitsGenerated = false;
                    generatedExample = false;
                    // Interface
                    uIManager.TextureLoader(gamemanager);
                    uIManager.UpdateMenu(gamemanager);
                    uIManager.DrawMenu();
                    // 🔊 Musique
                    soundsManager.SoundsLoader(this);
                    break;

                case GameState.InGame:
                    
                    // Interface
                    uIManager.TextureLoader(gamemanager);
                    // 🔊 Musique
                    soundsManager.SoundsLoader(this);

                    if (StateSelectingPortrait)
                    {
                        Generate();
                        if (player1.TargetPortrait == null || player2.TargetPortrait == null)
                            uIManager.DrawSelectingPortraits(gamemanager);
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
                        uIManager.DrawGame(gamemanager);
                    }

                    break;

                case GameState.Options:

                    // Interface
                    uIManager.TextureLoader(gamemanager);
                    uIManager.DrawOptions(gamemanager);
                    // 🔊 Musique
                    soundsManager.SoundsLoader(this);
                    break;

                case GameState.Generation:

                    // Interface
                    uIManager.TextureLoader(gamemanager);
                    // 🔊 Musique
                    soundsManager.SoundsLoader(this);
                    if (!generatedExample)
                    {
                        GenerateExample();
                        generatedExample = true;
                    }
                    uIManager.DrawGeneration(gamemanager);

                    break;

                case GameState.Creating:

                    InitializeCreator();

                    // Interface
                    uIManager.TextureLoader(gamemanager);
                    uIManager.DrawCreator(gamemanager);
                    // 🔊 Musique
                    soundsManager.SoundsLoader(this);

                    break;

                case GameState.Guessing:

                    break;

                case GameState.Victory:

                    uIManager.DrawEndScreen(CurrentState, currentPlayerTurn);

                    break;
            }
        }

        public void Initialize()
        {
            uIManager = new UIManager();
            soundsManager = new SoundManager();
            currentPlayerTurn = 1;
            CurrentState = GameState.Menu;
        }

        public void InitializeCreator()
        {
            uIManager = new UIManager();
            creator = new PortraitCreator();
        }

        public void Generate()
        {
            if (!portraitsGenerated)
            {
                renderer = new PortraitRenderer();
                generator = new PortraitGenerator();

                allPortraits = generator.GeneratePortraits(24, userMaxAttributesInput);

                // Créer les deux joueur
                player1 = new Player(allPortraits.Take(24).ToArray(), 1);
                player2 = new Player(allPortraits.Skip(24).Take(24).ToArray(), 2);

                // Afficher les portraits générés
                foreach (Portrait p in allPortraits)
                {
                    renderer.LoadPotraitTextures(p);
                }

                portraitsGenerated = true;
            }
        }

        public void GenerateExample()
        { 
            renderer = new PortraitRenderer();
            generator = new PortraitGenerator();
            allPortraits = generator.GeneratePortraits(2, userMaxAttributesInput);

            player1 = new Player(allPortraits.Take(2).ToArray(), 1);

            foreach (Portrait p in allPortraits)
            {
                renderer.LoadPotraitTextures(p);
            }
        }

        public void CheckVictory(Player guesser, Player opponent)
        {
            // Vérifier si la supposition du joueur est correcte
            if (guesser.SelectedGuess == opponent.TargetPortrait)
            {
                CurrentState = GameState.Victory;
            }
        }

        public void SelectedPortrait(Player player, Portrait portrait)
        {
            player.TargetPortrait = portrait;
            portrait.isTarget = true;
        }

        public void Reset(Player player1, Player player2)
        {
            player1.Reset();
            player2.Reset();
            ResetTurn();
            StateSelectingPortrait = true;
            CurrentState = GameState.InGame;
        }

        public void EndGame(bool player1Won)
        {
            CurrentState = GameState.Victory;
        }
        

    }
}
