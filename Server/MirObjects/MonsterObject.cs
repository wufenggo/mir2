﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Server.Library.MirEnvir;
using Server.MirDatabase;
using Server.MirEnvir;
using Server.MirObjects.Monsters;
using S = ServerPackets;


namespace Server.MirObjects
{
    /// <summary>
    /// 怪物对象
    /// </summary>
    public class MonsterObject : MapObject
    {
        public static MonsterObject GetMonster(MonsterInfo info)
        {
            if (info == null) return null;
           
            switch (info.AI)
            {
                case 1:
                case 2://鸡,鹿
                    return new Deer(info);
                case 3://龙蛇宝箱,树
                    return new Tree(info);
                case 4://毒蜘蛛
                    return new SpittingSpider(info);
                case 5://食人花
                    return new CannibalPlant(info);
                case 6://带刀护卫
                    return new Guard(info);
                case 7://洞蛆,楔蛾
                    return new CaveMaggot(info);
                case 8://掷斧骷髅,黑暗战士,祖玛弓箭手
                    return new AxeSkeleton(info);
                case 9://狼,蝎子
                    return new HarvestMonster(info);
                case 10://火焰沃玛
                    return new FlamingWooma(info);
                case 11://沃玛教主
                    return new WoomaTaurus(info);
                case 12://角蝇
                    return new BugBagMaggot(info);
                case 13://赤月恶魔
                    return new RedMoonEvil(info);
                case 14://触龙神
                    return new EvilCentipede(info);
                case 15://祖玛雕像
                    return new ZumaMonster(info);
                case 16://祖玛赤雷
                    return new RedThunderZuma(info);
                case 17://祖玛教主
                    return new ZumaTaurus(info);
                case 18://神兽
                    return new Shinsu(info);
                case 19://虹魔蝎卫
                    return new KingScorpion(info);
                case 20://虹魔教主
                    return new DarkDevil(info);
                case 21://肉食性食尸鬼(未知是什么)
                    return new IncarnatedGhoul(info);
                case 22://祖玛教主_封魔
                    return new IncarnatedZT(info);
                case 23://商店护卫,变异骷髅
                    return new BoneFamiliar(info);
                case 24://僧侣僵尸
                    return new DigOutZombie(info);
                case 25://腐肉僵尸
                    return new RevivingZombie(info);
                case 26://雷电僵尸
                    return new ShamanZombie(info);
                case 27://狂热血蜥蜴,蚂蚁司令官
                    return new Khazard(info);
                case 28://恶灵尸王
                    return new ToxicGhoul(info);
                case 29://骷髅长枪兵
                    return new BoneSpearman(info);
                case 30://黄泉教主
                    return new BoneLord(info);
                case 31://牛魔法师
                    return new RightGuard(info);
                case 32://牛魔祭祀
                    return new LeftGuard(info);
                case 33://牛魔王
                    return new MinotaurKing(info);
                case 34://幻影寒虎
                    return new FrostTiger(info);
                case 35://沙虫
                    return new SandWorm(info);
                case 36://浮龙金蛇
                    return new Yimoogi(info);
                case 37://神石毒魔蛛1
                    return new CrystalSpider(info);
                case 38://精灵
                    return new HolyDeva(info);
                case 39://幻影蜘蛛
                    return new RootSpider(info);
                case 40://爆裂蜘蛛
                    return new BombSpider(info);
                case 41:
                case 42://镇魂石
                    return new YinDevilNode(info);
                case 43://破凰魔神,火龙教主？
                    return new OmaKing(info);
                case 44://狐狸战士
                    return new BlackFoxman(info);
                case 45://狐狸法师
                    return new RedFoxman(info);
                case 46://狐狸道士
                    return new WhiteFoxman(info);
                case 47://悲月魂石
                    return new TrapRock(info);
                case 48://九尾魂石
                    return new GuardianRock(info);
                case 49://闪电元素
                    return new ThunderElement(info);
                case 50://悲月天珠
                    return new GreatFoxSpirit(info);
                case 51://悲月刺蛙
                    return new HedgeKekTal(info);
                case 52://破天魔龙
                    return new EvilMir(info);
                case 53://破天魔龙
                    return new EvilMirBody(info);
                case 54://火龙守护兽
                    return new DragonStatue(info);
                case 55://分身
                    return new HumanWizard(info);
                case 56://练功师
                    return new Trainer(info);
                case 57://弓箭护卫
                    return new TownArcher(info);
                case 58://大刀护卫
                    return new Guard(info);
                case 59://刺客分身
                    return new HumanAssassin(info);
                case 60://召唤蜘蛛
                    return new VampireSpider(info);
                case 61://召唤蛤蟆
                    return new SpittingToad(info);
                case 62://召唤图腾
                    return new SnakeTotem(info);
                case 63://鬼魅蛇
                    return new CharmedSnake(info);
                case 64://宝贝猪,小鸡
                    return new IntelligentCreatureObject(info);
                case 65://赤血利刃
                    return new MutatedManworm(info);
                case 66://赤血狂魔
                    return new CrazyManworm(info);
                case 67://黑暗多脚怪
                    return new DarkDevourer(info);
                case 68://足球
                    return new Football(info);
                case 69://紫电小蜘蛛
                    return new PoisonHugger(info);
                case 70://剧毒小蜘蛛
                    return new Hugger(info);
                case 71://怨恶
                    return new Behemoth(info);
                case 72://幽冥龟
                    return new FinialTurtle(info);
                case 73://大龟王
                    return new TurtleKing(info);
                case 74://光明龟
                    return new LightTurtle(info);
                case 75://溶混鬼
                    return new WitchDoctor(info);
                case 76://弯刀流魂
                    return new HellSlasher(info);
                case 77://拔舌流魂
                    return new HellPirate(info);
                case 78://吞魂鬼
                    return new HellCannibal(info);
                case 79://地狱守门人
                    return new HellKeeper(info);
                case 80://守卫弓手
                    return new ConquestArcher(info);
                case 81://大门
                    return new Gate(info);
                case 82://城堡Gi西
                    return new Wall(info);
                case 83://风暴战士
                    return new Tornado(info);
                case 84://野兽王
                    return new WingedTigerLord(info);

                case 86://冰狱战将
                    return new ManectricClaw(info);
                case 87://冰狱天将
                    return new ManectricBlest(info);
                case 88://冰狱魔王
                    return new ManectricKing(info);
                case 89://冰柱
                    return new IcePillar(info);
                case 90://地狱炮兵
                    return new TrollBomber(info);
                case 91://地狱统领
                    return new TrollKing(info);
                case 92://地狱长矛鬼
                    return new FlameSpear(info);
                case 93://地狱魔焰鬼
                    return new FlameMage(info);
                case 94://地狱巨镰鬼
                    return new FlameScythe(info);
                case 95://地狱双刃鬼
                    return new FlameAssassin(info);
                case 96://地狱将军
                    return new FlameQueen(info);
                case 97://寒冰守护神,紫电守护神
                    return new HellKnight(info);
                case 98://炎魔太子
                    return new HellLord(info);
                case 99://寒冰球
                    return new HellBomb(info);
                case 100://
                    return new VenomSpider(info);
                case 101:
                    return new Jar2(info);
                case 102:
                    return new RestlessJar(info);
                case 103://赤血鬼魂
                    return new CyanoGhast(info);
                case 104://阳龙王
                    return new ChieftainSword(info);
                case 105://暴雪僵尸
                    return new FrozenZombie(info);
                case 106://火焰僵尸
                    return new BurningZombie(info);
                case 107://DarkBeast LightBeast 暗黑剑齿虎 光明剑齿虎 2种攻击
                    return new DarkBeast(info);
                case 108://WhiteMammoth 猛犸象,普通攻击和蹲地板
                    return new WhiteMammoth(info);
                case 109://HardenRhino 铁甲犀牛
                    return new HardenRhino(info);
                case 110://Demonwolf 赤炎狼 1近身普攻，3格内喷火 火焰灵猫 共用
                    return new Demonwolf(info);
                case 111://BloodBaboon 血狒狒
                    return new BloodBaboon(info);
                case 112://DeathCrawler 死灵
                    return new DeathCrawler(info);
                case 113://AncientBringer 丹墨
                    return new AncientBringer(info);
                case 114://CatWidow 长枪灵猫
                    return new CatWidow(info);
                case 115://StainHammerCat 铁锤猫卫
                    return new StainHammerCat(info);
                case 116://BlackHammerCat 黑镐猫卫
                    return new BlackHammerCat(info);
                case 117://StrayCat 双刃猫卫
                    return new StrayCat(info);
                case 118://CatShaman 灵猫法师
                    return new CatShaman(info);
                case 119://SeedingsGeneral 灵猫圣兽
                    return new SeedingsGeneral(info);
                case 120://SeedingsGeneral 灵猫将军
                    return new GeneralJinmYo(info);
                case 122://GasToad 神气蛤蟆
                    return new GasToad(info);
                case 123://Mantis 螳螂
                    return new Mantis(info);
                case 124://SwampWarrior 神殿树人
                    return new SwampWarrior(info);
                case 125://SwampWarrior 神殿刺鸟
                    return new AssassinBird(info);
                case 126://RhinoWarrior 犀牛勇士
                    return new RhinoWarrior(info);
                case 127://RhinoPriest 犀牛牧师
                    return new RhinoPriest(info);
                case 128://SwampSlime 泥战士
                    return new SwampSlime(info);
                case 129://RockGuard 石巨人
                    return new RockGuard(info);
                case 130://MudWarrior 泥土巨人
                    return new MudWarrior(info);
                case 131://SmallPot 小如来
                    return new SmallPot(info);
                case 132://TreeQueen 树王
                    return new TreeQueen(info);
                case 133://ShellFighter 斗争者
                    return new ShellFighter(info);
                case 134://黑暗的沸沸 
                    return new DarkBaboon(info);
                case 135://双头兽 
                    return new TwinHeadBeast(info);
                case 136://奥玛食人族 
                    return new OmaCannibal(info);
                case 137://奥玛祝福 普通攻击，砸地板 
                    return new OmaBlest(info);
                case 138://奥玛斧头兵 破防
                    return new OmaSlasher(info);
                case 139://奥玛刺客 闪现近身，破防，攻击完，又随机闪开
                    return new OmaAssassin(info);
                case 140://奥玛法师，随机闪开
                    return new OmaMage(info);
                case 141://奥玛巫医，3种攻击手段
                    return new OmaWitchDoctor(info);
                case 142://长鼻猴 普通攻击 攻击并净化，回血
                    return new Mandrill(info);
                case 143://瘟疫蟹 雷电攻击
                    return new PlagueCrab(info);
                case 144://攀缘花 
                    return new CreeperPlant(info);
                case 145://幽灵射手 
                    return new FloatingWraith(info);
                case 146://幽灵厨子 破防 
                    return new ArmedPlant(info);
                case 147://淹死的奴隶 
                    return new Nadz(info);
                case 148://复仇的恶灵 
                    return new AvengingSpirit(info);
                case 149://复仇的勇士 
                    return new AvengingWarrior(info);
                case 150://ClawBeast 水手长 
                    return new ClawBeast(info);
                case 151://WoodBox 爆炸箱子 
                    return new WoodBox(info);
                case 152://KillerPlant 黑暗船长 
                    return new KillerPlant(info);
                case 153://FrozenFighter 雪原战士 
                    return new FrozenFighter(info);
                case 154://FrozenKnight 雪原勇士 
                    return new FrozenKnight(info);
                case 155://FrozenGolem 雪原鬼尊 
                    return new FrozenGolem(info);
                case 156://IcePhantom 雪原恶鬼 
                    return new IcePhantom(info);
                case 157://SnowWolf 雪原冰狼 
                    return new SnowWolf(info);
                case 158://SnowWolfKing 雪太狼 
                    return new SnowWolfKing(info);
                case 159://FrozenMiner 冰魄矿工 
                    return new FrozenMiner(info);
                case 160://FrozenAxeman 冰魄斧兵 
                    return new FrozenAxeman(info);
                case 161://FrozenMagician 冰魄法师 
                    return new FrozenMagician(info);
                case 162://SnowYeti 冰魄雪人 
                    return new SnowYeti(info);
                case 163://IceCrystalSoldier 冰晶战士 
                    return new IceCrystalSoldier(info);
                case 164://DarkWraith 暗黑战士 
                    return new DarkWraith(info);
                case 165://DarkSpirit 幽灵战士 
                    return new DarkSpirit(info);
                case 166://CrystalBeast 水晶兽 冰雪守护神 
                    return new CrystalBeast(info);
              
                //unfinished

                case 250://如果是250，则用image来做怪物AI
                    return GetMonsterByImage(info);

                case 253://鸟人像？
                    return new FlamingMutant(info);
                case 254:
                    return new StoningStatue(info);
                //unfinished END
                case 255://custom
                    return new TestAttackMon(info);


                case 200://custom
                    return new Runaway(info);
                case 201://custom
                    return new TalkingMonster(info);
                case 202://弓箭护卫（战场中的弓箭护卫）
                    return new WarTownArcher(info);
                case 210://custom
                    return new FlameTiger(info);
          
                    

                default:
                    return GetMonsterByImage(info);
            }
        }

