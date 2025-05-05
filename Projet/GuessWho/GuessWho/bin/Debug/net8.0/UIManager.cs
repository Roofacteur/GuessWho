using Raylib_cs;
using static Raylib_cs.Raylib;
using System.Numerics;
using static GuessWho.GameManager;


namespace GuessWho
{
    public class UIManager
    {
        private string[] menuLabels = { "Play", "Generation", "Characters", "Options", "Quit" };
        private GameState previousState = GameState.None;
        private Model guessWhoTitle;
        static Texture2D backgroundMenu;
        static Texture2D backgroundInGame;
        const int BasePortraitSize = 350;

        bool inputActive = false;
        string inputText = "";
        Rectangle inputBox = new Rectangle(50, 290, 200, 30);

        Camera3D camera;

        public void UpdateMenu(GameManager gameManager)
        {
            Vector2 mouse = GetMousePosition();
            DrawMenu();

            if (IsMouseButtonPressed(MouseButton.Left))
            {
                for (int i = 0; i < 5; i++)
                {
                    if (CheckCollisionPointRec(mouse, GetButtonRect(i)))
                    {
                        if (i == 0) gameManager.CurrentState = GameState.InGame;
                        if (i == 1) gameManager.CurrentState = GameState.Generation;
                        if (i == 2) gameManager.CurrentState = GameState.Creating;
                        if (i == 3) gameManager.CurrentState = GameState.Options;
                        if (i == 4) Environment.Exit(0);
                    }
                }
            }
        }
        public void DrawMenu()
        {
            // Interface 2D
            for (int i = 0; i < menuLabels.Length; i++)
            {
                Rectangle btn = GetButtonRect(i);
                bool hover = CheckCollisionPointRec(GetMousePosition(), btn);
                Color color = hover ? Color.SkyBlue : Color.LightGray;

                DrawRectangleRec(btn, color);

                int fontSize = 30;
                Vector2 textSize = MeasureTextEx(GetFontDefault(), menuLabels[i], fontSize, 1);
                Vector2 textPos = new Vector2(btn.X + (btn.Width - textSize.X) / 2, btn.Y + (btn.Height - textSize.Y) / 2);
                DrawText(menuLabels[i], (int)textPos.X, (int)textPos.Y, fontSize, Color.DarkBlue);
            }
        }

        public void DrawGame(GameManager gameManager)
        {
            gameManager.player1.Zone = new Rectangle(0, 0, GetScreenWidth() / 2, GetScreenHeight());
            gameManager.player2.Zone = new Rectangle(GetScreenWidth() / 2, 0, GetScreenWidth() / 2, GetScreenHeight());

            DrawPortraitGrid(gameManager.player1.Board.Portraits, gameManager.renderer, gameManager.player1.Zone, 100, 6, BasePortraitSize, 1, gameManager);
            DrawPortraitGrid(gameManager.player2.Board.Portraits, gameManager.renderer, gameManager.player2.Zone, 100, 6, BasePortraitSize, 2, gameManager);

            string turnText = $"Ask player {gameManager.GetCurrentPlayer()} a question !";
            int positionXText = (gameManager.GetCurrentPlayer() == 1) ? GetScreenWidth() / 6 : GetScreenWidth() / 2 + GetScreenWidth() / 6;

            DrawText(turnText, positionXText, 30, 40, Color.White);

            //string similarAttributes = gameManager.userMaxAttributesInput.ToString();
            //DrawText("DNA : " + similarAttributes, 50, 150, 20, Color.White);

        }

