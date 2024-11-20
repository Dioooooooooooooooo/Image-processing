using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageProcessing
{
    public partial class CoinResultsForm : Form
    {
        public CoinResultsForm()
        {
            InitializeComponent();
            SetupControls();
        }

        private void SetupControls()
        {
            this.Text = "Coin Detection Results";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            TableLayoutPanel tableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 5,
                Padding = new Padding(10)
            };

            AddRow(tableLayout, "5 Centavo:", 0);
            AddRow(tableLayout, "10 Centavo:", 1);
            AddRow(tableLayout, "25 Centavo:", 2);
            AddRow(tableLayout, "1 Peso:", 3);
            AddRow(tableLayout, "5 Peso:", 4);
            AddRow(tableLayout, "Total Coins: ", 5, true);

            Button okButton = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Dock = DockStyle.Bottom,
                Height = 30
            };

            this.Controls.Add(tableLayout);
            this.Controls.Add(okButton);
        }

        private void AddRow(TableLayoutPanel table, string labelText, int row, bool isBold = false)
        {
            Label label = new Label
            {
                Text = labelText,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            Label value = new Label
            {
                Text = "0",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            if (isBold)
            {
                label.Font = new Font(label.Font, FontStyle.Bold);
                value.Font = new Font(value.Font, FontStyle.Bold);
            }

            table.Controls.Add(label, 0, row);
            table.Controls.Add(value, 1, row);
        }

        public void UpdateResults(Dictionary<string, double> results)
        {
            TableLayoutPanel table = (TableLayoutPanel)this.Controls[0];

            UpdateValue(table, 0, results.ContainsKey("5 Centavo") ? results["5 Centavo"].ToString() : "0");
            UpdateValue(table, 1, results.ContainsKey("10 Centavo") ? results["10 Centavo"].ToString() : "0");
            UpdateValue(table, 2, results.ContainsKey("25 Centavo") ? results["25 Centavo"].ToString() : "0");
            UpdateValue(table, 3, results.ContainsKey("1 Peso") ? results["1 Peso"].ToString() : "0");
            UpdateValue(table, 4, results.ContainsKey("5 Peso") ? results["5 Peso"].ToString() : "0");
            UpdateValue(table, 5, results.ContainsKey("5 Peso") ? results["5 Peso"].ToString() : "0");
           
            UpdateValue(table, 5, results.ContainsKey("Value") ? results["Value"].ToString() : "0");
        }


        private void UpdateValue(TableLayoutPanel table, int row, string value)
        {
            ((Label)table.GetControlFromPosition(1, row)).Text = value;
        }


    }
}