        //AI不够用，用image来做吧，每个都不一样的，用image来做识别，这个比较好。
        //之前很多的ai，其实都可以用这个来做，坑了
        public static MonsterObject GetMonsterByImage(MonsterInfo info)
        {
            if (info == null) return null;

            switch (info.Image)
            {
                case Monster.KingGuard://Monster403 紫花仙子
                    return new KingGuard(info);

                case Monster.Monster403://Monster403 紫花仙子
                    return new Monster403(info);
                case Monster.Monster404://Monster404 冰焰鼠
                    return new Monster404(info);
                case Monster.Monster405://Monster405 冰蜗牛
                    return new Monster405(info);
                case Monster.Monster406://Monster406 冰宫战士
                    return new Monster406(info);
                case Monster.Monster407://Monster407 冰宫射手
                    return new Monster407(info);
                case Monster.Monster408://Monster408 冰宫卫士
                    return new Monster408(info);
                case Monster.Monster409://Monster409 虹花仙子
                    return new Monster409(info);
                case Monster.Monster410://Monster410 冰宫鼠卫
                    return new Monster410(info);
                case Monster.Monster411://Monster411 冰宫骑士
                    return new Monster411(info);
                case Monster.Monster412://Monster412 冰宫刀卫
                    return new Monster412(info);
                case Monster.Monster413://Monster413 冰宫护法
                    return new Monster413(info);
                case Monster.Monster414://Monster414 冰宫画卷
                    return new Monster414(info);
                case Monster.Monster415://Monster415 冰宫画卷
                    return new Monster415(info);
                case Monster.Monster416://Monster416 冰宫画卷
                    return new Monster416(info);
                case Monster.Monster417://Monster417 冰宫画卷
                    return new Monster417(info);
                case Monster.Monster418://Monster418 冰宫学者
                    return new Monster418(info);
                case Monster.Monster419://Monster419 冰宫巫师
                    return new Monster419(info);
                case Monster.Monster420://Monster420 冰宫祭师
                    return new Monster420(info);
                case Monster.Monster421://Monster421 冰雪女皇
                    return new Monster421(info);
                case Monster.Monster422:// 未知
                    return new Monster422(info);
                case Monster.Monster423:// 未知
                    return new Monster423(info);
                case Monster.Monster424:// 昆仑虎
                    return new Monster424(info);
                case Monster.Monster425:// 部落祭师
                    return new Monster425(info);
                case Monster.Monster426:// 部落法师
                    return new Monster426(info);
                case Monster.Monster427:// 部落刺客
                    return new Monster427(info);
                case Monster.Monster428:// 老树盘根
                    return new Monster428(info);
                case Monster.Monster429:// 叛军道士
                    return new Monster429(info);
                case Monster.Monster430:// 多手怪
                    return new Monster430(info);
                case Monster.Monster431:// 未知
                    return new Monster431(info);
                case Monster.Monster432:// 未知
                    return new Monster432(info);
                case Monster.Monster433:// 未知
                    return new Monster433(info);
                case Monster.Monster434:// 叛军法师
                    return new Monster434(info);
                case Monster.Monster435:// 九尾火狐
                    return new Monster435(info);
                case Monster.Monster436:// 叛军刺客
                    return new Monster436(info);
                case Monster.Monster437:// 叛军武僧
                    return new Monster437(info);
                case Monster.Monster438:// 盘蟹花
                    return new Monster438(info);
                case Monster.Monster439:// 叛军武士
                    return new Monster439(info);
                case Monster.Monster440:// 叛军射手
                    return new Monster440(info);
                case Monster.Monster441:// 叛军战神
                    return new Monster441(info);
                case Monster.Monster442:// 叛军箭神
                    return new Monster442(info);
                case Monster.Monster443:// 叛军道尊
                    return new Monster443(info);
                case Monster.Monster444:// 叛军刺皇
                    return new Monster444(info);
                case Monster.Monster445:// 昆仑门
                    return new Monster445(info);
                case Monster.Monster446:// 叛军首领
                    return new Monster446(info);
                case Monster.Monster447:// 孽火花
                    return new Monster447(info);
                case Monster.Monster448:// 孽冰花
                    return new Monster448(info);
                case Monster.Monster449:// 毒妖武士
                    return new Monster449(info);
                case Monster.Monster450:// 毒妖射手
                    return new Monster450(info);
                case Monster.Monster451:// 碑石妖 毒妖林小BOSS
                    return new Monster451(info);
                case Monster.Monster452:// 多毒妖 毒妖林小BOSS
                    return new Monster452(info);
                case Monster.Monster453:// 巨斧妖 毒妖林小BOSS
                    return new Monster453(info);
                case Monster.Monster454:// 毒妖女皇 毒妖林BOSS
                    return new Monster454(info);
                case Monster.Monster455:// 未知
                    return new Monster455(info);
                case Monster.Monster456:// 未知
                    return new Monster456(info);
                case Monster.Monster457:// 未知
                    return new Monster457(info);
                case Monster.Monster458:// 未知
                    return new Monster458(info);
                case Monster.Monster460:// 未知
                    return new Monster460(info);// 强化骷髅

                default:
                    return new MonsterObject(info);
            }
        }

        public override ObjectType Race
        {
            get { return ObjectType.Monster; }
        }

        public MonsterInfo Info;
        public MapRespawn Respawn;
        public List<PlayerObject> Contributers = new List<PlayerObject>();
        public bool Retreat = false;
        public Point SpawnedLocation;
        public override string Name
        {
            get { return Master == null ? Info.GameName : string.Format("{0}({1})", Info.GameName, Master.Name); }
            set { throw new NotSupportedException(); }
        }

        public override int CurrentMapIndex { get; set; }
        public override Point CurrentLocation { get; set; }
        public override sealed MirDirection Direction { get; set; }
        public override ushort Level
        {
            get { return Info.Level; }
            set { throw new NotSupportedException(); }
        }

        public override sealed AttackMode AMode
        {
            get
            {
                return base.AMode;
            }
            set
            {
                base.AMode = value;
            }
        }
        public override sealed PetMode PMode
        {
            get
            {
                return base.PMode;
            }
            set
            {
                base.PMode = value;
            }
        }

        public override sealed PetType PType
        {
            get
            {
                return base.PType;
            }
            set
            {
                base.PType = value;
            }
        }

        public override uint Health
        {
            get { return HP; }
        }

        public override uint MaxHealth
        {
            get { return MaxHP; }
        }

        public uint HP, MaxHP;
        public ushort MoveSpeed;

        //怪物的经验，经验这里太乱了，先根据血量,防御，攻击等计算经验
        //血量在1-1.5倍之间，在100-1万之间
        //
        public virtual uint Experience
        {
            get
            {
                //根据怪物的血量，敏捷，防御，攻击等属性计算怪物的最终经验值
                uint cc = (uint)(Math.Min(Math.Max(0, Info.Agility - 10), 20) * 10 + Math.Max(Info.MaxAC - 5, 0) * 5 + Math.Max(Info.MaxMAC - 5, 0) * 5 + Math.Max(Info.MaxDC, Info.MaxMC) * 5);
                if (cc > Info.HP * 2)
                {
                    cc = Info.HP * 2;
                }
                cc = cc + Info.HP;
                if (cc > 10000)
                {
                    return cc * 3 / 2;
                }
                return (uint)(cc / 10000.0 * 0.5 * cc) + cc;
            }
        }
        public int DeadDelay
        {
            get
            {
                switch (Info.AI)
                {
                    case 81:
                    case 82:
                        return int.MaxValue;
                    case 252:
                        return 5000;
                    default:
                        return 180000;
                }
            }
        }
        public const int RegenDelay = 10000, EXPOwnerDelay = 5000, SearchDelay = 3000, RoamDelay = 1000, HealDelay = 600, RevivalDelay = 2000;
        public long ActionTime, MoveTime, AttackTime, RegenTime, DeadTime, SearchTime, RoamTime, HealTime;
        public long ShockTime, RageTime, HallucinationTime;
        public bool BindingShotCenter, PoisonStopRegen = true;

        public byte PetLevel;
        public uint PetExperience;
        public byte MaxPetLevel;
        public long TameTime;

        public int RoutePoint;
        public bool Waiting;

        public List<MonsterObject> SlaveList = new List<MonsterObject>();
        public List<RouteInfo> Route = new List<RouteInfo>();

        public override bool Blocking
        {
            get
            {
                return !Dead;
            }
        }
        protected virtual bool CanRegen
        {
            get { return Envir.Time >= RegenTime; }
        }
        protected virtual bool CanMove
        {
            get
            {
                return !Dead && Envir.Time > MoveTime && Envir.Time > ActionTime && Envir.Time > ShockTime &&
                       (Master == null || Master.PMode == PetMode.MoveOnly || Master.PMode == PetMode.Both) && !CurrentPoison.HasFlag(PoisonType.Paralysis)
                       && !CurrentPoison.HasFlag(PoisonType.LRParalysis) && !CurrentPoison.HasFlag(PoisonType.Stun) && !CurrentPoison.HasFlag(PoisonType.Frozen);
            }
        }
        protected virtual bool CanAttack
        {
            get
            {
                return !Dead && Envir.Time > AttackTime && Envir.Time > ActionTime &&
                     (Master == null || Master.PMode == PetMode.AttackOnly || Master.PMode == PetMode.Both || !CurrentMap.Info.NoFight) && !CurrentPoison.HasFlag(PoisonType.Paralysis)
                       && !CurrentPoison.HasFlag(PoisonType.LRParalysis) && !CurrentPoison.HasFlag(PoisonType.Stun) && !CurrentPoison.HasFlag(PoisonType.Frozen);
            }
        }

        protected internal MonsterObject(MonsterInfo info)
        {
            Info = info;

            Undead = Info.Undead;
            AutoRev = info.AutoRev;
            CoolEye = info.CoolEye > RandomUtils.Next(100);
            Direction = (MirDirection)RandomUtils.Next(8);

            AMode = AttackMode.All;
            PMode = PetMode.Both;

            RegenTime = RandomUtils.Next(RegenDelay) + Envir.Time;
            SearchTime = RandomUtils.Next(SearchDelay) + Envir.Time;
            RoamTime = RandomUtils.Next(RoamDelay) + Envir.Time;
        }
        public bool Spawn(Map temp, Point location)
        {
            if (!temp.ValidPoint(location)) return false;

            CurrentMap = temp;
            CurrentLocation = location;

            CurrentMap.AddObject(this);

            RefreshAll();
            SetHP(MaxHP);

            Spawned();
            Envir.MonsterCount++;
            CurrentMap.MonsterCount++;
            return true;
        }
        //怪物重生
        public bool Spawn(MapRespawn respawn)
        {
            Respawn = respawn;

            if (Respawn.Map == null) return false;

            for (int i = 0; i < 10; i++)
            {
                CurrentLocation = new Point(Respawn.Info.Location.X + RandomUtils.Next(-Respawn.Info.Spread, Respawn.Info.Spread + 1),
                                            Respawn.Info.Location.Y + RandomUtils.Next(-Respawn.Info.Spread, Respawn.Info.Spread + 1));

                if (!respawn.Map.ValidPoint(CurrentLocation)) continue;

                respawn.Map.AddObject(this);

                CurrentMap = respawn.Map;

                if (Respawn.Route.Count > 0)
                    Route.AddRange(Respawn.Route);

                RefreshAll();
                SetHP(MaxHP);

                Spawned();
                Respawn.Count++;
                respawn.Map.MonsterCount++;
                Envir.MonsterCount++;
                return true;
            }
            return false;
        }

        public override void Spawned()
        {
            base.Spawned();
            SpawnedLocation = CurrentLocation;
            ActionTime = Envir.Time + 2000;
            if (Info.HasSpawnScript && (Envir.MonsterNPC != null))
            {
                Envir.MonsterNPC.Call(this, string.Format("[@_SPAWN({0})]", Info.Index));
            }
        }

