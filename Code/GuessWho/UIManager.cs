using Raylib_cs;
using static Raylib_cs.Raylib;
using System.Numerics;
using static GuessWho.GameManager;

namespace GuessWho
{
    /// <summary>
    /// Gère toute l'interface utilisateur graphique du jeu, incluant les menus, écrans de jeu,
    /// affichage des portraits, titres, boutons de son, et transitions visuelles selon l'état du jeu.
    /// </summary>
    public class UIManager
    {
        #region Propriétés
        // === Ressources graphiques ===
        private static Texture2D backgroundMenu;
        private static Texture2D backgroundInGame;
        private static Texture2D backgroundInGameSelecting;
        private static Texture2D guessWhoTitle;
        private static Texture2D screenIcon;
        private static Texture2D addCharacter;
        private static Texture2D speakerIcon;
        private static Texture2D sfxIcon;
        private static Texture2D rules1Icon;
        private static Texture2D rules2Icon;
        private static Texture2D rules3Icon;

        // === Couleurs et typographies ===
        private readonly Color trueYellow = new(255, 196, 0);
        private readonly string[] menuLabels = { "PLAY", "RULES", "CHARACTERS", "SETTINGS", "QUIT" };

        // === Variables de mise en page ===
        private static int BasePortraitSize;
        private static int ExamplePortraitSize = 760;
        private static int cols;

        // === État de navigation ===
        private GameState previousState = GameState.None;
        private int page = 1;
        private readonly int lastPage = 3;

        // === Zone d’entrée textuelle ===
        private bool inputInitialized = false;
        private bool inputActive = false;
        private string inputText = "";
        private Rectangle inputBox = new(320, 210, 50, 40);

        // === Export ===
        private Rectangle exportButton = new Rectangle(50, GetScreenHeight() - 100, 180, 50); // Position du bouton
        private bool exportClicked = false;
        int portraitsExported = 0;

        #endregion

        #region Boucle du menu principal
        /// <summary>
        /// Met à jour le menu principal. Détecte les clics sur les boutons et change l'état du jeu en conséquence.
        /// </summary>
        public void UpdateMenu(GameManager gameManager)
        {
            Vector2 mouse = GetMousePosition();
            DrawMenu(gameManager);

            if (IsMouseButtonPressed(MouseButton.Left))
            {
                for (int i = 0; i < menuLabels.Length; i++)
                {
                    if (CheckCollisionPointRec(mouse, GetButtonRect(i)))
                    {
                        switch (i)
                        {
                            case 0: gameManager.CurrentState = GameState.InGame; break;
                            case 1: gameManager.CurrentState = GameState.Rules; break;
                            case 2: gameManager.CurrentState = GameState.Creating; break;
                            case 3: gameManager.CurrentState = GameState.Settings; break;
                            case 4: Environment.Exit(0); break;
                        }
                    }
                }
            }
        }
        #endregion

        #region Dessins d'interfaces

        /// <summary>
        /// Affiche les éléments visuels du menu principal (fond, titre, boutons).
        /// </summary>
        public void DrawMenu(GameManager gameManager)
        {
            DrawTexture(backgroundMenu, 0, 0, Color.White);
            DrawSoundButtons(gameManager);
            DrawTexture(guessWhoTitle, 0, 0, Color.White);

            for (int i = 0; i < menuLabels.Length; i++)
            {
                Rectangle btn = GetButtonRect(i);
                bool hover = CheckCollisionPointRec(GetMousePosition(), btn);
                Color color = hover ? trueYellow : Color.White;

                DrawRectangleLinesEx(btn, 2, color);

                int fontSize = 30;
                Vector2 textSize = MeasureTextEx(GetFontDefault(), menuLabels[i], fontSize, 1);
                Vector2 textPos = new(btn.X + (btn.Width - textSize.X) / 2, btn.Y + (btn.Height - textSize.Y) / 2);

                DrawText(menuLabels[i], (int)textPos.X, (int)textPos.Y, fontSize, color);
            }
        }

