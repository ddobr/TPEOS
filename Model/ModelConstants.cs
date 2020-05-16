using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPEOS.Model
{
    class ModelConstants
    {
        public const int PlayerHealth = 1000;
        public const int PlayerWalkSpeed = 2; //Возможно игрок будет без этого ограничения 
        public const int PlayerShootSpeed = 40;
        public const int InitialPlayerAmmo = 10;
        public const int InitialPlayerBlocks = 20;
        public const int PlayerHitPower = 1;

        public const int BulletHitPower = 2;
        public const int BulletSpeed = 6;
        public const int BulletHealth = 2;

        public const int BlockInitialHealth = 10;

        public const int WalkerInitialHealth = 4;
        public const int WalkerSpeed = 60;
        public const int WalkerHitPower = 6;
        public const int WalkerSpawnerCooldown = 400000; //6sec

        public const int ShooterInitialHealth = 2;
        public const int ShooterWalkSpeed = 30;
        public const int ShooterShootSpeed = 40;
        public const int ShooterSpawnerCooldown = 200000;
        public const int ShooterHitPower = 0;
    }
}
