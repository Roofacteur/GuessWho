using Raylib_cs;
using static Raylib_cs.Raylib;
using System.Numerics;
using static GuessWho.GameManager;
using System.ComponentModel;


namespace GuessWho
{
    public class UIManager
    {
        private string[] menuLabels = { "Play", "Generation", "Characters", "Options", "Quit" };
        private GameState previousState = GameState.None;
        private Model guessWhoTitle;
        static Texture2D backgroundMenu;
        static Texture2D backgroundInGame;
        static Texture2D backgroundInGameSelecting;
        static Texture2D screenIcon;
        static Texture2D addCharacter;
        static int BasePortraitSize;
        static int cols;

        private bool inputInitialized = false;
        bool inputActive = false;
        string inputText = "";
        Rectangle inputBox = new Rectangle(320, 210, 50, 40);

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
            camera = new Camera3D(
                new Vector3(0.0f, 2.5f, 5.0f),
                new Vector3(0.0f, 1.5f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
                45.0f,
                CameraProjection.Perspective);

            DrawTexture(backgroundMenu, 0, 0, Color.White);
            BeginMode3D(camera);
            DrawModel(guessWhoTitle, new Vector3(0.0f, 2.5f, 3f), 2.0f, Color.White);
            EndMode3D();

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
        public void DrawSelectingPortraits(GameManager gameManager)
        {
            DrawTexture(backgroundInGameSelecting, 0, 0, Color.White);

            string hiddenPlayer;
            string displayedPlayer;

            gameManager.player1.Zone = new Rectangle(0, 0, GetScreenWidth() / 2, GetScreenHeight());
            gameManager.player2.Zone = new Rectangle(GetScreenWidth() / 2, 0, GetScreenWidth() / 2, GetScreenHeight());


            if (gameManager.userHasDualScreen)
            {
                cols = 6;
            }
            else
            {
                cols = 5;
            }

            DrawPortraitGrid(gameManager.player1.Board.Portraits, gameManager.renderer, gameManager.player1.Zone, 100, cols, BasePortraitSize, 1, gameManager);
            DrawPortraitGrid(gameManager.player2.Board.Portraits, gameManager.renderer, gameManager.player2.Zone, 100, cols, BasePortraitSize, 2, gameManager);

            HideBoard(gameManager);

            if (gameManager.GetCurrentPlayer() == 1)
            {
                hiddenPlayer = $"Player {gameManager.GetCurrentPlayer()} is choosing a character !\nPlayer 2 don't you dare look...";
                displayedPlayer = "Player 1 choose a character !\nPlayer 2 is gonna have to find it";
            }
            else
            {
                hiddenPlayer = $"Player {gameManager.GetCurrentPlayer()} is choosing a character !\nPlayer 1 don't you dare look...";
                displayedPlayer = "Player 2 choose a character !\nPlayer 1 is gonna have to find it";
            }

            DrawTitle(gameManager,
              gameManager.GetCurrentPlayer() == 1 ? displayedPlayer : hiddenPlayer,
              gameManager.GetCurrentPlayer() == 1 ? hiddenPlayer : displayedPlayer);

        }

        public void DrawGame(GameManager gameManager)
        {

            DrawTexture(backgroundInGame, 0, 0, Color.White);
            DrawBackToMenuButton(gameManager);

            gameManager.player1.Zone = new Rectangle(0, 0, GetScreenWidth() / 2, GetScreenHeight());
            gameManager.player2.Zone = new Rectangle(GetScreenWidth() / 2, 0, GetScreenWidth() / 2, GetScreenHeight());


            DrawPortraitGrid(gameManager.player1.Board.Portraits, gameManager.renderer, gameManager.player1.Zone, 100, cols, BasePortraitSize, 1, gameManager);
            DrawPortraitGrid(gameManager.player2.Board.Portraits, gameManager.renderer, gameManager.player2.Zone, 100, cols, BasePortraitSize, 2, gameManager);
            HideBoard(gameManager);

            if (gameManager.userHasDualScreen)
            {
                cols = 6;

                int reference = BasePortraitSize / 2;
                int namePositionY = reference / 4 + reference;
                int targetPortraitPositionY = reference / 4;

                if (gameManager.GetCurrentPlayer() == 1)
                {
                    DrawText(gameManager.player2.TargetPortrait.Name, GetScreenWidth() - reference - reference / 3, namePositionY, 20, Color.White);
                    gameManager.renderer.DrawPortrait(gameManager.player2.TargetPortrait, GetScreenWidth() - reference - reference / 3, targetPortraitPositionY, reference);

                }
                else
                {
                    DrawText(gameManager.player1.TargetPortrait.Name, GetScreenWidth() / 2 - reference - reference / 3, namePositionY, 20, Color.White);
                    gameManager.renderer.DrawPortrait(gameManager.player1.TargetPortrait, GetScreenWidth() / 2 - reference - reference / 3, targetPortraitPositionY, reference);
                }

            }
            else
            {
                cols = 5;
            }

            int currentPlayer = gameManager.GetCurrentPlayer();
            int otherPlayer = currentPlayer == 1 ? 2 : 1;
            string hiddenPlayer = $"Player {currentPlayer} is asking you a question !";
            string displayedPlayer = $"Ask player {otherPlayer} a question !";

            if(gameManager.GetCurrentPlayer() == 1)
            {
                DrawTitle(gameManager, displayedPlayer, hiddenPlayer);
            }
            else
            {
                DrawTitle(gameManager, hiddenPlayer, displayedPlayer);
            }
            
        }

        public void DrawGeneration(GameManager gameManager)
        {
            DrawTexture(backgroundMenu, 0, 0, Color.White);

            if (!inputInitialized)
            {
                inputText = gameManager.userMaxAttributesInput.ToString();
                inputInitialized = true;
            }

            string title = "Portrait generator";
            DrawTitle(gameManager, title);

            DrawText("Press R !", GetScreenWidth() / 2 + 300, GetScreenHeight() - 100, 40, Color.White);

            DrawText("Max similar genes in DNA :", 50, 220, 20, Color.White);
            DrawRectangleRec(inputBox, inputActive ? Color.SkyBlue : Color.LightGray);
            DrawRectangleLinesEx(inputBox, 1, Color.Black);
            DrawText(inputText, (int)inputBox.X + 5, (int)inputBox.Y + 5, 35, Color.Black);

            if (CheckCollisionPointRec(GetMousePosition(), inputBox) && IsMouseButtonPressed(MouseButton.Left))
            {
                inputActive = true;

                if (string.IsNullOrEmpty(inputText))
                {
                    inputText = gameManager.userMaxAttributesInput.ToString();
                }
            }
            else if (IsMouseButtonPressed(MouseButton.Left))
            {
                inputActive = false;
                if (int.TryParse(inputText, out int newValue))
                {
                    if (newValue > 1 && newValue <= 10)
                    {
                        gameManager.userMaxAttributesInput = newValue;
                    }
                    else
                    {
                        gameManager.userMaxAttributesInput = 4;
                        inputText = "4";
                    }
                }
                else
                {
                    gameManager.userMaxAttributesInput = 4;
                    inputText = "4";
                }
                inputActive = false;
            }

            if (inputActive)
            {
                int key = GetCharPressed();
                while (key > 0)
                {
                    if ((key >= 48 && key <= 57) && inputText.Length < 2)
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
                        if (newValue > 1 && newValue <= 10)
                        {
                            gameManager.userMaxAttributesInput = newValue;
                        }
                        else
                        {
                            gameManager.userMaxAttributesInput = 4;
                            inputText = "4";
                        }
                    }
                    else
                    {
                        gameManager.userMaxAttributesInput = 4;
                        inputText = "4";
                    }
                    inputActive = false;
                }

            }

            gameManager.player1.Zone = new Rectangle(
                GetScreenWidth() - 200,
                GetScreenHeight() / 6,
                GetScreenWidth() / 2,
                GetScreenHeight() / 2
            );
            DrawPortraitGrid(gameManager.player1.Board.Portraits, gameManager.renderer, gameManager.player1.Zone, 100, 6, BasePortraitSize * 2, 1, gameManager);

            DrawBackToMenuButton(gameManager);
        }

