using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trainee_Management.Entities
{
   public  class Trainee
    {
        public int TraineeId { get; set; }
        public string TraineeName { get; set; }
        public string MobileNo { get; set; }
        public bool IsEnrolled { get; set; }
        public DateTime AdmissionDate { get; set; }
        public string ImageUrl { get; set; }
        public int CourseId { get; set; }
        public int RegFee { get; set; }

        public List<CourseModule> Modules { get; set; } = new List<CourseModule>();
    }
}
