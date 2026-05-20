using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Trainee_Management.Entities;
using Trainee_Management.Repositories;
using Trainee_Management.RptViewers;
using Trainee_Management.ViewModels;


namespace Trainee_Management
{
    public partial class Form1 : Form
    {
        private readonly OpenFileDialog ofd = new OpenFileDialog();
        private bool isDefaultImage = true;
        private string previousImage = "";
        private int intTraineeId = 0;

        private Trainee trainee = new Trainee();
        private readonly TraineeRepo repo = new TraineeRepo();
        private List<CourseModule> courseModules = new List<CourseModule>();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadCourseCombo();
            LoadTraineeGrid();
            ClearAll();

        }

        private void ClearAll()
        {
            string noImagePath = Path.Combine(System.Windows.Forms.Application.StartupPath, "images", "noimage.png");
            if (File.Exists(noImagePath))
            {
                using (var stream = new FileStream(noImagePath, FileMode.Open, FileAccess.Read))
                {
                    pbUpload.Image = Image.FromStream(stream);
                }
            }

            isDefaultImage = true;
            previousImage = "";
            chkEnrolled.Checked = false;
            txtName.Clear();
            txtMobile.Clear();
            txtRegFee.Clear();
            dgvNewModule.DataSource = null;
            dgvModules.DataSource = null;
            btnSave.Text = "Save";
            intTraineeId = 0;
            trainee = new Trainee();
        }

