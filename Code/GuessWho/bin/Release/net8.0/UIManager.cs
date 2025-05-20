using Raylib_cs;
using static Raylib_cs.Raylib;
using System.Numerics;
using static GuessWho.GameManager;
using System.ComponentModel;


namespace GuessWho
{
    public class UIManager
    {
        private string[] menuLabels = { "PLAY", "RULES", "CHARACTERS", "SETTINGS", "QUIT" };
        private GameState previousState = GameState.None;
        private Model guessWhoTitle;
        static Texture2D backgroundMenu;
        static Texture2D backgroundInGame;
        static Texture2D backgroundInGameSelecting;
        static Texture2D screenIcon;
        static Texture2D addCharacter;
        static Texture2D speakerIcon;
        static Texture2D sfxIcon;
        static Texture2D rules1Icon;
        static Texture2D rules2Icon;
        static Texture2D rules3Icon;
        Color trueYellow = new Color(255, 196, 0);
        static int BasePortraitSize;
        static int ExamplePortraitSize = 760;
        static int cols;

        private int page = 1;
        private readonly int lastPage = 3;

        private bool inputInitialized = false;
        bool inputActive = false;
        string inputText = "";
        Rectangle inputBox = new Rectangle(320, 210, 50, 40);

        Camera3D camera;

