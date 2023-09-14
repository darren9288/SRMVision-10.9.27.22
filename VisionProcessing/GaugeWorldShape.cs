using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
#if (Debug_2_12 || Release_2_12)
using Euresys.Open_eVision_2_12;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
using Euresys.Open_eVision_1_2;
#endif
using Common;

namespace VisionProcessing
{
    public class GaugeWorldShape
    {
        #region Member Variables

        private EWorldShape m_objWorldShape = new EWorldShape();

        #endregion

        #region Properties

        public EWorldShape ref_objWorldShape { get { return m_objWorldShape; } }

        #endregion

        public GaugeWorldShape()
        {

        }

        public void SetZoom(float fZoomX, float fZoomY)
        {
            m_objWorldShape.SetZoom(fZoomX, fZoomY);
        }
    }
}
