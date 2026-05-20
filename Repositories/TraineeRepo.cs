using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trainee_Management.DAL;
using Trainee_Management.Entities;

namespace Trainee_Management.Repositories
{
 public class TraineeRepo
    {
        TraineeGateWay dal = new TraineeGateWay();
        public DataTable GetAllCourses()
        {
            DataTable dt = new DataTable();
            dt = dal.GetAllCourses();
            return dt;
        }
        public DataTable GetAllTrainee()
        {
            DataTable dt = new DataTable();
            dt = dal.GetAllTrainee();
            return dt;
        }
        public int SaveTrainee(Trainee trainee)
        {
            int saveCount = dal.SaveTrainee(trainee);
            return saveCount;
        }

        public DataTable GetModulesByTraineeId(int traineeId)
        {
            DataTable dt = dal.GetAllModuleByTraineeId(traineeId);
            return dt;
        }
        public int DeleteModuleByTraineeId(int traineeId, int moduleId)
        {
            int deleteResult = dal.DeleteModulesByTraineeAndModuleId(traineeId, moduleId);
            return deleteResult;
        }
        public DataTable GetTraineeById(int tId)
        {
            DataTable dt = new DataTable();
            dt = dal.GetTraineeByTraineeId(tId);
            return dt;
        }
        public int DeleteTrainee(int traineeId)
        {
            int count = dal.DeleteTraineeByTraineeId(traineeId);
            return count;

        }
        public int DeleteModuleByTraineeId(int traineeId)
        {
            int deleteResult = dal.DeleteModulesByTraineeAndModuleId( traineeId);
            return deleteResult;
        }
        public int UpdateTrainee(Trainee trainee)
        {
            int updateCount = dal.UpdateTrainee(trainee);
            return updateCount;
        }
        public DataTable GetAllTraineeInfo()
        {
            DataTable dt = new DataTable();
            dt = dal.GetAllTraineeInfo();
            return dt;
        }
        public DataTable GetAllModules()
        {
            DataTable dt = dal.GetAllModules();
            return dt;
        }
        public DataTable GetAllTraineeInfoForReport()
        {
            DataTable dt = new DataTable();
            dt = dal.GetAlltraineeInfoForReport();
            return dt;
        }


    }
}
