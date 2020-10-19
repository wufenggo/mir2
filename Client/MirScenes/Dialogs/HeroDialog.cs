using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client.MirControls;
using Client.MirGraphics;
using Client.MirNetwork;
using Client.MirObjects;
using Client.MirSounds;
using ClientPackets;

namespace Client.MirScenes.Dialogs
{
    public sealed class HeroOPControl : MirImageControl
    {
        public MirButton SummonHeroButton;
        public MirButton ShowOPLabelButton;
        public HeroOPLabel OpLabel;

        public HeroOPControl(MirControl parent)
        {
            Parent = parent;
            Location = new Point(parent.Size.Width / 2 + 240, parent.Size.Height - 83);
            Size = new Size(20, 40);
            ShowOPLabelButton = new MirButton
            {
                Index = 2164,
                HoverIndex = 2165,
                PressedIndex = 2166,
                Library = Libraries.Prguse,
                Location = new Point(0, 0),
                Parent = this,
                Sound = SoundList.ButtonC,
                Hint = "英雄资料"
            };
            SummonHeroButton = new MirButton
            {
                Index = 2167,
                HoverIndex = 2168,
                PressedIndex = 2169,
                Library = Libraries.Prguse,
                Location = new Point(0, 23),
                Parent = this,
                Sound = SoundList.ButtonC,
                Hint = "召唤英雄"
            };

            ShowOPLabelButton.Click += (sender, args) =>
            {
                OpLabel.Visible = !OpLabel.Visible;
                OpLabel.BringToFront();
            };

            OpLabel = new HeroOPLabel(this);

            SummonHeroButton.Click += SummonHeroButtonOnClick;
        }

        private void SummonHeroButtonOnClick(object sender, EventArgs eventArgs)
        {
            if (GameScene.Scene.CurrentHeroIndex < 1)
            {
                GameScene.Scene.ChatDialog.ReceiveChat("没有英雄", ChatType.System);
                return;
            }
            if (GameScene.Scene.HeroSummoned)
            {
                return;
            }
            Network.Enqueue(new SummonHero());
        }
    }

    public sealed class HeroOPLabel : MirImageControl
    {
        public MirButton HeroBuffButton;
        public MirButton HeroInventoryButton;
        public MirButton HeroInfoButton;

        public HeroOPLabel(MirControl parent)
        {
            Parent = parent;
            Index = 2179;
            Library = Libraries.Prguse;
            Location = new Point(-1, 0 - Size.Height + 8);
            Visible = false;

            HeroInfoButton = new MirButton
            {
                Index = 2176,
                HoverIndex = 2177,
                PressedIndex = 2178,
                Library = Libraries.Prguse,
                Location = new Point(3, 37),
                Parent = this,
                Sound = SoundList.ButtonC,
                Hint = "英雄信息"
            };

            HeroInventoryButton = new MirButton
            {
                Index = 2170,
                HoverIndex = 2171,
                PressedIndex = 2172,
                Library = Libraries.Prguse,
                Location = new Point(3, 20),
                Parent = this,
                Sound = SoundList.ButtonC,
                Hint = "英雄背包"
            };
            HeroBuffButton = new MirButton
            {
                Index = 2173,
                HoverIndex = 2174,
                PressedIndex = 2175,
                Library = Libraries.Prguse,
                Location = new Point(3, 3),
                Parent = this,
                Sound = SoundList.ButtonC,
                Hint = "英雄Buff"
            };
        }
    }

    public sealed class HeroAvatarControl : MirImageControl
    {
        public MirImageControl NameBackGround;
        public MirImageControl Avatar;
        public MirImageControl LockState;
        public MirImageControl BarBackGround;
        public MirImageControl HpBar;
        public MirImageControl MpBar;
        public MirImageControl ExpBar;
        public MirLabel HpLabel;
        public MirLabel MpLabel;
        public MirLabel ExpLabel;
        public MirLabel NameLabel;
        public MirLabel LevelLabel;

        public HeroAvatarControl(MirControl parent)
        {
            Parent = parent;
            Location = new Point(parent.Size.Width / 2 - 290, parent.Size.Height - 84);
            Size = new Size(100, 80);
            Avatar = new MirImageControl()//英雄脸部图像
            {
                Index = 1379,
                Library = Libraries.Prguse,
                Location = new Point(0, 0),
                Parent = this,
            };

            NameBackGround = new MirImageControl()//英雄名字
            {
                Index = 10,
                Library = Libraries.Prguse,
                Location = new Point(15, 41),
                Parent = this,
            };

            BarBackGround = new MirImageControl()//英雄的HP MP EXP面板
            {
                Index = 11,
                Library = Libraries.Prguse,
                Location = new Point(46, 8),
                Parent = this,
            };

            HpBar = new MirImageControl()//HP条
            {
                Index = 1951,
                Library = Libraries.Prguse,
                Location = new Point(19, 6),
                Parent = BarBackGround,//隶属于 英雄的HP MP EXP面板
                DrawImage = false,
            };
            HpBar.BeforeDraw += new EventHandler(Bar_BeforeDraw);

            MpBar = new MirImageControl()//MP条
            {
                Index = 1952,
                Library = Libraries.Prguse,
                Location = new Point(19, 19),
                Parent = BarBackGround,//隶属于 英雄的HP MP EXP面板
                DrawImage = false,
            };
            MpBar.BeforeDraw += new EventHandler(Bar_BeforeDraw);

            ExpBar = new MirImageControl()//经验条
            {
                Index = 1953,
                Library = Libraries.Prguse,
                Location = new Point(19, 32),
                Parent = BarBackGround,
                DrawImage = false,
            };
            ExpBar.BeforeDraw += new EventHandler(Bar_BeforeDraw);

            LevelLabel = new MirLabel()//等级条
            {
                AutoSize = true,
                Parent = NameBackGround,
                Location = new Point(5, 0),
                Font = new Font(Settings.FontName, 8F),
            };

            NameLabel = new MirLabel()//名字条
            {
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Parent = NameBackGround,
                Location = new Point(5, 13),
                Size = new Size(90, 16),
                Font = new Font(Settings.FontName, 8F),
            };

            HpLabel = new MirLabel()//HP条
            {
                DrawFormat = TextFormatFlags.Left | TextFormatFlags.VerticalCenter,
                Parent = BarBackGround,
                Location = new Point(15, 1),
                Size = new Size(90, 15),
                Font = new Font(Settings.FontName, 7F),
            };

            MpLabel = new MirLabel()//MP条
            {
                DrawFormat = TextFormatFlags.Left | TextFormatFlags.VerticalCenter,
                Parent = BarBackGround,
                Location = new Point(15, 15),
                Size = new Size(90, 15),
                Font = new Font(Settings.FontName, 7F),
            };

            ExpLabel = new MirLabel()//Exp条
            {
                DrawFormat = TextFormatFlags.Left | TextFormatFlags.VerticalCenter,
                Parent = BarBackGround,
                Location = new Point(15, 29),
                Size = new Size(90, 15),
                Font = new Font(Settings.FontName, 7F),
            };
        }


        public void RefreashInfo()
        {
            UserObject user = GameScene.User;
            if (user != null && GameScene.Scene.HeroSummoned)
            {
                Avatar.Index = (int)((1750 + (int)user.Hero.Gender * 10) + (byte)user.Hero.Class);
                NameLabel.Text = user.Hero.Name.ToString();
                LevelLabel.Text = user.Hero.Level.ToString();
                HpLabel.Text = user.Hero.HP.ToString() + "/" + user.Hero.MaxHP.ToString();

                MpLabel.Text = user.Hero.MP.ToString() + "/" + user.Hero.MaxMP.ToString();
                ExpLabel.Text =
                    $"{(object)((double)GameScene.User.Hero.Experience / (double)GameScene.User.Hero.MaxExperience):#0.##%}";
                int num1 = (int)((double)user.Hero.HP / (double)user.Hero.MaxHP * 100.0);
                int num2 = (int)((double)user.Hero.MP / (double)user.Hero.MaxMP * 100.0);
                if (GameScene.Scene.HeroInventoryDialog == null ||
                    (user.Hero == null || user.Hero.Dead ||
                     (user.Hero.Poison.HasFlag((Enum)PoisonType.Frozen) ||
                      user.Hero.Poison.HasFlag((Enum)PoisonType.Paralysis) ||
                      user.Hero.Poison.HasFlag((Enum)PoisonType.Stun)) ||
                     GameScene.Scene.HeroInventoryDialog.UsePotionTime >= CMain.Time))
                    return;
                GameScene.Scene.HeroInventoryDialog.UsePotionTime = CMain.Time + 2000;
                if (num1 < Settings.HeroHpRate)
                    GameScene.Scene.HeroInventoryDialog.UseHpPotion();
                if (num2 < Settings.HeroMpRate)
                    GameScene.Scene.HeroInventoryDialog.UseMpPotion();
            }
            else
            {
                Avatar.Index = 1379;
                NameLabel.Text = "";
                LevelLabel.Text = "";
                HpLabel.Text = "";
                MpLabel.Text = "";
                ExpLabel.Text = "";
            }
        }