        public void UpdateMenu(GameManager gameManager)
        {
            Vector2 mouse = GetMousePosition();
            DrawMenu(gameManager);

            if (IsMouseButtonPressed(MouseButton.Left))
            {
                for (int i = 0; i < 5; i++)
                {
                    if (CheckCollisionPointRec(mouse, GetButtonRect(i)))
                    {
                        if (i == 0) gameManager.CurrentState = GameState.InGame;
                        if (i == 1) gameManager.CurrentState = GameState.Rules;
                        if (i == 2) gameManager.CurrentState = GameState.Creating;
                        if (i == 3) gameManager.CurrentState = GameState.Settings;
                        if (i == 4) Environment.Exit(0);
                        PlaySound(gameManager.soundManager.flickSound);
                    }
                }
            }
        }
        public void DrawMenu(GameManager gameManager)
        {
           
            camera = new Camera3D(
                new Vector3(0.0f, 2.5f, 5.0f),
                new Vector3(0.0f, 1.5f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
                45.0f,
                CameraProjection.Perspective);

            DrawTexture(backgroundMenu, 0, 0, Color.White);
            DrawSoundButtons(gameManager);
            BeginMode3D(camera);
            DrawModel(guessWhoTitle, new Vector3(0.0f, 2.5f, 3f), 2.0f, Color.White);
            EndMode3D();

            for (int i = 0; i < menuLabels.Length; i++)
            {
                Rectangle btn = GetButtonRect(i);

                bool hover = CheckCollisionPointRec(GetMousePosition(), btn);

                Color color = hover ? trueYellow : Color.White;

                DrawRectangleLinesEx(btn, 2, color);  

                int fontSize = 30;
                Vector2 textSize = MeasureTextEx(GetFontDefault(), menuLabels[i], fontSize, 1);
                Vector2 textPos = new Vector2(btn.X + (btn.Width - textSize.X) / 2, btn.Y + (btn.Height - textSize.Y) / 2);

                DrawText(menuLabels[i], (int)textPos.X, (int)textPos.Y, fontSize, color);
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

            DrawSoundButtons(gameManager);

        }

        public void DrawGame(GameManager gameManager)
        {

            DrawTexture(backgroundInGame, 0, 0, Color.White);

            gameManager.player1.Zone = new Rectangle(0, 0, GetScreenWidth() / 2, GetScreenHeight());
            gameManager.player2.Zone = new Rectangle(GetScreenWidth() / 2, 0, GetScreenWidth() / 2, GetScreenHeight());

            DrawPortraitGrid(gameManager.player1.Board.Portraits, gameManager.renderer, gameManager.player1.Zone, 100, cols, BasePortraitSize, 1, gameManager);
            DrawPortraitGrid(gameManager.player2.Board.Portraits, gameManager.renderer, gameManager.player2.Zone, 100, cols, BasePortraitSize, 2, gameManager);
            HideBoard(gameManager);

            DrawBackToMenuButton(gameManager, GameState.Menu);
            DrawSoundButtons(gameManager);

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

            string title = "PORTRAIT GENERATOR";
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
            DrawPortraitGrid(gameManager.player1.Board.Portraits, gameManager.renderer, gameManager.player1.Zone, 100, 6, ExamplePortraitSize, 1, gameManager);

            DrawBackToMenuButton(gameManager, GameState.Creating);
        }

        public void DrawRules(GameManager gameManager)
        {
            int screenWidth = GetScreenWidth();
            int screenHeight = GetScreenHeight();

            Rectangle rightArrow = new Rectangle(screenWidth / 2 + 50, screenHeight - 100, 50, 50);
            Rectangle leftArrow = new Rectangle(screenWidth / 2 - 50, screenHeight - 100, 50, 50);

            DrawTexture(backgroundMenu, 0, 0, Color.White);
            DrawBackToMenuButton(gameManager, GameState.Menu);

            Vector2 mouse = GetMousePosition();

            bool isHoverRight = CheckCollisionPointRec(mouse, rightArrow);
            Color rightColor = (page == lastPage) ? Color.Gray : (isHoverRight ? trueYellow : Color.White);
            DrawText("->", (int)rightArrow.X, (int)rightArrow.Y, 50, rightColor);

            if (isHoverRight && IsMouseButtonPressed(MouseButton.Left) && page < lastPage)
            {
                page++;
            }

            bool isHoverLeft = CheckCollisionPointRec(mouse, leftArrow);
            Color leftColor = (page == 1) ? Color.Gray : (isHoverLeft ? trueYellow : Color.White);
            DrawText("<-", (int)leftArrow.X, (int)leftArrow.Y, 50, leftColor);

            if (isHoverLeft && IsMouseButtonPressed(MouseButton.Left) && page > 1)
            {
                page--;
            }


            if(page == 1)
            {

                string pg1 = "Stages";
                int textWidth = MeasureText(pg1, 30);
                DrawText(pg1, screenWidth / 2 - textWidth / 2, 100, 30, Color.White);
                DrawTexture(rules2Icon, 0, 0, Color.White);       
            }
            else if(page == 2)
            {
                string pg2 = "Commands";
                int textWidth = MeasureText(pg2, 30);
                DrawText(pg2, screenWidth / 2 - textWidth / 2, 100, 30, Color.White);
                DrawTexture(rules1Icon, 0, 0, Color.White);
            }
            else if(page == 3)
            {
                string pg3 = "In-game rules";
                int textWidth = MeasureText(pg3, 30);
                DrawText(pg3, screenWidth / 2 - textWidth / 2, 100, 30, Color.White);
                DrawTexture(rules3Icon, 0, 0, Color.White);
            }
        }

        public void DrawCreator(GameManager gameManager)
        {
            // Définition des constantes
            const string title = "YOUR CHARACTERS";
            const string genButtonText = "GENERATION";
            const int genButtonWidth = 200;
            const int genButtonHeight = 50;
            const int buttonWidth = 500;
            const int buttonHeight = 500;
            const int margin = 20;
            const float scale = 0.5f;
            const int fontSize = 20;

            // Dimensions et position de l'image
            float texWidth = addCharacter.Width * scale;
            float texHeight = addCharacter.Height * scale;
            Vector2 position = new Vector2(
                GetScreenWidth() / 2 - texWidth / 2,
                GetScreenHeight() / 2 - texHeight / 2
            );

            Vector2 mousePosition = GetMousePosition();

            // Calcul des bounds du bouton principal
            Rectangle buttonBounds = new Rectangle(
                GetScreenWidth() / 2 - buttonWidth / 2,
                GetScreenHeight() / 2 - buttonHeight / 2,
                buttonWidth, buttonHeight
            );
            bool isHovering = CheckCollisionPointRec(mousePosition, buttonBounds);
            bool isClicked = isHovering && IsMouseButtonPressed(MouseButton.Left);

            // Bounds et interaction du bouton "Génération"
            Rectangle genButtonBounds = new Rectangle(
                margin,
                GetScreenHeight() - genButtonHeight - margin,
                genButtonWidth,
                genButtonHeight
            );

            int textWidth = MeasureText(genButtonText, fontSize);
            int textX = (int)genButtonBounds.X + (int)(genButtonBounds.Width - textWidth) / 2;
            int textY = (int)genButtonBounds.Y + (int)(genButtonBounds.Height - fontSize) / 2;
            bool isHoveringGen = CheckCollisionPointRec(mousePosition, genButtonBounds);
            bool isClickedGen = isHoveringGen && IsMouseButtonPressed(MouseButton.Left);

            DrawTexture(backgroundMenu, 0, 0, Color.White);
            DrawTitle(gameManager, title);
            DrawBackToMenuButton(gameManager, GameState.Menu);
            DrawTextureEx(addCharacter, position, 0.0f, scale, Color.White);
            DrawRectangleRec(buttonBounds, new Color(255, 255, 255, 0));

            if (isClicked)
            {
                Console.WriteLine("Click");
            }

            Color genButtonColor = isHoveringGen ? trueYellow : Color.White;

            DrawText(genButtonText, textX, textY, fontSize, genButtonColor);

            if (isClickedGen)
            {
                gameManager.CurrentState = GameState.Generation;
            }
        }


        public void DrawOptions(GameManager gameManager)
        {
            DrawTexture(backgroundMenu, 0, 0, Color.White);
            DrawBackToMenuButton(gameManager, GameState.Menu);
            string title = "SETTINGS";
            DrawTitle(gameManager, title);

            int screenWidth = GetScreenWidth();
            int iconWidth = 300;
            int screenConfigTitleY = 130;
            int singleX = screenWidth / 4 - 100;
            int dualX = 3 * screenWidth / 4 - 100;
            int textY = 280; 
            int textWidth = 230;
            int textHeight = 40;


            Vector2 mousePos = GetMousePosition();
            Rectangle singleRect = new Rectangle(singleX, textY, textWidth, textHeight);
            Rectangle dualRect = new Rectangle(dualX, textY, textWidth, textHeight);

            DrawText("Screen configuration", screenWidth / 2 - MeasureText("Screen configuration", 30) / 2, 130, 30, Color.White);
            int lineMargin = 120;
            int lineY = screenConfigTitleY + 40;
            DrawRectangle(lineMargin, lineY, screenWidth - 2 * lineMargin, 2, Color.White);

            if (CheckCollisionPointRec(mousePos, singleRect))
            {
                DrawText("Single screen", singleX, textY, 30, trueYellow);
                if (IsMouseButtonPressed(MouseButton.Left))
                {
                    gameManager.userHasDualScreen = false;
                    PlaySound(gameManager.soundManager.flickSound);
                }
            }
            else
            {
                DrawText("Single screen", singleX, textY, 30, Color.White);
            }
            if (CheckCollisionPointRec(mousePos, dualRect))
            {
                DrawText("Dualscreen", dualX, textY, 30, trueYellow);
                if (IsMouseButtonPressed(MouseButton.Left))
                {
                    gameManager.userHasDualScreen = true;
                    PlaySound(gameManager.soundManager.flickSound);
                }
            }
            else
            {
                DrawText("Dualscreen", dualX, textY, 30, Color.White);
            }

            int iconY = textY + 50; 

            int singleIconX = screenWidth / 4 - iconWidth / 2;
            DrawTexture(screenIcon, singleIconX, iconY, Color.White);

            int dualStartX = screenWidth / 2 + (screenWidth / 2 - iconWidth * 2) / 2;
            DrawTexture(screenIcon, dualStartX, iconY, Color.White);
            DrawTexture(screenIcon, dualStartX + iconWidth, iconY, Color.White);


            if (gameManager.userHasDualScreen)
            {
                DrawText("Selected", dualX, textY - 30, 30, Color.Green);
            }
            else
            {
                DrawText("Selected", singleX, textY - 30, 30, Color.Green);
            }

            int soundConfigY = iconY + iconWidth + 20;
            DrawText("Sound configuration", screenWidth / 2 - MeasureText("Sound configuration", 30) / 2, soundConfigY, 30, Color.White);
            DrawRectangle(lineMargin, lineY * 4 + 12, screenWidth - 2 * lineMargin, 2, Color.White);

            int volumeBarY = soundConfigY + 100;
            int volumeBarX = screenWidth / 6;
            int barWidth = 50;
            int barHeight = 40;
            int spacing = 10;
            int numBars = 5;

            DrawText("Background music volume", volumeBarX, volumeBarY - 40, 30, Color.White);

            for (int i = 0; i < numBars; i++)
            {
                int x = volumeBarX + i * (barWidth + spacing);
                Rectangle barRect = new Rectangle(x, volumeBarY, barWidth, barHeight);

                if (CheckCollisionPointRec(mousePos, barRect) && IsMouseButtonPressed(MouseButton.Left))
                {
                    gameManager.soundManager.MusicVolume = (i + 1) * 20;
                }

                if (gameManager.soundManager.MusicVolume >= (i + 1) * 20)
                {
                    DrawRectangle(x, volumeBarY, barWidth, barHeight, gameManager.isMusicMuted ? Color.Gray : Color.White);
                }
                else
                {
                    DrawRectangle(x, volumeBarY, barWidth, barHeight, Color.Gray);
                }
            }

            int effectsBarY = volumeBarY + barHeight + 60;
            DrawText("Sound effects volume", volumeBarX, effectsBarY - 40, 30, Color.White);

            for (int i = 0; i < numBars; i++)
            {
                int x = volumeBarX + i * (barWidth + spacing);
                Rectangle barRect = new Rectangle(x, effectsBarY, barWidth, barHeight);

                if (CheckCollisionPointRec(mousePos, barRect) && IsMouseButtonPressed(MouseButton.Left))
                {
                    gameManager.soundManager.SfxVolume = (i + 1) * 20;
                    gameManager.soundManager.UpdateSFX();
                    PlaySound(gameManager.soundManager.flickSound);
                }

                if (gameManager.soundManager.SfxVolume >= (i + 1) * 20)
                {
                    DrawRectangle(x, effectsBarY, barWidth, barHeight, gameManager.isSfxMuted ? Color.Gray : Color.White);
                }
                else
                {
                    DrawRectangle(x, effectsBarY, barWidth, barHeight, Color.Gray);
                }
            }

            DrawSoundButtons(gameManager);
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
                        MoveMouse(gameManager);
                    }
                    else if (gameManager.player2.TargetPortrait == null && gameManager.GetCurrentPlayer() == 2)
                    {
                        gameManager.SelectedPortrait(gameManager.player2, portraits[i].Clone());
                        gameManager.NextTurn();
                        MoveMouse(gameManager);
                    }
                    
                }

                renderer.DrawPortrait(portraits[i], x, y, size);
                
            }
        }
        public void MoveMouse(GameManager gameManager)
        {
            if (gameManager.GetCurrentPlayer() == 1)
            {
                SetMousePosition(200, 500);
            }
            else
            {
                SetMousePosition(GetScreenWidth() / 2 + 200, 500);
            }
        }

