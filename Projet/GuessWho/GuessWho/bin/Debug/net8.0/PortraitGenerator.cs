using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessWho
{
    public static class PortraitGenerator
    {
        private static HashSet<string> UsedCombinations = new();

        public static List<Character> GenerateCharacters(List<string> names)
        {
            List<Character> characters = new();
            foreach (var name in names)
            {
                Character c;
                do
                {
                    c = GenerateRandomCharacter(name);
                } while (!UsedCombinations.Add(c.Hair + c.EyeColor + c.Beard + c.Mouth + c.Clothes + c.Glasses + c.Hat));

                c.PortraitTexture = ComposeTexture(c);
                characters.Add(c);
            }
            return characters;
        }

        public static Character GenerateRandomCharacter(string name)
        {
            Random rng = new();
            return new Character
            {
                Name = name,
                Gender = rng.Next(2) == 0 ? "Male" : "Female",
                Hair = RandomFrom("assets/portraits/hair/"),
                EyeColor = RandomFrom("assets/portraits/eyes/"),
                Beard = RandomFrom("assets/portraits/beard/"),
                Mouth = RandomFrom("assets/portraits/mouth/"),
                Clothes = RandomFrom("assets/portraits/clothes/"),
                Glasses = RandomFrom("assets/portraits/glasses/"),
                Hat = RandomFrom("assets/portraits/hat/")
            };
        }

        private static string RandomFrom(string folder)
        {
            var files = Directory.GetFiles(folder, "*.svg");
            return files.Length > 0 ? files[new Random().Next(files.Length)] : "";
        }

        public static Texture2D ComposeTexture(Character c)
        {
            // Convertir et superposer les SVG ici selon ton moteur
            return new Texture2D(); // Placeholder
        }
    }

}
