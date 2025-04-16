using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GuessWho
{
    public class GameManager
    {
        public enum GameState
        {
            Menu,
            SelectingPortraits,
            InGame,
            Guessing,
            Victory,
            Defeat
        }

        public GameState currentState;
        public GameState CurrentState => currentState;

        public Player player1;
        public Player player2;
        public PortraitGenerator generator;
        public UIManager uiManager;
        private InputHandler inputHandler = new();
        public int currentPlayerTurn = 1;
        public int GetCurrentPlayer() => currentPlayerTurn;
        public void NextTurn() => currentPlayerTurn = (currentPlayerTurn == 1) ? 2 : 1;
        public void ResetTurn() => currentPlayerTurn = 1;


        public void Initialize()
        {
            generator = new PortraitGenerator();
            var portraits = generator.GeneratePortraits(48);
            player1 = new Player(portraits.Take(24).ToArray(), 1);
            player2 = new Player(portraits.Skip(24).ToArray(), 2);
            currentPlayerTurn = 1;
            uiManager = new UIManager();
            currentState = GameState.InGame;
        }

        public void Update()
        {
            switch (currentState)
            {
                case GameState.Menu:
                    uiManager.DrawMenu();
                    if (Raylib.IsKeyPressed(KeyboardKey.End)) Initialize();
                    break;

                case GameState.SelectingPortraits:
                    HandlePortraitSelection();
                    break;

                case GameState.InGame:
                    HandleGameLogic();
                    break;

                case GameState.Guessing:
                    HandleGuessing();
                    break;

                case GameState.Victory:
                case GameState.Defeat:
                    uiManager.DrawEndScreen(currentState, currentPlayerTurn);
                    if (Raylib.IsKeyPressed(KeyboardKey.R)) Initialize();
                    break;
            }
        }

        public void CheckVictory(Player guesser, Player opponent)
        {
            // Vérifier si la supposition du joueur est correcte
            if (guesser.SelectedGuess == opponent.TargetPortrait)
            {
                currentState = GameState.Victory;
            }
            else
            {
                currentState = GameState.Defeat;
            }
        }


        private void HandleGameLogic()
        {
            // Logique principale de gestion des actions du joueur (sélection portrait, etc.)
        }

        private void HandleGuessing()
        {
            // Gérer la phase de deviner le portrait de l’adversaire
        }
        private void HandlePortraitSelection()
        {
            // Afficher l'écran de sélection des portraits pour les joueurs
            // Exemple de code d'affichage :
            uiManager.DrawBoard(player1.Board);
            uiManager.DrawBoard(player2.Board);

            // Pour chaque joueur, gérer la sélection d'un portrait
            if (Raylib.IsKeyPressed(KeyboardKey.Enter))  // Une fois que les joueurs ont choisi
            {
                currentState = GameState.InGame;
            }
        }

        public void Reset(Player player1, Player player2, List<Portrait> newPortraits)
        {
            player1.Reset();
            player2.Reset();
            ResetTurn();
            currentState = GameState.SelectingPortraits;
        }

        public void EndGame(bool player1Won)
        {
            currentState = player1Won ? GameState.Victory : GameState.Defeat;
        }
    }
}
