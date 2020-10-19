using Client.MirScenes;
using S = ServerPackets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Client.MirObjects
{
    public class HeroObject : PlayerObject//英雄
    {
        public UserItem[] Inventory = new UserItem[10];
        public UserItem[] Equipment = new UserItem[14];
        public List<ClientMagic> Magics = new List<ClientMagic>();
        public List<ItemSets> ItemSets = new List<ItemSets>();
        public List<EquipmentSlot> MirSet = new List<EquipmentSlot>();//天龙套集合
        public List<EquipmentSlot> RedOrchidSet = new List<EquipmentSlot>();//虹膜套集合
        public List<EquipmentSlot> RedFlowerSet = new List<EquipmentSlot>();//魔血套集合

        public long Id;
        public uint MaxHP;
        public uint MP;
        public uint MaxMP;
        public ulong HP;
        public ushort MinAC;
        public ushort MaxAC;
        public ushort MinMAC;
        public ushort MaxMAC;
        public ushort MinDC;
        public ushort MaxDC;
        public ushort MinMC;
        public ushort MaxMC;
        public ushort MinSC;
        public ushort MaxSC;
        public byte ADC;
        public byte AMC;
        public byte ASC;
        public byte AAC;
        public byte AMAC;
        public byte Accuracy;
        public byte Agility;
        public sbyte ASpeed;
        public sbyte Luck;
        public int AttackSpeed;
        public ushort CurrentHandWeight;
        public ushort MaxHandWeight;
        public ushort CurrentWearWeight;
        public ushort MaxWearWeight;
        public ushort CurrentBagWeight;
        public ushort MaxBagWeight;
        public long Experience;
        public long MaxExperience;
        public byte LifeOnHit;
        public byte MagicResist;
        public byte PoisonResist;
        public byte HealthRecovery;
        public byte SpellRecovery;
        public byte PoisonRecovery;
        public byte CriticalRate;
        public byte CriticalDamage;
        public byte Holy;
        public byte Freezing;
        public byte PoisonAttack;
        public byte HpDrainRate;
        public BaseStats CoreStats;
        public HeroMode Mode;

        public HeroObject(uint objectID) : base(objectID)
        {
        }

        public void Load(S.HeroInformation info)//读取服务端发来的英雄服务包信息
        {
            Id = info.RealId;
            Name = info.Name;
            NameColour = info.NameColour;

            Class = info.Class;
            CoreStats = new BaseStats(Class);
            Gender = info.Gender;
            Level = info.Level;
            CurrentLocation = info.Location;
            MapLocation = info.Location;
            GameScene.Scene.MapControl.AddObject((MapObject)this);
            Direction = info.Direction;
            Hair = info.Hair;
            HP = info.HP;
            MP = info.MP;
            Experience = info.Experience;
            MaxExperience = info.MaxExperience;
            LevelEffects = info.LevelEffects;
            if (GameScene.User != null)
            {
                Inventory = info.Inventory;
                Equipment = info.Equipment;
            }
            Magics = info.Magics;
            MaxHP = info.MaxHP;
            MP = info.MP;
            MaxMP = info.MaxMP;
            HP = info.HP;
            MinAC = info.MinAC;
            MaxAC = info.MaxAC;
            MinMAC = info.MinMAC;
            MaxMAC = info.MaxMAC;
            MinDC = info.MinDC;
            MaxDC = info.MaxDC;
            MinMC = info.MinMC;
            MaxMC = info.MaxMC;
            MinSC = info.MinSC;
            MaxSC = info.MaxSC;
            Accuracy = info.Accuracy;
            Agility = info.Agility;
            ASpeed = info.ASpeed;
            Luck = info.Luck;
            AttackSpeed = info.AttackSpeed;
            CurrentHandWeight = info.CurrentHandWeight;
            MaxHandWeight = info.MaxHandWeight;
            CurrentWearWeight = info.CurrentWearWeight;
            MaxWearWeight = info.MaxWearWeight;
            CurrentBagWeight = info.CurrentBagWeight;
            MaxBagWeight = info.MaxBagWeight;
            Experience = info.Experience;
            MaxExperience = info.MaxExperience;
            LifeOnHit = info.LifeOnHit;
            MagicResist = info.MagicResist;
            PoisonResist = info.PoisonResist;
            HealthRecovery = info.HealthRecovery;
            SpellRecovery = info.SpellRecovery;
            PoisonRecovery = info.PoisonRecovery;
            CriticalRate = info.CriticalRate;
            CriticalDamage = info.CriticalDamage;
            Holy = info.Holy;
            Freezing = info.Freezing;
            PoisonAttack = info.PoisonAttack;
            HpDrainRate = info.HpDrainRate;
            Mode = info.Mode;
            Inventory = info.Inventory;
            Equipment = info.Equipment;
            BindAllItems();
            RefreshStats();
            SetAction();
        }

        public void Refresh(S.HeroInformation info)
        {
            Id = info.RealId;
            Name = info.Name;
            NameColour = info.NameColour;

            Class = info.Class;
            CoreStats = new BaseStats(Class);
            Gender = info.Gender;
            Level = info.Level;
            CurrentLocation = info.Location;
            MapLocation = info.Location;
            Direction = info.Direction;
            Hair = info.Hair;
            HP = info.HP;
            MP = info.MP;
            Experience = info.Experience;
            MaxExperience = info.MaxExperience;
            LevelEffects = info.LevelEffects;
            Magics = info.Magics;
            MaxHP = info.MaxHP;
            MP = info.MP;
            MaxMP = info.MaxMP;
            HP = info.HP;
            MinAC = info.MinAC;
            MaxAC = info.MaxAC;
            MinMAC = info.MinMAC;
            MaxMAC = info.MaxMAC;
            MinDC = info.MinDC;
            MaxDC = info.MaxDC;
            MinMC = info.MinMC;
            MaxMC = info.MaxMC;
            MinSC = info.MinSC;
            MaxSC = info.MaxSC;
            Accuracy = info.Accuracy;
            Agility = info.Agility;
            ASpeed = info.ASpeed;
            Luck = info.Luck;
            AttackSpeed = info.AttackSpeed;
            CurrentHandWeight = info.CurrentHandWeight;
            MaxHandWeight = info.MaxHandWeight;
            CurrentWearWeight = info.CurrentWearWeight;
            MaxWearWeight = info.MaxWearWeight;
            CurrentBagWeight = info.CurrentBagWeight;
            MaxBagWeight = info.MaxBagWeight;
            Experience = info.Experience;
            MaxExperience = info.MaxExperience;
            LifeOnHit = info.LifeOnHit;
            MagicResist = info.MagicResist;
            PoisonResist = info.PoisonResist;
            HealthRecovery = info.HealthRecovery;
            SpellRecovery = info.SpellRecovery;
            PoisonRecovery = info.PoisonRecovery;
            CriticalRate = info.CriticalRate;
            CriticalDamage = info.CriticalDamage;
            Holy = info.Holy;
            Freezing = info.Freezing;
            PoisonAttack = info.PoisonAttack;
            HpDrainRate = info.HpDrainRate;
            Mode = info.Mode;
        }

        public void RefreshStats()
        {
            RefreshLevelStats();
            RefreshEquipmentStats();
            RefreshItemSetStats();
            RefreshMirSetStats();//刷新天龙套装设置属性
            RefreshRedOrchidSetStats();//刷新虹膜套设置属性
            RefreshRedFlowerSetStats();//刷新魔血套设置属性
            RefreshSkills();
            RefreshBuffs();
            SetLibraries();
            SetEffects();
            Luck = Math.Min((sbyte)10, Math.Max((sbyte)-10, Luck));
            MagicResist = Math.Min((byte)20, Math.Max((byte)1, MagicResist));
            PoisonResist = Math.Min((byte)20, Math.Max((byte)0, PoisonResist));
            HealthRecovery = Math.Min((byte)200, Math.Max((byte)0, HealthRecovery));
            SpellRecovery = Math.Min((byte)200, Math.Max((byte)0, SpellRecovery));
            PoisonRecovery = Math.Min((byte)200, Math.Max((byte)0, PoisonRecovery));
            Freezing = Math.Min((byte)20, Math.Max((byte)0, Freezing));
            PoisonAttack = Math.Min((byte)20, Math.Max((byte)0, PoisonAttack));
            AttackSpeed = 1400 - (ASpeed >= 20 ? 600 : ASpeed * 30) - Math.Min(400, Level * 16);//25级之内完成
            if (AttackSpeed < 400) AttackSpeed = 400;
            GameScene.Scene.Redraw();
        }

        private void RefreshLevelStats()
        {
            MaxHP = 0;
            MaxMP = 0;
            MinAC = 0;
            MaxAC = 0;
            MinMAC = 0;
            MaxMAC = 0;
            MinDC = 0;
            MaxDC = 0;
            MinMC = 0;
            MaxMC = 0;
            MinSC = 0;
            MaxSC = 0;
            MaxBagWeight = 0;
            MaxWearWeight = 0;
            MaxHandWeight = 0;
            ASpeed = 0;
            Luck = 0;
            Light = 0;
            LifeOnHit = 0;
            HpDrainRate = 0;
            MagicResist = 0;
            PoisonResist = 0;
            HealthRecovery = 0;
            SpellRecovery = 0;
            PoisonRecovery = 0;
            Holy = 0;
            Freezing = 0;
            PoisonAttack = 0;
            Accuracy = CoreStats.StartAccuracy;
            Agility = CoreStats.StartAgility;
            CriticalRate = CoreStats.StartCriticalRate;
            CriticalDamage = CoreStats.StartCriticalDamage;
            MaxHP = (uint)(ushort)Math.Min(65535F, (float)(14.0 + ((double)Level / (double)CoreStats.HpGain + (double)CoreStats.HpGainRate) * (double)Level));
            MinAC = (double)CoreStats.MinAc != 0.0 ? (ushort)Math.Min(65535F, Math.Max((float)Level / CoreStats.MinAc, 0.0F)) : (ushort)0;
            MaxAC = (double)CoreStats.MaxAc != 0.0 ? (ushort)Math.Min(65535F, Math.Max((float)Level / CoreStats.MaxAc, 0.0F)) : (ushort)0;
            MinMAC = (double)CoreStats.MinMac != 0.0 ? (ushort)Math.Min(65535F, Math.Max((float)((double)Level / (double)CoreStats.MinMac / 2.0), 0.0F)) : (ushort)0;
            MaxMAC = (double)CoreStats.MaxMac != 0.0 ? (ushort)Math.Min(65535F, Math.Max((float)((double)Level / (double)CoreStats.MaxMac + 1.0), 0.0F)) : (ushort)0;
            MinDC = 0;
            MaxDC = 0;
            MinMC = 0;
            MaxMC = 0;
            MinSC = 0;
            MaxSC = 0;
            CriticalRate = (byte)Math.Min(255F, (double)CoreStats.CritialRateGain > 0.0 ? (float)CriticalRate + (float)Level / CoreStats.CritialRateGain : (float)CriticalRate);
            CriticalDamage = (byte)Math.Min(255F, (double)CoreStats.CriticalDamageGain > 0.0 ? (float)CriticalDamage + (float)Level / CoreStats.CriticalDamageGain : (float)CriticalDamage);
            MaxBagWeight = (ushort)Math.Min(65535F, (float)(50.0 + (double)Level / (double)CoreStats.BagWeightGain * (double)Level));
            MaxWearWeight = (ushort)Math.Min(65535F, (float)(15.0 + (double)Level / (double)CoreStats.WearWeightGain * (double)Level));
            MaxHandWeight = (ushort)Math.Min(65535F, (float)(12.0 + (double)Level / (double)CoreStats.HandWeightGain * (double)Level));
            MagicResist = Math.Min(byte.MaxValue, (byte)1);
            switch (Class)
            {
                case MirClass.Warrior:
                    MaxHP = (uint)(ushort)Math.Min(65535F, (float)(14.0 + ((double)Level / (double)CoreStats.HpGain + (double)CoreStats.HpGainRate + (double)Level / 20.0) * (double)Level));
                    MaxMP = (uint)(ushort)Math.Min(65535F, (float)(11.0 + (double)CoreStats.MpGainRate * (double)Level));
                    MinDC = (ushort)Math.Min(65535F, Math.Max((float)((double)Level / (double)CoreStats.MinDc - 1.0), 1f));
                    MaxDC = (ushort)Math.Min(65535F, Math.Max((float)Level / CoreStats.MaxDc, 1f));
                    break;
                case MirClass.Wizard:
                    MaxMP = (uint)(ushort)Math.Min(65535F, (float)(13.0 + ((double)Level / 5.0 + 2.0) * (double)CoreStats.MpGainRate * (double)Level));
                    MinDC = (ushort)Math.Min(65535F, Math.Max((float)((double)Level / (double)CoreStats.MinDc - 1.0), 0.0F));
                    MaxDC = (ushort)Math.Min(65535F, Math.Max((float)Level / CoreStats.MaxDc, 1f));
                    MinMC = (ushort)Math.Min(65535F, Math.Max((float)((double)Level / (double)CoreStats.MinMc - 1.0), 0.0F));
                    MaxMC = (ushort)Math.Min(65535F, Math.Max((float)Level / CoreStats.MaxMc, 1f));
                    break;
                case MirClass.Taoist:
                    MaxMP = (uint)(ushort)Math.Min(65535F, (float)(13.0 + (double)Level / 8.0 * (double)CoreStats.MpGainRate * (double)Level));
                    MinDC = (ushort)Math.Min(65535F, Math.Max((float)((double)Level / (double)CoreStats.MinDc - 1.0), 0.0F));
                    MaxDC = (ushort)Math.Min(65535F, Math.Max((float)Level / CoreStats.MaxDc, 1f));
                    MinSC = (ushort)Math.Min(65535F, Math.Max((float)((double)Level / (double)CoreStats.MinSc - 1.0), 0.0F));
                    MaxSC = (ushort)Math.Min(65535F, Math.Max((float)Level / CoreStats.MaxSc, 1f));
                    break;
                case MirClass.Assassin:
                    MaxMP = (uint)(ushort)Math.Min(65535F, (float)(11.0 + (double)Level * 5.0 + (double)Level * (double)CoreStats.MpGainRate));
                    MinDC = (ushort)Math.Min(65535F, Math.Max((float)((double)Level / (double)CoreStats.MinDc - 1.0), 1f));
                    MaxDC = (ushort)Math.Min(65535F, Math.Max((float)Level / CoreStats.MaxDc, 1f));
                    break;
                case MirClass.Archer:
                    MaxMP = (uint)(ushort)Math.Min(65535F, (float)(13.0 + ((double)Level / 11.0 + 2.0) * (double)CoreStats.MpGainRate * (double)Level));
                    MinDC = (ushort)Math.Min(65535F, Math.Max((float)((double)Level / (double)CoreStats.MinDc - 1.0), 1f));
                    MaxDC = (ushort)Math.Min(65535F, Math.Max((float)Level / CoreStats.MaxDc, 1f));
                    MinMC = (ushort)Math.Min(65535F, Math.Max((float)((double)Level / (double)CoreStats.MinMc - 1.0), 1f));
                    MaxMC = (ushort)Math.Min(65535F, Math.Max((float)Level / CoreStats.MaxMc, 1f));
                    MaxAC = (double)CoreStats.MaxAc != 0.0 ? (ushort)Math.Min((float)short.MaxValue, Math.Max((float)Level / CoreStats.MaxAc, 3f)) : (ushort)3;
                    break;
            }
            MinAC = (ushort)Math.Min((int)ushort.MaxValue, (int)MinAC + (int)AAC);
            MaxAC = (ushort)Math.Min((int)ushort.MaxValue, (int)MaxAC + (int)AAC);
            MinMAC = (ushort)Math.Min((int)ushort.MaxValue, (int)MinMAC + (int)AMAC);
            MaxMAC = (ushort)Math.Min((int)ushort.MaxValue, (int)MaxMAC + (int)AMAC);
            MinDC = (ushort)Math.Min((int)ushort.MaxValue, (int)MinDC + (int)ADC);
            MaxDC = (ushort)Math.Min((int)ushort.MaxValue, (int)MaxDC + (int)ADC);
            MinMC = (ushort)Math.Min((int)ushort.MaxValue, (int)MinMC + (int)AMC);
            MaxMC = (ushort)Math.Min((int)ushort.MaxValue, (int)MaxMC + (int)AMC);
            MinSC = (ushort)Math.Min((int)ushort.MaxValue, (int)MinSC + (int)ASC);
            MaxSC = (ushort)Math.Min((int)ushort.MaxValue, (int)MaxSC + (int)ASC);
        }

        private void RefreshEquipmentStats()//刷新装备属性
        {
            Weapon = -1;
            //WeaponEffect = 0;
            Armour = 0;
            WingEffect = 0;
            MountType = -1;

            CurrentWearWeight = 0;
            CurrentHandWeight = 0;

            FastRun = false;
            short Macrate = 0, Acrate = 0, HPrate = 0, MPrate = 0;

            ItemSets.Clear();
            MirSet.Clear();//天龙套装移除
            RedOrchidSet.Clear();//虹膜套装移除
            RedFlowerSet.Clear();//魔血套装移除

            for (int i = 0; i < Equipment.Length; i++)
            {
                UserItem temp = Equipment[i];

                if (temp == null)
                    continue;

                ItemInfo RealItem = Functions.GetRealItem(temp.Info, Level, Class, GameScene.ItemInfoList);
                if (RealItem.Type == ItemType.Weapon || RealItem.Type == ItemType.Torch)
                    CurrentHandWeight = (ushort)Math.Min(ushort.MaxValue, CurrentHandWeight + temp.Weight);
                else
                    CurrentWearWeight = (ushort)Math.Min(ushort.MaxValue, CurrentWearWeight + temp.Weight);

                if (temp.CurrentDura == 0 && RealItem.Durability > 0) continue;


                MinAC = (ushort)Math.Min(ushort.MaxValue, MinAC + RealItem.MinAC + temp.Awake.GetAC());
                MaxAC = (ushort)Math.Min(ushort.MaxValue, MaxAC + RealItem.MaxAC + temp.AC + temp.Awake.GetAC());

                MinMAC = (ushort)Math.Min(ushort.MaxValue, MinMAC + RealItem.MinMAC + temp.Awake.GetMAC());
                MaxMAC = (ushort)Math.Min(ushort.MaxValue, MaxMAC + RealItem.MaxMAC + temp.MAC + temp.Awake.GetMAC());

                MinDC = (ushort)Math.Min(ushort.MaxValue, MinDC + RealItem.MinDC + temp.Awake.GetDC());
                MaxDC = (ushort)Math.Min(ushort.MaxValue, MaxDC + RealItem.MaxDC + temp.DC + temp.Awake.GetDC());

                MinMC = (ushort)Math.Min(ushort.MaxValue, MinMC + RealItem.MinMC + temp.Awake.GetMC());
                MaxMC = (ushort)Math.Min(ushort.MaxValue, MaxMC + RealItem.MaxMC + temp.MC + temp.Awake.GetMC());

                MinSC = (ushort)Math.Min(ushort.MaxValue, MinSC + RealItem.MinSC + temp.Awake.GetSC());
                MaxSC = (ushort)Math.Min(ushort.MaxValue, MaxSC + RealItem.MaxSC + temp.SC + temp.Awake.GetSC());

                Accuracy = (byte)Math.Min(byte.MaxValue, Accuracy + RealItem.Accuracy + temp.Accuracy);
                Agility = (byte)Math.Min(byte.MaxValue, Agility + RealItem.Agility + temp.Agility);
                MaxHP = (ushort)Math.Min(ushort.MaxValue, MaxHP + RealItem.HP + temp.HP + temp.Awake.GetHPMP());
                MaxMP = (ushort)Math.Min(ushort.MaxValue, MaxMP + RealItem.MP + temp.MP + temp.Awake.GetHPMP());

                ASpeed = (sbyte)Math.Max(sbyte.MinValue, (Math.Min(sbyte.MaxValue, ASpeed + temp.AttackSpeed + RealItem.AttackSpeed)));
                Luck = (sbyte)Math.Max(sbyte.MinValue, (Math.Min(sbyte.MaxValue, Luck + temp.Luck + RealItem.Luck)));

                MaxBagWeight = (ushort)Math.Max(ushort.MinValue, (Math.Min(ushort.MaxValue, MaxBagWeight + RealItem.BagWeight)));
                MaxWearWeight = (ushort)Math.Max(ushort.MinValue, (Math.Min(ushort.MaxValue, MaxWearWeight + RealItem.WearWeight)));
                MaxHandWeight = (ushort)Math.Max(ushort.MinValue, (Math.Min(ushort.MaxValue, MaxHandWeight + RealItem.HandWeight)));
                HPrate = (short)Math.Max(short.MinValue, Math.Min(short.MaxValue, HPrate + RealItem.HPrate));
                MPrate = (short)Math.Max(short.MinValue, Math.Min(short.MaxValue, MPrate + RealItem.MPrate));
                Acrate = (short)Math.Max(short.MinValue, Math.Min(short.MaxValue, Acrate + RealItem.MaxAcRate));
                Macrate = (short)Math.Max(short.MinValue, Math.Min(short.MaxValue, Macrate + RealItem.MaxMacRate));
                MagicResist = (byte)Math.Max(0, Math.Min((int)byte.MaxValue, (int)MagicResist + (int)temp.MagicResist + (int)RealItem.MagicResist));
                PoisonResist = (byte)Math.Max(byte.MinValue, (Math.Min(byte.MaxValue, PoisonResist + temp.PoisonResist + RealItem.PoisonResist)));
                HealthRecovery = (byte)Math.Max(byte.MinValue, (Math.Min(byte.MaxValue, HealthRecovery + temp.HealthRecovery + RealItem.HealthRecovery)));
                SpellRecovery = (byte)Math.Max(byte.MinValue, (Math.Min(byte.MaxValue, SpellRecovery + temp.ManaRecovery + RealItem.SpellRecovery)));
                PoisonRecovery = (byte)Math.Max(byte.MinValue, (Math.Min(byte.MaxValue, PoisonRecovery + temp.PoisonRecovery + RealItem.PoisonRecovery)));
                CriticalRate = (byte)Math.Max(byte.MinValue, (Math.Min(byte.MaxValue, CriticalRate + temp.CriticalRate + RealItem.CriticalRate)));
                CriticalDamage = (byte)Math.Max(byte.MinValue, (Math.Min(byte.MaxValue, CriticalDamage + temp.CriticalDamage + RealItem.CriticalDamage)));
                Holy = (byte)Math.Max(byte.MinValue, (Math.Min(byte.MaxValue, Holy + RealItem.Holy)));
                Freezing = (byte)Math.Max(byte.MinValue, (Math.Min(byte.MaxValue, Freezing + temp.Freezing + RealItem.Freezing)));
                PoisonAttack = (byte)Math.Max(byte.MinValue, (Math.Min(byte.MaxValue, PoisonAttack + temp.PoisonAttack + RealItem.PoisonAttack)));
                HpDrainRate = (byte)Math.Max(100, Math.Min(byte.MaxValue, HpDrainRate + RealItem.HpDrainRate));


                if (RealItem.Light > Light)
                    Light = RealItem.Light;


                if (RealItem.CanFastRun)
                    FastRun = true;

                if (RealItem.Type == ItemType.Armour)
                {
                    Armour = RealItem.Shape;
                    WingEffect = RealItem.Effect;
                }

                if (RealItem.Type == ItemType.Weapon)
                {
                    Weapon = RealItem.Shape;
                    //WeaponEffect = RealItem.Effect;
                }

                if (RealItem.Type == ItemType.Mount)
                    MountType = RealItem.Shape;

                if (RealItem.Set == ItemSet.None) continue;

                ItemSets itemSet = ItemSets.Where(set => set.Set == RealItem.Set && !set.Type.Contains(RealItem.Type) && !set.SetComplete).FirstOrDefault();

                if (itemSet != null)
                {
                    itemSet.Type.Add(RealItem.Type);
                    itemSet.Count++;
                }
                else
                {
                    ItemSets.Add(new ItemSets { Count = 1, Set = RealItem.Set, Type = new List<ItemType> { RealItem.Type } });
                }

                if (RealItem.Set == ItemSet.Mir && !MirSet.Contains((EquipmentSlot)i))//天龙套装
                    MirSet.Add((EquipmentSlot)i);

                //虹膜套装
                if (RealItem.Set == ItemSet.RedOrchid && !RedOrchidSet.Contains((EquipmentSlot)i))
                    RedOrchidSet.Add((EquipmentSlot)i);

                //魔血套装
                if (RealItem.Set == ItemSet.RedFlower && !RedFlowerSet.Contains((EquipmentSlot)i))
                    RedFlowerSet.Add((EquipmentSlot)i);


            }

            MaxHP = (ushort)Math.Min(ushort.MaxValue, (((double)HPrate / 100) + 1) * MaxHP);
            MaxMP = (ushort)Math.Min(ushort.MaxValue, (((double)MPrate / 100) + 1) * MaxMP);
            MaxAC = (ushort)Math.Min(ushort.MaxValue, (((double)Acrate / 100) + 1) * MaxAC);
            MaxMAC = (ushort)Math.Min(ushort.MaxValue, (((double)Macrate / 100) + 1) * MaxMAC);
        }

        private void RefreshItemSetStats()//刷新物品设置属性
        {
            foreach (var s in ItemSets)
            {
                if ((s.Set == ItemSet.Smash) && (s.Type.Contains(ItemType.Ring)) && (s.Type.Contains(ItemType.Bracelet)))//如果破碎套装 装备了Ring和Bracelet
                    ASpeed = (sbyte)Math.Min(sbyte.MaxValue, ASpeed + 2);//攻击速度+2
                if ((s.Set == ItemSet.Purity) && (s.Type.Contains(ItemType.Ring)) && (s.Type.Contains(ItemType.Bracelet)))//如果灵玉套装 装备了Ring和Bracelet
                    Holy = Math.Min(byte.MaxValue, (byte)(Holy + 3));//神圣+3
                if ((s.Set == ItemSet.HwanDevil) && (s.Type.Contains(ItemType.Ring)) && (s.Type.Contains(ItemType.Bracelet)))//如果幻魔石套装 装备了Ring和Bracelet
                {
                    MaxWearWeight = (ushort)Math.Min(ushort.MaxValue, MaxWearWeight + 5);//穿戴负重+5
                    MaxBagWeight = (ushort)Math.Min(ushort.MaxValue, MaxBagWeight + 20);//背包负重+20
                }

                if (!s.SetComplete) continue;
                switch (s.Set)
                {
                    case ItemSet.Mundane://平凡套装
                        MaxHP = (ushort)Math.Min(ushort.MaxValue, MaxHP + 50);
                        break;
                    case ItemSet.NokChi://魔力套装
                        MaxMP = (ushort)Math.Min(ushort.MaxValue, MaxMP + 50);
                        break;
                    case ItemSet.TaoProtect://道护套装
                        MaxHP = (ushort)Math.Min(ushort.MaxValue, MaxHP + 30);
                        MaxMP = (ushort)Math.Min(ushort.MaxValue, MaxMP + 30);
                        break;
                    //case ItemSet.RedOrchid://虹膜套
                    //    Accuracy = (byte)Math.Min(byte.MaxValue, Accuracy + 2);
                    //    HpDrainRate = (byte)Math.Min(byte.MaxValue, HpDrainRate + 10);
                    //    break;
                    //case ItemSet.RedFlower://魔血套
                    //    MaxHP = (ushort)Math.Min(ushort.MaxValue, MaxHP + 50);
                    //    MaxMP = (ushort)Math.Min(ushort.MaxValue, MaxMP - 25);
                    //    break;
                    case ItemSet.Smash://破碎套装
                        MinDC = (ushort)Math.Min(ushort.MaxValue, MinDC + 1);
                        MaxDC = (ushort)Math.Min(ushort.MaxValue, MaxDC + 3);
                        //ASpeed = (sbyte)Math.Min(sbyte.MaxValue, ASpeed + 2);
                        break;
                    case ItemSet.HwanDevil://幻魔石套装
                        MinMC = (ushort)Math.Min(ushort.MaxValue, MinMC + 1);
                        MaxMC = (ushort)Math.Min(ushort.MaxValue, MaxMC + 2);
                        //MaxBagWeight = (ushort)Math.Min(ushort.MaxValue, MaxBagWeight + 20);
                        //MaxWearWeight = (ushort)Math.Min(ushort.MaxValue, MaxWearWeight + 5);
                        break;
                    case ItemSet.Purity://灵玉套装
                        MinSC = (ushort)Math.Min(ushort.MaxValue, MinSC + 1);
                        MaxSC = (ushort)Math.Min(ushort.MaxValue, MaxSC + 2);
                        //Holy = (byte)Math.Min(ushort.MaxValue, Holy + 3);
                        break;
                    case ItemSet.FiveString://五玄套装
                        MaxHP = (ushort)Math.Min(ushort.MaxValue, MaxHP + ((MaxHP / 100) * 30));
                        MinAC = (ushort)Math.Min(ushort.MaxValue, MinAC + 2);
                        MaxAC = (ushort)Math.Min(ushort.MaxValue, MaxAC + 2);
                        break;
                    case ItemSet.Spirit://祈祷套装
                        MinDC = (ushort)Math.Min(ushort.MaxValue, MinDC + 2);
                        MaxDC = (ushort)Math.Min(ushort.MaxValue, MaxDC + 5);
                        ASpeed = (sbyte)Math.Min(sbyte.MaxValue, ASpeed + 2);
                        break;
                    case ItemSet.Bone://白骨套装
                        MaxAC = (ushort)Math.Min(ushort.MaxValue, MaxAC + 2);
                        MaxMC = (ushort)Math.Min(ushort.MaxValue, MaxMC + 1);
                        MaxSC = (ushort)Math.Min(ushort.MaxValue, MaxSC + 1);
                        break;
                    case ItemSet.Bug://恶虫套装
                        MaxDC = (ushort)Math.Min(ushort.MaxValue, MaxDC + 1);
                        MaxMC = (ushort)Math.Min(ushort.MaxValue, MaxMC + 1);
                        MaxSC = (ushort)Math.Min(ushort.MaxValue, MaxSC + 1);
                        MaxMAC = (ushort)Math.Min(ushort.MaxValue, MaxMAC + 1);
                        PoisonResist = (byte)Math.Min(byte.MaxValue, PoisonResist + 1);
                        break;
                    case ItemSet.WhiteGold://白金套装
                        MaxDC = (ushort)Math.Min(ushort.MaxValue, MaxDC + 2);
                        MaxAC = (ushort)Math.Min(ushort.MaxValue, MaxAC + 2);
                        break;
                    case ItemSet.WhiteGoldH://强化白金套装
                        MaxDC = (ushort)Math.Min(ushort.MaxValue, MaxDC + 3);
                        MaxHP = (ushort)Math.Min(ushort.MaxValue, MaxHP + 30);
                        ASpeed = (sbyte)Math.Min(sbyte.MaxValue, ASpeed + 2);
                        break;
                    case ItemSet.RedJade://红玉套装
                        MaxMC = (ushort)Math.Min(ushort.MaxValue, MaxMC + 2);
                        MaxMAC = (ushort)Math.Min(ushort.MaxValue, MaxMAC + 2);
                        break;
                    case ItemSet.RedJadeH://强化红玉套装
                        MaxMC = (ushort)Math.Min(ushort.MaxValue, MaxMC + 2);
                        MaxMP = (ushort)Math.Min(ushort.MaxValue, MaxMP + 40);
                        Agility = (byte)Math.Min(byte.MaxValue, Agility + 2);
                        break;
                    case ItemSet.Nephrite://软玉套装
                        MaxSC = (ushort)Math.Min(ushort.MaxValue, MaxSC + 2);
                        MaxAC = (ushort)Math.Min(ushort.MaxValue, MaxAC + 1);
                        MaxMAC = (ushort)Math.Min(ushort.MaxValue, MaxMAC + 1);
                        break;
                    case ItemSet.NephriteH://强化软玉套装
                        MaxSC = (ushort)Math.Min(ushort.MaxValue, MaxSC + 2);
                        MaxHP = (ushort)Math.Min(ushort.MaxValue, MaxHP + 15);
                        MaxMP = (ushort)Math.Min(ushort.MaxValue, MaxMP + 20);
                        Holy = (byte)Math.Min(byte.MaxValue, Holy + 1);
                        Accuracy = (byte)Math.Min(byte.MaxValue, Accuracy + 1);
                        break;
                    case ItemSet.Whisker1://贵宾套装 战士
                        MinDC = (ushort)Math.Min(ushort.MaxValue, MinDC + 1);
                        MaxDC = (ushort)Math.Min(ushort.MaxValue, MaxDC + 1);
                        MaxBagWeight = (ushort)Math.Min(ushort.MaxValue, MaxBagWeight + 25);
                        break;
                    case ItemSet.Whisker2:
                        MinMC = (ushort)Math.Min(ushort.MaxValue, MinMC + 1);
                        MaxMC = (ushort)Math.Min(ushort.MaxValue, MaxMC + 1);
                        MaxBagWeight = (ushort)Math.Min(ushort.MaxValue, MaxBagWeight + 17);
                        break;
                    case ItemSet.Whisker3:
                        MinSC = (ushort)Math.Min(ushort.MaxValue, MinSC + 1);
                        MaxSC = (ushort)Math.Min(ushort.MaxValue, MaxSC + 1);
                        MaxBagWeight = (ushort)Math.Min(ushort.MaxValue, MaxBagWeight + 17);
                        break;
                    case ItemSet.Whisker4:
                        MinDC = (ushort)Math.Min(ushort.MaxValue, MinDC + 1);
                        MaxDC = (ushort)Math.Min(ushort.MaxValue, MaxDC + 1);
                        MaxBagWeight = (ushort)Math.Min(ushort.MaxValue, MaxBagWeight + 20);
                        break;
                    case ItemSet.Whisker5:
                        MinDC = (ushort)Math.Min(ushort.MaxValue, MinDC + 1);
                        MaxDC = (ushort)Math.Min(ushort.MaxValue, MaxDC + 1);
                        MinMC = (ushort)Math.Min(ushort.MaxValue, MinMC + 1);
                        MaxMC = (ushort)Math.Min(ushort.MaxValue, MaxMC + 1);
                        MaxBagWeight = (ushort)Math.Min(ushort.MaxValue, MaxBagWeight + 20);
                        break;
                    case ItemSet.Hyeolryong://血龙套装
                        MaxSC = (ushort)Math.Min(ushort.MaxValue, MaxSC + 2);
                        MaxHP = (ushort)Math.Min(ushort.MaxValue, MaxHP + 15);
                        MaxMP = (ushort)Math.Min(ushort.MaxValue, MaxMP + 20);
                        Holy = (byte)Math.Min(byte.MaxValue, Holy + 1);
                        Accuracy = (byte)Math.Min(byte.MaxValue, Accuracy + 1);
                        break;
                    case ItemSet.Monitor://监视者套装
                        MagicResist = (byte)Math.Min(byte.MaxValue, MagicResist + 1);
                        PoisonResist = (byte)Math.Min(byte.MaxValue, PoisonResist + 1);
                        break;
                    case ItemSet.Oppressive://暴压套装
                        MaxAC = (ushort)Math.Min(ushort.MaxValue, MaxAC + 1);
                        Agility = (byte)Math.Min(byte.MaxValue, Agility + 1);//敏捷+1
                        break;
                    case ItemSet.Paeok://贝玉套装 自己添加的
                        MagicResist = (byte)Math.Min(byte.MaxValue, MagicResist + 2);//魔法躲避+2
                        Agility = (byte)Math.Min(byte.MaxValue, Agility + 1);//敏捷+1
                        break;
                    case ItemSet.Sulgwan://黑术套装 自己添加的
                        PoisonResist = (byte)Math.Min(byte.MaxValue, PoisonResist + 2);//毒物躲避+2
                        Accuracy = (byte)Math.Min(byte.MaxValue, Accuracy + 1);//准确+1
                        break;
                }
            }
        }

        private void RefreshMirSetStats()//刷新天龙套设置属性
        {
            if (MirSet.Count() == 10)//全部穿戴
            {
                MinAC = (ushort)Math.Min(ushort.MaxValue, MaxAC + 1);//最小防御+1
                MinMAC = (ushort)Math.Min(ushort.MaxValue, MaxMAC + 1);//最小魔御+1
                MaxAC = (ushort)Math.Min(ushort.MaxValue, MaxAC + 4);//最大防御+4
                MaxMAC = (ushort)Math.Min(ushort.MaxValue, MaxMAC + 4);//最大魔御+4
                MaxBagWeight = (ushort)Math.Min(ushort.MaxValue, MaxBagWeight + 120);//最大背包负重+120
                MaxWearWeight = (ushort)Math.Min(ushort.MaxValue, MaxWearWeight + 27);//最大穿戴负重+27
                MaxHandWeight = (ushort)Math.Min(ushort.MaxValue, MaxHandWeight + 34);//腕力+34
                Luck = (sbyte)Math.Min(sbyte.MaxValue, Luck + 2);//幸运+2
                ASpeed = (sbyte)Math.Min(int.MaxValue, ASpeed + 2);//攻击速度+2
                MaxHP = (ushort)Math.Min(ushort.MaxValue, MaxHP + 70);//HP上限+70
                MaxMP = (ushort)Math.Min(ushort.MaxValue, MaxMP + 80);//MP上限+80
                MagicResist = (byte)Math.Min(byte.MaxValue, MagicResist + 6);//魔法躲避+6
                PoisonResist = (byte)Math.Min(byte.MaxValue, PoisonResist + 6);//毒物躲避+6
            }

            if (MirSet.Contains(EquipmentSlot.RingL) && MirSet.Contains(EquipmentSlot.RingR))//左Ring和右Ring
            {
                MaxMAC = (ushort)Math.Min(ushort.MaxValue, MaxMAC + 1);//最大防御+1
                MaxAC = (ushort)Math.Min(ushort.MaxValue, MaxAC + 1);//最大魔御+1
            }
            if (MirSet.Contains(EquipmentSlot.BraceletL) && MirSet.Contains(EquipmentSlot.BraceletR))//左Bracelet和右Bracelet
            {
                MinAC = (ushort)Math.Min(ushort.MaxValue, MinAC + 1);//最小防御+1
                MinMAC = (ushort)Math.Min(ushort.MaxValue, MinMAC + 1);//最小魔御+1
            }
            if ((MirSet.Contains(EquipmentSlot.RingL) | MirSet.Contains(EquipmentSlot.RingR)) && (MirSet.Contains(EquipmentSlot.BraceletL) | MirSet.Contains(EquipmentSlot.BraceletR)) && MirSet.Contains(EquipmentSlot.Necklace))//一个Ring 一个Bracelet 一个项链
            {
                MaxMAC = (ushort)Math.Min(ushort.MaxValue, MaxMAC + 1);//最大魔御+1
                MaxAC = (ushort)Math.Min(ushort.MaxValue, MaxAC + 1);//最大防御+1
                MaxBagWeight = (ushort)Math.Min(ushort.MaxValue, MaxBagWeight + 30);//最大背包负重+30
                MaxWearWeight = (ushort)Math.Min(ushort.MaxValue, MaxWearWeight + 17);//最大穿戴负重+17
            }
            if (MirSet.Contains(EquipmentSlot.RingL) && MirSet.Contains(EquipmentSlot.RingR) && MirSet.Contains(EquipmentSlot.BraceletL) && MirSet.Contains(EquipmentSlot.BraceletR) && MirSet.Contains(EquipmentSlot.Necklace))//双Ring 双Bracelet 项链
            {
                MinMAC = (ushort)Math.Min(ushort.MaxValue, MaxMAC + 1);
                MinAC = (ushort)Math.Min(ushort.MaxValue, MaxAC + 1);
                MaxMAC = (ushort)Math.Min(ushort.MaxValue, MaxMAC + 3);
                MaxAC = (ushort)Math.Min(ushort.MaxValue, MaxAC + 3);
                MaxBagWeight = (ushort)Math.Min(ushort.MaxValue, MaxBagWeight + 50);//最大背包负重+50
                MaxWearWeight = (ushort)Math.Min(ushort.MaxValue, MaxWearWeight + 27);//最大穿戴负重+27
            }
            if (MirSet.Contains(EquipmentSlot.Armour) && MirSet.Contains(EquipmentSlot.Helmet) && MirSet.Contains(EquipmentSlot.Weapon))//盔甲 头盔 Weapon
            {
                MaxDC = (ushort)Math.Min(ushort.MaxValue, MaxDC + 2);
                MaxMC = (ushort)Math.Min(ushort.MaxValue, MaxMC + 1);
                MaxSC = (ushort)Math.Min(ushort.MaxValue, MaxSC + 1);
                Agility = (byte)Math.Min(byte.MaxValue, Agility + 1);//敏捷+1
            }
            if (MirSet.Contains(EquipmentSlot.Armour) && MirSet.Contains(EquipmentSlot.Boots) && MirSet.Contains(EquipmentSlot.Belt))//盔甲 靴子 腰带
            {
                MaxDC = (ushort)Math.Min(ushort.MaxValue, MaxDC + 1);
                MaxMC = (ushort)Math.Min(ushort.MaxValue, MaxMC + 1);
                MaxSC = (ushort)Math.Min(ushort.MaxValue, MaxSC + 1);
                MaxHandWeight = (ushort)Math.Min(ushort.MaxValue, MaxHandWeight + 17);//腕力+17
            }
            if (MirSet.Contains(EquipmentSlot.Armour) && MirSet.Contains(EquipmentSlot.Boots) && MirSet.Contains(EquipmentSlot.Belt) && MirSet.Contains(EquipmentSlot.Helmet) && MirSet.Contains(EquipmentSlot.Weapon))//盔甲 靴子 腰带 头盔 Weapon
            {
                MinDC = (ushort)Math.Min(ushort.MaxValue, MinDC + 1);
                MaxDC = (ushort)Math.Min(ushort.MaxValue, MaxDC + 4);
                MinMC = (ushort)Math.Min(ushort.MaxValue, MinMC + 1);
                MaxMC = (ushort)Math.Min(ushort.MaxValue, MaxMC + 3);
                MinSC = (ushort)Math.Min(ushort.MaxValue, MinSC + 1);
                MaxSC = (ushort)Math.Min(ushort.MaxValue, MaxSC + 3);
                MaxHandWeight = (ushort)Math.Min(ushort.MaxValue, MaxHandWeight + 34);//腕力+34
                Agility = (byte)Math.Min(byte.MaxValue, Agility + 1);//敏捷+1
            }
        }

        private void RefreshRedOrchidSetStats()//刷新虹膜套设置属性
        {
            if (RedOrchidSet.Contains(EquipmentSlot.Necklace))//项链
            {
                HpDrainRate = (byte)Math.Min(byte.MaxValue, HpDrainRate + 2);
            }
            if (RedOrchidSet.Contains(EquipmentSlot.BraceletL) | RedOrchidSet.Contains(EquipmentSlot.BraceletR))//左Bracelet或右Bracelet
            {
                HpDrainRate = (byte)Math.Min(byte.MaxValue, HpDrainRate + 2);//基础
            }
            if (RedOrchidSet.Contains(EquipmentSlot.RingL) | RedOrchidSet.Contains(EquipmentSlot.RingR))//左Ring或右Ring
            {
                HpDrainRate = (byte)Math.Min(byte.MaxValue, HpDrainRate + 10);//基础
            }
            if (RedOrchidSet.Contains(EquipmentSlot.BraceletL) && RedOrchidSet.Contains(EquipmentSlot.BraceletR))//左Bracelet 右Bracelet
            {
                HpDrainRate = (byte)Math.Min(byte.MaxValue, HpDrainRate + 2);//在基础上+2
            }
            if (RedOrchidSet.Contains(EquipmentSlot.RingL) && RedOrchidSet.Contains(EquipmentSlot.RingR))//左Ring 右Ring
            {
                HpDrainRate = (byte)Math.Min(byte.MaxValue, HpDrainRate + 10);//在基础上+10
            }
            if (RedOrchidSet.Contains(EquipmentSlot.Necklace) && (RedOrchidSet.Contains(EquipmentSlot.RingL) | RedOrchidSet.Contains(EquipmentSlot.RingR)) && (RedOrchidSet.Contains(EquipmentSlot.BraceletL) | RedOrchidSet.Contains(EquipmentSlot.BraceletR)))//项链 一个Ring 一个Bracelet
            {
                Accuracy = (byte)Math.Min(byte.MaxValue, Accuracy + 2);
            }
        }

        private void RefreshRedFlowerSetStats()//刷新魔血套设置属性
        {
            if (RedFlowerSet.Contains(EquipmentSlot.Necklace))//项链
            {
                MaxHP = (ushort)Math.Min(ushort.MaxValue, MaxHP + 25);//HP上限+25
                MaxMP = (ushort)Math.Min(ushort.MaxValue, MaxMP - 25);//MP上限-25
            }
            if (RedFlowerSet.Contains(EquipmentSlot.BraceletL) | RedFlowerSet.Contains(EquipmentSlot.BraceletR))//左Bracelet或右Bracelet
            {
                MaxHP = (ushort)Math.Min(ushort.MaxValue, MaxHP + 25);//HP上限+25
                MaxMP = (ushort)Math.Min(ushort.MaxValue, MaxMP - 25);//MP上限-25
            }
            if (RedFlowerSet.Contains(EquipmentSlot.RingL) | RedFlowerSet.Contains(EquipmentSlot.RingR))//左Ring或右Ring
            {
                MaxHP = (ushort)Math.Min(ushort.MaxValue, MaxHP + 25);//HP上限+25
                MaxMP = (ushort)Math.Min(ushort.MaxValue, MaxMP - 25);//MP上限-25
            }
            if (RedFlowerSet.Contains(EquipmentSlot.BraceletL) && RedFlowerSet.Contains(EquipmentSlot.BraceletR))//左Bracelet 右Bracelet
            {
                MaxHP = (ushort)Math.Min(ushort.MaxValue, MaxHP + 25);//HP上限+25
                MaxMP = (ushort)Math.Min(ushort.MaxValue, MaxMP - 25);//MP上限-25
            }
            if (RedFlowerSet.Contains(EquipmentSlot.RingL) && RedFlowerSet.Contains(EquipmentSlot.RingR))//左Ring 右Ring
            {
                MaxHP = (ushort)Math.Min(ushort.MaxValue, MaxHP + 25);//HP上限+25
                MaxMP = (ushort)Math.Min(ushort.MaxValue, MaxMP - 25);//MP上限-25
            }
            if (RedFlowerSet.Contains(EquipmentSlot.Necklace) && (RedFlowerSet.Contains(EquipmentSlot.RingL) | RedFlowerSet.Contains(EquipmentSlot.RingR)) && (RedFlowerSet.Contains(EquipmentSlot.BraceletL) | RedFlowerSet.Contains(EquipmentSlot.BraceletR)))//项链 一个Ring 一个Bracelet
            {
                MaxHP = (ushort)Math.Min(ushort.MaxValue, MaxHP + 50);//HP上限+50
                MaxMP = (ushort)Math.Min(ushort.MaxValue, MaxMP + 50);//MP上限+50
            }
        }

        private void RefreshSkills()//刷新技能
        {
            for (int i = 0; i < Magics.Count; i++)
            {
                ClientMagic magic = Magics[i];
                switch (magic.Spell)
                {
                    case Spell.Fencing:
                        Accuracy = (byte)Math.Min(byte.MaxValue, Accuracy + magic.Level * 3);//准确
                        break;
                    case Spell.Slaying://添加攻杀 根据等级提高准确
                        Accuracy = (byte)Math.Min(byte.MaxValue, Accuracy + magic.Level);
                        break;
                    case Spell.FatalSword:
                        Accuracy = (byte)Math.Min(byte.MaxValue, Accuracy + magic.Level);
                        break;
                    case Spell.SpiritSword:
                        Accuracy = (byte)Math.Min(byte.MaxValue, Accuracy + (byte)Math.Round(8F / 3F * magic.Level));
                        break;
                }
            }
        }

        private void RefreshBuffs()//刷新增益
        {
            for (int i = 0; i < GameScene.Scene.HeroBuffs.Count; i++)
            {
                Buff heroBuff = GameScene.Scene.HeroBuffs[i];
                switch (heroBuff.Type)
                {
                    case BuffType.Haste:
                    case BuffType.Fury:
                        ASpeed = (sbyte)Math.Max(sbyte.MinValue, Math.Min(sbyte.MaxValue, ASpeed + heroBuff.Values[0]));
                        break;
                    case BuffType.SwiftFeet:
                        Sprint = true;
                        break;
                    case BuffType.SoulShield:
                        MaxMAC = (ushort)Math.Min(ushort.MaxValue, MaxMAC + heroBuff.Values[0]);
                        break;
                    case BuffType.BlessedArmour:
                        MaxAC = (ushort)Math.Min(ushort.MaxValue, MaxAC + heroBuff.Values[0]);
                        break;
                    case BuffType.LightBody:
                        Agility = (byte)Math.Min(byte.MaxValue, Agility + heroBuff.Values[0]);
                        break;
                    case BuffType.UltimateEnhancer:
                        MaxDC = (ushort)Math.Min(ushort.MaxValue, MaxDC + heroBuff.Values[0]);
                        break;
                    case BuffType.ProtectionField:
                        MaxAC = (ushort)Math.Min(ushort.MaxValue, MaxAC + heroBuff.Values[0]);
                        break;
                    case BuffType.Rage:
                        MaxDC = (ushort)Math.Min(ushort.MaxValue, MaxDC + heroBuff.Values[0]);
                        break;
                    case BuffType.Curse:
                        ushort rMaxDC = (ushort)(((int)MaxDC / 100) * heroBuff.Values[0]);
                        ushort rMaxMC = (ushort)(((int)MaxMC / 100) * heroBuff.Values[0]);
                        ushort rMaxSC = (ushort)(((int)MaxSC / 100) * heroBuff.Values[0]);
                        byte rASpeed = (byte)(((int)ASpeed / 100) * heroBuff.Values[0]);

                        MaxDC = (ushort)Math.Max(ushort.MinValue, MaxDC - rMaxDC);
                        MaxMC = (ushort)Math.Max(ushort.MinValue, MaxMC - rMaxMC);
                        MaxSC = (ushort)Math.Max(ushort.MinValue, MaxSC - rMaxSC);
                        ASpeed = (sbyte)Math.Min(sbyte.MaxValue, (Math.Max(sbyte.MinValue, ASpeed - rASpeed)));
                        break;
                    case BuffType.CounterAttack:
                        MinAC = (ushort)Math.Min(ushort.MaxValue, MinAC + heroBuff.Values[0]);
                        MinMAC = (ushort)Math.Min(ushort.MaxValue, MinMAC + heroBuff.Values[0]);
                        MaxAC = (ushort)Math.Min(ushort.MaxValue, MaxAC + heroBuff.Values[0]);
                        MaxMAC = (ushort)Math.Min(ushort.MaxValue, MaxMAC + heroBuff.Values[0]);
                        break;
                    case BuffType.MagicBooster:
                        MinMC = (ushort)Math.Min(ushort.MaxValue, MinMC + heroBuff.Values[0]);
                        MaxMC = (ushort)Math.Min(ushort.MaxValue, MaxMC + heroBuff.Values[0]);
                        break;
                    case BuffType.BagWeight:
                    case BuffType.Knapsack:
                        MaxBagWeight = (ushort)Math.Min(ushort.MaxValue, MaxBagWeight + heroBuff.Values[0]);
                        break;

                    case BuffType.Impact:
                        MaxDC = (ushort)Math.Min(ushort.MaxValue, MaxDC + heroBuff.Values[0]);
                        break;
                    case BuffType.Magic:
                        MaxMC = (ushort)Math.Min(ushort.MaxValue, MaxMC + heroBuff.Values[0]);
                        break;
                    case BuffType.Taoist:
                        MaxSC = (ushort)Math.Min(ushort.MaxValue, MaxSC + heroBuff.Values[0]);
                        break;
                    case BuffType.Storm:
                        ASpeed = (sbyte)Math.Max(sbyte.MinValue, Math.Min(sbyte.MaxValue, ASpeed + heroBuff.Values[0]));
                        break;
                    case BuffType.HealthAid:
                        MaxHP = (ushort)Math.Min(ushort.MaxValue, MaxHP + heroBuff.Values[0]);
                        break;
                    case BuffType.ManaAid:
                        MaxMP = (ushort)Math.Min(ushort.MaxValue, MaxMP + heroBuff.Values[0]);
                        break;
                    //case BuffType.AccuracyUp:
                    //    Accuracy = (byte)Math.Min((int)byte.MaxValue, (int)Accuracy + heroBuff.Values[0]);
                    //    break;
                    //case BuffType.CustumBuff:
                    //    MinDC = (ushort)Math.Min((int)ushort.MaxValue, (int)MinDC + heroBuff.Values[0]);
                    //    MaxDC = (ushort)Math.Min((int)ushort.MaxValue, (int)MaxDC + heroBuff.Values[1]);
                    //    MinMC = (ushort)Math.Min((int)ushort.MaxValue, (int)MinMC + heroBuff.Values[2]);
                    //    MaxMC = (ushort)Math.Min((int)ushort.MaxValue, (int)MaxMC + heroBuff.Values[3]);
                    //    MinSC = (ushort)Math.Min((int)ushort.MaxValue, (int)MinSC + heroBuff.Values[4]);
                    //    MaxSC = (ushort)Math.Min((int)ushort.MaxValue, (int)MaxSC + heroBuff.Values[5]);
                    //    Accuracy = (byte)Math.Min((int)byte.MaxValue, (int)Accuracy + heroBuff.Values[6]);
                    //    ASpeed = (sbyte)Math.Max((int)sbyte.MinValue, Math.Min((int)sbyte.MaxValue, (int)ASpeed + heroBuff.Values[7]));
                    //    Agility = (byte)Math.Min((int)byte.MaxValue, (int)Agility + heroBuff.Values[8]);
                    //    MagicResist = (byte)Math.Min((int)byte.MaxValue, (int)MagicResist + heroBuff.Values[9]);
                    //    PoisonResist = (byte)Math.Min((int)byte.MaxValue, (int)PoisonResist + heroBuff.Values[10]);
                    //    Luck = (sbyte)Math.Max((int)sbyte.MinValue, Math.Min((int)sbyte.MaxValue, (int)Luck + heroBuff.Values[11]));
                    //    MinAC = (ushort)Math.Min((int)ushort.MaxValue, (int)MinAC + heroBuff.Values[12]);
                    //    MaxAC = (ushort)Math.Min((int)ushort.MaxValue, (int)MaxAC + heroBuff.Values[13]);
                    //    MinMAC = (ushort)Math.Min((int)ushort.MaxValue, (int)MinMAC + heroBuff.Values[14]);
                    //    MaxMAC = (ushort)Math.Min((int)ushort.MaxValue, (int)MaxMAC + heroBuff.Values[15]);
                    //    MaxHP = (uint)Math.Min((long)uint.MaxValue, (long)MaxHP + (long)heroBuff.Values[16]);
                    //    MaxMP = (uint)Math.Min((long)uint.MaxValue, (long)MaxMP + (long)heroBuff.Values[17]);
                    //    break;
                    case BuffType.WonderDrug:
                        switch (heroBuff.Values[0])
                        {
                            case 2:
                                MaxHP = (ushort)Math.Min(ushort.MaxValue, MaxHP + heroBuff.Values[1]);
                                break;
                            case 3:
                                MaxMP = (ushort)Math.Min(ushort.MaxValue, MaxMP + heroBuff.Values[1]);
                                break;
                            case 4:
                                MinAC = (ushort)Math.Min(ushort.MaxValue, MinAC + heroBuff.Values[1]);
                                MaxAC = (ushort)Math.Min(ushort.MaxValue, MaxAC + heroBuff.Values[1]);
                                break;
                            case 5:
                                MinMAC = (ushort)Math.Min(ushort.MaxValue, MinMAC + heroBuff.Values[1]);
                                MaxMAC = (ushort)Math.Min(ushort.MaxValue, MaxMAC + heroBuff.Values[1]);
                                break;
                            case 6:
                                ASpeed = (sbyte)Math.Max(sbyte.MinValue, Math.Min(sbyte.MaxValue, ASpeed + heroBuff.Values[1]));
                                break;
                            default:
                                break;
                        }
                        break;
                    case BuffType.HeroWarrior:
                        MaxHP = (ushort)Math.Min(ushort.MaxValue, MaxHP + heroBuff.Values[1]);
                        break;
                    case BuffType.HeroWizard:
                        MaxMP = (ushort)Math.Min(ushort.MaxValue, MaxMP + heroBuff.Values[1]);
                        break;
                    case BuffType.HeroTaoist:
                        HealthRecovery = (byte)Math.Min(byte.MaxValue, Math.Max(0, HealthRecovery + heroBuff.Values[1]));
                        break;
                    case BuffType.HeroAssassin:
                        Accuracy = (byte)Math.Min(byte.MaxValue, Accuracy + heroBuff.Values[0]);
                        SpellRecovery = (byte)Math.Min(byte.MaxValue, Math.Max(0, SpellRecovery + heroBuff.Values[1]));
                        break;
                    case BuffType.HeroArcher:
                        Agility = (byte)Math.Min(byte.MaxValue, Agility + heroBuff.Values[0]);
                        MaxDC = (ushort)Math.Min(ushort.MaxValue, MaxDC + heroBuff.Values[1] / 2);
                        break;
                }
            }
        }

        public void BindAllItems()
        {
            for (int i = 0; i < Inventory.Length; i++)
            {
                if (Inventory[i] != null)
                    GameScene.Bind(Inventory[i]);
            }
            for (int i = 0; i < Equipment.Length; i++)
            {
                if (Equipment[i] != null)
                    GameScene.Bind(Equipment[i]);
            }
        }

        public ClientMagic GetMagic(Spell spell)
        {
            for (int i = 0; i < Magics.Count; i++)
            {
                ClientMagic magic = Magics[i];
                if (magic.Spell == spell)
                    return magic;
            }
            return null;
        }

    }
}
