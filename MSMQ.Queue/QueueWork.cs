using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Messaging;
using System.Threading;

namespace MSMQ.Queue
{
    public class QueueWork
    {
        /**********************  队列路径语法 Begin  **************************/

        // 公用队列                       MachineName\QueueName
        // 专用队列                       MachineName\Private$\QueueName
        // 日记队列                       MachineName\QueueName\Journal$
        // 计算机日志队列                 MachineName\Journal$
        // 计算机死信队列                 MachineName\Deadletter$
        // 计算机事务性死信队列           MachineName\XactDeadletter$

        /**********************  队列路径语法 End  **************************/

        public static Queue<string> que = new Queue<string>();

        static MessageQueue queue = null;

        #region Test队列
        /// <summary>
        /// 获取队列数量
        /// </summary>
        /// <returns></returns>
        public int GetCount()
        {
            return que.Count();
        }

        /// <summary>
        /// 队列添加数据
        /// </summary>
        /// <param name="qStr">要添加的数据</param>
        public void IntoData(string qStr)
        {
            string threadId = Thread.CurrentThread.ManagedThreadId.ToString();
            que.Enqueue(qStr);
            Console.WriteLine($"队列添加数据: {qStr};当前线程id:{threadId}");
        }

        /// <summary>
        /// 队列输出数据
        /// </summary>
        /// <returns></returns>
        public string OutData()
        {
            string threadId = Thread.CurrentThread.ManagedThreadId.ToString();
            string str = que.Dequeue();
            Console.WriteLine($"队列输出数据: {str};当前线程id:{threadId}");
            return str;
        }
        #endregion

        /// <summary>
        /// 创建消息队列
        /// </summary>
        /// <param name="queuePath">队列路径</param>
        public void Createqueue(string queuePath)
        {
            try
            {
                if (MessageQueue.Exists(queuePath))
                    queue = new MessageQueue(queuePath);
                else
                    queue = MessageQueue.Create(queuePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 发送消息到队列
        /// </summary>
        /// <param name="qStr">消息</param>
        /// <param name="queuePath">队列路径</param>
        public string SendMessage(string qStr, string queuePath)
        {
            try
            {
                // string queueName = @".\Private$\msg_Rendal";        --本地队列格式
                // string queueName = "FormatName:Direct=TCP:121.0.0.1//private$//msg_rendal";     --远程队列格式
                MessageQueue myQueue = new MessageQueue(queuePath);
                Message myMessage = new Message();
                myMessage.Body = qStr;
                myMessage.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });

                myQueue.Send(myMessage);        //发生消息到队列中

                return $"消息发送成功: {qStr};当前线程id:{Thread.CurrentThread.ManagedThreadId.ToString()}";
            }
            catch (MessageQueueException ex)
            {
                return ex.Message;
            }
        }
        /// <summary>
        /// 连接消息队列读取消息
        /// </summary>
        /// <param name="queuePath"></param>
        public string ReceiveMessage(string queuePath)
        {
            MessageQueue myQueue = new MessageQueue(queuePath);
            myQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
            try
            {
                Message myMessage = myQueue.Receive(new TimeSpan(10));// myQueue.Peek();--接收后不消息从队列中移除
                myQueue.Close();

                string context = myMessage.Body.ToString();

                return $"-----------------------消息内容: {context};当前线程id:{Thread.CurrentThread.ManagedThreadId.ToString()}";
            }
            catch (MessageQueueException ex)
            {
                return ex.Message;
            }
            catch (InvalidCastException ex)
            {
                return ex.Message;
            }
            //string strRx = "";
            //do
            //{
            //    Message msgRx = new MessageQueue(queuePath).Receive();
            //    //指定格式化程序
            //    msgRx.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
            //    //接收到的内容
            //    strRx = msgRx.Body.ToString();
            //    Console.WriteLine("接受" + strRx);
            //} while (string.IsNullOrEmpty(strRx));
        }

        /// <summary>
        /// 获取指定消息队列的数量
        /// </summary>
        /// <param name="queuePath">队列路径</param>
        /// <returns></returns>
        public int GetMessageCount(string queuePath)
        {
            try
            {
                if (MessageQueue.Exists(queuePath))
                {
                    queue = new MessageQueue(queuePath);
                    if (queue != null)
                    {
                        int count = queue.GetAllMessages().Length;
                        return count;
                    }
                    return 0;
                }
                else
                    return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
    }
}
