using Raylib_cs;
using System.Drawing;
using static Raylib_cs.Raylib;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GuessWho
{
    public class GameManager
    {
        bool gameStarted = false;
        public bool portraitsGenerated = false;
        public bool generatedExample = false;

        public enum GameState
        {
            None,
            Menu,
            Options,
            Generation,
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
        public PortraitRenderer renderer = new PortraitRenderer();
        public UIManager uIManager;
        public int currentPlayerTurn = 1;
        public int userMaxAttributesInput;
        public int GetCurrentPlayer() => currentPlayerTurn;
        public void NextTurn() => currentPlayerTurn = (currentPlayerTurn == 1) ? 2 : 1;
        public void ResetTurn() => currentPlayerTurn = 1;

        public void Update(GameManager gamemanager)
        {
            switch (CurrentState)
            {
                case GameState.Menu:
                    gameStarted = false;
                    portraitsGenerated = false;
                    generatedExample = false;
                    uIManager.DrawBackground(gamemanager);
                    uIManager.UpdateMenu(gamemanager);
                    uIManager.DrawMenu();
                    break;

                case GameState.Options:
                    gameStarted = false;
                    portraitsGenerated = false;
                    uIManager.DrawOptions();
                    break;

                case GameState.Generation:

                    uIManager.DrawBackground(gamemanager);
                    if (!generatedExample)
                    {
                        GenerateExample();
                        generatedExample = true;
                    }
                    uIManager.DrawGeneration(gamemanager);
                    break;

                case GameState.SelectingPortraits:
                    
                    break;

                case GameState.InGame:

                    if (!gameStarted)
                    {
                        ResetTurn();
                        Generate();
                        gameStarted = true;
                    }
                    uIManager.DrawBackground(gamemanager);
                    uIManager.DrawGame(gamemanager);
                    break;

                case GameState.Guessing:
                    HandleGuessing();
                    break;

                case GameState.Victory:
                    uIManager.DrawEndScreen(CurrentState, currentPlayerTurn);
                    if (IsKeyPressed(KeyboardKey.R))
                    {
                        gameStarted = false;
                        ResetTurn();
                        Generate();
                    }
                    break;
            }
        }

        public void Initialize()
        {
            uIManager = new UIManager();
            currentPlayerTurn = 1;
            CurrentState = GameState.Menu;
        }

        public void Generate()
        {
            if (!portraitsGenerated)
            {
                // Récupérer les noms
                List<string> names = LoadNames(Path.Combine(Directory.GetCurrentDirectory(), "assets", "names.txt"));
                if (names.Count < 48)
                {
                    throw new Exception("Erreur critique : le fichier 'names.txt' doit contenir au moins 48 noms **uniques** !");
                }

                // Mélange aléatoire des noms en triant selon des GUID générés aléatoirement
                names = names.OrderBy(str => Guid.NewGuid()).Take(48).ToList();

                renderer = new PortraitRenderer();
                generator = new PortraitGenerator();
                allPortraits = generator.GeneratePortraits(48, userMaxAttributesInput);

                // Ajouter le nom au portrait
                for (int i = 0; i < allPortraits.Length; i++)
                {
                    allPortraits[i].Name = names[i];
                }

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
            List<string> names = LoadNames(Path.Combine(Directory.GetCurrentDirectory(), "assets", "names.txt"));
            if (names.Count < 48)
            {
                throw new Exception("Erreur critique : le fichier 'names.txt' doit contenir au moins 48 noms **uniques** !");
            }

            // Mélange aléatoire des noms en triant selon des GUID générés aléatoirement
            names = names.OrderBy(str => Guid.NewGuid()).Take(48).ToList();

            renderer = new PortraitRenderer();
            generator = new PortraitGenerator();
            allPortraits = generator.GeneratePortraits(2, userMaxAttributesInput);

            for (int i = 0; i < allPortraits.Length; i++)
            {
                allPortraits[i].Name = names[i];
            }

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

        private void HandleGuessing()
        {
            // Gérer la phase de deviner le portrait de l’adversaire
        }
        private void HandlePortraitSelection()
        { 
            
        }

        public void Reset(Player player1, Player player2)
        {
            //Recommencer la partie

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
                //Récupération d'un nom différent dans le fichier

                string fileContent = File.ReadAllText(filePath);
                return fileContent
                    .Split(',')
                    .Select(name => name.Trim()) //Éviter les espaces
                    .Where(name => !string.IsNullOrWhiteSpace(name)) //Éviter les chaînes vides
                    .Distinct() //Éviter les doublons
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur de lecture du fichier : " + ex.Message);
                return new List<string>();
            }
        }

    }
}
