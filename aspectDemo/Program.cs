using System;
using AspectCore.DynamicProxy;

namespace aspectDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            ProxyGeneratorBuilder builder=new ProxyGeneratorBuilder();
            using (IProxyGenerator generator=builder.Build())
            {
                Person p = generator.CreateClassProxy<Person>();
                p.Say("rock");
            }

            Console.ReadKey();
        }
    }
}
