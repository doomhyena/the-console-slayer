using System;
using System.Collections.Generic;
using System.Threading;

namespace the_console_slayer
{
    internal class Program
    {
        class Position
        {
            public int X { get; set; }
            public int Y { get; set; }

            public Position(int x, int y) { X = x; Y = y; }

            public static Position Add(Position p1, Position p2) =>
                new Position(p1.X + p2.X, p1.Y + p2.Y);

            public static double Distance(Position p1, Position p2) =>
                Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        class ConsoleSprite
        {
            public ConsoleColor Background { get; }
            public ConsoleColor Foreground { get; }
            public char Glyph { get; }

            public ConsoleSprite(ConsoleColor bg, ConsoleColor fg, char glyph)
            {
                Background = bg; Foreground = fg; Glyph = glyph;
            }
        }

        enum ItemType { Ammo, BFGCell, Door, LevelExit, Medikit, ToxicWaste, Wall }

        class GameItem
        {
            public Position Position { get; }
            public ConsoleSprite Sprite { get; private set; }
            public ItemType Type { get; }
            public double FillingRatio { get; private set; }
            public bool Available { get; private set; } = true;

            public GameItem(int x, int y, ItemType type)
            {
                Position = new Position(x, y);
                Type = type;
                SetInitialProperties();
            }

            private void SetInitialProperties()
            {
                switch (Type)
                {
                    case ItemType.Ammo:
                        Sprite = new ConsoleSprite(ConsoleColor.Black, ConsoleColor.Yellow, 'A');
                        FillingRatio = 0.0; break;
                    case ItemType.BFGCell:
                        Sprite = new ConsoleSprite(ConsoleColor.Black, ConsoleColor.Green, 'B');
                        FillingRatio = 0.0; break;
                    case ItemType.Door:
                        Sprite = new ConsoleSprite(ConsoleColor.DarkYellow, ConsoleColor.White, '/');
                        FillingRatio = 1.0; break;
                    case ItemType.LevelExit:
                        Sprite = new ConsoleSprite(ConsoleColor.Black, ConsoleColor.Cyan, 'E');
                        FillingRatio = 1.0; break;
                    case ItemType.Medikit:
                        Sprite = new ConsoleSprite(ConsoleColor.Black, ConsoleColor.Red, '+');
                        FillingRatio = 0.0; break;
                    case ItemType.ToxicWaste:
                        Sprite = new ConsoleSprite(ConsoleColor.Black, ConsoleColor.DarkGreen, ':');
                        FillingRatio = 0.0; break;
                    case ItemType.Wall:
                        Sprite = new ConsoleSprite(ConsoleColor.DarkGray, ConsoleColor.Gray, '#');
                        FillingRatio = 1.0; break;
                }
            }

            public void Interact()
            {
                if (Type == ItemType.Ammo || Type == ItemType.BFGCell || Type == ItemType.Medikit)
                    Available = false;
                else if (Type == ItemType.Door)
                {
                    if (FillingRatio == 1.0)
                    {
                        FillingRatio = 0.0;
                        Sprite = new ConsoleSprite(ConsoleColor.Black, ConsoleColor.DarkYellow, '/');
                    }
                    else
                    {
                        FillingRatio = 1.0;
                        Sprite = new ConsoleSprite(ConsoleColor.DarkYellow, ConsoleColor.White, '/');
                    }
                }
            }
        }
        class Player
        {
            public Position Position { get; set; }
            public ConsoleSprite Sprite { get; }
            public double FillingRatio { get; } = 0.4;
            public int Health { get; private set; } = 100;
            public int MaxHealth { get; } = 100;
            public int Ammo { get; private set; } = 20;
            public int MaxAmmo { get; } = 50;
            public int BFGCells { get; private set; } = 0;
            public int MaxBFGCells { get; } = 3;
            public bool Alive { get; private set; } = true;
            public int SightRange { get; } = 8;
            public int Score { get; private set; } = 0;

            public Player(int x, int y)
            {
                Position = new Position(x, y);
                Sprite = new ConsoleSprite(ConsoleColor.Black, ConsoleColor.Green, '@');
            }

