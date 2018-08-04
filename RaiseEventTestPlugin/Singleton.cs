using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPlugin
{
    class Singleton<T> where T : class , new()
    {
        static T s_instance;

        public static T GetInstance()
        {
            if(s_instance == null)
            {
                s_instance = new T();
            }
            return s_instance;
        }

        protected Singleton()
        {

        }
    }
}
