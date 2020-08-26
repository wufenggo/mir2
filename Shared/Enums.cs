﻿using System;

public enum PanelType : byte
{
    Buy = 0,
    Sell,
    Repair,
    SpecialRepair,
    Consign,
    Craft,
    Refine,
    CheckRefine,
    Disassemble,
    Downgrade,
    Reset,
    CollectRefine,
    ReplaceWedRing,
}

public enum MarketItemType : byte
{
    Consign,
    Auction,
    GameShop
}

public enum MarketPanelType : byte
{
    Market,
    Consign,
    Auction,
    GameShop
}

public enum BlendMode : sbyte
{
    NONE = -1,
    NORMAL = 0,
    LIGHT = 1,
    LIGHTINV = 2,
    INVNORMAL = 3,
    INVLIGHT = 4,
    INVLIGHTINV = 5,
    INVCOLOR = 6,
    INVBACKGROUND = 7
}

public enum DamageType : byte
{
    Hit = 0,
    Miss = 1,
    Critical = 2
}

[Flags]
public enum GMOptions : byte
{
    None = 0,
    GameMaster = 0x0001,
    Observer = 0x0002,
    Superman = 0x0004
}

public enum AwakeType : byte
{
    None = 0,
    DC,
    MC,
    SC,
    AC,
    MAC,
    HPMP,
}

[Flags]
public enum LevelEffects : byte
{
    None = 0,
    Mist = 0x0001,
    RedDragon = 0x0002,
    BlueDragon = 0x0004
}

public enum OutputMessageType : byte
{
    Normal,
    Quest,
    Guild
}

public enum ItemGrade : byte
{
    None = 0,
    Common = 1,
    Rare = 2,
    Legendary = 3,
    Mythical = 4,
}
public enum StatType : byte
{
    AC = 0,
    MAC = 1,
    DC = 2,
    MC = 3,
    SC = 4,
    HP = 5,
    MP = 6,
    HP_Percent = 7,
    MP_Percent = 8,
    HP_Regen = 9,
    MP_Regen = 10,
    ASpeed = 11,
    Luck = 12,
    Strong = 13,
    Accuracy = 14,
    Agility = 15,
    MagicResist = 16,
    PoisonResist = 17,
    PoisonAttack = 18,
    PoisonRegen = 19,
    Freezing = 20,
    Holy = 21,
    Durability = 22,
    Unknown = 23
}
public enum RefinedValue : byte
{
    None = 0,
    DC = 1,
    MC = 2,
    SC = 3,
}

public enum QuestType : byte
{
    General = 0,
    Daily = 1,
    Repeatable = 2,
    Story = 3
}

public enum QuestIcon : byte
{
    None = 0,
    QuestionWhite = 1,
    ExclamationYellow = 2,
    QuestionYellow = 3,
    ExclamationBlue = 5,
    QuestionBlue = 6,
    ExclamationGreen = 52,
    QuestionGreen = 53
}

public enum QuestState : byte
{
    Add,
    Update,
    Remove
}

public enum DefaultNPCType : byte
{
    Login,
    LevelUp,
    UseItem,
    MapCoord,
    MapEnter,
    Die,
    Trigger,
    CustomCommand,
    OnAcceptQuest,
    OnFinishQuest,
    Daily,
    Client
}

public enum IntelligentCreatureType : byte
{
    None = 99,
    BabyPig = 0,
    Chick = 1,
    Kitten = 2,
    BabySkeleton = 3,
    Baekdon = 4,
    Wimaen = 5,
    BlackKitten = 6,
    BabyDragon = 7,
    OlympicFlame = 8,
    BabySnowMan = 9,
    Frog = 10,
    BabyMonkey = 11,
    AngryBird = 12,
    Foxey = 13,
    MedicalRat = 14,
}

