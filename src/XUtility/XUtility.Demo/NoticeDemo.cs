using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using XUtility.Notice;

namespace XUtility.Demo
{
    public static class NoticeDemo
    {
        public static void PublishSubscribeDemo()
        {
            var publisherName = "PublishSubscribeDemo";

            IPublisher publisher = new Publisher(publisherName, 2);

            ISubscriber subscribe1 = new Subscriber("Subscribe1", publisherName, TimeSpan.FromSeconds(10));
            ISubscriber subscribe2 = new Subscriber("Subscribe2", publisherName, TimeSpan.FromSeconds(10));

            int subscribe1NoticeCount = 0;
            int subscribe2NoticeCount = 0;

            Console.WriteLine($"Subscriber1Status={subscribe1.Status}");
            Console.WriteLine($"Subscriber2Status={subscribe2.Status}");
            Console.WriteLine("------------------------------------------------");

            subscribe1.StartWaitNotice(() =>
            {
                var sb1 = new StringBuilder();
                sb1.AppendLine($"Subscriber1Status={subscribe1.Status}");
                sb1.AppendLine("Subscribe1 recieve notice.");
                sb1.AppendLine("------------------------------------------------");
                Console.WriteLine(sb1.ToString());
                subscribe1NoticeCount++;
            }, info =>
            {
                var sb11 = new StringBuilder();
                sb11.AppendLine($"Subscriber1Status={subscribe1.Status}");
                sb11.AppendLine($"Subscribe1 completed. Exception={info.Exception?.Message}");
                sb11.AppendLine(($"Subscribe1 notice count={subscribe1NoticeCount}"));
                sb11.AppendLine("------------------------------------------------");
                Console.WriteLine(sb11.ToString());
            });

            subscribe2.StartWaitNotice(() =>
            {
                var sb2 = new StringBuilder();
                sb2.AppendLine($"Subscriber2Status = {subscribe2.Status}");
                sb2.AppendLine("Subscribe2 recieve notice.");
                sb2.AppendLine("------------------------------------------------");
                Console.WriteLine(sb2.ToString());
                subscribe2NoticeCount++;
            }, info =>
            {
                var sb22 = new StringBuilder();
                sb22.AppendLine($"Subscriber2Status = {subscribe2.Status}");
                sb22.AppendLine($"Subscribe2 completed. Exception={info.Exception?.Message}");
                sb22.AppendLine(($"Subscribe1 notice count={subscribe2NoticeCount}"));
                sb22.AppendLine("------------------------------------------------");
                Console.WriteLine(sb22.ToString());
            });

            var fullExceptionCount = 0;
            for (var i = 0; i < 10; i++)
            {
                try
                {
                    publisher.Notice();
                }
                catch (SemaphoreFullException)
                {
                    fullExceptionCount++;
                }
            }

            Console.ReadLine();
            subscribe1.UnSubscriber();
            var sb = new StringBuilder();
            sb.AppendLine($"Subscriber1Status={subscribe1.Status}");
            sb.AppendLine("Subscriber1 request unSubscribe");
            sb.AppendLine("------------------------------------------------");
            subscribe2.UnSubscriber();
            sb.AppendLine($"Subscriber1Status={subscribe2.Status}");
            sb.AppendLine("Subscriber2 request unSubscribe");
            sb.AppendLine("------------------------------------------------");
            sb.AppendLine($"FullExceptionCount={fullExceptionCount}");
            sb.AppendLine("------------------------------------------------");
            Console.WriteLine(sb.ToString());
        }

