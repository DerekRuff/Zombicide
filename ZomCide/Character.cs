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
        public char PlayerIcon { get; set; }
        public int PlayerX { get; set; }
        public int PlayerY { get; set; }
        public Tile PlayerTile { get; set; } = new Tile();

        public int DeathThreshold { get; } = 3;

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

        public Vector2 Position { get; set; }

        public Item MainHandSlot;
        public Item OffHandSlot;
        public Item ArmorSlot;
        public Weapon ActiveWeapon;

        List<Item> Backpack;

        Skill BlueSkill;
        Skill YellowSkill;
        List<Skill> OrangeSkills;
        List<Skill> RedSkills;


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
            PlayerIcon = CharacterName.First();
            numOfMoves = 2;
            movesLeft = numOfMoves;
            MainHandSlot = new Weapon(1, 4, false, 0, 1, 3);
            ActiveWeapon = (Weapon)MainHandSlot;
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

        }

        public void Draw(Zombicide game)
        {

        }

        internal int Attack(Zombicide game, Zombie Z)
        {
            int hits = 0;
            if (CheckCanAttack(game, Z))
            {
                
                var diceList = new List<Dice>();
                for (int i = 0; i < ActiveWeapon.Dice; i++)
                {
                    var d = new Dice(game);
                    diceList.Add(d);
                    d.Roll();
                    if (d.value >= ActiveWeapon.DiceThreshold)
                    { hits++; }
                }
                
                var mainScreen = (MainGameScreen)game.CurrentScreen;
                mainScreen.DicePopup();
                movesLeft--;
                mainScreen.moves.Text = "Moves Left: " + (game.ActiveCharacter.movesLeft).ToString();
                
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
    }
}