//3 blank mob files
public enum Monster : ushort
{
    Guard = 0,
    TaoistGuard = 1,
    Guard2 = 2,
    Hen = 3,
    Deer = 4,
    Scarecrow = 5,
    HookingCat = 6,
    RakingCat = 7,
    Yob = 8,
    Oma = 9,
    CannibalPlant = 10,
    ForestYeti = 11,
    SpittingSpider = 12,
    ChestnutTree = 13,
    EbonyTree = 14,
    LargeMushroom = 15,
    CherryTree = 16,
    OmaFighter = 17,
    OmaWarrior = 18,
    CaveBat = 19,
    CaveMaggot = 20,
    Scorpion = 21,
    Skeleton = 22,
    BoneFighter = 23,
    AxeSkeleton = 24,
    BoneWarrior = 25,
    BoneElite = 26,
    Dung = 27,
    Dark = 28,
    WoomaSoldier = 29,
    WoomaFighter = 30,
    WoomaWarrior = 31,
    FlamingWooma = 32,
    WoomaGuardian = 33,
    WoomaTaurus = 34,
    WhimperingBee = 35,
    GiantWorm = 36,
    Centipede = 37,
    BlackMaggot = 38,
    Tongs = 39,
    EvilTongs = 40,
    EvilCentipede = 41,
    BugBat = 42,
    BugBatMaggot = 43,
    WedgeMoth = 44,
    RedBoar = 45,
    BlackBoar = 46,
    SnakeScorpion = 47,
    WhiteBoar = 48,
    EvilSnake = 49,
    BombSpider = 50,
    RootSpider = 51,
    SpiderBat = 52,
    VenomSpider = 53,
    GangSpider = 54,
    GreatSpider = 55,
    LureSpider = 56,
    BigApe = 57,
    EvilApe = 58,
    GrayEvilApe = 59,
    RedEvilApe = 60,
    CrystalSpider = 61,
    RedMoonEvil = 62,
    BigRat = 63,
    ZumaArcher = 64,
    ZumaStatue = 65,
    ZumaGuardian = 66,
    RedThunderZuma = 67,
    ZumaTaurus = 68,
    DigOutZombie = 69,
    ClZombie = 70,
    NdZombie = 71,
    CrawlerZombie = 72,
    ShamanZombie = 73,
    Ghoul = 74,
    KingScorpion = 75,
    KingHog = 76,
    DarkDevil = 77,
    BoneFamiliar = 78,
    Shinsu = 79,
    Shinsu1 = 80,
    SpiderFrog = 81,
    HoroBlaster = 82,
    BlueHoroBlaster = 83,
    KekTal = 84,
    VioletKekTal = 85,
    Khazard = 86,
    RoninGhoul = 87,
    ToxicGhoul = 88,
    BoneCaptain = 89,
    BoneSpearman = 90,
    BoneBlademan = 91,
    BoneArcher = 92,
    BoneLord = 93,
    Minotaur = 94,
    IceMinotaur = 95,
    ElectricMinotaur = 96,
    WindMinotaur = 97,
    FireMinotaur = 98,
    RightGuard = 99,
    LeftGuard = 100,
    MinotaurKing = 101,
    FrostTiger = 102,
    Sheep = 103,
    Wolf = 104,
    ShellNipper = 105,
    Keratoid = 106,
    GiantKeratoid = 107,
    SkyStinger = 108,
    SandWorm = 109,
    VisceralWorm = 110,
    RedSnake = 111,
    TigerSnake = 112,
    Yimoogi = 113,
    GiantWhiteSnake = 114,
    BlueSnake = 115,
    YellowSnake = 116,
    HolyDeva = 117,
    AxeOma = 118,
    SwordOma = 119,
    CrossbowOma = 120,
    WingedOma = 121,
    FlailOma = 122,
    OmaGuard = 123,
    YinDevilNode = 124,
    YangDevilNode = 125,
    OmaKing = 126,
    BlackFoxman = 127,
    RedFoxman = 128,
    WhiteFoxman = 129,
    TrapRock = 130,
    GuardianRock = 131,
    ThunderElement = 132,
    CloudElement = 133,
    GreatFoxSpirit = 134,
    HedgeKekTal = 135,
    BigHedgeKekTal = 136,
    RedFrogSpider = 137,
    BrownFrogSpider = 138,
    ArcherGuard = 139,
    KatanaGuard = 140,
    ArcherGuard2 = 141,
    Pig = 142,
    Bull = 143,
    Bush = 144,
    ChristmasTree = 145,
    HighAssassin = 146,
    DarkDustPile = 147,
    DarkBrownWolf = 148,
    Football = 149,
    GingerBreadman = 150,
    HalloweenScythe = 151,
    GhastlyLeecher = 152,
    CyanoGhast = 153,
    MutatedManworm = 154,
    CrazyManworm = 155,
    MudPile = 156,
    TailedLion = 157,
    Behemoth = 158,//done BOSS
    DarkDevourer = 159,//done
    PoisonHugger = 160,//done
    Hugger = 161,//done
    MutatedHugger = 162,//done
    DreamDevourer = 163,//done
    Treasurebox = 164,//done
    SnowPile = 165,//done
    Snowman = 166,//done
    SnowTree = 167,//done
    GiantEgg = 168,//done
    RedTurtle = 169,//done
    GreenTurtle = 170,//done
    BlueTurtle = 171,//done
    Catapult1 = 172, //not added frames
    Catapult2 = 173, //not added frames
    Blank1 = 174,
    SiegeRepairman = 175, //not added frames
    BlueSanta = 176,//done
    BattleStandard = 177,//done
    Blank2 = 178,
    RedYimoogi = 179,//done
    LionRiderMale = 180, //frames not added
    LionRiderFemale = 181, //frames not added
    Tornado = 182,//done
    FlameTiger = 183,//done
    WingedTigerLord = 184,//done BOSS
    TowerTurtle = 185,//done
    FinialTurtle = 186,//done
    TurtleKing = 187,//done BOSS
    DarkTurtle = 188,//done
    LightTurtle = 189,//done  
    DarkSwordOma = 190,//done
    DarkAxeOma = 191,//done
    DarkCrossbowOma = 192,//done
    DarkWingedOma = 193,//done
    BoneWhoo = 194,//done
    DarkSpider = 195,//done
    ViscusWorm = 196,//done
    ViscusCrawler = 197,//done
    CrawlerLave = 198,//done
    DarkYob = 199,//done

