using Server.MirEnvir;
using Server.MirObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.MirDatabase
{
    public class Hero_Info
    {
        //  The heroes Index
        public int Index;
        //  Deleted or not
        public bool Deleted;
        //  The heroes Name
        public string Name;
        //  The Heroes Class
        public MirClass Class;
        //  The Heroes Gender
        public MirGender Gender;
        //  Owner Index
        public int PlayerIndex;
        //  The Heroes Inventory
        public UserItem[] Inventory;
        //  The Heroes Equipment
        public UserItem[] Equipment;
        //  The Heroes Quest Inventory
        public UserItem[] QuestInventory;
        //  The Heroes Magics
        public List<UserMagic> Magics = new List<UserMagic>();
        public List<Buff> Buffs = new List<Buff>();
        public List<Poison> Poisons = new List<Poison>();
        public PlayerObject Player;
        //  The Heroes Level
        public int Level;
        //  The Heroes Current EXP
        public long CurrentEXP;
        //  The EXP needed to level
        public long NeedEXP;
        //  The Heroes Hair
        public byte Hair;
        //  If the hero was summoned or not
        public bool Summoned;
        //  Heroes Current Health;
        public uint CurrentHP;
        //  Heoes Max HP
        public uint MaxHP;
        //  Heroes Current MP
        public uint CurrentMP;
        //  Heroes Max MP
        public uint MaxMP;



        public Hero_Info() { }

        public Hero_Info(BinaryReader reader)
        {
            Index = reader.ReadInt32();
            Deleted = reader.ReadBoolean();
            Name = reader.ReadString();
            Class = (MirClass)reader.ReadByte();
            Gender = (MirGender)reader.ReadByte();
            Level = reader.ReadInt32();
            CurrentEXP = reader.ReadInt64();
            Hair = reader.ReadByte();
            CurrentHP = reader.ReadUInt32();
            CurrentMP = reader.ReadUInt32();
            PlayerIndex = reader.ReadInt32();
            int count = reader.ReadInt32();
            Array.Resize(ref Inventory, count);
            for (int i = 0; i < count; i++)
            {
                if (!reader.ReadBoolean()) continue;
                UserItem item = new UserItem(reader, Envir.LoadVersion, Envir.LoadCustomVersion);
                if (MessageQueue.Envir.BindItem(item) && i < Inventory.Length)
                    Inventory[i] = item;
            }
            count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                
                if (!reader.ReadBoolean()) continue;
                UserItem item = new UserItem(reader, Envir.LoadVersion, Envir.LoadCustomVersion);
                if (MessageQueue.Envir.BindItem(item) && i < Equipment.Length)
                    Equipment[i] = item;
            }
            count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                if (!reader.ReadBoolean()) continue;
                UserItem item = new UserItem(reader, Envir.LoadVersion, Envir.LoadCustomVersion);
                if (MessageQueue.Envir.BindItem(item) && i < QuestInventory.Length)
                    QuestInventory[i] = item;
            }
            count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                UserMagic magic = new UserMagic(reader);
                if (magic.Info == null) continue;
                Magics.Add(magic);
            }
            //reset all magic cooldowns on char loading < stops ppl from having none working skills after a server crash
            for (int i = 0; i < Magics.Count; i++)
            {
                Magics[i].CastTime = 0;
            }
            count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                Buff buff = new Buff(reader);

                if (Envir.LoadVersion == 51)
                {
                    buff.Caster = MessageQueue.Envir.GetObject(reader.ReadUInt32());
                }

                Buffs.Add(buff);
            }
        }
    }
}