        public static void ReliablePublishSubscribeDemo()
        {
            var publisherName = "ReliablePublishSubscribeDemo";

            IPublisher publisher = new ReliablePublisher(publisherName, 1, 1);

            var fullExceptionCount = 0;
            for (var i = 0; i < 10; i++)
            {
                try
                {
                    publisher.Notice();
                }
                catch (SemaphoreFullException)
                {
                    fullExceptionCount++;
                }
            }

            ISubscriber subscribe1 = new Subscriber("Subscribe1", publisherName, TimeSpan.FromSeconds(10));
            ISubscriber subscribe2 = new Subscriber("Subscribe2", publisherName, TimeSpan.FromSeconds(10));

            int subscribe1NoticeCount = 0;
            int subscribe2NoticeCount = 0;

            Console.WriteLine($"Subscriber1Status={subscribe1.Status}");
            Console.WriteLine($"Subscriber2Status={subscribe2.Status}");
            Console.WriteLine("------------------------------------------------");

            Console.ReadLine();

            subscribe1.StartWaitNotice(() =>
            {
                var sb1 = new StringBuilder();
                sb1.AppendLine($"Subscriber1Status={subscribe1.Status}");
                sb1.AppendLine("Subscribe1 recieve notice.");
                sb1.AppendLine("------------------------------------------------");
                Console.WriteLine(sb1.ToString());
                subscribe1NoticeCount++;
                Thread.Sleep(1000);
            }, info =>
            {
                var sb11 = new StringBuilder();
                sb11.AppendLine($"Subscriber1Status={subscribe1.Status}");
                sb11.AppendLine($"Subscribe1 completed. Exception={info.Exception?.Message}");
                sb11.AppendLine(($"Subscribe1 notice count={subscribe1NoticeCount}"));
                sb11.AppendLine("------------------------------------------------");
                Console.WriteLine(sb11.ToString());
            });

            Console.ReadLine();
            subscribe1.UnSubscriber();
            var sb = new StringBuilder();
            sb.AppendLine($"Subscriber1Status={subscribe1.Status}");
            sb.AppendLine("Subscriber1 request unSubscribe");
            sb.AppendLine("------------------------------------------------");
            Console.WriteLine(sb.ToString());

            subscribe2.StartWaitNotice(() =>
            {
                var sb2 = new StringBuilder();
                sb2.AppendLine($"Subscriber2Status = {subscribe2.Status}");
                sb2.AppendLine("Subscribe2 recieve notice.");
                sb2.AppendLine("------------------------------------------------");
                Console.WriteLine(sb2.ToString());
                subscribe2NoticeCount++;
                Thread.Sleep(1500);
            }, info =>
            {
                var sb22 = new StringBuilder();
                sb22.AppendLine($"Subscriber2Status = {subscribe2.Status}");
                sb22.AppendLine($"Subscribe2 completed. Exception={info.Exception?.Message}");
                sb22.AppendLine(($"Subscribe2 notice count={subscribe2NoticeCount}"));
                sb22.AppendLine("------------------------------------------------");
                Console.WriteLine(sb22.ToString());
            });

            Console.ReadLine();
            subscribe2.UnSubscriber();
            sb = new StringBuilder();
            sb.AppendLine($"Subscriber1Status={subscribe2.Status}");
            sb.AppendLine("Subscriber2 request unSubscribe");
            sb.AppendLine("------------------------------------------------");
            sb.AppendLine($"FullExceptionCount={fullExceptionCount}");
            sb.AppendLine("------------------------------------------------");
            Console.WriteLine(sb.ToString());
        }

        public static void NoticeManagerDefaultDemo()
        {
            StringBuilder sbCompletedInfo1 = new StringBuilder();
            StringBuilder sbCompletedInfo2 = new StringBuilder();

            var count1 = 0;
            var count2 = 0;

            var manager = new NoticeManager<string>();

            var pre1 = 0;
            manager.AddSubscribe("Subscriber1", new Action<string>(message =>
            {
                var val = Convert.ToInt32(message.Split(' ')[1]);
                if (val > 0 && val - pre1 != 1)
                {
                    throw new Exception("Error");
                }
                pre1 = val;

                if (message == "Message 100")
                {
                    throw new Exception("AAA");
                }

                count1++;
                Console.WriteLine($"Subscriber1 recieve notice: {message}");
            }), new Action<string, Exception>((message, ex) =>
            {
                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine($"Subscriber1 Message={message}, Exception={ex.Message}");
                Console.WriteLine("----------------------------------------------------------");
            }));

            var pre2 = 0;
            manager.AddSubscribe("Subscriber2", new Action<string>(message =>
            {
                var val = Convert.ToInt32(message.Split(' ')[1]);
                if (val > 0 && val - pre2 != 1)
                {
                    throw new Exception("Error");
                }
                pre2 = val;

                count2++;
                Console.WriteLine($"Subscriber2 recieve notice: {message}");
            }), new Action<string, Exception>((message, ex) =>
            {
                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine($"Subscriber2 Message={message}, Exception={ex.Message}");
                Console.WriteLine("----------------------------------------------------------");
            }));

            manager.StartSubscribeNotice();

            for (var i = 0; i < 1000; i++)
            {
                manager.Publish($"Message {i}");
            }

            for (var i = 1000; i < 1020; i++)
            {
                Thread.Sleep(500);
                manager.Publish($"Message {i}");
            }

            Console.ReadLine();

            Console.WriteLine("Stop Subscribe");

            manager.StopSubscribeNotice().ContinueWith(task =>
            {
                Console.WriteLine($"Subscriber1 count={count1}");
                Console.WriteLine($"Subscriber2 count={count2}");
                Console.WriteLine($"Status={manager.Status}");
            });
        }

