using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client.MirControls;
using Client.MirGraphics;
using Client.MirNetwork;
using Client.MirSounds;
using C = ClientPackets;

namespace Client.MirScenes.Dialogs
{
    public class NewHeroDialog : MirImageControl
    {
        private static readonly Regex Reg = new Regex(@"^.{" + Globals.MinCharacterNameLength + "," + Globals.MaxCharacterNameLength + "}$");

        public MirImageControl TitleLabel;
        public MirAnimatedControl CharacterDisplay;

        public MirButton OKButton,
                         CancelButton,
                         WarriorButton,
                         WizardButton,
                         TaoistButton,
                         AssassinButton,
                         ArcherButton,
                         MaleButton,
                         FemaleButton;

        public MirTextBox NameTextBox;

        public MirLabel Description;

        protected MirClass _class;
        protected MirGender _gender;

        #region Descriptions
        public const string WarriorDescription =
                 "以强有力的体格为基础，特殊的地方在于用剑法及道法等技术。对打猎、战斗比较适用" +
                 "体力强的战士能带许多东西，即便带沉重的武器及铠甲也可以自由活动" +
                 "。但战士所带的铠甲对魔法的防御能力相对较低.";

        public const string WizardDescription =
            "以长时间锻炼的内功为基础，能发挥强大的攻击型魔法。魔法攻击卓越。体力较弱，" +
            "对体力上直接受到攻击的防御能力较低，还有能发挥高水平的魔法时需要较长时间，此时可能受到对方的快速攻击" +
            "魔法师的魔法比任何攻击能力都强大，能有效的威胁对方.";

        public const string TaoistDescription =
            "以强大的精神力作为基础，可以使用治疗术帮助别人。对自然很熟悉，在用毒方面的能力最强。" +
            "道士博学多知，能使用剑术和魔法，所以每时每刻都能发挥多样的法术，随机应变性强.";


        public const string AssassinDescription =
            "以敏捷快速的攻击为基础，矫健的刺客还拥有超强的爆发性，他们熟悉各种技能，尤其擅长瞬移、" +
            "潜行技能！他们是暗夜的主人，是绝对的伤害高、攻击高、爆发型的职业.";


        public const string ArcherDescription =
            "弓箭手是一个类的精度和强度,利用他们强大的技能与弓非同寻常的伤害范围。" +
            "就像法师那样,他们依靠敏锐的本能躲避迎面而来的攻击,因为他们倾向于让自己正面攻击。" +
            " 然而,他们的身体能力和致命的目的让他们恐惧渗透到任何他们.";
        #endregion

        public NewHeroDialog()
        {
            Index = 73;
            Library = Libraries.Prguse;
            Location = new Point((Settings.ScreenWidth - Size.Width) / 2, (Settings.ScreenHeight - Size.Height) / 2);
            Modal = true;

            TitleLabel = new MirImageControl
            {
                Index = 40,
                Library = Libraries.Title,
                Location = new Point(256, 11),
                Parent = this,
            };

            CancelButton = new MirButton
            {
                HoverIndex = 281,
                Index = 280,
                Library = Libraries.Title,
                Location = new Point(425, 425),
                Parent = this,
                PressedIndex = 282
            };
            CancelButton.Click += (o, e) => Dispose();


            OKButton = new MirButton
            {
                Enabled = false,
                HoverIndex = 361,
                Index = 360,
                Library = Libraries.Title,
                Location = new Point(160, 425),
                Parent = this,
                PressedIndex = 362,
            };
            OKButton.Click += (o, e) => CreateHero();

            NameTextBox = new MirTextBox
            {
                Location = new Point(325, 268),
                Parent = this,
                Size = new Size(240, 20),
                MaxLength = Globals.MaxCharacterNameLength
            };
            NameTextBox.TextBox.KeyPress += TextBox_KeyPress;
            NameTextBox.TextBox.TextChanged += CharacterNameTextBox_TextChanged;
            NameTextBox.SetFocus();

            CharacterDisplay = new MirAnimatedControl
            {
                Animated = true,
                AnimationCount = 16,
                AnimationDelay = 250,
                Index = 20,
                Library = Libraries.ChrSel,
                Location = new Point(120, 250),
                Parent = this,
                UseOffSet = true,
            };
            CharacterDisplay.AfterDraw += (o, e) =>
            {
                if (_class == MirClass.Wizard)
                    Libraries.ChrSel.DrawBlend(CharacterDisplay.Index + 560, CharacterDisplay.DisplayLocationWithoutOffSet, Color.White, true);
            };


            WarriorButton = new MirButton
            {
                HoverIndex = 2427,
                Index = 2427,
                Library = Libraries.Prguse,
                Location = new Point(323, 296),
                Parent = this,
                PressedIndex = 2428,
                Sound = SoundList.ButtonA,
            };
            WarriorButton.Click += (o, e) =>
            {
                _class = MirClass.Warrior;
                UpdateInterface();
            };


            WizardButton = new MirButton
            {
                HoverIndex = 2430,
                Index = 2429,
                Library = Libraries.Prguse,
                Location = new Point(373, 296),
                Parent = this,
                PressedIndex = 2431,
                Sound = SoundList.ButtonA,
            };
            WizardButton.Click += (o, e) =>
            {
                _class = MirClass.Wizard;
                UpdateInterface();
            };


            TaoistButton = new MirButton
            {
                HoverIndex = 2433,
                Index = 2432,
                Library = Libraries.Prguse,
                Location = new Point(423, 296),
                Parent = this,
                PressedIndex = 2434,
                Sound = SoundList.ButtonA,
            };
            TaoistButton.Click += (o, e) =>
            {
                _class = MirClass.Taoist;
                UpdateInterface();
            };

            AssassinButton = new MirButton
            {
                HoverIndex = 2436,
                Index = 2435,
                Library = Libraries.Prguse,
                Location = new Point(473, 296),
                Parent = this,
                PressedIndex = 2437,
                Sound = SoundList.ButtonA,
            };
            AssassinButton.Click += (o, e) =>
            {
                _class = MirClass.Assassin;
                UpdateInterface();
            };

            ArcherButton = new MirButton
            {
                HoverIndex = 2439,
                Index = 2438,
                Library = Libraries.Prguse,
                Location = new Point(523, 296),
                Parent = this,
                PressedIndex = 2440,
                Sound = SoundList.ButtonA,
            };
            ArcherButton.Click += (o, e) =>
            {
                _class = MirClass.Archer;
                UpdateInterface();
            };


            MaleButton = new MirButton
            {
                HoverIndex = 2421,
                Index = 2421,
                Library = Libraries.Prguse,
                Location = new Point(323, 343),
                Parent = this,
                PressedIndex = 2422,
                Sound = SoundList.ButtonA,
            };
            MaleButton.Click += (o, e) =>
            {
                _gender = MirGender.Male;
                UpdateInterface();
            };

            FemaleButton = new MirButton
            {
                HoverIndex = 2424,
                Index = 2423,
                Library = Libraries.Prguse,
                Location = new Point(373, 343),
                Parent = this,
                PressedIndex = 2425,
                Sound = SoundList.ButtonA,
            };
            FemaleButton.Click += (o, e) =>
            {
                _gender = MirGender.Female;
                UpdateInterface();
            };

            Description = new MirLabel
            {
                Border = true,
                Location = new Point(279, 70),
                Parent = this,
                Size = new Size(278, 170),
                Text = WarriorDescription,
            };
        }

        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (sender == null) return;
            if (e.KeyChar != (char)Keys.Enter) return;
            e.Handled = true;