    FlamingMutant = 200,//FINISH
    StoningStatue = 201,//FINISH BOSS
    FlyingStatue = 202,//FINISH
    ValeBat = 203,//done
    Weaver = 204,//done
    VenomWeaver = 205,//done
    CrackingWeaver = 206,//done
    ArmingWeaver = 207,//done
    CrystalWeaver = 208,//done
    FrozenZumaStatue = 209,//done
    FrozenZumaGuardian = 210,//done
    FrozenRedZuma = 211,//done
    GreaterWeaver = 212,//done
    SpiderWarrior = 213,//done
    SpiderBarbarian = 214,//done
    HellSlasher = 215,//done
    HellPirate = 216,//done
    HellCannibal = 217,//done
    HellKeeper = 218, //done BOSS
    HellBolt = 219, //done
    WitchDoctor = 220,//done
    ManectricHammer = 221,//done
    ManectricClub = 222,//done
    ManectricClaw = 223,//done
    ManectricStaff = 224,//done
    NamelessGhost = 225,//done
    DarkGhost = 226,//done
    ChaosGhost = 227,//done
    ManectricBlest = 228,//done
    ManectricKing = 229,//done
    Blank3 = 230,
    IcePillar = 231,//done
    FrostYeti = 232,//done
    ManectricSlave = 233,//done
    TrollHammer = 234,//done
    TrollBomber = 235,//done
    TrollStoner = 236,//done
    TrollKing = 237,//done BOSS
    FlameSpear = 238,//done
    FlameMage = 239,//done
    FlameScythe = 240,//done
    FlameAssassin = 241,//done
    FlameQueen = 242, //finish BOSS
    HellKnight1 = 243,//done
    HellKnight2 = 244,//done
    HellKnight3 = 245,//done
    HellKnight4 = 246,//done
    HellLord = 247,//done BOSS
    WaterGuard = 248,//done
    IceGuard = 249,
    ElementGuard = 250,
    DemonGuard = 251,
    KingGuard = 252,
    Snake10 = 253,//done
    Snake11 = 254,//done
    Snake12 = 255,//done
    Snake13 = 256,//done
    Snake14 = 257,//done
    Snake15 = 258,//done
    Snake16 = 259,//done
    Snake17 = 260,//done

    DeathCrawler = 261,
    BurningZombie = 262,
    MudZombie = 263,
    FrozenZombie = 264,
    UndeadWolf = 265,
    Demonwolf = 266,
    WhiteMammoth = 267,
    DarkBeast = 268,
    LightBeast = 269,
    BloodBaboon = 270,
    HardenRhino = 271,
    AncientBringer = 272,
    FightingCat = 273,
    FireCat = 274,
    CatWidow = 275,
    StainHammerCat = 276,
    BlackHammerCat = 277,
    StrayCat = 278,
    CatShaman = 279,
    Jar1 = 280,
    Jar2 = 281,
    SeedingsGeneral = 282,
    RestlessJar = 283,
    GeneralJinmYo = 284,
    Bunny = 285,
    Tucson = 286,
    TucsonFighter = 287,
    TucsonMage = 288,
    TucsonWarrior = 289,
    Armadillo = 290,
    ArmadilloElder = 291,
    TucsonEgg = 292,
    PlaguedTucson = 293,
    SandSnail = 294,
    CannibalTentacles = 295,
    TucsonGeneral = 296,
    GasToad = 297,
    Mantis = 298,
    SwampWarrior = 299,

    AssassinBird = 300,
    RhinoWarrior = 301,
    RhinoPriest = 302,
    SwampSlime = 303,
    RockGuard = 304,
    MudWarrior = 305,
    SmallPot = 306,
    TreeQueen = 307,
    ShellFighter = 308,
    DarkBaboon = 309,
    TwinHeadBeast = 310,
    OmaCannibal = 311,
    OmaBlest = 312,
    OmaSlasher = 313,
    OmaAssassin = 314,
    OmaMage = 315,
    OmaWitchDoctor = 316,
    LightningBead = 317,
    HealingBead = 318,
    PowerUpBead = 319,
    DarkOmaKing = 320,
    CaveMage = 321,
    Mandrill = 322,
    PlagueCrab = 323,
    CreeperPlant = 324,
    FloatingWraith = 325,
    ArmedPlant = 326,
    AvengerPlant = 327,
    Nadz = 328,
    AvengingSpirit = 329,
    AvengingWarrior = 330,
    AxePlant = 331,
    WoodBox = 332,
    ClawBeast = 333,
    KillerPlant = 334,
    SackWarrior = 335,
    WereTiger = 336,
    KingHydrax = 337,
    Hydrax = 338,
    HornedMage = 339,
    Basiloid = 340,
    HornedArcher = 341,
    ColdArcher = 342,
    HornedWarrior = 343,
    FloatingRock = 344,
    ScalyBeast = 345,
    HornedSorceror = 346,
    BoulderSpirit = 347,
    HornedCommander = 348,
    MoonStone = 349,

    SunStone = 350,
    LightningStone = 351,
    Turtlegrass = 352,
    Mantree = 353,
    Bear = 354,
    Leopard = 355,
    ChieftainArcher = 356,
    ChieftainSword = 357,
    StoningSpider = 358, //Archer Spell mob (not yet coded)
    VampireSpider = 359, //Archer Spell mob
    SpittingToad = 360, //Archer Spell mob
    SnakeTotem = 361, //Archer Spell mob
    CharmedSnake = 362, //Archer Spell mob
    FrozenSoldier = 363,
    FrozenFighter = 364,
    FrozenArcher = 365,
    FrozenKnight = 366,
    FrozenGolem = 367,
    IcePhantom = 368,
    SnowWolf = 369,
    SnowWolfKing = 370,
    WaterDragon = 371,
    BlackTortoise = 372,
    Manticore = 373,
    DragonWarrior = 374,
    DragonArcher = 375,
    Kirin = 376,
    Guard3 = 377,
    ArcherGuard3 = 378,
    Bunny2 = 379,
    FrozenMiner = 380,
    FrozenAxeman = 381,
    FrozenMagician = 382,
    SnowYeti = 383,
    IceCrystalSoldier = 384,
    DarkWraith = 385,
    DarkSpirit = 386,
    CrystalBeast = 387,
    RedOrb = 388,
    BlueOrb = 389,
    YellowOrb = 390,
    GreenOrb = 391,
    WhiteOrb = 392,
    FatalLotus = 393,
    AntCommander = 394,
    CargoBoxwithlogo = 395,
    Doe = 396,
    Reindeer = 397, //frames not added
    AngryReindeer = 398,
    CargoBox = 399,