            public void Shoot() { if (Ammo > 0) Ammo--; }
            public void ShootBFG() { if (BFGCells > 0) BFGCells--; }
            public void PickUpAmmo(int amount) => Ammo = Math.Min(Ammo + amount, MaxAmmo);
            public void PickUpHealth(int amount) => Health = Math.Min(Health + amount, MaxHealth);
            public void PickUpBFGCell() { if (BFGCells < MaxBFGCells) BFGCells++; }
            public void AddCombatPoints(int pts) => Score += pts;
            public void TakeDamage(int dmg)
            {
                Health = Math.Max(0, Health - dmg);
                if (Health == 0) Alive = false;
            }
        }

        enum DemonType { Zombieman, Imp, Mancubus }

        class Demon
        {
            public Position Position { get; set; }
            public ConsoleSprite Sprite { get; private set; }
            public DemonType Type { get; }
            public double FillingRatio { get; private set; }
            public int Health { get; set; }
            public bool Alive { get; private set; } = true;
            public int SightRange { get; private set; }
            public int AttackRange { get; private set; }
            public int Speed { get; private set; }
            public int DmgMin { get; private set; }
            public int DmgMax { get; private set; }

            public Demon(int x, int y, DemonType type)
            {
                Position = new Position(x, y);
                Type = type;
                SetInitialProperties();
            }

            private void SetInitialProperties()
            {
                switch (Type)
                {
                    case DemonType.Zombieman:
                        Sprite = new ConsoleSprite(ConsoleColor.Black, ConsoleColor.DarkRed, 'z');
                        FillingRatio = 0.4; Health = 20; SightRange = 3; AttackRange = 1;
                        Speed = 70; DmgMin = 3; DmgMax = 15; break;
                    case DemonType.Imp:
                        Sprite = new ConsoleSprite(ConsoleColor.Black, ConsoleColor.Red, 'i');
                        FillingRatio = 0.4; Health = 60; SightRange = 5; AttackRange = 2;
                        Speed = 50; DmgMin = 3; DmgMax = 24; break;
                    case DemonType.Mancubus:
                        Sprite = new ConsoleSprite(ConsoleColor.Black, ConsoleColor.DarkMagenta, 'M');
                        FillingRatio = 0.6; Health = 600; SightRange = 7; AttackRange = 3;
                        Speed = 25; DmgMin = 10; DmgMax = 80; break;
                }
            }

            public void TakeDamage(int dmg)
            {
                Health = Math.Max(0, Health - dmg);
                if (Health == 0) Alive = false;
            }
        }
        class Game
        {
            public Player Player { get; private set; }
            public List<GameItem> Items { get; private set; } = new List<GameItem>();
            public List<Demon> Demons { get; private set; } = new List<Demon>();
            public bool Exited { get; set; } = false;

            private Random rng = new Random();

            public Game() { Player = new Player(1, 1); }

            private void RenderSingleSprite(Position pos, ConsoleSprite sprite)
            {
                if (pos.X < 0 || pos.Y < 0 ||
                    pos.X >= Console.WindowWidth || pos.Y >= Console.WindowHeight - 2) return;
                Console.SetCursorPosition(pos.X, pos.Y);
                Console.BackgroundColor = sprite.Background;
                Console.ForegroundColor = sprite.Foreground;
                Console.Write(sprite.Glyph);
            }

            private void RenderGame()
            {
                Console.CursorVisible = false;
                Console.ResetColor();
                Console.Clear();

                foreach (var item in Items)
                    if (item.Available &&
                        Position.Distance(item.Position, Player.Position) <= Player.SightRange)
                        RenderSingleSprite(item.Position, item.Sprite);

                foreach (var d in Demons)
                    if (d.Alive &&
                        Position.Distance(d.Position, Player.Position) <= Player.SightRange)
                        RenderSingleSprite(d.Position, d.Sprite);

                RenderSingleSprite(Player.Position, Player.Sprite);
                RenderHUD();
            }

            private void RenderHUD()
            {
                int row = Console.WindowHeight - 1;
                Console.SetCursorPosition(0, row);
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.ForegroundColor = ConsoleColor.White;
                string hud = $" HP:{Player.Health}/{Player.MaxHealth}  " +
                             $"Ammo:{Player.Ammo}/{Player.MaxAmmo}  " +
                             $"BFG:{Player.BFGCells}/{Player.MaxBFGCells}  " +
                             $"Score:{Player.Score}  ";
                Console.Write(hud.PadRight(Console.WindowWidth - 1));
                Console.ResetColor();
            }