        /// <summary>
        /// Affiche l'écran de sélection des portraits, un pour chaque joueur.
        /// </summary>
        public void DrawSelectingPortraits(GameManager gameManager)
        {
            DrawTexture(backgroundInGameSelecting, 0, 0, Color.White);

            gameManager.player1.Zone = new Rectangle(0, 0, GetScreenWidth() / 2, GetScreenHeight());
            gameManager.player2.Zone = new Rectangle(GetScreenWidth() / 2, 0, GetScreenWidth() / 2, GetScreenHeight());

            cols = gameManager.userHasDualScreen ? 6 : 5;

            DrawPortraitGrid(gameManager.player1.Board.Portraits, gameManager.renderer, gameManager.player1.Zone, 100, cols, BasePortraitSize, 1, gameManager);
            DrawPortraitGrid(gameManager.player2.Board.Portraits, gameManager.renderer, gameManager.player2.Zone, 100, cols, BasePortraitSize, 2, gameManager);

            HideBoard(gameManager);

            string hiddenPlayer, displayedPlayer;
            if (gameManager.GetCurrentPlayer() == 1)
            {
                hiddenPlayer = "Player 1 is choosing a character!\nPlayer 2, don't peek!";
                displayedPlayer = "Player 1, choose a character!\nPlayer 2 will guess it.";
            }
            else
            {
                hiddenPlayer = "Player 2 is choosing a character!\nPlayer 1, don't peek!";
                displayedPlayer = "Player 2, choose a character!\nPlayer 1 will guess it.";
            }

            DrawTitle(gameManager,
                gameManager.GetCurrentPlayer() == 1 ? displayedPlayer : hiddenPlayer,
                gameManager.GetCurrentPlayer() == 1 ? hiddenPlayer : displayedPlayer);

            DrawSoundButtons(gameManager);
        }

        /// <summary>
        /// Affiche l'écran principal de jeu où les joueurs posent des questions et éliminent des portraits.
        /// </summary>
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

            cols = gameManager.userHasDualScreen ? 6 : 5;

            if (gameManager.userHasDualScreen)
            {
                int refSize = BasePortraitSize / 2;
                int namePosY = refSize / 4 + refSize;
                int portraitPosY = refSize / 4;

                if (gameManager.GetCurrentPlayer() == 1)
                {
                    DrawText(gameManager.player2.TargetPortrait.Name, GetScreenWidth() - refSize - refSize / 3, namePosY, 20, Color.White);
                    gameManager.renderer.DrawPortrait(gameManager.player2.TargetPortrait, GetScreenWidth() - refSize - refSize / 3, portraitPosY, refSize);
                }
                else
                {
                    DrawText(gameManager.player1.TargetPortrait.Name, GetScreenWidth() / 2 - refSize - refSize / 3, namePosY, 20, Color.White);
                    gameManager.renderer.DrawPortrait(gameManager.player1.TargetPortrait, GetScreenWidth() / 2 - refSize - refSize / 3, portraitPosY, refSize);
                }
            }

            int currentPlayer = gameManager.GetCurrentPlayer();
            int otherPlayer = currentPlayer == 1 ? 2 : 1;

            string hiddenPlayer = $"Player {currentPlayer} is asking a question!";
            string displayedPlayer = $"Ask player {otherPlayer} a question!";

            if (currentPlayer == 1)
                DrawTitle(gameManager, displayedPlayer, hiddenPlayer);
            else
                DrawTitle(gameManager, hiddenPlayer, displayedPlayer);
        }

        /// <summary>
        /// Affiche l'écran de génération de portraits.
        /// Permet à l'utilisateur de définir un paramètre de similarité génétique (1 à 10)
        /// et affiche la grille des portraits générés.
        /// </summary>
        public void DrawGeneration(GameManager gameManager)
        {

            DrawTexture(backgroundMenu, 0, 0, Color.White);

            if (!inputInitialized)
            {
                inputText = gameManager.userMaxAttributesInput.ToString();
                inputInitialized = true;
            }

            DrawTitle(gameManager, "PORTRAIT GENERATOR");
            DrawText("Press R !", GetScreenWidth() / 2 + 300, GetScreenHeight() - 100, 40, Color.White);

            // === Input génétique ===
            DrawText("Max similar genes in DNA :", 50, 220, 20, Color.White);
            DrawRectangleRec(inputBox, inputActive ? Color.SkyBlue : Color.LightGray);
            DrawRectangleLinesEx(inputBox, 1, Color.Black);
            DrawText(inputText, (int)inputBox.X + 5, (int)inputBox.Y + 5, 35, Color.Black);

            Vector2 mousePos = GetMousePosition();

            if (CheckCollisionPointRec(mousePos, inputBox) && IsMouseButtonPressed(MouseButton.Left))
            {
                inputActive = true;
                if (string.IsNullOrEmpty(inputText))
                    inputText = gameManager.userMaxAttributesInput.ToString();
            }
            else if (IsMouseButtonPressed(MouseButton.Left))
            {
                inputActive = false;
                UpdateUserMaxAttributes(gameManager);
            }

            if (inputActive)
            {
                int key = GetCharPressed();
                while (key > 0)
                {
                    if (char.IsDigit((char)key) && inputText.Length < 2)
                        inputText += (char)key;

                    key = GetCharPressed();
                }

                if (IsKeyPressed(KeyboardKey.Backspace) && inputText.Length > 0)
                    inputText = inputText[..^1];

                if (IsKeyPressed(KeyboardKey.Enter))
                {
                    UpdateUserMaxAttributes(gameManager);
                    inputActive = false;
                }
            }

            // === Affichage des portraits ===
            gameManager.player1.Zone = new Rectangle(
                GetScreenWidth() - 200,
                GetScreenHeight() / 6,
                GetScreenWidth() / 2,
                GetScreenHeight() / 2
            );

            var portraits = gameManager.player1.Board.Portraits;

            DrawPortraitGrid(
                portraits,
                gameManager.renderer,
                gameManager.player1.Zone,
                100,
                6,
                ExamplePortraitSize,
                1,
                gameManager
            );

            // === BOUTON EXPORT PNG ===
            DrawRectangleRec(exportButton, Color.DarkGreen);
            DrawText("Exporter PNG", (int)exportButton.X + 10, (int)exportButton.Y + 15, 20, Color.White);

            if (CheckCollisionPointRec(mousePos, exportButton) && IsMouseButtonPressed(MouseButton.Left))
            {
                portraitsExported++;

                if (portraits.Count() > 0)
                {
                    Portrait portraitToExport = portraits[0];

                    // Si la texture n'est pas encore générée, on la génère
                    if (portraitToExport.portraitTexture.Id == 0)
                    {
                        gameManager.renderer.GenerateAndAssignPortraitTexture(portraitToExport, 500);
                    }

                    // === Dossier d'export ===
                    string exportFolder = "exports";
                    if (!Directory.Exists(exportFolder))
                    {
                        Directory.CreateDirectory(exportFolder);
                    }

                    // === Export de l’image ===
                    Image img = LoadImageFromTexture(portraitToExport.portraitTexture);
                    ImageFlipVertical(ref img); // Corrige l'orientation
                    string exportPath = Path.Combine(exportFolder, $"portrait_{portraitsExported}_exported.png");
                    ExportImage(img, exportPath);
                    UnloadImage(img);
                }
            }




            // === Retour au menu ===
            DrawBackToMenuButton(gameManager, GameState.Creating);
        }


