using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ZomCide.GamePlayers;

// using GeonBit UI elements
using GeonBit.UI;
using GeonBit.UI.Entities;

namespace ZomCide
{
    public class CharacterSelectScreen : GameScreen
    {
        public Character SelectedCharacter { get; private set; }
        private int weaponIndex;

        public CharacterSelectScreen(Zombicide game)
        {
            Item.Initialize(game);
            LoadCharSelection(game);
        }

        public override void Update(Zombicide game)
        {
        }

        public override void Draw(Zombicide game)
        {
        }

        public int LimitToRange(int value, int inclusiveMinimum, int inclusiveMaximum)
        {
            if (value < inclusiveMinimum) { return inclusiveMinimum; }
            if (value > inclusiveMaximum) { return inclusiveMaximum; }
            return value;
        }

        void LoadCharSelection(Zombicide game)
        {


            //Setup Last Panel to hold skills and confirm button
            Panel skillPanel = new Panel(new Vector2(400, 600), PanelSkin.Default, Anchor.AutoInline);
            Paragraph bs = new Paragraph("Blue Skill", Anchor.Auto, Color.DeepSkyBlue);
            Paragraph ys = new Paragraph("Yellow Skill", Anchor.Auto, Color.Yellow);
            Paragraph os1 = new Paragraph("Orange Skill", Anchor.Auto, Color.Orange);
            Paragraph os2 = new Paragraph("Orange Skill", Anchor.Auto, Color.Orange);
            Paragraph rs1 = new Paragraph("Red Skill", Anchor.Auto, Color.Red);
            Paragraph rs2 = new Paragraph("Red Skill", Anchor.Auto, Color.Red);
            Paragraph rs3 = new Paragraph("Red Skill", Anchor.Auto, Color.Red);
            Paragraph ArmAlt = new Paragraph("Item", Anchor.Auto, Color.White);
            Button ConfirmBut = new Button("CONFIRM", ButtonSkin.Default, Anchor.BottomCenter);
            ConfirmBut.Enabled = false;

            skillPanel.AddChild(new Header("Skills"));
            skillPanel.AddChild(new HorizontalLine());
            skillPanel.AddChild(bs);
            skillPanel.AddChild(ys);
            skillPanel.AddChild(os1);
            skillPanel.AddChild(os2);
            skillPanel.AddChild(rs1);
            skillPanel.AddChild(rs2);
            skillPanel.AddChild(rs3);
            skillPanel.AddChild(new LineSpace(5));
            skillPanel.AddChild(new Header("Armor Alternative"));
            skillPanel.AddChild(new HorizontalLine());
            skillPanel.AddChild(ArmAlt);
            skillPanel.AddChild(ConfirmBut);


            //Setup First Panel to hold character list
            Panel Charpanel = new Panel(new Vector2(400, 600), PanelSkin.Default, Anchor.AutoInline, new Vector2((game.GraphicsDevice.Viewport.Width / 2) - 700, 100));
            Charpanel.AddChild(new Header("Select Character"));
            Charpanel.AddChild(new HorizontalLine());
            SelectList list = new SelectList(new Vector2(0, 500));
            var CharDocument = PopulateCharacters();
            foreach (XmlNode node in CharDocument.DocumentElement.ChildNodes)
            {
                list.AddItem(node.FirstChild.InnerText);

            }
            Charpanel.AddChild(list);

            //Setup middle panel to hold character image
            Panel picpanel = new Panel(new Vector2(600, 600), PanelSkin.Default, Anchor.AutoInline);
            Image img = new Image(new Texture2D(game.GraphicsDevice, 400, 400));
            picpanel.AddChild(img);

            //Add Panels to UI
            UserInterface.Active.AddEntity(Charpanel);
            UserInterface.Active.AddEntity(picpanel);
            UserInterface.Active.AddEntity(skillPanel);

            //Set events
            ConfirmBut.OnClick = (Entity btn) =>
            {
                Character.Initialize(game, list.SelectedValue.First());
                SelectedCharacter = new Character(list.SelectedValue, bs.Text, ys.Text, os1.Text, os2.Text, rs1.Text, rs2.Text, rs3.Text, ArmAlt.Text);
                StarterPopup(game);
            };

            list.OnValueChange = (Entity lst) =>
            {
                ConfirmBut.Enabled = true;
                var name = list.SelectedValue;
                XmlNode selectedNode = null;
                foreach (XmlNode node in CharDocument.DocumentElement.ChildNodes)
                {
                    if (node.FirstChild.InnerText == name)
                    {
                        selectedNode = node;
                    }

                }
                string blueSkill = selectedNode.ChildNodes.Item(1).InnerText;
                string yellowSkill = selectedNode.ChildNodes.Item(2).InnerText;
                string orangeSkill1 = selectedNode.ChildNodes.Item(3).InnerText;
                string OrangeSkill2 = selectedNode.ChildNodes.Item(4).InnerText;
                string RedSkill1 = selectedNode.ChildNodes.Item(5).InnerText;
                string RedSkill2 = selectedNode.ChildNodes.Item(6).InnerText;
                string RedSkill3 = selectedNode.ChildNodes.Item(7).InnerText;
                string ArmorAlternative = selectedNode.ChildNodes.Item(8).InnerText;

                bs.Text = blueSkill;
                ys.Text = yellowSkill;
                os1.Text = orangeSkill1;
                os2.Text = OrangeSkill2;
                rs1.Text = RedSkill1;
                rs2.Text = RedSkill2;
                rs3.Text = RedSkill3;
                ArmAlt.Text = ArmorAlternative;

                string baseDir = Directory.GetCurrentDirectory();
                string fileName = name + ".jpg";
                var imgPath = Path.Combine(baseDir, @"Data\", fileName);
                FileStream imgFile = File.OpenRead(imgPath);
                img.Texture = Texture2D.FromStream(game.GraphicsDevice, imgFile);
            };
        }

        XmlDocument PopulateCharacters()
        {
            XmlDocument doc = new XmlDocument();
            string fileName = "Zombicide Characters.xml";
            var CharPath = Path.Combine(Directory.GetCurrentDirectory(), @"Data\", fileName);
            doc.Load(CharPath);
            return doc;

        }

        private void StarterPopup(Zombicide game)
        {
            UserInterface.Active.Clear();
            Panel StarterPopup = new Panel(new Vector2(600, 700), PanelSkin.Default, Anchor.Center);
            var header = new Header("Select Starter Weapon", Anchor.TopCenter);

            List<Texture2D> starterTextures = new List<Texture2D>();
            foreach (Item I in Item.StarterList)
            {
                string fileName = I.Name.Replace(' ', '_');
                Texture2D item = game.Content.Load<Texture2D>(@"Items\" + fileName);
                starterTextures.Add(item);
            }


            Image wpnImage = new Image(starterTextures.First(), new Vector2(350, 500), anchor: Anchor.AutoCenter);
            weaponIndex = 0;

            var leftButton = new Button("", ButtonSkin.Default, Anchor.CenterLeft, new Vector2(50, 50));
            leftButton.Padding = new Vector2(0, 0);
            leftButton.AddChild(new Paragraph("<", Anchor.Center));
            leftButton.OnClick = (Entity btn) =>
            {
                weaponIndex = LimitToRange(weaponIndex - 1,0,3);
                
                wpnImage.Texture = starterTextures.ElementAt(weaponIndex);
            };
            
            var rightButton = new Button("", ButtonSkin.Default, Anchor.CenterRight, new Vector2(50, 50));
            rightButton.Padding = new Vector2(0, 0);
            rightButton.AddChild(new Paragraph(">", Anchor.Center));
            rightButton.OnClick = (Entity btn) =>
            {
                weaponIndex = LimitToRange(weaponIndex + 1, 0, 3);
                wpnImage.Texture = starterTextures.ElementAt(weaponIndex);
            };

            var button = new Button("OK", ButtonSkin.Default, Anchor.BottomCenter, new Vector2(300, 50));
            button.OnClick = (Entity btn) =>
            {
                SelectedCharacter.MainHandSlot = (Weapon)Item.StarterList.Where(x => wpnImage.TextureName.Replace('_', ' ').Contains(x.Name)).First();
                SelectedCharacter.ActiveWeapon = (Weapon)SelectedCharacter.MainHandSlot;
                SelectedCharacter.ActiveWeapon.Active = true;
                UserInterface.Active.Clear();
                game.SetNextScreen(nameof(MainGameScreen));
            };

            StarterPopup.AddChild(header);
            StarterPopup.AddChild(new HorizontalLine());
            StarterPopup.AddChild(wpnImage);
            StarterPopup.AddChild(leftButton);
            StarterPopup.AddChild(rightButton);
            StarterPopup.AddChild(button);
            UserInterface.Active.AddEntity(StarterPopup);

        }

        

    }
}