            private List<GameItem> GetItemsWithinDistance(Position pos, double dist)
            {
                var result = new List<GameItem>();
                foreach (var item in Items)
                    if (item.Available && Position.Distance(item.Position, pos) <= dist)
                        result.Add(item);
                return result;
            }

            private List<Demon> GetDemonsWithinDistance(Position pos, double dist)
            {
                var result = new List<Demon>();
                foreach (var d in Demons)
                    if (d.Alive && Position.Distance(d.Position, pos) <= dist)
                        result.Add(d);
                return result;
            }

            private double GetTotalFillingRatio(Position pos)
            {
                double total = 0;
                foreach (var item in GetItemsWithinDistance(pos, 0))
                    total += item.FillingRatio;
                return total;
            }

            private void Move(Player player, Position target)
            {
                double fill = GetTotalFillingRatio(target) + player.FillingRatio;
                if (fill <= 1.0) player.Position = target;
            }

            private void Move(Demon demon, Position target, long dt)
            {
                double pMove = demon.Speed / 100.0 * dt / 1000.0;
                if (rng.NextDouble() >= pMove) return;
                double fill = GetTotalFillingRatio(target) + demon.FillingRatio;
                if (fill <= 1.0) demon.Position = target;
            }

            private void PlayerDirectInteraction()
            {
                foreach (var item in GetItemsWithinDistance(Player.Position, 1))
                {
                    if (item.Type == ItemType.Door) item.Interact();
                    if (item.Type == ItemType.LevelExit) { item.Interact(); Exited = true; }
                }
            }

            private void PlayerIndirectInteraction()
            {
                foreach (var item in GetItemsWithinDistance(Player.Position, 0))
                {
                    if (item.Type == ItemType.Ammo) { item.Interact(); Player.PickUpAmmo(5); }
                    else if (item.Type == ItemType.BFGCell) { item.Interact(); Player.PickUpBFGCell(); }
                    else if (item.Type == ItemType.Medikit) { item.Interact(); Player.PickUpHealth(25); }
                    else if (item.Type == ItemType.ToxicWaste) Player.TakeDamage(5);
                }
            }

            private void PlayerAttack()
            {
                if (Player.Ammo <= 0) return;
                Player.Shoot();
                foreach (var d in GetDemonsWithinDistance(Player.Position, Player.SightRange))
                {
                    double dist = Position.Distance(Player.Position, d.Position);
                    int dmg = 2 * rng.Next(35, 106) / (1 + (int)dist);
                    d.TakeDamage(dmg);
                    if (!d.Alive)
                    {
                        int pts = d.Type == DemonType.Zombieman ? 1 :
                                  d.Type == DemonType.Imp ? 3 : 10;
                        Player.AddCombatPoints(pts);
                    }
                }
            }

            private void PlayerBFGAttack()
            {
                if (Player.BFGCells <= 0) return;
                Player.ShootBFG();
                foreach (var d in GetDemonsWithinDistance(Player.Position, Player.SightRange))
                {
                    d.TakeDamage(rng.Next(100, 801));
                    if (!d.Alive) Player.AddCombatPoints(1);
                }
            }

            private void DemonAttack(Demon d, long dt)
            {
                double dist = Position.Distance(d.Position, Player.Position);
                int dmg = rng.Next(d.DmgMin, d.DmgMax + 1);
                int scaledDmg = (int)(dmg * dt / 1000.0);
                Player.TakeDamage(scaledDmg);
            }

            private void UpdateDemons(long dt)
            {
                foreach (var d in Demons)
                {
                    if (!d.Alive) continue;
                    double dist = Position.Distance(d.Position, Player.Position);

                    if (dist <= d.AttackRange)
                        DemonAttack(d, dt);
                    else if (dist <= d.SightRange)
                    {
                        int dx = Math.Sign(Player.Position.X - d.Position.X);
                        int dy = Math.Sign(Player.Position.Y - d.Position.Y);
                        var tryX = Position.Add(d.Position, new Position(dx, 0));
                        var tryY = Position.Add(d.Position, new Position(0, dy));
                        var target = rng.Next(2) == 0 ? tryX : tryY;
                        Move(d, target, dt);
                    }

                    foreach (var item in GetItemsWithinDistance(d.Position, 0))
                        if (item.Type == ItemType.ToxicWaste)
                            d.TakeDamage(5);
                }
            }

