using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjTest
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> lists= CommonHelper.SendMessage<List<string>>("http://192.168.31.134:8088/Api/AdRtx/");
            foreach(string list in lists)
            {
                Console.WriteLine(list);
            }
            
            Console.ReadKey();
        }
    }
}
