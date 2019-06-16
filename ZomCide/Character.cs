using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using GeonBit;
using GeonBit.UI;
using System.Xml;

namespace ZomCide
{
    public class Character : IDrawableGameObject
    {
        public static Texture2D PlayerIcon { get; set; }
        public static Texture2D EmptyHand { get; set; }
        public static Texture2D EmptyArmor { get; set; }

        public char PlayerInitial { get; set; }

        public int DeathThreshold { get; } = 3;
        public Tile PlayerTile { get; set; } = new Tile();
        public Tile LastTile { get; set; } = new Tile();
        public string CharacterName { get; set; }
        private int DamageTaken { get; set; }
        public int Experience { get; set; }
        public bool IsAlive { get; set; }
        private int SuperLevel { get; set; }
        public SkillLevel Level { get; set; }
        private string ArmorAlt { get; set; }
        public int numOfMoves { get; set; }
        public int movesLeft { get; set; }
        public bool moving { get; set; }
        public bool SearchedThisTurn { get; set; }

        public Texture2D Texture { get; set; }
        public Point Position { get; set; }
        public Point Size { get; set; }
        public int animationOffset { get; private set; }

        public Item MainHandSlot;
        public Item OffHandSlot;
        public Item ArmorSlot;
        public Weapon ActiveWeapon;

        List<Item> Backpack;

        public Skill BlueSkill;
        public Skill YellowSkill;
        public List<Skill> OrangeSkills;
        public List<Skill> RedSkills;


        public Character(string CharacterSelection, string bs, string ys, string os1, string os2, string rs1, string rs2, string rs3, string Aalt)
        {

            DamageTaken = 0;
            Experience = 0;
            IsAlive = true;
            SuperLevel = 0;
            Level = SkillLevel.Blue;
            Backpack = new List<Item>();
            OrangeSkills = new List<Skill>();
            RedSkills = new List<Skill>();
            CharacterName = CharacterSelection;
            BlueSkill = new Skill(bs);
            YellowSkill = new Skill(ys);
            OrangeSkills.Add(new Skill(os1));
            OrangeSkills.Add(new Skill(os2));
            RedSkills.Add(new Skill(rs1));
            RedSkills.Add(new Skill(rs2));
            RedSkills.Add(new Skill(rs3));
            ArmorAlt = Aalt;
            PlayerInitial = CharacterName.First();
            numOfMoves = 3;
            movesLeft = numOfMoves;
            //MainHandSlot = new Weapon("Sword of Summoning", 1, 4, false, 0, 1, 3, true);
            //OffHandSlot = new Weapon("Dagger of Danger", 1, 5, false, 0, 4, 6);
            //ActiveWeapon = (Weapon)MainHandSlot;
            Size = new Point(40, 40);
            Texture = PlayerIcon;
            SearchedThisTurn = false;
            animationOffset = 0;
        }

        public static void Initialize(Zombicide game,char initial)
        {
            PlayerIcon = game.Content.Load<Texture2D>(@"playerIcons/" + initial);
            EmptyHand = game.Content.Load<Texture2D>(@"EmptyHand");
            EmptyArmor = game.Content.Load<Texture2D>("ArmorSlot");
        }

        public List<Item> Search()
        {
            if (SearchedThisTurn) { return null; }
            //Check to see if the player is in a room or if there are zombies present
            if (MainGameScreen.tileData.Where(x=>x.row == PlayerTile.row&& x.column == PlayerTile.column).First().room == false || Zombie.zombieList.Where(x=> x.ZombieTile[0] == PlayerTile.row).Where(x => x.ZombieTile[1] == PlayerTile.column).Count() >0) { return null; }
            int draws = 1;

            //Check for torch
            if (MainHandSlot != null)
            {
                if (MainHandSlot.Name == "Torch")
                {
                    draws++;
                }
            }
            if (OffHandSlot != null)
            {

                if (OffHandSlot.Name == "Torch")
                {
                    draws++;
                }
            }

            //Randomly draw Items
            List<Item> Searchlist = new List<Item>();
            for (int i = 1; i <= draws; i++)
            {
                var undrawn = Item.ItemList.Where(x => x.drawn == false);
                if (undrawn.Count() == 0)
                {
                    foreach (Item I in Item.ItemList)
                    {
                        I.drawn = false;
                    }
                    undrawn = Item.ItemList.Where(x => x.drawn == false);
                }
                Item draw = undrawn.ElementAt(MainGameScreen.RNG.Next(0, undrawn.Count()));
                draw.drawn = true;
                Searchlist.Add(draw);
            }
            SearchedThisTurn = true;
            movesLeft--;
            return Searchlist;
        }