    Ram1 = 400,
    Ram2 = 401,
    Kite = 402,
    PurpleFaeFlower = 403,
    Furball = 404,
    GlacierSnail = 405,
    FurbolgWarrior = 406,
    FurbolgArcher = 407,
    FurbolgCommander = 408,
    RedFaeFlower = 409,
    FurbolgGuard = 410,
    GlacierBeast = 411,
    GlacierWarrior = 412,
    ShardGuardian = 413,
    WarriorScroll = 414,
    TaoistScroll = 415,
    WizardScroll = 416,
    AssassinScroll = 417,
    HoodedSummoner = 418, //Summons Scrolls
    HoodedIceMage = 419,
    HoodedPriest = 420,
    ShardMaiden = 421,
    KingKong = 422,
    WarBear = 423,
    ReaperPriest = 424,
    ReaperWizard = 425,
    ReaperAssassin = 426,
    LivingVines = 427,
    BlueMonk = 428,
    MutantBeserker = 429,
    MutantGuardian = 430,
    MutantHighPriest = 431,
    MysteriousMage = 432,
    FeatheredWolf = 433,
    MysteriousAssassin = 434,
    MysteriousMonk = 435,
    ManEatingPlant = 436,
    HammerDwarf = 437,
    ArcherDwarf = 438,
    NobleWarrior = 439,
    NobleArcher = 440,
    NoblePriest = 441,
    NobleAssassin = 442,
    Swain = 443,
    RedMutantPlant = 444,
    BlueMutantPlant = 445,
    UndeadHammerDwarf = 446,
    UndeadDwarfArcher = 447,
    AncientStoneGolem = 448,
    Serpentirian = 449,

    Butcher = 450,
    Riklebites = 451,
    FeralTundraFurbolg = 452,
    FeralFlameFurbolg = 453,
    ArcaneTotem = 454,
    SpectralWraith = 455,
    BabyMagmaDragon = 456,
    BloodLord = 457,
    SerpentLord = 458,
    MirEmperor = 459,
    MutantManEatingPlant = 460,
    MutantWarg = 461,
    GrassElemental = 462,
    RockElemental = 463,

    EvilMir = 900,
    EvilMirBody = 901,
    DragonStatue = 902,
    HellBomb1 = 903,
    HellBomb2 = 904,
    HellBomb3 = 905,

    SabukGate = 950,
    PalaceWallLeft = 951,
    PalaceWall1 = 952,
    PalaceWall2 = 953,
    GiGateSouth = 954,
    GiGateEast = 955,
    GiGateWest = 956,
    SSabukWall1 = 957,
    SSabukWall2 = 958,
    SSabukWall3 = 959,
    NammandGate1 = 960, //Not Coded
    NammandGate2 = 961, //Not Coded
    SabukWallSection = 962, //Not Coded
    NammandWallSection = 963, //Not Coded
    FrozenDoor = 964, //Not Coded

    BabyPig = 10000,//Permanent
    Chick = 10001,//Special
    Kitten = 10002,//Permanent
    BabySkeleton = 10003,//Special
    Baekdon = 10004,//Special
    Wimaen = 10005,//Event
    BlackKitten = 10006,//unknown
    BabyDragon = 10007,//unknown
    OlympicFlame = 10008,//unknown
    BabySnowMan = 10009,//unknown
    Frog = 10010,//unknown
    BabyMonkey = 10011,//unknown
    AngryBird = 10012,
    Foxey = 10013,
    MedicalRat = 10014,
}

public enum MirAction : byte
{
    Standing,
    Walking,
    Running,
    Pushed,
    DashL,
    DashR,
    DashFail,
    Stance,
    Stance2,
    Attack1,
    Attack2,
    Attack3,
    Attack4,
    Attack5,
    AttackRange1,
    AttackRange2,
    AttackRange3,
    Special,
    Struck,
    Harvest,
    Spell,
    Die,
    Dead,
    Skeleton,
    Show,
    Hide,
    Stoned,
    Appear,
    Revive,
    SitDown,
    Mine,
    Sneek,
    DashAttack,
    Lunge,

    WalkingBow,
    RunningBow,
    Jump,

    MountStanding,
    MountWalking,
    MountRunning,
    MountStruck,
    MountAttack,

    FishingCast,
    FishingWait,
    FishingReel
}

public enum CellAttribute : byte
{
    Walk = 0,
    HighWall = 1,
    LowWall = 2,
}

public enum LightSetting : byte
{
    Normal = 0,
    Dawn = 1,
    Day = 2,
    Evening = 3,
    Night = 4
}

public enum MirGender : byte
{
    Male = 0,
    Female = 1
}

public enum MirClass : byte
{
    Warrior = 0,
    Wizard = 1,
    Taoist = 2,
    Assassin = 3,
    Archer = 4
}

