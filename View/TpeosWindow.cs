using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TPEOS.Model;

namespace TPEOS.View
{
    class TpeosWindow : Form
    {
        private readonly GameState gameState;
        public int TickCount;
        public static Timer Timer = new Timer { Interval = 10 };

        public TpeosWindow()
        {
            gameState = new GameState();
            ClientSize = new Size(
                32 * Field.Size,
                32 * Field.Size + 64);
            
            Game.StartGame();
            Timer.Tick += TimerTick;
            Timer.Tick += Game.Field.Player.Tick;
            Timer.Tick += gameState.GameTick;
            Timer.Start();
            
            //Game.ProcessSpawn(new Walker(new Point(1, 1)));
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            FormBorderStyle = FormBorderStyle.FixedDialog;
            //FormBorderStyle = FormBorderStyle.None;
            //WindowState = FormWindowState.Maximized;
            Text = @"Two Punches Equals One Shot";
            DoubleBuffered = true;
            MaximizeBox = false;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            Controller.Controller.KeyPressed(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            Controller.Controller.KeyReleased(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.TranslateTransform(0, 64);
            e.Graphics.FillRectangle(Brushes.Black, 0, 0, 32 * Field.Size, 32 * Field.Size);
            for (var x = 0; x < Field.Size; x++)
            for (int y = 0; y < Field.Size; y++)
            {
                if (x == 0 || y == 0 || x == Field.Size - 1 || y == Field.Size - 1)
                    e.Graphics.DrawImage(Textures.Background[BackgroundType.Border], new Point(x * 32, y * 32));
                else
                    e.Graphics.DrawImage(Textures.Background[BackgroundType.Tile], new Point(x * 32, y * 32));
                e.Graphics.DrawImage(Textures.DropTextures[Game.Field.DropsMap[x, y]], new Point(x * 32, y * 32));
            }



            foreach (var creature in Game.Field.CreaturesList)
            {
                if (creature is Bullet)
                    e.Graphics.DrawImage(Textures.CreaturesTextures[CreatureType.Bullet],
                        new Point(creature.Location.X * 32, creature.Location.Y * 32));
                else if (creature is Walker)
                    e.Graphics.DrawImage(Textures.CreaturesTextures[CreatureType.Walker],
                        new Point(creature.Location.X * 32, creature.Location.Y * 32));
                else if (creature is Block)
                    e.Graphics.DrawImage(Textures.CreaturesTextures[CreatureType.Block],
                        new Point(creature.Location.X * 32, creature.Location.Y * 32));
                else if (creature is Shooter)
                {
                    e.Graphics.DrawImage(Textures.CreaturesTextures[CreatureType.Shooter],
                        new Point(creature.Location.X * 32, creature.Location.Y * 32));
                }
                
            }

            //TODO вращение сущностей вправо-влево
            //e.Graphics.TranslateTransform(Game.Field.Player.Location.X * 32 + 16, Game.Field.Player.Location.Y * 32 + 16);
            //e.Graphics.RotateTransform(TickCount * 10);
            //e.Graphics.DrawImage(Textures.Player,
            //    new Point(-16, -16));
            e.Graphics.DrawImage(Textures.Player,
                    new Point(Game.Field.Player.Location.X * 32, Game.Field.Player.Location.Y * 32));


            e.Graphics.ResetTransform();
            e.Graphics.DrawString(
                Game.Field.Player.Health + "  " + Game.Field.Player.BlocksAmount + "  " + Game.Field.Player.Ammo,
                new Font("Arial", 16), Brushes.Green, 0, 0);
        }

        private void TimerTick(object sender, EventArgs args)
        {
            //if (TickCount == 0) gameState.ProcessActs();
            TickCount++;
            if (TickCount == 4) TickCount = 0;
            Invalidate();
        }
    }
}
