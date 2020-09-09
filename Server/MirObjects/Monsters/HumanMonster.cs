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
        public bool Casting = false;
        public int OriginalAttackSpeed;
        public MirClass mobsClass;
        public MirGender mobsGender;
        public short weapon, armour;
        public byte wing, hair, light;
        public bool MagicShieldUp = false;
        public uint MP = 65535;
        public bool Summoned;
      
        public int HumanAttackSpeed
        {
            get
            {
                int tmp = OriginalAttackSpeed;
                if (mobsClass == MirClass.Taoist)
                    tmp = 1200;
                if (mobsClass == MirClass.Wizard)
                    tmp = 1400;
                if (mobsClass == MirClass.Warrior)
                {
                    if (Envir.Time > _RageTime)
                        tmp = 1000;
                    else
                        tmp = 1000 / 2;
                }
                if (mobsClass == MirClass.Assassin)
                    tmp = 800;
                return tmp;
            }
        }

        public HumanMonster(MonsterInfo info)
            : base(info)
        {
            GetHumanInfo();
            Direction = MirDirection.Down;
            Summoned = true;
            OriginalAttackSpeed = Info.AttackSpeed;
            switch (mobsClass)
            {
                case MirClass.Warrior:
                    MP = 1000;
                    break;
                case MirClass.Wizard:
                    MP = 2500;
                    break;
                case MirClass.Taoist:
                    MP = 1750;
                    break;
            }
        }

        public int GetHitCount()
        {
            int count = 0;
            if (Functions.MaxDistance(CurrentLocation, Target.CurrentLocation) <= 2)
                if (Functions.MaxDistance(CurrentLocation, Target.CurrentLocation) == 1)
                {
                    Point tmmp = Functions.PointMove(CurrentLocation, Direction, 2);
                    if (CurrentMap.ValidPoint(tmmp))
                    {
                        Cell cell = CurrentMap.GetCell(tmmp);
                        if (cell.Objects != null)
                            for (int i = 0; i < cell.Objects.Count; i++)
                                if ((cell.Objects[i].Race == ObjectType.Player ||
                                    cell.Objects[i].Race == ObjectType.Monster) &&
                                    cell.Objects[i].IsAttackTarget(this))
                                    count = 2;
                    }
                }
                else
                    count = 1;
            return count;
        }

        public override int Attacked(MonsterObject attacker, int damage, DefenceType type = DefenceType.ACAgility)
        {
            if (MagicShieldUp)
            {
                int temp = damage * 15 / 100;
                damage -= temp;
            }
            return base.Attacked(attacker, damage, type);
        }

        public override int Attacked(PlayerObject attacker, int damage, DefenceType type = DefenceType.ACAgility, bool damageWeapon = true)
        {
            if (MagicShieldUp)
            {
                int temp = damage * 15 / 100;
                damage -= temp;
            }
            return base.Attacked(attacker, damage, type, damageWeapon);
        }

        public long MPRegenTime;
        protected override void ProcessRegen()
        {
            if (Envir.Time > MPRegenTime)
            {
                int MPRegen = 10;
                if (MP + MPRegen > uint.MaxValue)
                    MP = uint.MaxValue;
                else
                    MP += (uint)MPRegen;
                MPRegenTime = Envir.Time + Settings.Second * 3;
            }
            base.ProcessRegen();
        }

        /// <summary>
        /// Override the ProcessTarget in order to setup the AI.
        /// </summary>
        protected override void ProcessTarget()
        {
            //  Ensure we're not trying to attack and invalid Target (Dead or non existent)
            if (Target == null || Target.Dead)
            {
                FindTarget();
                return;
            }

            //  Get the Direction to face and use on the ObjectAttack Packet
            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);
            List<MapObject> targets = FindAllTargets(1, CurrentLocation, false);
            int closeTargets = targets.Count;
            if (Envir.Time > AttackTime)
            {
                AttackTime = Envir.Time + HumanAttackSpeed;
                ActionTime = Envir.Time + 300;
                switch (mobsClass)
                {
                    #region Taoist
                    case MirClass.Taoist:
                        {
                            if (Functions.InRange(CurrentLocation, Target.CurrentLocation, 11))
                            {
                                if (Envir.Time > PoisonCloudTime && MP >= 28)
                                {
                                    MP -= 28;
                                    PoisonCloudTime = Envir.Time + Settings.Second * 30;
                                    Broadcast(new S.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
                                    PerformPoisonCloud();
                                }
                                else if (Envir.Time > CurseTime && MP >= 37)
                                {
                                    MP -= 37;
                                    CurseTime = Envir.Time + Settings.Second * 26;
                                    Broadcast(new S.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
                                    PerformCurse();
                                }
                                else if (Envir.Time > NextBlessedArmourTime && MP >= 15)
                                {
                                    MP -= 15;
                                    NextBlessedArmourTime = Envir.Time + Settings.Second * 34;
                                    BlessedArmourTime = Envir.Time + Settings.Second * 15;
                                    Broadcast(new S.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
                                    PerformSoulArmour();
                                }
                                else if (Envir.Time > NextSoulShieldTime && MP >= 15)
                                {
                                    MP -= 15;
                                    NextSoulShieldTime = Envir.Time + Settings.Second * 34;
                                    SoulShieldTime = Envir.Time + Settings.Second * 15;
                                    Broadcast(new S.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
                                    PerformSoulShield();
                                }
                                else if (Envir.Random.Next(5) == 0 && MP >= 6)
                                {
                                    MP -= 6;
                                    Broadcast(new S.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
                                    PerformPoisoning();
                                }
                                else if (MP >= 8)
                                {
                                    MP -= 6;
                                    Broadcast(new S.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
                                    PerformSoulFireBall();
                                }
                            }
                        }
                        break;
                    #endregion
                    #region Warrior
                    case MirClass.Warrior:
                        {
                            if (Envir.Time > _RageTime)
                                AttackSpeed = OriginalAttackSpeed;
                            if (InAttackRange())
                            {
                                if (Envir.Time > NextFlamingSwordTime && MP >= 45)
                                {
                                    MP -= 45;
                                    NextFlamingSwordTime = Envir.Time + Settings.Second * Envir.Random.Next(15, 25);
                                    Broadcast(new S.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
                                    PerformFlamingSword();
                                    return;
                                }
                                else if (Envir.Time > NextRageTime && MP >= 18 && Envir.Time > _RageTime)
                                {
                                    MP -= 18;
                                    _RageTime = Envir.Time + Settings.Second * 20;
                                    NextRageTime = Envir.Time + Settings.Second + Envir.Random.Next(30, 45);
                                    Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
                                    PerformRage();
                                    return;
                                }
                                else if (MP >= 9 && closeTargets <= 1)
                                {
                                    MP -= 9;
                                    Broadcast(new S.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
                                    PerformTwinDrakeBlade();
                                    return;
                                }
                                else if (MP >= 6 && closeTargets >= 2)
                                {
                                    if (Envir.Random.Next(0, 10) >= 5
                                        && MP >= 8)
                                    {
                                        MP -= 8;
                                        Broadcast(new S.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
                                        PerformCrossHalfMoon();
                                        return;
                                    }
                                    else
                                    {
                                        MP -= 6;
                                        Broadcast(new S.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
                                        PerformHalfmoon();
                                        return;
                                    }
                                }
                                else if (GetHitCount() == 2)
                                {
                                    Broadcast(new S.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
                                    PerformThrusting();
                                    return;
                                }
                                else if (Functions.InRange(CurrentLocation, Target.CurrentLocation, 1))
                                {
                                    Broadcast(new S.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
                                    Attack();
                                    return;
                                }
                            }
                        }
                        break;
                    #endregion
                    #region Wizard
                    case MirClass.Wizard:
                        {
                            if (MagicShieldUp &&
                                Envir.Time > MagicShieldTime)
                                MagicShieldUp = false;
                            if (!Casting)
                            {
                                //  Favour MagicShield
                                if (!MagicShieldUp && Envir.Time > NextMagicShieldTime && MP >= 28)
                                {
                                    MP -= 28;
                                    Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
                                    PerformMagicShield();
                                    //CastTime = Envir.Time + HumanAttackSpeed;
                                    return;
                                }
                                else if (Envir.Time > MeteorBlizzTime && MP >= 89)
                                {
                                    MP -= 89;
                                    Casting = true;
                                    MeteorBlizzTime = Envir.Time + Settings.Second * 50;
                                    if (Envir.Random.Next(0, 10) >= 5)
                                        PerformMeteorStrike();
                                    else
                                        PerformBlizzard();
                                    //CastTime = Envir.Time + HumanAttackSpeed;
                                    return;
                                }
                                else if (Envir.Time > SpellCastTime && MP >= 20)
                                {
                                    MP -= 20;
                                    SpellCastTime = Envir.Time + Settings.Second * 2;
                                    if (Envir.Random.Next(0, 10) >= 5)
                                        PerformThunderBolt();
                                    else
                                        PerformFireBall();
                                    //CastTime = Envir.Time + HumanAttackSpeed;
                                    return;
                                }
                                else if (Envir.Time > FireWallTime && MP >= 48)
                                {
                                    MP -= 48;
                                    FireWallTime = Envir.Time + Settings.Second * 32;
                                    PerformFireWall();
                                    //CastTime = Envir.Time + HumanAttackSpeed;
                                    return;
                                }
                                else if (Envir.Time > RepulseTime && MP >= 18)
                                {
                                    MP -= 18;
                                    RepulseTime = Envir.Time + Settings.Second * Envir.Random.Next(10, 30);
                                    PerformRepulse();
                                    //CastTime = Envir.Time + HumanAttackSpeed;
                                    return;
                                }
                            }
                            else if (Envir.Time > CastTime)
                                Casting = false;
                        }
                        break;
                        #endregion
                }
            }
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
                case MirClass.Wizard:
                    int dist = Functions.MaxDistance(CurrentLocation, Target.CurrentLocation);

                    if (dist >= 11)
                        MoveTo(Target.CurrentLocation);
                    else
                    {
                        MirDirection dir = Functions.DirectionFromPoint(Target.CurrentLocation, CurrentLocation);

                        if (Walk(dir))
                            return;

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
                case MirClass.Taoist:
                    dist = Functions.MaxDistance(CurrentLocation, Target.CurrentLocation);

                    if (dist >= 11)
                        MoveTo(Target.CurrentLocation);
                    else
                    {
                        MirDirection dir = Functions.DirectionFromPoint(Target.CurrentLocation, CurrentLocation);

                        if (Walk(dir))
                            return;

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


            //  If we're not in Range, move to the target
            if (!InAttackRange())
                MoveTo(Target.CurrentLocation);
            //  If the targets invalid or dead, find a new one.
            if (Target == null || Target.Dead)
            {
                FindTarget();
                return;
            }
            //  Move to the Target (it'll check if in range)
            MoveTo(Target.CurrentLocation);

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

        public void PerformMagicShield()
        {
            NextMagicShieldTime = Envir.Time + Settings.Second * 45;
            MagicShieldTime = Envir.Time + Settings.Second * 20;
            MagicShieldUp = true;
            Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Direction = Direction, Effect = SpellEffect.HumanMagicShield, Time = 20 });
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

        public void PerformRage()
        {
            AttackSpeed = AttackSpeed / 2;
            Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Direction = Direction, Effect = SpellEffect.HumanRage, Time = 20 });
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
