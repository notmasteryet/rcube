using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RCube
{
    public partial class SolutionViewForm : Form
    {
        public SolutionViewForm()
        {
            InitializeComponent();
        }

        public void Clear()
        {
            listView1.Items.Clear();
            imageList1.Images.Clear();

        }

        public void AddLine(string name, string instruction, Image icon)
        {
            int index = listView1.Items.Count;
            imageList1.Images.Add(icon);

            ListViewItem item = listView1.Items.Add(name);
            item.SubItems.Add(instruction);
            item.ImageIndex = index;
        }
    }
}