            private void CleanUp()
            {
                var deadItems = new List<GameItem>();
                foreach (var item in Items)
                    if (!item.Available) deadItems.Add(item);
                foreach (var i in deadItems) Items.Remove(i);

                var deadDemons = new List<Demon>();
                foreach (var d in Demons)
                    if (!d.Alive) deadDemons.Add(d);
                foreach (var d in deadDemons) Demons.Remove(d);
            }

            private void UserAction()
            {
                if (!Console.KeyAvailable) return;
                var key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.Escape: Exited = true; break;
                    case ConsoleKey.UpArrow:
                        Move(Player, Position.Add(Player.Position, new Position(0, -1))); break;
                    case ConsoleKey.DownArrow:
                        Move(Player, Position.Add(Player.Position, new Position(0, 1))); break;
                    case ConsoleKey.LeftArrow:
                        Move(Player, Position.Add(Player.Position, new Position(-1, 0))); break;
                    case ConsoleKey.RightArrow:
                        Move(Player, Position.Add(Player.Position, new Position(1, 0))); break;
                    case ConsoleKey.A:
                        PlayerAttack(); break;
                    case ConsoleKey.S:
                        PlayerBFGAttack(); break;
                    case ConsoleKey.D:
                        PlayerDirectInteraction(); break;
                }
                PlayerIndirectInteraction();
            }
            public void LoadMap(string[] mapLines)
            {
                for (int row = 0; row < mapLines.Length; row++)
                {
                    for (int col = 0; col < mapLines[row].Length; col++)
                    {
                        char c = mapLines[row][col];
                        switch (c)
                        {
                            case 'W': Items.Add(new GameItem(col, row, ItemType.Wall)); break;
                            case 'D': Items.Add(new GameItem(col, row, ItemType.Door)); break;
                            case 'E': Items.Add(new GameItem(col, row, ItemType.LevelExit)); break;
                            case 'A': Items.Add(new GameItem(col, row, ItemType.Ammo)); break;
                            case 'B': Items.Add(new GameItem(col, row, ItemType.BFGCell)); break;
                            case 'M': Items.Add(new GameItem(col, row, ItemType.Medikit)); break;
                            case 'T': Items.Add(new GameItem(col, row, ItemType.ToxicWaste)); break;
                            case 'z': Demons.Add(new Demon(col, row, DemonType.Zombieman)); break;
                            case 'i': Demons.Add(new Demon(col, row, DemonType.Imp)); break;
                            case 'm': Demons.Add(new Demon(col, row, DemonType.Mancubus)); break;
                            case 'p': Player = new Player(col, row); break;
                        }
                    }
                }
            }
            public void Run()
            {
                var swLogic = System.Diagnostics.Stopwatch.StartNew();
                var swRender = System.Diagnostics.Stopwatch.StartNew();

                while (!Exited && Player.Alive)
                {
                    UserAction();

                    if (swLogic.ElapsedMilliseconds > 500)
                    {
                        UpdateDemons(swLogic.ElapsedMilliseconds);
                        CleanUp();
                        swLogic.Restart();
                    }

                    if (swRender.ElapsedMilliseconds > 25)
                    {
                        RenderGame();
                        swRender.Restart();
                    }

                    Thread.Sleep(10);
                }

                Console.ResetColor();
                Console.Clear();
                Console.SetCursorPosition(0, 0);
                if (!Player.Alive)
                    Console.WriteLine("Meghaltál! Pontszámod: " + Player.Score);
                else
                    Console.WriteLine("Szint teljesítve! Pontszámod: " + Player.Score);
                Console.ReadLine();
            }
        }

        static void Main(string[] args)
        {
            string[] map = {
                "WWWWWWWWWWWWW",
                "W_M_WTTTB__pW",
                "W_A_Dz____WWW",
                "W___WTTT__W__",
                "W_m_WWWW__W__",
                "W_E_W__WiiW__",
                "WWWWW__WWWW__"
             };

            var game = new Game();
            game.LoadMap(map);
            game.Run();
          
        }
    }
}
