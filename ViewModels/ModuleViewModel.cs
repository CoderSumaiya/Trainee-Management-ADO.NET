using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trainee_Management.ViewModels
{
    public class ModuleViewModel
    {
        public int CourseModuleId { get; set; }
        public string ModuleName { get; set; }
        public int Duration { get; set; }
        public int TraineeId { get; set; }
    }
}
