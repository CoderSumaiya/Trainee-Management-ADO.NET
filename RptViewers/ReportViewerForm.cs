using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Trainee_Management.Reports;
using Trainee_Management.Repositories;
using Trainee_Management.ViewModels;

namespace Trainee_Management.RptViewers
{
    public partial class ReportViewerForm : Form
    {
        IEnumerable<TraineeInfoVewModel> myList = new List<TraineeInfoVewModel>();
        TraineeRepo repo = new TraineeRepo();
        public ReportViewerForm(IEnumerable<TraineeInfoVewModel> list)
        {
            InitializeComponent();
            myList = list;
        }
        public ReportViewerForm()
        {
            InitializeComponent();
        }

        private void ReportViewerForm_Load(object sender, EventArgs e)
        {
            rptTraineeInfo rptObj = new rptTraineeInfo();
            rptObj.SetDataSource(myList);
            var moduleData = repo.GetAllModules();
            rptObj.Subreports["TraineeModule.rpt"].SetDataSource(moduleData);
            crystalReportViewer1.ReportSource = rptObj;
            crystalReportViewer1.Refresh();
        }
    }
}