        private void Bar_BeforeDraw(object sender, EventArgs e)
        {
            UserObject user = GameScene.User;
            if (user == null || !GameScene.Scene.HeroSummoned)
                return;
            Rectangle section;
            if (HpBar.Library != null)
            {
                double num = (double)user.Hero.HP / (double)user.Hero.MaxHP;
                if (num > 1.0)
                    num = 1.0;
                if (num <= 0.0)
                    return;
                section = new Rectangle();
                section.Size = new Size((int)((double)(HpBar.Size.Width - 2) * num), HpBar.Size.Height);
                HpBar.Library.Draw(HpBar.Index, section, HpBar.DisplayLocation, Color.White, false);
            }
            if (MpBar.Library != null)
            {
                double num = (double)user.Hero.MP / (double)user.Hero.MaxMP;
                if (num > 1.0)
                    num = 1.0;
                if (num <= 0.0)
                    return;
                section = new Rectangle();
                section.Size = new Size((int)((double)(MpBar.Size.Width - 2) * num), MpBar.Size.Height);
                MpBar.Library.Draw(MpBar.Index, section, MpBar.DisplayLocation, Color.White, false);
            }
            if (ExpBar.Library == null)
                return;
            double num1 = (double)user.Hero.Experience / (double)user.Hero.MaxExperience;
            if (num1 > 1.0)
                num1 = 1.0;
            if (num1 <= 0.0)
                return;
            section = new Rectangle();
            section.Size = new Size((int)((double)(ExpBar.Size.Width - 2) * num1), ExpBar.Size.Height);
            ExpBar.Library.Draw(ExpBar.Index, section, ExpBar.DisplayLocation, Color.White, false);
        }

        public void Show()
        {
            Visible = true;
        }
    }

    public sealed class HeroInventoryDialog : MirImageControl//英雄背包对话框  密封类
    {
        private long UseItemDelay = 700;//使用物品延迟
        private long UseItemTime = 0;//使用物品时间
        public MirImageControl[] LockBar;//锁条 用这个条来锁定格子将格子盖住 需要达到设定的等级或条件才可以解锁
        public MirImageControl[] AutoPotionLockBar;//自动喝药锁条 原理同上
        public MirItemCell[] Grid;
        public MirButton CloseButton;
        public MirButton HpButton;
        public MirButton MpButton;
        public MirLabel HpLabel;
        public MirLabel MpLabel;
        public HeroBeltDialog BeltBar;//英雄腰带(喝药栏)
        public long UsePotionTime;

        public HeroInventoryDialog()//构造方法
        {
            Index = 1422;//英雄背包索引
            Library = Libraries.Prguse;//资源文件
            Movable = true;//可移动
            Sort = true;//分类
            Visible = false;//默认不可见

            CloseButton = new MirButton//关闭英雄背包按钮
            {
                Index = 360,//按钮的索引
                HoverIndex = 361,//按钮浮动的索引
                Location = new Point(303, 1),//位置
                Library = Libraries.Prguse2,//资源文件
                Parent = this,
                PressedIndex = 362,//按钮被按下的索引
                Sound = SoundList.ButtonA,//播放声音
                Hint = "关闭",
            };
            CloseButton.Click += (o, e) => Hide();


            Grid = new MirItemCell[42];//英雄背包格子

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 5; y++)//2层循环遍历英雄背包
                {
                    int idx = 8 * y + x + 2;
                    Grid[idx] = new MirItemCell
                    {
                        ItemSlot = idx,//格子
                        GridType = MirGridType.HeroInventory,//格子类型
                        Library = Libraries.Items,//素材
                        Parent = this,
                        Location = new Point(x * 36 + 13 + x, y % 5 * 32 + 22 + y % 5),//位置
                    };

                    if (idx >= 10)
                        Grid[idx].Visible = false;//如果格子数量大于等于10 遍历到的格子就不可见
                };
            }

            LockBar = new MirImageControl[4];//英雄背包锁条=>盖住格子 一共4个条
            for (int i = 0; i < 4; i++)
            {
                LockBar[i] = new MirImageControl
                {
                    Index = 1423,
                    Library = Libraries.Prguse,
                    Location = new Point(13, 56 + 33 * i),
                    Parent = this,
                    Visible = true,
                };
            }

            AutoPotionLockBar = new MirImageControl[2];//英雄自动喝药锁条
            for (int i = 0; i < 2; i++)
            {
                AutoPotionLockBar[i] = new MirImageControl
                {
                    Index = 1428 + i,
                    Library = Libraries.Prguse,
                    Location = new Point(56 + i * 105, 195),
                    Parent = this,
                    Visible = false,
                };
            }

            HpButton = new MirButton//HP按钮
            {
                Index = 560,//按钮的索引
                HoverIndex = 561,//按钮浮动的索引
                PressedIndex = 562,//按钮被按下的索引
                Location = new Point(58, 206),//位置
                Library = Libraries.Title,//资源文件
                Parent = this,
                Sound = SoundList.ButtonA,//声音
                Hint = "英雄的HP值低于指定的数值时自动使用药水.\n点击按钮可以更改.",
            };

            HpButton.Click += (o, e) =>
            {
                MirAmountBox amountBox = new MirAmountBox("设置英雄的 HP 达到百分之几以下.\n才使用药水?", 1994, 99, 10, 10);//设置HP数量的方块
                amountBox.OKButton.Click += (c, a) =>
                {
                    HpLabel.Text = ((int)amountBox.Amount).ToString() + "%";
                    Settings.HeroHpRate = (int)amountBox.Amount;
                };
                amountBox.Show();
            };

            MpButton = new MirButton//MP按钮
            {
                Index = 563,//按钮的索引
                HoverIndex = 564,//按钮浮动的索引
                PressedIndex = 565,//按钮被按下的索引
                Location = new Point(206, 206),//位置
                Library = Libraries.Title,//资源文件
                Parent = this,
                Sound = SoundList.ButtonA,//声音
                Hint = "英雄的MP值低于指定的数值时自动使用药水.\n点击按钮可以更改.",
            };

            MpButton.Click += (o, e) =>
            {
                MirAmountBox amountBox = new MirAmountBox("设置英雄的 MP 达到百分之几以下.\n才使用药水?", 1994, 99, 10, 10);//设置MP数量的方块
                amountBox.OKButton.Click += (c, a) =>
                {
                    MpLabel.Text = ((int)amountBox.Amount).ToString() + "%";
                    Settings.HeroMpRate = (int)amountBox.Amount;
                };
                amountBox.Show();
            };

            HpLabel = new MirLabel//HP标签
            {
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,//绘画格式
                Location = new Point(58, 227),//位置
                Size = new Size(60, 25),//尺寸
                NotControl = true,//是否控制
                Parent = this,
                Text = Settings.HeroHpRate.ToString() + "%",
            };

            MpLabel = new MirLabel//MP标签
            {
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,//绘画格式
                Location = new Point(206, 227),//位置
                Size = new Size(60, 25),//尺寸
                NotControl = true,//是否控制
                Parent = this,
                Text = Settings.HeroMpRate.ToString() + "%",
            };

