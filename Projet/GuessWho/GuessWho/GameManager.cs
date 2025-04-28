using Raylib_cs;
using static Raylib_cs.Raylib;

namespace GuessWho
{
    public class GameManager
    {
        bool gameStarted = false;

        public enum GameState
        {
            Menu,
            SelectingPortraits,
            InGame,
            Guessing,
            Victory
        }
        public GameState CurrentState;

        public Portrait[] allPortraits;
        public Player player1;
        public Player player2;
        public PortraitGenerator generator;
        public PortraitRenderer renderer;
        public UIManager uiManager;
        public int currentPlayerTurn = 1;
        public int GetCurrentPlayer() => currentPlayerTurn;
        public void NextTurn() => currentPlayerTurn = (currentPlayerTurn == 1) ? 2 : 1;
        public void ResetTurn() => currentPlayerTurn = 1;

        public void Update()
        {
            switch (CurrentState)
            {
                case GameState.Menu:
                    uiManager.DrawMenu();
                    if (IsKeyPressed(KeyboardKey.End)) Initialize();
                    break;

                case GameState.SelectingPortraits:
                    HandlePortraitSelection();
                    break;

                case GameState.InGame:
                    if (!gameStarted)
                    {
                        Generate();
                        gameStarted = true;
                    }
                    break;

                case GameState.Guessing:
                    HandleGuessing();
                    break;

                case GameState.Victory:
                    uiManager.DrawEndScreen(CurrentState, currentPlayerTurn);
                    if (IsKeyPressed(KeyboardKey.R))
                    {
                        gameStarted = false;
                        Generate();
                    }
                    break;
            }
        }

        public void Initialize()
        {
            uiManager = new UIManager();
            currentPlayerTurn = 1;
            CurrentState = GameState.Menu;
        }

        public void Generate()
        {
            // Chargement et mélange des noms
            List<string> names = LoadNames(Path.Combine(Directory.GetCurrentDirectory(), "assets", "names.txt"));
            if (names.Count < 48)
            {
                throw new Exception("Erreur critique : pas assez de noms dans 'names.txt' !");
            }

            names = names.OrderBy(_ => new Random().Next()).ToList();

            renderer = new PortraitRenderer();
            generator = new PortraitGenerator();
            allPortraits = generator.GeneratePortraits(48);

            // Attribution des noms
            for (int i = 0; i < allPortraits.Length; i++)
            {
                allPortraits[i].Name = names[i];
            }

            // Attribution aux joueurs
            player1 = new Player(allPortraits.Take(24).ToArray(), 1);
            player2 = new Player(allPortraits.Skip(24).Take(24).ToArray(), 2);

            // Chargement des textures
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

        private void HandleGuessing()
        {
            // Gérer la phase de deviner le portrait de l’adversaire
        }
        private void HandlePortraitSelection()
        {
            // Pour chaque joueur, gérer la sélection d'un portrait
            if (IsKeyPressed(KeyboardKey.Enter))
            {
                CurrentState = GameState.InGame;
            }
        }

        public void Reset(Player player1, Player player2, List<Portrait> newPortraits)
        {
            player1.Reset();
            player2.Reset();
            ResetTurn();
            CurrentState = GameState.SelectingPortraits;
        }

        public void EndGame(bool player1Won)
        {
            CurrentState = GameState.Victory;
        }
        public static List<string> LoadNames(string filePath)
        {
            try
            {
                string fileContent = File.ReadAllText(filePath);
                return fileContent.Split(',').Select(name => name.Trim()).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur de lecture du fichier : " + ex.Message);
                return new List<string>();


            }
        }
    }
}
