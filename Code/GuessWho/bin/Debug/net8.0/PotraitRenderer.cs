using Raylib_cs;
using static Raylib_cs.Raylib;
using System.Numerics;

namespace GuessWho
{
    public class PortraitRenderer
    {
        private Dictionary<string, Texture2D> textures = new();
        public void LoadPotraitTextures(Portrait portrait)
        {
            LoadPortraitSafe(portrait.Skin);
            LoadPortraitSafe(portrait.Clothes);
            LoadPortraitSafe(portrait.Logo);
            LoadPortraitSafe(portrait.Eyebrows);
            LoadPortraitSafe(portrait.Eyes);
            LoadPortraitSafe(portrait.Beard);
            LoadPortraitSafe(portrait.Glasses);
            LoadPortraitSafe(portrait.Hair);
            LoadPortraitSafe(portrait.Mouth);
            LoadPortraitSafe(portrait.Gender);
        }

        private void LoadPortraitSafe(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                Console.WriteLine("Chemin vide ou null !");
                return;
            }

            if (!File.Exists(path))
            {
                Console.WriteLine($"Fichier introuvable : {path}");
                return;
            }

            if (!textures.ContainsKey(path))
            {
                Texture2D texture = LoadTexture(path);
                if (texture.Id == 0)
                {
                    Console.WriteLine($"Erreur de chargement de texture : {path}");
                    return;
                }

                SetTextureFilter(texture, TextureFilter.Bilinear);
                textures[path] = texture;
            }
        }


        public void DrawPortrait(Portrait portrait, int x, int y, int size)
        {

            // Ordre des textures
            string[] layers = { portrait.Skin, portrait.Clothes, portrait.Logo, portrait.Eyes, portrait.Eyebrows, portrait.Hair, portrait.Beard, portrait.Mouth, portrait.Glasses, portrait.Gender };

            foreach (string path in layers)
            {
                if (textures.TryGetValue(path, out Texture2D texture))
                {
                    Rectangle gridDestination = new Rectangle(x, y, size, size);

                    DrawTexturePro(
                        texture,
                        new Rectangle(0, 0, texture.Width, texture.Height),
                        gridDestination,
                        Vector2.Zero,
                        0f,
                        portrait.IsEliminated ? new Color(0, 0, 0, 255) : Color.White
                    );
                }
            }
        }

        public void UnloadAll()
        {
            foreach (var tex in textures.Values)
            {
                   UnloadTexture(tex);
            }
            textures.Clear();
        }
    }

}