        public static void NoticeManagerSequenceDemo()
        {
            StringBuilder sbCompletedInfo1 = new StringBuilder();
            StringBuilder sbCompletedInfo2 = new StringBuilder();

            var count1 = 0;
            var count2 = 0;

            var manager = new NoticeManager<string>(new SequenceNoticeMode<string>());

            var pre1 = 0;
            manager.AddSubscribe("Subscriber1", new Action<string>(message =>
            {
                var val = Convert.ToInt32(message.Split(' ')[1]);
                if (val > 0 && val - pre1 != 1)
                {
                    throw new Exception("Error");
                }
                pre1 = val;

                if (message == "Message 100")
                {
                    throw new Exception("AAA");
                }

                count1++;
                Console.WriteLine($"Subscriber1 recieve notice: {message}");
            }), new Action<string, Exception>((message, ex) =>
            {
                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine($"Subscriber1 Message={message}, Exception={ex.Message}");
                Console.WriteLine("----------------------------------------------------------");
            }));

            var pre2 = 0;
            manager.AddSubscribe("Subscriber2", new Action<string>(message =>
            {
                var val = Convert.ToInt32(message.Split(' ')[1]);
                if (val > 0 && val - pre2 != 1)
                {
                    throw new Exception("Error");
                }
                pre2 = val;

                count2++;
                Console.WriteLine($"Subscriber2 recieve notice: {message}");
            }), new Action<string, Exception>((message, ex) =>
            {
                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine($"Subscriber2 Message={message}, Exception={ex.Message}");
                Console.WriteLine("----------------------------------------------------------");
            }));

            manager.StartSubscribeNotice();

            for (var i = 0; i < 1000; i++)
            {
                manager.Publish($"Message {i}");
            }

            for (var i = 1000; i < 1020; i++)
            {
                Thread.Sleep(500);
                manager.Publish($"Message {i}");
            }

            Console.ReadLine();

            Console.WriteLine("Stop Subscribe");

            manager.StopSubscribeNotice().ContinueWith(task =>
            {
                Console.WriteLine($"Subscriber1 count={count1}");
                Console.WriteLine($"Subscriber2 count={count2}");
                Console.WriteLine($"Status={manager.Status}");
            });
        }

        public static void NoticeManagerDefaultStackDemo()
        {
            StringBuilder sbCompletedInfo1 = new StringBuilder();
            StringBuilder sbCompletedInfo2 = new StringBuilder();

            var count1 = 0;
            var count2 = 0;

            var manager = new NoticeManager<string>(null, new NoticeStack<string>());

            var pre1 = 1019;
            manager.AddSubscribe("Subscriber1", new Action<string>(message =>
            {
                var val = Convert.ToInt32(message.Split(' ')[1]);
                if (val < 1019 && pre1 - val != 1)
                {
                    throw new Exception("Error");
                }
                pre1 = val;

                if (message == "Message 100")
                {
                    throw new Exception("AAA");
                }

                count1++;
                Console.WriteLine($"Subscriber1 recieve notice: {message}");
            }), new Action<string, Exception>((message, ex) =>
            {
                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine($"Subscriber1 Message={message}, Exception={ex.Message}");
                Console.WriteLine("----------------------------------------------------------");
            }));

            var pre2 = 1019;
            manager.AddSubscribe("Subscriber2", new Action<string>(message =>
            {
                var val = Convert.ToInt32(message.Split(' ')[1]);
                if (val < 1019 && pre2 - val != 1)
                {
                    throw new Exception("Error");
                }
                pre2 = val;

                count2++;
                Console.WriteLine($"Subscriber2 recieve notice: {message}");
            }), new Action<string, Exception>((message, ex) =>
            {
                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine($"Subscriber2 Message={message}, Exception={ex.Message}");
                Console.WriteLine("----------------------------------------------------------");
            }));

            for (var i = 0; i < 1020; i++)
            {
                //if (i == 500)
                //{
                //    try
                //    {
                //        manager.RemoveSubscribe("Subscriber1");
                //    }
                //    catch (Exception ex)
                //    {
                //        Console.WriteLine(ex.Message);
                //    }
                //}
                manager.Publish($"Message {i}");
            }

            manager.StartSubscribeNotice();

            Console.ReadLine();

            Console.WriteLine("Stop Subscribe");

            manager.StopSubscribeNotice().ContinueWith(task =>
            {
                Console.WriteLine($"Subscriber1 count={count1}");
                Console.WriteLine($"Subscriber2 count={count2}");
                Console.WriteLine($"Status={manager.Status}");
            });
        }