        public void SetActiveWeapon(Weapon w)
        {
            if (w != null)
            {
                ActiveWeapon = w;
                w.Active = true;
            }
        }

        private void LoadCharacter(string CharacterSelection)
        {


        }

        public int GetDamageTaken()
        {
            return DamageTaken;
        }

        public void ApplyDamage(int damage = 1)
        {
            DamageTaken += damage;
            if (DamageTaken >= DeathThreshold) { IsAlive = false; }
        }

        public void PickupObjective()
        {
            movesLeft--;
            ApplyExperience(5);
        }

        public void ApplyExperience(int XP = 1)
        {
            Experience += XP;
            if (Experience >= 0 && Experience < 7)
            { Level = SkillLevel.Blue; }
            else if (Experience >= 7 && Experience < 19)
            { Level = SkillLevel.Yellow; }
            else if (Experience >= 19 && Experience < 43)
            { Level = SkillLevel.Orange; }
            else if (Experience == 43)
            { Level = SkillLevel.Red; }
            else if (Experience > 43)
            {
                int Carryover = Experience - 43;
                Experience = 0;
                SuperLevel += 1;
                ApplyExperience(Carryover);
            }
        }

        public void CheckForNewSkill()
        {
            //After gaining XP, check if you leveled up and can gain a new skill
        }

        public void resetMoves(GeonBit.UI.Entities.Paragraph moves)
        {
            movesLeft = numOfMoves;
            moves.Text = "Moves Left: " + (movesLeft).ToString();
            SearchedThisTurn = false;
        }

        //public void breakDoor(Tile T, string side, Map map)
        //{
        //    if (side == "left")
        //    {
        //        T.LeftSide = RoomSide.opendoor;
        //        var neighborTile = map.TileData.Find(x => x.row == T.row && x.column == T.column - 1);
        //        neighborTile.RighSide = RoomSide.opendoor;

        //    }
        //    else if (side == "top")
        //    {
        //        T.TopSide = RoomSide.opendoor;
        //        var neighborTile = map.TileData.Find(x => x.row == T.row - 1 && x.column == T.column);
        //        neighborTile.BottomSide = RoomSide.opendoor;
        //    }
        //    map.applyMoveTiles(PlayerTile.row, PlayerTile.column);
        //    movesLeft--;
        //}