        public void DrawGeneration(GameManager gameManager)
        {
            DrawBackToMenuButton(gameManager);
            DrawText("Portrait generation examples", 50, 60, 30, Color.White);
            DrawText("Press R !", GetScreenWidth()/2 -100, GetScreenHeight() - 100, 40, Color.White);

            
            string similarAttributes = gameManager.userMaxAttributesInput.ToString();
            DrawText("Maximum of similar genes in DNA : " + similarAttributes, 50, 150, 20, Color.White);

            // Zone d’affichage de la boîte de texte
            DrawText("Set maximum of similar genes in DNA", 50, 270, 20, Color.White);
            DrawRectangleRec(inputBox, inputActive ? Color.SkyBlue : Color.LightGray);
            DrawRectangleLinesEx(inputBox, 1, Color.Black);

            // Affichage du texte saisi
            DrawText(inputText, (int)inputBox.X + 5, (int)inputBox.Y + 5, 20, Color.Black);

            // Activation de la zone de saisie si clic
            if (CheckCollisionPointRec(GetMousePosition(), inputBox) && IsMouseButtonPressed(MouseButton.Left))
            {
                inputActive = true;
            }
            else if (IsMouseButtonPressed(MouseButton.Left))
            {
                inputActive = false;
            }

            if (inputActive)
            {
                int key = GetCharPressed();
                while (key > 0)
                {
                    if ((key >= 48 && key <= 57) && inputText.Length < 3)
                    {
                        inputText += (char)key;
                    }
                    key = GetCharPressed();
                }

                if (IsKeyPressed(KeyboardKey.Backspace) && inputText.Length > 0)
                {
                    inputText = inputText.Substring(0, inputText.Length - 1);
                }

                if (IsKeyPressed(KeyboardKey.Enter) && inputText.Length > 0)
                {
                    if (int.TryParse(inputText, out int newValue))
                    {
                        if (newValue > 1 && newValue <= 10 )
                        {
                            gameManager.userMaxAttributesInput = newValue;
                        }
                        
                    }
                    inputText = ""; // Réinitialise le champ après validation
                    inputActive = false;
                }
            }

            // Dessin des portraits
            gameManager.player1.Zone = new Rectangle(
                GetScreenWidth() / 1.5f,
                GetScreenHeight() / 6,
                GetScreenWidth() / 2,
                GetScreenHeight() / 2
            );
            DrawPortraitGrid(gameManager.player1.Board.Portraits, gameManager.renderer, gameManager.player1.Zone, 100, 6, BasePortraitSize * 2, 1, gameManager);
        }

        public void DrawCreator(GameManager gameManager)
        {
            DrawBackToMenuButton(gameManager);
        }


        public void DrawOptions(GameManager gameManager)
        {
            DrawBackToMenuButton(gameManager);

        }

        public void DrawPortraitGrid(
            Portrait[] portraits,
            PortraitRenderer renderer,
            Rectangle zone,
            int startY,
            int cols,
            int originalSize,
            int playerId,
            GameManager gameManager)
        {

            DrawBackToMenuButton(gameManager);

            int size = originalSize / 2;
            int spacing = 20;
            int gridWidth = cols * (size + spacing);
            int startX = (int)(zone.X + (zone.Width / 2) - (gridWidth / 2));

            const float hoverTarget = -8f;
            const float hoverSpeed = 10f;

            for (int i = 0; i < portraits.Length; i++)
            {
                int row = i / cols;
                int col = i % cols;
                int x = startX + col * (size + spacing);

                int baseY = (int)(zone.Y + startY + row * (size + spacing));

                Rectangle rect = new(x, baseY, size, size);
                bool isHovered = gameManager.GetCurrentPlayer() == playerId && CheckCollisionPointRec(GetMousePosition(), rect);

                float targetOffset = isHovered ? hoverTarget : 0f;
                portraits[i].HoverOffset = Raymath.Lerp(portraits[i].HoverOffset, targetOffset, hoverSpeed * GetFrameTime());

                int y = (int)(baseY + portraits[i].HoverOffset);

                // Dessin
                if (!string.IsNullOrEmpty(portraits[i].Name))
                    DrawText(portraits[i].Name, x, y + size + 5, 25, Color.White);

                if (isHovered && IsMouseButtonPressed(MouseButton.Left))
                {
                    portraits[i].IsEliminated = !portraits[i].IsEliminated;
                }

                renderer.DrawPortrait(portraits[i], x, y, size);
            }
        }

