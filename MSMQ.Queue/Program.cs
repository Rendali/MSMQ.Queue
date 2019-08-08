using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;

namespace MSMQ.Queue
{
    class Program
    {
        static void Main(string[] args)
        {
            //TaskInfo.QueueA();
            //TaskInfo.QueueB();
            
            TaskInfo.MSMQA();

            Thread.Sleep(5000);
        }
    }
}
