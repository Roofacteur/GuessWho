using Raylib_cs;
using static Raylib_cs.Raylib;
using System.Numerics;
using static GuessWho.GameManager;

namespace GuessWho
{
    public class UIManager
    {
        public static UIManager Instance { get; } = new UIManager();
        private string[] menuLabels = { "Jouer", "Génération", "Options", "Quitter" };
        public void UpdateMenu(GameManager gameManager)
        {
            Vector2 mouse = GetMousePosition();
            DrawMenu();

            if (IsMouseButtonPressed(MouseButton.Left))
            {
                for (int i = 0; i < 4; i++)
                {
                    if (CheckCollisionPointRec(mouse, GetButtonRect(i)))
                    {
                        if (i == 0) gameManager.CurrentState = GameState.InGame;
                        if (i == 1) Console.WriteLine("Clic sur génération");
                        if (i == 2) Console.WriteLine("Clic sur options");
                        if (i == 3) Environment.Exit(0);
                    }
                }
            }
            
        }
        public void DrawMenu()
        {
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
            int startX = (int)(zone.X + (zone.Width / 2) - (gridWidth / 2)); // Centrer dans la zone du joueur

            // Rectangle bouton
            int btnX = 10;
            int btnY = 10;
            int btnWidth = 150;
            int btnHeight = 40;

            // Dessiner le rectangle du bouton
            DrawRectangleLines(btnX, btnY, btnWidth, btnHeight, Color.White);

            DrawText("Retour au Menu", btnX + 10, btnY + 10, 18, Color.White);

            if (CheckCollisionPointRec(GetMousePosition(), new Rectangle(btnX, btnY, btnWidth, btnHeight)))
            {
                DrawRectangle(btnX, btnY, btnWidth, btnHeight, new Color(255, 255, 255, 64));

                if (IsMouseButtonPressed(MouseButton.Left))
                {
                    gameManager.CurrentState = GameState.Menu; // Changer d'état vers le Menu
                }
            }

            const float hoverTarget = -8f;      // Valeur de décalage max
            const float hoverSpeed = 10f;       // Vitesse d'interpolation

            for (int i = 0; i < portraits.Length; i++)
            {
                int row = i / cols;
                int col = i % cols;
                int x = startX + col * (size + spacing);

                // Y de base sans hover
                int baseY = (int)(zone.Y + startY + row * (size + spacing));

                Rectangle rect = new(x, baseY, size, size);
                bool isHovered = gameManager.GetCurrentPlayer() == playerId && CheckCollisionPointRec(GetMousePosition(), rect);

                // Interpolation fluide vers la cible (hoverTarget ou 0)
                float targetOffset = isHovered ? hoverTarget : 0f;
                portraits[i].HoverOffset = Raymath.Lerp(portraits[i].HoverOffset, targetOffset, hoverSpeed * GetFrameTime());

                // Application de l'offset
                int y = (int)(baseY + portraits[i].HoverOffset);

                // Dessin
                if (!string.IsNullOrEmpty(portraits[i].Name))
                    DrawText(portraits[i].Name, x, y + size + 5, 25, Color.White);

                if (isHovered && IsMouseButtonPressed(MouseButton.Left))
                {
                    portraits[i].IsEliminated = !portraits[i].IsEliminated;
                }

                renderer.DrawPortraits(portraits[i], x, y, size);
            }

        }


        public void DrawEndScreen(GameState state, int winner)
        {
            DrawRectangle(0, 0, 1280, 720, Color.Green);
            DrawText($"Victoire du joueur {winner} !", 400, 300, 40, Color.Black);
            DrawText("Appuyez sur R pour recommencer", 360, 360, 20, Color.Black);

        }

        private Rectangle GetButtonRect(int index)
        {
            float width = 200;
            float height = 60;
            float spacing = 20;
            float totalHeight = (height + spacing) * 3 - spacing;
            float startY = (GetScreenHeight() - totalHeight) / 2;

            return new Rectangle(
                (GetScreenWidth() - width) / 2,
                startY + index * (height + spacing),
                width,
                height
            );
        }
    }

}
