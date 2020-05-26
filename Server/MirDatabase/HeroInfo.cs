using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Server.MirEnvir;
using Server.MirObjects;
using S = ServerPackets;

namespace Server.MirDatabase
{
    public class HeroInfo
    {
        [Key]
        public int Index { get; set; }
        public string Name { get; set; }
        [NotMapped]
        public ushort Level { get; set; }

        public int DBLevel
        {
            get { return Level; }
            set { Level = (ushort)value; }
        }

        public MirClass Class { get; set; }
        public MirGender Gender { get; set; }
        public byte Hair { get; set; }

        public ListViewItem ListItem;

        //Location
        public int CurrentMapIndex { get; set; }
        public Point CurrentLocation;

        public string DBCurrentLocation
        {
            get { return CurrentLocation.X + "," + CurrentLocation.Y; }
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                var tempArray = value.Split(',');
                if (tempArray.Length != 2)
                {
                    CurrentLocation.X = 0;
                    CurrentLocation.Y = 0;
                }
                else
                {
                    int result = 0;
                    int.TryParse(tempArray[0], out result);
                    CurrentLocation.X = result;
                    int.TryParse(tempArray[1], out result);
                    CurrentLocation.Y = result;
                }
            }
        }
        public MirDirection Direction { get; set; }

        [NotMapped]
        public ushort HP { get; set; }

        public int DBHP
        {
            get { return HP; }
            set { HP = (ushort)value; }
        }
        [NotMapped]
        public ushort MP { get; set; }

        public int DBMP
        {
            get { return MP; }
            set { MP = (ushort)value; }
        }
        public long Experience { get; set; }

        public AttackMode AMode { get; set; }

        public bool NewDay;

        public bool Thrusting { get; set; }
        public bool HalfMoon { get; set; }
        public bool CrossHalfMoon { get; set; }
        public bool DoubleSlash { get; set; }
        public byte MentalState { get; set; }
        public byte MentalStateLvl { get; set; }

        public UserItem[] Inventory = new UserItem[10];
        public UserItem[] Equipment = new UserItem[14];

        public List<HeroMagic> Magics = new List<HeroMagic>();
        //public List<PetInfo> Pets = new List<PetInfo>();
        public List<Buff> Buffs = new List<Buff>();
        public List<Poison> Poisons = new List<Poison>();

        public byte Grade { get; set; }
        public bool NeedUnlock { get; set; }
        public HeroMode HeroMode { get; set; }

        public int PearlCount { get; set; }

        public long HeroOrbId { get; set; } = 0;

        public PlayerObject Player;

        [ForeignKey("CharacterInfo")]
        public int CharacterIndex { get; set; }
        public CharacterInfo CharacterInfo { get; set; }

        public static bool CanLearnSpell(Spell spell)
        {
            return HeroCanUseSpells.Contains(spell);
        }

