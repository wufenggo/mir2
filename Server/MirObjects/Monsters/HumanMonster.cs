using Server.MirDatabase;
using Server.MirEnvir;
using System.Collections.Generic;
using System.Drawing;
using S = ServerPackets;

namespace Server.MirObjects.Monsters
{
    public class HumanMonster : MonsterObject
    {
        public long MagicShieldTime, MeteorBlizzTime, FireWallTime, RepulseTime, _RageTime, PoisonCloudTime, SoulShieldTime, BlessedArmourTime, CurseTime;
        public long CastTime, SpellCastTime;
        public long NextMagicShieldTime, NextRageTime, NextFlamingSwordTime, NextSoulShieldTime, NextBlessedArmourTime, NextCurseTime;
        public long FearTime, DecreaseMPTime;
        
        public byte AttackRange = 6;

        public bool Summoned;
        public MirClass mobsClass;
        public MirGender mobsGender;
        public short weapon, armour;
        public byte wing, hair, light;
        public int AttackDamage = 0;

        public int HumanAttackSpeed
        {
            get
            {
                int tmp = AttackRange;
                if (mobsClass == MirClass.Taoist)
                    tmp = 6;
                if (mobsClass == MirClass.Wizard)
                    tmp = 7;
                if (mobsClass == MirClass.Warrior)
                {
                  
                        tmp = 1;
              
                }
                if (mobsClass == MirClass.Assassin)
                    tmp = 1;
                return tmp;
            }
        }
        protected internal HumanMonster(MonsterInfo info)
            : base(info)
        {
            Direction = MirDirection.Down;
            Summoned = true;
        }

        protected override bool InAttackRange()
        {
            return CurrentMap == Target.CurrentMap && Functions.InRange(CurrentLocation, Target.CurrentLocation, HumanAttackSpeed);
        }

        protected override void Attack()
        {
            List<MapObject> targets = FindAllTargets(1, CurrentLocation, false);

            if (!Target.IsAttackTarget(this))
            {
                Target = null;
                return;
            }

            ShockTime = 0;
            switch (mobsClass)
            {
                case MirClass.Warrior:
                    {

                        Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);
                        Broadcast(new S.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });


                        ActionTime = Envir.Time + 300;
                        AttackTime = Envir.Time + AttackSpeed;

                        int damage2 = GetAttackPower(MinDC, MaxDC);
                        AttackDamage += damage2;

                        if (damage2 == 0) return;

                        Target.Attacked(this, damage2);
                    }
                    break;