        protected virtual void RefreshBase()
        {
            MaxHP = Info.HP;
            MinAC = Info.MinAC;
            MaxAC = Info.MaxAC;
            MinMAC = Info.MinMAC;
            MaxMAC = Info.MaxMAC;
            MinDC = Info.MinDC;
            MaxDC = Info.MaxDC;
            MinMC = Info.MinMC;
            MaxMC = Info.MaxMC;
            MinSC = Info.MinSC;
            MaxSC = Info.MaxSC;
            Accuracy = Info.Accuracy;
            Agility = Info.Agility;

            MoveSpeed = Info.MoveSpeed;
            AttackSpeed = Info.AttackSpeed;
        }
        public virtual void RefreshAll()
        {
            RefreshBase();

            MaxHP = (uint)Math.Min(uint.MaxValue, MaxHP + PetLevel * 20);
            MinAC = (ushort)Math.Min(ushort.MaxValue, MinAC + PetLevel * 2);
            MaxAC = (ushort)Math.Min(ushort.MaxValue, MaxAC + PetLevel * 2);
            MinMAC = (ushort)Math.Min(ushort.MaxValue, MinMAC + PetLevel * 2);
            MaxMAC = (ushort)Math.Min(ushort.MaxValue, MaxMAC + PetLevel * 2);
            MinDC = (ushort)Math.Min(ushort.MaxValue, MinDC + PetLevel);
            MaxDC = (ushort)Math.Min(ushort.MaxValue, MaxDC + PetLevel);

            if (Info.Name == Settings.SkeletonName || Info.Name == Settings.ShinsuName || Info.Name == Settings.AngelName || Info.Name == Settings.SkeletonName1)
            {
                MoveSpeed = (ushort)Math.Min(ushort.MaxValue, (Math.Max(ushort.MinValue, MoveSpeed - MaxPetLevel * 130)));
                AttackSpeed = (ushort)Math.Min(ushort.MaxValue, (Math.Max(ushort.MinValue, AttackSpeed - MaxPetLevel * 70)));
            }

            if (MoveSpeed < 400) MoveSpeed = 400;
            if (AttackSpeed < 400) AttackSpeed = 400;

            RefreshBuffs();
        }
        protected virtual void RefreshBuffs()
        {
            for (int i = 0; i < Buffs.Count; i++)
            {
                Buff buff = Buffs[i];

                if (buff.Values == null || buff.Values.Length < 1) continue;

                switch (buff.Type)
                {
                    case BuffType.Haste:
                        ASpeed = (sbyte)Math.Max(sbyte.MinValue, (Math.Min(sbyte.MaxValue, ASpeed + buff.Values[0])));
                        break;
                    case BuffType.SwiftFeet:
                        MoveSpeed = (ushort)Math.Max(ushort.MinValue, MoveSpeed + 100 * buff.Values[0]);
                        break;
                    case BuffType.LightBody:
                        Agility = (byte)Math.Min(byte.MaxValue, Agility + buff.Values[0]);
                        break;
                    case BuffType.SoulShield:
                        MaxMAC = (ushort)Math.Min(ushort.MaxValue, MaxMAC + buff.Values[0]);
                        break;
                    case BuffType.BlessedArmour:
                        MaxAC = (ushort)Math.Min(ushort.MaxValue, MaxAC + buff.Values[0]);
                        break;
                    case BuffType.UltimateEnhancer:
                        MaxDC = (ushort)Math.Min(ushort.MaxValue, MaxDC + buff.Values[0]);
                        break;
                    case BuffType.Curse:
                        ushort rMaxDC = (ushort)(((int)MaxDC / 100) * buff.Values[0]);
                        ushort rMaxMC = (ushort)(((int)MaxMC / 100) * buff.Values[0]);
                        ushort rMaxSC = (ushort)(((int)MaxSC / 100) * buff.Values[0]);
                        sbyte rASpeed = (sbyte)(((int)ASpeed / 100) * buff.Values[0]);
                        ushort rMSpeed = (ushort)((MoveSpeed / 100) * buff.Values[0]);

                        MaxDC = (ushort)Math.Max(ushort.MinValue, MaxDC - rMaxDC);
                        MaxMC = (ushort)Math.Max(ushort.MinValue, MaxMC - rMaxMC);
                        MaxSC = (ushort)Math.Max(ushort.MinValue, MaxSC - rMaxSC);
                        ASpeed = (sbyte)Math.Min(sbyte.MaxValue, (Math.Max(sbyte.MinValue, ASpeed - rASpeed)));
                        MoveSpeed = (ushort)Math.Max(ushort.MinValue, MoveSpeed - rMSpeed);
                        break;

                    case BuffType.PetEnhancer:
                        MinDC = (ushort)Math.Min(ushort.MaxValue, MinDC + buff.Values[0]);
                        MaxDC = (ushort)Math.Min(ushort.MaxValue, MaxDC + buff.Values[0]);
                        MinAC = (ushort)Math.Min(ushort.MaxValue, MinAC + buff.Values[1]);
                        MaxAC = (ushort)Math.Min(ushort.MaxValue, MaxAC + buff.Values[1]);
                        break;
                }

            }
        }
        public void RefreshNameColour(bool send = true)
        {
            if (ShockTime < Envir.Time) BindingShotCenter = false;

            Color colour = Color.White;

            switch (PetLevel)
            {
                case 1:
                    colour = Color.Aqua;
                    break;
                case 2:
                    colour = Color.Aquamarine;
                    break;
                case 3:
                    colour = Color.LightSeaGreen;
                    break;
                case 4:
                    colour = Color.SlateBlue;
                    break;
                case 5:
                    colour = Color.SteelBlue;
                    break;
                case 6:
                    colour = Color.Blue;
                    break;
                case 7:
                    colour = Color.Navy;
                    break;
            }

            if (Envir.Time < ShockTime)
                colour = Color.Peru;
            else if (Envir.Time < RageTime)
                colour = Color.Red;
            else if (Envir.Time < HallucinationTime)
                colour = Color.MediumOrchid;

            if (colour == NameColour || !send) return;

            NameColour = colour;

            Broadcast(new S.ObjectColourChanged { ObjectID = ObjectID, NameColour = NameColour });
        }

        public void SetHP(uint amount)
        {
            if (HP == amount) return;

            HP = amount <= MaxHP ? amount : MaxHP;

            if (!Dead && HP == 0) Die();

            //  HealthChanged = true;
            BroadcastHealthChange();
        }
        public virtual void ChangeHP(int amount)
        {

            uint value = (uint)Math.Max(uint.MinValue, Math.Min(MaxHP, HP + amount));

            if (value == HP) return;

            HP = value;

            if (!Dead && HP == 0) Die();

            // HealthChanged = true;
            BroadcastHealthChange();
        }

        //use this so you can have mobs take no/reduced poison damage
        public virtual void PoisonDamage(int amount, MapObject Attacker)
        {
            ChangeHP(amount);
        }


        public override bool Teleport(Map temp, Point location, bool effects = true, byte effectnumber = 0)
        {
            if (temp == null || !temp.ValidPoint(location)) return false;

            CurrentMap.RemoveObject(this);
            if (effects) Broadcast(new S.ObjectTeleportOut { ObjectID = ObjectID, Type = effectnumber });
            Broadcast(new S.ObjectRemove { ObjectID = ObjectID });

            CurrentMap.MonsterCount--;

            CurrentMap = temp;
            CurrentLocation = location;

            CurrentMap.MonsterCount++;

            InTrapRock = false;

            CurrentMap.AddObject(this);
            BroadcastInfo();

            if (effects) Broadcast(new S.ObjectTeleportIn { ObjectID = ObjectID, Type = effectnumber });

            BroadcastHealthChange();

            return true;
        }