            if (OKButton.Enabled)
                OKButton.InvokeMouseClick(null);
        }
        private void CharacterNameTextBox_TextChanged(object sender, EventArgs e)
        {
            var whiteSpacePattern = new Regex(@"\s+");
            NameTextBox.Text = whiteSpacePattern.Replace(NameTextBox.Text, "");
            NameTextBox.TextBox.SelectionStart = NameTextBox.Text.Length;

            if (string.IsNullOrEmpty(NameTextBox.Text))
            {
                OKButton.Enabled = false;
                NameTextBox.Border = false;
            }
            else if (!Reg.IsMatch(NameTextBox.Text))
            {
                OKButton.Enabled = false;
                NameTextBox.Border = true;
                NameTextBox.BorderColour = Color.Red;
            }
            else
            {
                OKButton.Enabled = true;
                NameTextBox.Border = true;
                NameTextBox.BorderColour = Color.Green;
            }
        }

        private void CreateHero()
        {
            OKButton.Enabled = false;

            Network.Enqueue(new C.NewHero()
            {
                Name = NameTextBox.Text,
                Class = _class,
                Gender = _gender
            });
            this.Dispose();
        }

        private void UpdateInterface()
        {
            MaleButton.Index = 2420;
            FemaleButton.Index = 2423;

            WarriorButton.Index = 2426;
            WizardButton.Index = 2429;
            TaoistButton.Index = 2432;
            AssassinButton.Index = 2435;
            ArcherButton.Index = 2438;

            switch (_gender)
            {
                case MirGender.Male:
                    MaleButton.Index = 2421;
                    break;
                case MirGender.Female:
                    FemaleButton.Index = 2424;
                    break;
            }

            switch (_class)
            {
                case MirClass.Warrior:
                    WarriorButton.Index = 2427;
                    Description.Text = WarriorDescription;
                    CharacterDisplay.Index = (byte)_gender == 0 ? 20 : 300; //220 : 500;
                    break;
                case MirClass.Wizard:
                    WizardButton.Index = 2430;
                    Description.Text = WizardDescription;
                    CharacterDisplay.Index = (byte)_gender == 0 ? 40 : 320; //240 : 520;
                    break;
                case MirClass.Taoist:
                    TaoistButton.Index = 2433;
                    Description.Text = TaoistDescription;
                    CharacterDisplay.Index = (byte)_gender == 0 ? 60 : 340; //260 : 540;
                    break;
                case MirClass.Assassin:
                    AssassinButton.Index = 2436;
                    Description.Text = AssassinDescription;
                    CharacterDisplay.Index = (byte)_gender == 0 ? 80 : 360; //280 : 560;
                    break;
                case MirClass.Archer:
                    ArcherButton.Index = 2439;
                    Description.Text = ArcherDescription;
                    CharacterDisplay.Index = (byte)_gender == 0 ? 100 : 140; //160 : 180;
                    break;
            }

            //CharacterDisplay.Index = ((byte)_class + 1) * 20 + (byte)_gender * 280;
        }
    }
}