        /// <summary>
        /// Affiche l'écran des règles avec navigation entre les pages.
        /// </summary>
        public void DrawRules(GameManager gameManager)
        {
            int screenWidth = GetScreenWidth();
            int screenHeight = GetScreenHeight();

            DrawTexture(backgroundMenu, 0, 0, Color.White);
            DrawBackToMenuButton(gameManager, GameState.Menu);

            Vector2 mouse = GetMousePosition();

            Rectangle rightArrow = new(screenWidth / 2 + 50, screenHeight - 100, 50, 50);
            Rectangle leftArrow = new(screenWidth / 2 - 50, screenHeight - 100, 50, 50);

            // Flèche droite
            bool hoverRight = CheckCollisionPointRec(mouse, rightArrow);
            Color colorRight = (page == lastPage) ? Color.Gray : (hoverRight ? trueYellow : Color.White);
            DrawText("->", (int)rightArrow.X, (int)rightArrow.Y, 50, colorRight);
            if (hoverRight && IsMouseButtonPressed(MouseButton.Left) && page < lastPage) page++;

            // Flèche gauche
            bool hoverLeft = CheckCollisionPointRec(mouse, leftArrow);
            Color colorLeft = (page == 1) ? Color.Gray : (hoverLeft ? trueYellow : Color.White);
            DrawText("<-", (int)leftArrow.X, (int)leftArrow.Y, 50, colorLeft);
            if (hoverLeft && IsMouseButtonPressed(MouseButton.Left) && page > 1) page--;

            // Affichage du contenu selon la page active
            switch (page)
            {
                case 1:
                    DrawCenteredText("Stages", 30, Color.White);
                    DrawTexture(rules2Icon, 0, 0, Color.White);
                    break;
                case 2:
                    DrawCenteredText("Commands", 30, Color.White);
                    DrawTexture(rules1Icon, 0, 0, Color.White);
                    break;
                case 3:
                    DrawCenteredText("In-game rules", 30, Color.White);
                    DrawTexture(rules3Icon, 0, 0, Color.White);
                    break;
            }
        }

        /// <summary>
        /// Affiche un texte centré horizontalement à l'écran.
        /// </summary>
        private void DrawCenteredText(string text, int fontSize, Color color)
        {
            int screenWidth = GetScreenWidth();
            int textWidth = MeasureText(text, fontSize);
            DrawText(text, screenWidth / 2 - textWidth / 2, 100, fontSize, color);
        }

