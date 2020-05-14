using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPEOS.Model;

namespace TPEOS.View
{
    static class Textures
    {
        public static Dictionary<BackgroundType, Bitmap> Background;
        public static Bitmap Player;
        public static Dictionary<CreatureType, Bitmap> CreaturesTextures;
        public static Dictionary<Drop, Bitmap> DropTextures;

        static Textures()
        {
            LoadBackgroundTextures();
            LoadPlayerTexture();
            LoadCreaturesTextures();
            LoadDropTextures();
        }

        private static void LoadBackgroundTextures()
        {
            var directory = new DirectoryInfo(@"Textures\Background");
            Background = new Dictionary<BackgroundType, Bitmap>();
            foreach (var e in directory.GetFiles("*.png"))
            {
                var name = e.Name.Substring(0, e.Name.Length - 4);
                Enum.TryParse<BackgroundType>(name, out var type);
                var texture = (Bitmap)Image.FromFile(e.FullName);
                //texture.SetResolution(64f, 64f);
                Background.Add(type, texture);
            }
        }

        private static void LoadPlayerTexture()
        {
            Player = (Bitmap) Image.FromFile(@"Textures\Player\Player.png");
            //Player.SetResolution(32f, 32f);
        }

        private static void LoadCreaturesTextures()
        {
            var directory = new DirectoryInfo(@"Textures\Creatures");
            CreaturesTextures = new Dictionary<CreatureType, Bitmap>();
            foreach (var e in directory.GetFiles("*.png"))
            {
                var name = e.Name.Substring(0, e.Name.Length - 4);
                Enum.TryParse<CreatureType>(name, out var type);
                var texture = (Bitmap)Image.FromFile(e.FullName);
                CreaturesTextures.Add(type, texture);
            }
        }

        private static void LoadDropTextures()
        {
            var directory = new DirectoryInfo(@"Textures\Drop");
            DropTextures = new Dictionary<Drop, Bitmap>();
            foreach (var e in directory.GetFiles("*.png"))
            {
                var name = e.Name.Substring(0, e.Name.Length - 4);
                Enum.TryParse<Drop>(name, out var type);
                var texture = (Bitmap)Image.FromFile(e.FullName);
                DropTextures.Add(type, texture);
            }
        }
    }
}