        public void DrawCreator(GameManager gameManager)
        {
            DrawTexture(backgroundMenu, 0, 0, Color.White);
            string title = "Your characters";
            DrawTitle(gameManager, title);
            DrawBackToMenuButton(gameManager);

            float scale = 0.5f;
            float texWidth = addCharacter.Width * scale;
            float texHeight = addCharacter.Height * scale;
            Vector2 position = new Vector2(
                GetScreenWidth() / 2 - texWidth / 2,
                GetScreenHeight() / 2 - texHeight / 2
            );

            DrawTextureEx(addCharacter, position, 0.0f, scale, Color.White);

            // Dimensions du bouton
            int buttonWidth = 500;
            int buttonHeight = 500;

            // Calcul de la position centrale
            Rectangle buttonBounds = new Rectangle(
                GetScreenWidth() / 2 - buttonWidth / 2,
                GetScreenHeight() / 2 - buttonHeight / 2,
                buttonWidth,
                buttonHeight
            );

            // Détection de la souris
            Vector2 mousePosition = GetMousePosition();
            bool isHovering = CheckCollisionPointRec(mousePosition, buttonBounds);
            bool isClicked = isHovering && IsMouseButtonPressed(MouseButton.Left);

            // Affichage du bouton
            Color buttonColor = new Color(255, 255, 255, 0);
            DrawRectangleRec(buttonBounds, buttonColor);

            if(isClicked )
            {
                Console.WriteLine("Click");
            }
        }