public enum MirDirection : byte
{
    Up = 0,
    UpRight = 1,
    Right = 2,
    DownRight = 3,
    Down = 4,
    DownLeft = 5,
    Left = 6,
    UpLeft = 7
}

public enum ObjectType : byte
{
    None = 0,
    Player = 1,
    Item = 2,
    Merchant = 3,
    Spell = 4,
    Monster = 5,
    Deco = 6,
    Creature = 7
}

public enum ChatType : byte
{
    Normal = 0,
    Shout = 1,
    System = 2,
    Hint = 3,
    Announcement = 4,
    Group = 5,
    WhisperIn = 6,
    WhisperOut = 7,
    Guild = 8,
    Trainer = 9,
    LevelUp = 10,
    System2 = 11,
    Relationship = 12,
    Mentor = 13,
    Shout2 = 14,
    Shout3 = 15
}

public enum ItemType : byte
{
    Nothing = 0,
    Weapon = 1,
    Armour = 2,
    Helmet = 4,
    Necklace = 5,
    Bracelet = 6,
    Ring = 7,
    Amulet = 8,
    Belt = 9,
    Boots = 10,
    Stone = 11,
    Torch = 12,
    Potion = 13,
    Ore = 14,
    Meat = 15,
    CraftingMaterial = 16,
    Scroll = 17,
    Gem = 18,
    Mount = 19,
    Book = 20,
    Script = 21,
    Reins = 22,
    Bells = 23,
    Saddle = 24,
    Ribbon = 25,
    Mask = 26,
    Food = 27,
    Hook = 28,
    Float = 29,
    Bait = 30,
    Finder = 31,
    Reel = 32,
    Fish = 33,
    Quest = 34,
    Awakening = 35,
    Pets = 36,
    Transform = 37,
    Deco = 38
}

public enum MirGridType : byte
{
    None = 0,
    Inventory = 1,
    Equipment = 2,
    Trade = 3,
    Storage = 4,
    BuyBack = 5,
    DropPanel = 6,
    Inspect = 7,
    TrustMerchant = 8,
    GuildStorage = 9,
    GuestTrade = 10,
    Mount = 11,
    Fishing = 12,
    QuestInventory = 13,
    AwakenItem = 14,
    Mail = 15,
    Refine = 16,
    Renting = 17,
    GuestRenting = 18,
    Craft = 19
}

public enum EquipmentSlot : byte
{
    Weapon = 0,
    Armour = 1,
    Helmet = 2,
    Torch = 3,
    Necklace = 4,
    BraceletL = 5,
    BraceletR = 6,
    RingL = 7,
    RingR = 8,
    Amulet = 9,
    Belt = 10,
    Boots = 11,
    Stone = 12,
    Mount = 13
}

public enum MountSlot : byte
{
    Reins = 0,
    Bells = 1,
    Saddle = 2,
    Ribbon = 3,
    Mask = 4
}

public enum FishingSlot : byte
{
    Hook = 0,
    Float = 1,
    Bait = 2,
    Finder = 3,
    Reel = 4
}

public enum AttackMode : byte
{
    Peace = 0,
    Group = 1,
    Guild = 2,
    EnemyGuild = 3,
    RedBrown = 4,
    All = 5
}

public enum PetMode : byte
{
    Both = 0,
    MoveOnly = 1,
    AttackOnly = 2,
    None = 3,
}

[Flags]
public enum PoisonType : ushort
{
    None = 0,
    Green = 1,
    Red = 2,
    Slow = 4,
    Frozen = 8,
    Stun = 16,
    Paralysis = 32,
    DelayedExplosion = 64,
    Bleeding = 128,
    LRParalysis = 256
}

[Flags]

public enum BindMode : short
{
    None = 0,
    DontDeathdrop = 1,//0x0001
    DontDrop = 2,//0x0002
    DontSell = 4,//0x0004
    DontStore = 8,//0x0008
    DontTrade = 16,//0x0010
    DontRepair = 32,//0x0020
    DontUpgrade = 64,//0x0040
    DestroyOnDrop = 128,//0x0080
    BreakOnDeath = 256,//0x0100
    BindOnEquip = 512,//0x0200
    NoSRepair = 1024,//0x0400
    NoWeddingRing = 2048,//0x0800
    UnableToRent = 4096,
    UnableToDisassemble = 8192,
    NoMail = 16384
}

[Flags]
public enum SpecialItemMode : short
{
    None = 0,
    Paralize = 0x0001,
    Teleport = 0x0002,
    Clearring = 0x0004,
    Protection = 0x0008,
    Revival = 0x0010,
    Muscle = 0x0020,
    Flame = 0x0040,
    Healing = 0x0080,
    Probe = 0x0100,
    Skill = 0x0200,
    NoDuraLoss = 0x0400,
    Blink = 0x800,
}

[Flags]
public enum RequiredClass : byte
{
    Warrior = 1,
    Wizard = 2,
    Taoist = 4,
    Assassin = 8,
    Archer = 16,
    WarWizTao = Warrior | Wizard | Taoist,
    None = WarWizTao | Assassin | Archer
}

[Flags]
public enum RequiredGender : byte
{
    Male = 1,
    Female = 2,
    None = Male | Female
}

public enum RequiredType : byte
{
    Level = 0,
    MaxAC = 1,
    MaxMAC = 2,
    MaxDC = 3,
    MaxMC = 4,
    MaxSC = 5,
    MaxLevel = 6,
    MinAC = 7,
    MinMAC = 8,
    MinDC = 9,
    MinMC = 10,
    MinSC = 11,
}

