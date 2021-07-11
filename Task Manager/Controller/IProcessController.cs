using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Manager.ViewModels;

namespace Task_Manager.Controller
{
    interface IProcessController
    {
        IEnumerable<ProcessInfo> ParceProcessInfos(string ProcessInfos);
    }
}
