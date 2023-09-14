using System;
using System.Collections.Generic;
using System.Text;
#if (Debug_2_12 || Release_2_12)
using Euresys.Open_eVision_2_12;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
using Euresys.Open_eVision_1_2;
#endif

namespace VisionProcessing
{
    public class Color24
    {
        #region Member Variables
        private EC24 m_Color24 = new EC24();
        #endregion

        #region Properties
        public EC24 ref_Color24 { get { return m_Color24; } set { m_Color24 = value; } }
        #endregion

        public Color24()
        {
        }

        public Color24(int intRedColor, int intGreenColor, int intBlueColor)
        {
            m_Color24.C0 = (byte)intRedColor;
            m_Color24.C1 = (byte)intGreenColor;
            m_Color24.C2 = (byte)intBlueColor;
        }

    


        /// <summary>
        /// Set color by specifying the rgb value
        /// </summary>
        /// <param name="intRedColor">red color value</param>
        /// <param name="intGreenColor">green color value</param>
        /// <param name="intBlueColor">blue color value</param>
        public void SetColor(int intRedColor, int intGreenColor, int intBlueColor)
        {
            m_Color24.C0 = (byte)intRedColor;
            m_Color24.C1 = (byte)intGreenColor;
            m_Color24.C2 = (byte)intBlueColor;
        }
    }
}