        public static List<Spell> HeroCanUseSpells = new List<Spell>()
        {
            Spell.Fencing,
            Spell.FireBall,
            Spell.Healing
        };
    }

    public class HeroInventoryItem
    {
        public long id { get; set; }

        [ForeignKey("HeroInfo")]
        public int HeroIndex { get; set; }
        public HeroInfo HeroInfo { get; set; }

        [ForeignKey("UserItem")]
        public long? ItemUniqueID { get; set; }
        public UserItem UserItem { get; set; }
    }

    public class HeroEquipmentItem
    {
        public long id { get; set; }

        [ForeignKey("HeroInfo")]
        public int HeroIndex { get; set; }
        public HeroInfo HeroInfo { get; set; }

        [ForeignKey("UserItem")]
        public long? ItemUniqueID { get; set; }
        public UserItem UserItem { get; set; }
    }

    public class HeroBuff
    {
        [Key]
        public int Index { get; set; }

        public BuffType Type { get; set; }
        [NotMapped]
        public MapObject Caster { get; set; }

        public string CasterName { get; set; }
        public bool Visible { get; set; }
        [NotMapped]
        public uint ObjectID { get; set; }

        public long DBObjectID
        {
            get { return ObjectID; }
            set { ObjectID = (uint)value; }
        }
        public long ExpireTime { get; set; }
        public int[] Values;

        public string DbValues
        {
            get { return string.Join(",", Values); }
            set { Values = value.Split(',').Select(int.Parse).ToArray(); }
        }
        public bool Infinite { get; set; }

        public bool RealTime { get; set; }
        public DateTime? RealTimeExpire { get; set; } = SqlDateTime.MinValue.Value;

        public bool Paused { get; set; }

        [ForeignKey("HeroInfo")]
        public int HeroIndex { get; set; }
        public HeroInfo HeroInfo { get; set; }
    }

    public class HeroMagic
    {
        public long id { get; set; }

        public Spell Spell { get; set; }
        [NotMapped]
        public MagicInfo Info => GetMagicInfo(Spell);

        [ForeignKey("HeroInfo")]
        public int HeroIndex { get; set; }
        public HeroInfo HeroInfo { get; set; }

        public byte Level { get; set; }
        public byte Key { get; set; }
        [NotMapped]
        public ushort Experience { get; set; }

        public int DBExperience
        {
            get { return Experience; }
            set { Experience = (ushort)value; }
        }
        public bool IsTempSpell { get; set; }
        public long CastTime { get; set; }

        private MagicInfo GetMagicInfo(Spell spell)
        {
            for (int i = 0; i < MessageQueue.Envir.MagicInfoList.Count; i++)
            {
                MagicInfo info = MessageQueue.Envir.MagicInfoList[i];
                if (info.Spell != spell) continue;
                return info;
            }
            return null;
        }

        public HeroMagic() { }

        public HeroMagic(Spell spell)
        {
            Spell = spell;

            //Info = GetMagicInfo(Spell);
        }
        public HeroMagic(BinaryReader reader)
        {
            Spell = (Spell)reader.ReadByte();
            //Info = GetMagicInfo(Spell);

            Level = reader.ReadByte();
            Key = reader.ReadByte();
            Experience = reader.ReadUInt16();

            if (Envir.LoadVersion < 15) return;
            IsTempSpell = reader.ReadBoolean();

            if (Envir.LoadVersion < 65) return;
            CastTime = reader.ReadInt64();
        }
        public void Save(BinaryWriter writer)
        {
            writer.Write((byte)Spell);

            writer.Write(Level);
            writer.Write(Key);
            writer.Write(Experience);
            writer.Write(IsTempSpell);
            writer.Write(CastTime);
        }

        public Packet GetInfo()
        {
            return new S.NewMagic
            {
                Magic = CreateClientMagic()
            };
        }

        public ClientMagic CreateClientMagic()
        {
            return new ClientMagic
            {
                Spell = Spell,
                BaseCost = Info.BaseCost,
                LevelCost = Info.LevelCost,
                Icon = Info.Icon,
                Level1 = Info.Level1,
                Level2 = Info.Level2,
                Level3 = Info.Level3,
                Need1 = Info.Need1,
                Need2 = Info.Need2,
                Need3 = Info.Need3,
                Level = Level,
                Key = Key,
                Experience = Experience,
                IsTempSpell = IsTempSpell,
                Delay = GetDelay(),
                Range = Info.Range,
                CastTime = (CastTime != 0) && (MessageQueue.Envir.Time > CastTime) ? MessageQueue.Envir.Time - CastTime : 0,
                // IsHeroMagic = true
            };
        }

        public int GetDamage(int DamageBase)
        {
            return (int)((DamageBase + GetPower()) * GetMultiplier());
        }

        public float GetMultiplier()
        {
            return (Info.MultiplierBase + (Level * Info.MultiplierBonus));
        }

        public int GetPower()
        {
            return (int)Math.Round((MPower() / 4F) * (Level + 1) + DefPower());
        }

        public int MPower()
        {
            if (Info.MPowerBonus > 0)
            {
                return MessageQueue.Envir.Random.Next(Info.MPowerBase, Info.MPowerBonus + Info.MPowerBase);
            }
            else
                return Info.MPowerBase;
        }
        public int DefPower()
        {
            if (Info.PowerBonus > 0)
            {
                return MessageQueue.Envir.Random.Next(Info.PowerBase, Info.PowerBonus + Info.PowerBase);
            }
            else
                return Info.PowerBase;
        }

        public int GetPower(int power)
        {
            return (int)Math.Round(power / 4F * (Level + 1) + DefPower());
        }

        public long GetDelay()
        {
            return Info.DelayBase - (Level * Info.DelayReduction);
        }
    }
}