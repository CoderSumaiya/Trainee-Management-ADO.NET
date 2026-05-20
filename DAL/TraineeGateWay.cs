using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trainee_Management.Entities;

namespace Trainee_Management.DAL
{
    public class TraineeGateWay
    {
        string conStr = ConfigurationManager.ConnectionStrings["db"].ConnectionString;

        public DataTable GetAllCourses()
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(conStr))
            {
                string query = "SELECT * FROM Course";
                SqlCommand cmd = new SqlCommand(query, sqlcon);
                sqlcon.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

            }
            return dt;
        }
        public DataTable GetAllTrainee()
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(conStr))
            {
                string query = "SELECT TraineeId,IsEnrolled, TraineeName,MobileNo,Format(AdmissionDate,'yyyy-MM-dd') AS AdmissionDate,RegFee,ImageUrl,CASE WHEN IsEnrolled=1 THEN 'Enrolled' ELSE 'Not Enrolled' END AS TraineeStatus,c.CourseName  FROM Trainee s JOIN Course c ON s.CourseId=c.CourseId";
                SqlCommand cmd = new SqlCommand(query, sqlcon);
                sqlcon.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

            }
            return dt;
        }

        public int SaveTrainee(Trainee trainee)
        {
            int traineeId = 0;
            int moduleInserted = 0;
            using (SqlConnection sqlcon = new SqlConnection(conStr))
            {
                sqlcon.Open();
                using (SqlTransaction tran = sqlcon.BeginTransaction())
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand("INSERT INTO Trainee (ImageUrl,TraineeName,MobileNo,IsEnrolled,AdmissionDate,CourseId,RegFee) VALUES (@ImageUrl,@TraineeName,@MobileNo,@IsEnrolled,@AdmissionDate,@CourseId,@RegFee); SELECT SCOPE_IDENTITY();", sqlcon, tran);
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add("@ImageUrl", SqlDbType.VarChar).Value = trainee.ImageUrl;
                        cmd.Parameters.Add("@TraineeName", SqlDbType.VarChar).Value = trainee.TraineeName;
                        cmd.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = trainee.MobileNo;
                        cmd.Parameters.Add("@IsEnrolled", SqlDbType.Bit).Value = trainee.IsEnrolled;
                        cmd.Parameters.Add("@AdmissionDate", SqlDbType.DateTime).Value = trainee.AdmissionDate;
                        cmd.Parameters.Add("@CourseId", SqlDbType.Int).Value = trainee.CourseId;
                        cmd.Parameters.Add("RegFee", SqlDbType.Int).Value = trainee.RegFee;
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            traineeId = Convert.ToInt32(result);
                        }
                        foreach (CourseModule module in trainee.Modules)
                        {
                            SqlCommand mcmd = new SqlCommand("INSERT INTO CourseModule (TraineeId,Duration,ModuleName) VALUES (@TraineeId,@Duration,@ModuleName)", sqlcon, tran);
                            mcmd.CommandType = CommandType.Text;
                            mcmd.Parameters.Add("@TraineeId", SqlDbType.Int).Value = traineeId;
                            mcmd.Parameters.Add("@Duration", SqlDbType.Int).Value = module.Duration;
                            mcmd.Parameters.Add("@ModuleName", SqlDbType.VarChar).Value = module.ModuleName;
                            moduleInserted += mcmd.ExecuteNonQuery();
                        }
                        tran.Commit();
                        return traineeId;
                    }
                    catch (Exception ex)
                    {

                        tran.Rollback();
                        Console.WriteLine($"Error Occured: {ex.Message}");
                        throw;
                    }
                }
            }

        }

        public DataTable GetAllModuleByTraineeId(int traineeId)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(conStr))
            {
                string query = "SELECT * FROM CourseModule WHERE TraineeId='" + traineeId + "'";
                SqlCommand cmd = new SqlCommand(query, sqlcon);
                sqlcon.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

            }
            return dt;
        }

        public int DeleteModulesByTraineeAndModuleId(int traineeId, int moduleId)
        {
            int count = 0;
            using (SqlConnection sqlcon = new SqlConnection(conStr))
            {
                sqlcon.Open();
                using (SqlTransaction tran = sqlcon.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand deleteModulesCmd = new SqlCommand("DELETE FROM CourseModule WHERE TraineeId=@TraineeId AND CourseModuleId=@CourseModuleId", sqlcon, tran))
                        {
                            deleteModulesCmd.CommandType = CommandType.Text;
                            deleteModulesCmd.Parameters.Add("@TraineeId", SqlDbType.Int).Value = traineeId;
                            deleteModulesCmd.Parameters.Add("@CourseModuleId", SqlDbType.Int).Value = moduleId;
                            count = deleteModulesCmd.ExecuteNonQuery();
                        }
                        tran.Commit();
                        return count;
                    }
                    catch (Exception)
                    {
                        tran.Rollback();
                        return 0;
                    }
                }


            }
        }
        public DataTable GetTraineeBytraineeId(int traineeId)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(conStr))
            {
                string query = "SELECT TraineeId,IsEnrolled, TraineeName,MobileNo,Format(AdmissionDate,'yyyy-MM-dd') AS AdmissionDate,RegFee,ImageUrl,CASE WHEN IsEnrolled=1 THEN 'Enrolled' ELSE 'Not Enrolled' END AS TraineeStatus,c.CourseName,t.CourseId  FROM Trainee t JOIN Course c ON t.CourseId=c.CourseId WHERE TraineeId='" + traineeId + "'";
                SqlCommand cmd = new SqlCommand(query, sqlcon);
                sqlcon.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

            }
            return dt;
        }
        public int DeleteModulesByTraineeAndModuleId(int traineeId)
        {
            int count = 0;
            using (SqlConnection sqlcon = new SqlConnection(conStr))
            {
                sqlcon.Open();
                using (SqlTransaction tran = sqlcon.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand deleteModulesCmd = new SqlCommand("DELETE FROM CourseModule WHERE TraineeId=@TraineeId ", sqlcon, tran))
                        {
                            deleteModulesCmd.CommandType = CommandType.Text;
                            deleteModulesCmd.Parameters.Add("@TraineeId", SqlDbType.Int).Value = traineeId;

                            count = deleteModulesCmd.ExecuteNonQuery();
                        }
                        tran.Commit();
                        return count;
                    }
                    catch (Exception)
                    {
                        tran.Rollback();
                        return 0;
                    }
                }
            }
        }

        public int DeleteTraineeByTraineeId(int traineeId)
        {
            int count = 0;
            using (SqlConnection sqlcon = new SqlConnection(conStr))
            {
                sqlcon.Open();
                using (SqlTransaction tran = sqlcon.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand deleteStudentCmd = new SqlCommand("DELETE FROM Trainee WHERE TraineeId=@TraineeId ", sqlcon, tran))
                        {
                            deleteStudentCmd.CommandType = CommandType.Text;
                            deleteStudentCmd.Parameters.Add("@TraineeId", SqlDbType.Int).Value = traineeId;
                            count = deleteStudentCmd.ExecuteNonQuery();
                        }
                        tran.Commit();
                        return count;
                    }
                    catch (Exception)
                    {
                        tran.Rollback();
                        return 0;
                    }
                }


            }
        }
        public int UpdateTrainee(Trainee trainee)
        {
            int count = 0;
            using (SqlConnection sqlcon = new SqlConnection(conStr))
            {
                sqlcon.Open();
                using (SqlTransaction tran = sqlcon.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("UPDATE Trainee SET ImageUrl=@ImageUrl,TraineeName=@TraineeName,MobileNo=@MobileNo,IsEnrolled=@IsEnrolled,AdmissionDate=@AdmissionDate,CourseId=@CourseId,RegFee=@RegFee WHERE TraineeId=@TraineeId", sqlcon, tran))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.Add("@TraineeId", SqlDbType.Int).Value = trainee.TraineeId;
                            cmd.Parameters.Add("@ImageUrl", SqlDbType.VarChar).Value = trainee.ImageUrl;
                            cmd.Parameters.Add("@TraineeName", SqlDbType.VarChar).Value = trainee.TraineeName;
                            cmd.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = trainee.MobileNo;
                            cmd.Parameters.Add("@IsEnrolled", SqlDbType.Bit).Value = trainee.IsEnrolled;
                            cmd.Parameters.Add("@AdmissionDate", SqlDbType.DateTime).Value = trainee.AdmissionDate;
                            cmd.Parameters.Add("@CourseId", SqlDbType.Int).Value = trainee.CourseId;
                            cmd.Parameters.Add("@RegFee", SqlDbType.Int).Value = trainee.RegFee;
                            count = cmd.ExecuteNonQuery();
                        }
                        if (count > 0)
                        {
                            using (SqlCommand deleteModulesCmd = new SqlCommand("DELETE FROM CourseModule WHERE TraineeId = @TraineeId", sqlcon, tran))
                            {
                                deleteModulesCmd.CommandType = CommandType.Text;
                                deleteModulesCmd.Parameters.Add("@TraineeId", SqlDbType.Int).Value = trainee.TraineeId;
                                deleteModulesCmd.ExecuteNonQuery();
                            }

                            foreach (CourseModule module in trainee.Modules)
                            {
                                using (SqlCommand moduleCmd = new SqlCommand("INSERT INTO CourseModule (TraineeId, ModuleName,Duration) VALUES (@TraineeId, @ModuleName,@Duration)", sqlcon, tran))
                                {
                                    moduleCmd.CommandType = CommandType.Text;
                                    moduleCmd.Parameters.Add("@TraineeId", SqlDbType.Int).Value = module.TraineeId;
                                    moduleCmd.Parameters.Add("@ModuleName", SqlDbType.VarChar).Value = module.ModuleName;
                                    moduleCmd.Parameters.Add("@Duration", SqlDbType.Int).Value = module.Duration;
                                    moduleCmd.ExecuteNonQuery();
                                }
                            }
                        }

                        tran.Commit();

                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.Write(ex.Message);
                    }
                }
            }
            return count;
        }
        public DataTable GetAllTraineeInfo()
        {
            DataTable dt = new DataTable();

            using (SqlConnection sqlCon = new SqlConnection(conStr))
            {
                SqlCommand cmd = sqlCon.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT t.TraineeId,t.TraineeName,t.MobileNo,CASE WHEN t.IsEnrolled=1 THEN 'Enrolled' ELSE 'Not Enrolled' END AS IsEnrolled,FORMAT(t.AdmissionDate,'yyyy-MM-dd') AS AdmissionDate, t.ImageUrl, t.RegFee,t.CourseId, c.CourseName FROM Trainee t JOIN Course c ON t.CourseId=c.CourseId";
                sqlCon.Open();
                var rdr = cmd.ExecuteReader();
                dt.Load(rdr, LoadOption.Upsert);

            }
            return dt;
        }
        public DataTable GetAllModules()
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(conStr))
            {
                string query = "SELECT * FROM CourseModule ";
                SqlCommand cmd = new SqlCommand(query, sqlcon);
                sqlcon.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

            }
            return dt;
        }
        public DataTable GetAlltraineeInfoForReport()
        {
            DataTable dt = new DataTable();

            using (SqlConnection sqlCon = new SqlConnection(conStr))
            {
                SqlCommand cmd = sqlCon.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT t.TraineeId,t.TraineeName,t.MobileNo,CASE WHEN t.IsEnrolled=1 THEN 'Enrolled' ELSE 'Not Enrolled' END AS IsEnrolled,FORMAT(t.AdmissionDate,'yyyy-MM-dd') AS AdmissionDate, t.ImageUrl, t.RegFee,t.CourseId, c.CourseName,cm.ModuleName,cm.Duration,cm.CourseModuleId,cm.TraineeId FROM Trainee t JOIN Course c ON t.CourseId=c.CourseId JOIN CourseModule as cm ON cm.TraineeId=t.TraineeId";
                sqlCon.Open();
                var rdr = cmd.ExecuteReader();
                dt.Load(rdr, LoadOption.Upsert);

            }
            return dt;
        }

        public DataTable GetTraineeByTraineeId(int tId)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(conStr))
            {
                string query = "SELECT TraineeId,IsEnrolled,TraineeName,MobileNo,Format(AdmissionDate,'yyyy-MM-dd') AS AdmissionDate,RegFee,ImageUrl,CASE WHEN IsEnrolled=1 THEN 'Enrolled' ELSE 'Not Enrolled' END AS TraineeStatus,c.CourseName,t.CourseId  FROM Trainee t JOIN Course c ON t.CourseId=c.CourseId WHERE TraineeId='" + tId + "'";
                SqlCommand cmd = new SqlCommand(query, sqlcon);
                sqlcon.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

            }
            return dt;
        }
    }
}

   
 