        public void DrawBackToMenuButton(GameManager gameManager)
        {
            // Rectangle bouton
            int btnX = 10;
            int btnY = 10;
            int btnWidth = 150;
            int btnHeight = 40;

            // Dessiner le rectangle du bouton
            DrawRectangleLines(btnX, btnY, btnWidth, btnHeight, Color.White);

            DrawText("Back to menu", btnX + 10, btnY + 10, 18, Color.White);

            if (CheckCollisionPointRec(GetMousePosition(), new Rectangle(btnX, btnY, btnWidth, btnHeight)))
            {
                DrawRectangle(btnX, btnY, btnWidth, btnHeight, new Color(255, 255, 255, 64));

                if (IsMouseButtonPressed(MouseButton.Left))
                {
                    gameManager.CurrentState = GameState.Menu;
                }
            }
                
        }

        public void DrawEndScreen(GameState state, int winner)
        {
            DrawRectangle(0, 0, 1280, 720, Color.Green);
            DrawText($"Player {winner} wins !", 400, 300, 40, Color.Black);
            DrawText("Press R to restart", 360, 360, 20, Color.Black);

        }

        private Rectangle GetButtonRect(int index)
        {
            float width = 200;
            float height = 60;
            float spacing = 20;
            float totalHeight = (height + spacing) * 3 - spacing;
            float startY = (GetScreenHeight() - totalHeight) / 2 + 70;

            return new Rectangle(
                (GetScreenWidth() - width) / 2,
                startY + index * (height + spacing),
                width,
                height
            );
        }

        public void DrawBackground(GameManager gamemanager)
        {
            GameState state = gamemanager.CurrentState;

            // Chargement des ressources uniquement si l'état a changé
            if (state != previousState)
            {
                switch (state)
                {
                    case GameState.Menu:
                        backgroundMenu = LoadTexture("assets/backgrounds/MenuBackground.png");
                        guessWhoTitle = LoadModel("assets/model3D/title/GuessWho3DTitle.glb");

                        camera = new Camera3D(
                            new Vector3(0.0f, 2.5f, 5.0f),
                            new Vector3(0.0f, 1.5f, 0.0f),
                            new Vector3(0.0f, 1.0f, 0.0f),
                            45.0f,
                            CameraProjection.Perspective);
                        break;

                    case GameState.Generation:
                        backgroundMenu = LoadTexture("assets/backgrounds/MenuBackground.png");
                        break;
                    
                    case GameState.Creating:
                        backgroundMenu = LoadTexture("assets/backgrounds/MenuBackground.png");
                        break;

                    case GameState.Options:
                        backgroundMenu = LoadTexture("assets/backgrounds/MenuBackground.png");
                        break;

                    case GameState.InGame:
                        backgroundInGame = LoadTexture("assets/backgrounds/GameBackground.png");
                        break;
                }

                previousState = state;
            }

            // Affichage selon l'état actuel
            switch (state)
            {
                case GameState.Menu:
                    DrawTexture(backgroundMenu, 0, 0, Color.White);

                    BeginMode3D(camera);
                    DrawModel(guessWhoTitle, new Vector3(0.0f, 2.5f, 3f), 2.0f, Color.White);
                    EndMode3D();
                    break;

                case GameState.Generation:
                    DrawTexture(backgroundMenu, 0, 0, Color.White);
                    break;

                case GameState.Creating:
                    DrawTexture(backgroundMenu, 0, 0, Color.White);
                    break;

                case GameState.Options:
                    DrawTexture(backgroundMenu, 0, 0, Color.White);
                    break;

                case GameState.InGame:
                    DrawTexture(backgroundInGame, 0, 0, Color.White);
                    break;
            }
        }
        public void UnloadAll()
        {
            if (backgroundMenu.Id != 0)
            {
                UnloadTexture(backgroundMenu);
                backgroundMenu = new Texture2D();
            } 

            if (backgroundInGame.Id != 0)
            {
                UnloadTexture(backgroundInGame);
                backgroundInGame = new Texture2D();
            }

            if (guessWhoTitle.MeshCount > 0)
            {
                UnloadModel(guessWhoTitle);
                guessWhoTitle = new Model();
            }
        }


    }

}
