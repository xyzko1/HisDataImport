using HisModel;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisDataImport.Quartz
{
    [PersistJobDataAfterExecution,DisallowConcurrentExecution]
    public class TimeJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
             
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(@"C:\log\HisData\log.txt", true))
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Service Start.");
                }
            HisDal hisDal = new HisDal();
            hisDal.ReadData();
        }
    }
}
