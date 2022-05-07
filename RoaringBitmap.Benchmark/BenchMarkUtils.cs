using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ConsoleTableExt;

namespace RoaringBitmap.Benchmark
{
    /// <summary>
    /// 高性能测试类，方便快速启动测试
    /// </summary>
    public class BenchMarkUtils
    {
        //public static DataTable Run(string path = "HPR.ToUInt32Bench")
        //{
        //    var asmbs = AppDomain.CurrentDomain.GetAssemblies();
        //    foreach (Assembly asm in asmbs)
        //    {
        //        if (asm.GetName().Name != "HPR")
        //            continue;
        //        var type = asm.GetType(path);
        //        var table = Run(type);
        //        return table;
        //    }
        //    return null;
        //}

        ///// <summary>
        ///// 获得所有的测试集合
        ///// </summary>
        ///// <returns></returns>
        //public static List<(string name, string intro, string[] methods)> GetBenchs()
        //{
        //    List<(string name, string intro, string[] methods)> list = new();
        //    var asmbs = AppDomain.CurrentDomain.GetAssemblies();
        //    foreach (Assembly asm in asmbs)
        //    {
        //        if (asm.GetName().Name != "HPR")
        //            continue;
        //        foreach (var type in asm.DefinedTypes)
        //        {
        //            if (IsTypeDecoratedByAttribute<SimpleJobAttribute>(type.GetCustomAttributes(false)))
        //            {
        //                var methods = type.DeclaredMethods.Select(x => x.Name);
        //                methods = methods.Except(new string[] { "Setup" });
        //                var intros = type.GetCustomAttributes<IntroAttribute>(false);
        //                if (intros?.Any() == true)
        //                    list.Add((type.Name, intros.First().Intro, methods.ToArray()));
        //                else
        //                    list.Add((type.Name, "", methods.ToArray()));
        //            }
        //        }
        //    }
        //    return list;
        //}

        internal static bool IsTypeDecoratedByAttribute<T>(object[] t)
        {
            foreach (Attribute attr in t)
            {
                if (attr is T)
                {
                    //T a = (T)Convert.ChangeType(attr, typeof(T));
                    return true;
                }
            }
            return false;
        }

        public static DataTable Run<T>()
        {
            var type = typeof(T);
            return Run(type);
        }