        /// <summary>
        /// Affiche l'écran de création de personnage.
        /// Permet d’accéder à la génération de portraits.
        /// </summary>
        public void DrawCreator(GameManager gameManager)
        {
            const string title = "YOUR CHARACTERS";
            const string genButtonText = "GENERATION";
            const int buttonWidth = 500, buttonHeight = 500;
            const int genButtonWidth = 200, genButtonHeight = 50;
            const int margin = 20, fontSize = 20;
            const float scale = 0.5f;

            DrawTexture(backgroundMenu, 0, 0, Color.White);
            DrawTitle(gameManager, title);
            DrawBackToMenuButton(gameManager, GameState.Menu);

            Vector2 mouse = GetMousePosition();

            // Affichage du bouton d'ajout de personnage
            float texW = addCharacter.Width * scale;
            float texH = addCharacter.Height * scale;
            Vector2 position = new(GetScreenWidth() / 2 - texW / 2, GetScreenHeight() / 2 - texH / 2);
            DrawTextureEx(addCharacter, position, 0f, scale, Color.White);

            Rectangle addButtonBounds = new(GetScreenWidth() / 2 - buttonWidth / 2, GetScreenHeight() / 2 - buttonHeight / 2, buttonWidth, buttonHeight);
            bool addHover = CheckCollisionPointRec(mouse, addButtonBounds);
            if (addHover && IsMouseButtonPressed(MouseButton.Left)) Console.WriteLine("Click");
            DrawRectangleRec(addButtonBounds, new Color(255, 255, 255, 0));

            // Affichage du bouton "Génération"
            Rectangle genButtonBounds = new(margin, GetScreenHeight() - genButtonHeight - margin, genButtonWidth, genButtonHeight);
            int textWidth = MeasureText(genButtonText, fontSize);
            int textX = (int)genButtonBounds.X + (genButtonWidth - textWidth) / 2;
            int textY = (int)genButtonBounds.Y + (genButtonHeight - fontSize) / 2;
            bool genHover = CheckCollisionPointRec(mouse, genButtonBounds);
            Color genColor = genHover ? trueYellow : Color.White;
            DrawText(genButtonText, textX, textY, fontSize, genColor);

            if (genHover && IsMouseButtonPressed(MouseButton.Left))
            {
                gameManager.CurrentState = GameState.Generation;
            }
        }


        /// <summary>
        /// Affiche l'écran des options du jeu, permettant de configurer l'affichage (écran simple ou double) 
        /// ainsi que le volume de la musique et des effets sonores.
        /// </summary>
        /// <param name="gameManager">Référence au GameManager pour accéder aux états et aux ressources sonores.</param>
        public void DrawOptions(GameManager gameManager)
        {
            // === Affichage général ===
            DrawTexture(backgroundMenu, 0, 0, Color.White);
            DrawBackToMenuButton(gameManager, GameState.Menu);
            DrawTitle(gameManager, "SETTINGS");

            int screenWidth = GetScreenWidth();
            Vector2 mousePos = GetMousePosition();


            int titleY = 130;
            int lineY = titleY + 40;
            int textY = 280;
            int textWidth = 230;
            int textHeight = 40;
            int iconWidth = 300;
            int iconY = textY + 50;

            int singleX = screenWidth / 4 - 100;
            int dualX = 3 * screenWidth / 4 - 100;

            Rectangle singleRect = new Rectangle(singleX, textY, textWidth, textHeight);
            Rectangle dualRect = new Rectangle(dualX, textY, textWidth, textHeight);

            DrawText("Screen configuration", screenWidth / 2 - MeasureText("Screen configuration", 30) / 2, titleY, 30, Color.White);
            DrawRectangle(120, lineY, screenWidth - 240, 2, Color.White);

            // Boutons de configuration écran
            DrawScreenOption(singleRect, "Single screen", singleX, textY, mousePos, () =>
            {
                gameManager.userHasDualScreen = false;
                PlaySound(gameManager.soundManager.flickSound);
            }, !gameManager.userHasDualScreen);

            DrawScreenOption(dualRect, "Dualscreen", dualX, textY, mousePos, () =>
            {
                gameManager.userHasDualScreen = true;
                PlaySound(gameManager.soundManager.flickSound);
            }, gameManager.userHasDualScreen);

            // Icônes de visualisation
            int singleIconX = screenWidth / 4 - iconWidth / 2;
            DrawTexture(screenIcon, singleIconX, iconY, Color.White);

            int dualStartX = screenWidth / 2 + (screenWidth / 2 - iconWidth * 2) / 2;
            DrawTexture(screenIcon, dualStartX, iconY, Color.White);
            DrawTexture(screenIcon, dualStartX + iconWidth, iconY, Color.White);

            int soundConfigY = iconY + iconWidth + 20;
            DrawText("Sound configuration", screenWidth / 2 - MeasureText("Sound configuration", 30) / 2, soundConfigY, 30, Color.White);
            DrawRectangle(120, lineY * 4 + 12, screenWidth - 240, 2, Color.White);

            int volumeBarY = soundConfigY + 100;
            int volumeBarX = screenWidth / 6;

            DrawVolumeBars("Background music volume", volumeBarX, volumeBarY, gameManager.soundManager.MusicVolume, gameManager.isMusicMuted, (value) =>
            {
                gameManager.soundManager.MusicVolume = value;
            });

            int effectsBarY = volumeBarY + 100;
            DrawVolumeBars("Sound effects volume", volumeBarX, effectsBarY, gameManager.soundManager.SfxVolume, gameManager.isSfxMuted, (value) =>
            {
                gameManager.soundManager.SfxVolume = value;
                gameManager.soundManager.UpdateSFX();
                PlaySound(gameManager.soundManager.flickSound);
            });

            DrawSoundButtons(gameManager);
        }

