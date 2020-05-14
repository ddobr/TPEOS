using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace TPEOS.Model
{
    [TestFixture]
    [SuppressMessage("ReSharper", "CommentTypo")]
    public class Tests
    {
        [Test]
        public void PlaceBlockInFrontOfPlayer()
        {
            Game.Field = new Field(4) {CreaturesMap = new Creature[4, 4], Player = new Player(new Point(2, 2))};
            Game.Field.Player.Act(Control.SwapAction);
            Assert.That(Game.Field.Player.DoesHoldWeapon == false);
            var command = Game.Field.Player.Act(Control.ActionUp);
            Game.ProcessPlayersMove(Game.Field.Player, command);
            Assert.That(Game.Field.CreaturesMap[2,1] is Block);
        }

        [Test]
        public void PlayerShootsBack()
        {
            Game.Field = new Field(4) {CreaturesMap = new Creature[4, 4], Player = new Player(new Point(2, 1))};
            var command = Game.Field.Player.Act(Control.ActionDown);
            var bullet = command.Spawn;
            Game.ProcessPlayersMove(Game.Field.Player, command);
            for (var i = 0; i < ModelConstants.BulletSpeed; i++)
            {
                bullet.Tick(command, EventArgs.Empty);
                command = bullet.Act();
                Game.ProcessMove(bullet, command);
            }
            Assert.That(Game.Field.CreaturesMap[2, 2] is Bullet);
        }

        [Test]
        public void BulletHitsWall()
        {
            Game.Field = new Field(5) { CreaturesMap = new Creature[5, 5], Player = new Player(new Point(2, 3)) };
            Game.Field.CreaturesMap[2, 1] = new Block(new Point(2, 1));

            var command = Game.Field.Player.Act(Control.ActionUp);
            var bullet = command.Spawn;
            Game.ProcessPlayersMove(Game.Field.Player, command);

            for (var i = 0; i <= ModelConstants.BulletSpeed; i++)
                bullet.Tick(command, EventArgs.Empty);
            
            command = bullet.Act();
            Game.ProcessMove(bullet, command);
            Assert.That(Game.Field.CreaturesMap[2, 2] is Bullet);

            for (var i = 0; i <= ModelConstants.BulletSpeed; i++)
                bullet.Tick(command, EventArgs.Empty);

            command = bullet.Act();
            Assert.That(bullet.Health == 0);
            Assert.That(command.Hit.HitPower == ModelConstants.BulletHitPower);
            Assert.That(command.Hit.Sender == bullet); //именно ссылка!
            Assert.That(command.Hit.Direction == new Point(0, -1));
            Game.ProcessMove(bullet, command);
            Assert.That(Game.Field.CreaturesMap[2, 1].Health ==
                        ModelConstants.BlockInitialHealth - ModelConstants.BulletHitPower);
        }

        [Test]
        public void WalkerDijkstraFinder()
        {
            Game.Field = new Field(6) { CreaturesMap = new Creature[6, 6] };
            Game.Field.CreaturesMap[3, 2] = new Block(Point.Empty);
            Game.Field.CreaturesMap[2, 2] = new Block(Point.Empty);
            Game.Field.CreaturesMap[1, 2] = new Block(Point.Empty);
            Game.Field.CreaturesMap[1, 3] = new Block(Point.Empty);
            Game.Field.CreaturesMap[1, 4] = new Block(Point.Empty);
            Game.Field.CreaturesMap[3, 4] = new Block(Point.Empty);
            Game.Field.CreaturesMap[3, 1] = new Block(Point.Empty);
            Game.Field.CreaturesMap[3, 0] = new Block(Point.Empty);
            Game.Field.CreaturesMap[0, 2] = new Block(Point.Empty);
            /*
             *OOOXOO
             *O2OXOO
             *XXXXOO
             *OXOO1O
             *OXTXOO
             *OOOOOO
             */
           Assert.That(WalkerDijkstraAlgorithm.TryGetPathsDelta(new Point(4, 3), new Point(2, 4), out var _));
           Assert.That(WalkerDijkstraAlgorithm.TryGetPathsDelta(new Point(1, 1), new Point(2, 4), out var _)); //находит путь, ломая стены))
        }

        [Test]
        public void ShooterDodgesSimpleSituation()
        {
            Game.Field = new Field(6) { CreaturesMap = new Creature[6, 6] };
            Game.ProcessSpawn(new Bullet(new Point(2, 0), new Point(0, 1)));
            var shooter = new Shooter(new Point(2, 3));
            shooter.Tick(null, EventArgs.Empty);
            shooter.Act();
            Assert.That(shooter.Location == new Point(1, 3) || shooter.Location == new Point(3, 3));
        }

        [Test]
        public void ShooterDodgesBlockSituation()
        {
            Game.Field = new Field(6) { CreaturesMap = new Creature[6, 6] };
            Game.ProcessSpawn(new Bullet(new Point(2, 0), new Point(0, 1)));
            Game.ProcessSpawn(new Block(new Point(3, 3)));
            var shooter = new Shooter(new Point(2, 3));
            shooter.Tick(null, EventArgs.Empty);
            shooter.Act();
            Assert.That(shooter.Location == new Point(1, 3));
        }

        [Test]
        public void ShooterDoesntDodgeTwoBlocksSituation()
        {
            Game.Field = new Field(6) { CreaturesMap = new Creature[6, 6] };
            Game.ProcessSpawn(new Bullet(new Point(2, 0), new Point(0, 1)));
            Game.ProcessSpawn(new Block(new Point(3, 3)));
            Game.ProcessSpawn(new Block(new Point(1, 3)));
            var shooter = new Shooter(new Point(2, 3));
            shooter.Tick(null, EventArgs.Empty);
            shooter.Act();
            Assert.That(shooter.Location == new Point(2, 3));
        }

        [Test]
        public void ShooterDoesntDodgeWrongBulletDirection()
        {
            Game.Field = new Field(6) { CreaturesMap = new Creature[6, 6] };
            Game.ProcessSpawn(new Bullet(new Point(2, 0), new Point(1, 0)));
            var shooter = new Shooter(new Point(2, 3));
            shooter.Tick(null, EventArgs.Empty);
            shooter.Act();
            Assert.That(shooter.Location == new Point(2, 3));
        }

        [Test]
        public void ShooterShootsAtPlayer()
        {
            Game.Field = new Field(5) { CreaturesMap = new Creature[5, 5] };
            var shooter = new Shooter(new Point(2, 3));
            Game.ProcessSpawn(shooter);
            var command = shooter.Act();
            Assert.That(command.Spawn.Location == new Point(2, 3) && ((Bullet)command.Spawn).Direction == new Point(0, -1));
        }

        [Test]
        public void GetUnitVectorTest()
        {
            Assert.That(Shooter.GetUnitVector(new Point(5, 0)) == new Point(1, 0));
            Assert.That(Shooter.GetUnitVector(new Point(-9, 0)) == new Point(-1, 0));
            Assert.That(Shooter.GetUnitVector(new Point(0, 10)) == new Point(0, 1));
            Assert.That(Shooter.GetUnitVector(new Point(0, -6)) == new Point(0, -1));
        }

        [Test]
        public void GetShootPositionsTest()
        {
            Game.Field = new Field(5) { CreaturesMap = new Creature[5, 5] };
            var shooter = new Shooter(new Point(0, 0));
            Game.ProcessSpawn(shooter);
            Game.ProcessSpawn(new Block(new Point(2, 1)));
            Game.ProcessSpawn(new Block(new Point(4, 2)));
            var positions = shooter.GetShootPositions(new Point(2, 2), 3); //При длине 3х - выход за карту
            Assert.That(positions.Length == 3);
            Assert.That(positions.Contains(new Point(3, 2)));
            Assert.That(positions.Contains(new Point(2, 4)));
            Assert.That(positions.Contains(new Point(0, 2)));
        }
    }
}
