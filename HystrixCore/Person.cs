using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using RuPeng.HystrixCore;

namespace HystrixCore
{
   public class Person
    {
        [HystrixCommand(nameof(HelloFallBackAsync))]
        public virtual async Task<String> HelloAsync(string name)
        {
            Console.WriteLine($"hello{name}");
            string s = null;
            s.ToString();
            return "OK";

        }
        [HystrixCommand(nameof(Hello2FallBackAsync))]
        public virtual async Task<String> HelloFallBackAsync(string name)
        {
            Console.WriteLine($"hell 降级1{name}");
            string s = null;
            s.ToString();
            return "fail_1";

        }
        public virtual async Task<String> Hello2FallBackAsync(string name)
        {
            Console.WriteLine($"hell 降级2{name}");
        
            return "fail_2";
        }
        [HystrixCommand(nameof(AddFall))]
        public virtual int Add(int i, int j)
        {
            string s = null;
            s.ToString();
            return i + j;
        }

        public virtual int AddFall(int i, int j)
        {
            Console.WriteLine("降级AddFall");
            return i + j;
        }
    }
}
