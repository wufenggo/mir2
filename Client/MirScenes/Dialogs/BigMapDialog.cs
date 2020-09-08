using Client.MirControls;
using Client.MirGraphics;
using Client.MirNetwork;
using Client.MirObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SlimDX;
using SlimDX.Direct3D9;
using S = ServerPackets;
using C = ClientPackets;
using ServerPackets;

namespace Client.MirScenes.Dialogs
{
    public sealed class BigMapDialog : MirImageControl
    {
        public List<ObjectNPC> NpcList = new List<ObjectNPC>();
        public List<MirImageControl> PublicEvents = new List<MirImageControl>();
        private MirLabel pointlab;
        //List<MirLabel> ListLabelTown = new List<MirLabel>();
        //List<NameTown> ListTown = new List<NameTown>();
        private long LastTeleportTime = 0;
        public BigMapDialog()
        {
            
            //NotControl = true;
            Location = new Point(130, 100);
            //Border = true;
            //BorderColour = Color.Lime;
            BeforeDraw += (o, e) => OnBeforeDraw();
            Sort = true;


            //loadTonw();
            //加入坐标显示
            pointlab = new MirLabel
            {
                AutoSize = true,
                ForeColour = Color.White,
                //OutLineColour = Color.Black,
                Parent = this,
                Location = new Point(2, 2),
                Visible = true
            };
            //鼠标移动事件监听，鼠标移动的时候，显示坐标变换
            MouseMove += (o, e) => OnMouseMove();
            MouseDown += (o, e) => OnMouseDown(o, e);


            BeforeDraw += (o, e) => OnBeforeDraw();
        }


        private void OnBeforeDraw()
        {
            MapControl map = GameScene.Scene.MapControl;
            if (map == null || !Visible) return;

            //int index = map.BigMap <= 0 ? map.MiniMap : map.BigMap;
            int index = map.BigMap;

            if (index <= 0)
            {
                if (Visible)
                {
                    Visible = false;
                }
                return;
            }

            TrySort();

            Rectangle viewRect = new Rectangle(0, 0, 600, 400);

            Size = Libraries.MiniMap.GetSize(index);

            if (Size.Width < 600)
                viewRect.Width = Size.Width;

            if (Size.Height < 400)
                viewRect.Height = Size.Height;

            viewRect.X = (Settings.ScreenWidth - viewRect.Width) / 2;
            viewRect.Y = (Settings.ScreenHeight - 120 - viewRect.Height) / 2;

            Location = viewRect.Location;
            Size = viewRect.Size;

            float scaleX = Size.Width / (float)map.Width;
            float scaleY = Size.Height / (float)map.Height;

            viewRect.Location = new Point(
                (int)(scaleX * MapObject.User.CurrentLocation.X) - viewRect.Width / 2,
                (int)(scaleY * MapObject.User.CurrentLocation.Y) - viewRect.Height / 2);

            if (viewRect.Right >= Size.Width)
                viewRect.X = Size.Width - viewRect.Width;
            if (viewRect.Bottom >= Size.Height)
                viewRect.Y = Size.Height - viewRect.Height;

            if (viewRect.X < 0) viewRect.X = 0;
            if (viewRect.Y < 0) viewRect.Y = 0;

            Libraries.MiniMap.Draw(index, Location, Size, Color.FromArgb(255, 255, 255));

            int startPointX = (int)(viewRect.X / scaleX);
            int startPointY = (int)(viewRect.Y / scaleY);

            //画地图上的点，应该是只画玩家和怪物，NPC
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];


                if (ob.Race == ObjectType.Item || ob.Dead || ob.Race == ObjectType.Spell) continue; // || (ob.ObjectID != MapObject.User.ObjectID)
                float x = ((ob.CurrentLocation.X - startPointX) * scaleX) + Location.X;
                float y = ((ob.CurrentLocation.Y - startPointY) * scaleY) + Location.Y;

                Color colour;

                if ((GroupDialog.GroupList.Contains(ob.Name) && MapObject.User != ob) || ob.Name.EndsWith(string.Format("({0})", MapObject.User.Name)))
                    colour = Color.FromArgb(0, 0, 255);
                else
                    if (ob is PlayerObject)
                    colour = Color.FromArgb(255, 255, 255);
                else if (ob is NPCObject || ob.AI == 6)
                    colour = Color.FromArgb(0, 255, 50);
                else
                    colour = Color.FromArgb(255, 0, 0);

                DXManager.Sprite.Draw(DXManager.RadarTexture, new Rectangle(0, 0, 2, 2), Vector3.Zero, new Vector3((float)(x - 0.5), (float)(y - 0.5), 0.0F), colour);
            }
            //for (int i = 0; i < ListTown.Count; i++)
            //{

