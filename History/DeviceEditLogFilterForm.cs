using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common;

namespace History
{
    public partial class DeviceEditLogFilterForm : Form
    {
        private string m_strPreviousFilterText = "";
        private string m_strPreviousFilterColumn = "";
        private string m_strPreviousFilterMatchField = "";
        private string m_strPreviousFilterVisionModule = "";
        private string m_strPreviousFilterCategory = "";
        private string m_strPreviousFilterMethod = "";
        private string m_strPreviousViewDaily = "";
        private string m_strPreviousViewWeekly = "";
        private string m_strSelectedMonth = "";
        private int addon = 0;

        public string ref_strFilterText { get { return txt_FilterText.Text; } }
        public string ref_strFilterColumn { get { return cbo_FilterColumn.SelectedItem.ToString(); } }
        public string ref_strFilterMatchField { get { return cbo_MatchField.SelectedItem.ToString(); } }
        public string ref_strFilterVisionModule { get { return cbo_VisionModule.SelectedItem.ToString(); } }
        public string ref_strFilterCategory { get { return cbo_Category.SelectedItem.ToString(); } }
        public string ref_strFilterMethod { get { return cbo_OrderBy.SelectedItem.ToString(); } }
        public string ref_strFilterViewWeekly { get { return cbo_Weekly.SelectedItem.ToString(); } }
        public string ref_strFilterViewDaily { get { return cbo_Daily.SelectedItem.ToString(); } }

        public DeviceEditLogFilterForm(string strPreviousFilterText, string strPreviousFilterColumn, string strPreviousFilterMatchField, string strPreviousFilterVisionModule, string strPreviousFilterCategory, string strPreviousFilterMethod, string strPreviousFilterViewDaily, string strPreviousFilterViewWeekly, string strSelectedMonth, int intVisionModuleCount)
        {
            InitializeComponent();
            m_strSelectedMonth = strSelectedMonth;
            txt_FilterText.Text = m_strPreviousFilterText = strPreviousFilterText;

            if (strPreviousFilterColumn == "")
                cbo_FilterColumn.SelectedItem = m_strPreviousFilterColumn = "All";
            else
                cbo_FilterColumn.SelectedItem = m_strPreviousFilterColumn = strPreviousFilterColumn;

            if (strPreviousFilterColumn == "")
                cbo_MatchField.SelectedItem = m_strPreviousFilterMatchField = "Any Part Of Field";
            else
                cbo_MatchField.SelectedItem = m_strPreviousFilterMatchField = strPreviousFilterMatchField;

            cbo_VisionModule.Items.Clear();
            cbo_VisionModule.Items.Add("All Vision");
            for (int i = 0; i < intVisionModuleCount; i++)
            {
                cbo_VisionModule.Items.Add("Vision" + (i+1).ToString());
            }
            cbo_VisionModule.SelectedIndex = 0;

            if (strPreviousFilterVisionModule == "")
                cbo_VisionModule.SelectedItem = m_strPreviousFilterVisionModule = "All Vision";
            else
                cbo_VisionModule.SelectedItem = m_strPreviousFilterVisionModule = strPreviousFilterVisionModule;

            //cbo_Category.SelectedIndex = 0;
            if (strPreviousFilterCategory == "")
                cbo_Category.SelectedItem = m_strPreviousFilterCategory = "Any Category";
            else
                cbo_Category.SelectedItem = m_strPreviousFilterCategory = strPreviousFilterCategory;

            if (strPreviousFilterMethod == "")
                cbo_OrderBy.SelectedItem = m_strPreviousFilterMethod = "Monthly";
            else
                cbo_OrderBy.SelectedItem = m_strPreviousFilterMethod = strPreviousFilterMethod;

            DateTime d = DateTime.Now;
            DateTime d1 = new DateTime(d.Year, 1, 1);
            DateTime d2 = new DateTime(d.Year, d.Month, 1);
            DateTime first = new DateTime(Int16.Parse(strSelectedMonth.Substring(0, 4)), 1, 1);

            if (first.DayOfWeek != DayOfWeek.Sunday)
            {
                addon += (7 - (int)first.DayOfWeek); //if january 1 not sunday nid addback to total days
            }


            int dayspass = (d - d1).Days + 1 - addon;
            int TotalWeek = (int)(Math.Ceiling((double)dayspass/7));

            int daysofMonth = DateTime.DaysInMonth(Int16.Parse(strSelectedMonth.Substring(0, 4)), Int16.Parse(strSelectedMonth.Substring(5)));
            int NumberofWeek = (int)Math.Ceiling((double)daysofMonth / 7);
            DateTime t = new DateTime(Int16.Parse(strSelectedMonth.Substring(0, 4)), Int16.Parse(strSelectedMonth.Substring(5)), 1);

            if (t.DayOfWeek == DayOfWeek.Saturday || t.DayOfWeek == DayOfWeek.Friday) //become 6 weeks
                NumberofWeek += 1;

            if (d.Month == Int16.Parse(strSelectedMonth.Substring(5)))
            {
                double WeekNo = (Math.Ceiling((double)(d - t).Days / 7));
                NumberofWeek = (int)WeekNo;
            }
            else
            {
                for (int i = d.Month; i > Int16.Parse(strSelectedMonth.Substring(5)); i--)
                {
                    if (i == d.Month)
                        TotalWeek -= (int)(Math.Ceiling((double)(d - d2).Days / 7));
                    else
                    {
                        int temp = DateTime.DaysInMonth(Int16.Parse(strSelectedMonth.Substring(0, 4)), i);
                        int temp2 = (int)Math.Ceiling((double)temp / 7);
                        DateTime temp3 = new DateTime(Int16.Parse(strSelectedMonth.Substring(0, 4)), i, 1);

                        if (temp3.DayOfWeek != DayOfWeek.Sunday)
                            temp2 -= 1;

                        TotalWeek -= temp2;
                    }
                }
            }

            for(int i=0;i<NumberofWeek;i++)
            { 
                cbo_Weekly.Items.Add((TotalWeek - i).ToString());
            }

            if (strPreviousFilterViewWeekly == "")
                cbo_Weekly.SelectedItem = m_strPreviousViewWeekly = cbo_Weekly.Items[0].ToString();
            else
                cbo_Weekly.SelectedItem = m_strPreviousViewWeekly = strPreviousFilterViewWeekly;

            if(cbo_Weekly.SelectedItem == null)
            {
                cbo_Weekly.SelectedIndex = 0;
            }

            if (Int16.Parse(strSelectedMonth.Substring(5)) != d.Month)
            {
                int dayinComboBox = DateTime.DaysInMonth(Int16.Parse(strSelectedMonth.Substring(0, 4)), Int16.Parse(strSelectedMonth.Substring(5)));

                for(int i=0; i<dayinComboBox;i++)
                {
                    cbo_Daily.Items.Add((i + 1).ToString());
                }

                int Totaldays = 0;
                for (int i = 0; i < Int16.Parse(strSelectedMonth.Substring(5)); i++)
                {
                    int month = i + 1;
                    Totaldays += DateTime.DaysInMonth(Int16.Parse(strSelectedMonth.Substring(0, 4)), month);
                }

                if ((Int16.Parse(cbo_Weekly.SelectedItem.ToString()) * 7) + addon > Totaldays)
                {
                    DateTime date = new DateTime(Int16.Parse(strSelectedMonth.Substring(0, 4)), Int16.Parse(strSelectedMonth.Substring(5)), daysofMonth);
                    if (date.DayOfWeek != DayOfWeek.Sunday)
                    {
                        cbo_Daily.SelectedItem = (daysofMonth - (int)date.DayOfWeek).ToString();
                    }
                }
                else
                {
                    int temp = Math.Abs(Totaldays - (((Int16.Parse(cbo_Weekly.SelectedItem.ToString())) * 7) + addon));

                    if (temp > daysofMonth)
                        cbo_Daily.SelectedItem = (temp - daysofMonth).ToString();
                    else
                        cbo_Daily.SelectedItem = (daysofMonth - temp).ToString();
                }
            }
            else
            {
                int dayinComboBox = (d - d2).Days + 1;

                for (int i = 0; i < dayinComboBox; i++)
                {
                    cbo_Daily.Items.Add((i + 1).ToString());
                    cbo_Daily.SelectedItem = m_strPreviousViewDaily = d.Day.ToString();
                }
            }
        }


