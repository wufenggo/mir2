﻿using Server.MirDatabase;
using Server.MirEnvir;
using System;
using S = ServerPackets;

namespace Server.MirObjects.Monsters
{   //战场弓箭护卫
    public class WarTownArcher : MonsterObject
    {
        public long FearTime;
        public byte AttackRange = 10;


        protected override bool CanMove
        {
            get { return Route.Count > 0 && !Dead && Envir.Time > MoveTime && Envir.Time > ActionTime && Envir.Time > ShockTime; }
        }

        protected internal WarTownArcher(MonsterInfo info) : base(info) { }

        public override bool IsAttackTarget(MonsterObject attacker) { return false; }

        public override void Spawned()
        {
            if (Respawn != null && Respawn.Info.Direction < 8)
                Direction = (MirDirection)Respawn.Info.Direction;

            base.Spawned();
        }
        protected override void Attack()
        {
            if (!Target.IsAttackTarget(this))
            {
                Target = null;
                return;
            }

            ShockTime = 0;

            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);
            Broadcast(new S.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, TargetID = Target.ObjectID });

            ActionTime = Envir.Time + 300;
            AttackTime = Envir.Time + AttackSpeed;

            int damage = GetAttackPower(MinDC, MaxDC);
            if (damage == 0) return;

            int delay = Functions.MaxDistance(CurrentLocation, Target.CurrentLocation) * 50 + 500;

            DelayedAction action = new DelayedAction(DelayedType.Damage, Envir.Time + delay, Target, damage, DefenceType.ACAgility);
            ActionList.Add(action);

            if (Target.Dead)
                FindTarget();
        }
        protected override void ProcessTarget()
        {
            if (Target == null || !CanAttack) return;

            if (InAttackRange() && Envir.Time < FearTime)
            {
                Attack();
                return;
            }

            FearTime = Envir.Time + 2000;

            if (Envir.Time < ShockTime)
            {
                Target = null;
                return;
            }

            int dist = Functions.MaxDistance(CurrentLocation, Target.CurrentLocation);

            if (dist > AttackRange) // || Target.PKPoints <= 99
            {
                Target = null;

                if (Respawn != null)
                    Direction = (MirDirection)Respawn.Info.Direction;
            }
        }
        protected override void FindTarget()
        {
            for (int d = 0; d <= Info.ViewRange; d++)
            {
                for (int y = CurrentLocation.Y - d; y <= CurrentLocation.Y + d; y++)
                {
                    if (y < 0) continue;
                    if (y >= CurrentMap.Height) break;

                    for (int x = CurrentLocation.X - d; x <= CurrentLocation.X + d; x += Math.Abs(y - CurrentLocation.Y) == d ? 1 : d * 2)
                    {
                        if (x < 0) continue;
                        if (x >= CurrentMap.Width) break;

                        //Cell cell = CurrentMap.GetCell(x, y);
                        if (!CurrentMap.Valid(x,y) || CurrentMap.Objects[x,y] == null) continue;

                        for (int i = 0; i < CurrentMap.Objects[x, y].Count; i++)
                        {
                            MapObject ob = CurrentMap.Objects[x, y][i];
                            switch (ob.Race)
                            {
                                case ObjectType.Player:
                                    PlayerObject playerob = (PlayerObject)ob;
                                    if (!ob.IsAttackTarget(this)) continue;
                                    if (playerob.PKPoints < 200 || ob.Hidden && (!CoolEye || Level < ob.Level)) continue;
                                    Target = ob;
                                    return;
                                default:
                                    continue;
                            }
                        }
                    }
                }
            }
        }
        protected override bool InAttackRange()
        {
            return CurrentMap == Target.CurrentMap && Functions.InRange(CurrentLocation, Target.CurrentLocation, AttackRange);
        }
    }
}