        //怪物死亡
        public override void Die()
        {
            if (Dead) return;

            HP = 0;
            Dead = true;

            DeadTime = Envir.Time + DeadDelay;

            Broadcast(new S.ObjectDied { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

            if (Info.HasDieScript && (Envir.MonsterNPC != null))
            {
                Envir.MonsterNPC.Call(this, string.Format("[@_DIE({0})]", Info.Index));
            }

            if (EXPOwner != null && Master == null && EXPOwner.Race == ObjectType.Player)
            {
                EXPOwner.WinExp(Experience, Level);

                PlayerObject playerObj = (PlayerObject)EXPOwner;
                playerObj.CheckGroupQuestKill(Info);
            }

            if (Respawn != null)
            {
                Respawn.Count--;
                if (Respawn.IsEventObjective && Respawn.Event != null)
                    Respawn.Event.EventMonsterDied(Contributers);

                Contributers.Clear();
            }
            if (Master == null && EXPOwner != null)
                Drop();

            Master = null;

            PoisonList.Clear();
            Envir.MonsterCount--;
            if (CurrentMap != null)
            {
                CurrentMap.MonsterCount--;
            }
        }

        public void Revive(uint hp, bool effect)
        {
            if (!Dead) return;

            SetHP(hp);

            Dead = false;
            ActionTime = Envir.Time + RevivalDelay;

            Broadcast(new S.ObjectRevived { ObjectID = ObjectID, Effect = effect });

            if (Respawn != null)
                Respawn.Count++;

            Envir.MonsterCount++;
            CurrentMap.MonsterCount++;
        }

        public override int Pushed(MapObject pusher, MirDirection dir, int distance)
        {
            if (!Info.CanPush) return 0;
            //if (!CanMove) return 0; //stops mobs that can't move (like cannibalplants) from being pushed

            int result = 0;
            MirDirection reverse = Functions.ReverseDirection(dir);
            for (int i = 0; i < distance; i++)
            {
                Point location = Functions.PointMove(CurrentLocation, dir, 1);

                if (!CurrentMap.ValidPoint(location)) return result;

                //Cell cell = CurrentMap.GetCell(location);

                bool stop = false;
                if (CurrentMap.Objects[location.X, location.Y] != null)
                    for (int c = 0; c < CurrentMap.Objects[location.X, location.Y].Count; c++)
                    {
                        MapObject ob = CurrentMap.Objects[location.X, location.Y][c];
                        if (!ob.Blocking) continue;
                        stop = true;
                    }
                if (stop) break;

                CurrentMap.Remove(CurrentLocation.X, CurrentLocation.Y, this);

                Direction = reverse;
                RemoveObjects(dir, 1);
                CurrentLocation = location;
                //CurrentMap.GetCell(CurrentLocation).Add(this);
                CurrentMap.Add(CurrentLocation.X, CurrentLocation.Y, this);
                AddObjects(dir, 1);

                Broadcast(new S.ObjectPushed { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

                result++;
            }

            ActionTime = Envir.Time + 300 * result;
            MoveTime = Envir.Time + 500 * result;

            if (result > 0)
            {
                //Cell cell = CurrentMap.GetCell(CurrentLocation);

                for (int i = 0; i < CurrentMap.Objects[CurrentLocation.X, CurrentLocation.Y].Count; i++)
                {
                    if (CurrentMap.Objects[CurrentLocation.X, CurrentLocation.Y][i].Race != ObjectType.Spell) continue;
                    SpellObject ob = (SpellObject)CurrentMap.Objects[CurrentLocation.X, CurrentLocation.Y][i];

                    ob.ProcessSpell(this);
                    //break;
                }
            }

            return result;
        }
        //怪物掉落物品
        //怪物掉落物品
        protected virtual void Drop()
        {
            for (int i = 0; i < Info.Drops.Count; i++)
            {
                DropInfo drop = Info.Drops[i];

                int rate = (int)(drop.Chance / (Settings.DropRate));

                if (EXPOwner != null && EXPOwner.ItemDropRateOffset > 0)
                    rate -= (int)(rate * (EXPOwner.ItemDropRateOffset / 100));

                if (rate < 1) rate = 1;

                if (RandomUtils.Next(rate) != 0) continue;

                if (drop.Gold > 0)
                {
                    int lowerGoldRange = (int)(drop.Gold / 2);
                    int upperGoldRange = (int)(drop.Gold + drop.Gold / 2);

                    if (EXPOwner != null && EXPOwner.GoldDropRateOffset > 0)
                        lowerGoldRange += (int)(lowerGoldRange * (EXPOwner.GoldDropRateOffset / 100));

                    if (lowerGoldRange > upperGoldRange) lowerGoldRange = upperGoldRange;

                    int gold = RandomUtils.Next(lowerGoldRange, upperGoldRange);

                    if (gold <= 0) continue;

                    if (!DropGold((uint)gold)) return;
                }
                else
                {
                    UserItem item = Envir.CreateDropItem(drop.Item);
                    if (item == null) continue;

                    if (EXPOwner != null && EXPOwner.Race == ObjectType.Player)
                    {
                        PlayerObject ob = (PlayerObject)EXPOwner;

                        if (ob.CheckGroupQuestItem(item))
                        {
                            continue;
                        }
                    }

                    if (drop.QuestRequired) continue;
                    if (!DropItem(item)) return;
                }
            }
        }

        protected virtual bool DropItem(UserItem item)
        {
            if (CurrentMap.Info.NoDropMonster)
                return false;

            ItemObject ob = new ItemObject(this, item)
            {
                Owner = EXPOwner,
                OwnerTime = Envir.Time + Settings.Minute,
            };

            if (!item.Info.GlobalDropNotify)
                return ob.Drop(Settings.DropRange);

            foreach (var player in Envir.Players)
            {
                player.ReceiveChat($"{Name} has dropped {item.FriendlyName}.", ChatType.System2);
            }

            return ob.Drop(Settings.DropRange);
        }

        protected virtual bool DropGold(uint gold)
        {
            if (EXPOwner != null && EXPOwner.CanGainGold(gold) && !Settings.DropGold)
            {
                EXPOwner.WinGold(gold);
                return true;
            }

            uint count = gold / Settings.MaxDropGold == 0 ? 1 : gold / Settings.MaxDropGold + 1;
            for (int i = 0; i < count; i++)
            {
                ItemObject ob = new ItemObject(this, i != count - 1 ? Settings.MaxDropGold : gold % Settings.MaxDropGold)
                {
                    Owner = EXPOwner,
                    OwnerTime = Envir.Time + Settings.Minute,
                };

                ob.Drop(Settings.DropRange);
            }

            return true;
        }

        public override void Process()
        {
            base.Process();

            RefreshNameColour();

            if (Target != null && (Target.CurrentMap != CurrentMap || !Target.IsAttackTarget(this) || !Functions.InRange(CurrentLocation, Target.CurrentLocation, Globals.DataRange)))
                Target = null;

            for (int i = SlaveList.Count - 1; i >= 0; i--)
                if (SlaveList[i].Dead || SlaveList[i].Node == null)
                    SlaveList.RemoveAt(i);

            if (Dead && Envir.Time >= DeadTime)
            {
                CurrentMap.RemoveObject(this);
                if (Master != null)
                {
                    Master.Pets.Remove(this);
                    Master = null;
                }

                Despawn();
                return;
            }

            if (Master != null && TameTime > 0 && Envir.Time >= TameTime)
            {
                Master.Pets.Remove(this);
                Master = null;
                Broadcast(new S.ObjectName { ObjectID = ObjectID, Name = Name });
            }

            ProcessAI();

            ProcessBuffs();
            ProcessRegen();
            ProcessPoison();


            /*   if (!HealthChanged) return;

               HealthChanged = false;

               BroadcastHealthChange();*/
        }

        public override void SetOperateTime()
        {
            long time = Envir.Time + 2000;

            if (DeadTime < time && DeadTime > Envir.Time)
                time = DeadTime;

            if (OwnerTime < time && OwnerTime > Envir.Time)
                time = OwnerTime;

            if (ExpireTime < time && ExpireTime > Envir.Time)
                time = ExpireTime;

            if (PKPointTime < time && PKPointTime > Envir.Time)
                time = PKPointTime;

            if (LastHitTime < time && LastHitTime > Envir.Time)
                time = LastHitTime;

            if (EXPOwnerTime < time && EXPOwnerTime > Envir.Time)
                time = EXPOwnerTime;

            if (SearchTime < time && SearchTime > Envir.Time)
                time = SearchTime;

            if (RoamTime < time && RoamTime > Envir.Time)
                time = RoamTime;


            if (ShockTime < time && ShockTime > Envir.Time)
                time = ShockTime;

            if (RegenTime < time && RegenTime > Envir.Time && Health < MaxHealth)
                time = RegenTime;

            if (RageTime < time && RageTime > Envir.Time)
                time = RageTime;

            if (HallucinationTime < time && HallucinationTime > Envir.Time)
                time = HallucinationTime;

            if (ActionTime < time && ActionTime > Envir.Time)
                time = ActionTime;

            if (MoveTime < time && MoveTime > Envir.Time)
                time = MoveTime;

            if (AttackTime < time && AttackTime > Envir.Time)
                time = AttackTime;

            if (HealTime < time && HealTime > Envir.Time && HealAmount > 0)
                time = HealTime;

            if (BrownTime < time && BrownTime > Envir.Time)
                time = BrownTime;

            for (int i = 0; i < ActionList.Count; i++)
            {
                if (ActionList[i].Time >= time && ActionList[i].Time > Envir.Time) continue;
                time = ActionList[i].Time;
            }

            for (int i = 0; i < PoisonList.Count; i++)
            {
                if (PoisonList[i].TickTime >= time && PoisonList[i].TickTime > Envir.Time) continue;
                time = PoisonList[i].TickTime;
            }

            for (int i = 0; i < Buffs.Count; i++)
            {
                if (Buffs[i].ExpireTime >= time && Buffs[i].ExpireTime > Envir.Time) continue;
                time = Buffs[i].ExpireTime;
            }


            if (OperateTime <= Envir.Time || time < OperateTime)
                OperateTime = time;
        }

        public override void Process(DelayedAction action)
        {
            switch (action.Type)
            {
                case DelayedType.Damage:
                    CompleteAttack(action.Params);
                    break;
                case DelayedType.RangeDamage:
                    CompleteRangeAttack(action.Params);
                    break;
                case DelayedType.Die:
                    CompleteDeath(action.Params);
                    break;
                case DelayedType.Recall:
                    PetRecall();
                    break;
            }
        }

        public void PetRecall()
        {
            if (Master == null) return;
            if (!Teleport(Master.CurrentMap, Master.Back))
                Teleport(Master.CurrentMap, Master.CurrentLocation);
        }
        protected virtual void CompleteAttack(IList<object> data)
        {
            MapObject target = (MapObject)data[0];
            int damage = (int)data[1];
            DefenceType defence = (DefenceType)data[2];

            if (target == null || !target.IsAttackTarget(this) || target.CurrentMap != CurrentMap || target.Node == null) return;

            target.Attacked(this, damage, defence);
        }

        protected virtual void CompleteRangeAttack(IList<object> data)
        {
            MapObject target = (MapObject)data[0];
            int damage = (int)data[1];
            DefenceType defence = (DefenceType)data[2];

            if (target == null || !target.IsAttackTarget(this) || target.CurrentMap != CurrentMap || target.Node == null) return;

            target.Attacked(this, damage, defence);
        }

        protected virtual void CompleteDeath(IList<object> data)
        {
            throw new NotImplementedException();
        }

        protected virtual void ProcessRegen()
        {
            if (Dead) return;

            int healthRegen = 0;

            if (CanRegen)
            {
                RegenTime = Envir.Time + RegenDelay;


                if (HP < MaxHP)
                    healthRegen += (int)(MaxHP * 0.022F) + 1;
            }


            if (Envir.Time > HealTime)
            {
                HealTime = Envir.Time + HealDelay;

                if (HealAmount > 5)
                {
                    healthRegen += 5;
                    HealAmount -= 5;
                }
                else
                {
                    healthRegen += HealAmount;
                    HealAmount = 0;
                }
            }

            if (healthRegen > 0) ChangeHP(healthRegen);
            if (HP == MaxHP) HealAmount = 0;
        }
        protected virtual void ProcessPoison()
        {
            PoisonType type = PoisonType.None;
            ArmourRate = 1F;
            DamageRate = 1F;

            for (int i = PoisonList.Count - 1; i >= 0; i--)
            {
                if (Dead) return;

                Poison poison = PoisonList[i];
                if (poison.Owner != null && poison.Owner.Node == null)
                {
                    PoisonList.RemoveAt(i);
                    continue;
                }

                if (Envir.Time > poison.TickTime)
                {
                    poison.Time++;
                    poison.TickTime = Envir.Time + poison.TickSpeed;

                    if (poison.Time >= poison.Duration)
                        PoisonList.RemoveAt(i);

                    if (poison.PType == PoisonType.Green || poison.PType == PoisonType.Bleeding)
                    {
                        if (EXPOwner == null || EXPOwner.Dead)
                        {
                            EXPOwner = poison.Owner;
                            EXPOwnerTime = Envir.Time + EXPOwnerDelay;
                        }
                        else if (EXPOwner == poison.Owner)
                            EXPOwnerTime = Envir.Time + EXPOwnerDelay;

                        if (poison.PType == PoisonType.Bleeding)
                        {
                            Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Effect = SpellEffect.Bleeding, EffectType = 0 });
                        }

                        //ChangeHP(-poison.Value);
                        PoisonDamage(-poison.Value, poison.Owner);
                        if (PoisonStopRegen)
                            RegenTime = Envir.Time + RegenDelay;
                    }

                    if (poison.PType == PoisonType.DelayedExplosion)
                    {
                        if (Envir.Time > ExplosionInflictedTime) ExplosionInflictedStage++;

                        if (!ProcessDelayedExplosion(poison))
                        {
                            ExplosionInflictedStage = 0;
                            ExplosionInflictedTime = 0;

                            if (Dead) break; //temp to stop crashing

                            PoisonList.RemoveAt(i);
                            continue;
                        }
                    }
                }

                switch (poison.PType)
                {
                    case PoisonType.Red:
                        ArmourRate -= 0.5F;
                        break;
                    case PoisonType.Stun:
                        DamageRate += 0.5F;
                        break;
                    case PoisonType.Slow:
                        MoveSpeed += 100;
                        AttackSpeed += 100;

                        if (poison.Time >= poison.Duration)
                        {
                            MoveSpeed = Info.MoveSpeed;
                            AttackSpeed = Info.AttackSpeed;
                        }
                        break;
                }
                type |= poison.PType;
                /*
                if ((int)type < (int)poison.PType)
                    type = poison.PType;
                 */
            }


            if (type == CurrentPoison) return;

            CurrentPoison = type;
            Broadcast(new S.ObjectPoisoned { ObjectID = ObjectID, Poison = type });
        }

        private bool ProcessDelayedExplosion(Poison poison)
        {
            if (Dead) return false;

            if (ExplosionInflictedStage == 0)
            {
                Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Effect = SpellEffect.DelayedExplosion, EffectType = 0 });
                return true;
            }
            if (ExplosionInflictedStage == 1)
            {
                if (Envir.Time > ExplosionInflictedTime)
                    ExplosionInflictedTime = poison.TickTime + 3000;
                Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Effect = SpellEffect.DelayedExplosion, EffectType = 1 });
                return true;
            }
            if (ExplosionInflictedStage == 2)
            {
                Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Effect = SpellEffect.DelayedExplosion, EffectType = 2 });
                if (poison.Owner != null)
                {
                    switch (poison.Owner.Race)
                    {
                        case ObjectType.Player:
                            PlayerObject caster = (PlayerObject)poison.Owner;
                            DelayedAction action = new DelayedAction(DelayedType.Magic, Envir.Time, poison.Owner, caster.GetMagic(Spell.DelayedExplosion), poison.Value, this.CurrentLocation);
                            CurrentMap.ActionList.Add(action);
                            //Attacked((PlayerObject)poison.Owner, poison.Value, DefenceType.MAC, false);
                            break;
                        case ObjectType.Monster://this is in place so it could be used by mobs if one day someone chooses to
                            Attacked((MonsterObject)poison.Owner, poison.Value, DefenceType.MAC);
                            break;
                    }
                    LastHitter = poison.Owner;
                }
                return false;
            }
            return false;
        }


        private void ProcessBuffs()
        {
            bool refresh = false;
            for (int i = Buffs.Count - 1; i >= 0; i--)
            {
                Buff buff = Buffs[i];

                if (Envir.Time <= buff.ExpireTime) continue;

                Buffs.RemoveAt(i);

                switch (buff.Type)
                {
                    case BuffType.MoonLight:
                    case BuffType.Hiding:
                    case BuffType.DarkBody:
                        Hidden = false;
                        break;
                }

                refresh = true;
            }

            if (refresh) RefreshAll();
        }
        protected virtual void ProcessAI()
        {
            if (Dead) return;

            if (Respawn != null && Respawn.IsEventObjective)
            {
                //if monster is 10 yards away from the spawned location it goes into retreat or monster is outside event area
                if (!Retreat && (!Functions.InRange(CurrentLocation, SpawnedLocation, 10) || !Functions.InRange(Respawn.Event.CurrentLocation, CurrentLocation, Respawn.Event.Info.EventSize)))
                {
                    Retreat = true;
                    Target = null;
                }

                if (Retreat)
                {
                    if (HP != MaxHP)
                        ChangeHP((int)MaxHP);
                    if (Functions.InRange(CurrentLocation, SpawnedLocation, 2))
                        Retreat = false;
                    else
                    {
                        MoveTo(SpawnedLocation);
                        return;
                    }
                }
            }
            if (Master != null)
            {
                if ((Master.PMode == PetMode.Both || Master.PMode == PetMode.MoveOnly))
                {
                    if (!Functions.InRange(CurrentLocation, Master.CurrentLocation, Globals.DataRange) || CurrentMap != Master.CurrentMap)
                        PetRecall();
                }

                if (Master.PMode == PetMode.MoveOnly || Master.PMode == PetMode.None)
                    Target = null;
            }

            ProcessSearch();
            ProcessRoam();
            ProcessTarget();
        }
        protected virtual void ProcessSearch()
        {
            if (Envir.Time < SearchTime) return;
            if (Master != null && (Master.PMode == PetMode.MoveOnly || Master.PMode == PetMode.None)) return;

            SearchTime = Envir.Time + SearchDelay;

            if (CurrentMap.Inactive(5)) return;

            //Stacking or Infront of master - Move
            bool stacking = CheckStacked();

            if (CanMove && ((Master != null && Master.Front == CurrentLocation) || stacking))
            {
                //Walk Randomly
                if (!Walk(Direction))
                {
                    MirDirection dir = Direction;

                    switch (RandomUtils.Next(3)) // favour Clockwise
                    {
                        case 0:
                            for (int i = 0; i < 7; i++)
                            {
                                dir = Functions.NextDir(dir);

                                if (Walk(dir))
                                    break;
                            }
                            break;
                        default:
                            for (int i = 0; i < 7; i++)
                            {
                                dir = Functions.PreviousDir(dir);

                                if (Walk(dir))
                                    break;
                            }
                            break;
                    }
                }
            }

            if (Target == null || RandomUtils.Next(3) == 0)
                FindTarget();
        }
        protected virtual void ProcessRoam()
        {
            if (Target != null || Envir.Time < RoamTime) return;

            if (ProcessRoute()) return;

            if (CurrentMap.Inactive(30)) return;

            if (Master != null)
            {
                MoveTo(Master.Back);
                return;
            }

            RoamTime = Envir.Time + RoamDelay;
            if (RandomUtils.Next(10) != 0) return;

            switch (RandomUtils.Next(3)) //Face Walk
            {
                case 0:
                    Turn((MirDirection)RandomUtils.Next(8));
                    break;
                default:
                    Walk(Direction);
                    break;
            }
        }
        protected virtual void ProcessTarget()
        {
            if (Target == null || !CanAttack) return;

            if (InAttackRange())
            {
                Attack();
                if (Target.Dead)
                    FindTarget();

                return;
            }

            if (Envir.Time < ShockTime)
            {
                Target = null;
                return;
            }

            MoveTo(Target.CurrentLocation);
        }
        protected virtual bool InAttackRange()
        {
            if (Target.CurrentMap != CurrentMap) return false;

            return Target.CurrentLocation != CurrentLocation && Functions.InRange(CurrentLocation, Target.CurrentLocation, 1);
        }
        //这个是怪物的仇恨？
        //这个循环很坑啊
        protected virtual void FindTarget()
        {
            //if (CurrentMap.Players.Count < 1) return;
            Map Current = CurrentMap;

            for (int d = 0; d <= Info.ViewRange; d++)
            {
                for (int y = CurrentLocation.Y - d; y <= CurrentLocation.Y + d; y++)
                {
                    if (y < 0) continue;
                    if (y >= Current.Height) break;

                    for (int x = CurrentLocation.X - d; x <= CurrentLocation.X + d; x += Math.Abs(y - CurrentLocation.Y) == d ? 1 : d * 2)
                    {
                        if (x < 0) continue;
                        if (x >= Current.Width) break;
                        //Cell cell = Current.Cells[x, y];
                        if (Current.Objects[x, y] == null || !Current.Valid(x, y)) continue;
                        for (int i = 0; i < Current.Objects[x, y].Count; i++)
                        {
                            MapObject ob = Current.Objects[x, y][i];
                            switch (ob.Race)
                            {
                                case ObjectType.Monster:
                                    if (!ob.IsAttackTarget(this)) continue;
                                    if (ob.Hidden && (!CoolEye || Level < ob.Level)) continue;
                                    if (this is TrapRock && ob.InTrapRock) continue;
                                    Target = ob;
                                    return;
                                case ObjectType.Player:
                                    PlayerObject playerob = (PlayerObject)ob;
                                    if (!ob.IsAttackTarget(this)) continue;
                                    if (playerob.GMGameMaster || ob.Hidden && (!CoolEye || Level < ob.Level) || Envir.Time < HallucinationTime) continue;

                                    Target = ob;

                                    if (Master != null)
                                    {
                                        for (int j = 0; j < playerob.Pets.Count; j++)
                                        {
                                            MonsterObject pet = playerob.Pets[j];

                                            if (!pet.IsAttackTarget(this)) continue;
                                            Target = pet;
                                            break;
                                        }
                                    }
                                    return;
                                default:
                                    continue;
                            }
                        }
                    }
                }
            }
        }

        protected virtual bool ProcessRoute()
        {
            if (Route.Count < 1) return false;

            RoamTime = Envir.Time + 500;

            if (CurrentLocation == Route[RoutePoint].Location)
            {
                if (Route[RoutePoint].Delay > 0 && !Waiting)
                {
                    Waiting = true;
                    RoamTime = Envir.Time + RoamDelay + Route[RoutePoint].Delay;
                    return true;
                }

                Waiting = false;
                RoutePoint++;
            }

            if (RoutePoint > Route.Count - 1) RoutePoint = 0;

            if (!CurrentMap.ValidPoint(Route[RoutePoint].Location)) return true;

            MoveTo(Route[RoutePoint].Location);

            return true;
        }

        protected virtual void MoveTo(Point location)
        {
            if (CurrentLocation == location) return;

            bool inRange = Functions.InRange(location, CurrentLocation, 1);

            if (inRange)
            {
                if (!CurrentMap.ValidPoint(location)) return;
                //Cell cell = CurrentMap.GetCell(location);
                if (CurrentMap.Objects[location.X, location.Y] != null)
                    for (int i = 0; i < CurrentMap.Objects[location.X, location.Y].Count; i++)
                    {
                        MapObject ob = CurrentMap.Objects[location.X, location.Y][i];
                        if (!ob.Blocking) continue;
                        return;
                    }
            }

            MirDirection dir = Functions.DirectionFromPoint(CurrentLocation, location);

            if (Walk(dir)) return;

            switch (RandomUtils.Next(2)) //No favour
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

        public virtual void Turn(MirDirection dir)
        {
            if (!CanMove) return;

            Direction = dir;

            InSafeZone = CurrentMap.GetSafeZone(CurrentLocation) != null;


            //Cell cell = CurrentMap.GetCell(CurrentLocation);

            for (int i = 0; i < CurrentMap.Objects[CurrentLocation.X, CurrentLocation.Y].Count; i++)
            {
                if (CurrentMap.Objects[CurrentLocation.X, CurrentLocation.Y][i].Race != ObjectType.Spell) continue;
                SpellObject ob = (SpellObject)CurrentMap.Objects[CurrentLocation.X, CurrentLocation.Y][i];

                ob.ProcessSpell(this);
                //break;
            }


            Broadcast(new S.ObjectTurn { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
        }


        public virtual bool Walk(MirDirection dir)
        {
            if (!CanMove) return false;

            Point location = Functions.PointMove(CurrentLocation, dir, 1);

            if (!CurrentMap.ValidPoint(location)) return false;

            //Cell cell = CurrentMap.GetCell(location);

            if (CurrentMap.Objects[location.X, location.Y] != null)
            {
                for (int i = 0; i < CurrentMap.Objects[location.X, location.Y].Count; i++)
                {
                    MapObject ob = CurrentMap.Objects[location.X, location.Y][i];
                    if (!ob.Blocking || Race == ObjectType.Creature) continue;

                    return false;
                }
            }

            CurrentMap.Remove(CurrentLocation.X, CurrentLocation.Y, this);

            Direction = dir;
            RemoveObjects(dir, 1);
            CurrentLocation = location;
            CurrentMap.Add(this);
            AddObjects(dir, 1);

            if (Hidden)
            {
                Hidden = false;

                for (int i = 0; i < Buffs.Count; i++)
                {
                    if (Buffs[i].Type != BuffType.Hiding) continue;

                    Buffs[i].ExpireTime = 0;
                    break;
                }
            }


            CellTime = Envir.Time + 500;
            ActionTime = Envir.Time + 300;
            MoveTime = Envir.Time + MoveSpeed;
            if (MoveTime > AttackTime)
                AttackTime = MoveTime;

            InSafeZone = CurrentMap.GetSafeZone(CurrentLocation) != null;

            Broadcast(new S.ObjectWalk { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });


            //cell = CurrentMap.GetCell(CurrentLocation);

            for (int i = 0; i < CurrentMap.Objects[CurrentLocation.X, CurrentLocation.Y].Count; i++)
            {
                if (CurrentMap.Objects[CurrentLocation.X, CurrentLocation.Y][i].Race != ObjectType.Spell) continue;
                SpellObject ob = (SpellObject)CurrentMap.Objects[CurrentLocation.X, CurrentLocation.Y][i];

                ob.ProcessSpell(this);
                //break;
            }

            return true;
        }

        //这里增加一种怪物跑动的方法
        public virtual bool Run(MirDirection dir, byte distance = 2, bool Blocking = true)
        {
            if (!CanMove) return false;

            Point location = Functions.PointMove(CurrentLocation, dir, distance);

            if (!CurrentMap.ValidPoint(location)) return false;

            //Cell cell = CurrentMap.GetCell(location);

            if (CurrentMap.Objects[location.X, location.Y] != null)
            {
                for (int i = 0; i < CurrentMap.Objects[location.X, location.Y].Count; i++)
                {
                    MapObject ob = CurrentMap.Objects[location.X, location.Y][i];
                    if (Blocking)
                    {
                        if (!ob.Blocking || Race == ObjectType.Creature) continue;
                    }
                    return false;
                }
            }

            CurrentMap.Remove(CurrentLocation.X, CurrentLocation.Y, this);

            Direction = dir;
            RemoveObjects(dir, distance);
            CurrentLocation = location;
            CurrentMap.Add(this);
            AddObjects(dir, distance);

            if (Hidden)
            {
                Hidden = false;

                for (int i = 0; i < Buffs.Count; i++)
                {
                    if (Buffs[i].Type != BuffType.Hiding) continue;

                    Buffs[i].ExpireTime = 0;
                    break;
                }
            }


            CellTime = Envir.Time + 500;
            ActionTime = Envir.Time + 300;
            MoveTime = Envir.Time + MoveSpeed;
            if (MoveTime > AttackTime)
                AttackTime = MoveTime;

            InSafeZone = CurrentMap.GetSafeZone(CurrentLocation) != null;

            Broadcast(new S.ObjectRun { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });


            //cell = CurrentMap.GetCell(CurrentLocation);

            for (int i = 0; i < CurrentMap.Objects[CurrentLocation.X, CurrentLocation.Y].Count; i++)
            {
                if (CurrentMap.Objects[CurrentLocation.X, CurrentLocation.Y][i].Race != ObjectType.Spell) continue;
                SpellObject ob = (SpellObject)CurrentMap.Objects[CurrentLocation.X, CurrentLocation.Y][i];

                ob.ProcessSpell(this);
                //break;
            }

            return true;
        }








        protected virtual void Attack()
        {
            if (BindingShotCenter) ReleaseBindingShot();

            ShockTime = 0;

            if (!Target.IsAttackTarget(this))
            {
                Target = null;
                return;
            }


            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);
            Broadcast(new S.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });


            ActionTime = Envir.Time + 300;
            AttackTime = Envir.Time + AttackSpeed;

            int damage = GetAttackPower(MinDC, MaxDC);

            if (damage == 0) return;

            Target.Attacked(this, damage);
        }




        public List<MapObject> FindFriendsNearby(int distance)
        {
            List<MapObject> Friends = new List<MapObject>();
            for (int d = 0; d <= distance; d++)
            {
                for (int y = CurrentLocation.Y - d; y <= CurrentLocation.Y + d; y++)
                {
                    if (y < 0) continue;
                    if (y >= CurrentMap.Height) break;

                    for (int x = CurrentLocation.X - d; x <= CurrentLocation.X + d; x += Math.Abs(y - CurrentLocation.Y) == d ? 1 : d * 2)
                    {
                        if (x < 0) continue;
                        if (x >= CurrentMap.Width) break;
                        if (!CurrentMap.ValidPoint(x, y)) continue;
                        //Cell cell = CurrentMap.GetCell(x, y);
                        if (CurrentMap.Objects[x, y] == null) continue;

                        for (int i = 0; i < CurrentMap.Objects[x, y].Count; i++)
                        {
                            MapObject ob = CurrentMap.Objects[x, y][i];
                            switch (ob.Race)
                            {
                                case ObjectType.Monster:
                                case ObjectType.Player:
                                    if (ob == this || ob.Dead) continue;
                                    if (ob.IsAttackTarget(this)) continue;
                                    if (ob.Race == ObjectType.Player)
                                    {
                                        PlayerObject player = ((PlayerObject)ob);
                                        if (player.GMGameMaster) continue;
                                    }
                                    Friends.Add(ob);
                                    break;
                                default:
                                    continue;
                            }
                        }
                    }
                }
            }

            return Friends;
        }

        public void ReleaseBindingShot()
        {
            if (!BindingShotCenter) return;

            ShockTime = 0;
            Broadcast(GetInfo());//update clients in range (remove effect)
            BindingShotCenter = false;

            //the centertarget is escaped so make all shocked mobs awake (3x3 from center)
            Point place = CurrentLocation;
            for (int y = place.Y - 1; y <= place.Y + 1; y++)
            {
                if (y < 0) continue;
                if (y >= CurrentMap.Height) break;

                for (int x = place.X - 1; x <= place.X + 1; x++)
                {
                    if (x < 0) continue;
                    if (x >= CurrentMap.Width) break;

                    //Cell cell = CurrentMap.GetCell(x, y);
                    if (!CurrentMap.Valid(x, y) || CurrentMap.Objects[x, y] == null) continue;

                    for (int i = 0; i < CurrentMap.Objects[x, y].Count; i++)
                    {
                        MapObject targetob = CurrentMap.Objects[x, y][i];
                        if (targetob == null || targetob.Node == null || targetob.Race != ObjectType.Monster) continue;
                        if (((MonsterObject)targetob).ShockTime == 0) continue;

                        //each centerTarget has its own effect which needs to be cleared when no longer shocked
                        if (((MonsterObject)targetob).BindingShotCenter) ((MonsterObject)targetob).ReleaseBindingShot();
                        else ((MonsterObject)targetob).ShockTime = 0;

                        break;
                    }
                }
            }
        }

        public bool FindNearby(int distance)
        {
            for (int d = 0; d <= distance; d++)
            {
                for (int y = CurrentLocation.Y - d; y <= CurrentLocation.Y + d; y++)
                {
                    if (y < 0) continue;
                    if (y >= CurrentMap.Height) break;

                    for (int x = CurrentLocation.X - d; x <= CurrentLocation.X + d; x += Math.Abs(y - CurrentLocation.Y) == d ? 1 : d * 2)
                    {
                        if (x < 0) continue;
                        if (x >= CurrentMap.Width) break;
                        if (!CurrentMap.ValidPoint(x, y)) continue;
                        // Cell cell = CurrentMap.GetCell(x, y);
                        if (CurrentMap.Objects[x, y] == null) continue;

                        for (int i = 0; i < CurrentMap.Objects[x, y].Count; i++)
                        {
                            MapObject ob = CurrentMap.Objects[x, y][i];
                            switch (ob.Race)
                            {
                                case ObjectType.Monster:
                                case ObjectType.Player:
                                    if (!ob.IsAttackTarget(this)) continue;
                                    if (ob.Hidden && (!CoolEye || Level < ob.Level)) continue;
                                    if (ob.Race == ObjectType.Player)
                                    {
                                        PlayerObject player = ((PlayerObject)ob);
                                        if (player.GMGameMaster) continue;
                                    }
                                    return true;
                                default:
                                    continue;
                            }
                        }
                    }
                }
            }

            return false;
        }


        public List<MapObject> FindAllNearby(int dist, Point location, bool needSight = true)
        {
            List<MapObject> targets = new List<MapObject>();
            for (int d = 0; d <= dist; d++)
            {
                for (int y = location.Y - d; y <= location.Y + d; y++)
                {
                    if (y < 0) continue;
                    if (y >= CurrentMap.Height) break;

                    for (int x = location.X - d; x <= location.X + d; x += Math.Abs(y - location.Y) == d ? 1 : d * 2)
                    {
                        if (x < 0) continue;
                        if (x >= CurrentMap.Width) break;

                        //Cell cell = CurrentMap.GetCell(x, y);
                        if (!CurrentMap.Valid(x, y) || CurrentMap.Objects[x, y] == null) continue;

                        for (int i = 0; i < CurrentMap.Objects[x, y].Count; i++)
                        {
                            MapObject ob = CurrentMap.Objects[x, y][i];
                            switch (ob.Race)
                            {
                                case ObjectType.Monster:
                                case ObjectType.Player:
                                    targets.Add(ob);
                                    continue;
                                default:
                                    continue;
                            }
                        }
                    }
                }
            }
            return targets;
        }

        protected List<MapObject> FindAllTargets(int dist, Point location, bool needSight = true)
        {
            List<MapObject> targets = new List<MapObject>();
            for (int d = 0; d <= dist; d++)
            {
                for (int y = location.Y - d; y <= location.Y + d; y++)
                {
                    if (y < 0) continue;
                    if (y >= CurrentMap.Height) break;

                    for (int x = location.X - d; x <= location.X + d; x += Math.Abs(y - location.Y) == d ? 1 : d * 2)
                    {
                        if (x < 0) continue;
                        if (x >= CurrentMap.Width) break;

                        //Cell cell = CurrentMap.GetCell(x, y);
                        if (!CurrentMap.Valid(x, y) || CurrentMap.Objects[x, y] == null) continue;

                        for (int i = 0; i < CurrentMap.Objects[x, y].Count; i++)
                        {
                            MapObject ob = CurrentMap.Objects[x, y][i];
                            switch (ob.Race)
                            {
                                case ObjectType.Monster:
                                case ObjectType.Player:
                                    if (!ob.IsAttackTarget(this)) continue;
                                    if (ob.Hidden && (!CoolEye || Level < ob.Level) && needSight) continue;
                                    if (ob.Race == ObjectType.Player)
                                    {
                                        PlayerObject player = ((PlayerObject)ob);
                                        if (player.GMGameMaster) continue;
                                    }
                                    targets.Add(ob);
                                    continue;
                                default:
                                    continue;
                            }
                        }
                    }
                }
            }
            return targets;
        }

        public override bool IsAttackTarget(PlayerObject attacker)
        {
            if (attacker == null || attacker.Node == null) return false;
            if (Dead) return false;
            if (Master == null) return true;
            if (attacker.AMode == AttackMode.Peace) return false;
            if (Master == attacker) return attacker.AMode == AttackMode.All;
            if (Master.Race == ObjectType.Player && (attacker.InSafeZone || InSafeZone)) return false;

            switch (attacker.AMode)
            {
                case AttackMode.Group:
                    return Master.GroupMembers == null || !Master.GroupMembers.Contains(attacker);
                case AttackMode.Guild:
                    {
                        if (!(Master is PlayerObject)) return false;
                        PlayerObject master = (PlayerObject)Master;
                        return master.MyGuild == null || master.MyGuild != attacker.MyGuild;
                    }
                case AttackMode.EnemyGuild:
                    {
                        if (!(Master is PlayerObject)) return false;
                        PlayerObject master = (PlayerObject)Master;
                        return (master.MyGuild != null && attacker.MyGuild != null) && master.MyGuild.IsEnemy(attacker.MyGuild);
                    }
                case AttackMode.RedBrown:
                    return Master.PKPoints >= 200 || Envir.Time < Master.BrownTime;
                default:
                    return true;
            }
        }
        public override bool IsAttackTarget(MonsterObject attacker)
        {
            if (attacker == null || attacker.Node == null) return false;
            if (Dead || attacker == this) return false;
            if (attacker.Race == ObjectType.Creature) return false;

            if (attacker.Info.AI == 6) // Guard
            {
                if (Info.AI != 1 && Info.AI != 2 && Info.AI != 3 && (Master == null || Master.PKPoints >= 200)) //Not Dear/Hen/Tree/Pets or Red Master 
                    return true;
            }
            else if (attacker.Info.AI == 58) // Tao Guard - attacks Pets
            {
                if (Info.AI != 1 && Info.AI != 2 && Info.AI != 3) //Not Dear/Hen/Tree
                    return true;
            }
            else if (Master != null) //Pet Attacked
            {
                if (attacker.Master == null) //Wild Monster
                    return true;

                //Pet Vs Pet
                if (Master == attacker.Master)
                    return false;

                if (Envir.Time < ShockTime) //Shocked
                    return false;

                if (Master.Race == ObjectType.Player && attacker.Master.Race == ObjectType.Player && (Master.InSafeZone || attacker.Master.InSafeZone)) return false;

                switch (attacker.Master.AMode)
                {
                    case AttackMode.Group:
                        if (Master.GroupMembers != null && Master.GroupMembers.Contains((PlayerObject)attacker.Master)) return false;
                        break;
                    case AttackMode.Guild:
                        break;
                    case AttackMode.EnemyGuild:
                        break;
                    case AttackMode.RedBrown:
                        if (attacker.Master.PKPoints < 200 || Envir.Time > attacker.Master.BrownTime) return false;
                        break;
                    case AttackMode.Peace:
                        return false;
                }

                for (int i = 0; i < Master.Pets.Count; i++)
                    if (Master.Pets[i].EXPOwner == attacker.Master) return true;

                for (int i = 0; i < attacker.Master.Pets.Count; i++)
                {
                    MonsterObject ob = attacker.Master.Pets[i];
                    if (ob == Target || ob.Target == this) return true;
                }

                return Master.LastHitter == attacker.Master;
            }
            else if (attacker.Master != null) //Pet Attacking Wild Monster
            {
                if (Envir.Time < ShockTime) //Shocked
                    return false;

                for (int i = 0; i < attacker.Master.Pets.Count; i++)
                {
                    MonsterObject ob = attacker.Master.Pets[i];
                    if (ob == Target || ob.Target == this) return true;
                }

                if (Target == attacker.Master)
                    return true;
            }

            if (Envir.Time < attacker.HallucinationTime) return true;

            return Envir.Time < attacker.RageTime;
        }
        public override bool IsFriendlyTarget(PlayerObject ally)
        {
            if (Master == null) return false;
            if (Master == ally) return true;

            switch (ally.AMode)
            {
                case AttackMode.Group:
                    return Master.GroupMembers != null && Master.GroupMembers.Contains(ally);
                case AttackMode.Guild:
                    return false;
                case AttackMode.EnemyGuild:
                    return true;
                case AttackMode.RedBrown:
                    return Master.PKPoints < 200 & Envir.Time > Master.BrownTime;
            }
            return true;
        }

        public override bool IsFriendlyTarget(MonsterObject ally)
        {
            if (Master != null) return false;
            if (ally.Race != ObjectType.Monster) return false;
            if (ally.Master != null) return false;

            return true;
        }

        public override int Attacked(PlayerObject attacker, int damage, DefenceType type = DefenceType.ACAgility, bool damageWeapon = true)
        {
            #region Weapon Effects Pete107 26/1/2016//武器效果
            UserItem _item = attacker.Info.Equipment[(int)EquipmentSlot.Weapon];
            Random randy = new Random();
            if (_item != null && Target != null && randy.Next(0, 100) >= 10)
            {
                switch (_item.Info.Effect)
                {
                    case 0:
                        break;
                    case 1:
                        CurrentMap.Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Effect = SpellEffect.FatalSword }, CurrentLocation);
                        
                        break;
                    case 2:
                        CurrentMap.Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Effect = SpellEffect.Teleport }, CurrentLocation);
                        break;
                    case 3:
                        CurrentMap.Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Effect = SpellEffect.Healing }, CurrentLocation);
                        break;
                    case 4:
                        CurrentMap.Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Effect = SpellEffect.RedMoonEvil }, CurrentLocation);
                        break;
                    case 5:
                        CurrentMap.Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Effect = SpellEffect.TwinDrakeBlade }, CurrentLocation);
                        break;
                    case 6:
                        CurrentMap.Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Effect = SpellEffect.MagicShieldUp }, CurrentLocation);
                        break;
                    case 7:
                        CurrentMap.Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Effect = SpellEffect.MagicShieldDown }, CurrentLocation);
                        break;
                    case 8:
                        CurrentMap.Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Effect = SpellEffect.GreatFoxSpirit }, CurrentLocation);
                        break;
                    case 9:
                        CurrentMap.Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Effect = SpellEffect.Entrapment }, CurrentLocation);
                        break;
                    case 10:
                        CurrentMap.Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Effect = SpellEffect.Reflect }, CurrentLocation);
                        break;
                    case 11:
                        CurrentMap.Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Effect = SpellEffect.Critical }, CurrentLocation);
                        break;
                    case 12:
                        CurrentMap.Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Effect = SpellEffect.Mine }, CurrentLocation);
                        break;
                    case 13:
                        CurrentMap.Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Effect = SpellEffect.ElementalBarrierUp }, CurrentLocation);
                        break;
                    case 14:
                        CurrentMap.Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Effect = SpellEffect.ElementalBarrierDown }, CurrentLocation);
                        break;
                    case 15:
                        CurrentMap.Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Effect = SpellEffect.DelayedExplosion }, CurrentLocation);
                        break;
                    case 16:
                        CurrentMap.Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Effect = SpellEffect.MPEater }, CurrentLocation);
                        break;
                    case 17:
                        CurrentMap.Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Effect = SpellEffect.Hemorrhage }, CurrentLocation);
                        break;
                    case 18:
                        CurrentMap.Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Effect = SpellEffect.Bleeding }, CurrentLocation);
                        break;
                    case 19:
                        CurrentMap.Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Effect = SpellEffect.StormEscape }, CurrentLocation);
                        break;
                    default:
                        break;
                }
            }
            #endregion
            if (Target == null && attacker.IsAttackTarget(this))
            {
                Target = attacker;
            }

            int armour = 0;

            switch (type)
            {
                case DefenceType.ACAgility:
                    if (RandomUtils.Next(Agility + 1) > attacker.Accuracy)
                    {
                        BroadcastDamageIndicator(DamageType.Miss);
                        return 0;
                    }
                    armour = GetDefencePower(MinAC, MaxAC);
                    break;
                case DefenceType.AC:
                    armour = GetDefencePower(MinAC, MaxAC);
                    break;
                case DefenceType.MACAgility:
                    if (RandomUtils.Next(Agility + 1) > attacker.Accuracy)
                    {
                        BroadcastDamageIndicator(DamageType.Miss);
                        return 0;
                    }
                    armour = GetDefencePower(MinMAC, MaxMAC);
                    break;
                case DefenceType.MAC:
                    armour = GetDefencePower(MinMAC, MaxMAC);
                    break;
                case DefenceType.Agility:
                    if (RandomUtils.Next(Agility + 1) > attacker.Accuracy)
                    {
                        BroadcastDamageIndicator(DamageType.Miss);
                        return 0;
                    }
                    break;
            }

            armour = (int)Math.Max(int.MinValue, (Math.Min(int.MaxValue, (decimal)(armour * ArmourRate))));
            damage = (int)Math.Max(int.MinValue, (Math.Min(int.MaxValue, (decimal)(damage * DamageRate))));

            if (damageWeapon)
                attacker.DamageWeapon();
            damage += attacker.AttackBonus;

            if (armour >= damage)
            {
                BroadcastDamageIndicator(DamageType.Miss);
                return 0;
            }

            if ((attacker.CriticalRate * Settings.CriticalRateWeight) > RandomUtils.Next(100))
            {
                Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Effect = SpellEffect.Critical });
                damage = Math.Min(int.MaxValue, damage + (int)Math.Floor(damage * (((double)attacker.CriticalDamage / (double)Settings.CriticalDamageWeight) * 10)));
                BroadcastDamageIndicator(DamageType.Critical);
            }

            if (attacker.LifeOnHit > 0)
                attacker.ChangeHP(attacker.LifeOnHit);

            if (Target != this && attacker.IsAttackTarget(this))
            {
                if (attacker.Info.MentalState == 2)
                {
                    if (Functions.MaxDistance(CurrentLocation, attacker.CurrentLocation) < (8 - attacker.Info.MentalStateLvl))
                        Target = attacker;
                }
                else
                    Target = attacker;
            }

            if (BindingShotCenter) ReleaseBindingShot();
            ShockTime = 0;

            for (int i = PoisonList.Count - 1; i >= 0; i--)
            {
                if (PoisonList[i].PType != PoisonType.LRParalysis) continue;

                PoisonList.RemoveAt(i);
                OperateTime = 0;
            }

            if (Master != null && Master != attacker)
                if (Envir.Time > Master.BrownTime && Master.PKPoints < 200)
                    attacker.BrownTime = Envir.Time + Settings.Minute;

            if (EXPOwner == null || EXPOwner.Dead)
                EXPOwner = attacker;

            if (EXPOwner == attacker)
                EXPOwnerTime = Envir.Time + EXPOwnerDelay;

            ushort LevelOffset = (ushort)(Level > attacker.Level ? 0 : Math.Min(10, attacker.Level - Level));

            if (attacker.HasParalysisRing && type != DefenceType.MAC && type != DefenceType.MACAgility && 1 == RandomUtils.Next(1, 15))
            {
                ApplyPoison(new Poison { PType = PoisonType.Paralysis, Duration = 5, TickSpeed = 1000 }, attacker);
            }

            if (attacker.Freezing > 0 && type != DefenceType.MAC && type != DefenceType.MACAgility)
            {
                if ((RandomUtils.Next(Settings.FreezingAttackWeight) < attacker.Freezing) && (RandomUtils.Next(LevelOffset) == 0))
                    ApplyPoison(new Poison { PType = PoisonType.Slow, Duration = Math.Min(10, (3 + RandomUtils.Next(attacker.Freezing))), TickSpeed = 1000 }, attacker);
            }

            if (attacker.PoisonAttack > 0 && type != DefenceType.MAC && type != DefenceType.MACAgility)
            {
                if ((RandomUtils.Next(Settings.PoisonAttackWeight) < attacker.PoisonAttack) && (RandomUtils.Next(LevelOffset) == 0))
                    ApplyPoison(new Poison { PType = PoisonType.Green, Duration = 5, TickSpeed = 1000, Value = Math.Min(10, 3 + RandomUtils.Next(attacker.PoisonAttack)) }, attacker);
            }

            Broadcast(new S.ObjectStruck { ObjectID = ObjectID, AttackerID = attacker.ObjectID, Direction = Direction, Location = CurrentLocation });

            if (attacker.HpDrainRate > 0)
            {
                attacker.HpDrain += Math.Max(0, ((float)(damage - armour) / 100) * attacker.HpDrainRate);
                if (attacker.HpDrain > 2)
                {
                    int HpGain = (int)Math.Floor(attacker.HpDrain);
                    attacker.ChangeHP(HpGain);
                    attacker.HpDrain -= HpGain;

                }
            }

            attacker.GatherElement();

            if (attacker.Info.Mentor != 0 && attacker.Info.isMentor)
            {
                Buff buff = attacker.Buffs.Where(e => e.Type == BuffType.Mentor).FirstOrDefault();
                if (buff != null)
                {
                    CharacterInfo Mentee = Envir.GetCharacterInfo(attacker.Info.Mentor);
                    PlayerObject player = Envir.GetPlayer(Mentee.Name);
                    if (player.CurrentMap == attacker.CurrentMap && Functions.InRange(player.CurrentLocation, attacker.CurrentLocation, Globals.DataRange) && !player.Dead)
                    {
                        damage += ((damage / 100) * Settings.MentorDamageBoost);
                    }
                }
            }

            if (Master != null && Master != attacker && Master.Race == ObjectType.Player && Envir.Time > Master.BrownTime && Master.PKPoints < 200 && !((PlayerObject)Master).AtWar(attacker))
            {
                attacker.BrownTime = Envir.Time + Settings.Minute;
            }

            for (int i = 0; i < attacker.Pets.Count; i++)
            {
                MonsterObject ob = attacker.Pets[i];

                if (IsAttackTarget(ob) && (ob.Target == null)) ob.Target = this;
            }

            BroadcastDamageIndicator(DamageType.Hit, armour - damage);
            if (Respawn != null && Respawn.IsEventObjective && attacker.tempEvent != null)
            {
                if (!Contributers.Contains(attacker))
                    Contributers.Add(attacker);
            }
            ChangeHP(armour - damage);
            return damage - armour;
        }
        public override int Attacked(MonsterObject attacker, int damage, DefenceType type = DefenceType.ACAgility)
        {
            if (Target == null && attacker.IsAttackTarget(this))
                Target = attacker;

            int armour = 0;

            switch (type)
            {
                case DefenceType.ACAgility:
                    if (RandomUtils.Next(Agility + 1) > attacker.Accuracy)
                    {
                        BroadcastDamageIndicator(DamageType.Miss);
                        return 0;
                    }
                    armour = GetDefencePower(MinAC, MaxAC);
                    break;
                case DefenceType.AC:
                    armour = GetDefencePower(MinAC, MaxAC);
                    break;
                case DefenceType.MACAgility:
                    if (RandomUtils.Next(Agility + 1) > attacker.Accuracy)
                    {
                        BroadcastDamageIndicator(DamageType.Miss);
                        return 0;
                    }
                    armour = GetDefencePower(MinMAC, MaxMAC);
                    break;
                case DefenceType.MAC:
                    armour = GetDefencePower(MinMAC, MaxMAC);
                    break;
                case DefenceType.Agility:
                    if (RandomUtils.Next(Agility + 1) > attacker.Accuracy)
                    {
                        BroadcastDamageIndicator(DamageType.Miss);
                        return 0;
                    }
                    break;
            }

            armour = (int)Math.Max(int.MinValue, (Math.Min(int.MaxValue, (decimal)(armour * ArmourRate))));
            damage = (int)Math.Max(int.MinValue, (Math.Min(int.MaxValue, (decimal)(damage * DamageRate))));

            if (armour >= damage)
            {
                BroadcastDamageIndicator(DamageType.Miss);
                return 0;
            }

            if (Target != this && attacker.IsAttackTarget(this))
                Target = attacker;

            if (BindingShotCenter) ReleaseBindingShot();
            ShockTime = 0;

            for (int i = PoisonList.Count - 1; i >= 0; i--)
            {
                if (PoisonList[i].PType != PoisonType.LRParalysis) continue;

                PoisonList.RemoveAt(i);
                OperateTime = 0;
            }

            if (attacker.Info.AI == 6 || attacker.Info.AI == 58)
                EXPOwner = null;

            else if (attacker.Master != null)
            {
                if (attacker.CurrentMap != attacker.Master.CurrentMap || !Functions.InRange(attacker.CurrentLocation, attacker.Master.CurrentLocation, Globals.DataRange))
                    EXPOwner = null;
                else
                {

                    if (EXPOwner == null || EXPOwner.Dead)
                        EXPOwner = attacker.Master;

                    if (EXPOwner == attacker.Master)
                        EXPOwnerTime = Envir.Time + EXPOwnerDelay;
                }

            }

            Broadcast(new S.ObjectStruck { ObjectID = ObjectID, AttackerID = attacker.ObjectID, Direction = Direction, Location = CurrentLocation });

            BroadcastDamageIndicator(DamageType.Hit, armour - damage);
            if (Respawn != null && Respawn.IsEventObjective && attacker.Master != null && attacker.Master is PlayerObject)
            {
                var playerAttacker = (PlayerObject)attacker.Master;
                if (!Contributers.Contains(playerAttacker) && playerAttacker.tempEvent != null)
                    Contributers.Add(playerAttacker);
            }

            ChangeHP(armour - damage);
            return damage - armour;
        }

        public override int Struck(int damage, DefenceType type = DefenceType.ACAgility)
        {
            int armour = 0;

            switch (type)
            {
                case DefenceType.ACAgility:
                    armour = GetDefencePower(MinAC, MaxAC);
                    break;
                case DefenceType.AC:
                    armour = GetDefencePower(MinAC, MaxAC);
                    break;
                case DefenceType.MACAgility:
                    armour = GetDefencePower(MinMAC, MaxMAC);
                    break;
                case DefenceType.MAC:
                    armour = GetDefencePower(MinMAC, MaxMAC);
                    break;
                case DefenceType.Agility:
                    break;
            }

            armour = (int)Math.Max(int.MinValue, (Math.Min(int.MaxValue, (decimal)(armour * ArmourRate))));
            damage = (int)Math.Max(int.MinValue, (Math.Min(int.MaxValue, (decimal)(damage * DamageRate))));

            if (armour >= damage) return 0;
            Broadcast(new S.ObjectStruck { ObjectID = ObjectID, AttackerID = 0, Direction = Direction, Location = CurrentLocation });

            ChangeHP(armour - damage);
            return damage - armour;
        }

        public override void ApplyPoison(Poison p, MapObject Caster = null, bool NoResist = false, bool ignoreDefence = true)
        {
            if (p.Owner != null && p.Owner.IsAttackTarget(this))
                Target = p.Owner;

            if (Master != null && p.Owner != null && p.Owner.Race == ObjectType.Player && p.Owner != Master)
            {
                if (Envir.Time > Master.BrownTime && Master.PKPoints < 200)
                    p.Owner.BrownTime = Envir.Time + Settings.Minute;
            }

            if (!ignoreDefence && (p.PType == PoisonType.Green))
            {
                int armour = GetDefencePower(MinMAC, MaxMAC);

                if (p.Value < armour)
                    p.PType = PoisonType.None;
                else
                    p.Value -= armour;
            }

            if (p.PType == PoisonType.None) return;

            for (int i = 0; i < PoisonList.Count; i++)
            {
                if (PoisonList[i].PType != p.PType) continue;
                if ((PoisonList[i].PType == PoisonType.Green) && (PoisonList[i].Value > p.Value)) return;//cant cast weak poison to cancel out strong poison
                if ((PoisonList[i].PType != PoisonType.Green) && ((PoisonList[i].Duration - PoisonList[i].Time) > p.Duration)) return;//cant cast 1 second poison to make a 1minute poison go away!
                if (p.PType == PoisonType.DelayedExplosion) return;
                if ((PoisonList[i].PType == PoisonType.Frozen) || (PoisonList[i].PType == PoisonType.Slow) || (PoisonList[i].PType == PoisonType.Paralysis) || (PoisonList[i].PType == PoisonType.LRParalysis)) return;//prevents mobs from being perma frozen/slowed
                PoisonList[i] = p;
                return;
            }

            if (p.PType == PoisonType.DelayedExplosion)
            {
                ExplosionInflictedTime = Envir.Time + 4000;
                Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Effect = SpellEffect.DelayedExplosion });
            }

            PoisonList.Add(p);
        }
        public override void AddBuff(Buff b)
        {
            if (Buffs.Any(d => d.Infinite && d.Type == b.Type)) return; //cant overwrite infinite buff with regular buff

            string caster = b.Caster != null ? b.Caster.Name : string.Empty;

            if (b.Values == null) b.Values = new int[1];

            S.AddBuff addBuff = new S.AddBuff { Type = b.Type, Caster = caster, Expire = b.ExpireTime - Envir.Time, Values = b.Values, Infinite = b.Infinite, ObjectID = ObjectID, Visible = b.Visible };

            if (b.Visible) Broadcast(addBuff);

            base.AddBuff(b);
            RefreshAll();
        }

        public override Packet GetInfo()
        {
            return new S.ObjectMonster
            {
                ObjectID = ObjectID,
                Name = Name,
                NameColour = NameColour,
                Location = CurrentLocation,
                Image = Info.Image,
                Direction = Direction,
                Effect = Info.Effect,
                AI = Info.AI,
                Light = Info.Light,
                Dead = Dead,
                Skeleton = Harvested,
                Poison = CurrentPoison,
                Hidden = Hidden,
                ShockTime = (ShockTime > 0 ? ShockTime - Envir.Time : 0),
                BindingShotCenter = BindingShotCenter
            };
        }

        public override void ReceiveChat(string text, ChatType type)
        {
            throw new NotSupportedException();
        }

        public void RemoveObjects(MirDirection dir, int count)
        {
            switch (dir)
            {
                case MirDirection.Up:
                    //Bottom Block
                    for (int a = 0; a < count; a++)
                    {
                        int y = CurrentLocation.Y + Globals.DataRange - a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = -Globals.DataRange; b <= Globals.DataRange; b++)
                        {
                            int x = CurrentLocation.X + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            //Cell cell = CurrentMap.GetCell(x, y);

                            if (!CurrentMap.Valid(x, y) || CurrentMap.Objects[x, y] == null) continue;

                            for (int i = 0; i < CurrentMap.Objects[x, y].Count; i++)
                            {
                                MapObject ob = CurrentMap.Objects[x, y][i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Remove(this);
                            }
                        }
                    }
                    break;
                case MirDirection.UpRight:
                    //Bottom Block
                    for (int a = 0; a < count; a++)
                    {
                        int y = CurrentLocation.Y + Globals.DataRange - a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = -Globals.DataRange; b <= Globals.DataRange; b++)
                        {
                            int x = CurrentLocation.X + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            //Cell cell = CurrentMap.GetCell(x, y);

                            if (!CurrentMap.Valid(x, y) || CurrentMap.Objects[x, y] == null) continue;

                            for (int i = 0; i < CurrentMap.Objects[x, y].Count; i++)
                            {
                                MapObject ob = CurrentMap.Objects[x, y][i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Remove(this);
                            }
                        }
                    }

                    //Left Block
                    for (int a = -Globals.DataRange; a <= Globals.DataRange - count; a++)
                    {
                        int y = CurrentLocation.Y + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = 0; b < count; b++)
                        {
                            int x = CurrentLocation.X - Globals.DataRange + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            //Cell cell = CurrentMap.GetCell(x, y);

                            if (!CurrentMap.Valid(x, y) || CurrentMap.Objects[x, y] == null) continue;

                            for (int i = 0; i < CurrentMap.Objects[x, y].Count; i++)
                            {
                                MapObject ob = CurrentMap.Objects[x, y][i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Remove(this);
                            }
                        }
                    }
                    break;
                case MirDirection.Right:
                    //Left Block
                    for (int a = -Globals.DataRange; a <= Globals.DataRange; a++)
                    {
                        int y = CurrentLocation.Y + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = 0; b < count; b++)
                        {
                            int x = CurrentLocation.X - Globals.DataRange + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            //Cell cell = CurrentMap.GetCell(x, y);

                            if (!CurrentMap.Valid(x, y) || CurrentMap.Objects[x, y] == null) continue;

                            for (int i = 0; i < CurrentMap.Objects[x, y].Count; i++)
                            {
                                MapObject ob = CurrentMap.Objects[x, y][i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Remove(this);
                            }
                        }
                    }
                    break;
                case MirDirection.DownRight:
                    //Top Block
                    for (int a = 0; a < count; a++)
                    {
                        int y = CurrentLocation.Y - Globals.DataRange + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = -Globals.DataRange; b <= Globals.DataRange; b++)
                        {
                            int x = CurrentLocation.X + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            //Cell cell = CurrentMap.GetCell(x, y);

                            if (!CurrentMap.Valid(x, y) || CurrentMap.Objects[x, y] == null) continue;

                            for (int i = 0; i < CurrentMap.Objects[x, y].Count; i++)
                            {
                                MapObject ob = CurrentMap.Objects[x, y][i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Remove(this);
                            }
                        }
                    }

                    //Left Block
                    for (int a = -Globals.DataRange + count; a <= Globals.DataRange; a++)
                    {
                        int y = CurrentLocation.Y + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = 0; b < count; b++)
                        {
                            int x = CurrentLocation.X - Globals.DataRange + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            //Cell cell = CurrentMap.GetCell(x, y);

                            if (!CurrentMap.Valid(x, y) || CurrentMap.Objects[x, y] == null) continue;

                            for (int i = 0; i < CurrentMap.Objects[x, y].Count; i++)
                            {
                                MapObject ob = CurrentMap.Objects[x, y][i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Remove(this);
                            }
                        }
                    }
                    break;
                case MirDirection.Down:
                    for (int a = 0; a < count; a++)
                    {
                        int y = CurrentLocation.Y - Globals.DataRange + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = -Globals.DataRange; b <= Globals.DataRange; b++)
                        {
                            int x = CurrentLocation.X + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            //Cell cell = CurrentMap.GetCell(x, y);

                            if (!CurrentMap.Valid(x, y) || CurrentMap.Objects[x, y] == null) continue;

                            for (int i = 0; i < CurrentMap.Objects[x, y].Count; i++)
                            {
                                MapObject ob = CurrentMap.Objects[x, y][i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Remove(this);
                            }
                        }
                    }
                    break;
                case MirDirection.DownLeft:
                    //Top Block
                    for (int a = 0; a < count; a++)
                    {
                        int y = CurrentLocation.Y - Globals.DataRange + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = -Globals.DataRange; b <= Globals.DataRange; b++)
                        {
                            int x = CurrentLocation.X + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            //Cell cell = CurrentMap.GetCell(x, y);

                            if (!CurrentMap.Valid(x, y) || CurrentMap.Objects[x, y] == null) continue;

                            for (int i = 0; i < CurrentMap.Objects[x, y].Count; i++)
                            {
                                MapObject ob = CurrentMap.Objects[x, y][i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Remove(this);
                            }
                        }
                    }

                    //Right Block
                    for (int a = -Globals.DataRange + count; a <= Globals.DataRange; a++)
                    {
                        int y = CurrentLocation.Y + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = 0; b < count; b++)
                        {
                            int x = CurrentLocation.X + Globals.DataRange - b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            //Cell cell = CurrentMap.GetCell(x, y);

                            if (!CurrentMap.Valid(x, y) || CurrentMap.Objects[x, y] == null) continue;

                            for (int i = 0; i < CurrentMap.Objects[x, y].Count; i++)
                            {
                                MapObject ob = CurrentMap.Objects[x, y][i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Remove(this);
                            }
                        }
                    }
                    break;
                case MirDirection.Left:
                    for (int a = -Globals.DataRange; a <= Globals.DataRange; a++)
                    {
                        int y = CurrentLocation.Y + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = 0; b < count; b++)
                        {
                            int x = CurrentLocation.X + Globals.DataRange - b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            //Cell cell = CurrentMap.GetCell(x, y);

                            if (!CurrentMap.Valid(x, y) || CurrentMap.Objects[x, y] == null) continue;

                            for (int i = 0; i < CurrentMap.Objects[x, y].Count; i++)
                            {
                                MapObject ob = CurrentMap.Objects[x, y][i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Remove(this);
                            }
                        }
                    }
                    break;
                case MirDirection.UpLeft:
                    //Bottom Block
                    for (int a = 0; a < count; a++)
                    {
                        int y = CurrentLocation.Y + Globals.DataRange - a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = -Globals.DataRange; b <= Globals.DataRange; b++)
                        {
                            int x = CurrentLocation.X + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            //Cell cell = CurrentMap.GetCell(x, y);

                            if (!CurrentMap.Valid(x, y) || CurrentMap.Objects[x, y] == null) continue;

                            for (int i = 0; i < CurrentMap.Objects[x, y].Count; i++)
                            {
                                MapObject ob = CurrentMap.Objects[x, y][i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Remove(this);
                            }
                        }
                    }

                    //Right Block
                    for (int a = -Globals.DataRange; a <= Globals.DataRange - count; a++)
                    {
                        int y = CurrentLocation.Y + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = 0; b < count; b++)
                        {
                            int x = CurrentLocation.X + Globals.DataRange - b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            //Cell cell = CurrentMap.GetCell(x, y);

                            if (!CurrentMap.Valid(x, y) || CurrentMap.Objects[x, y] == null) continue;

                            for (int i = 0; i < CurrentMap.Objects[x, y].Count; i++)
                            {
                                MapObject ob = CurrentMap.Objects[x, y][i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Remove(this);
                            }
                        }
                    }
                    break;
            }
        }
        public void AddObjects(MirDirection dir, int count)
        {
            switch (dir)
            {
                case MirDirection.Up:
                    for (int a = 0; a < count; a++)
                    {
                        int y = CurrentLocation.Y - Globals.DataRange + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = -Globals.DataRange; b <= Globals.DataRange; b++)
                        {
                            int x = CurrentLocation.X + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            //Cell cell = CurrentMap.GetCell(x, y);

                            if (!CurrentMap.Valid(x, y) || CurrentMap.Objects[x, y] == null) continue;

                            for (int i = 0; i < CurrentMap.Objects[x, y].Count; i++)
                            {
                                MapObject ob = CurrentMap.Objects[x, y][i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Add(this);
                            }
                        }
                    }
                    break;
                case MirDirection.UpRight:
                    //Top Block
                    for (int a = 0; a < count; a++)
                    {
                        int y = CurrentLocation.Y - Globals.DataRange + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = -Globals.DataRange; b <= Globals.DataRange; b++)
                        {
                            int x = CurrentLocation.X + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            //Cell cell = CurrentMap.GetCell(x, y);

                            if (!CurrentMap.Valid(x, y) || CurrentMap.Objects[x, y] == null) continue;

                            for (int i = 0; i < CurrentMap.Objects[x, y].Count; i++)
                            {
                                MapObject ob = CurrentMap.Objects[x, y][i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Add(this);
                            }
                        }
                    }

                    //Right Block
                    for (int a = -Globals.DataRange + count; a <= Globals.DataRange; a++)
                    {
                        int y = CurrentLocation.Y + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = 0; b < count; b++)
                        {
                            int x = CurrentLocation.X + Globals.DataRange - b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            //Cell cell = CurrentMap.GetCell(x, y);

                            if (!CurrentMap.Valid(x, y) || CurrentMap.Objects[x, y] == null) continue;

                            for (int i = 0; i < CurrentMap.Objects[x, y].Count; i++)
                            {
                                MapObject ob = CurrentMap.Objects[x, y][i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Add(this);
                            }
                        }
                    }
                    break;
                case MirDirection.Right:
                    for (int a = -Globals.DataRange; a <= Globals.DataRange; a++)
                    {
                        int y = CurrentLocation.Y + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = 0; b < count; b++)
                        {
                            int x = CurrentLocation.X + Globals.DataRange - b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            //Cell cell = CurrentMap.GetCell(x, y);

                            if (!CurrentMap.Valid(x, y) || CurrentMap.Objects[x, y] == null) continue;

                            for (int i = 0; i < CurrentMap.Objects[x, y].Count; i++)
                            {
                                MapObject ob = CurrentMap.Objects[x, y][i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Add(this);
                            }
                        }
                    }
                    break;
                case MirDirection.DownRight:
                    //Bottom Block
                    for (int a = 0; a < count; a++)
                    {
                        int y = CurrentLocation.Y + Globals.DataRange - a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = -Globals.DataRange; b <= Globals.DataRange; b++)
                        {
                            int x = CurrentLocation.X + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            //Cell cell = CurrentMap.GetCell(x, y);

                            if (!CurrentMap.Valid(x, y) || CurrentMap.Objects[x, y] == null) continue;

                            for (int i = 0; i < CurrentMap.Objects[x, y].Count; i++)
                            {
                                MapObject ob = CurrentMap.Objects[x, y][i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Add(this);
                            }
                        }
                    }

                    //Right Block
                    for (int a = -Globals.DataRange; a <= Globals.DataRange - count; a++)
                    {
                        int y = CurrentLocation.Y + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = 0; b < count; b++)
                        {
                            int x = CurrentLocation.X + Globals.DataRange - b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            //Cell cell = CurrentMap.GetCell(x, y);

                            if (!CurrentMap.Valid(x, y) || CurrentMap.Objects[x, y] == null) continue;

                            for (int i = 0; i < CurrentMap.Objects[x, y].Count; i++)
                            {
                                MapObject ob = CurrentMap.Objects[x, y][i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Add(this);
                            }
                        }
                    }
                    break;
                case MirDirection.Down:
                    //Bottom Block
                    for (int a = 0; a < count; a++)
                    {
                        int y = CurrentLocation.Y + Globals.DataRange - a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = -Globals.DataRange; b <= Globals.DataRange; b++)
                        {
                            int x = CurrentLocation.X + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            //Cell cell = CurrentMap.GetCell(x, y);

                            if (!CurrentMap.Valid(x, y) || CurrentMap.Objects[x, y] == null) continue;

                            for (int i = 0; i < CurrentMap.Objects[x, y].Count; i++)
                            {
                                MapObject ob = CurrentMap.Objects[x, y][i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Add(this);
                            }
                        }
                    }
                    break;
                case MirDirection.DownLeft:
                    //Bottom Block
                    for (int a = 0; a < count; a++)
                    {
                        int y = CurrentLocation.Y + Globals.DataRange - a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = -Globals.DataRange; b <= Globals.DataRange; b++)
                        {
                            int x = CurrentLocation.X + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            // Cell cell = CurrentMap.GetCell(x, y);

                            if (!CurrentMap.Valid(x, y) || CurrentMap.Objects[x, y] == null) continue;

                            for (int i = 0; i < CurrentMap.Objects[x, y].Count; i++)
                            {
                                MapObject ob = CurrentMap.Objects[x, y][i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Add(this);
                            }
                        }
                    }

                    //Left Block
                    for (int a = -Globals.DataRange; a <= Globals.DataRange - count; a++)
                    {
                        int y = CurrentLocation.Y + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = 0; b < count; b++)
                        {
                            int x = CurrentLocation.X - Globals.DataRange + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            //Cell cell = CurrentMap.GetCell(x, y);

                            if (!CurrentMap.Valid(x, y) || CurrentMap.Objects[x, y] == null) continue;

                            for (int i = 0; i < CurrentMap.Objects[x, y].Count; i++)
                            {
                                MapObject ob = CurrentMap.Objects[x, y][i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Add(this);
                            }
                        }
                    }
                    break;
                case MirDirection.Left:
                    //Left Block
                    for (int a = -Globals.DataRange; a <= Globals.DataRange; a++)
                    {
                        int y = CurrentLocation.Y + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = 0; b < count; b++)
                        {
                            int x = CurrentLocation.X - Globals.DataRange + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            //Cell cell = CurrentMap.GetCell(x, y);

                            if (!CurrentMap.Valid(x, y) || CurrentMap.Objects[x, y] == null) continue;

                            for (int i = 0; i < CurrentMap.Objects[x, y].Count; i++)
                            {
                                MapObject ob = CurrentMap.Objects[x, y][i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Add(this);
                            }
                        }
                    }
                    break;
                case MirDirection.UpLeft:
                    //Top Block
                    for (int a = 0; a < count; a++)
                    {
                        int y = CurrentLocation.Y - Globals.DataRange + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = -Globals.DataRange; b <= Globals.DataRange; b++)
                        {
                            int x = CurrentLocation.X + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            //Cell cell = CurrentMap.GetCell(x, y);

                            if (!CurrentMap.Valid(x, y) || CurrentMap.Objects[x, y] == null) continue;

                            for (int i = 0; i < CurrentMap.Objects[x, y].Count; i++)
                            {
                                MapObject ob = CurrentMap.Objects[x, y][i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Add(this);
                            }
                        }
                    }

                    //Left Block
                    for (int a = -Globals.DataRange + count; a <= Globals.DataRange; a++)
                    {
                        int y = CurrentLocation.Y + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = 0; b < count; b++)
                        {
                            int x = CurrentLocation.X - Globals.DataRange + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            //Cell cell = CurrentMap.GetCell(x, y);

                            if (!CurrentMap.Valid(x, y) || CurrentMap.Objects[x, y] == null) continue;

                            for (int i = 0; i < CurrentMap.Objects[x, y].Count; i++)
                            {
                                MapObject ob = CurrentMap.Objects[x, y][i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Add(this);
                            }
                        }
                    }
                    break;
            }
        }
      
        public override void Add(PlayerObject player)
        {
            player.Enqueue(GetInfo());
            SendHealth(player);
        }

        public override void SendHealth(PlayerObject player)
        {
            if (!player.IsMember(Master) && !(player.IsMember(EXPOwner) && AutoRev) && Envir.Time > RevTime) return;
            byte time = Math.Min(byte.MaxValue, (byte)Math.Max(5, (RevTime - Envir.Time) / 1000));
            player.Enqueue(new S.ObjectHealth { ObjectID = ObjectID, HP = this.HP, MaxHP = this.MaxHP, Expire = time });
        }

        public void PetExp(uint amount)
        {
            if (PetLevel >= MaxPetLevel) return;

            if (Info.Name == Settings.SkeletonName || Info.Name == Settings.ShinsuName || Info.Name == Settings.AngelName || Info.Name == Settings.SkeletonName1)
                amount *= 3;

            PetExperience += amount;

            if (PetExperience < (PetLevel + 1) * 20000) return;

            PetExperience = (uint)(PetExperience - ((PetLevel + 1) * 20000));
            PetLevel++;
            RefreshAll();
            OperateTime = 0;
            BroadcastHealthChange();
        }
        public override void Despawn()
        {
            SlaveList.Clear();
            base.Despawn();
        }

    }

    /// <summary>
    /// 采用怪物对象包裹器，包裹怪物
    /// 这里只记录简单的怪物的位置，血量，仇恨等个性东西，其他的都用具体的怪物属性
    /// 采用怪物属性引用的方式，避免大量刷怪占用的内存属性
    /// </summary>
    public class MonsterObjectInstance
    {

    }
}