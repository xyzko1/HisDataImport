using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using HisDataImport.Quartz;
using HisModel;
using Quartz;
using Quartz.Impl;

namespace HisDataImport
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }
        Timer _timer = new Timer();
        //调度器
        IScheduler scheduler;
        //调度器工厂
        ISchedulerFactory factory;
        public void statrt(){
            string LogPath = @"C:\log\HisData\";
            if (!Directory.Exists(LogPath))//判断文件夹是否存在 
            {
                Directory.CreateDirectory(LogPath);//不存在则创建文件夹 
            }
            //创建一个调度器
            factory = new StdSchedulerFactory();
            scheduler = factory.GetScheduler();
            scheduler.Clear();
            //2、创建一个任务
            IJobDetail job = JobBuilder.Create<TimeJob>().WithIdentity("job1", "group1").Build();
            //3、创建一个触发器
            DateTimeOffset runTime = DateBuilder.EvenMinuteDate(DateTimeOffset.UtcNow);
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartNow()
                .WithCronSchedule("0 0/20 * * * ?")     //半小时执行一次
                .Build();

            //4、将任务与触发器添加到调度器中
            scheduler.ScheduleJob(job, trigger);
            //5、开始执行
            scheduler.Start();
        }
        public void TestStop() {
            if (scheduler != null)
            {
                scheduler.Shutdown(true);
            }
        }
        protected override void OnStart(string[] args)
        {
            string LogPath = @"C:\log\HisData\";
            if (!Directory.Exists(LogPath))//判断文件夹是否存在 
            {
                Directory.CreateDirectory(LogPath);//不存在则创建文件夹 
            }

            //_timer.Interval = 1000 * 10; // 十秒执行一次
            //_timer.AutoReset = true;   //执行一次 false，一直执行true                     
            //_timer.Enabled = true;
            //_timer.Start();
            //_timer.Elapsed += (sender, eventArgs) =>
            //{
            //    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(@"C:\log\HisData\log.txt", true))
            //    {
            //        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Service Start.");
            //    }
            //    HisDal hisDal = new HisDal();
            //    hisDal.ReadData();
            //};
            if (scheduler != null)
            {
                scheduler.Shutdown(true);
            }

            //创建一个调度器
            factory = new StdSchedulerFactory();
            scheduler = factory.GetScheduler();
            scheduler.Start();
            //2、创建一个任务
            IJobDetail job = JobBuilder.Create<TimeJob>().WithIdentity("job1", "group1").Build();
            //3、创建一个触发器
            //DateTimeOffset runTime = DateBuilder.EvenMinuteDate(DateTimeOffset.UtcNow);
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                 .WithCronSchedule("0 0/30 * * * ?")     //半小时执行一次
                .Build();
            //4、将任务与触发器添加到调度器中
            scheduler.ScheduleJob(job, trigger);
            //5、开始执行
            scheduler.Start();
        }

        protected override void OnStop()
        {
            //_timer.Stop();
            if (scheduler != null)
            {
                scheduler.Shutdown(true);
            }
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(@"C:\log\HisData\log.txt", true))
            {
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "=============" + " Service Stop.");
            }
        }
    }
}