        public void DrawOptions(GameManager gameManager)
        {
            DrawTexture(backgroundMenu, 0, 0, Color.White);
            DrawBackToMenuButton(gameManager);
            string title = "Screen configuration";
            DrawTitle(gameManager, title);

            int singleX = GetScreenWidth() / 4 - 100;
            int dualX = 3 * GetScreenWidth() / 4 - 100;
            int textY = 350;
            int textWidth = 230;
            int textHeight = 40;

            Vector2 mousePos = GetMousePosition();
            Rectangle singleRect = new Rectangle(singleX, textY, textWidth, textHeight);
            Rectangle dualRect = new Rectangle(dualX, textY, textWidth, textHeight);

            Color hoverColor = new Color(255, 255, 255, 128);

            if (CheckCollisionPointRec(mousePos, singleRect))
            {
                DrawRectangle((int)singleRect.X, (int)singleRect.Y, (int)singleRect.Width, (int)singleRect.Height, hoverColor);
                if (IsMouseButtonPressed(MouseButton.Left))
                {
                    gameManager.userHasDualScreen = false;
                }
            }
            if (CheckCollisionPointRec(mousePos, dualRect))
            {
                DrawRectangle((int)dualRect.X, (int)dualRect.Y, (int)dualRect.Width, (int)dualRect.Height, hoverColor);
                if (IsMouseButtonPressed(MouseButton.Left))
                {
                    gameManager.userHasDualScreen = true;
                }
            }

            DrawText("Single screen", singleX, textY, 30, Color.White);
            DrawText("Dualscreen", dualX, textY, 30, Color.White);

            int screenWidth = GetScreenWidth();
            int iconWidth = 300;

            int singleIconX = screenWidth / 4 - iconWidth / 2;
            DrawTexture(screenIcon, singleIconX, 400, Color.White);

            int dualStartX = screenWidth / 2 + (screenWidth / 2 - iconWidth * 2) / 2;
            DrawTexture(screenIcon, dualStartX, 400, Color.White);
            DrawTexture(screenIcon, dualStartX + iconWidth, 400, Color.White);

            if (gameManager.userHasDualScreen)
            {
                DrawText("Selected", dualX, 320, 30, Color.Green);
            }
            else
            {
                DrawText("Selected", singleX, 320, 30, Color.Green);
            }
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

            int size = originalSize / 2;
            int spacing = 20;
            int gridWidth = cols * (size + spacing);
            int startX = (int)(zone.X + (zone.Width / 2) - (gridWidth / 2));

            const float hoverTarget = -8f;
            const float hoverSpeed = 10f;


            if (gameManager.userHasDualScreen) 
            {
                BasePortraitSize = 380;
            }
            else
            {
                BasePortraitSize = 300;
            }

            for (int i = 0; i < portraits.Length; i++)
            {
                int row = i / cols;
                int col = i % cols;
                int x = startX + col * (size + spacing);

                int baseY = (int)(zone.Y + startY + row * (size + spacing));

                Rectangle rect = new(x, baseY, size, size);
                bool isHovered = gameManager.GetCurrentPlayer() == playerId && CheckCollisionPointRec(GetMousePosition(), rect) && gameManager.CurrentState == GameState.InGame;

                float targetOffset = isHovered ? hoverTarget : 0f;

                portraits[i].HoverOffset = Raymath.Lerp(portraits[i].HoverOffset, targetOffset, hoverSpeed * GetFrameTime());

                int y = (int)(baseY + portraits[i].HoverOffset);

                if (!string.IsNullOrEmpty(portraits[i].Name))
                    DrawText(portraits[i].Name, x, y + size + 5, 25, Color.White);

                if (isHovered && IsMouseButtonPressed(MouseButton.Left) && gameManager.StateSelectingPortrait == false)
                {
                    portraits[i].IsEliminated = !portraits[i].IsEliminated;
                }

                if (isHovered && IsMouseButtonPressed(MouseButton.Left) && gameManager.StateSelectingPortrait)
                {
                    if(gameManager.player1.TargetPortrait == null && gameManager.GetCurrentPlayer() == 1)
                    {
                        gameManager.SelectedPortrait(gameManager.player1, portraits[i].Clone());
                        gameManager.NextTurn();
                    }
                    else if (gameManager.player2.TargetPortrait == null && gameManager.GetCurrentPlayer() == 2)
                    {
                        gameManager.SelectedPortrait(gameManager.player2, portraits[i].Clone());
                        gameManager.NextTurn();
                    }
                    
                }

                renderer.DrawPortrait(portraits[i], x, y, size);
                
            }
        }

