using System;
using System.Threading;

namespace Server.Library.MirEnvir
{
    public class RandomUtils
    {
        private static int seed = Environment.TickCount;
        private static ThreadLocal<Random> RandomWrapper = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));
        //private static Random random = new Random();
        private static string[] ChineseTerms = { "夜火", "传奇", "怪物", "野猪", "老虎", "攻击", "打架", "屠夫", "鞋子", "老婆", "老板", "女人", "情人", "情妇", "大方", "总裁", "工会", "行会", "跑路", "农民", "吃货", "笨蛋", "爸爸", "杀怪", "傻瓜", "高手", "无敌", "神仙", "神奇", "权利", "天才", "男人", "乌龟", "牛逼", "游戏", "地狱", "天堂", "轮回", "灵性", "品质", "魔法", "战士", "法师", "射手", "刺客", "裁决", "龙纹", "骨玉", "力量", "戒指", "头盔", "衣服", "鞋子", "手镯", "腰带" };

        //100个汉语常用字
        private static string ChineseChar = "的一国在人了有中是年和大业不为发会工经上地市要个产这出行作生家以成到日民来我部对进多全建他公开们场展时理新方主企资实学报制政济用同于法高长现本月定化加动合品重关机分力自外者区能设后就等体下万元社过前面夜火传奇白野猪怪物";

        public static int Next()
        {
            return RandomWrapper.Value.Next();
        }
        public static int Next(int maxValue)
        {
            return RandomWrapper.Value.Next(maxValue);
        }
        public static int Next(int minValue, int maxValue)
        {
            return RandomWrapper.Value.Next(minValue, maxValue);
        }
        public static double NextDouble()
        {
            return RandomWrapper.Value.NextDouble();
        }

        //最大增加数，增加几率1/x
        //装备极品是用这个做的做的，所以加1点属性是
        public static int RandomomRange(int count, int rate)
        {
            int x = 0;
            for (int i = 0; i < count; i++) if (Next(rate) == 0) x++;
            return x;
        }

        public static string RandomomRangeChineseTerm()
        {
            if (Next(100) < 30)
            {
                return ChineseTerms[Next(ChineseTerms.Length)];
            }
            else
            {
                return ChineseChar.Substring(Next(ChineseChar.Length), 1) + ChineseChar.Substring(Next(ChineseChar.Length), 1);
            }
        }



    }
}