public enum ItemSet : byte
{
    None = 0,
    Spirit = 1,
    Recall = 2,
    RedOrchid = 3,
    RedFlower = 4,
    Smash = 5,
    HwanDevil = 6,
    Purity = 7,
    FiveString = 8,
    Mundane = 9,
    NokChi = 10,
    TaoProtect = 11,
    Mir = 12,
    Bone = 13,
    Bug = 14,
    WhiteGold = 15,
    WhiteGoldH = 16,
    RedJade = 17,
    RedJadeH = 18,
    Nephrite = 19,
    NephriteH = 20,
    Whisker1 = 21,
    Whisker2 = 22,
    Whisker3 = 23,
    Whisker4 = 24,
    Whisker5 = 25,
    Hyeolryong = 26,
    Monitor = 27,
    Oppressive = 28,
    Paeok = 29,
    Sulgwan = 30,
    BlueFrost = 31,
    DarkGhost = 38,
    BlueFrostH = 39
}

public enum Spell : byte
{
    None = 0,

    //Warrior
    Fencing = 1,
    Slaying = 2,
    Thrusting = 3,
    HalfMoon = 4,
    ShoulderDash = 5,
    TwinDrakeBlade = 6,
    Entrapment = 7,
    FlamingSword = 8,
    LionRoar = 9,
    CrossHalfMoon = 10,
    BladeAvalanche = 11,
    ProtectionField = 12,
    Rage = 13,
    CounterAttack = 14,
    SlashingBurst = 15,
    Fury = 16,
    ImmortalSkin = 17,

    //Wizard
    FireBall = 31,
    Repulsion = 32,
    ElectricShock = 33,
    GreatFireBall = 34,
    HellFire = 35,
    ThunderBolt = 36,
    Teleport = 37,
    FireBang = 38,
    FireWall = 39,
    Lightning = 40,
    FrostCrunch = 41,
    ThunderStorm = 42,
    MagicShield = 43,
    TurnUndead = 44,
    Vampirism = 45,
    IceStorm = 46,
    FlameDisruptor = 47,
    Mirroring = 48,
    FlameField = 49,
    Blizzard = 50,
    MagicBooster = 51,
    MeteorStrike = 52,
    IceThrust = 53,
    FastMove = 54,
    StormEscape = 55,

    //Taoist
    Healing = 61,
    SpiritSword = 62,
    Poisoning = 63,
    SoulFireBall = 64,
    SummonSkeleton = 65,
    Hiding = 67,
    MassHiding = 68,
    SoulShield = 69,
    Revelation = 70,
    BlessedArmour = 71,
    EnergyRepulsor = 72,
    TrapHexagon = 73,
    Purification = 74,
    MassHealing = 75,
    Hallucination = 76,
    UltimateEnhancer = 77,
    SummonShinsu = 78,
    Reincarnation = 79,
    SummonHolyDeva = 80,
    Curse = 81,
    Plague = 82,
    PoisonCloud = 83,
    EnergyShield = 84,
    PetEnhancer = 85,
    HealingCircle = 86,

    //Assassin
    FatalSword = 91,
    DoubleSlash = 92,
    Haste = 93,
    FlashDash = 94,
    LightBody = 95,
    HeavenlySword = 96,
    FireBurst = 97,
    Trap = 98,
    PoisonSword = 99,
    MoonLight = 100,
    MPEater = 101,
    SwiftFeet = 102,
    DarkBody = 103,
    Hemorrhage = 104,
    CrescentSlash = 105,
    MoonMist = 106,

    //Archer
    Focus = 121,
    StraightShot = 122,
    DoubleShot = 123,
    ExplosiveTrap = 124,
    DelayedExplosion = 125,
    Meditation = 126,
    BackStep = 127,
    ElementalShot = 128,
    Concentration = 129,
    Stonetrap = 130,
    ElementalBarrier = 131,
    SummonVampire = 132,
    VampireShot = 133,
    SummonToad = 134,
    PoisonShot = 135,
    CrippleShot = 136,
    SummonSnakes = 137,
    NapalmShot = 138,
    OneWithNature = 139,
    BindingShot = 140,
    MentalState = 141,

    //Custom
    Blink = 151,
    Portal = 152,
    BattleCry = 153,

    //Map Events
    DigOutZombie = 200,
    Rubble = 201,
    MapLightning = 202,
    MapLava = 203,
    MapQuake1 = 204,
    MapQuake2 = 205
}

public enum SpellEffect : byte
{
    None,
    FatalSword,
    Teleport,
    Healing,
    RedMoonEvil,
    TwinDrakeBlade,
    MagicShieldUp,
    MagicShieldDown,
    GreatFoxSpirit,
    Entrapment,
    Reflect,
    Critical,
    Mine,
    ElementalBarrierUp,
    ElementalBarrierDown,
    DelayedExplosion,
    MPEater,
    Hemorrhage,
    Bleeding,
    AwakeningSuccess,
    AwakeningFail,
    AwakeningMiss,
    AwakeningHit,
    StormEscape,
    TurtleKing,
    Behemoth,
    Stunned,
    IcePillar
}

public enum BuffType : byte
{
    None = 0,