            //    if (ListTown[i].BigMap != map.BigMap) continue;
            //    float xx = ((ListTown[i].Location.X - startPointX) * scaleX);
            //    float yy = ((ListTown[i].Location.Y - startPointY) * scaleY);


            //    ListLabelTown.Add(new MirLabel
            //    {
            //        AutoSize = true,
            //        Parent = this,
            //        Font = new Font(Settings.FontName, 9f, FontStyle.Regular),
            //        DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            //        Text = ListTown[i].Name,
            //        ForeColour = ListTown[i].Color,
            //        Location = new Point((int)(xx), (int)(yy)),
            //        NotControl = true,
            //        Visible = true,
            //        Modal = true
            //    });
            //}
            //这里画自动寻路的路径
            for (int i = 0; i < map.RouteList.Count; i++)
            {
                Color colour = Color.White;
                float x = ((map.RouteList[i].X - startPointX) * scaleX) + Location.X;
                float y = ((map.RouteList[i].Y - startPointY) * scaleY) + Location.Y;
                DXManager.Sprite.Draw(DXManager.RadarTexture, new Rectangle(0, 0, 2, 2), Vector3.Zero, new Vector3((float)(x - 0.5), (float)(y - 0.5), 0.0F), colour);
            }
          

        }

        //重写鼠标移动事件
        private void OnMouseMove()
        {
            MapControl map = GameScene.Scene.MapControl;
            if (map == null || !Visible) return;
            float scaleX = Size.Width / (float)map.Width;
            float scaleY = Size.Height / (float)map.Height;
            int x = CMain.MPoint.X - Location.X;

            if (x > 0)
            {
                x = (int)(x / scaleX);
            }
            int y = CMain.MPoint.Y - Location.Y;
            if (y > 0)
            {
                y = (int)(y / scaleY);
            }
            pointlab.Text = "地图坐标：" + x + "," + y;
            //MirLog.debug(CMain.MPoint.X+","+ CMain.MPoint.Y);
        }

        //响应右键事件，右键点击地图，进行寻址
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }
            //开启自动寻址
            MapControl map = GameScene.Scene.MapControl;
            if (map == null || !Visible) return;
            float scaleX = Size.Width / (float)map.Width;
            float scaleY = Size.Height / (float)map.Height;
            int x = CMain.MPoint.X - Location.X;

            if (x > 0)
            {
                x = (int)(x / scaleX);
            }
            int y = CMain.MPoint.Y - Location.Y;
            if (y > 0)
            {
                y = (int)(y / scaleY);
            }
            if (x > map.Width || y > map.Height)
            {
                return;
            }
            if (!map.M2CellInfo[x, y].CanWalk())
            {
                GameScene.Scene.ChatDialog.ReceiveChat("自动寻路目标不可达", ChatType.System);
                return;
            }
            //这里判断下是否可以传送，如果可以传送，则直接传送即可哦

            if (GameScene.User.HasTeleportRing && LastTeleportTime < CMain.Time)
            {
                LastTeleportTime = CMain.Time + 3000;//3秒之内只触发一次传送
                GameScene.Scene.ChatDialog.ReceiveChat("3秒才能传送一次", ChatType.Hint);
                Network.Enqueue(new C.Chat
                {
                    Message = "@move " + x + " " + y

                });
                return;
            }
            long startTime = CMain.Timer.ElapsedMilliseconds;
            //目标位置
            map.RouteTarget = new Point(x, y);
            if (map.StartRoute())
            {
                long endTime = CMain.Timer.ElapsedMilliseconds;
                GameScene.Scene.ChatDialog.ReceiveChat("[自动寻路:开启]", ChatType.Hint);
            }
        }
        //public sealed class NameTown
        //{
        //    public int BigMap;
        //    public Point Location;
        //    public string Name;
        //    public Color Color;


        //    public NameTown(string path)
        //    {
        //        int x;
        //        int y;


        //        string[] data = path.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);


        //        if (data.Length < 4) return;

        //        if (!int.TryParse(data[1], out x)) return;
        //        if (!int.TryParse(data[2], out y)) return;


        //        BigMap = Convert.ToInt32(data[0].ToString());
        //        Name = data[3].ToString();
        //        Location = new Point(x, y);
        //        Color = Color.FromName(data[4].ToString());

        //    }


        //}
        public void DisposeEvents()
        {
            for (int i = 0; i < PublicEvents.Count; i++)
                PublicEvents[i].Dispose();

            PublicEvents.Clear();
        }
        public void Hide()
        {
            DisposeEvents();
            Visible = false;
        }
        public void Toggle()
        {
            DisposeEvents();
            Visible = !Visible;

            Redraw();
        }
    }
}
