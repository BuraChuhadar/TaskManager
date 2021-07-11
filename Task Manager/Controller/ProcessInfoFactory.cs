using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Manager.ViewModels;

namespace Task_Manager.Controller
{
    class ProcessInfoFactory : IProcessInfoFactory
    {
        public IEnumerable<ProcessInfo> CreateProcessInfo(string ProcessInfos)
        {
            var result = new List<ProcessInfo>();
            var processInfoList = ProcessInfos.Split("@").Skip(1);
            foreach (var processInfoRow in processInfoList)
            {
                //Format will be provided like this:
                //{Name=WmiPrvSE; CPU=46; PID=8844; Memory(MB)=16; Memory(%)=0.05; Disk(MB)=0; Network=0}   
                var Name = processInfoRow.Split(";")[0].Split("=")[1];
                var CPU = Convert.ToInt32(processInfoRow.Split(";")[1].Split("=")[1]);
                var PID = Convert.ToInt32(processInfoRow.Split(";")[2].Split("=")[1]);
                var Memory = Convert.ToInt32(processInfoRow.Split(";")[3].Split("=")[1]);
                var MemoryPercent = Convert.ToDouble(processInfoRow.Split(";")[4].Split("=")[1]);
                var DiskMB = Convert.ToInt32(processInfoRow.Split(";")[5].Split("=")[1]);
                var Network = Convert.ToInt32(processInfoRow.Split(";")[6].Split("=")[1].TrimEnd('}'));
                yield return new ProcessInfo(Name, CPU, PID, Memory, MemoryPercent, DiskMB, Network);
            }
        }
    }
}
