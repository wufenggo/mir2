using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Collections;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Data.SQLite;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Drawing;
using System.Collections.Specialized;











//快速随机函数的工具而已嘛，需要搞什么线程么。
//不用线程产生的随机数竟然有问题。坑爹啊,还是用线程变量吧
public class RandomUtils
{
    private static int seed = Environment.TickCount;
    private static ThreadLocal<Random> RandomWrapper = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));
    //private static Random random = new Random();


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


}