using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Manager.ViewModels;

namespace Task_Manager.Controller
{
    class ProcessController: IProcessController
    {
        private readonly IProcessInfoFactory processInfoFactory;
        public ProcessController(IProcessInfoFactory processInfoFactory)
        {
            this.processInfoFactory = processInfoFactory;
        }

        public IEnumerable<ProcessInfo> ParceProcessInfos(string ProcessInfos)
        {
            return processInfoFactory.CreateProcessInfo(ProcessInfos).ToList();
        }
    }
}