        public static void NoticeManagerSequenceStopDemo()
        {
            StringBuilder sbCompletedInfo1 = new StringBuilder();
            StringBuilder sbCompletedInfo2 = new StringBuilder();

            var count1 = 0;
            var count2 = 0;

            var manager = new NoticeManager<string>(new SequenceNoticeMode<string>());

            var pre1 = 0;
            manager.AddSubscribe("Subscriber1", new Action<string>(message =>
            {
                var val = Convert.ToInt32(message.Split(' ')[1]);
                if (val > 0 && val - pre1 != 1)
                {
                    throw new Exception("Error");
                }
                pre1 = val;

                if (message == "Message 100")
                {
                    throw new Exception("AAA");
                }

                count1++;
                Console.WriteLine($"Subscriber1 recieve notice: {message}");
            }), new Action<string, Exception>((message, ex) =>
            {
                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine($"Subscriber1 Message={message}, Exception={ex.Message}");
                Console.WriteLine("----------------------------------------------------------");
            }));

            var pre2 = 0;
            manager.AddSubscribe("Subscriber2", new Action<string>(message =>
            {
                var val = Convert.ToInt32(message.Split(' ')[1]);
                if (val > 0 && val - pre2 != 1)
                {
                    throw new Exception("Error");
                }
                pre2 = val;

                count2++;
                Console.WriteLine($"Subscriber2 recieve notice: {message}");
            }), new Action<string, Exception>((message, ex) =>
            {
                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine($"Subscriber2 Message={message}, Exception={ex.Message}");
                Console.WriteLine("----------------------------------------------------------");
            }));

            manager.StartSubscribeNotice();

            for (var i = 0; i < 1000; i++)
            {
                manager.Publish($"Message {i}");
            }

            Console.ReadLine();

            manager.StopSubscribeNotice().ContinueWith(task =>
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Subscriber1 count={count1}");
                sb.AppendLine($"Subscriber2 count={count2}");
                sb.AppendLine($"Status={manager.Status}");
                sb.AppendLine("Stop Subscribe");
                sb.AppendLine("************************************************************");
                Console.WriteLine(sb.ToString());
            });

            Console.ReadLine();

            manager.StartSubscribeNotice();

            for (var i = 1000; i < 1020; i++)
            {
                Thread.Sleep(300);
                manager.Publish($"Message {i}");
            }

            Console.ReadLine();

            manager.StopSubscribeNotice().ContinueWith(task =>
            {
                Console.WriteLine($"Subscriber1 count={count1}");
                Console.WriteLine($"Subscriber2 count={count2}");
                Console.WriteLine($"Status={manager.Status}");
            });
        }

