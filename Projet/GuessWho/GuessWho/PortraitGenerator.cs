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
            var characters = new List<Character>();
            var usedCombinations = new HashSet<string>();

            foreach (var name in names)
            {
                Character character;
                do
                {
                    character = GenerateRandomCharacter(name);
                }
                while (!usedCombinations.Add(GenerateCombinationKey(character)));

                character.PortraitTexture = ComposeTexture(character);
                characters.Add(character);
            }

            return characters;
        }

        public static Character GenerateRandomCharacter(string name)
        {
            var rng = new Random();

            return new Character
            {
                Name = name,
                Skin = RandomFrom("assets\\portrait\\base\\"),
                Hair = RandomFrom("assets\\portrait\\hair\\"),
                Eyes = RandomFrom("assets\\portrait\\eyes\\"),
                Eyebrows = RandomFrom("assets\\portrait\\eyebrows\\"),
                Beard = RandomFrom("assets\\portrait\\beard\\"),
                Mouth = RandomFrom("assets\\portrait\\mouth\\"),
                Clothes = RandomFrom("assets\\portrait\\clothes\\"),
                Logo = RandomFrom("assets\\portrait\\clothes\\logos\\"),
                Glasses = RandomFrom("assets\\portrait\\glasses\\")
            };
        }

        private static string GenerateCombinationKey(Character c)
        {
            return $"{c.Skin}|{c.Hair}|{c.Eyes}|{c.Eyebrows}|{c.Beard}|{c.Mouth}|{c.Clothes}|{c.Logo}|{c.Glasses}";
        }


        private static string RandomFrom(string folder)
        {
            var files = Directory.GetFiles(folder, "*.png");
            return files.Length > 0 ? files[new Random().Next(files.Length)] : "";
        }

        public static Texture2D ComposeTexture(Character c)
        {
            
            return new Texture2D(); // Placeholder
        }
    }

}