        /// <summary>
        /// Gère l’affichage et l’interaction d’une option écran (single/dual).
        /// </summary>
        private void DrawScreenOption(Rectangle rect, string label, int x, int y, Vector2 mousePos, Action onClick, bool isSelected)
        {
            bool isHovered = CheckCollisionPointRec(mousePos, rect);
            Color textColor = isHovered ? trueYellow : Color.White;

            DrawText(label, x, y, 30, textColor);

            if (isHovered && IsMouseButtonPressed(MouseButton.Left))
                onClick?.Invoke();

            if (isSelected)
                DrawText("Selected", x, y - 30, 30, Color.Green);
        }

        /// <summary>
        /// Affiche une ligne de barres interactives pour configurer un volume.
        /// </summary>
        private void DrawVolumeBars(string label, int startX, int y, int currentVolume, bool isMuted, Action<int> onVolumeChange)
        {
            const int barWidth = 50, barHeight = 40, spacing = 10, numBars = 5;

            DrawText(label, startX, y - 40, 30, Color.White);

            Vector2 mousePos = GetMousePosition();

            for (int i = 0; i < numBars; i++)
            {
                int x = startX + i * (barWidth + spacing);
                Rectangle rect = new(x, y, barWidth, barHeight);

                if (CheckCollisionPointRec(mousePos, rect) && IsMouseButtonPressed(MouseButton.Left))
                    onVolumeChange((i + 1) * 20);

                Color fill = currentVolume >= (i + 1) * 20 ? (isMuted ? Color.Gray : Color.White) : Color.Gray;
                DrawRectangle(x, y, barWidth, barHeight, fill);
            }
        }

        /// <summary>
        /// Affiche une grille de portraits sélectionnables ou éliminables avec effets visuels.
        /// </summary>
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

            // Configuration taille portrait selon type d'écran
            BasePortraitSize = gameManager.userHasDualScreen ? 380 : 300;

            Vector2 mousePos = GetMousePosition();

