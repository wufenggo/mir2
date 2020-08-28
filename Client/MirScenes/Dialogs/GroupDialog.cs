using Client.MirControls;
using Client.MirGraphics;
using Client.MirNetwork;
using Client.MirObjects;
using Client.MirSounds;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using C = ClientPackets;

namespace Client.MirScenes.Dialogs
{
    public sealed class GroupDialog : MirImageControl
    {
        public static bool AllowGroup;
        public static List<string> GroupList = new List<string>();

        public MirImageControl TitleLabel;
        public MirButton SwitchButton, CloseButton, AddButton, DelButton;
        public MirLabel[] GroupMembers;
        public MirLabel GroupStage, GroupExperient, checkBoxLabel;
        public MirCheckBox LockInfoBox;

        public bool LockInfo = false;
        public GroupDialog()
        {
            Index = 120;
            Library = Libraries.Prguse;
            Movable = true;
            Sort = true;
            Location = Center;

            GroupMembers = new MirLabel[Globals.MaxGroup];

            GroupMembers[0] = new MirLabel
            {
                AutoSize = true,
                Location = new Point(16, 33),
                Parent = this,
                NotControl = true,
            };

            for (int i = 1; i < GroupMembers.Length; i++)
            {
                GroupMembers[i] = new MirLabel
                {
                    AutoSize = true,
                    Location = new Point(((i + 1) % 2) * 100 + 16, 55 + ((i - 1) / 2) * 20),
                    Parent = this,
                    NotControl = true,
                };
            }



            TitleLabel = new MirImageControl
            {
                Index = 5,
                Library = Libraries.Title,
                Location = new Point(18, 8),
                Parent = this
            };

            CloseButton = new MirButton
            {
                HoverIndex = 361,
                Index = 360,
                Location = new Point(209, 3),
                Library = Libraries.Prguse2,
                Parent = this,
                PressedIndex = 362,
                Sound = SoundList.ButtonA,
                Hint = "关闭"
            };
            CloseButton.Click += (o, e) => Hide();

            GroupStage = new MirLabel
            {
                AutoSize = true,
                Location = new Point(78, 248),
                Parent = this,
                NotControl = true,
                Text = "阶段: 0",
                Font = new Font(Settings.FontName, 8F, FontStyle.Bold),
            };

            GroupExperient = new MirLabel
            {
                AutoSize = true,
                Location = new Point(78, 284),
                Parent = this,
                NotControl = true,
                Text = "额外经验+ 0%",
                Font = new Font(Settings.FontName, 8F, FontStyle.Bold),
            };

            SwitchButton = new MirButton
            {
                HoverIndex = 115,
                Index = 114,
                Location = new Point(25, 219),
                Library = Libraries.Prguse,
                Parent = this,
                PressedIndex = 116,
                Sound = SoundList.ButtonA,
            };
            SwitchButton.Click += (o, e) => Network.Enqueue(new C.SwitchGroup { AllowGroup = !AllowGroup });

            LockInfoBox = new MirCheckBox
            {
                Index = 2086,
                UnTickedIndex = 2086,
                TickedIndex = 2087,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(136, 268),
                Hint = "组队信息开/关.",
                Checked = false,
                Visible = false
            };
            checkBoxLabel = new MirLabel
            {
                Text = "组队信息开/关.",
                Location = new Point(136, 268),
                ForeColour = Color.Goldenrod,
                Font = new Font(Settings.FontName, 12f),
                Parent = this,
                Visible = false,
                NotControl = true
            };
            LockInfoBox.Click += (o, e) =>
            {
                if (LockInfoBox.Checked)
                    LockInfo = true;
                else
                    LockInfo = false;
            };
            AddButton = new MirButton
            {
                HoverIndex = 134,
                Index = 133,
                Location = new Point(70, 219),
                Library = Libraries.Title,
                Parent = this,
                PressedIndex = 135,
                Sound = SoundList.ButtonA,
                Hint = "添加"
            };
            AddButton.Click += (o, e) => AddMember();

            DelButton = new MirButton
            {
                HoverIndex = 137,
                Index = 136,
                Location = new Point(140, 219),
                Library = Libraries.Title,
                Parent = this,
                PressedIndex = 138,
                Sound = SoundList.ButtonA,
                Hint = "删除"
            };
            DelButton.Click += (o, e) => DelMember();

            BeforeDraw += GroupPanel_BeforeDraw;

            GroupList.Clear();
        }

