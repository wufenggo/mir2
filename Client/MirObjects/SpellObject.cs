using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Client.MirGraphics;
using Client.MirScenes;
using S = ServerPackets; 

namespace Client.MirObjects
{
    class SpellObject : MapObject
    {
        public override ObjectType Race
        {
            get { return ObjectType.Spell; }
        }

        public override bool Blocking
        {
            get { return false; }
        }

        public Spell Spell;
        public int FrameCount, FrameInterval, FrameIndex;
        public bool Repeat;
        public int EffectStart, EffectCount, EffectInterval, EffectIndex;

        public SpellObject(uint objectID) : base(objectID)
        {
        }

        public void Load(S.ObjectSpell info)
        {
            CurrentLocation = info.Location;
            MapLocation = info.Location;
            GameScene.Scene.MapControl.AddObject(this);
            Spell = info.Spell;
            Direction = info.Direction;
            Repeat = true;

            switch (Spell)
            {
                case Spell.HealingCircle://增加一个阴阳五行
                    BodyLibrary = Libraries.Magic3;
                    DrawFrame = 630;
                    FrameInterval = 100;
                    FrameCount = 10;
                    Blend = true;
                    break;
                case Spell.TrapHexagon:
                    BodyLibrary = Libraries.Magic;
                    DrawFrame = 1390;
                    FrameInterval = 100;
                    FrameCount = 10;
                    Blend = true;
                    break;
                case Spell.FireWall:
                    BodyLibrary = Libraries.Magic;
                    DrawFrame = 1630;
                    FrameInterval = 120;
                    FrameCount = 6;
                    Light = 3;
                    Blend = true;
                    break;
                case Spell.PoisonCloud:
                    BodyLibrary = Libraries.Magic2;
                    DrawFrame = 1650;
                    FrameInterval = 120;
                    FrameCount = 20;
                    Light = 3;
                    Blend = true;
                    break;
                case Spell.DigOutZombie:
                    BodyLibrary = (ushort)Monster.DigOutZombie < Libraries.Monsters.Count() ? Libraries.Monsters[(ushort)Monster.DigOutZombie] : Libraries.Magic;
                    DrawFrame = 304 + (byte) Direction;
                    FrameCount = 0;
                    Blend = false;
                    break;
                case Spell.Blizzard:
                    CurrentLocation.Y = Math.Max(0, CurrentLocation.Y - 20);
                    BodyLibrary = Libraries.Magic2;
                    DrawFrame = 1550;
                    FrameInterval = 100;
                    FrameCount = 30;
                    Light = 3;
                    Blend = true;
                    Repeat = false;
                    break;
                case Spell.MeteorStrike:
                    MapControl.Effects.Add(new Effect(Libraries.Magic2, 1600, 10, 800, CurrentLocation) { Repeat = true, RepeatUntil = CMain.Time + 3000 });
                    CurrentLocation.Y = Math.Max(0, CurrentLocation.Y - 20);
                    BodyLibrary = Libraries.Magic2;
                    DrawFrame = 1610;
                    FrameInterval = 100;
                    FrameCount = 30;
                    Light = 3;
                    Blend = true;
                    Repeat = false;
                    break;
                case Spell.Rubble:
                    if (Direction == 0)
                        BodyLibrary = null;
                    else
                    {
                        BodyLibrary = Libraries.Effect;
                        DrawFrame = 64 + Math.Min(4, (int)(Direction - 1));
                        FrameCount = 1;
                        FrameInterval = 10000;
                    }
                    break;
                case Spell.Reincarnation:
                    BodyLibrary = Libraries.Magic2;
                    DrawFrame = 1680;
                    FrameInterval = 100;
                    FrameCount = 10;
                    Light = 1;
                    Blend = true;
                    Repeat = true;
                    break;
                case Spell.ExplosiveTrap:
                    BodyLibrary = Libraries.Magic3;
                    if (info.Param)
                    {
                        DrawFrame = 1570;
                        FrameInterval = 100;
                        FrameCount = 9;
                        Repeat = false;
                        MirSounds.SoundManager.PlaySound(20000 + 124 * 10 + 5);//Boom for all players in range
                    }
                    else
                    {
                        DrawFrame = 1560;
                        FrameInterval = 100;
                        FrameCount = 10;
                        Repeat = true;
                    }
                    //Light = 1;
                    Blend = true;
                    break;
                case Spell.Trap:
                    BodyLibrary = Libraries.Magic2;
                    DrawFrame = 2360;
                    FrameInterval = 100;
                    FrameCount = 8;
                    Blend = true;
                    break;
                case Spell.MapLightning:
                    MapControl.Effects.Add(new Effect(Libraries.Dragon, 400 + (CMain.Random.Next(3) * 10), 5, 600, CurrentLocation));
                    MirSounds.SoundManager.PlaySound(8301);
                    break;
                case Spell.MapLava:
                    MapControl.Effects.Add(new Effect(Libraries.Dragon, 440, 20, 1600, CurrentLocation) { Blend = false });
                    MapControl.Effects.Add(new Effect(Libraries.Dragon, 470, 10, 800, CurrentLocation));
                    MirSounds.SoundManager.PlaySound(8302);
                    break;
                case Spell.MapQuake1:
                    MapControl.Effects.Add(new Effect(Libraries.Monsters[(ushort)Monster.HellLord], 27, 12, 1200, CurrentLocation) { Blend = false });
                    break;
                case Spell.MapQuake2:
                    MapControl.Effects.Add(new Effect(Libraries.Monsters[(ushort)Monster.HellLord], 39, 13, 1300, CurrentLocation) { Blend = false });
                    break;

                case Spell.Portal:
                    BodyLibrary = Libraries.Magic2;
                    DrawFrame = 2360;
                    FrameInterval = 100;
                    FrameCount = 8;
                    Blend = true;
                    break;


                case Spell.MonKITO://鬼头
                    BodyLibrary = Libraries.Monsters[446];
                    if (info.Param)
                    {
                        DrawFrame = 965;
                        FrameInterval = 100;
                        FrameCount = 8;
                        Repeat = false;
                    }
                    else
                    {
                        DrawFrame = 954;
                        FrameInterval = 100;
                        FrameCount = 10;
                        Repeat = true;
                    }
                    //Light = 1;
                    Blend = true;
                    break;
                case Spell.MonFireCircle://鬼头
                    BodyLibrary = Libraries.Monsters[(ushort)Monster.Monster446];
                    DrawFrame = 1366;
                    FrameInterval = 100;
                    FrameCount = 5;
                    Repeat = true;
                    if (info.Param)
                    {
                        MapControl.Effects.Add(new Effect(Libraries.Monsters[(ushort)Monster.Monster446], 1380, 4, 400, CurrentLocation) { Blend = true });
                    }
                    //Light = 1;
                    Blend = true;
                    break;
                case Spell.MonPoisonFog://怪物的毒雾，类似毒云吧
                    BodyLibrary = Libraries.Monsters[(ushort)Monster.Monster452];
                    DrawFrame = 400;
                    FrameInterval = 100;
                    FrameCount = 7;
                    Repeat = true;
                    //Light = 1;
                    Blend = true;
                    break;
                case Spell.MonRotateAxe://怪物的旋转斧头
                    BodyLibrary = Libraries.Monsters[(ushort)Monster.Monster453];
                    DrawFrame = 653;
                    FrameInterval = 150;
                    FrameCount = 5;
                    Repeat = true;
                    //Light = 1;
                    Blend = true;
                    break;
                case Spell.MonGhostFlag1://怪物鬼旗
                    BodyLibrary = Libraries.Monsters[(ushort)Monster.Monster453];
                    DrawFrame = 675;
                    FrameInterval = 100;
                    FrameCount = 8;
                    Repeat = true;
                    //Light = 1;
                    Blend = false;
                    //MapControl.Effects.Add(new Effect(Libraries.Monsters[(ushort)Monster.Monster453], 683, 7, 700, CurrentLocation) { Blend = true });
                    break;
                case Spell.MonGhostHead://鬼头2
                    BodyLibrary = Libraries.Monsters[454];
                    if (info.Param)
                    {
                        DrawFrame = 1041;
                        FrameInterval = 100;
                        FrameCount = 11;
                        Repeat = false;
                        Blend = true;
                    }
                    else
                    {
                        DrawFrame = 1024;
                        FrameInterval = 100;
                        FrameCount = 6;
                        Repeat = true;
                        EffectStart = 1035;
                        EffectCount = 6;
                        EffectInterval = 100;
                        Blend = false;
                    }
                    break;
                    //Light = 1;
            }


            NextMotion = CMain.Time + FrameInterval;
            NextMotion -= NextMotion % 100;
        }
        public override void Process()
        {
            if (CMain.Time >= NextMotion)
            {
                if (++FrameIndex >= FrameCount && Repeat)
                    FrameIndex = 0;
                NextMotion = CMain.Time + FrameInterval;
            }

            DrawLocation = new Point((CurrentLocation.X - User.Movement.X + MapControl.OffSetX) * MapControl.CellWidth, (CurrentLocation.Y - User.Movement.Y + MapControl.OffSetY) * MapControl.CellHeight);
            DrawLocation.Offset(GlobalDisplayLocationOffset);
            DrawLocation.Offset(User.OffSetMove);
        }

        public override void Draw()
        {
            if (FrameIndex >= FrameCount && !Repeat) return;
            if (BodyLibrary == null) return;

            if (Blend)
                BodyLibrary.DrawBlend(DrawFrame + FrameIndex, DrawLocation, DrawColour, true, 0.8F);
            else
                BodyLibrary.Draw(DrawFrame + FrameIndex, DrawLocation, DrawColour, true);
        }

        public override bool MouseOver(Point p)
        {
            return false;
        }

        public override void DrawBehindEffects(bool effectsEnabled)
        {
        }

        public override void DrawEffects(bool effectsEnabled)
        { 
        }
    }
}
