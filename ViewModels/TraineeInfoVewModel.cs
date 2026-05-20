using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trainee_Management.ViewModels
{
  public  class TraineeInfoVewModel
    {
        public int TraineeId { get; set; }
        public string TraineeName { get; set; }
        public string MobileNo { get; set; }
        public string IsEnrolled { get; set; }
        public DateTime AdmissionDate { get; set; }
        public string ImageUrl { get; set; }
        public byte[] ImageBinary { get; set; }
        public int CourseId { get; set; }
        public int RegFee { get; set; }
        public string CourseName { get; set; }
        public int CourseModuleId { get; set; }
        public string ModuleName { get; set; }
        public int Duration { get; set; }

    }
}