        public static DataTable Run(Type type)
        {
            var w = Stopwatch.StartNew();
            var o = Activator.CreateInstance(type);
            var setup = type.GetMember("Setup")?.FirstOrDefault() as MethodInfo;
            var classAtts = type.GetCustomAttributes(typeof(SimpleJobAttribute), true);
            var warmupCount = 1;
            var iterationCount = 1;
            if (classAtts.Length > 0 && classAtts[0] is SimpleJobAttribute job)
            {
                warmupCount = job.Config.GetJobs().First().Job.Run.WarmupCount;
                iterationCount = job.Config.GetJobs().First().Job.Run.IterationCount;
                //Console.WriteLine(job.Config.GetJobs().First().Job.Run.RunStrategy);
                //Console.WriteLine(job.Config.GetJobs().First().Job.Run.WarmupCount);
                //Console.WriteLine(job.Config.GetJobs().First().Job.Run.LaunchCount);
                //Console.WriteLine(job.Config.GetJobs().First().Job.Run.IterationCount);
                //Console.WriteLine(job.Config.GetJobs().First().Job.Run.InvocationCount);
            }

            w = Stopwatch.StartNew();
            setup?.Invoke(o, null);
            Console.WriteLine($"Setup Cost:{w.ElapsedMilliseconds}");


            var table = new DataTable();
            //|   Method |     Value |          N |     Mean | Error | Ratio | Rank |
            table.Columns.Add("Method", typeof(string));
            table.Columns.Add("Mean", typeof(string));
            table.Columns.Add("Rank", typeof(string));
            table.Columns.Add("Ratio", typeof(string));

            var fields = type.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField);
            var methods = type.GetMembers(BindingFlags.Public | BindingFlags.Instance);
            foreach (var item in fields)
            {
                var atts = item.GetCustomAttributes(typeof(ParamsAttribute), true);
                if (atts.Length > 0 && atts[0] is ParamsAttribute p)
                {
                    table.Columns.Add(item.Name, typeof(string));
                    foreach (var v in p.Values)
                    {
                        var list = new List<Ranks>();
                        ((FieldInfo)item).SetValue(o, v);

                        //进行预热，不用于计时
                        for (int i = 0; i < warmupCount; i++)
                            Exec(methods, o, null, 1, new Tuple<string, object>(item.Name, v));

                        Exec(methods, o, list, iterationCount, new Tuple<string, object>(item.Name, v));
                        //for (int i = 0; i < iterationCount; i++)
                        //    Exec(methods, o, list, new Tuple<string, object>(item.Name, v));

                        //var mean = list.Where(item => item.IsBaseLine == true).Average(item => item.Mean);


                        var row = list.FirstOrDefault(item => item.IsBaseLine == true);
                        if (row == null)
                            row = list[0];
                        foreach (var r in list)
                            r.Ratio = ((float)r.Mean / (float)row.Mean);
                        list = list.OrderBy(p => p.Mean).ToList();
                        var num = 1;
                        foreach (var r in list)
                            r.Rank = num++;
                        list = list.OrderBy(p => p.Id).ToList();
                        foreach (var r in list)
                            table.Rows.Add(r.Method, r.Mean.ToString(), r.Rank.ToString(), r.Ratio.ToString("f2"), v);
                        table.Rows.Add("", "", "", "", "");
                    }
                }
                //无参数的处理情况
                //else
                //{
                //    var list = new List<Ranks>();
                //    //进行预热，不用于计时
                //    for (int i = 0; i < warmupCount; i++)
                //        Exec(methods, o, null, 1, new Tuple<string, object>(item.Name, null));

                //    Exec(methods, o, list, iterationCount, new Tuple<string, object>(item.Name, null));
                //    //for (int i = 0; i < iterationCount; i++)
                //    //    Exec(methods, o, list, new Tuple<string, object>(item.Name, v));

                //    //var mean = list.Where(item => item.IsBaseLine == true).Average(item => item.Mean);


                //    var row = list.FirstOrDefault(item => item.IsBaseLine == true);
                //    if (row == null)
                //        row = list[0];
                //    foreach (var r in list)
                //        r.Ratio = ((float)r.Mean / (float)row.Mean);
                //    list = list.OrderBy(p => p.Mean).ToList();
                //    var num = 1;
                //    foreach (var r in list)
                //        r.Rank = num++;
                //    list = list.OrderBy(p => p.Id).ToList();
                //    foreach (var r in list)
                //        table.Rows.Add(r.Method, r.Mean.ToString(), r.Rank.ToString(), r.Ratio.ToString("f2"));
                //    table.Rows.Add("", "", "", "");
                //}
            }

            if (table.Rows.Count > 0)
                table.Rows.RemoveAt(table.Rows.Count - 1);
            var output = ConsoleTableBuilder
                        .From(table)
                        .WithTitle($"{type.Name} Benchmark Params")
                        .WithPaddingLeft(string.Empty)
                        .WithTextAlignment(new Dictionary<int, TextAligntment>() { { 0, TextAligntment.Right } })
                        .WithFormat(ConsoleTableBuilderFormat.MarkDown)
                        .Export();
            Console.WriteLine(output);

            return table;
        }

        public static void Exec(MemberInfo[] methods, object o, List<Ranks> list, int iterationCount = 1, params Tuple<string, object>[] tuples)
        {
            foreach (var item in methods)
            {
                var atts = item.GetCustomAttributes(typeof(BenchmarkAttribute), true);
                if (atts.Length > 0)
                {
                    var method = (MethodInfo)item;
                    var w = Stopwatch.StartNew();
                    for (int i = 0; i < iterationCount; i++)
                        method.Invoke(o, null);
                    var baseline = ((BenchmarkAttribute)atts[0]).Baseline;
                    list?.Add(new Ranks
                    {
                        Id = list.Count,
                        IsBaseLine = baseline,
                        Method = method.Name,
                        Mean = (int)w.ElapsedMilliseconds,
                    });
                }
            }
        }

        public class Ranks
        {
            public int Id;
            public bool IsBaseLine;
            public string Method;
            public int Mean;
            public int Rank;
            public float Ratio;
            public List<object> Params;
        }


    }
}