    //magics
    TemporalFlux,
    Hiding,
    Haste,
    SwiftFeet,
    Fury,
    SoulShield,
    BlessedArmour,
    LightBody,
    UltimateEnhancer,
    ProtectionField,
    Rage,
    Curse,
    MoonLight,
    DarkBody,
    Concentration,
    VampireShot,
    PoisonShot,
    CounterAttack,
    MentalState,
    EnergyShield,
    MagicBooster,
    PetEnhancer,
    ImmortalSkin,
    MagicShield,

    //special
    GameMaster = 100,
    General,
    Exp,
    Drop,
    Gold,
    BagWeight,
    Transform,
    RelationshipEXP,
    Mentee,
    Mentor,
    Guild,
    Prison,
    Rested,

    //stats
    Impact = 200,
    Magic,
    Taoist,
    Storm,
    HealthAid,
    ManaAid,
    Defence,
    MagicDefence,
    WonderDrug,
    Knapsack
}

public enum DefenceType : byte
{
    ACAgility,
    AC,
    MACAgility,
    MAC,
    Agility,
    Repulsion,
    None
}

public enum ServerPacketIds : short
{
    Connected,
    ClientVersion,
    Disconnect,
    KeepAlive,
    NewAccount,
    ChangePassword,
    ChangePasswordBanned,
    Login,
    LoginBanned,
    LoginSuccess,
    NewCharacter,
    NewCharacterSuccess,
    DeleteCharacter,
    DeleteCharacterSuccess,
    StartGame,
    StartGameBanned,
    StartGameDelay,
    MapInformation,
    UserInformation,
    UserLocation,
    ObjectPlayer,
    ObjectRemove,
    ObjectTurn,
    ObjectWalk,
    ObjectRun,
    Chat,
    ObjectChat,
    NewItemInfo,
    NewChatItem,
    MoveItem,
    EquipItem,
    MergeItem,
    RemoveItem,
    RemoveSlotItem,
    TakeBackItem,
    StoreItem,
    SplitItem,
    SplitItem1,
    DepositRefineItem,
    RetrieveRefineItem,
    RefineCancel,
    RefineItem,
    DepositTradeItem,
    RetrieveTradeItem,
    UseItem,
    DropItem,
    PlayerUpdate,
    PlayerInspect,
    LogOutSuccess,
    LogOutFailed,
    TimeOfDay,
    ChangeAMode,
    ChangePMode,
    ObjectItem,
    ObjectGold,
    GainedItem,
    GainedGold,
    LoseGold,
    GainedCredit,
    LoseCredit,
    ObjectMonster,
    ObjectAttack,
    Struck,
    ObjectStruck,
    DamageIndicator,
    DuraChanged,
    HealthChanged,
    DeleteItem,
    Death,
    ObjectDied,
    ColourChanged,
    ObjectColourChanged,
    ObjectGuildNameChanged,
    GainExperience,
    LevelChanged,
    ObjectLeveled,
    ObjectHarvest,
    ObjectHarvested,
    ObjectNpc,
    NPCResponse,
    ObjectHide,
    ObjectShow,
    Poisoned,
    ObjectPoisoned,
    MapChanged,
    ObjectTeleportOut,
    ObjectTeleportIn,
    TeleportIn,
    NPCGoods,
    NPCSell,
    NPCRepair,
    NPCSRepair,
    NPCRefine,
    NPCCheckRefine,
    NPCCollectRefine,
    NPCReplaceWedRing,
    NPCStorage,
    SellItem,
    CraftItem,
    RepairItem,
    ItemRepaired,
    NewMagic,
    RemoveMagic,
    MagicLeveled,
    Magic,
    MagicDelay,
    MagicCast,
    ObjectMagic,
    ObjectEffect,
    RangeAttack,
    Pushed,
    ObjectPushed,
    ObjectName,
    UserStorage,
    SwitchGroup,
    DeleteGroup,
    DeleteMember,
    GroupInvite,
    AddMember,
    Revived,
    ObjectRevived,
    SpellToggle,
    ObjectHealth,
    MapEffect,
    ObjectRangeAttack,
    AddBuff,
    RemoveBuff,
    ObjectHidden,
    RefreshItem,
    ObjectSpell,
    UserDash,
    ObjectDash,
    UserDashFail,
    ObjectDashFail,
    NPCConsign,
    NPCMarket,
    NPCMarketPage,
    ConsignItem,
    MarketFail,
    MarketSuccess,
    ObjectSitDown,
    InTrapRock,
    BaseStatsInfo,
    UserName,
    ChatItemStats,
    GuildNoticeChange,
    GuildMemberChange,
    GuildStatus,
    GuildInvite,
    GuildExpGain,
    GuildNameRequest,
    GuildStorageGoldChange,
    GuildStorageItemChange,
    GuildStorageList,
    GuildRequestWar,
    DefaultNPC,
    NPCUpdate,
    NPCImageUpdate,
    MarriageRequest,
    DivorceRequest,
    MentorRequest,
    TradeRequest,
    TradeAccept,
    TradeGold,
    TradeItem,
    TradeConfirm,
    TradeCancel,
    MountUpdate,
    EquipSlotItem,
    FishingUpdate,
    ChangeQuest,
    CompleteQuest,
    ShareQuest,
    NewQuestInfo,
    GainedQuestItem,
    DeleteQuestItem,
    CancelReincarnation,
    RequestReincarnation,
    UserBackStep,
    ObjectBackStep,
    UserDashAttack,
    ObjectDashAttack,
    UserAttackMove,
    CombineItem,
    ItemUpgraded,
    SetConcentration,
    SetObjectConcentration,
    SetElemental,
    SetObjectElemental,
    RemoveDelayedExplosion,
    ObjectDeco,
    ObjectSneaking,
    ObjectLevelEffects,
    SetBindingShot,
    SendOutputMessage,
    NPCAwakening,
    NPCDisassemble,
    NPCDowngrade,
    NPCReset,
    AwakeningNeedMaterials,
    AwakeningLockedItem,
    Awakening,
    ReceiveMail,
    MailLockedItem,
    MailSendRequest,
    MailSent,
    ParcelCollected,
    MailCost,
    ResizeInventory,
    ResizeStorage,
    NewIntelligentCreature,
    UpdateIntelligentCreatureList,
    IntelligentCreatureEnableRename,
    IntelligentCreaturePickup,
    NPCPearlGoods,
    TransformUpdate,
    FriendUpdate,
    LoverUpdate,
    MentorUpdate,
    GuildBuffList,
    NPCRequestInput,
    GameShopInfo,
    GameShopStock,
    Rankings,
    Opendoor,
    GetRentedItems,
    ItemRentalRequest,
    ItemRentalFee,
    ItemRentalPeriod,
    DepositRentalItem,
    RetrieveRentalItem,
    UpdateRentalItem,
    CancelItemRental,
    ItemRentalLock,
    ItemRentalPartnerLock,
    CanConfirmItemRental,
    ConfirmItemRental,
    NewRecipeInfo,
    OpenBrowser,
    PlaySound,
    SetTimer,
    ExpireTimer
}