        public void DrawBackToMenuButton(GameManager gameManager)
        {
            int btnX = 10;
            int btnY = 5;
            int btnWidth = 70;
            int btnHeight = 65;

            DrawRectangleLines(btnX, btnY, btnWidth, btnHeight, Color.White);

            DrawText("<-", btnX + 10, btnY, 60, Color.White);

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

        public void TextureLoader(GameManager gamemanager)
        {
            GameState state = gamemanager.CurrentState;

            if (state != previousState)
            {
                switch (state)
                {
                    case GameState.Menu:
                        backgroundMenu = LoadTexture("assets/backgrounds/MenuBackground.png");
                        guessWhoTitle = LoadModel("assets/model3D/title/GuessWho3DTitle.glb");
                        break;

                    case GameState.Generation:
                        backgroundMenu = LoadTexture("assets/backgrounds/MenuBackground.png");
                        break;
                    
                    case GameState.Creating:
                        backgroundMenu = LoadTexture("assets/backgrounds/MenuBackground.png");
                        addCharacter = LoadTexture("assets/icons/addCharacter.png");
                        break;

                    case GameState.Options:
                        backgroundMenu = LoadTexture("assets/backgrounds/MenuBackground.png");
                        screenIcon = LoadTexture("assets/icons/singlescreenicon.png");
                        break;

                    case GameState.InGame:
                        if (gamemanager.userHasDualScreen)
                        {
                            backgroundInGameSelecting = LoadTexture("assets/backgrounds/GameBackground.png");
                            backgroundInGame = LoadTexture("assets/backgrounds/GameBackgroundInverted.png");
                        }
                        else
                        {
                            backgroundInGameSelecting = LoadTexture("assets/backgrounds/GameSmallBackground.png");
                            backgroundInGame = LoadTexture("assets/backgrounds/GameSmallBackgroundInverted.png");
                        }
                        break;
                }

                previousState = state;
            }
        }
        void DrawTitle(GameManager gameManager, string textPlayer1, string? textPlayer2 = null, int yPosition = 30, int fontSize = 30, Color? color = null)
        {
            int screenWidth = GetScreenWidth();
            int halfScreenWidth = screenWidth / 2;
            Color finalColor = color ?? Color.White;

            if (string.IsNullOrEmpty(textPlayer2))
            {
                // Un seul texte à afficher, centré sur l'écran complet
                int textWidth = MeasureText(textPlayer1, fontSize);
                int positionX = (screenWidth - textWidth) / 2;
                DrawText(textPlayer1, positionX, yPosition, fontSize, finalColor);
            }
            else
            {
                // Deux textes à afficher, un par moitié d'écran
                int textWidthP1 = MeasureText(textPlayer1, fontSize);
                int textWidthP2 = MeasureText(textPlayer2, fontSize);

                int positionXP1 = (halfScreenWidth - textWidthP1) / 2;
                int positionXP2 = halfScreenWidth + (halfScreenWidth - textWidthP2) / 2;

                DrawText(textPlayer1, positionXP1, yPosition, fontSize, finalColor);
                DrawText(textPlayer2, positionXP2, yPosition, fontSize, finalColor);
            }
        }


        void HideBoard(GameManager gameManager)
        {
            int screenWidth = GetScreenWidth();
            int screenHeight = GetScreenHeight();

            Rectangle hidden = (gameManager.GetCurrentPlayer() == 1)
                ? new Rectangle(screenWidth / 2, 0, screenWidth / 2, screenHeight)
                : new Rectangle(0, 0, screenWidth / 2, screenHeight);

            Color transparentBlack = new Color(0, 0, 0, 128);

            DrawRectangle((int)hidden.X, (int)hidden.Y, (int)hidden.Width, (int)hidden.Height, transparentBlack);
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
            
            if (backgroundInGameSelecting.Id != 0)
            {
                UnloadTexture(backgroundInGameSelecting);
                backgroundInGameSelecting = new Texture2D();
            }

            if (guessWhoTitle.MeshCount > 0)
            {
                UnloadModel(guessWhoTitle);
                guessWhoTitle = new Model();
            }
        }


    }

}