        public static void NoticeManagerParallelStopDemo()
        {
            StringBuilder sbCompletedInfo1 = new StringBuilder();
            StringBuilder sbCompletedInfo2 = new StringBuilder();

            var count1 = 0;
            var count2 = 0;

            var manager = new NoticeManager<string>(new ParallelNoticeMode<string>());

            var pre1 = 0;
            manager.AddSubscribe("Subscriber1", new Action<string>(message =>
            {
                var val = Convert.ToInt32(message.Split(' ')[1]);
                if (val > 0 && val - pre1 != 1)
                {
                    throw new Exception("Error");
                }
                pre1 = val;

                if (message == "Message 100")
                {
                    throw new Exception("AAA");
                }

                count1++;
                Console.WriteLine($"Subscriber1 recieve notice: {message}");
            }), new Action<string, Exception>((message, ex) =>
            {
                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine($"Subscriber1 Message={message}, Exception={ex.Message}");
                Console.WriteLine("----------------------------------------------------------");
            }));

            var pre2 = 0;
            manager.AddSubscribe("Subscriber2", new Action<string>(message =>
            {
                var val = Convert.ToInt32(message.Split(' ')[1]);
                if (val > 0 && val - pre2 != 1)
                {
                    throw new Exception("Error");
                }
                pre2 = val;

                count2++;
                Console.WriteLine($"Subscriber2 recieve notice: {message}");
            }), new Action<string, Exception>((message, ex) =>
            {
                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine($"Subscriber2 Message={message}, Exception={ex.Message}");
                Console.WriteLine("----------------------------------------------------------");
            }));

            manager.StartSubscribeNotice();

            for (var i = 0; i < 1000; i++)
            {
                manager.Publish($"Message {i}");
            }

            Console.ReadLine();

            manager.StopSubscribeNotice().ContinueWith(task =>
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Subscriber1 count={count1}");
                sb.AppendLine($"Subscriber2 count={count2}");
                sb.AppendLine($"Status={manager.Status}");
                sb.AppendLine("Stop Subscribe");
                sb.AppendLine("************************************************************");
                Console.WriteLine(sb.ToString());
            });

            Console.ReadLine();

            manager.StartSubscribeNotice();

            for (var i = 1000; i < 1020; i++)
            {
                Thread.Sleep(300);
                manager.Publish($"Message {i}");
            }

            Console.ReadLine();

            manager.StopSubscribeNotice().ContinueWith(task =>
            {
                Console.WriteLine($"Subscriber1 count={count1}");
                Console.WriteLine($"Subscriber2 count={count2}");
                Console.WriteLine($"Status={manager.Status}");
            });
        }

        public static void NoticeManagerLargeNumberDemo()
        {
            Console.WriteLine("Press enter to start");
            Console.ReadLine();

            StringBuilder sbCompletedInfo1 = new StringBuilder();
            StringBuilder sbCompletedInfo2 = new StringBuilder();

            var count1 = 0;
            var count2 = 0;

            var manager = new NoticeManager<string>();

            var pre1 = 0;
            manager.AddSubscribe("Subscriber1", new Action<string>(message =>
            {
                var val = Convert.ToInt32(message.Split(' ')[1]);
                if (val > 0 && val - pre1 != 1)
                {
                    throw new Exception("Error");
                }
                pre1 = val;

                if (message == "Message 999999")
                {
                    throw new Exception("AAA");
                }

                count1++;
                Console.WriteLine($"Subscriber1 recieve notice: {message}");
            }), new Action<string, Exception>((message, ex) =>
            {
                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine($"Subscriber1 Message={message}, Exception={ex.Message}");
                Console.WriteLine("----------------------------------------------------------");
            }));

            var pre2 = 0;
            manager.AddSubscribe("Subscriber2", new Action<string>(message =>
            {
                var val = Convert.ToInt32(message.Split(' ')[1]);
                if (val > 0 && val - pre2 != 1)
                {
                    throw new Exception("Error");
                }
                pre2 = val;

                count2++;
                Console.WriteLine($"Subscriber2 recieve notice: {message}");
            }), new Action<string, Exception>((message, ex) =>
            {
                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine($"Subscriber2 Message={message}, Exception={ex.Message}");
                Console.WriteLine("----------------------------------------------------------");
            }));

            manager.StartSubscribeNotice();

            for (var i = 0; i < 1000000; i++)
            {
                manager.Publish($"Message {i}");
            }

            for (var i = 1000000; i < 1000020; i++)
            {
                Thread.Sleep(500);
                manager.Publish($"Message {i}");
            }

            Console.ReadLine();

            Console.WriteLine("Stop Subscribe");

            manager.StopSubscribeNotice().ContinueWith(task =>
            {
                Console.WriteLine($"Subscriber1 count={count1}");
                Console.WriteLine($"Subscriber2 count={count2}");
                Console.WriteLine($"Status={manager.Status}");
            });
        }
    }
}