        private void LoadCourseCombo()
        {
            DataTable dt = repo.GetAllCourses();
            DataRow topRow = dt.NewRow();
            topRow[0] = 0;
            topRow[1] = "--Select Course--";
            dt.Rows.InsertAt(topRow, 0);

            cmbCourse.DataSource = dt;
            cmbCourse.DisplayMember = "CourseName";
            cmbCourse.ValueMember = "CourseId";
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            ofd.Filter = "Images(.jpg,.png)|*.jpg;*.png";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                using (var stream = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read))
                {
                    pbUpload.Image = Image.FromStream(stream);
                }
                isDefaultImage = false;
                previousImage = "";
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            using (var stream = new FileStream(Application.StartupPath + "\\images\\noimage.png", FileMode.Open, FileAccess.Read))
            {
                pbUpload.Image = Image.FromStream(stream);
            }
            isDefaultImage = true;
            previousImage = "";

        }
        public bool IsValidated()
        {
            return !string.IsNullOrWhiteSpace(txtName.Text) &&
                   cmbCourse.SelectedIndex > 0 &&
                   !string.IsNullOrWhiteSpace(txtMobile.Text);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!IsValidated())
            {
                MessageBox.Show("Provide correct information", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                trainee.TraineeId = intTraineeId;
                trainee.TraineeName = txtName.Text;
                trainee.MobileNo = txtMobile.Text;
                trainee.IsEnrolled = chkEnrolled.Checked;
                trainee.AdmissionDate = dtpAdmissionDate.Value;
                trainee.CourseId = Convert.ToInt32(cmbCourse.SelectedValue);
                trainee.RegFee = Convert.ToInt32(txtRegFee.Text);

                if (isDefaultImage)
                {
                    trainee.ImageUrl = "noimage.png";
                }
                else if (!string.IsNullOrEmpty(ofd.FileName))
                {
                   

                    if (intTraineeId > 0 && previousImage != "" && previousImage != "noimage.png")
                    {
                        DeleteImageFile(previousImage);
                    }
                    trainee.ImageUrl = SaveImage(ofd.FileName);
                }

                if (intTraineeId == 0)
                {
                    if (repo.SaveTrainee(trainee) > 0)
                    {
                        MessageBox.Show("Saved Successfully");
                    }
                }
                else
                {
                    if (repo.UpdateTrainee(trainee) > 0)
                    {
                        MessageBox.Show("Updated Successfully");
                    }
                }

                LoadTraineeGrid();
                ClearAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadTraineeGrid()
        {
            DataTable dt = repo.GetAllTrainee();
            if (!dt.Columns.Contains("Image"))
                dt.Columns.Add("Image", typeof(byte[]));

            foreach (DataRow dr in dt.Rows)
            {
                string imgName = dr["ImageUrl"].ToString();
                string imagePath = Path.Combine(System.Windows.Forms.Application.StartupPath, "images", imgName);
                string defaultPath = Path.Combine(System.Windows.Forms.Application.StartupPath, "images", "noimage.png");

                try
                {
                    dr["Image"] = File.Exists(imagePath) ? File.ReadAllBytes(imagePath) : File.ReadAllBytes(defaultPath);
                }
                catch { dr["Image"] = null; }
            }

            dgvTrainee.DataSource = null;
            dgvTrainee.Columns.Clear();
            dgvTrainee.DataSource = dt;

            
            AddGridButton("Details", "Details");
            AddGridButton("Edit", "Edit");
            AddGridButton("Delete", "Delete");

           

            dgvTrainee.Columns["Details"].DisplayIndex = 0;
            dgvTrainee.Columns["Edit"].DisplayIndex = 1;
            dgvTrainee.Columns["Delete"].DisplayIndex = 2;

            
            dgvTrainee.RowTemplate.Height = 80;

            if (dgvTrainee.Columns["Image"] is DataGridViewImageColumn imgCol)
            {
                imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
                imgCol.DisplayIndex = 3; 
            }

            if (dgvTrainee.Columns.Contains("TraineeId")) dgvTrainee.Columns["TraineeId"].Visible = false;
            if (dgvTrainee.Columns.Contains("ImageUrl")) dgvTrainee.Columns["ImageUrl"].Visible = false;
        }

        private void AddGridButton(string name, string text)
        {
            DataGridViewButtonColumn btn = new DataGridViewButtonColumn
            {
                Name = name,
                Text = text,
                HeaderText = text,
                UseColumnTextForButtonValue = true
            };
            dgvTrainee.Columns.Add(btn);
        }
        private string SaveImage(string imgPath)
        {
            string fileName = Path.GetFileNameWithoutExtension(imgPath);
            string ext = Path.GetExtension(imgPath);
            fileName = fileName.Length <= 15 ? fileName : fileName.Substring(0, 15);
            fileName = fileName + DateTime.Now.ToString("yymmssfff") + ext;
            string directoryPath = Path.Combine(Application.StartupPath, "images");
        
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string fullSavePath = Path.Combine(directoryPath, fileName);
            using (Bitmap bmp = new Bitmap(pbUpload.Image))
            {
                bmp.Save(fullSavePath);
            }
            return fileName;
        }

        private void btnNewModuleSave_Click(object sender, EventArgs e)
        {
            CourseModule module = new CourseModule
            {
                ModuleName = txtModuleTitle.Text,
                Duration = Convert.ToInt32(txtDuration.Text)
            };
            trainee.Modules.Add(module);
            LoadModuleGrid(trainee.Modules, 0);
        }

        private void LoadModuleGrid(List<CourseModule> modules, int traineeId)
        {
            dgvNewModule.DataSource = null;
            dgvNewModule.Columns.Clear();

            if (traineeId == 0)
                dgvNewModule.DataSource = ConvertToDataTable(modules);
            else
                dgvNewModule.DataSource = repo.GetModulesByTraineeId(traineeId);

            AddGridButtonToControl(dgvNewModule, "Delete", "Delete");
            ClearModuleInputs();
        }

        private void AddGridButtonToControl(DataGridView grid, string name, string text)
        {
            DataGridViewButtonColumn btn = new DataGridViewButtonColumn
            {
                Name = name,
                Text = text,
                HeaderText = text,
                Width = 60,
                UseColumnTextForButtonValue = true
            };
            grid.Columns.Add(btn);
        }

        private void ClearModuleInputs()
        {
            txtModuleTitle.Clear();
            txtDuration.Text = "0";
        }


        private object ConvertToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props) dataTable.Columns.Add(prop.Name);

            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++) values[i] = Props[i].GetValue(item, null);
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }

        private void dgvNewModule_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dgvNewModule.Columns[e.ColumnIndex].Name == "Delete")
            {
                if (MessageBox.Show("Delete this module?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        int tId = Convert.ToInt32(dgvNewModule.Rows[e.RowIndex].Cells["TraineeId"].Value);
                        int mId = Convert.ToInt32(dgvNewModule.Rows[e.RowIndex].Cells["CourseModuleId"].Value);

                        if (tId > 0 && mId > 0)
                        {
                            repo.DeleteModuleByTraineeId(tId, mId);
                        }
                    }
                    catch { }

                    if (trainee.Modules.Count > e.RowIndex) trainee.Modules.RemoveAt(e.RowIndex);
                    LoadModuleGrid(trainee.Modules, 0);
                }
            }
        }

        private void dgvTrainee_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            try
            {
                int tId = Convert.ToInt32(dgvTrainee.Rows[e.RowIndex].Cells["TraineeId"].Value);
                string command = dgvTrainee.Columns[e.ColumnIndex].Name;
                switch (command)
                {
                    case "Details":
                        dgvModules.DataSource = repo.GetModulesByTraineeId(tId);
                        break;

                    case "Edit":
                        EditTraineeInfo(tId);
                        break;

                    case "Delete":
                        if (MessageBox.Show("Are you sure you want to delete this student and their modules?",
                                            "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        {
                            DeleteTraineeInfo(tId);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while processing the request: {ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void DeleteTraineeInfo(int traineeId)
        {
            DataTable dt = repo.GetTraineeById(traineeId);
            if (dt.Rows.Count > 0) DeleteImageFile(dt.Rows[0]["ImageUrl"].ToString());

            repo.DeleteModuleByTraineeId(traineeId);
            repo.DeleteTrainee(traineeId);

            LoadTraineeGrid();
            ClearAll();
        }

        private void DeleteImageFile(string fileName)
        {
            try
            {
                string path = Path.Combine(Application.StartupPath, "images", fileName);
                if (File.Exists(path) && fileName != "noimage.png")
                {
                    pbUpload.Image?.Dispose();
                    pbUpload.Image = null;
                    File.Delete(path);
                }
            }
            catch (Exception ex) { MessageBox.Show("Image Delete Error: " + ex.Message); }
        }

        private void EditTraineeInfo(int tId)
        {
            DataTable dt = repo.GetTraineeById(tId);
            if (dt == null || dt.Rows.Count == 0) return;

            btnSave.Text = "Update";
            intTraineeId = tId;
            DataRow row = dt.Rows[0];

            txtName.Text = row["TraineeName"].ToString();
            cmbCourse.SelectedValue = row["CourseId"];
            txtMobile.Text = row["MobileNo"].ToString();
            dtpAdmissionDate.Value = Convert.ToDateTime(row["AdmissionDate"]);
            chkEnrolled.Checked = row["IsEnrolled"] != DBNull.Value && (bool)row["IsEnrolled"];
            txtRegFee.Text = row["RegFee"].ToString();

            previousImage = row["ImageUrl"].ToString();
            string imgPath = Path.Combine(Application.StartupPath, "images", previousImage);

            if (File.Exists(imgPath))
            {
                using (var stream = new FileStream(imgPath, FileMode.Open, FileAccess.Read))
                {
                    pbUpload.Image = Image.FromStream(stream);
                }
                isDefaultImage = false;
            }
            else { isDefaultImage = true; }

            trainee.Modules = ConvertDataTableToCourseModules(tId);
            LoadModuleGrid(trainee.Modules, 0);
        }

        private List<CourseModule> ConvertDataTableToCourseModules(int traineeId)
        {
            List<CourseModule> list = new List<CourseModule>();
            DataTable dt = repo.GetModulesByTraineeId(traineeId);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new CourseModule
                {
                    CourseModuleId = Convert.ToInt32(row["CourseModuleId"]),
                    ModuleName = row["ModuleName"].ToString(),
                    TraineeId = traineeId,
                    Duration = Convert.ToInt32(row["Duration"])
                });
            }
            return list;
        }
        private void btnViewReport_Click(object sender, EventArgs e)
        {
            List<TraineeInfoVewModel> list = new List<TraineeInfoVewModel>();
            DataTable dt = repo.GetAllTraineeInfo();
            TraineeInfoVewModel obj;
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    obj = new TraineeInfoVewModel();
                    obj.TraineeId = Convert.ToInt32(dt.Rows[i]["TraineeId"].ToString());
                    obj.TraineeName = dt.Rows[i]["TraineeName"].ToString();
                    obj.MobileNo = dt.Rows[i]["MobileNo"].ToString();
                    obj.IsEnrolled = dt.Rows[i]["IsEnrolled"].ToString();
                    obj.AdmissionDate = Convert.ToDateTime(dt.Rows[i]["AdmissionDate"].ToString());

                    obj.CourseId = Convert.ToInt32(dt.Rows[i]["CourseId"].ToString());
                    obj.RegFee = Convert.ToInt32(dt.Rows[i]["RegFee"].ToString());
                    obj.CourseName = dt.Rows[i]["CourseName"].ToString();


                    string fullPath = Application.StartupPath + "\\images\\" + dt.Rows[i]["ImageUrl"].ToString();
                    obj.ImageUrl = fullPath;

                    if (System.IO.File.Exists(fullPath))
                    {
                        obj.ImageBinary = System.IO.File.ReadAllBytes(fullPath);
                    }
                    list.Add(obj);
                }
                using (ReportViewerForm frmObj = new ReportViewerForm(list))
                {
                    frmObj.ShowDialog();
                }

            }
        }

    }
}