                case MirClass.Wizard:
                    {
                        Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);
                        Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Spell = Spell.ThunderBolt, TargetID = Target.ObjectID, Target = Target.CurrentLocation, Cast = true, Level = 3 });
                       
                    }
                    ActionTime = Envir.Time + 300;
                    AttackTime = Envir.Time + AttackSpeed;

                    int damage = GetAttackPower(MinMC, MaxMC);
                    if (damage == 0) return;

                    DelayedAction action1 = new DelayedAction(DelayedType.Damage, Envir.Time + 500, Target, damage, DefenceType.MAC);
                    ActionList.Add(action1);
                    break;
                case MirClass.Taoist:
                    {
                        Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);
                        Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Spell = Spell.SoulFireBall, TargetID = Target.ObjectID, Target = Target.CurrentLocation, Cast = true, Level = 3 });
                        
                    }
                    ActionTime = Envir.Time + 1800;
                    AttackTime = Envir.Time + AttackSpeed;

                    int damage1 = GetAttackPower(MinSC, MaxSC);
                    if (damage1 == 0) return;

                    DelayedAction action = new DelayedAction(DelayedType.Damage, Envir.Time + 500, Target, damage1, DefenceType.MAC);
                    ActionList.Add(action);
                    break;

            }



           

            if (Target.Dead)
                FindTarget();
        }

        protected override void ProcessAI()
        {
            base.ProcessAI();

            if (Master != null && Master is PlayerObject && Envir.Time > DecreaseMPTime)
            {
                DecreaseMPTime = Envir.Time + 1000;
                if (!Master.Dead) ((PlayerObject)Master).ChangeMP(-10);

                if (((PlayerObject)Master).MP <= 0) Die();
            }
        }

        protected override void ProcessTarget()
        {
            if (Target == null || !CanAttack) return;

            if (Master != null)
                MoveTo(Master.CurrentLocation);

            if (InAttackRange())
            {
                Attack();
                return;
            }

            FearTime = Envir.Time + 5000;

            if (Envir.Time < ShockTime)
            {
                Target = null;
                return;
            }



                switch (mobsClass)
            {

                

                case MirClass.Warrior:
                    if (!InAttackRange())
                        MoveTo(Target.CurrentLocation);
                    break;
                case MirClass.Taoist:
                    FearTime = Envir.Time + 5000;

                    if (Envir.Time < ShockTime)
                    {
                        Target = null;
                        return;
                    }
                    int dist1 = Functions.MaxDistance(CurrentLocation, Target.CurrentLocation);

                    if (dist1 < AttackRange)
                    {
                        MirDirection dir1 = Functions.DirectionFromPoint(Target.CurrentLocation, CurrentLocation);

                        if (Walk(dir1)) return;

                        switch (Envir.Random.Next(2)) //No favour
                        {
                            case 0:
                                for (int i = 0; i < 7; i++)
                                {
                                    dir1 = Functions.NextDir(dir1);

                                    if (Walk(dir1))
                                        return;
                                }
                                break;
                            default:
                                for (int i = 0; i < 7; i++)
                                {
                                    dir1 = Functions.PreviousDir(dir1);

                                    if (Walk(dir1))
                                        return;
                                }
                                break;
                        }

                    }
                    break;

                case MirClass.Wizard:
                    FearTime = Envir.Time + 5000;

                    if (Envir.Time < ShockTime)
                    {
                        Target = null;
                        return;
                    }

                    int dist = Functions.MaxDistance(CurrentLocation, Target.CurrentLocation);

                    if (dist < AttackRange)
                    {
                        MirDirection dir = Functions.DirectionFromPoint(Target.CurrentLocation, CurrentLocation);

                        if (Walk(dir)) return;

                        switch (Envir.Random.Next(2)) //No favour
                        {
                            case 0:
                                for (int i = 0; i < 7; i++)
                                {
                                    dir = Functions.NextDir(dir);

                                    if (Walk(dir))
                                        return;
                                }
                                break;
                            default:
                                for (int i = 0; i < 7; i++)
                                {
                                    dir = Functions.PreviousDir(dir);

                                    if (Walk(dir))
                                        return;
                                }
                                break;
                        }

                    }
                    break;
            }



        }

        public void GetHumanInfo()
        {
            if (Settings.HumMobs != null && Settings.HumMobs.Count > 0)
            {
                for (int i = 0; i < Settings.HumMobs.Count; i++)
                {
                    if (Settings.HumMobs[i].HumansName.ToLower() == Info.Name.ToLower())
                    {
                        mobsClass = Settings.HumMobs[i].MobsClass;
                        mobsGender = Settings.HumMobs[i].MobsGender;
                        weapon = Settings.HumMobs[i].Weapon;
                        armour = Settings.HumMobs[i].Armour;
                        wing = Settings.HumMobs[i].Wing;
                        hair = Settings.HumMobs[i].Hair;
                        light = Settings.HumMobs[i].Light;
                    }
                }
            }
        }

        #region Wizard
        public void PerformFireBall()
        {
            if (!Target.IsAttackTarget(this))
            {
                Target = null;
                return;
            }

            ShockTime = 0;

            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);

            int damage = GetAttackPower(MinMC, MaxMC);
            if (damage == 0)
                return;

            if (Envir.Random.Next(Settings.MagicResistWeight) >= Target.MagicResist)
            {
                int delay = Functions.MaxDistance(CurrentLocation, Target.CurrentLocation) * 50 + 500; //50 MS per Step

                DelayedAction action = new DelayedAction(DelayedType.Damage, Envir.Time + delay, Target, damage, DefenceType.MACAgility);
                ActionList.Add(action);
                Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Spell = Spell.FireBall, TargetID = Target.ObjectID, Target = Target.CurrentLocation, Cast = true, Level = 3 });
            }
            if (Target.Dead)
                FindTarget();
        }

        public void PerformThunderBolt()
        {
            if (!Target.IsAttackTarget(this))
            {
                Target = null;
                return;
            }

            ShockTime = 0;

            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);

            int damage = GetAttackPower(MinMC, MaxMC);
            if (damage == 0)
                return;

            if (Envir.Random.Next(Settings.MagicResistWeight) >= Target.MagicResist)
            {
                int delay = Functions.MaxDistance(CurrentLocation, Target.CurrentLocation) * 50 + 500; //50 MS per Step

                DelayedAction action = new DelayedAction(DelayedType.Damage, Envir.Time + delay, Target, damage, DefenceType.MACAgility);
                ActionList.Add(action);
                Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Spell = Spell.ElectricShock, TargetID = Target.ObjectID, Target = Target.CurrentLocation, Cast = true, Level = 3 });
            }
            if (Target.Dead)
                FindTarget();
        }

        public void PerformRepulse()
        {
            List<MapObject> targets = FindAllTargets(1, CurrentLocation, false);
            for (int i = 0; i < targets.Count; i++)
                if (targets[i].IsAttackTarget(this))
                    targets[i].Pushed(this, Functions.DirectionFromPoint(targets[i].CurrentLocation, targets[i].Back), 4);
            Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Spell = Spell.Repulsion, Cast = true, Level = 3 });
        }

        public void PerformFireWall()
        {
            Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Spell = Spell.FireWall, Cast = true, Level = 3 });
            int damage = GetAttackPower(MinMC, MaxMC);
            DelayedAction action = new DelayedAction(DelayedType.MonsterMagic, Envir.Time + 500, this, Spell.FireWall, damage, Target.CurrentLocation);
            //  Add the action to the current map
            CurrentMap.ActionList.Add(action);
        }

        public void PerformBlizzard()
        {
            Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Spell = Spell.Blizzard, Cast = true, Level = 3 });
            int damage = GetAttackPower(MinMC, MaxMC);
            DelayedAction action = new DelayedAction(DelayedType.MonsterMagic, Envir.Time + 500, this, Spell.Blizzard, damage, Target.CurrentLocation);
            //  Add the action to the current map
            CurrentMap.ActionList.Add(action);
        }

        public void PerformMeteorStrike()
        {
            Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Spell = Spell.MeteorStrike, Cast = true, Level = 3 });
            int damage = GetAttackPower(MinMC, MaxMC);
            DelayedAction action = new DelayedAction(DelayedType.MonsterMagic, Envir.Time + 500, this, Spell.MeteorStrike, damage, Target.CurrentLocation);
            //  Add the action to the current map
            CurrentMap.ActionList.Add(action);
        }


        #endregion

        #region Warrior
        public void PerformFlamingSword()
        {
            int damage = GetAttackPower(MinDC * 2, MaxDC * 2);
            Target.Attacked(this, damage, DefenceType.AC);
            Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Direction = Direction, Effect = SpellEffect.HumanFlamingSword });
        }

        public void PerformTwinDrakeBlade()
        {
            int damage = GetReducedAttackPower(MinDC, MaxDC);
            Target.Attacked(this, damage, DefenceType.ACAgility);
            damage = GetReducedAttackPower(MinDC, MaxDC);
            DelayedAction action = new DelayedAction(DelayedType.Damage, Envir.Time + 800, Target, damage, DefenceType.ACAgility);
            ActionList.Add(action);
            Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Direction = Direction, Effect = SpellEffect.HumanTwinDrakeBlade });
        }

        public int GetReducedAttackPower(int min, int max)
        {
            int damage = GetAttackPower(min, max);
            float tmp = damage / 0.75f;
            damage = (int)tmp;
            return damage;
        }

        public void PerformHalfmoon()
        {
            MirDirection dir = Functions.PreviousDir(Direction);
            for (int i = 0; i < 3; i++)
            {
                Point location = Functions.PointMove(CurrentLocation, dir, 1);
                if (!CurrentMap.ValidPoint(location))
                    continue;
                Cell cell = CurrentMap.GetCell(location);
                if (cell != null &&
                    cell.Objects != null)
                {
                    for (int x = 0; x < cell.Objects.Count; x++)
                    {
                        if (cell.Objects[x].Race == ObjectType.Player ||
                            cell.Objects[x].Race == ObjectType.Monster)
                        {
                            if (cell.Objects[x].IsAttackTarget(this))
                            {
                                int damage = GetAttackPower(MinDC, MaxDC);
                                cell.Objects[x].Attacked(this, damage, DefenceType.ACAgility);
                            }
                        }
                    }
                }
                dir = Functions.NextDir(dir);
            }
            Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Direction = Direction, Effect = SpellEffect.HumanHalfMoon });
        }

        public void PerformCrossHalfMoon()
        {
            int damage = GetAttackPower(MinDC, MaxDC);
            List<MapObject> targets = FindAllTargets(1, CurrentLocation, false);
            for (int i = 0; i < targets.Count; i++)
                if (targets[i].IsAttackTarget(this))
                    targets[i].Attacked(this, damage, DefenceType.ACAgility);
            Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Direction = Direction, Effect = SpellEffect.HumanCrossHalfMoon });
        }

        public void PerformThrusting()
        {
            int damage = GetAttackPower(MinDC, MaxDC);
            if (damage == 0)
                return;

            for (int i = 1; i <= 2; i++)
            {
                Point target = Functions.PointMove(CurrentLocation, Direction, i);

                if (target == Target.CurrentLocation)
                {
                    if (Target.Attacked(this, damage, DefenceType.MACAgility) > 0 && Envir.Random.Next(8) == 0)
                    {
                        if (Envir.Random.Next(Settings.PoisonResistWeight) >= Target.PoisonResist)
                        {
                            int poison = GetAttackPower(MinSC, MaxSC);

                            Target.ApplyPoison(new Poison
                            {
                                Owner = this,
                                Duration = 5,
                                PType = PoisonType.Green,
                                Value = poison,
                                TickSpeed = 2000
                            }, this);
                        }
                    }
                }
                else
                {
                    if (!CurrentMap.ValidPoint(target))
                        continue;

                    Cell cell = CurrentMap.GetCell(target);
                    if (cell.Objects == null)
                        continue;

                    for (int o = 0; o < cell.Objects.Count; o++)
                    {
                        MapObject ob = cell.Objects[o];
                        if (ob.Race == ObjectType.Monster || ob.Race == ObjectType.Player)
                        {
                            if (!ob.IsAttackTarget(this))
                                continue;

                            if (ob.Attacked(this, damage, DefenceType.MACAgility) > 0 && Envir.Random.Next(8) == 0)
                            {
                                if (Envir.Random.Next(Settings.PoisonResistWeight) >= Target.PoisonResist)
                                {
                                    int poison = GetAttackPower(MinSC, MaxSC);

                                    ob.ApplyPoison(new Poison
                                    {
                                        Owner = this,
                                        Duration = 5,
                                        PType = PoisonType.Green,
                                        Value = poison,
                                        TickSpeed = 2000
                                    }, this);
                                }
                            }
                        }
                        else
                            continue;

                        break;
                    }
                }
            }
            Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Direction = Direction, Effect = SpellEffect.HumanThrusting });
        }


        #endregion

        #region Taoist
        public void PerformSoulFireBall()
        {

            if (!Target.IsAttackTarget(this))
            {
                Target = null;
                return;
            }

            ShockTime = 0;

            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);

            int damage = GetAttackPower(MinMC, MaxMC);
            if (damage == 0)
                return;

            if (Envir.Random.Next(Settings.MagicResistWeight) >= Target.MagicResist)
            {
                int delay = Functions.MaxDistance(CurrentLocation, Target.CurrentLocation) * 50 + 500; //50 MS per Step

                DelayedAction action = new DelayedAction(DelayedType.Damage, Envir.Time + delay, Target, damage, DefenceType.MACAgility);
                ActionList.Add(action);
                Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Spell = Spell.SoulFireBall, TargetID = Target.ObjectID, Target = Target.CurrentLocation, Cast = true, Level = 3 });
            }
            if (Target.Dead)
                FindTarget();
        }

        public void PerformPoisoning()
        {
            Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Spell = Spell.Poisoning, TargetID = Target.ObjectID, Target = Target.CurrentLocation, Cast = true, Level = 3 });
            if (Envir.Random.Next(0, 100) > 50)
                Target.ApplyPoison(new Poison { PType = PoisonType.Green, Duration = 30, TickSpeed = 1000, Value = 10 }, this);
            else
                Target.ApplyPoison(new Poison { PType = PoisonType.Red, Duration = 30, TickSpeed = 1000, Value = 10 }, this);
        }

        public void PerformCurse()
        {
            Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Direction = Direction, Effect = SpellEffect.HumanCurseCast });
            List<MapObject> targets = FindAllTargets(4, Target.CurrentLocation, false);
            for (int i = 0; i < targets.Count; i++)
                if (targets[i].IsAttackTarget(this))
                    if (Envir.Random.Next(5) == 0)
                        targets[i].ApplyPoison(new Poison { PType = PoisonType.Slow, Duration = 5, Owner = this, Value = 10, TickSpeed = 2000 }, Owner = this, true, true);
            Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Direction = Direction, Effect = SpellEffect.HumanCurseCast });
        }

        public void PerformPoisonCloud()
        {
            Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Direction = Direction, Effect = SpellEffect.HumanCastPoisonCloud });
            int damage = GetAttackPower(MinMC / 2, MaxMC / 2);
            DelayedAction action = new DelayedAction(DelayedType.MonsterMagic, Envir.Time + 500, this, Spell.PoisonCloud, damage, Target.CurrentLocation);
            //  Add the action to the current map
            CurrentMap.ActionList.Add(action);
        }

        public void PerformSoulShield()
        {
            List<MapObject> targets = FindAllTargets(4, CurrentLocation, false);
            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i].Race == ObjectType.Monster &&
                    targets[i].Master == null)
                {
                    targets[i].AddBuff(new Buff { Type = BuffType.SoulShield, Caster = this, ExpireTime = Envir.Time + Settings.Second * 10, ObjectID = targets[i].ObjectID, Values = new int[] { 10 } });
                }
            }
            Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Direction = Direction, Effect = SpellEffect.HumanCastSoulShield });

        }

        public void PerformSoulArmour()
        {
            List<MapObject> targets = FindAllTargets(4, CurrentLocation, false);
            for (int i = 0; i < targets.Count; i++)
                if (targets[i].Race == ObjectType.Monster &&
                    targets[i].Master == null)
                    targets[i].AddBuff(new Buff { Type = BuffType.BlessedArmour, Caster = this, ExpireTime = Envir.Time + Settings.Second * 10, ObjectID = targets[i].ObjectID, Values = new int[] { 10 } });
            Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Direction = Direction, Effect = SpellEffect.HumanCastBlessArm });
        }
        #endregion


        public override void Spawned()
        {
            base.Spawned();
            Summoned = false;
        }
        public override void Die()
        {
            if (Dead)
                return;

            HP = 0;
            Dead = true;

            //DeadTime = Envir.Time + DeadDelay;
            DeadTime = 0;

            Broadcast(new S.ObjectDied { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Type = (byte)(Master != null ? 1 : 0) });

            if (EXPOwner != null && Master == null && EXPOwner.Race == ObjectType.Player)
                EXPOwner.WinExp(Experience);

            if (Respawn != null)
                Respawn.Count--;

            if (Master == null)
                Drop();

            PoisonList.Clear();
            Envir.MonsterCount--;
            CurrentMap.MonsterCount--;
        }

        public override Packet GetInfo()
        {
            GetHumanInfo();
            if (weapon < 0)
                weapon = 0;
            if (armour < 0)
                armour = 0;
            if (wing < 0)
                wing = 0;
            if (hair < 0)
                hair = 0;
            if (light < 0)
                light = 0;
            return new S.ObjectPlayer
            {
                ObjectID = ObjectID,
                Name = Name,
                NameColour = NameColour,
                Class = mobsClass,
                Gender = mobsGender,
                Location = CurrentLocation,
                Direction = Direction,
                Hair = hair,
                Weapon = weapon,
                Armour = armour,
                Light = light,
                Poison = CurrentPoison,
                Dead = Dead,
                Hidden = Hidden,
                Effect = SpellEffect.None,
                WingEffect = wing,
                Extra = Summoned,
                TransformType = -1
            };
        }
    }
}
