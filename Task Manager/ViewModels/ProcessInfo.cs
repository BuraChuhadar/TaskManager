using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Manager.ViewModels
{
    class ProcessInfo
    {
        //Name, CPU, PID, "Memory(MB)", "Memory(%)", "Disk(MB)", "Network"

        public string Name { get; set; }
        public int CPU { get; set; }
        public int PID { get; set; }
        public int MemoryMB { get; set; }
        public double MemoryPercent { get; set; }
        public int DiskMB { get; set; }
        public int Network { get; set; }

        public ProcessInfo(string name, int CPU, int PID, int MemoryMB, double MemoryPercent, int DiskMB, int Network)
        {
            this.Name = name;
            this.CPU = CPU;
            this.PID = PID;
            this.MemoryMB = MemoryMB;
            this.MemoryPercent = MemoryPercent;
            this.DiskMB = DiskMB;
            this.Network = Network;
        }
    }
}