        private void btn_Close_Click(object sender, EventArgs e)
        {
            txt_FilterText.Text = m_strPreviousFilterText;
            cbo_FilterColumn.SelectedItem = m_strPreviousFilterColumn;
            cbo_MatchField.SelectedItem = m_strPreviousFilterMatchField;
            cbo_VisionModule.SelectedItem = m_strPreviousFilterVisionModule;
            cbo_VisionModule.SelectedItem = m_strPreviousFilterCategory;
            cbo_OrderBy.SelectedItem = m_strPreviousFilterMethod;
            cbo_Weekly.SelectedItem = m_strPreviousViewWeekly;

            this.Close();
            this.Dispose();
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            //if (txt_FilterText.Text == "")
            //{
            //    SRMMessageBox.Show("Please Enter Keyword!", "Error",MessageBoxButtons.OK);
            //    return;
            //}
            //if (txt_FilterText.Text == "" && cbo_VisionModule.SelectedIndex == 0 && cbo_Category.SelectedIndex == 0)
            //{
            //    this.DialogResult = DialogResult.Cancel;
            //}
            //else
            {
                this.DialogResult = DialogResult.OK;
            }
        }

        private void cbo_OrderBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(cbo_OrderBy.SelectedIndex)
            {
                case 1: cbo_Daily.Visible = false;
                        lbl_Daily.Visible = false;
                        cbo_Weekly.Visible = true;
                        lbl_Weekly.Visible = true;
                    break;
                case 2: cbo_Weekly.Visible = false;
                        lbl_Weekly.Visible = false;
                        cbo_Daily.Visible = true;
                        lbl_Daily.Visible = true;
                    break;
                default:
                    cbo_Daily.Visible = false;
                    lbl_Daily.Visible = false;
                    cbo_Weekly.Visible = false;
                    lbl_Weekly.Visible = false;
                    break;
            }
        }

        private void cbo_Daily_SelectedIndexChanged(object sender, EventArgs e)
        {
            DateTime d1 = new DateTime(Int16.Parse(m_strSelectedMonth.Substring(0, 4)), 1, 1);
            DateTime d2 = new DateTime(Int16.Parse(m_strSelectedMonth.Substring(0, 4)), Int16.Parse(m_strSelectedMonth.Substring(5)), Int16.Parse(cbo_Daily.SelectedItem.ToString()));
            int TotalWeek = (int)Math.Ceiling(((double)((d2 - d1).Days + 1 - addon) / 7));
            cbo_Weekly.SelectedItem = m_strPreviousViewWeekly = TotalWeek.ToString();
        }
    }
}