        private void GroupPanel_BeforeDraw(object sender, EventArgs e)
        {
            if (GroupList.Count == 0)
            {
                AddButton.Index = 130;
                AddButton.HoverIndex = 131;
                AddButton.PressedIndex = 132;

                GroupExperient.Visible = false;
                GroupStage.Visible = false;
                LockInfoBox.Visible = false;
                checkBoxLabel.Visible = false;
            }
            else
            {

                AddButton.Index = 133;
                AddButton.HoverIndex = 134;
                AddButton.PressedIndex = 135;

                GroupExperient.Visible = true;
                GroupStage.Visible = true;
            }
            if (GroupList.Count > 0 && GroupList[0] != MapObject.User.Name)
            {
                LockInfoBox.Visible = true;
                LockInfoBox.Location = new Point(136, 268);
                checkBoxLabel.Visible = true;
                checkBoxLabel.Location = new Point(136, 268);
                AddButton.Visible = false;
                DelButton.Visible = false;
            }
            else
            {
                AddButton.Visible = true;
                DelButton.Visible = true;

                LockInfoBox.Location = new Point(136, 268);
                checkBoxLabel.Location = new Point(136, 268);
                LockInfoBox.Visible = true;
                checkBoxLabel.Visible = true;
            }

            if (AllowGroup)
            {
                SwitchButton.Index = 117;
                SwitchButton.HoverIndex = 118;
                SwitchButton.PressedIndex = 119;
            }
            else
            {
                SwitchButton.Index = 114;
                SwitchButton.HoverIndex = 115;
                SwitchButton.PressedIndex = 116;
            }

            for (int i = 0; i < GroupMembers.Length; i++)
                GroupMembers[i].Text = i >= GroupList.Count ? string.Empty : GroupList[i];

            GroupExperient.Text = "额外经验+ 0%";
            GroupStage.Text = "阶段: 0";

            var grpBuff = GameScene.Scene.Buffs.FirstOrDefault(x => x.Type == BuffType.Group);
            if (grpBuff != null)
            {
                GroupExperient.Text = "额外经验+ " + grpBuff.Values[0] + "%";
                GroupStage.Text = "阶段: " + grpBuff.Values[1];
            }
        }

        public void AddMember(string name)
        {
            if (GroupList.Count >= Globals.MaxGroup)
            {
                GameScene.Scene.ChatDialog.ReceiveChat("你的小组已经满员了", ChatType.System);
                return;
            }
            if (GroupList.Count > 0 && GroupList[0] != MapObject.User.Name)
            {
                GameScene.Scene.ChatDialog.ReceiveChat("你不是小组队长", ChatType.System);
                return;
            }

            Network.Enqueue(new C.AddMember { Name = name });
        }

        private void AddMember()
        {
            if (GroupList.Count >= Globals.MaxGroup)
            {
                GameScene.Scene.ChatDialog.ReceiveChat("你的小组已经满员了", ChatType.System);
                return;
            }
            if (GroupList.Count > 0 && GroupList[0] != MapObject.User.Name)
            {

                GameScene.Scene.ChatDialog.ReceiveChat("你不是小组队长", ChatType.System);
                return;
            }

            MirInputBox inputBox = new MirInputBox("请输入你想要加入玩家的名字");

            inputBox.OKButton.Click += (o, e) =>
            {
                Network.Enqueue(new C.AddMember { Name = inputBox.InputTextBox.Text });
                inputBox.Dispose();
            };
            inputBox.Show();
        }
        private void DelMember()
        {
            if (GroupList.Count > 0 && GroupList[0] != MapObject.User.Name)
            {

                GameScene.Scene.ChatDialog.ReceiveChat("你不是小组队长", ChatType.System);
                return;
            }

            MirInputBox inputBox = new MirInputBox("请输入你想要踢除玩家的名字");

            inputBox.OKButton.Click += (o, e) =>
            {
                Network.Enqueue(new C.DelMember { Name = inputBox.InputTextBox.Text });
                inputBox.Dispose();
            };
            inputBox.Show();
        }


        public void Hide()
        {
            if (!Visible) return;
            Visible = false;
        }
        public void Show()
        {
            if (Visible) return;
            Visible = true;
        }
    }
}
