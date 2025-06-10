using Raylib_cs;
using static Raylib_cs.Raylib;
using System.Numerics;

namespace GuessWho
{
    /// <summary>
    /// Gère le chargement, le rendu et la libération des textures associées à un portrait.
    /// </summary>
    public class PortraitRenderer
    {
        #region Champs privés

        /// <summary>
        /// Dictionnaire de textures chargées pour éviter les redondances.
        /// </summary>
        private readonly Dictionary<string, Texture2D> textures = new();

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Charge toutes les textures nécessaires à l'affichage d'un portrait.
        /// </summary>
        /// <param name="portrait">Portrait à préparer.</param>
        public void LoadPortraitTextures(Portrait portrait)
        {
            LoadValidTexture(portrait.Skin);
            LoadValidTexture(portrait.Clothes);
            LoadValidTexture(portrait.Logo);
            LoadValidTexture(portrait.Eyebrows);
            LoadValidTexture(portrait.Eyes);
            LoadValidTexture(portrait.Beard);
            LoadValidTexture(portrait.Glasses);
            LoadValidTexture(portrait.Hair);
            LoadValidTexture(portrait.Mouth);
            LoadValidTexture(portrait.Gender);
        }

        /// <summary>
        /// Dessine un portrait à l'écran à une position et une taille donnée.
        /// </summary>
        /// <param name="portrait">Le portrait à dessiner.</param>
        /// <param name="x">Coordonnée X sur l'écran.</param>
        /// <param name="y">Coordonnée Y sur l'écran.</param>
        /// <param name="size">Taille du portrait (largeur = hauteur).</param>
        public void DrawPortrait(Portrait portrait, int x, int y, int size)
        {
            string[] layers = {
                portrait.Skin,
                portrait.Clothes,
                portrait.Logo,
                portrait.Eyes,
                portrait.Eyebrows,
                portrait.Hair,
                portrait.Beard,
                portrait.Mouth,
                portrait.Glasses,
                portrait.Gender
            };

            foreach (string path in layers)
            {
                if (textures.TryGetValue(path, out Texture2D texture))
                {
                    Rectangle destination = new(x, y, size, size);

                    DrawTexturePro(
                        texture,
                        new Rectangle(0, 0, texture.Width, texture.Height),
                        destination,
                        Vector2.Zero,
                        0f,
                        portrait.IsEliminated ? new Color(0, 0, 0, 255) : Color.White
                    );
                }
            }
        }
        /// <summary>
        /// Génère et assigne une texture composite à un portrait (utilisé pour l'export ou cache).
        /// </summary>
        /// <param name="portrait">Le portrait à préparer.</param>
        /// <param name="size">La taille finale de la texture carrée.</param>
        public void GenerateAndAssignPortraitTexture(Portrait portrait, int size)
        {
            RenderTexture2D renderTex = LoadRenderTexture(size, size);
            BeginTextureMode(renderTex);
            ClearBackground(Color.Blank);

            string[] layers = {
                portrait.Skin,
                portrait.Clothes,
                portrait.Logo,
                portrait.Eyes,
                portrait.Eyebrows,
                portrait.Hair,
                portrait.Beard,
                portrait.Mouth,
                portrait.Glasses,
                portrait.Gender
            };

            foreach (string path in layers)
            {
                if (textures.TryGetValue(path, out Texture2D texture))
                {
                    DrawTexturePro(
                        texture,
                        new Rectangle(0, 0, texture.Width, texture.Height),
                        new Rectangle(0, 0, size, size),
                        Vector2.Zero,
                        0f,
                        Color.White
                    );
                }
            }

            EndTextureMode();

            Image img = LoadImageFromTexture(renderTex.Texture);
            Texture2D finalTex = LoadTextureFromImage(img);

            UnloadImage(img);
            UnloadRenderTexture(renderTex);

            // Libère l'ancienne texture si déjà existante
            if (portrait.portraitTexture.Id != 0)
            {
                UnloadTexture(portrait.portraitTexture);
            }

            portrait.portraitTexture = finalTex;
        }



        /// <summary>
        /// Libère toutes les textures chargées de la mémoire GPU.
        /// </summary>
        public void UnloadAll()
        {
            foreach (var tex in textures.Values)
            {
                UnloadTexture(tex);
            }

            textures.Clear();
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Charge une texture uniquement si le chemin est valide et qu'elle n'a pas déjà été chargée.
        /// </summary>
        /// <param name="path">Chemin d'accès à la texture.</param>
        private void LoadValidTexture(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                Console.WriteLine("Chemin de texture vide ou null.");
                return;
            }

            if (!File.Exists(path))
            {
                Console.WriteLine($"Fichier de texture introuvable : {path}");
                return;
            }

            if (!textures.ContainsKey(path))
            {
                Texture2D texture = LoadTexture(path);

                if (texture.Id == 0)
                {
                    Console.WriteLine($"Échec du chargement de la texture : {path}");
                    return;
                }

                SetTextureFilter(texture, TextureFilter.Bilinear);
                textures[path] = texture;
            }
        }

        #endregion
    }
}
