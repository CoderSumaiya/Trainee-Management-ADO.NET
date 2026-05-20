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

namespace Trainee_Management.RptViewers
{
    public partial class ModuleViewer : Form
    {
        IEnumerable<CourseModule> myList = new List<CourseModule>();
        public ModuleViewer()
        {
            InitializeComponent();
        }
    }
}
