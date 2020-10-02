using System;
using System.Threading;
using Polly;

namespace PollyDemo
{
    class Program
    {
        static void Main(string[] args)
        {

            {
                //fallback 回落降级
                Policy policy = Policy.Handle<Exception>()
                    .Fallback(() =>
                    {
                        Console.WriteLine("降级处理了"); 

                    });
                policy.Execute(() =>
                {
                    Console.WriteLine("开始执行");
                    throw new Exception();
                    Console.WriteLine("程序结束！");
                });

            }
            {
                Console.WriteLine("=============带有返回值");
                //带有返回值
                Policy<string> policy = Policy<string>.Handle<Exception>()
                    .Fallback(() =>
                    {
                        Console.WriteLine("程序降级处理");
                        return "返回降级的值";
                    });
                string res = policy.Execute(() =>
                {
                    Console.WriteLine("开始执行");
                    throw new Exception();
                    Console.WriteLine("程序结束！");
                    return "返回正常值";
                });
                Console.WriteLine($"返回值:{res}");
            }
            {
                Console.WriteLine("====重试 retry");
                //Policy policy = Policy.Handle<Exception>().RetryForever();
                Policy policy = Policy.Handle<Exception>().Retry(7);
                policy.Execute(() =>
                {
                    Console.WriteLine("开始计算");
                    if (DateTime.Now.Second % 10 != 0)
                    {
                        Console.WriteLine("出错!");
                        throw new Exception();
                    }

                    Console.WriteLine("执行完成");
                });
            }

            //{
            //    Console.WriteLine("================熔断器");
            //    Policy policy = Policy.Handle<Exception>()
            //        .CircuitBreaker(3, TimeSpan.FromSeconds(5));
            //    while (true)
            //    {
            //        Console.WriteLine("开始执行");
            //        try
            //        {
            //            policy.Execute(() =>
            //            {
            //                Console.WriteLine("进入Execute");
            //                throw new Exception();
            //                Console.WriteLine("结束");
            //            });
            //        }
            //        catch (Exception ex)
            //        {
            //            Console.WriteLine($"try异常:{ex}");

            //        }
            //        Thread.Sleep(500);
            //    }


            //}

            {
                Console.WriteLine("=========Wrap");
                Policy policyRetry = Policy.Handle<Exception>().Retry(3);
                Policy policyFallback = Policy.Handle<Exception>().Fallback(() =>
                {
                    Console.WriteLine("降级处理了");
                });
                Policy policy = policyFallback.Wrap(policyRetry);
                policy.Execute(() =>
                {
                    Console.WriteLine("开始计算");
                    if (DateTime.Now.Second % 10 != 0)
                    {
                        Console.WriteLine("出错!");
                        throw new Exception();
                    }

                    Console.WriteLine("执行完成");
                });

            }
        }
    }
}
