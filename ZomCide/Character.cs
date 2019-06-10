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

        public char PlayerInitial { get; set; }
        public int PlayerX { get; set; }
        public int PlayerY { get; set; }

        public int DeathThreshold { get; } = 3;
        public Tile PlayerTile { get; set; } = new Tile();
        public string CharacterName { get; set; }
        private int DamageTaken { get; set; }
        public int Experience { get; set; }
        public bool IsAlive { get; set; }
        private int SuperLevel { get; set; }
        public SkillLevel Level { get; set; }
        private string ArmorAlt { get; set; }
        public int numOfMoves { get; set; }
        public int movesLeft { get; set; }

        public Texture2D Texture { get; set; }
        public Point Position { get; set; }
        public Point Size { get; set; }

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
            numOfMoves = 2;
            movesLeft = numOfMoves;
            MainHandSlot = new Weapon("Sword of Summoning", 1, 4, false, 0, 1, 3, true);
            OffHandSlot = new Weapon("Dagger of Danger", 1, 5, false, 0, 4, 6);
            ActiveWeapon = (Weapon)MainHandSlot;
            Size = new Point(40, 40);
            Texture = PlayerIcon;
        }

        public static void Initialize(Zombicide game,char initial)
        {
            PlayerIcon = game.Content.Load<Texture2D>(@"playerIcons/" + initial);
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
            Position = new Point(MainGameScreen.mapX + (Convert.ToInt32(PlayerTile.column) * MainGameScreen.tileWidth) + (MainGameScreen.tileWidth / 2) - (Size.X / 2), MainGameScreen.mapY + (Convert.ToInt32(PlayerTile.row) * MainGameScreen.tileHeight) + (MainGameScreen.tileHeight / 2) - (Size.Y / 2));
            ActiveWeapon = (((Weapon)OffHandSlot).Active) ? (Weapon)OffHandSlot : (Weapon)MainHandSlot;
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

        public void Move(string[] startTile)
        {
            PlayerTile.row = Convert.ToInt32(startTile[0]);
            PlayerTile.column = Convert.ToInt32(startTile[1]);
        }
    }
}
