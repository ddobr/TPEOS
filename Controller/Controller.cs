using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TPEOS.Model;
using Control = TPEOS.Model.Control;

namespace TPEOS.Controller
{
    static class Controller
    {
        private static readonly Dictionary<Keys, bool> HoldingKeys;
        private static readonly Dictionary<Keys, Model.Control> KeyToControl;

        public static void KeyPressed(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && Game.Stage != GameStage.Started && Game.Stage != GameStage.Finished)
                Game.Stage = Game.Stage == GameStage.Playing ? GameStage.Paused : GameStage.Playing;
            if (Game.Stage == GameStage.Started) Game.Stage = GameStage.Playing;
            if (Game.Stage == GameStage.Playing) InGameKeyPressed(e);
        }

        public static void InGameKeyPressed(KeyEventArgs e)
        {
            if (HoldingKeys.TryGetValue(e.KeyCode, out var isPressed) && !isPressed)
            {
                HoldingKeys[e.KeyCode] = true;
                Game.Controls.Enqueue(KeyToControl[e.KeyCode]);
            }
        }

        public static void KeyReleased(KeyEventArgs e)
        {
            if (HoldingKeys.TryGetValue(e.KeyCode, out _))
                HoldingKeys[e.KeyCode] = false;
        }

        static Controller()
        {
            HoldingKeys = new Dictionary<Keys, bool>
            {
                {Keys.W, false},
                {Keys.A, false},
                {Keys.S, false},
                {Keys.D, false},
                {Keys.Up, false},
                {Keys.Left, false},
                {Keys.Down, false},
                {Keys.Right, false},
                {Keys.Q, false}
            };

            KeyToControl = new Dictionary<Keys, Control>
            {
                {Keys.W, Control.GoUp},
                {Keys.A, Control.GoLeft},
                {Keys.S, Control.GoDown},
                {Keys.D, Control.GoRight},
                {Keys.Up, Control.ActionUp},
                {Keys.Left, Control.ActionLeft},
                {Keys.Down, Control.ActionDown},
                {Keys.Right, Control.ActionRight},
                {Keys.Q, Control.SwapAction}
            };
        }
    }
}
