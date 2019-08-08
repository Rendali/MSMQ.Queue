using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;

namespace MSMQ.Queue
{
    public static class TaskInfo
    {
        #region 队列的操作模拟
        public static async void QueueA()
        {
            QueueWork queueTest = new QueueWork();
            var task = Task.Run(() =>
            {
                for (int i = 0; i < 20; i++)
                {
                    queueTest.IntoData("Queue-A" + i);
                }
            });
            await task;

            Console.WriteLine("**************************"+task.IsCompleted);

            Console.WriteLine("Queue-A插入完成,进行输出:");

            while (queueTest.GetCount() > 0)
            {
                queueTest.OutData();
            }
        }

        public static async void QueueB()
        {
            QueueWork queueTest = new QueueWork();
            var task = Task.Run(() =>
            {
                for (int i = 0; i < 20; i++)
                {
                    queueTest.IntoData("Queue-B" + i);
                }
            });
            await task;
            Console.WriteLine("Queue-B插入完成,进行输出:");

            while (queueTest.GetCount() > 0)
            {
                queueTest.OutData();
            }
        }
        #endregion

        static QueueWork queueWork = null;
        static string queuePath = ConfigurationSettings.AppSettings["TCPqueuePath"].ToString();
        public static async void MSMQA()
        {
            queueWork = new QueueWork();
            var task = Task.Run(() =>
            {
                for (int i = 0; i < 20; i++)
                {
                    //queueWork.Createqueue(queuePath);
                    string sendMsg = queueWork.SendMessage($"MSMQ-A{i}", queuePath);
                    Console.WriteLine(sendMsg);
                }
            });
            await task;
            Console.WriteLine("MSMQA发送完成,进行读取:");
            while (queueWork.GetMessageCount(queuePath) > 0)
            {
                string receiveMsg = queueWork.ReceiveMessage(queuePath);
                Console.WriteLine(receiveMsg);
            }
        }
    }
}
