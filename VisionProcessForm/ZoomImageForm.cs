using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VisionProcessForm
{
    public partial class ZoomImageForm : Form
    {
        #region Member Variables
        private float m_fZoomValue = 0;
        #endregion

        #region Properties
        public float ref_fZoomValue { get { return m_fZoomValue; } }

        #endregion

        public ZoomImageForm(float fZoomCount)
        {
            m_fZoomValue = fZoomCount;
            InitializeComponent();
            UpdateGUI();
        }

        private void UpdateGUI()
        {
            cbo_ZoomValue.SelectedItem = (m_fZoomValue * 100).ToString() + "%";
        }

        private void cbo_ZoomValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbo_ZoomValue.SelectedItem.ToString())
            {
                case "25%":
                    m_fZoomValue = 0.25f;
                    break;
                case "50%":
                    m_fZoomValue = 0.5f;
                    break;
                case "75%":
                    m_fZoomValue = 0.75f;
                    break;
                case "100%":
                    m_fZoomValue = 1f;
                    break;
                case "125%":
                    m_fZoomValue = 1.25f;
                    break;
                case "150%":
                    m_fZoomValue = 1.5f;
                    break;
                case "175%":
                    m_fZoomValue = 1.75f;
                    break;
                case "200%":
                    m_fZoomValue = 2f;
                    break;
                case "225%":
                    m_fZoomValue = 2.25f;
                    break;
                case "250%":
                    m_fZoomValue = 2.5f;
                    break;
                case "275%":
                    m_fZoomValue = 2.75f;
                    break;
                case "300%":
                    m_fZoomValue = 3f;
                    break;
                case "325%":
                    m_fZoomValue = 3.25f;
                    break;
                case "350%":
                    m_fZoomValue = 3.5f;
                    break;
                case "375%":
                    m_fZoomValue = 3.75f;
                    break;
                case "400%":
                    m_fZoomValue = 4f;
                    break;
                case "425%":
                    m_fZoomValue = 4.25f;
                    break;
                case "450%":
                    m_fZoomValue = 4.5f;
                    break;
                case "475%":
                    m_fZoomValue = 4.75f;
                    break;
                case "500%":
                    m_fZoomValue = 5f;
                    break;
                default:
                    m_fZoomValue = 1f;
                    break;
            }

        }
    }
}