        public void DrawBackToMenuButton(GameManager gameManager, GameState lastState)
        {
            int btnX = 10;
            int btnY = 5;
            int btnWidth = 70;
            int btnHeight = 65;

            bool hover = CheckCollisionPointRec(GetMousePosition(), new Rectangle(btnX, btnY, btnWidth, btnHeight));

            Color color = hover ? trueYellow : Color.White;

            if (hover)
            {
                if (IsMouseButtonPressed(MouseButton.Left))
                {
                    gameManager.CurrentState = lastState;
                    PlaySound(gameManager.soundManager.flickSound);
                }
            }

            DrawRectangleLines(btnX, btnY, btnWidth, btnHeight, color);
            DrawText("<-", btnX + 10, btnY, 60, color);

        }

        public void DrawEndScreen(GameState state, int winner)
        {
            DrawRectangle(0, 0, 1280, 720, Color.Green);
            DrawText($"Player {winner} wins !", 400, 300, 40, Color.Black);
            DrawText("Press R to restart", 360, 360, 20, Color.Black);

        }

        private Rectangle GetButtonRect(int index)
        {
            float width = 300;
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

        void DrawSoundButtons(GameManager gameManager)
        {
            float scale = 0.5f;
            int speakerX;
            int speakerY;
            int sfxX;
            int sfxY;
            int screenWidth = GetScreenWidth();
            int screenHeight = GetScreenHeight();

            Vector2 mousePos = GetMousePosition();

            if (gameManager.CurrentState == GameState.Settings)
            {
                speakerX = 200;
                speakerY = screenHeight - 270;

                sfxX = 200;
                sfxY = screenHeight - 170;
            }
            else if (gameManager.CurrentState == GameState.Menu)
            {
                speakerX = screenWidth - 100;
                speakerY = 30;

                sfxX = screenWidth - 100;
                sfxY = 110;
            }
            else
            {
                bool isInGame = gameManager.CurrentState == GameState.InGame;
                bool isDualScreen = isInGame && gameManager.userHasDualScreen;
                bool isPlayerOne = gameManager.GetCurrentPlayer() == 1;

                if (isDualScreen)
                {
                    speakerX = isPlayerOne ? screenWidth / 2 - 100 : screenWidth - 100;
                    speakerY = 30;

                    sfxX = speakerX;
                    sfxY = 110;
                }
                else
                {
                    if (isPlayerOne)
                    {
                        speakerX = screenWidth / 2 - 100;
                        speakerY = 30;

                        sfxX = isInGame ? screenWidth / 2 - 180 : screenWidth - 100;
                        sfxY = 30;
                    }
                    else
                    {
                        speakerX = screenWidth - 100;
                        speakerY = 30;

                        sfxX = isInGame ? screenWidth - 180 : screenWidth - 100;
                        sfxY = isInGame ? 30 : 110;
                    }
                }
            }


            int speakerWidth = (int)(speakerIcon.Width * scale);
            int speakerHeight = (int)(speakerIcon.Height * scale);

            int sfxWidth = (int)(sfxIcon.Height * scale);
            int sfxHeight = (int)(sfxIcon.Height * scale);

            Vector2 speakerStart1 = new Vector2(speakerX, speakerY);
            Vector2 speakerEnd1 = new Vector2(speakerX + speakerWidth, speakerY + speakerHeight);
            Vector2 speakerStart2 = new Vector2(speakerX, speakerY + speakerHeight);
            Vector2 speakerEnd2 = new Vector2(speakerX + speakerWidth, speakerY);
            Rectangle speakerRect = new Rectangle(speakerX, speakerY, speakerWidth, speakerHeight);

            Vector2 sfxStart1 = new Vector2(sfxX, sfxY);
            Vector2 sfxEnd1 = new Vector2(sfxX + sfxWidth, sfxY + sfxHeight);
            Vector2 sfxStart2 = new Vector2(sfxX, sfxY + sfxHeight);
            Vector2 sfxEnd2 = new Vector2(sfxX + sfxWidth, sfxY);
            Rectangle sfxRect = new Rectangle(sfxX, sfxY, sfxWidth, sfxHeight);

            DrawTextureEx(speakerIcon, new Vector2(speakerX, speakerY), 0.0f, scale, Color.White);
            DrawTextureEx(sfxIcon, new Vector2(sfxX, sfxY), 0.0f, scale, Color.White);

            if (gameManager.isMusicMuted)
            {
                DrawLineEx(speakerStart1, speakerEnd1, 3.0f, Color.White);
                DrawLineEx(speakerStart2, speakerEnd2, 3.0f, Color.White);
            }

            if (gameManager.isSfxMuted)
            {
                DrawLineEx(sfxStart1, sfxEnd1, 3.0f, Color.White);
                DrawLineEx(sfxStart2, sfxEnd2, 3.0f, Color.White);
            }

            if (CheckCollisionPointRec(mousePos, speakerRect) && IsMouseButtonPressed(MouseButton.Left))
            {
                gameManager.isMusicMuted = !gameManager.isMusicMuted;
            }
            
            if (CheckCollisionPointRec(mousePos, sfxRect) && IsMouseButtonPressed(MouseButton.Left))
            {
                gameManager.isSfxMuted = !gameManager.isSfxMuted;
            }
        }


        void DrawTitle(GameManager gameManager, string textPlayer1, string? textPlayer2 = null, int yPosition = 30, int fontSize = 30, Color? color = null)
        {
            int screenWidth = GetScreenWidth();
            int halfScreenWidth = screenWidth / 2;
            Color finalColor = color ?? Color.White;

            if (string.IsNullOrEmpty(textPlayer2))
            {
                int textWidth = MeasureText(textPlayer1, fontSize);
                int positionX = (screenWidth - textWidth) / 2;
                DrawText(textPlayer1, positionX, yPosition, fontSize, finalColor);
            }
            else
            {
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
                        speakerIcon = LoadTexture("assets/icons/speaker.png");
                        sfxIcon = LoadTexture("assets/icons/sfx.png");
                        break;

                    case GameState.Generation:
                        backgroundMenu = LoadTexture("assets/backgrounds/MenuBackground.png");
                        break;

                    case GameState.Rules:
                        backgroundMenu = LoadTexture("assets/backgrounds/MenuBackground.png");
                        rules1Icon = LoadTexture("assets/icons/rules1.png");
                        rules2Icon = LoadTexture("assets/icons/rules2.png");
                        rules3Icon = LoadTexture("assets/icons/rules3.png");
                        break;

                    case GameState.Creating:
                        backgroundMenu = LoadTexture("assets/backgrounds/MenuBackground.png");
                        addCharacter = LoadTexture("assets/icons/addCharacter.png");
                        break;

                    case GameState.Settings:
                        backgroundMenu = LoadTexture("assets/backgrounds/MenuBackground.png");
                        screenIcon = LoadTexture("assets/icons/singlescreenicon.png");
                        speakerIcon = LoadTexture("assets/icons/speaker.png");
                        sfxIcon = LoadTexture("assets/icons/sfx.png");
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

                        screenIcon = LoadTexture("assets/icons/singlescreenicon.png");
                        speakerIcon = LoadTexture("assets/icons/speaker.png");
                        sfxIcon = LoadTexture("assets/icons/sfx.png");
                        break;
                }

                previousState = state;
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
