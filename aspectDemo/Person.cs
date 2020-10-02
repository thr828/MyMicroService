using System;
using System.Collections.Generic;
using System.Text;

namespace aspectDemo
{
   public class Person
    {
        [CustomInject]
        public virtual void Say(string name)
        {
            Console.WriteLine($"大家好，我是{name}");
        }
    }
}