            for (int i = 0; i < portraits.Length; i++)
            {
                int row = i / cols;
                int col = i % cols;
                int x = startX + col * (size + spacing);
                int baseY = (int)(zone.Y + startY + row * (size + spacing));

                Rectangle rect = new(x, baseY, size, size);
                bool isHovered = gameManager.GetCurrentPlayer() == playerId &&
                                 CheckCollisionPointRec(mousePos, rect) &&
                                 gameManager.CurrentState == GameState.InGame;

                float targetOffset = isHovered ? -8f : 0f;
                portraits[i].HoverOffset = Raymath.Lerp(portraits[i].HoverOffset, targetOffset, 10f * GetFrameTime());

                int y = (int)(baseY + portraits[i].HoverOffset);

                if (!string.IsNullOrEmpty(portraits[i].Name))
                    DrawText(portraits[i].Name, x, y + size + 5, 25, Color.White);

                if (isHovered && IsMouseButtonPressed(MouseButton.Left))
                {
                    if (!gameManager.StateSelectingPortrait)
                    {
                        portraits[i].IsEliminated = !portraits[i].IsEliminated;
                    }
                    else
                    {
                        HandlePortraitSelection(gameManager, playerId, portraits[i]);
                    }
                }

                renderer.DrawPortrait(portraits[i], x, y, size);
            }
        }

        /// <summary>
        /// Affiche un bouton pour revenir au menu précédent.
        /// </summary>
        public void DrawBackToMenuButton(GameManager gameManager, GameState lastState)
        {
            int btnX = 10, btnY = 5, btnWidth = 70, btnHeight = 65;
            bool hover = CheckCollisionPointRec(GetMousePosition(), new Rectangle(btnX, btnY, btnWidth, btnHeight));
            Color color = hover ? trueYellow : Color.White;

            if (hover && IsMouseButtonPressed(MouseButton.Left))
            {
                gameManager.CurrentState = lastState;
                PlaySound(gameManager.soundManager.flickSound);
            }

            DrawRectangleLines(btnX, btnY, btnWidth, btnHeight, color);
            DrawText("<-", btnX + 10, btnY, 60, color);
        }

        /// <summary>
        /// Affiche les boutons de contrôle sonore (musique et SFX).
        /// Gère également leur activation/désactivation.
        /// </summary>
        private void DrawSoundButtons(GameManager gameManager)
        {
            float scale = 0.5f;
            int screenWidth = GetScreenWidth(), screenHeight = GetScreenHeight();
            Vector2 mousePos = GetMousePosition();

            int speakerX, speakerY, sfxX, sfxY;

            // Positionnement selon l'état du jeu
            if (gameManager.CurrentState == GameState.Settings)
            {
                speakerX = 200; speakerY = screenHeight - 270;
                sfxX = 200; sfxY = screenHeight - 170;
            }
            else if (gameManager.CurrentState == GameState.Menu)
            {
                speakerX = screenWidth - 100; speakerY = 30;
                sfxX = screenWidth - 100; sfxY = 110;
            }
            else
            {
                bool isInGame = gameManager.CurrentState == GameState.InGame;
                bool isDual = isInGame && gameManager.userHasDualScreen;
                bool isP1 = gameManager.GetCurrentPlayer() == 1;

                if (isDual)
                {
                    speakerX = isP1 ? screenWidth / 2 - 100 : screenWidth - 100;
                    speakerY = 30;
                    sfxX = speakerX;
                    sfxY = 110;
                }
                else
                {
                    speakerX = isP1 ? screenWidth / 2 - 100 : screenWidth - 100;
                    speakerY = 30;

                    sfxX = isInGame
                        ? (isP1 ? screenWidth / 2 - 180 : screenWidth - 180)
                        : screenWidth - 100;
                    sfxY = 30;
                }
            }

            // Dimensions des icônes
            int speakerWidth = (int)(speakerIcon.Width * scale);
            int speakerHeight = (int)(speakerIcon.Height * scale);
            int sfxWidth = (int)(sfxIcon.Width * scale);
            int sfxHeight = (int)(sfxIcon.Height * scale);

            Rectangle speakerRect = new Rectangle(speakerX, speakerY, speakerWidth, speakerHeight);
            Rectangle sfxRect = new Rectangle(sfxX, sfxY, sfxWidth, sfxHeight);

            DrawTextureEx(speakerIcon, new Vector2(speakerX, speakerY), 0.0f, scale, Color.White);
            DrawTextureEx(sfxIcon, new Vector2(sfxX, sfxY), 0.0f, scale, Color.White);

            // Affichage des lignes barrées si le son est désactivé
            if (gameManager.isMusicMuted)
            {
                DrawLineEx(new Vector2(speakerX, speakerY), new Vector2(speakerX + speakerWidth, speakerY + speakerHeight), 3.0f, Color.White);
                DrawLineEx(new Vector2(speakerX, speakerY + speakerHeight), new Vector2(speakerX + speakerWidth, speakerY), 3.0f, Color.White);
            }

            if (gameManager.isSfxMuted)
            {
                DrawLineEx(new Vector2(sfxX, sfxY), new Vector2(sfxX + sfxWidth, sfxY + sfxHeight), 3.0f, Color.White);
                DrawLineEx(new Vector2(sfxX, sfxY + sfxHeight), new Vector2(sfxX + sfxWidth, sfxY), 3.0f, Color.White);
            }

            // Interactions utilisateur
            if (CheckCollisionPointRec(mousePos, speakerRect) && IsMouseButtonPressed(MouseButton.Left))
                gameManager.isMusicMuted = !gameManager.isMusicMuted;

            if (CheckCollisionPointRec(mousePos, sfxRect) && IsMouseButtonPressed(MouseButton.Left))
                gameManager.isSfxMuted = !gameManager.isSfxMuted;
        }
        /// <summary>
        /// Dessine l'écran de victoire
        /// </summary>
        /// <param name="gameManager"></param>
        /// <param name="renderer"></param>
        public void DrawWinScreen(GameManager gameManager, PortraitRenderer renderer)
        {
            HideBoard(gameManager);

            int screenWidth = GetScreenWidth();
            int screenHeight = GetScreenHeight();
            int halfWidth = screenWidth / 2;
            int fontSize = 40;

            // Messages
            string messageP1 = gameManager.player1.IsTheWinner
                ? $"{gameManager.player1.Name} lost..."
                : $"{gameManager.player1.Name} won !!!";

            string messageP2 = gameManager.player2.IsTheWinner
                ? $"{gameManager.player2.Name} lost..."
                : $"{gameManager.player2.Name} won !!!";

            int textWidthP1 = MeasureText(messageP1, fontSize);
            int textXP1 = (halfWidth - textWidthP1) / 2;
            int textYP1 = screenHeight / 2 - fontSize;

            int textWidthP2 = MeasureText(messageP2, fontSize);
            int textXP2 = halfWidth + (halfWidth - textWidthP2) / 2;
            int textYP2 = screenHeight / 2 - fontSize;

            DrawText(messageP1, textXP1, textYP1, fontSize, trueYellow);
            DrawText(messageP2, textXP2, textYP2, fontSize, trueYellow);

            // Portraits cibles
            int portraitSize = 200;
            int spacingY = 40;

            // Player 1
            Portrait p1Target = gameManager.player1.TargetPortrait;
            if (p1Target != null)
            {
                int portraitX = (halfWidth - portraitSize) / 2;
                int portraitY = textYP1 - portraitSize - spacingY;
                renderer.DrawPortrait(p1Target, portraitX, portraitY, portraitSize);

                // Nom du portrait
                int nameTextSize = 25;
                int nameTextWidth = MeasureText(p1Target.Name, nameTextSize);
                int nameTextX = portraitX + (portraitSize - nameTextWidth) / 2;
                int nameTextY = portraitY + portraitSize + 5;

                DrawText(p1Target.Name, nameTextX, nameTextY, nameTextSize, Color.White);
            }

            // Player 2
            Portrait p2Target = gameManager.player2.TargetPortrait;
            if (p2Target != null)
            {
                int portraitX = halfWidth + (halfWidth - portraitSize) / 2;
                int portraitY = textYP2 - portraitSize - spacingY;
                renderer.DrawPortrait(p2Target, portraitX, portraitY, portraitSize);

                // Nom du portrait
                int nameTextSize = 25;
                int nameTextWidth = MeasureText(p2Target.Name, nameTextSize);
                int nameTextX = portraitX + (portraitSize - nameTextWidth) / 2;
                int nameTextY = portraitY + portraitSize + 5;

                DrawText(p2Target.Name, nameTextX, nameTextY, nameTextSize, Color.White);
            }


            string continueMessage = "Press Enter to go to menu, Press R to restart";
            int continueTextSize = 20;
            int continueTextWidth = MeasureText(continueMessage, continueTextSize);
            int continueTextY = screenHeight - 60;

            if (gameManager.userHasDualScreen)
            {
                // Affiche sur la moitié gauche
                int continueTextX_Left = (screenWidth / 4) - (continueTextWidth / 2);
                DrawText(continueMessage, continueTextX_Left, continueTextY, continueTextSize, Color.LightGray);

                // Affiche sur la moitié droite
                int continueTextX_Right = (3 * screenWidth / 4) - (continueTextWidth / 2);
                DrawText(continueMessage, continueTextX_Right, continueTextY, continueTextSize, Color.LightGray);
            }
            else
            {
                // Affiche centré sur tout l’écran
                int continueTextX = (screenWidth - continueTextWidth) / 2;
                DrawText(continueMessage, continueTextX, continueTextY, continueTextSize, Color.LightGray);
            }

            if (IsKeyPressed(KeyboardKey.Enter))
            {
                gameManager.CurrentState = GameState.Menu;
            }
            if(IsKeyPressed(KeyboardKey.R))
            {
                gameManager.CurrentState = GameState.InGame;
            }
        }

        /// <summary>
        /// Affiche un titre centré (1 ou 2 joueurs selon les paramètres).
        /// </summary>
        private void DrawTitle(GameManager gameManager, string textPlayer1, string? textPlayer2 = null, int yPosition = 30, int fontSize = 30, Color? color = null)
        {
            int screenWidth = GetScreenWidth();
            int halfScreenWidth = screenWidth / 2;
            Color finalColor = color ?? Color.White;

            if (string.IsNullOrEmpty(textPlayer2))
            {
                int textWidth = MeasureText(textPlayer1, fontSize);
                DrawText(textPlayer1, (screenWidth - textWidth) / 2, yPosition, fontSize, finalColor);
            }
            else
            {
                int textWidthP1 = MeasureText(textPlayer1, fontSize);
                int textWidthP2 = MeasureText(textPlayer2, fontSize);

                DrawText(textPlayer1, (halfScreenWidth - textWidthP1) / 2, yPosition, fontSize, finalColor);
                DrawText(textPlayer2, halfScreenWidth + (halfScreenWidth - textWidthP2) / 2, yPosition, fontSize, finalColor);
            }
        }
        #endregion

        #region Logique de selection et rendu UI

        /// <summary>
        /// Gère la sélection d’un portrait pour un joueur donné.
        /// </summary>
        private void HandlePortraitSelection(GameManager gameManager, int playerId, Portrait selectedPortrait)
        {
            if (playerId == 1 && gameManager.player1.TargetPortrait == null)
            {
                gameManager.SelectedPortrait(gameManager.player1, selectedPortrait.Clone());
                gameManager.NextTurn();
                MoveMouse(gameManager);
            }
            else if (playerId == 2 && gameManager.player2.TargetPortrait == null)
            {
                gameManager.SelectedPortrait(gameManager.player2, selectedPortrait.Clone());
                gameManager.NextTurn();
                MoveMouse(gameManager);
            }
        }

        /// <summary>
        /// Déplace la souris à la position initiale selon le joueur courant.
        /// </summary>
        public void MoveMouse(GameManager gameManager)
        {
            if (gameManager.GetCurrentPlayer() == 1)
                SetMousePosition(200, 500);
            else
                SetMousePosition(GetScreenWidth() / 2 + 200, 500);
        }
        /// <summary>
        /// Retourne le rectangle du bouton de menu à une position donnée (par index).
        /// </summary>
        private Rectangle GetButtonRect(int index)
        {
            float width = 300, height = 60, spacing = 20;
            float totalHeight = (height + spacing) * 3 - spacing;
            float startY = (GetScreenHeight() - totalHeight) / 2 + 70;

            return new Rectangle(
                (GetScreenWidth() - width) / 2,
                startY + index * (height + spacing),
                width,
                height
            );
        }
        /// <summary>
        /// Cache le plateau adverse selon le joueur courant.
        /// </summary>
        private void HideBoard(GameManager gameManager)
        {
            int screenWidth = GetScreenWidth();
            int screenHeight = GetScreenHeight();

            Rectangle hidden = gameManager.GetCurrentPlayer() == 1
                ? new Rectangle(screenWidth / 2, 0, screenWidth / 2, screenHeight)
                : new Rectangle(0, 0, screenWidth / 2, screenHeight);

            Color transparentBlack = new Color(0, 0, 0, 128);
            DrawRectangle((int)hidden.X, (int)hidden.Y, (int)hidden.Width, (int)hidden.Height, transparentBlack);

            if (gameManager.CurrentState == GameState.Victory) 
            {
                DrawRectangle(0, 0, screenWidth, screenHeight, transparentBlack);
            }
        }

        /// <summary>
        /// Met à jour la valeur de similarité des gènes selon l'entrée utilisateur.
        /// </summary>
        private void UpdateUserMaxAttributes(GameManager gameManager)
        {
            if (int.TryParse(inputText, out int value) && value > 1 && value <= 10)
            {
                gameManager.userMaxAttributesInput = value;
            }
            else
            {
                gameManager.userMaxAttributesInput = 4;
                inputText = "4";
            }
        }
        #endregion

        #region Chargement et dechargement
        /// <summary>
        /// Charge les textures nécessaires en fonction de l’état du jeu.
        /// </summary>
        public void TextureLoader(GameManager gamemanager)
        {
            GameState state = gamemanager.CurrentState;

            if (state == previousState) return;

            backgroundMenu = LoadTexture("assets/backgrounds/MenuBackground.png");

            switch (state)
            {
                case GameState.Menu:
                    guessWhoTitle = LoadTexture("assets/icons/GuessWhoTitle.png");
                    speakerIcon = LoadTexture("assets/icons/speaker.png");
                    sfxIcon = LoadTexture("assets/icons/sfx.png");
                    break;

                case GameState.Generation:
                    break;

                case GameState.Rules:
                    rules1Icon = LoadTexture("assets/icons/rules1.png");
                    rules2Icon = LoadTexture("assets/icons/rules2.png");
                    rules3Icon = LoadTexture("assets/icons/rules3.png");
                    break;

                case GameState.Creating:
                    addCharacter = LoadTexture("assets/icons/addCharacter.png");
                    break;

                case GameState.Settings:
                    screenIcon = LoadTexture("assets/icons/singlescreenicon.png");
                    speakerIcon = LoadTexture("assets/icons/speaker.png");
                    sfxIcon = LoadTexture("assets/icons/sfx.png");
                    break;

                case GameState.InGame:
                    bool isDual = gamemanager.userHasDualScreen;
                    backgroundInGameSelecting = LoadTexture(isDual ? "assets/backgrounds/GameBackground.png" : "assets/backgrounds/GameSmallBackground.png");
                    backgroundInGame = LoadTexture(isDual ? "assets/backgrounds/GameBackgroundInverted.png" : "assets/backgrounds/GameSmallBackgroundInverted.png");
                    screenIcon = LoadTexture("assets/icons/singlescreenicon.png");
                    speakerIcon = LoadTexture("assets/icons/speaker.png");
                    sfxIcon = LoadTexture("assets/icons/sfx.png");
                    break;
            }

            previousState = state;
        }

        /// <summary>
        /// Décharge toutes les textures chargées pour libérer la mémoire.
        /// </summary>
        public void UnloadAll()
        {
            void Unload(ref Texture2D texture)
            {
                if (texture.Id > 0)
                {
                    UnloadTexture(texture);
                    texture = new Texture2D();
                }
            }

            Unload(ref backgroundMenu);
            Unload(ref backgroundInGame);
            Unload(ref backgroundInGameSelecting);
            Unload(ref guessWhoTitle);
        }
        #endregion
    }
}
