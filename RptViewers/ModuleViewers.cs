using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Trainee_Management.Entities;
using Trainee_Management.Reports;

namespace Trainee_Management.RptViewers
{
    public partial class ModuleViewers : Form
    {
        IEnumerable<CourseModule> myList = new List<CourseModule>();
        public ModuleViewers()
        {
            InitializeComponent();
        }
        public ModuleViewers(IEnumerable<CourseModule> list)
        {
            InitializeComponent();
            myList = list;
        }
        private void ModuleViewers_Load(object sender, EventArgs e)
        {
            TraineeModule rptObj = new TraineeModule();
            rptObj.SetDataSource(myList);
            crystalReportViewer1.ReportSource = rptObj;
            crystalReportViewer1.Refresh();
        }
    }
}