        public static XmlDocument PopulateCharacters(string baseDir)
        {
            XmlDocument doc = new XmlDocument();
            string fileName = "Zombicide Characters.xml";
            var CharPath = Path.Combine(baseDir, @"Data\", fileName);
            doc.Load(CharPath);
            return doc;
        }

        public void Update(Zombicide game)
        {
            //This handles move animation
            if (moving == true)
            {
                if (Position != new Point(MainGameScreen.mapX + (Convert.ToInt32(PlayerTile.column) * MainGameScreen.tileWidth) + (MainGameScreen.tileWidth / 2) - (Size.X / 2), MainGameScreen.mapY + (Convert.ToInt32(PlayerTile.row) * MainGameScreen.tileHeight) + (MainGameScreen.tileHeight / 2) - (Size.Y / 2)))
                {
                    animationOffset++;
                    int Ydir = PlayerTile.row - LastTile.row;
                    int Xdir = PlayerTile.column - LastTile.column;
                    Position = new Point((MainGameScreen.mapX + (Convert.ToInt32(LastTile.column) * MainGameScreen.tileWidth) + (MainGameScreen.tileWidth / 2) - (Size.X / 2)) + (animationOffset * Xdir), (MainGameScreen.mapY + (Convert.ToInt32(LastTile.row) * MainGameScreen.tileHeight) + (MainGameScreen.tileHeight / 2) - (Size.Y / 2)) + (animationOffset * Ydir));
                }
                else { moving = false; animationOffset = 0; }

            }
            else { Position = new Point(MainGameScreen.mapX + (Convert.ToInt32(PlayerTile.column) * MainGameScreen.tileWidth) + (MainGameScreen.tileWidth / 2) - (Size.X / 2), MainGameScreen.mapY + (Convert.ToInt32(PlayerTile.row) * MainGameScreen.tileHeight) + (MainGameScreen.tileHeight / 2) - (Size.Y / 2)); }

            ActiveWeapon = (((Weapon)MainHandSlot).Active) ? (Weapon)MainHandSlot : (Weapon)OffHandSlot;
        }

        public void Draw(Zombicide game)
        {
            game.SpriteBatch.Draw(Texture, new Rectangle(Position, Size), Color.White);
        }

        internal int Attack(Zombicide game, Zombie Z)
        {
            int hits = 0;
            if (CheckCanAttack(game, Z))
            {

                Dice.addDice(ActiveWeapon.Dice);
                Dice.Roll();
                hits = Dice.CalculateHits(ActiveWeapon.DiceThreshold);
                var mainScreen = (MainGameScreen)game.CurrentScreen;
                mainScreen.DicePopup();
                movesLeft--;

            }
            return hits;
        }

        private bool CheckCanAttack(Zombicide game, Zombie Z)
        {

            if (Z.ZombieTile[0] == PlayerTile.row && Math.Abs(Z.ZombieTile[1] - PlayerTile.column) <= ActiveWeapon.MaxRange && Math.Abs(Z.ZombieTile[1] - PlayerTile.column) >= ActiveWeapon.MinRange)
            { return true; }
            else if (Z.ZombieTile[1] == PlayerTile.column && Math.Abs(Z.ZombieTile[0] - PlayerTile.row) <= ActiveWeapon.MaxRange && Math.Abs(Z.ZombieTile[0] - PlayerTile.row) >= ActiveWeapon.MinRange)
            { return true; }
            else return false;
        }

        public void Move(string[] tile,bool firstPlacement=false)
        {
            if (firstPlacement)
            {
                PlayerTile.row = Convert.ToInt32(tile[0]);
                PlayerTile.column = Convert.ToInt32(tile[1]);
                LastTile.row = PlayerTile.row;
                LastTile.column = PlayerTile.column;
                Position = new Point(MainGameScreen.mapX + (Convert.ToInt32(PlayerTile.column) * MainGameScreen.tileWidth) + (MainGameScreen.tileWidth / 2) - (Size.X / 2), MainGameScreen.mapY + (Convert.ToInt32(PlayerTile.row) * MainGameScreen.tileHeight) + (MainGameScreen.tileHeight / 2) - (Size.Y / 2));
            }
            else if (CheckCanMove(out int extra))
            {
                moving = true;
                LastTile.row = PlayerTile.row;
                LastTile.column = PlayerTile.column;
                PlayerTile.row = Convert.ToInt32(tile[0]);
                PlayerTile.column = Convert.ToInt32(tile[1]);
                movesLeft = movesLeft - extra - 1;
            }
            
        }

        private bool CheckCanMove(out int extra)
        {
            extra = Zombie.zombieList.Where(x => x.ZombieTile[0] == PlayerTile.row && x.ZombieTile[1] == PlayerTile.column).Count();
            if (extra >= movesLeft)
                return false;
            else return true;
        }
    }
}