            BeltBar = new HeroBeltDialog(Grid)//英雄腰带条(喝药条)
            {
                Parent = GameScene.Scene,
                Visible = false,
            };
        }



        /// <summary>
        /// 使用Hp药水
        /// </summary>
        public void UseHpPotion()
        {
            for (int i = 0; i < Grid.Length; i++)
            {
                if (Grid[i].Item != null && (Grid[i].Item.Info.Type == ItemType.Potion && (int)Grid[i].Item.Info.Shape == 1 && (int)Grid[i].Item.Info.Effect == 1) && ((int)Grid[i].Item.Info.HP != 0 || (uint)Grid[i].Item.Info.HPrate > 0))
                {
                    if (UseItemTime >= CMain.Time)
                        break;
                    UseItemTime = CMain.Time + UseItemDelay;
                    Grid[i].UseItem(true);
                    GameScene.Scene.ChatDialog.ReceiveChat(string.Format("英雄使用 {0}.", (object)Grid[i].Item.FriendlyName), ChatType.System4);
                    break;
                }
            }
        }

        /// <summary>
        /// 使用Mp药水
        /// </summary>
        public void UseMpPotion()
        {
            for (int i = 0; i < Grid.Length; i++)
            {
                if (Grid[i].Item != null && (Grid[i].Item.Info.Type == ItemType.Potion && (int)Grid[i].Item.Info.Shape == 1 && (int)Grid[i].Item.Info.Effect == 1) && ((int)Grid[i].Item.Info.MP != 0 || (uint)Grid[i].Item.Info.MPrate > 0))
                {
                    if (UseItemTime >= CMain.Time)
                        break;
                    UseItemTime = CMain.Time + UseItemDelay;
                    Grid[i].UseItem(true);
                    GameScene.Scene.ChatDialog.ReceiveChat(string.Format("英雄使用 {0}.", (object)Grid[i].Item.FriendlyName), ChatType.System4);
                    break;
                }
            }
        }

        /// <summary>
        /// 刷新英雄背包
        /// </summary>
        public void UpdateInventory()
        {
            if (GameScene.User == null || GameScene.User.Hero == null)
                return;//如果玩家为空 或者 英雄为空 就返回
            ClientHeroInfo clientHeroInfo = GameScene.User.Heros.SingleOrDefault(a => a.Summoned);//获得英雄信息
            if (clientHeroInfo == null)
                return;//如果英雄信息为空 就返回

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 5; y++)//2层循环遍历英雄背包
                {
                    int i = 8 * y + x + 2;
                    Grid[i].Visible = i < (int)clientHeroInfo.InventoryLevel * 8 + 2;//根据英雄背包等级控制格子显示数量
                }
            }
            for (int z = 0; z < LockBar.Length; z++)//遍历锁定的格子
                LockBar[z].Visible = (int)clientHeroInfo.InventoryLevel - 1 <= z;//锁定的格子是否显示
        }

        /// <summary>
        /// 获得格子
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MirItemCell GetCell(ulong id)
        {
            for (int i = 0; i < Grid.Length; i++)
            {
                if (Grid[i].Item != null && (long)Grid[i].Item.UniqueID == (long)id)
                    return Grid[i];
            }
            return null;
        }

        public void Hide()
        {
            Visible = false;
        }

        public void Show()
        {
            if (GameScene.Scene.ChangeHeroDialog.Visible)
                return;
            Visible = true;
            UpdateInventory();
            if (GameScene.Scene.InventoryDialog.Visible)
                Location = TopRight;
            else
                Location = TopLeft;
        }
    }

    public class HeroBeltDialog : MirImageControl//英雄腰带对话框 密封类
    {
        public MirLabel[] Key = new MirLabel[2];//英雄的腰带(喝药栏) 2个空
        public MirButton CloseButton;
        public MirButton RotateButton;//旋转按钮
        public MirItemCell[] Grid;

        /// <summary>
        /// 英雄腰带对话框 喝药的药品栏
        /// </summary>
        /// <param name="grid"></param>
        public HeroBeltDialog(MirItemCell[] grid)//构造方法
        {
            Index = 1921;//索引
            Library = Libraries.Prguse;//资源文件
            Movable = true;//可移动
            Sort = true;//分类
            Visible = false;//默认不可见
            Grid = grid;//格子
            Location = new Point(470, Settings.ScreenHeight - 150);//位置

            BeforeDraw += new EventHandler(BeltPanel_BeforeDraw);//执行绘制方法

            for (int i = 0; i < Key.Length; i++)//遍历腰带
            {
                Key[i] = new MirLabel//创建Label
                {
                    Parent = this,//隶属于英雄腰带对话框
                    Size = new Size(32, 32),//尺寸
                    Location = new Point(11 + i * 36, 3),//位置
                    Text = (i + 7).ToString()//文本
                };
            }

            RotateButton = new MirButton//旋转腰带按钮
            {
                HoverIndex = 1927,
                Index = 1926,
                Location = new Point(82, 3),
                Library = Libraries.Prguse,
                Parent = this,
                PressedIndex = 1928,
                Sound = SoundList.ButtonA,
                Hint = "旋转"
            };
            RotateButton.Click += (o, e) => Flip();

            CloseButton = new MirButton//关闭腰带按钮
            {
                HoverIndex = 1924,
                Index = 1923,
                Location = new Point(82, 19),
                Library = Libraries.Prguse,
                Parent = this,
                PressedIndex = 1925,
                Sound = SoundList.ButtonA,
                Hint = "关闭 (" + CMain.InputKeys.GetKey(KeybindOptions.Belt) + ")"
            };
            CloseButton.Click += (o, e) => Hide();

            for (int i = 0; i < 2; i++)
            {
                Grid[i] = new MirItemCell()
                {
                    ItemSlot = i,
                    GridType = MirGridType.HeroInventory,
                    Library = Libraries.Items,
                    Parent = this,
                    Location = new Point(i * 36 + 11, 3),
                    Size = new Size(32, 32),
                    BackColour = Color.Gray,
                };

                Key[i] = new MirLabel()
                {
                    Parent = this,
                    Size = new Size(32, 32),
                    Location = new Point(i * 36 + 11, 3),
                    Text = (i + 7).ToString(),
                    NotControl = true,
                };
            }
        }

        /// <summary>
        /// 腰带面板绘制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BeltPanel_BeforeDraw(object sender, EventArgs e)
        {
            if (Libraries.Prguse == null)
                return;//如果文件不存在 就返回
            Libraries.Prguse.Draw(Index == 1921 ? 1934 /*横向*/: 1946/*纵向*/, DisplayLocation, Color.White, false, 0.5F);
        }

        /// <summary>
        /// 旋转
        /// </summary>
        public void Flip()
        {
            if (Index == 1921)//如果是英雄腰带
            {
                Index = 1943;//竖起来的索引
                Location = new Point(0, 440);//位置

                for (int x = 0; x < Key.Length; x++)//遍历英雄腰带
                    Grid[x].Location = new Point(3, x * 36 + 11);//英雄腰带格子的位置

                CloseButton.Index = 1935;//关闭按钮的索引
                CloseButton.HoverIndex = 1936;//关闭按钮浮动的索引
                CloseButton.Location = new Point(3, 82);//关闭按钮的位置
                CloseButton.PressedIndex = 1937;//关闭按钮被按下的索引

                RotateButton.Index = 1938;//旋转按钮索引
                RotateButton.HoverIndex = 1939;
                RotateButton.Location = new Point(19, 82);
                RotateButton.PressedIndex = 1940;
            }
            else//如果索引不是英雄腰带的索引
            {
                Index = 1921;
                Location = new Point(470, Settings.ScreenHeight - 150);//位置
                for (int x = 0; x < Key.Length; x++)//遍历英雄腰带
                    Grid[x].Location = new Point(x * 36 + 11, 3);//英雄腰带格子的位置

                CloseButton.Index = 1923;
                CloseButton.HoverIndex = 1924;
                CloseButton.Location = new Point(82, 19);
                CloseButton.PressedIndex = 1925;

                RotateButton.Index = 1926;
                RotateButton.HoverIndex = 1927;
                RotateButton.Location = new Point(82, 3);
                RotateButton.PressedIndex = 1928;
            }

            for (int i = 0; i < Key.Length; i++)//遍历英雄腰带
                Key[i].Location = (Index != 1921) ? new Point(3, i * 36 + 11) : new Point(i * 36 + 11, 3);//设置位置
        }

        public void Hide()
        {
            Visible = false;//隐藏
        }

        public void Show()
        {
            if (GameScene.Scene.ChangeHeroDialog.Visible)
                return;//如果英雄腰带已经显示 就返回
            Visible = true;//显示
        }
    }

    public class ChangeHeroDialog : MirImageControl//改变英雄对话框
    {
        public MirHerosCell[] Cells = new MirHerosCell[9];
        public MirLabel Title;
        public MirButton CloseButton;

        public ChangeHeroDialog()//构造方法
        {
            Index = 1688;
            Library = Libraries.Prguse;
            Movable = true;
            Sort = true;
            Location = new Point(264, 224);

            CloseButton = new MirButton//关闭按钮
            {
                Index = 360,
                HoverIndex = 361,
                PressedIndex = 362,
                Location = new Point(325, 3),
                Library = Libraries.Prguse2,
                Parent = this,
                Sound = SoundList.ButtonA,
                Hint = "关闭",
            };

            CloseButton.Click += (o, e) => Hide();

            for (int i = 0; i < 9; i++)//遍历8个位置
            {
                Cells[i] = new MirHerosCell()
                {
                    HeroInfo = null,
                    Parent = this,
                    Location = new Point(0, 0),
                    Sound = SoundList.ButtonC,
                    Visible = true,
                };

                if (i > 0)
                {
                    Cells[i].IsBorder = true;
                    Cells[i].Location = new Point(100 + (i - 1) % 4 * 60, 63 + (i - 1) / 4 * 41);
                    Cells[i].MouseDown += (o, e) =>
                    {
                        MirHerosCell mirHerosCell = (MirHerosCell)o;
                        if (mirHerosCell.HeroInfo == null || mirHerosCell.HeroInfo.Sealed)//英雄格子的英雄信息==null 或者 英雄格子的英雄信息是密封的
                            return;//退出方法

                        Network.Enqueue(new ChangeHero() { UniqueID = mirHerosCell.HeroInfo.UniqueID });//发送改变英雄数据包
                    };
                }
                else//<=0
                {
                    Cells[i].IsBorder = false;
                    Cells[i].Location = new Point(18, 63);
                }
            }
        }

        /// <summary>
        /// 复位
        /// </summary>
        public void Reset()//更新
        {
            if (GameScene.User == null)
                return;

            Cells[0].HeroInfo = GameScene.User.Heros.SingleOrDefault(a => a.Summoned);
            for (int i = 0; i < 8; i++)
                Cells[i + 1].HeroInfo = GameScene.User.Heros.Count <= i ? null : GameScene.User.Heros[i];
        }

        public void Clear()
        {
        }

        public void Show()//显示 在场景里需要npc
        {
            if (this.Visible)
                return;

            Visible = true;
            Reset();
            if (GameScene.Scene.HeroInfoDialog.Visible)
                GameScene.Scene.HeroInfoDialog.Hide();
            if (GameScene.Scene.HeroInventoryDialog.Visible)
                GameScene.Scene.HeroInventoryDialog.Hide();
            if (!GameScene.Scene.HeroSkillBarDialog.Visible)
                return;
            GameScene.Scene.HeroSkillBarDialog.Hide();
        }

        public void Hide()
        {
            Clear();
            if (!Visible)
                return;
            Visible = false;
        }
    }

    public sealed class HeroSkillBarDialog : MirImageControl//英雄技能条对话框
    {
        public MirImageControl[] Cells = new MirImageControl[7];//技能条
        public MirLabel[] KeyNameLabels = new MirLabel[7];//按键名Label
        public MirAnimatedControl[] CoolDowns = new MirAnimatedControl[7];//技能冷却时间
        public MirControl SkillLabel;//技能Label

        public HeroSkillBarDialog()//构造方法
        {
            Index = 2192;
            Library = Libraries.Prguse;
            Movable = true;
            Sort = true;
            Location = new Point(217, 0);
            Visible = false;

            BeforeDraw += new EventHandler(MagicKeyDialog_BeforeDraw);

            for (int i = 0; i < Cells.Length; i++)//遍历7个技能框
            {
                Cells[i] = new MirImageControl()
                {
                    Index = -1,
                    Library = Libraries.MagIcon,
                    Parent = (MirControl)this,
                    Location = new Point(i * 25 + 3, 3),
                };

                CoolDowns[i] = new MirAnimatedControl()
                {
                    Library = Libraries.Prguse2,
                    Parent = (MirControl)this,
                    Location = new Point(i * 25 + 3, 3),
                    NotControl = true,
                    UseOffSet = true,
                    Loop = false,
                    Animated = false,
                    Opacity = 0.600000023841858F,
                };
            }

            for (int i = 0; i < KeyNameLabels.Length; i++)
            {
                KeyNameLabels[i] = new MirLabel()
                {
                    Text = "Shift   F" + (i + 1).ToString(),
                    ForeColour = Color.White,
                    Parent = this,
                    Location = new Point(i * 25 + 2, 2),
                    Size = new Size(25, 25),
                    NotControl = true,
                };
            }
        }

        private void MagicKeyDialog_BeforeDraw(object sender, EventArgs e)
        {
            Libraries.Prguse.Draw(2194, new Point(DisplayLocation.X + 3, DisplayLocation.Y), Color.White, true, 0.5F);
        }

        public void Update()
        {
            if (!Visible || GameScene.User.Hero == null) return;

            for (int i = 0; i < Cells.Length; i++)
            {
                Cells[i].Index = -1;
                int num = 0;
                foreach (ClientMagic magic1 in GameScene.User.Hero.Magics)
                {
                    if ((int)magic1.Key == i + 1 + num)
                    {
                        ClientMagic magic2 = MapObject.User.Hero.GetMagic(magic1.Spell);
                        if (magic2 != null)
                        {
                            string str = string.Format("Shift+F{0}", (object)magic1.Key);
                            Cells[i].Index = (int)magic2.Icon * 2;
                            Cells[i].Hint = string.Format("[{0}] {1}\n{2} 冷却时间 {3}", magic2.Spell, str, magic2.ToString(), Functions.PrintTimeSpanFromMilliSeconds((double)magic2.Delay));
                            KeyNameLabels[i].Text = "";
                        }
                    }
                }
                CoolDowns[i].Dispose();
            }
        }

        public void CreateLabel(ClientMagic magic, string key)
        {
            if (SkillLabel != null)
            {
                SkillLabel.Dispose();
                SkillLabel = (MirControl)null;
            }
            MirControl mirControl = new MirControl();
            mirControl.BackColour = Color.FromArgb((int)byte.MaxValue, 0, 0, 0);
            int num1 = 1;
            mirControl.Border = num1 != 0;
            Color gray = Color.Gray;
            mirControl.BorderColour = gray;
            int num2 = 1;
            mirControl.DrawControlTexture = num2 != 0;
            int num3 = 1;
            mirControl.NotControl = num3 != 0;
            mirControl.Parent = (MirControl)this;
            double num4 = 0.300000011920929;
            mirControl.Opacity = (float)num4;
            SkillLabel = mirControl;
            MirLabel mirLabel1 = new MirLabel();
            mirLabel1.AutoSize = true;
            Color white1 = Color.White;
            mirLabel1.ForeColour = white1;
            Point point1 = new Point(4, 4);
            mirLabel1.Location = point1;
            int num5 = 1;
            mirLabel1.OutLine = num5 != 0;
            Color color1 = Color.FromArgb((int)byte.MaxValue, 70, 70, 70);
            mirLabel1.OutLineColour = color1;
            MirControl skillLabel1 = SkillLabel;
            mirLabel1.Parent = skillLabel1;
            string str1 = "[" + (magic.Spell) + "] ";
            mirLabel1.Text = str1;
            MirLabel mirLabel2 = mirLabel1;
            SkillLabel.Size = new Size(Math.Max(SkillLabel.Size.Width, mirLabel2.DisplayRectangle.Right + 4), Math.Max(SkillLabel.Size.Height, mirLabel2.DisplayRectangle.Bottom));
            MirLabel mirLabel3 = new MirLabel();
            mirLabel3.AutoSize = true;
            Color white2 = Color.White;
            mirLabel3.ForeColour = white2;
            Point point2 = new Point(mirLabel2.DisplayRectangle.Right, 4);
            mirLabel3.Location = point2;
            int num6 = 1;
            mirLabel3.OutLine = num6 != 0;
            Color color2 = Color.FromArgb((int)byte.MaxValue, 70, 70, 70);
            mirLabel3.OutLineColour = color2;
            MirControl skillLabel2 = SkillLabel;
            mirLabel3.Parent = skillLabel2;
            string str2 = key;
            mirLabel3.Text = str2;
            MirLabel mirLabel4 = mirLabel3;
            SkillLabel.Size = new Size(Math.Max(SkillLabel.Size.Width, mirLabel4.DisplayRectangle.Right + 4), Math.Max(SkillLabel.Size.Height, mirLabel4.DisplayRectangle.Bottom));
        }

        public void Process()
        {
            ProcessSkillDelay();
        }

        private void ProcessSkillDelay()
        {
            if (!Visible || GameScene.User.Hero == null)
                return;
            int num1 = 0;
            for (int index1 = 0; index1 < Cells.Length; ++index1)
            {
                foreach (ClientMagic magic in GameScene.User.Hero.Magics)
                {
                    if ((int)magic.Key == index1 + num1 + 1)
                    {
                        int num2 = 22;
                        long num3 = magic.CastTime + magic.Delay - CMain.Time;
                        if (num3 < 100L || CoolDowns[index1] != null && CoolDowns[index1].Animated)
                        {
                            if (num3 > 0L)
                                CoolDowns[index1].Dispose();
                            else
                                continue;
                        }
                        int num4 = (int)(magic.Delay / (long)num2);
                        int num5 = num2 - (int)(num3 / (long)num4);
                        if (CMain.Time <= magic.CastTime + magic.Delay && magic.CastTime > 0L)
                        {
                            CoolDowns[index1].Dispose();
                            MirAnimatedControl[] coolDowns = CoolDowns;
                            int index2 = index1;
                            MirAnimatedControl mirAnimatedControl1 = new MirAnimatedControl();
                            int num6 = 1260 + num5;
                            mirAnimatedControl1.Index = num6;
                            int num7 = num2 - num5;
                            mirAnimatedControl1.AnimationCount = num7;
                            long num8 = (long)num4;
                            mirAnimatedControl1.AnimationDelay = num8;
                            MLibrary prguse2 = Libraries.Prguse2;
                            mirAnimatedControl1.Library = prguse2;
                            mirAnimatedControl1.Parent = (MirControl)this;
                            Point point = new Point(index1 * 25 + 3, 3);
                            mirAnimatedControl1.Location = point;
                            int num9 = 1;
                            mirAnimatedControl1.NotControl = num9 != 0;
                            int num10 = 1;
                            mirAnimatedControl1.UseOffSet = num10 != 0;
                            int num11 = 0;
                            mirAnimatedControl1.Loop = num11 != 0;
                            int num12 = 1;
                            mirAnimatedControl1.Animated = num12 != 0;
                            double num13 = 0.600000023841858;
                            mirAnimatedControl1.Opacity = (float)num13;
                            coolDowns[index2] = mirAnimatedControl1;
                            CoolDowns[index1].AfterAnimation += (EventHandler)((o, e) =>
                            {
                                MirAnimatedControl EndEffect = new MirAnimatedControl
                                {
                                    Index = 1240,
                                    AnimationCount = 6,
                                    AnimationDelay = 100L,
                                    Library = Libraries.Prguse2,
                                    Parent = this,
                                    Location = ((MirControl)o).Location,
                                    NotControl = true,
                                    UseOffSet = true,
                                    Loop = false,
                                    Animated = true,
                                    Blending = true,
                                    ForeColour = Color.Goldenrod,
                                };
                                EndEffect.AfterAnimation += ((o1, e1) => EndEffect.Dispose());
                            });
                        }
                    }
                }
            }
        }

        public void Show()
        {
            if (Visible || GameScene.User.Hero == null) return;
            Visible = true;
            Update();
        }

        public void Hide()
        {
            if (!Visible) return;
            Visible = false;
        }
    }

    public sealed class MirHerosCell : MirControl//英雄格子
    {
        public ClientHeroInfo HeroInfo;
        public bool IsBorder;
        public static MirControl Label;

        public MirHerosCell()//构造方法
        {
            Size = new Size(55, 37);
            BorderColour = Color.Lime;
            BeforeDraw += (o, e) => Update();
            AfterDraw += (o, e) => DrawItem();
        }

        private void Update()
        {
            if (Label == null || Label.IsDisposed)
                return;

            Label.BringToFront();
            int x = CMain.MPoint.X + 15;
            int y = CMain.MPoint.Y;

            if (x + Label.Size.Width > Settings.ScreenWidth)
            {
                x = Settings.ScreenWidth - Label.Size.Width;
            }

            if (y + Label.Size.Height > Settings.ScreenHeight)
            {
                y = Settings.ScreenHeight - Label.Size.Height;
            }

            Label.Location = new Point(x, y);
        }

        protected override void OnMouseEnter()
        {
            base.OnMouseEnter();
            Border = IsBorder;
        }

        protected override void OnMouseLeave()
        {
            base.OnMouseLeave();
            Border = false;
            DisposeLabel();
        }

        public override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
        }

        public override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            CreateLabel();
        }

        private void DrawItem()
        {
            if (HeroInfo == null)
                return;

            int index = (int)((byte)(1770 + (int)HeroInfo.Gender * 10) + HeroInfo.Class);
            Point point = new Point((40 - Libraries.Prguse.GetTrueSize(index).Width) / 2, (39 - Libraries.Prguse.GetTrueSize(index).Height) / 2);
            Libraries.Prguse.Draw(index, new Point(point.X + DisplayLocation.X, point.Y + DisplayLocation.Y), HeroInfo.Sealed ? Color.DarkRed : Color.White, false);
        }

        private void CreateLabel()
        {
            if (HeroInfo == null)
            {
                DisposeLabel();
            }
            else
            {
                DisposeLabel();

                Label = new MirControl
                {
                    BackColour = Color.FromArgb(255, 0, 0, 0),
                    Border = true,
                    BorderColour = Color.Gray,
                    DrawControlTexture = true,
                    NotControl = true,
                    Parent = GameScene.Scene,
                    Opacity = 0.300000011920929F,
                };

                MirLabel mirLabel2 = new MirLabel()
                {
                    AutoSize = true,
                    ForeColour = Color.White,
                    Location = new Point(4, 4),
                    OutLine = true,
                    OutLineColour = Color.FromArgb(255, 70, 70, 70),
                    Parent = Label,
                    Text = "名字 : " + HeroInfo.Name + "\n等级 : " + HeroInfo.Level + "\n职业 : " + HeroInfo.Class + "\n状态 : " + (HeroInfo.Sealed ? "封印" : (HeroInfo.Summoned ? "召出" : "召回")) + "\n",
                };

                Label.Size = new Size(Math.Max(Label.Size.Width, mirLabel2.DisplayRectangle.Right + 4), Math.Max(Label.Size.Height, mirLabel2.DisplayRectangle.Bottom));
            }
        }

        public void DisposeLabel()
        {
            if (Label != null && !Label.IsDisposed)
                Label.Dispose();

            Label = null;
        }
    }

    public sealed class HeroInfoDialog : MirImageControl//英雄信息对话框
    {
        public MirButton CloseButton;
        public MirButton CharacterButton;
        public MirButton StatusButton;
        public MirButton StateButton;
        public MirButton SkillButton;
        public MirImageControl CharacterPage;
        public MirImageControl StatusPage;
        public MirImageControl StatePage;
        public MirImageControl SkillPage;
        public MirImageControl ClassImage;
        public MirLabel NameLabel;
        public MirLabel GuildLabel;
        public MirLabel LoverLabel;
        public MirLabel ACLabel;
        public MirLabel MACLabel;
        public MirLabel DCLabel;
        public MirLabel MCLabel;
        public MirLabel SCLabel;
        public MirLabel HealthLabel;
        public MirLabel ManaLabel;
        public MirLabel StatuspageHeaderLabel;
        public MirLabel StatuspageDataLabel;
        public MirLabel HeadingLabel;
        public MirLabel StatLabel;
        public MirButton NextButton;
        public MirButton BackButton;
        public MirItemCell[] Grid;
        public MagicButton[] Magics;
        public int StartIndex;
        public MirLabel CritRLabel, CritDLabel, LuckLabel, AttkSpdLabel, AccLabel, AgilLabel;
        public MirLabel ExpPLabel/*英雄对话框内的经验值*/, BagWLabel, WearWLabel, HandWLabel, MagicRLabel, PoisonRecLabel, HealthRLabel, ManaRLabel, PoisonResLabel, HolyTLabel, FreezeLabel, PoisonAtkLabel;
        bool first = true;

        public HeroInfoDialog()
        {
            Index = 505;
            Library = Libraries.Title;
            Location = new Point(Settings.ScreenWidth - 264, 0);
            Movable = true;
            Sort = true;

            BeforeDraw += (o, e) => RefreshInferface();

            CharacterPage = new MirImageControl
            {
                Index = 340,
                Parent = this,
                Library = Libraries.Prguse,
                Location = new Point(8, 90),
            };
            CharacterPage.AfterDraw += (o, e) =>
            {
                if (Libraries.StateItems == null)
                    return;
                ItemInfo itemInfo = null;
                if (Grid[1].Item != null)//盔甲
                {
                    if (GameScene.User.Hero.WingEffect == 1 || GameScene.User.Hero.WingEffect == 2)
                    {
                        int wingOffset = GameScene.User.Hero.WingEffect == 1 ? 2 : 4;//翅膀设置取值

                        int genderOffset = GameScene.User.Hero.Gender == MirGender.Male ? 0 : 1;//性别设置取值

                        Libraries.Prguse2.DrawBlend(1200 + wingOffset + genderOffset, DisplayLocation, Color.White, true, 1F);//人物穿上盔甲的颜色
                    }

                    itemInfo = Functions.GetRealItem(Grid[1].Item.Info, GameScene.User.Hero.Level, GameScene.User.Hero.Class, GameScene.ItemInfoList);
                    Libraries.StateItems.Draw(Grid[1].Item.Image, DisplayLocation, Color.White, true, 1F);//人物穿上盔甲的颜色
                }
                if (Grid[0].Item != null)
                {
                    itemInfo = Functions.GetRealItem(Grid[0].Item.Info, GameScene.User.Hero.Level, GameScene.User.Hero.Class, GameScene.ItemInfoList);
                    Libraries.StateItems.Draw(Grid[0].Item.Image, DisplayLocation, Color.White, true, 1F);//武器拿在手里的颜色
                }
                if (Grid[2].Item != null)
                {
                    Libraries.StateItems.Draw(Grid[2].Item.Info.Image, DisplayLocation, Color.White, true, 1F);//人物穿上头盔的颜色
                }
                else
                {
                    int hair = 441 + (int)GameScene.User.Hero.Hair + (GameScene.User.Hero.Gender == MirGender.Male ? 0 : 40);

                    Libraries.Prguse.Draw(hair, new Point(DisplayLocation.X, DisplayLocation.Y), Color.White, true, 1F);
                }
            };

            StatusPage = new MirImageControl
            {
                Index = 506,
                Parent = this,
                Library = Libraries.Title,
                Location = new Point(8, 90),
                Visible = false,
            };
            StatusPage.BeforeDraw += (o, e) =>
            {
                //-----------------------------------------------------------------------------英雄各属性值显示 STATS I----------------------------------------------------------
                ACLabel.Text = string.Format("防御 {0}-{1}", GameScene.User.Hero.MinAC, GameScene.User.Hero.MaxAC);
                MACLabel.Text = string.Format("魔御 {0}-{1}", GameScene.User.Hero.MinMAC, GameScene.User.Hero.MaxMAC);
                DCLabel.Text = string.Format("攻击 {0}-{1}", GameScene.User.Hero.MinDC, GameScene.User.Hero.MaxDC);
                MCLabel.Text = string.Format("魔法 {0}-{1}", GameScene.User.Hero.MinMC, GameScene.User.Hero.MaxMC);
                SCLabel.Text = string.Format("道术 {0}-{1}", GameScene.User.Hero.MinSC, GameScene.User.Hero.MaxSC);
                HealthLabel.Text = string.Format("生命值 {0}/{1}", GameScene.User.Hero.HP, GameScene.User.Hero.MaxHP);
                ManaLabel.Text = string.Format("魔法值 {0}/{1}", GameScene.User.Hero.MP, GameScene.User.Hero.MaxMP);
                CritRLabel.Text = string.Format("暴击率 {0}%", GameScene.User.Hero.CriticalRate);
                CritDLabel.Text = string.Format("暴击伤害 {0}", GameScene.User.Hero.CriticalDamage);
                AttkSpdLabel.Text = string.Format("攻击速度 {0}", GameScene.User.Hero.ASpeed);
                AccLabel.Text = string.Format("准确 +{0}", GameScene.User.Hero.Accuracy);
                AgilLabel.Text = string.Format("敏捷 +{0}", GameScene.User.Hero.Agility);
                LuckLabel.Text = string.Format("幸运 {0}", GameScene.User.Hero.Luck);
                //----------------------------------------------------------------------------------------------------------------------------------------------------------------
            };

            StatePage = new MirImageControl
            {
                Index = 507,
                Parent = this,
                Library = Libraries.Title,
                Location = new Point(8, 90),
                Visible = false,
            };
            StatePage.BeforeDraw += (o, e) =>
            {
                //-----------------------------------------------------------------------------英雄各属性值显示 STATS II----------------------------------------------------------
                ExpPLabel.Text = string.Format("经验 {0:0.##%}", (double)GameScene.User.Hero.Experience / (double)GameScene.User.Hero.MaxExperience);
                BagWLabel.Text = string.Format("背包负重 {0}/{1}", GameScene.User.Hero.CurrentBagWeight, GameScene.User.Hero.MaxBagWeight);
                WearWLabel.Text = string.Format("穿戴负重 {0}/{1}", GameScene.User.Hero.CurrentWearWeight, GameScene.User.Hero.MaxWearWeight);
                HandWLabel.Text = string.Format("腕力 {0}/{1}", GameScene.User.Hero.CurrentHandWeight, GameScene.User.Hero.MaxHandWeight);
                MagicRLabel.Text = string.Format("魔法躲避 +{0}", GameScene.User.Hero.MagicResist);
                PoisonResLabel.Text = string.Format("毒物躲避 +{0}", GameScene.User.Hero.PoisonResist);
                HealthRLabel.Text = string.Format("HP恢复 +{0}", GameScene.User.Hero.HealthRecovery);
                ManaRLabel.Text = string.Format("MP恢复 +{0}", GameScene.User.Hero.SpellRecovery);
                PoisonRecLabel.Text = string.Format("中毒恢复 +{0}", GameScene.User.Hero.PoisonRecovery);
                HolyTLabel.Text = string.Format("神圣 +{0}", GameScene.User.Hero.Holy);
                FreezeLabel.Text = string.Format("减速 +{0}", GameScene.User.Hero.Freezing);
                PoisonAtkLabel.Text = string.Format("毒 +{0}", GameScene.User.Hero.PoisonAttack);
                //----------------------------------------------------------------------------------------------------------------------------------------------------------------
            };

            SkillPage = new MirImageControl
            {
                Index = 508,//技能框架
                Parent = this,
                Library = Libraries.Title,
                Location = new Point(8, 90),
                Visible = false,
            };

            CharacterButton = new MirButton
            {
                Index = 500,
                Library = Libraries.Title,
                Location = new Point(8, 70),
                Parent = this,
                PressedIndex = 500,//char  
                Size = new Size(64, 20),
                Sound = SoundList.ButtonA,
                Hint = "英雄角色",
            };
            CharacterButton.Click += (o, e) => ShowCharacterPage();

            StatusButton = new MirButton
            {
                Library = Libraries.Title,
                Location = new Point(70, 70),
                Parent = this,
                PressedIndex = 501,//状态1 stats1
                Size = new Size(64, 20),
                Sound = SoundList.ButtonA,
                Hint = "英雄属性",
            };
            StatusButton.Click += (o, e) => ShowStatusPage();

            StateButton = new MirButton
            {
                Library = Libraries.Title,
                Location = new Point(132, 70),
                Parent = this,
                PressedIndex = 502,//状态2 stats2
                Size = new Size(64, 20),
                Sound = SoundList.ButtonA,
                Hint = "英雄状况",
            };
            StateButton.Click += (o, e) => ShowStatePage();

            SkillButton = new MirButton
            {
                Library = Libraries.Title,
                Location = new Point(194, 70),
                Parent = this,
                PressedIndex = 503,//状态3 stats3
                Size = new Size(64, 20),
                Sound = SoundList.ButtonA,
                Hint = "英雄技能",
            };
            SkillButton.Click += (o, e) => ShowSkillPage();

            CloseButton = new MirButton
            {
                HoverIndex = 361,
                Index = 360, //关闭人物栏 左上角按钮
                Location = new Point(241, 3),
                Library = Libraries.Prguse2,
                Parent = this,
                PressedIndex = 362,
                Sound = SoundList.ButtonA,
                Hint = "关闭"
            };
            CloseButton.Click += (o, e) => Hide();

            NameLabel = new MirLabel
            {
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Parent = this,
                Location = new Point(50, 12),
                Size = new Size(190, 20),
                NotControl = true,
            };

            GuildLabel = new MirLabel
            {
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Parent = this,
                Location = new Point(50, 33),
                Size = new Size(190, 30),
                NotControl = true,
            };

            ClassImage = new MirImageControl
            {
                Index = 100,//一把剑 武士的符号？
                Library = Libraries.Prguse,
                Location = new Point(15, 33),
                Parent = this,
                NotControl = true,
            };

            Grid = new MirItemCell[Enum.GetNames(typeof(EquipmentSlot)).Length];

            Grid[0] = new MirItemCell//武器格子
            {
                ItemSlot = 0,
                GridType = MirGridType.HeroEquipment,
                Parent = CharacterPage,
                Location = new Point(125, 7),
            };

            Grid[1] = new MirItemCell//盔甲格子
            {
                ItemSlot = 1,
                GridType = MirGridType.HeroEquipment,
                Parent = CharacterPage,
                Location = new Point(164, 7),
            };

            Grid[2] = new MirItemCell//头盔格子
            {
                ItemSlot = 2,
                GridType = MirGridType.HeroEquipment,
                Parent = CharacterPage,
                Location = new Point(203, 7),
            };

            Grid[3] = new MirItemCell//火把格子
            {
                ItemSlot = 3,
                GridType = MirGridType.HeroEquipment,
                Parent = CharacterPage,
                Location = new Point(203, 134),
            };

            Grid[4] = new MirItemCell//项链格子
            {
                ItemSlot = 4,
                GridType = MirGridType.HeroEquipment,
                Parent = CharacterPage,
                Location = new Point(203, 98),
            };

            Grid[5] = new MirItemCell//右手镯格子
            {
                ItemSlot = 5,
                GridType = MirGridType.HeroEquipment,
                Parent = CharacterPage,
                Location = new Point(8, 170),
            };

            Grid[6] = new MirItemCell//左手镯格子
            {
                ItemSlot = 6,
                GridType = MirGridType.HeroEquipment,
                Parent = CharacterPage,
                Location = new Point(203, 170),
            };

            Grid[7] = new MirItemCell//右戒指格子
            {
                ItemSlot = 7,
                GridType = MirGridType.HeroEquipment,
                Parent = CharacterPage,
                Location = new Point(8, 206),
            };


            Grid[8] = new MirItemCell//左戒指格子
            {
                ItemSlot = 8,
                GridType = MirGridType.HeroEquipment,
                Parent = CharacterPage,
                Location = new Point(203, 206),
            };

            Grid[9] = new MirItemCell//护身符或毒格子
            {
                ItemSlot = 9,
                GridType = MirGridType.HeroEquipment,
                Parent = CharacterPage,
                Location = new Point(8, 241),
            };

            Grid[10] = new MirItemCell//腰带格子
            {
                ItemSlot = 10,
                GridType = MirGridType.HeroEquipment,
                Parent = CharacterPage,
                Location = new Point(86, 241),
            };

            Grid[11] = new MirItemCell//靴子格子
            {
                ItemSlot = 11,
                GridType = MirGridType.HeroEquipment,
                Parent = CharacterPage,
                Location = new Point(47, 241),
            };

            Grid[12] = new MirItemCell//守护石格子
            {
                ItemSlot = 12,
                GridType = MirGridType.HeroEquipment,
                Parent = CharacterPage,
                Location = new Point(125, 241),
            };

            Grid[13] = new MirItemCell//坐骑格子
            {
                ItemSlot = 13,
                GridType = MirGridType.HeroEquipment,
                Parent = CharacterPage,
                Location = new Point(203, 62),
            };

            // STATS I
            HealthLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(126, 20),
                NotControl = true,
                Text = "0-0",
                ForeColour = Color.Yellow,
            };

            ManaLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(126, 38),
                NotControl = true,
                Text = "0-0",
                ForeColour = Color.Yellow,
            };

            ACLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(126, 56),
                NotControl = true,
                Text = "0-0",
                ForeColour = Color.Yellow,
            };

            MACLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(126, 74),
                NotControl = true,
                Text = "0-0",
                ForeColour = Color.Yellow,
            };
            DCLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(126, 92),
                NotControl = true,
                Text = "0-0",
                ForeColour = Color.Yellow,
            };
            MCLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(126, 110),
                NotControl = true,
                Text = "0/0",
                ForeColour = Color.Yellow,
            };
            SCLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(126, 128),
                NotControl = true,
                Text = "0/0",
                ForeColour = Color.Yellow,
            };
            //Breezer - New Labels
            CritRLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(126, 146),
                NotControl = true,
                ForeColour = Color.Yellow,
            };
            CritDLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(126, 164),
                NotControl = true,
                ForeColour = Color.Yellow,
            };
            AttkSpdLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(126, 182),
                NotControl = true,
                ForeColour = Color.Yellow,
            };
            AccLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(126, 200),
                NotControl = true,
                ForeColour = Color.Yellow,
            };
            AgilLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(126, 218),
                NotControl = true,
                ForeColour = Color.Yellow,
            };
            LuckLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(126, 236),
                NotControl = true,
                ForeColour = Color.Yellow,
            };

            // STATS II 
            ExpPLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatePage,
                Location = new Point(126, 20),
                NotControl = true,
                Text = "0-0",
                ForeColour = Color.DodgerBlue,
            };

            BagWLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatePage,
                Location = new Point(126, 38),
                NotControl = true,
                Text = "0-0",
                ForeColour = Color.DodgerBlue,
            };

            WearWLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatePage,
                Location = new Point(126, 56),
                NotControl = true,
                Text = "0-0",
                ForeColour = Color.DodgerBlue,
            };

            HandWLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatePage,
                Location = new Point(126, 74),
                NotControl = true,
                Text = "0-0",
                ForeColour = Color.DodgerBlue,
            };
            MagicRLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatePage,
                Location = new Point(126, 92),
                NotControl = true,
                Text = "0-0",
                ForeColour = Color.DodgerBlue,
            };
            PoisonResLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatePage,
                Location = new Point(126, 110),
                NotControl = true,
                Text = "0/0",
                ForeColour = Color.DodgerBlue,
            };
            HealthRLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatePage,
                Location = new Point(126, 128),
                NotControl = true,
                Text = "0/0",
                ForeColour = Color.DodgerBlue,
            };
            //Breezer
            ManaRLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatePage,
                Location = new Point(126, 146),
                NotControl = true,
                ForeColour = Color.DodgerBlue,
            };
            PoisonRecLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatePage,
                Location = new Point(126, 164),
                NotControl = true,
                ForeColour = Color.DodgerBlue,
            };
            HolyTLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatePage,
                Location = new Point(126, 182),
                NotControl = true,
                ForeColour = Color.DodgerBlue,
            };
            FreezeLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatePage,
                Location = new Point(126, 200),
                NotControl = true,
                ForeColour = Color.DodgerBlue,
            };
            PoisonAtkLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatePage,
                Location = new Point(126, 218),
                NotControl = true,
                ForeColour = Color.DodgerBlue,
            };

            Magics = new MagicButton[7];
            for (int i = 0; i < Magics.Length; i++)//这里开始向英雄添加技能页内的按钮
                Magics[i] = new MagicButton { Parent = SkillPage, Visible = false, Location = new Point(8, 8 + i * 33) };//开始添加各种按钮

            NextButton = new MirButton
            {
                Index = 396,
                Location = new Point(140, 250),
                Library = Libraries.Prguse,
                Parent = SkillPage,
                PressedIndex = 397,
                Sound = SoundList.ButtonA,
                Hint = "下一页",
            };
            NextButton.Click += (o, e) =>
            {
                if (StartIndex + 7 >= GameScene.User.Hero.Magics.Count) return;

                StartIndex += 7;
                RefreshInferface();

                ClearCoolDowns();//消除技能冷却
            };

            BackButton = new MirButton
            {
                Index = 398,
                Location = new Point(90, 250),
                Library = Libraries.Prguse,
                Parent = SkillPage,
                PressedIndex = 399,
                Sound = SoundList.ButtonA,
                Hint = "上一页",
            };
            BackButton.Click += (o, e) =>
            {
                if (StartIndex - 7 < 0) return;

                StartIndex -= 7;
                RefreshInferface();

                ClearCoolDowns();//消除技能冷却
            };
        }

        public void Hide()
        {
            if (!Visible)
                return;
            Visible = false;
        }

        public void Show()
        {
            if (GameScene.Scene.ChangeHeroDialog.Visible || Visible)
                return;
            Visible = true;

            ClearCoolDowns();//消除技能冷却
        }

        public void ShowCharacterPage()
        {
            CharacterPage.Visible = true;
            StatusPage.Visible = false;
            StatePage.Visible = false;
            SkillPage.Visible = false;
            CharacterButton.Index = 500;
            StatusButton.Index = -1;
            StateButton.Index = -1;
            SkillButton.Index = -1;
        }

        private void ShowStatusPage()
        {
            CharacterPage.Visible = false;
            StatusPage.Visible = true;
            StatePage.Visible = false;
            SkillPage.Visible = false;
            CharacterButton.Index = -1;
            StatusButton.Index = 501;
            StateButton.Index = -1;
            SkillButton.Index = -1;
        }

        private void ShowStatePage()
        {
            CharacterPage.Visible = false;
            StatusPage.Visible = false;
            StatePage.Visible = true;
            SkillPage.Visible = false;
            CharacterButton.Index = -1;
            StatusButton.Index = -1;
            StateButton.Index = 502;
            SkillButton.Index = -1;
        }

        public void ShowSkillPage()//显示技能页
        {
            CharacterPage.Visible = false;
            StatusPage.Visible = false;
            StatePage.Visible = false;
            SkillPage.Visible = true;
            CharacterButton.Index = -1;
            StatusButton.Index = -1;
            StateButton.Index = -1;
            SkillButton.Index = 503;
            StartIndex = 0;

            if (first)
            {
                if (GameScene.User.Hero.Magics.Count > Magics.Length)//大于7 用于技能学习数量>7或技能全学
                {
                    for (int i = 0; i < Magics.Length; i++)
                    {
                        Magics[i].Magic = GameScene.User.Hero.Magics[i];//赋给技能按钮
                    }
                }
                else//学习的技能数量<=7
                {
                    for (int i = 0; i < GameScene.User.Hero.Magics.Count; i++)//遍历玩家技能的真实长度
                    {
                        Magics[i].Magic = GameScene.User.Hero.Magics[i];//赋给技能按钮
                    }
                }
                first = false;
            }

            ClearCoolDowns();//消除技能冷却
            for (int i = 0; i < Magics.Length; i++)
            {
                Magics[i].Hint = Magics[i].Magic.ToString();
            }
        }

        private void RefreshInferface()//刷新角色界面
        {
            int offSet = GameScene.User.Hero.Gender == MirGender.Male ? 0 : 1;
            Index = 505;
            CharacterPage.Index = 340 + offSet;
            switch (GameScene.User.Hero.Class)
            {
                case MirClass.Warrior:
                    ClassImage.Index = 100;
                    break;
                case MirClass.Wizard:
                    ClassImage.Index = 101;
                    break;
                case MirClass.Taoist:
                    ClassImage.Index = 102;
                    break;
                case MirClass.Assassin:
                    ClassImage.Index = 103;
                    break;
                case MirClass.Archer:
                    ClassImage.Index = 104;
                    break;
            }
            NameLabel.Text = GameScene.User.Hero.Name;
            GuildLabel.Text = GameScene.User.Hero.GuildName + " " + GameScene.User.Hero.GuildRankName;
            for (int i = 0; i < Magics.Length; i++)
            {
                if (i + StartIndex >= GameScene.User.Hero.Magics.Count)
                {
                    Magics[i].Visible = false;
                }
                else
                {
                    Magics[i].Visible = true;
                    Magics[i].Update(GameScene.User.Hero.Magics[i + StartIndex]);
                }
            }
        }

        public MirItemCell GetCell(ulong id)
        {
            for (int i = 0; i < Grid.Length; i++)
            {
                if (Grid[i].Item != null && Grid[i].Item.UniqueID == id)
                    return Grid[i];
            }
            return null;
        }

        private void ClearCoolDowns()//消除技能冷却
        {
            for (int i = 0; i < Magics.Length; i++)
            {
                Magics[i].CoolDown.Dispose();
            }
        }
    }

    public sealed class HeroCommandBar : MirImageControl//英雄命令栏
    {
        public byte AiIndex = 1;
        public MirButton[] AIModeButton;
        public MirButton[] SkillButton;

        public HeroCommandBar(MirControl parent)//构造方法
        {
            Index = 140;
            Library = Libraries.Prguse2;
            Movable = true;//可移动
            Sort = true;//整理、排序
            Location = new Point(Settings.ScreenWidth / 2 - 249, Settings.ScreenHeight - 138);//帮助条的位置
            Parent = GameScene.Scene;

            #region AI条(一共4个按钮位于最下方)
            AIModeButton = new MirButton[4];

            AIModeButton[0] = new MirButton()//攻击型按钮
            {
                Index = 1840,
                HoverIndex = 1844,//悬浮索引
                PressedIndex = 1844,//按下索引
                Library = Libraries.Prguse,
                Location = new Point(Settings.ScreenWidth / 2 - parent.Size.Width / 2 + 159, Settings.ScreenHeight - 114),//与下一个按钮的距离X轴相差18像素 170-15
                Parent = GameScene.Scene,
                Sound = SoundList.ButtonA,
                Hint = "攻击型AI\n视野内发现敌人便会攻击.",
                Visible = false,//暂时显示 下同
            };
            AIModeButton[0].Click += (o, e) => ActiveAI(1);

            AIModeButton[1] = new MirButton()//反击型按钮
            {
                Index = 1841,//索引
                HoverIndex = 1845,//悬浮索引
                PressedIndex = 1845,//按下索引
                Library = Libraries.Prguse,
                Location = new Point(Settings.ScreenWidth / 2 - parent.Size.Width / 2 + 177, Settings.ScreenHeight - 114),
                Parent = GameScene.Scene,
                Sound = SoundList.ButtonA,
                Hint = "反击型AI\n自己攻击或被敌人攻击,英雄就反击.",
                Visible = false,
            };
            AIModeButton[1].Click += (o, e) => ActiveAI(2);

            AIModeButton[2] = new MirButton()//跟随按钮
            {
                Index = 1842,
                HoverIndex = 1846,
                PressedIndex = 1846,
                Library = Libraries.Prguse,
                Location = new Point(Settings.ScreenWidth / 2 - parent.Size.Width / 2 + 195, Settings.ScreenHeight - 114),
                Parent = GameScene.Scene,
                Sound = SoundList.ButtonA,
                Hint = "跟随AI\n除了强制命令以外英雄什么也不做.",
                Visible = false,
            };
            AIModeButton[2].Click += (o, e) => ActiveAI(3);

            AIModeButton[3] = new MirButton()//自定义AI按钮
            {
                Index = 1843,
                HoverIndex = 1847,
                PressedIndex = 1847,
                Library = Libraries.Prguse,
                Location = new Point(Settings.ScreenWidth / 2 - parent.Size.Width / 2 + 213, Settings.ScreenHeight - 114),
                Parent = GameScene.Scene,
                Sound = SoundList.ButtonA,
                Hint = "自定义AI\n可以根据自己的需要来设定英雄的行动.",
                Visible = false,
            };
            AIModeButton[3].Click += (o, e) => ActiveAI(4);
            #endregion

            #region 帮助条
            SkillButton = new MirButton[4];

            SkillButton[0] = new MirButton()
            {
                Index = 141,
                HoverIndex = 142,
                PressedIndex = 143,
                Library = Libraries.Prguse2,
                Location = new Point(12, 4),
                Parent = this,
                Sound = SoundList.ButtonA,
                Hint = "帮助",
            };
            SkillButton[0].Click += (o, e) => GameScene.Scene.HelpDialog.DisplayPage(32);

            SkillButton[1] = new MirButton()
            {
                Index = 149,
                HoverIndex = 150,
                PressedIndex = 151,
                Library = Libraries.Prguse2,
                Location = new Point(27, 4),
                Parent = this,
                Sound = SoundList.ButtonA,
                Hint = "守护\n攻击怪物或指定目标.\n冷却时间30秒",
            };
            SkillButton[1].Click += (o, e) => ActiveSkill(1);

            SkillButton[2] = new MirButton()
            {
                Index = 153,
                HoverIndex = 154,
                PressedIndex = 155,
                Library = Libraries.Prguse2,
                Location = new Point(45, 4),
                Parent = this,
                Sound = SoundList.ButtonA,
                Hint = "撤退\n停止攻击并撤退.\n冷却时间30秒",
            };
            SkillButton[2].Click += (o, e) => ActiveSkill(2);

            SkillButton[3] = new MirButton()
            {
                Index = 145,
                HoverIndex = 146,
                PressedIndex = 147,
                Library = Libraries.Prguse2,
                Location = new Point(62, 4),
                Parent = this,
                Sound = SoundList.ButtonA,
                Hint = "召回\n召回英雄回到玩家身边.\n冷却时间3分钟",
            };
            SkillButton[3].Click += (o, e) => ActiveSkill(3);
            #endregion
        }

        public void ActiveAI(byte i)
        {
            AiIndex = i;
            switch (i)
            {
                case 1:
                    AIModeButton[0].Index = 1740;//选中
                    AIModeButton[1].Index = 1841;//未被选中
                    AIModeButton[2].Index = 1842;//未被选中
                    AIModeButton[3].Index = 1843;//未被选中
                    Network.Enqueue(new ChangeHeroAttackMode()
                    {
                        AttackMode = HeroMode.Attack
                    });
                    GameScene.Scene.ChatDialog.ReceiveChat("[ 英雄行为模式：攻击 ]", ChatType.Hint);
                    break;
                case 2:
                    AIModeButton[0].Index = 1840;//未被选中
                    AIModeButton[1].Index = 1741;//选中
                    AIModeButton[2].Index = 1842;//未被选中
                    AIModeButton[3].Index = 1843;//未被选中
                    Network.Enqueue(new ChangeHeroAttackMode()
                    {
                        AttackMode = HeroMode.Defend
                    });
                    GameScene.Scene.ChatDialog.ReceiveChat("[ 英雄行为模式：反击 ]", ChatType.Hint);
                    break;
                case 3:
                    AIModeButton[0].Index = 1840;//未被选中
                    AIModeButton[1].Index = 1841;//未被选中
                    AIModeButton[2].Index = 1742;//选中
                    AIModeButton[3].Index = 1843;//未被选中
                    Network.Enqueue(new ChangeHeroAttackMode()
                    {
                        AttackMode = HeroMode.Follow
                    });
                    GameScene.Scene.ChatDialog.ReceiveChat("[ 英雄行为模式：跟随 ]", ChatType.Hint);
                    break;
                case 4:
                    AIModeButton[0].Index = 1840;//未被选中
                    AIModeButton[1].Index = 1841;//未被选中
                    AIModeButton[2].Index = 1842;//未被选中
                    AIModeButton[3].Index = 1743;//选中
                    Network.Enqueue(new ChangeHeroAttackMode()
                    {
                        AttackMode = HeroMode.Custom
                    });
                    GameScene.Scene.ChatDialog.ReceiveChat("[ 英雄行为模式：自定义 ]", ChatType.Hint);
                    break;
            }
        }

        public void ActiveSkill(byte i)
        {
            //Network.Enqueue(new CallHeroAction() { ActionID = i });
        }

        public void Show()
        {
            foreach (MirControl mirControl in AIModeButton)
                mirControl.Visible = true;
            Visible = true;
        }

        public void Hide()
        {
            foreach (MirControl mirControl in AIModeButton)
                mirControl.Visible = false;
            Visible = false;
        }
    }
}