public enum ClientPacketIds : short
{
    ClientVersion,
    Disconnect,
    KeepAlive,
    NewAccount,
    ChangePassword,
    Login,
    NewCharacter,
    DeleteCharacter,
    StartGame,
    LogOut,
    Turn,
    Walk,
    Run,
    Chat,
    MoveItem,
    StoreItem,
    TakeBackItem,
    MergeItem,
    EquipItem,
    RemoveItem,
    RemoveSlotItem,
    SplitItem,
    UseItem,
    DropItem,
    DepositRefineItem,
    RetrieveRefineItem,
    RefineCancel,
    RefineItem,
    CheckRefine,
    ReplaceWedRing,
    DepositTradeItem,
    RetrieveTradeItem,
    DropGold,
    PickUp,
    Inspect,
    ChangeAMode,
    ChangePMode,
    ChangeTrade,
    Attack,
    RangeAttack,
    Harvest,
    CallNPC,
    BuyItem,
    SellItem,
    CraftItem,
    RepairItem,
    BuyItemBack,
    SRepairItem,
    MagicKey,
    Magic,
    SwitchGroup,
    AddMember,
    DellMember,
    GroupInvite,
    TownRevive,
    SpellToggle,
    ConsignItem,
    MarketSearch,
    MarketRefresh,
    MarketPage,
    MarketBuy,
    MarketGetBack,
    RequestUserName,
    RequestChatItem,
    EditGuildMember,
    EditGuildNotice,
    GuildInvite,
    GuildNameReturn,
    RequestGuildInfo,
    GuildStorageGoldChange,
    GuildStorageItemChange,
    GuildWarReturn,
    MarriageRequest,
    MarriageReply,
    ChangeMarriage,
    DivorceRequest,
    DivorceReply,
    AddMentor,
    MentorReply,
    AllowMentor,
    CancelMentor,
    TradeRequest,
    TradeReply,
    TradeGold,
    TradeConfirm,
    TradeCancel,
    EquipSlotItem,
    FishingCast,
    FishingChangeAutocast,
    AcceptQuest,
    FinishQuest,
    AbandonQuest,
    ShareQuest,

    AcceptReincarnation,
    CancelReincarnation,
    CombineItem,

    SetConcentration,
    AwakeningNeedMaterials,
    AwakeningLockedItem,
    Awakening,
    DisassembleItem,
    DowngradeAwakening,
    ResetAddedItem,

    SendMail,
    ReadMail,
    CollectParcel,
    DeleteMail,
    LockMail,
    MailLockedItem,
    MailCost,

    UpdateIntelligentCreature,
    IntelligentCreaturePickup,

    AddFriend,
    RemoveFriend,
    RefreshFriends,
    AddMemo,
    GuildBuffUpdate,
    NPCConfirmInput,
    GameshopBuy,

    ReportIssue,
    GetRanking,
    Opendoor,

    GetRentedItems,
    ItemRentalRequest,
    ItemRentalFee,
    ItemRentalPeriod,
    DepositRentalItem,
    RetrieveRentalItem,
    CancelItemRental,
    ItemRentalLockFee,
    ItemRentalLockItem,
    ConfirmItemRental
}

public enum ConquestType : byte
{
    Request = 0,
    Auto = 1,
    Forced = 2,
}

public enum ConquestGame : byte
{
    CapturePalace = 0,
    KingOfHill = 1,
    Random = 2,
    Classic = 3,
    ControlPoints = 4
}

[Flags]
public enum GuildRankOptions : byte
{
    CanChangeRank = 1,
    CanRecruit = 2,
    CanKick = 4,
    CanStoreItem = 8,
    CanRetrieveItem = 16,
    CanAlterAlliance = 32,
    CanChangeNotice = 64,
    CanActivateBuff = 128
}

public enum DoorState : byte
{
    Closed = 0,
    Opening = 1,
    Open = 2,
    Closing = 3
}

public enum IntelligentCreaturePickupMode : byte
{
    Automatic = 0,
    SemiAutomatic = 1,
}