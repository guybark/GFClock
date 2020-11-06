using GFClockWinForms.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GFClockWinForms
{
    public partial class SettingsForm : Form
    {
        private ClockFace clockFace;

        public SettingsForm(ClockFace clockFace, float makeEverythingBiggerScaling)
        {
            this.clockFace = clockFace;

            InitializeComponent();

            if (makeEverythingBiggerScaling > 1)
            {
                // Barker: Account for text scaling in this dialog.
            }

            this.radioButton1.Checked = (this.clockFace.VisibleHandCount > 1);
            this.radioButton2.Checked = !this.radioButton1.Checked;

            var imageRunning = global::GFClockWinForms.Properties.Resources.ClockFace;
            var imageNotRunning = new Bitmap(1, 1);

            // Barker Todo: Localize all this!
            dataGridViewDetails.Rows.Add(imageRunning, "Sundials", "2000 years ago", "Sundials");
            dataGridViewDetails.Rows.Add(imageNotRunning, "Big Ben", "1859", "Big Ben");
            dataGridViewDetails.Rows.Add(imageRunning, "Dad's Grandfather Clock", "1777", "GFClock");
            dataGridViewDetails.Rows.Add(imageNotRunning, "Deep Space Atomic Clock", "2019", "NASA");

            dataGridViewDetails.SortCompare += DataGridViewDetails_SortCompare;
            dataGridViewDetails.Columns[0].SortMode = DataGridViewColumnSortMode.Automatic;

            this.FormClosing += SettingsForm_FormClosing;
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.clockFace.VisibleHandCount = (this.radioButton1.Checked ? 2 : 1);
        }

        private void DataGridViewDetails_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index > 0)
            {
                return;
            }

            var bm1 = e.CellValue1 as Bitmap;
            var bm2 = e.CellValue2 as Bitmap;

            e.SortResult = (bm1.Width < bm2.Width ? -1 : 1);

            e.Handled = true;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            dataGridViewDetails.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            comboBoxClockType.SelectedIndex = 0;

            // For this demo, consider the ComboBox to be the most likely
            // control that my customers will want to interact with when
            // the Setting window appears.
            comboBoxClockType.Select();

            // Note: This doesn't help with keyboard accessibility.
            // It shows full text for truncated cell on mouse hover.
            dataGridViewDetails.ShowCellToolTips = true;
        }

        private class DataGridViewImageCellWithCustomValueColumn : DataGridViewImageColumn
        {
            public DataGridViewImageCellWithCustomValueColumn()
            {
                this.CellTemplate = new DataGridViewImageCellWithCustomValue();
            }
        }

        private class DataGridViewImageCellWithCustomValue : DataGridViewImageCell
        {
            protected override AccessibleObject CreateAccessibilityInstance()
            {
                return new MyDataGridViewCellAccessibleObject(this);
            }

            protected class MyDataGridViewCellAccessibleObject : DataGridViewImageCellAccessibleObject
            {
                private DataGridViewImageCellWithCustomValue owner;

                public MyDataGridViewCellAccessibleObject(DataGridViewImageCellWithCustomValue owner) : base(owner)
                {
                    this.owner = owner;
                }

                public override string Name 
                { 
                    get
                    {
                        // TODO: Localize this.
                        return "Status, " + base.Name;
                    }
                }

                public override string Value
                {
                    get
                    {
                        // Todo: This Value string must be as localized as any visual string would be.
                        return (this.owner.Value as Bitmap).Width > 1 ?
                            "Running" : "Not running";
                    }
                }
            }
        }

    }
}
