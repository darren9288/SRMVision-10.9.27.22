using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if (Debug_2_12 || Release_2_12)
using Euresys.Open_eVision_2_12;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
using Euresys.Open_eVision_1_2;
#endif
using Common;
using System.Drawing;

namespace VisionProcessing
{
    public class CalibrationLead3D
    {
        #region Member Variables
        private Pen m_PenYellow = new Pen(Color.Yellow, 2);
        private Pen m_PenRed = new Pen(Color.Red, 2);
        private Pen m_PenLime = new Pen(Color.Lime, 2);
        private Pen m_PenCyan = new Pen(Color.Cyan, 2);
        private string m_strErrorMessage = "";
        private bool m_blnHorizontal = true;
        private bool m_blnBlobOK_Top = false;
        private bool m_blnBlobOK_Bottom = false;
        private bool m_blnBlobOK_Left = false;
        private bool m_blnBlobOK_Right = false;
        private EBlobs m_objEBlobs_Top;
        private EBlobs m_objEBlobs_Bottom;
        private EBlobs m_objEBlobs_Left;
        private EBlobs m_objEBlobs_Right;
        private TrackLog m_objTrackLog;
        private EWorldShape m_objWorldShape;
        private ArrayList m_arrBlobTop = new ArrayList();
        private ArrayList m_arrBlobBottom = new ArrayList();
        private ArrayList m_arrBlobLeft = new ArrayList();
        private ArrayList m_arrBlobRight = new ArrayList();
        private BlobsFeatures m_stcBlobTop = new BlobsFeatures();
        private BlobsFeatures m_stcBlobBottom = new BlobsFeatures();
        private BlobsFeatures m_stcBlobLeft = new BlobsFeatures();
        private BlobsFeatures m_stcBlobRight = new BlobsFeatures();

        private int m_intGaugeThickness = 20;
        private int m_intGaugeThreshold = 20;
        private int m_intGaugeTransitionChoice = 2;
        private int m_intGaugeMinAmplitude = 10;
        private int m_intGaugeMinArea = 1;
        private int m_intGaugeFilter = 1;
        private float m_fGaugeFilterThreshold = 3;
        private int m_intGaugeFilterPass = 3;
        private int m_intGaugeSamplingSteps = 2;

        private float m_fRotateAngle = 0;
        private float m_fRotateAngleSide = 0;

        private float m_fTopDistance = 0;
        private float m_fBottomDistance = 0;
        private float m_fLeftDistance = 0;
        private float m_fRightDistance = 0;

        private float m_fTopAngle = 0;
        private float m_fBottomAngle = 0;
        private float m_fLeftAngle = 0;
        private float m_fRightAngle = 0;

        private float m_f2DCenterPixelCountX1 = 0;
        private float m_f2DCenterPixelCountX2 = 0;
        private float m_f2DCenterPixelCountX3 = 0;
        private float m_f2DCenterPixelCountX4 = 0;
        private float m_f2DCenterPixelCountX5 = 0;

        private float m_f2DCenterPixelCountY1 = 0;
        private float m_f2DCenterPixelCountY2 = 0;
        private float m_f2DCenterPixelCountY3 = 0;
        private float m_f2DCenterPixelCountY4 = 0;
        private float m_f2DCenterPixelCountY5 = 0;

        private float m_f3DTopPixelCount1 = 0;
        private float m_f3DTopPixelCount2 = 0;

        private float m_f3DBottomPixelCount1 = 0;
        private float m_f3DBottomPixelCount2 = 0;

        private float m_f3DLeftPixelCount1 = 0;
        private float m_f3DLeftPixelCount2 = 0;

        private float m_f3DRightPixelCount1 = 0;
        private float m_f3DRightPixelCount2 = 0;
        
        private int m_intSelectedImage = 0;
        private int m_intROICount = 5;
        private float m_fImageGain = 1;
        private float m_fSize2DC1;
        private float m_fSize2DC2;
        private float m_fSize2DC3;
        private float m_fSize2DC4;
        private float m_fSize2DC5;
        private float m_fSize2DC6;
        private float m_fSize2DC7;
        private float m_fSize2DC8;
        private float m_fSize2DC9;
        private float m_fSize2DC10;
        private float m_fSize2DA1;
        private float m_fSize2DA2;
        private float m_fSize3DT1;
        private float m_fSize3DT2;
        private float m_fSize3DT3;
        private float m_fSize3DB1;
        private float m_fSize3DB2;
        private float m_fSize3DB3;
        private float m_fSize3DL1;
        private float m_fSize3DL2;
        private float m_fSize3DL3;
        private float m_fSize3DR1;
        private float m_fSize3DR2;
        private float m_fSize3DR3;
        private float m_fCalibration2DX;
        private float m_fCalibration2DY;
        private float m_fCalibration3DTop;
        private float m_fCalibration3DBottom;
        private float m_fCalibration3DLeft;
        private float m_fCalibration3DRight;
        private List<ROI> m_arrROIs;
        private List<List<LGauge>> m_arrLGauge;
        private int m_intSelectedROIPrev = 0;
        private bool m_blnCursorShapeVerifying = false;

        private int m_intThresholdCenter = 125;
        private int m_intThresholdTop = 125;
        private int m_intThresholdBottom = 125;
        private int m_intThresholdLeft = 125;
        private int m_intThresholdRight = 125;
        #endregion
        #region Struct 
        struct BlobsFeatures
        {
            // Object Data
            public int intObjNo;
            public float fCenterX;
            public float fCenterY;
            public float fWidth;
            public float fHeight;
      
        }
        #endregion
        #region Properties
        public float ref_fRotateAngle { get { return m_fRotateAngle; } set { m_fRotateAngle = value; } }
        public float ref_fRotateAngleSide { get { return ref_fRotateAngleSide; } set { m_fRotateAngleSide = value; } }
        public float ref_f2DCenterPixelCountX1 { get { return m_f2DCenterPixelCountX1; } }
        public float ref_f2DCenterPixelCountX2 { get { return m_f2DCenterPixelCountX2; } }
        public float ref_f2DCenterPixelCountX3 { get { return m_f2DCenterPixelCountX3; } }
        public float ref_f2DCenterPixelCountX4 { get { return m_f2DCenterPixelCountX4; } }
        public float ref_f2DCenterPixelCountX5 { get { return m_f2DCenterPixelCountX5; } }
        public float ref_f2DCenterPixelCountY1 { get { return m_f2DCenterPixelCountY1; } }
        public float ref_f2DCenterPixelCountY2 { get { return m_f2DCenterPixelCountY2; } }
        public float ref_f2DCenterPixelCountY3 { get { return m_f2DCenterPixelCountY3; } }
        public float ref_f2DCenterPixelCountY4 { get { return m_f2DCenterPixelCountY4; } }
        public float ref_f2DCenterPixelCountY5 { get { return m_f2DCenterPixelCountY5; } }
        public float ref_f3DTopPixelCount1 { get { return m_f3DTopPixelCount1; } }
        public float ref_f3DTopPixelCount2 { get { return m_f3DTopPixelCount2; } }
        public float ref_f3DBottomPixelCount1 { get { return m_f3DBottomPixelCount1; } }
        public float ref_f3DBottomPixelCount2 { get { return m_f3DBottomPixelCount2; } }
        public float ref_f3DLeftPixelCount1 { get { return m_f3DLeftPixelCount1; } }
        public float ref_f3DLeftPixelCount2 { get { return m_f3DLeftPixelCount2; } }
        public float ref_f3DRightPixelCount1 { get { return m_f3DRightPixelCount1; } }
        public float ref_f3DRightPixelCount2 { get { return m_f3DRightPixelCount2; } }
        
        public int ref_intGaugeThickness { get { return m_intGaugeThickness; } set { m_intGaugeThickness = value; } }
        public int ref_intGaugeThreshold { get { return m_intGaugeThreshold; } set { m_intGaugeThreshold = value; } }
        public int ref_intGaugeTransitionChoice { get { return m_intGaugeTransitionChoice; } set { m_intGaugeTransitionChoice = value; } }
        public int ref_intGaugeMinAmplitude { get { return m_intGaugeMinAmplitude; } set { m_intGaugeMinAmplitude = value; } }
        public int ref_intGaugeMinArea { get { return m_intGaugeMinArea; } set { m_intGaugeMinArea = value; } }
        public int ref_intGaugeFilter { get { return m_intGaugeFilter; } set { m_intGaugeFilter = value; } }
        public float ref_fGaugeFilterThreshold { get { return m_fGaugeFilterThreshold; } set { m_fGaugeFilterThreshold = value; } }
        public int ref_intGaugeFilterPass { get { return m_intGaugeFilterPass; } set { m_intGaugeFilterPass = value; } }
        public int ref_intGaugeSamplingSteps { get { return m_intGaugeSamplingSteps; } set { m_intGaugeSamplingSteps = value; } }
        public bool ref_blnHorizontal { get { return m_blnHorizontal; } set { m_blnHorizontal = value; } }
        public int ref_intThresholdCenter { get { return m_intThresholdCenter; } set { m_intThresholdCenter = value; } }
        public int ref_intThresholdTop { get { return m_intThresholdTop; } set { m_intThresholdTop = value; } }
        public int ref_intThresholdBottom { get { return m_intThresholdBottom; } set { m_intThresholdBottom = value; } }
        public int ref_intThresholdLeft { get { return m_intThresholdLeft; } set { m_intThresholdLeft = value; } }
        public int ref_intThresholdRight { get { return m_intThresholdRight; } set { m_intThresholdRight = value; } }
        public float ref_fPixelX { get { return m_objWorldShape.XResolution; } }
        public float ref_fPixelY { get { return m_objWorldShape.YResolution; } }
        public string ref_strErrorMessage { get { return m_strErrorMessage; } }
        public int ref_intROICount { get { return m_intROICount; } }
        public int ref_intSelectedImage { get { return m_intSelectedImage; } set { m_intSelectedImage = value; } }
        public float ref_fImageGain { get { return m_fImageGain; } set { m_fImageGain = value; } }
        public float ref_fSize2DC1 { get { return m_fSize2DC1; } set { m_fSize2DC1 = value; } }
        public float ref_fSize2DC2 { get { return m_fSize2DC2; } set { m_fSize2DC2 = value; } }
        public float ref_fSize2DC3 { get { return m_fSize2DC3; } set { m_fSize2DC3 = value; } }
        public float ref_fSize2DC4 { get { return m_fSize2DC4; } set { m_fSize2DC4 = value; } }
        public float ref_fSize2DC5 { get { return m_fSize2DC5; } set { m_fSize2DC5 = value; } }
        public float ref_fSize2DC6 { get { return m_fSize2DC6; } set { m_fSize2DC6 = value; } }
        public float ref_fSize2DC7 { get { return m_fSize2DC7; } set { m_fSize2DC7 = value; } }
        public float ref_fSize2DC8 { get { return m_fSize2DC8; } set { m_fSize2DC8 = value; } }
        public float ref_fSize2DC9 { get { return m_fSize2DC9; } set { m_fSize2DC9 = value; } }
        public float ref_fSize2DC10 { get { return m_fSize2DC10; } set { m_fSize2DC10 = value; } }
        public float ref_fSize2DA1 { get { return m_fSize2DA1; } set { m_fSize2DA1 = value; } }
        public float ref_fSize2DA2 { get { return m_fSize2DA2; } set { m_fSize2DA2 = value; } }
        public float ref_fSize3DT1 { get { return m_fSize3DT1; } set { m_fSize3DT1 = value; } }
        public float ref_fSize3DT2 { get { return m_fSize3DT2; } set { m_fSize3DT2 = value; } }
        public float ref_fSize3DT3 { get { return m_fSize3DT3; } set { m_fSize3DT3 = value; } }
        public float ref_fSize3DB1 { get { return m_fSize3DB1; } set { m_fSize3DB1 = value; } }
        public float ref_fSize3DB2 { get { return m_fSize3DB2; } set { m_fSize3DB2 = value; } }
        public float ref_fSize3DB3 { get { return m_fSize3DB3; } set { m_fSize3DB3 = value; } }
        public float ref_fSize3DL1 { get { return m_fSize3DL1; } set { m_fSize3DL1 = value; } }
        public float ref_fSize3DL2 { get { return m_fSize3DL2; } set { m_fSize3DL2 = value; } }
        public float ref_fSize3DL3 { get { return m_fSize3DL3; } set { m_fSize3DL3 = value; } }
        public float ref_fSize3DR1 { get { return m_fSize3DR1; } set { m_fSize3DR1 = value; } }
        public float ref_fSize3DR2 { get { return m_fSize3DR2; } set { m_fSize3DR2 = value; } }
        public float ref_fSize3DR3 { get { return m_fSize3DR3; } set { m_fSize3DR3 = value; } }
        public float ref_fCalibration2DX { get { return m_fCalibration2DX; } }
        public float ref_fCalibration2DY { get { return m_fCalibration2DY; } }
        public float ref_fCalibration3DTop { get { return m_fCalibration3DTop; } }
        public float ref_fCalibration3DBottom { get { return m_fCalibration3DBottom; } }
        public float ref_fCalibration3DLeft { get { return m_fCalibration3DLeft; } }
        public float ref_fCalibration3DRight { get { return m_fCalibration3DRight; } }
        public float ref_fTopDistance { get { return m_fTopDistance; } }
        public float ref_fBottomDistance { get { return m_fBottomDistance; } }
        public float ref_fLeftDistance { get { return m_fLeftDistance; } }
        public float ref_fRightDistance { get { return m_fRightDistance; } }
        public float ref_fTopAngle { get { return m_fTopAngle; } }
        public float ref_fBottomAngle { get { return m_fBottomAngle; } }
        public float ref_fLeftAngle { get { return m_fLeftAngle; } }
        public float ref_fRightAngle { get { return m_fRightAngle; } }
        public List<ROI> ref_arrROIs { get { return m_arrROIs; } }
        public List<List<LGauge>> ref_arrLGauge { get { return m_arrLGauge; } }
        #endregion

        public CalibrationLead3D()
        {
            m_arrLGauge = new List<List<LGauge>>();
            m_arrLGauge.Add(new List<LGauge>());
            m_arrLGauge.Add(new List<LGauge>());
            m_arrLGauge.Add(new List<LGauge>());
            m_arrLGauge.Add(new List<LGauge>());
            m_arrLGauge.Add(new List<LGauge>());
            m_arrROIs = new List<ROI>();
            m_intROICount = 5;
            m_objEBlobs_Top = new EBlobs();
            m_objEBlobs_Bottom = new EBlobs();
            m_objEBlobs_Left = new EBlobs();
            m_objEBlobs_Right = new EBlobs();
            m_objTrackLog = new TrackLog();
            m_objWorldShape = new EWorldShape();
        }

        public void Dispose()
        {
            for (int i = 0; i < m_arrROIs.Count; i++)
                m_arrROIs[i].Dispose();
            for (int i = 0; i < m_arrLGauge.Count; i++)
                for (int j = 0; j < m_arrLGauge[i].Count; j++)
                    m_arrLGauge[i][j].Dispose();

            m_objEBlobs_Top.Dispose();
            m_objEBlobs_Bottom.Dispose();
            m_objEBlobs_Left.Dispose();
            m_objEBlobs_Right.Dispose();
            m_objWorldShape.Dispose();
        }

        public void AddCalibrationROI(ImageDrawing objImage)
        {

            ROI objROI;

            for (int i = 0; i < m_intROICount; i++)
            {
                if (m_arrROIs.Count > i)
                {
                    m_arrROIs[i].AttachImage(objImage);
                    continue;
                }

                if (i == 0)
                {
                    objROI = new ROI("Center ROI", 1);
                    int intPositionX = 640 - (640 / 2) - 80;
                    int intPositionY = (480 / 2) - 80;
                    objROI.LoadROISetting(intPositionX, intPositionY, 170, 170);
                }
                else if (i == 1)
                {
                    objROI = new ROI("T", 1);
                    int intPostX = 640 - (640 / 2) - 65;
                    int intPostY = (480 / 2) - 180;
                    objROI.LoadROISetting(intPostX, intPostY, 150, 50);
                }
                else if (i == 2)
                {
                    objROI = new ROI("R", 1);
                    int intPostXR = 640 - (640 / 2) + 120;
                    int intPostYR = (480 / 2) - 70;
                    objROI.LoadROISetting(intPostXR, intPostYR, 50, 150);
                }
                else if (i == 3)
                {
                    objROI = new ROI("B", 1);
                    int intPostXB = 640 - (640 / 2) - 65;
                    int intPostYB = (480 / 2) + 120;
                    objROI.LoadROISetting(intPostXB, intPostYB, 150, 50);
                }
                else
                {
                    objROI = new ROI("L", 1);
                    int intPostXL = 640 - (640 / 2) - 170;
                    int intPostYL = (480 / 2) - 70;
                    objROI.LoadROISetting(intPostXL, intPostYL, 50, 150);
                }

                objROI.AttachImage(objImage);
                m_arrROIs.Add(objROI);
            }
        }
        public void AddLGauge(GaugeWorldShape objWorldShape)
        {
            for (int i = 0; i < m_arrLGauge.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        m_arrLGauge[0].Add(new LGauge(objWorldShape));
                        m_arrLGauge[0].Add(new LGauge(objWorldShape));
                        m_arrLGauge[0].Add(new LGauge(objWorldShape));
                        m_arrLGauge[0].Add(new LGauge(objWorldShape));
                        m_arrLGauge[0].Add(new LGauge(objWorldShape));
                        m_arrLGauge[0].Add(new LGauge(objWorldShape));
                        m_arrLGauge[0].Add(new LGauge(objWorldShape));
                        m_arrLGauge[0].Add(new LGauge(objWorldShape));
                        m_arrLGauge[0].Add(new LGauge(objWorldShape));
                        m_arrLGauge[0].Add(new LGauge(objWorldShape));
                        m_arrLGauge[0].Add(new LGauge(objWorldShape));
                        m_arrLGauge[0].Add(new LGauge(objWorldShape));
                        m_arrLGauge[0].Add(new LGauge(objWorldShape));
                        m_arrLGauge[0].Add(new LGauge(objWorldShape));
                        m_arrLGauge[0].Add(new LGauge(objWorldShape));
                        m_arrLGauge[0].Add(new LGauge(objWorldShape));
                        m_arrLGauge[0].Add(new LGauge(objWorldShape));
                        m_arrLGauge[0].Add(new LGauge(objWorldShape));
                        break;
                    case 1:
                        m_arrLGauge[1].Add(new LGauge(objWorldShape));
                        m_arrLGauge[1].Add(new LGauge(objWorldShape));
                        m_arrLGauge[1].Add(new LGauge(objWorldShape));
                        m_arrLGauge[1].Add(new LGauge(objWorldShape));
                        m_arrLGauge[1].Add(new LGauge(objWorldShape));
                        m_arrLGauge[1].Add(new LGauge(objWorldShape));
                        m_arrLGauge[1].Add(new LGauge(objWorldShape));
                        break;
                    case 2:
                        m_arrLGauge[2].Add(new LGauge(objWorldShape));
                        m_arrLGauge[2].Add(new LGauge(objWorldShape));
                        m_arrLGauge[2].Add(new LGauge(objWorldShape));
                        m_arrLGauge[2].Add(new LGauge(objWorldShape));
                        m_arrLGauge[2].Add(new LGauge(objWorldShape));
                        m_arrLGauge[2].Add(new LGauge(objWorldShape));
                        m_arrLGauge[2].Add(new LGauge(objWorldShape));
                        break;
                    case 3:
                        m_arrLGauge[3].Add(new LGauge(objWorldShape));
                        m_arrLGauge[3].Add(new LGauge(objWorldShape));
                        m_arrLGauge[3].Add(new LGauge(objWorldShape));
                        m_arrLGauge[3].Add(new LGauge(objWorldShape));
                        m_arrLGauge[3].Add(new LGauge(objWorldShape));
                        m_arrLGauge[3].Add(new LGauge(objWorldShape));
                        m_arrLGauge[3].Add(new LGauge(objWorldShape));
                        break;
                    case 4:
                        m_arrLGauge[4].Add(new LGauge(objWorldShape));
                        m_arrLGauge[4].Add(new LGauge(objWorldShape));
                        m_arrLGauge[4].Add(new LGauge(objWorldShape));
                        m_arrLGauge[4].Add(new LGauge(objWorldShape));
                        m_arrLGauge[4].Add(new LGauge(objWorldShape));
                        m_arrLGauge[4].Add(new LGauge(objWorldShape));
                        m_arrLGauge[4].Add(new LGauge(objWorldShape));
                        break;
                }
            }

            UpdateLGaugeSetting();
        }

        public void UpdateLGaugeSetting()
        {
            m_arrLGauge[0][0].ref_GaugeTransChoice =
                m_arrLGauge[0][1].ref_GaugeTransChoice =
                m_arrLGauge[0][2].ref_GaugeTransChoice =
                m_arrLGauge[0][3].ref_GaugeTransChoice =
                m_arrLGauge[0][4].ref_GaugeTransChoice =
                m_arrLGauge[0][5].ref_GaugeTransChoice =
                m_arrLGauge[0][6].ref_GaugeTransChoice =
                m_arrLGauge[0][7].ref_GaugeTransChoice =
                m_arrLGauge[0][8].ref_GaugeTransChoice =
                m_arrLGauge[0][9].ref_GaugeTransChoice =
                m_arrLGauge[0][10].ref_GaugeTransChoice =
                m_arrLGauge[0][11].ref_GaugeTransChoice =
                m_arrLGauge[0][12].ref_GaugeTransChoice =
                m_arrLGauge[0][13].ref_GaugeTransChoice =
                m_arrLGauge[0][14].ref_GaugeTransChoice =
                m_arrLGauge[0][15].ref_GaugeTransChoice =
                m_arrLGauge[0][16].ref_GaugeTransChoice =
                m_arrLGauge[0][17].ref_GaugeTransChoice =
                m_arrLGauge[1][0].ref_GaugeTransChoice =
                m_arrLGauge[1][1].ref_GaugeTransChoice =
                     m_arrLGauge[1][2].ref_GaugeTransChoice =
                m_arrLGauge[1][3].ref_GaugeTransChoice =
                     m_arrLGauge[1][4].ref_GaugeTransChoice =
                m_arrLGauge[1][5].ref_GaugeTransChoice =
                   m_arrLGauge[1][6].ref_GaugeTransChoice =
                m_arrLGauge[2][0].ref_GaugeTransChoice =
                m_arrLGauge[2][1].ref_GaugeTransChoice =
                  m_arrLGauge[2][2].ref_GaugeTransChoice =
                m_arrLGauge[2][3].ref_GaugeTransChoice =
                  m_arrLGauge[2][4].ref_GaugeTransChoice =
                m_arrLGauge[2][5].ref_GaugeTransChoice =
                   m_arrLGauge[2][6].ref_GaugeTransChoice =
                m_arrLGauge[3][0].ref_GaugeTransChoice =
                m_arrLGauge[3][1].ref_GaugeTransChoice =
                 m_arrLGauge[3][2].ref_GaugeTransChoice =
                m_arrLGauge[3][3].ref_GaugeTransChoice =
                 m_arrLGauge[3][4].ref_GaugeTransChoice =
                m_arrLGauge[3][5].ref_GaugeTransChoice =
                m_arrLGauge[3][6].ref_GaugeTransChoice =
                m_arrLGauge[4][0].ref_GaugeTransChoice =
                m_arrLGauge[4][1].ref_GaugeTransChoice =
                 m_arrLGauge[4][2].ref_GaugeTransChoice =
                m_arrLGauge[4][3].ref_GaugeTransChoice =
                 m_arrLGauge[4][4].ref_GaugeTransChoice =
                m_arrLGauge[4][5].ref_GaugeTransChoice =
                m_arrLGauge[4][6].ref_GaugeTransChoice = m_intGaugeTransitionChoice;

            m_arrLGauge[0][0].ref_GaugeThickness =
              m_arrLGauge[0][1].ref_GaugeThickness =
              m_arrLGauge[0][2].ref_GaugeThickness =
              m_arrLGauge[0][3].ref_GaugeThickness =
              m_arrLGauge[0][4].ref_GaugeThickness =
              m_arrLGauge[0][5].ref_GaugeThickness =
              m_arrLGauge[0][6].ref_GaugeThickness =
              m_arrLGauge[0][7].ref_GaugeThickness =
              m_arrLGauge[0][8].ref_GaugeThickness =
              m_arrLGauge[0][9].ref_GaugeThickness =
              m_arrLGauge[0][10].ref_GaugeThickness =
              m_arrLGauge[0][11].ref_GaugeThickness =
              m_arrLGauge[0][12].ref_GaugeThickness =
              m_arrLGauge[0][13].ref_GaugeThickness =
              m_arrLGauge[0][14].ref_GaugeThickness =
              m_arrLGauge[0][15].ref_GaugeThickness =
              m_arrLGauge[0][16].ref_GaugeThickness =
              m_arrLGauge[0][17].ref_GaugeThickness =
              m_arrLGauge[1][0].ref_GaugeThickness =
              m_arrLGauge[1][1].ref_GaugeThickness =
                 m_arrLGauge[1][2].ref_GaugeThickness =
              m_arrLGauge[1][3].ref_GaugeThickness =
                 m_arrLGauge[1][4].ref_GaugeThickness =
              m_arrLGauge[1][5].ref_GaugeThickness =
              m_arrLGauge[1][6].ref_GaugeThickness =
              m_arrLGauge[2][0].ref_GaugeThickness =
              m_arrLGauge[2][1].ref_GaugeThickness =
                m_arrLGauge[2][2].ref_GaugeThickness =
              m_arrLGauge[2][3].ref_GaugeThickness =
                m_arrLGauge[2][4].ref_GaugeThickness =
              m_arrLGauge[2][5].ref_GaugeThickness =
              m_arrLGauge[2][6].ref_GaugeThickness =
              m_arrLGauge[3][0].ref_GaugeThickness =
              m_arrLGauge[3][1].ref_GaugeThickness =
                 m_arrLGauge[3][2].ref_GaugeThickness =
              m_arrLGauge[3][3].ref_GaugeThickness =
                 m_arrLGauge[3][4].ref_GaugeThickness =
              m_arrLGauge[3][5].ref_GaugeThickness =
              m_arrLGauge[3][6].ref_GaugeThickness =
              m_arrLGauge[4][0].ref_GaugeThickness =
              m_arrLGauge[4][1].ref_GaugeThickness =
                   m_arrLGauge[4][2].ref_GaugeThickness =
              m_arrLGauge[4][3].ref_GaugeThickness =
                   m_arrLGauge[4][4].ref_GaugeThickness =
              m_arrLGauge[4][5].ref_GaugeThickness =
              m_arrLGauge[4][6].ref_GaugeThickness = m_intGaugeThickness;

            m_arrLGauge[0][0].ref_GaugeThreshold =
              m_arrLGauge[0][1].ref_GaugeThreshold =
              m_arrLGauge[0][2].ref_GaugeThreshold =
              m_arrLGauge[0][3].ref_GaugeThreshold =
              m_arrLGauge[0][4].ref_GaugeThreshold =
              m_arrLGauge[0][5].ref_GaugeThreshold =
              m_arrLGauge[0][6].ref_GaugeThreshold =
              m_arrLGauge[0][7].ref_GaugeThreshold =
              m_arrLGauge[0][8].ref_GaugeThreshold =
              m_arrLGauge[0][9].ref_GaugeThreshold =
              m_arrLGauge[0][10].ref_GaugeThreshold =
              m_arrLGauge[0][11].ref_GaugeThreshold =
              m_arrLGauge[0][12].ref_GaugeThreshold =
              m_arrLGauge[0][13].ref_GaugeThreshold =
              m_arrLGauge[0][14].ref_GaugeThreshold =
              m_arrLGauge[0][15].ref_GaugeThreshold =
              m_arrLGauge[0][16].ref_GaugeThreshold =
              m_arrLGauge[0][17].ref_GaugeThreshold =
              m_arrLGauge[1][0].ref_GaugeThreshold =
              m_arrLGauge[1][1].ref_GaugeThreshold =
                m_arrLGauge[1][2].ref_GaugeThreshold =
              m_arrLGauge[1][3].ref_GaugeThreshold =
                m_arrLGauge[1][4].ref_GaugeThreshold =
              m_arrLGauge[1][5].ref_GaugeThreshold =
              m_arrLGauge[1][6].ref_GaugeThreshold =
              m_arrLGauge[2][0].ref_GaugeThreshold =
              m_arrLGauge[2][1].ref_GaugeThreshold =
                m_arrLGauge[2][2].ref_GaugeThreshold =
              m_arrLGauge[2][3].ref_GaugeThreshold =
                m_arrLGauge[2][4].ref_GaugeThreshold =
              m_arrLGauge[2][5].ref_GaugeThreshold =
                    m_arrLGauge[2][6].ref_GaugeThreshold =
              m_arrLGauge[3][0].ref_GaugeThreshold =
              m_arrLGauge[3][1].ref_GaugeThreshold =
                m_arrLGauge[3][2].ref_GaugeThreshold =
              m_arrLGauge[3][3].ref_GaugeThreshold =
                m_arrLGauge[3][4].ref_GaugeThreshold =
              m_arrLGauge[3][5].ref_GaugeThreshold =
                    m_arrLGauge[3][6].ref_GaugeThreshold =
              m_arrLGauge[4][0].ref_GaugeThreshold =
              m_arrLGauge[4][1].ref_GaugeThreshold =
                  m_arrLGauge[4][2].ref_GaugeThreshold =
              m_arrLGauge[4][3].ref_GaugeThreshold =
                  m_arrLGauge[4][4].ref_GaugeThreshold =
              m_arrLGauge[4][5].ref_GaugeThreshold =
                    m_arrLGauge[4][6].ref_GaugeThreshold = m_intGaugeThreshold;

            m_arrLGauge[0][0].ref_GaugeMinAmplitude =
              m_arrLGauge[0][1].ref_GaugeMinAmplitude =
              m_arrLGauge[0][2].ref_GaugeMinAmplitude =
              m_arrLGauge[0][3].ref_GaugeMinAmplitude =
              m_arrLGauge[0][4].ref_GaugeMinAmplitude =
              m_arrLGauge[0][5].ref_GaugeMinAmplitude =
              m_arrLGauge[0][6].ref_GaugeMinAmplitude =
              m_arrLGauge[0][7].ref_GaugeMinAmplitude =
              m_arrLGauge[0][8].ref_GaugeMinAmplitude =
              m_arrLGauge[0][9].ref_GaugeMinAmplitude =
              m_arrLGauge[0][10].ref_GaugeMinAmplitude =
              m_arrLGauge[0][11].ref_GaugeMinAmplitude =
              m_arrLGauge[0][12].ref_GaugeMinAmplitude =
              m_arrLGauge[0][13].ref_GaugeMinAmplitude =
              m_arrLGauge[0][14].ref_GaugeMinAmplitude =
              m_arrLGauge[0][15].ref_GaugeMinAmplitude =
              m_arrLGauge[0][16].ref_GaugeMinAmplitude =
              m_arrLGauge[0][17].ref_GaugeMinAmplitude =
              m_arrLGauge[1][0].ref_GaugeMinAmplitude =
              m_arrLGauge[1][1].ref_GaugeMinAmplitude =
               m_arrLGauge[1][2].ref_GaugeMinAmplitude =
              m_arrLGauge[1][3].ref_GaugeMinAmplitude =
               m_arrLGauge[1][4].ref_GaugeMinAmplitude =
              m_arrLGauge[1][5].ref_GaugeMinAmplitude =
                    m_arrLGauge[1][6].ref_GaugeMinAmplitude =
              m_arrLGauge[2][0].ref_GaugeMinAmplitude =
              m_arrLGauge[2][1].ref_GaugeMinAmplitude =
              m_arrLGauge[2][2].ref_GaugeMinAmplitude =
              m_arrLGauge[2][3].ref_GaugeMinAmplitude =
              m_arrLGauge[2][4].ref_GaugeMinAmplitude =
              m_arrLGauge[2][5].ref_GaugeMinAmplitude =
              m_arrLGauge[2][6].ref_GaugeMinAmplitude =
              m_arrLGauge[3][0].ref_GaugeMinAmplitude =
              m_arrLGauge[3][1].ref_GaugeMinAmplitude =
                m_arrLGauge[3][2].ref_GaugeMinAmplitude =
              m_arrLGauge[3][3].ref_GaugeMinAmplitude =
                m_arrLGauge[3][4].ref_GaugeMinAmplitude =
              m_arrLGauge[3][5].ref_GaugeMinAmplitude =
              m_arrLGauge[3][6].ref_GaugeMinAmplitude =
              m_arrLGauge[4][0].ref_GaugeMinAmplitude =
              m_arrLGauge[4][1].ref_GaugeMinAmplitude =
                  m_arrLGauge[4][2].ref_GaugeMinAmplitude =
              m_arrLGauge[4][3].ref_GaugeMinAmplitude =
                  m_arrLGauge[4][4].ref_GaugeMinAmplitude =
              m_arrLGauge[4][5].ref_GaugeMinAmplitude =
              m_arrLGauge[4][6].ref_GaugeMinAmplitude = m_intGaugeMinAmplitude;

            m_arrLGauge[0][0].ref_GaugeSamplingStep =
              m_arrLGauge[0][1].ref_GaugeSamplingStep =
              m_arrLGauge[0][2].ref_GaugeSamplingStep =
              m_arrLGauge[0][3].ref_GaugeSamplingStep =
              m_arrLGauge[0][4].ref_GaugeSamplingStep =
              m_arrLGauge[0][5].ref_GaugeSamplingStep =
              m_arrLGauge[0][6].ref_GaugeSamplingStep =
              m_arrLGauge[0][7].ref_GaugeSamplingStep =
              m_arrLGauge[0][8].ref_GaugeSamplingStep =
              m_arrLGauge[0][9].ref_GaugeSamplingStep =
              m_arrLGauge[0][10].ref_GaugeSamplingStep =
              m_arrLGauge[0][11].ref_GaugeSamplingStep =
              m_arrLGauge[0][12].ref_GaugeSamplingStep =
              m_arrLGauge[0][13].ref_GaugeSamplingStep =
              m_arrLGauge[0][14].ref_GaugeSamplingStep =
              m_arrLGauge[0][15].ref_GaugeSamplingStep =
              m_arrLGauge[0][16].ref_GaugeSamplingStep =
              m_arrLGauge[0][17].ref_GaugeSamplingStep =
              m_arrLGauge[1][0].ref_GaugeSamplingStep =
              m_arrLGauge[1][1].ref_GaugeSamplingStep =
                 m_arrLGauge[1][2].ref_GaugeSamplingStep =
              m_arrLGauge[1][3].ref_GaugeSamplingStep =
                 m_arrLGauge[1][4].ref_GaugeSamplingStep =
              m_arrLGauge[1][5].ref_GaugeSamplingStep =
              m_arrLGauge[1][6].ref_GaugeSamplingStep =
              m_arrLGauge[2][0].ref_GaugeSamplingStep =
              m_arrLGauge[2][1].ref_GaugeSamplingStep =
               m_arrLGauge[2][2].ref_GaugeSamplingStep =
              m_arrLGauge[2][3].ref_GaugeSamplingStep =
               m_arrLGauge[2][4].ref_GaugeSamplingStep =
              m_arrLGauge[2][5].ref_GaugeSamplingStep =
              m_arrLGauge[2][6].ref_GaugeSamplingStep =
              m_arrLGauge[3][0].ref_GaugeSamplingStep =
              m_arrLGauge[3][1].ref_GaugeSamplingStep =
               m_arrLGauge[3][2].ref_GaugeSamplingStep =
              m_arrLGauge[3][3].ref_GaugeSamplingStep =
               m_arrLGauge[3][4].ref_GaugeSamplingStep =
              m_arrLGauge[3][5].ref_GaugeSamplingStep =
              m_arrLGauge[3][6].ref_GaugeSamplingStep =
              m_arrLGauge[4][0].ref_GaugeSamplingStep =
              m_arrLGauge[4][1].ref_GaugeSamplingStep =
               m_arrLGauge[4][2].ref_GaugeSamplingStep =
              m_arrLGauge[4][3].ref_GaugeSamplingStep =
               m_arrLGauge[4][4].ref_GaugeSamplingStep =
              m_arrLGauge[4][5].ref_GaugeSamplingStep =
              m_arrLGauge[4][6].ref_GaugeSamplingStep = m_intGaugeSamplingSteps;

            m_arrLGauge[0][0].ref_GaugeMinArea =
              m_arrLGauge[0][1].ref_GaugeMinArea =
              m_arrLGauge[0][2].ref_GaugeMinArea =
              m_arrLGauge[0][3].ref_GaugeMinArea =
              m_arrLGauge[0][4].ref_GaugeMinArea =
              m_arrLGauge[0][5].ref_GaugeMinArea =
              m_arrLGauge[0][6].ref_GaugeMinArea =
              m_arrLGauge[0][7].ref_GaugeMinArea =
              m_arrLGauge[0][8].ref_GaugeMinArea =
              m_arrLGauge[0][9].ref_GaugeMinArea =
              m_arrLGauge[0][10].ref_GaugeMinArea =
              m_arrLGauge[0][11].ref_GaugeMinArea =
              m_arrLGauge[0][12].ref_GaugeMinArea =
              m_arrLGauge[0][13].ref_GaugeMinArea =
              m_arrLGauge[0][14].ref_GaugeMinArea =
              m_arrLGauge[0][15].ref_GaugeMinArea =
              m_arrLGauge[0][16].ref_GaugeMinArea =
              m_arrLGauge[0][17].ref_GaugeMinArea =
              m_arrLGauge[1][0].ref_GaugeMinArea =
              m_arrLGauge[1][1].ref_GaugeMinArea =
               m_arrLGauge[1][2].ref_GaugeMinArea =
              m_arrLGauge[1][3].ref_GaugeMinArea =
               m_arrLGauge[1][4].ref_GaugeMinArea =
              m_arrLGauge[1][5].ref_GaugeMinArea =
              m_arrLGauge[1][6].ref_GaugeMinArea =
              m_arrLGauge[2][0].ref_GaugeMinArea =
              m_arrLGauge[2][1].ref_GaugeMinArea =
                 m_arrLGauge[2][2].ref_GaugeMinArea =
              m_arrLGauge[2][3].ref_GaugeMinArea =
                 m_arrLGauge[2][4].ref_GaugeMinArea =
              m_arrLGauge[2][5].ref_GaugeMinArea =
              m_arrLGauge[2][6].ref_GaugeMinArea =
              m_arrLGauge[3][0].ref_GaugeMinArea =
              m_arrLGauge[3][1].ref_GaugeMinArea =
                  m_arrLGauge[3][2].ref_GaugeMinArea =
              m_arrLGauge[3][3].ref_GaugeMinArea =
                  m_arrLGauge[3][4].ref_GaugeMinArea =
              m_arrLGauge[3][5].ref_GaugeMinArea =
              m_arrLGauge[3][6].ref_GaugeMinArea =
              m_arrLGauge[4][0].ref_GaugeMinArea =
              m_arrLGauge[4][1].ref_GaugeMinArea =
                 m_arrLGauge[4][2].ref_GaugeMinArea =
              m_arrLGauge[4][3].ref_GaugeMinArea =
                 m_arrLGauge[4][4].ref_GaugeMinArea =
              m_arrLGauge[4][5].ref_GaugeMinArea =
              m_arrLGauge[4][6].ref_GaugeMinArea = m_intGaugeMinArea;

            m_arrLGauge[0][0].ref_GaugeFilter =
              m_arrLGauge[0][1].ref_GaugeFilter =
              m_arrLGauge[0][2].ref_GaugeFilter =
              m_arrLGauge[0][3].ref_GaugeFilter =
              m_arrLGauge[0][4].ref_GaugeFilter =
              m_arrLGauge[0][5].ref_GaugeFilter =
              m_arrLGauge[0][6].ref_GaugeFilter =
              m_arrLGauge[0][7].ref_GaugeFilter =
              m_arrLGauge[0][8].ref_GaugeFilter =
              m_arrLGauge[0][9].ref_GaugeFilter =
              m_arrLGauge[0][10].ref_GaugeFilter =
              m_arrLGauge[0][11].ref_GaugeFilter =
              m_arrLGauge[0][12].ref_GaugeFilter =
              m_arrLGauge[0][13].ref_GaugeFilter =
              m_arrLGauge[0][14].ref_GaugeFilter =
              m_arrLGauge[0][15].ref_GaugeFilter =
              m_arrLGauge[0][16].ref_GaugeFilter =
              m_arrLGauge[0][17].ref_GaugeFilter =
              m_arrLGauge[1][0].ref_GaugeFilter =
              m_arrLGauge[1][1].ref_GaugeFilter =
               m_arrLGauge[1][2].ref_GaugeFilter =
              m_arrLGauge[1][3].ref_GaugeFilter =
               m_arrLGauge[1][4].ref_GaugeFilter =
              m_arrLGauge[1][5].ref_GaugeFilter =
              m_arrLGauge[1][6].ref_GaugeFilter =
              m_arrLGauge[2][0].ref_GaugeFilter =
              m_arrLGauge[2][1].ref_GaugeFilter =
                m_arrLGauge[2][2].ref_GaugeFilter =
              m_arrLGauge[2][3].ref_GaugeFilter =
                m_arrLGauge[2][4].ref_GaugeFilter =
              m_arrLGauge[2][5].ref_GaugeFilter =
              m_arrLGauge[2][6].ref_GaugeFilter =
              m_arrLGauge[3][0].ref_GaugeFilter =
              m_arrLGauge[3][1].ref_GaugeFilter =
              m_arrLGauge[3][2].ref_GaugeFilter =
              m_arrLGauge[3][3].ref_GaugeFilter =
              m_arrLGauge[3][4].ref_GaugeFilter =
              m_arrLGauge[3][5].ref_GaugeFilter =
              m_arrLGauge[3][6].ref_GaugeFilter =
              m_arrLGauge[4][0].ref_GaugeFilter =
              m_arrLGauge[4][1].ref_GaugeFilter =
               m_arrLGauge[4][2].ref_GaugeFilter =
              m_arrLGauge[4][3].ref_GaugeFilter =
               m_arrLGauge[4][4].ref_GaugeFilter =
              m_arrLGauge[4][5].ref_GaugeFilter =
              m_arrLGauge[4][6].ref_GaugeFilter = m_intGaugeFilter;

            m_arrLGauge[0][0].ref_GaugeFilterThreshold =
              m_arrLGauge[0][1].ref_GaugeFilterThreshold =
              m_arrLGauge[0][2].ref_GaugeFilterThreshold =
              m_arrLGauge[0][3].ref_GaugeFilterThreshold =
              m_arrLGauge[0][4].ref_GaugeFilterThreshold =
              m_arrLGauge[0][5].ref_GaugeFilterThreshold =
              m_arrLGauge[0][6].ref_GaugeFilterThreshold =
              m_arrLGauge[0][7].ref_GaugeFilterThreshold =
              m_arrLGauge[0][8].ref_GaugeFilterThreshold =
              m_arrLGauge[0][9].ref_GaugeFilterThreshold =
              m_arrLGauge[0][10].ref_GaugeFilterThreshold =
              m_arrLGauge[0][11].ref_GaugeFilterThreshold =
              m_arrLGauge[0][12].ref_GaugeFilterThreshold =
              m_arrLGauge[0][13].ref_GaugeFilterThreshold =
              m_arrLGauge[0][14].ref_GaugeFilterThreshold =
              m_arrLGauge[0][15].ref_GaugeFilterThreshold =
              m_arrLGauge[0][16].ref_GaugeFilterThreshold =
              m_arrLGauge[0][17].ref_GaugeFilterThreshold =
              m_arrLGauge[1][0].ref_GaugeFilterThreshold =
              m_arrLGauge[1][1].ref_GaugeFilterThreshold =
                m_arrLGauge[1][2].ref_GaugeFilterThreshold =
              m_arrLGauge[1][3].ref_GaugeFilterThreshold =
                m_arrLGauge[1][4].ref_GaugeFilterThreshold =
              m_arrLGauge[1][5].ref_GaugeFilterThreshold =
            m_arrLGauge[1][6].ref_GaugeFilterThreshold =
              m_arrLGauge[2][0].ref_GaugeFilterThreshold =
              m_arrLGauge[2][1].ref_GaugeFilterThreshold =
                  m_arrLGauge[2][2].ref_GaugeFilterThreshold =
              m_arrLGauge[2][3].ref_GaugeFilterThreshold =
                  m_arrLGauge[2][4].ref_GaugeFilterThreshold =
              m_arrLGauge[2][5].ref_GaugeFilterThreshold =
              m_arrLGauge[2][6].ref_GaugeFilterThreshold =
              m_arrLGauge[3][0].ref_GaugeFilterThreshold =
              m_arrLGauge[3][1].ref_GaugeFilterThreshold =
               m_arrLGauge[3][2].ref_GaugeFilterThreshold =
              m_arrLGauge[3][3].ref_GaugeFilterThreshold =
               m_arrLGauge[3][4].ref_GaugeFilterThreshold =
              m_arrLGauge[3][5].ref_GaugeFilterThreshold =
              m_arrLGauge[3][6].ref_GaugeFilterThreshold =
              m_arrLGauge[4][0].ref_GaugeFilterThreshold =
              m_arrLGauge[4][1].ref_GaugeFilterThreshold =
               m_arrLGauge[4][2].ref_GaugeFilterThreshold =
              m_arrLGauge[4][3].ref_GaugeFilterThreshold =
               m_arrLGauge[4][4].ref_GaugeFilterThreshold =
              m_arrLGauge[4][5].ref_GaugeFilterThreshold =
              m_arrLGauge[4][6].ref_GaugeFilterThreshold = m_fGaugeFilterThreshold;
            
            m_arrLGauge[0][0].ref_GaugeFilterPasses =
              m_arrLGauge[0][1].ref_GaugeFilterPasses =
              m_arrLGauge[0][2].ref_GaugeFilterPasses =
              m_arrLGauge[0][3].ref_GaugeFilterPasses =
              m_arrLGauge[0][4].ref_GaugeFilterPasses =
              m_arrLGauge[0][5].ref_GaugeFilterPasses =
              m_arrLGauge[0][6].ref_GaugeFilterPasses =
              m_arrLGauge[0][7].ref_GaugeFilterPasses =
              m_arrLGauge[0][8].ref_GaugeFilterPasses =
              m_arrLGauge[0][9].ref_GaugeFilterPasses =
              m_arrLGauge[0][10].ref_GaugeFilterPasses =
              m_arrLGauge[0][11].ref_GaugeFilterPasses =
              m_arrLGauge[0][12].ref_GaugeFilterPasses =
              m_arrLGauge[0][13].ref_GaugeFilterPasses =
              m_arrLGauge[0][14].ref_GaugeFilterPasses =
              m_arrLGauge[0][15].ref_GaugeFilterPasses =
              m_arrLGauge[0][16].ref_GaugeFilterPasses =
              m_arrLGauge[0][17].ref_GaugeFilterPasses =
              m_arrLGauge[1][0].ref_GaugeFilterPasses =
              m_arrLGauge[1][1].ref_GaugeFilterPasses =
               m_arrLGauge[1][2].ref_GaugeFilterPasses =
              m_arrLGauge[1][3].ref_GaugeFilterPasses =
               m_arrLGauge[1][4].ref_GaugeFilterPasses =
              m_arrLGauge[1][5].ref_GaugeFilterPasses =
              m_arrLGauge[1][6].ref_GaugeFilterPasses =
              m_arrLGauge[2][0].ref_GaugeFilterPasses =
              m_arrLGauge[2][1].ref_GaugeFilterPasses =
              m_arrLGauge[2][2].ref_GaugeFilterPasses =
              m_arrLGauge[2][3].ref_GaugeFilterPasses =
              m_arrLGauge[2][4].ref_GaugeFilterPasses =
              m_arrLGauge[2][5].ref_GaugeFilterPasses =
              m_arrLGauge[2][6].ref_GaugeFilterPasses =
              m_arrLGauge[3][0].ref_GaugeFilterPasses =
              m_arrLGauge[3][1].ref_GaugeFilterPasses =
                m_arrLGauge[3][2].ref_GaugeFilterPasses =
              m_arrLGauge[3][3].ref_GaugeFilterPasses =
                m_arrLGauge[3][4].ref_GaugeFilterPasses =
              m_arrLGauge[3][5].ref_GaugeFilterPasses =
              m_arrLGauge[3][6].ref_GaugeFilterPasses =
              m_arrLGauge[4][0].ref_GaugeFilterPasses =
              m_arrLGauge[4][1].ref_GaugeFilterPasses =
               m_arrLGauge[4][2].ref_GaugeFilterPasses =
              m_arrLGauge[4][3].ref_GaugeFilterPasses =
               m_arrLGauge[4][4].ref_GaugeFilterPasses =
              m_arrLGauge[4][5].ref_GaugeFilterPasses =
              m_arrLGauge[4][6].ref_GaugeFilterPasses = m_intGaugeFilterPass;
  
        }
        public void DragROI(int intPositionX, int intPositionY, int m_intClickedPad0T, int m_intClickedPad0R, int m_intClickedPad0B, int m_intClickedPad0L, int m_intClickedPad1B, int m_intClickedPad2L, int m_intClickedPad3T, int m_intClickedPad4R)
        {
            int intPositionX0, intPositionX1, intPositionX2, intPositionX3, intPositionX4;
            intPositionX0 = intPositionX1 = intPositionX2 = intPositionX3 = intPositionX4 = intPositionX;
            int intPositionY0, intPositionY1, intPositionY2, intPositionY3, intPositionY4;
            intPositionY0 = intPositionY1 = intPositionY2 = intPositionY3 = intPositionY4 = intPositionY;

            for (int i = 0; i < m_arrROIs.Count; i++)
            {
                if (m_arrROIs.Count > 1)
                {
                    if (i == 0)
                    {
                        if (intPositionX0 + m_intClickedPad0R > (m_arrROIs[2].ref_ROITotalX))
                        {
                            intPositionX0 = m_arrROIs[2].ref_ROITotalX - m_intClickedPad0R;
                        }
                        if (intPositionX0 - m_intClickedPad0L < (m_arrROIs[4].ref_ROITotalX + m_arrROIs[4].ref_ROIWidth))
                        {

                            intPositionX0 = (m_arrROIs[4].ref_ROITotalX + m_arrROIs[4].ref_ROIWidth) + m_intClickedPad0L;

                        }
                        if (intPositionY0 + m_intClickedPad0B > (m_arrROIs[3].ref_ROITotalY))
                        {
                            intPositionY0 = m_arrROIs[3].ref_ROITotalY - m_intClickedPad0B;
                        }
                        if (intPositionY0 - m_intClickedPad0T < (m_arrROIs[1].ref_ROITotalY + m_arrROIs[1].ref_ROIHeight))
                        {

                            intPositionY0 = (m_arrROIs[1].ref_ROITotalY + m_arrROIs[1].ref_ROIHeight) + m_intClickedPad0T;

                        }
                    }
                    else if (i == 1)
                    {

                        if (intPositionY1 + m_intClickedPad1B > (m_arrROIs[0].ref_ROITotalY))
                        {
                            intPositionY1 = m_arrROIs[0].ref_ROITotalY - m_intClickedPad1B;
                        }

                    }
                    else if (i == 2)
                    {

                        if (intPositionX2 - m_intClickedPad2L < (m_arrROIs[0].ref_ROITotalX + m_arrROIs[0].ref_ROIWidth))
                        {

                            intPositionX2 = (m_arrROIs[0].ref_ROITotalX + m_arrROIs[0].ref_ROIWidth) + m_intClickedPad2L;

                        }

                    }
                    else if (i == 3)
                    {

                        if (intPositionY3 - m_intClickedPad3T < (m_arrROIs[0].ref_ROITotalY + m_arrROIs[0].ref_ROIHeight))
                        {

                            intPositionY3 = (m_arrROIs[0].ref_ROITotalY + m_arrROIs[0].ref_ROIHeight) + m_intClickedPad3T;

                        }
                    }
                    else if (i == 4)
                    {
                        if (intPositionX4 + m_intClickedPad4R > (m_arrROIs[0].ref_ROITotalX))
                        {
                            intPositionX4 = m_arrROIs[0].ref_ROITotalX - m_intClickedPad4R;
                        }

                    }
                }

                if (m_arrROIs[i].GetROIHandle())
                {
                    if (i == 0)
                        m_arrROIs[i].DragROI(intPositionX0, intPositionY0);
                    else if (i == 1)
                        m_arrROIs[i].DragROI(intPositionX1, intPositionY1);
                    else if (i == 2)
                        m_arrROIs[i].DragROI(intPositionX2, intPositionY2);
                    else if (i == 3)
                        m_arrROIs[i].DragROI(intPositionX3, intPositionY3);
                    else if (i == 4)
                        m_arrROIs[i].DragROI(intPositionX4, intPositionY4);
                    //m_arrRectGauge4L[i].SetGaugePlace(m_arrROIs[i]);
                    break;
                }
            }

        }

        public void VerifyROIHandleShape(int intPositionX, int intPositionY)
        {
            for (int i = 0; i < m_arrROIs.Count; i++)
            {
                if (m_blnCursorShapeVerifying && m_intSelectedROIPrev != i)
                    continue;

                m_arrROIs[i].VerifyROIHandleShape(intPositionX, intPositionY);

                m_intSelectedROIPrev = i;
                m_blnCursorShapeVerifying = m_arrROIs[i].GetROIHandle2();
            }
        }

        public void DrawCalibrationROI(Graphics g, float fScaleX, float fScaleY)
        {
            for (int i = 0; i < m_arrROIs.Count; i++)
            {
                if (m_arrROIs[i].GetROIHandle())
                    m_arrROIs[i].DrawROI(g, fScaleX, fScaleY, true, 0);
                else
                    m_arrROIs[i].DrawROI(g, fScaleX, fScaleY, false, 0);
              
                    //if (i < m_arrRectGauge4L.Count)
                    //{
                    //    if (m_blnDrawSamplingPoint)
                    //        m_arrRectGauge4L[i].DrawGaugeResult_SamplingPoint(g);
                    //    if (m_blnDrawDraggingBox)
                    //        m_arrRectGauge4L[i].DrawGaugeSetting_Inside(g, fScaleX, fScaleY);
                    //    else
                    //        m_arrRectGauge4L[i].DrawGaugeResult_ResultLine(g, fScaleX, fScaleX);
                    //}
                
               
            }
        }

        public int GetSelectedROI()
        {
            for (int i = 0; i < m_arrROIs.Count; i++)
            {
                if (m_arrROIs[i].GetROIHandle())
                {
                    return i;
                }
            }

            return -1;
        }

        public bool MeasureTopBlob(ImageDrawing objImage, bool blnUpdateData)
        {
            // B = Blob, L = Line Gauge
            //            ___L0__         ____L3____               _______L6______
            //       L1  |   B0  |L2   L4|    B1    |L5         L7|       B2      |L8
            //           |       |_______|          |             |               | 
            //           |                          |             |               |
            //           |                          |             |               |
            //           |                          |_____________|               |
            //           |                                                        |
            //           |                                                        |

            m_blnBlobOK_Top = false;
            m_objEBlobs_Top.CleanAllBlobs();

            m_objEBlobs_Top.BuildObjects_Filter_GetElement(m_arrROIs[1], true, true, 0, m_intThresholdTop,
                20, m_arrROIs[1].ref_ROIWidth* m_arrROIs[1].ref_ROIHeight, false, 0xFF);
            //m_arrROIs[1].SaveImage("D:\\TS\\TopROI.bmp");
            if (m_objEBlobs_Top.ref_intNumSelectedObject == 0)
            {
                m_strErrorMessage = "*Fail to build Top ROI object - Please adjust ROI or change threshold setting.";
                return false;
            }

            //int intParentBlobHeight = (int)Math.Round(m_objEBlobs_Top.ref_arrHeight[0]);
            //int intParentBlobCenterY = (int)Math.Round(m_objEBlobs_Top.ref_arrLimitCenterY[0]);

            //ROI objROI = new ROI();
            //objROI.LoadROISetting(m_arrROIs[1].ref_ROIPositionX, m_arrROIs[1].ref_ROIPositionY, m_arrROIs[1].ref_ROIWidth, m_arrROIs[1].ref_ROIHeight - (int)Math.Round(m_objEBlobs_Top.ref_arrHeight[0] - 10));
            //objROI.AttachImage(objImage);
            ////objROI.SaveImage("D:\\TS\\objROI.bmp");
            //m_objEBlobs_Top.CleanAllBlobs();
            //m_objEBlobs_Top.BuildObjects_Filter_GetElement_SortByX(objROI, true, true, 0, m_intThresholdTop,
            //    20, objROI.ref_ROIWidth * objROI.ref_ROIHeight, false, 0xFF, true);

            //if (m_objEBlobs_Top.ref_intNumSelectedObject == 0)
            //{
            //    objROI.LoadROISetting(objROI.ref_ROIPositionX, objROI.ref_ROIPositionY, objROI.ref_ROIWidth, objROI.ref_ROIHeight + 10);

            //    m_objEBlobs_Top.CleanAllBlobs();
            //    m_objEBlobs_Top.BuildObjects_Filter_GetElement_SortByX(objROI, true, true, 0, m_intThresholdTop,
            //        20, objROI.ref_ROIWidth * objROI.ref_ROIHeight, false, 0xFF, true);
            //}

            //if (m_objEBlobs_Top.ref_intNumSelectedObject != 3)
            //{
            //    objROI.Dispose();
            //    m_strErrorMessage = "*Fail to build Top ROI object - Please adjust ROI or change threshold setting.";
            //    return false;
            //}

            // Scan blob to find 3 tip blob.
            bool blnFoundTip = false;
            float fParentBlobHeight = m_objEBlobs_Top.ref_arrHeight[0];
            ROI objROI = new ROI();
            for (int intThickness = 15; intThickness < fParentBlobHeight; intThickness++) // 2021-07-16 ZJYEOH : Change Max build pixel from 50 to fParentBlobHeight
            {
                objROI.LoadROISetting(m_arrROIs[1].ref_ROIPositionX, 
                                      m_arrROIs[1].ref_ROIPositionY, 
                                      m_arrROIs[1].ref_ROIWidth,
                                      m_arrROIs[1].ref_ROIHeight - (int)Math.Round(fParentBlobHeight - intThickness));

                objROI.AttachImage(objImage);
                m_objEBlobs_Top.CleanAllBlobs();
                m_objEBlobs_Top.BuildObjects_Filter_GetElement_SortByX(objROI, true, true, 0, m_intThresholdTop,
                    20, objROI.ref_ROIWidth * objROI.ref_ROIHeight, false, 0xFF, true);
                
                if (m_objEBlobs_Top.ref_intNumSelectedObject >= 3)
                {
                    blnFoundTip = true;
                    break;
                }
            }

            if (!blnFoundTip)
            {
                objROI.Dispose();
                m_strErrorMessage = "*Fail to build Top ROI object - Please adjust ROI or change threshold setting.";
                return false;
            }

            m_blnBlobOK_Top = true;
            //m_arrROIs[1].ThresholdTo_ROIToROISamePosition(ref objImage, m_intThresholdTop);

            if (m_arrLGauge[1].Count == 0)
                m_arrLGauge[1].Add(new LGauge());//Smallest blob center
            if (m_arrLGauge[1].Count == 1)
                m_arrLGauge[1].Add(new LGauge());//Smallest blob left
            if (m_arrLGauge[1].Count == 2)
                m_arrLGauge[1].Add(new LGauge());//Smallest blob right

            m_arrLGauge[1][0].ref_GaugeAngle = 0;
            m_arrLGauge[1][0].ref_GaugeTransType = 1;
            //m_arrLGauge[1][0].ref_GaugeTransChoice =  m_intGaugeTransitionChoice;
            //m_arrLGauge[1][0].ref_GaugeThickness =  m_intGaugeThickness;
            //m_arrLGauge[1][0].ref_GaugeThreshold = m_intGaugeThreshold;
            //m_arrLGauge[1][0].ref_GaugeMinAmplitude = m_intGaugeMinAmplitude;
            //m_arrLGauge[1][0].ref_GaugeSamplingStep = m_intGaugeSamplingSteps;
            //m_arrLGauge[1][0].ref_GaugeMinArea = m_intGaugeMinArea;
            //m_arrLGauge[1][0].ref_GaugeFilter = m_intGaugeFilter;
            //m_arrLGauge[1][0].ref_GaugeFilterThreshold = m_intGaugeFilterThreshold;
            //m_arrLGauge[1][0].ref_GaugeFilterPasses = m_intGaugeFilterPass;
            m_arrLGauge[1][0].ref_GaugeTolerance = Math.Max(5, m_objEBlobs_Top.ref_arrHeight[0]);
            m_arrLGauge[1][0].ref_GaugeLength = Math.Max(5, m_objEBlobs_Top.ref_arrWidth[0] / 2);

            m_arrLGauge[1][0].SetGaugeCenter(m_arrROIs[1].ref_ROIPositionX + m_objEBlobs_Top.ref_arrLimitCenterX[0], m_arrROIs[1].ref_ROIPositionY + (m_objEBlobs_Top.ref_arrLimitCenterY[0] - (m_objEBlobs_Top.ref_arrHeight[0] / 2)));
        
            m_arrLGauge[1][0].Measure(m_arrROIs[1]);

            if(m_arrLGauge[1][0].ref_ObjectScore == 0)
            {
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Top ROI point - Please adjust gauge setting.";
                return false;
            }
            
            m_arrLGauge[1][1].ref_GaugeAngle = -90;
            m_arrLGauge[1][1].ref_GaugeTransType = 1;
            m_arrLGauge[1][1].ref_GaugeTolerance = Math.Max(5, Math.Max(5, m_objEBlobs_Top.ref_arrWidth[0] / 2));
            m_arrLGauge[1][1].ref_GaugeLength = Math.Max(5, m_objEBlobs_Top.ref_arrHeight[0]);

            m_arrLGauge[1][1].SetGaugeCenter(m_arrROIs[1].ref_ROIPositionX + (m_objEBlobs_Top.ref_arrLimitCenterX[0] - (m_objEBlobs_Top.ref_arrWidth[0] / 2)), m_arrLGauge[1][0].ref_ObjectCenterY + 5);

            m_arrLGauge[1][1].Measure(m_arrROIs[1]);

            if (m_arrLGauge[1][1].ref_ObjectScore == 0)
            {
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Top ROI point - Please adjust gauge setting.";
                return false;
            }

            m_arrLGauge[1][2].ref_GaugeAngle = 90;
            m_arrLGauge[1][2].ref_GaugeTransType = 1;
            m_arrLGauge[1][2].ref_GaugeTolerance = Math.Max(5, m_objEBlobs_Top.ref_arrWidth[0] / 2);
            m_arrLGauge[1][2].ref_GaugeLength = Math.Max(5, m_objEBlobs_Top.ref_arrHeight[0]);

            m_arrLGauge[1][2].SetGaugeCenter(m_arrROIs[1].ref_ROIPositionX + (m_objEBlobs_Top.ref_arrLimitCenterX[0] + (m_objEBlobs_Top.ref_arrWidth[0] / 2)), m_arrLGauge[1][0].ref_ObjectCenterY + 5);

            m_arrLGauge[1][2].Measure(m_arrROIs[1]);

            if (m_arrLGauge[1][2].ref_ObjectScore == 0)
            {
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Top ROI point - Please adjust gauge setting.";
                return false;
            }

            if (blnUpdateData)
            {
                if (m_fRotateAngleSide != 0)
                    m_f3DTopPixelCount1 = FixAngle(m_fRotateAngleSide, 1, 2, 1, true);
                else
                    m_f3DTopPixelCount1 = m_arrLGauge[1][2].ref_ObjectCenterX - m_arrLGauge[1][1].ref_ObjectCenterX;
            }

            if (m_arrLGauge[1].Count == 3)
                m_arrLGauge[1].Add(new LGauge());//Middle blob center
            if (m_arrLGauge[1].Count == 4)
                m_arrLGauge[1].Add(new LGauge());//Middle blob left
            if (m_arrLGauge[1].Count == 5)
                m_arrLGauge[1].Add(new LGauge());//Middle blob right

            m_arrLGauge[1][3].ref_GaugeAngle = 0;
            m_arrLGauge[1][3].ref_GaugeTransType = 1;
            //m_arrLGauge[1][3].ref_GaugeTransChoice =  m_intGaugeTransitionChoice;
            //m_arrLGauge[1][3].ref_GaugeThickness =  m_intGaugeThickness;
            //m_arrLGauge[1][3].ref_GaugeThreshold = m_intGaugeThreshold;
            //m_arrLGauge[1][3].ref_GaugeMinAmplitude = m_intGaugeMinAmplitude;
            //m_arrLGauge[1][3].ref_GaugeSamplingStep = m_intGaugeSamplingSteps;
            //m_arrLGauge[1][3].ref_GaugeMinArea = m_intGaugeMinArea;
            //m_arrLGauge[1][3].ref_GaugeFilter = m_intGaugeFilter;
            //m_arrLGauge[1][3].ref_GaugeFilterThreshold = m_intGaugeFilterThreshold;
            //m_arrLGauge[1][3].ref_GaugeFilterPasses = m_intGaugeFilterPass;
            m_arrLGauge[1][3].ref_GaugeTolerance = Math.Max(5, m_objEBlobs_Top.ref_arrHeight[1]);
            m_arrLGauge[1][3].ref_GaugeLength = Math.Max(5, m_objEBlobs_Top.ref_arrWidth[1] / 2);

            m_arrLGauge[1][3].SetGaugeCenter(m_arrROIs[1].ref_ROIPositionX + m_objEBlobs_Top.ref_arrLimitCenterX[1], m_arrROIs[1].ref_ROIPositionY + (m_objEBlobs_Top.ref_arrLimitCenterY[1] - (m_objEBlobs_Top.ref_arrHeight[1] / 2)));

            m_arrLGauge[1][3].Measure(m_arrROIs[1]);

            if (m_arrLGauge[1][3].ref_ObjectScore == 0)
            {
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Top ROI point - Please adjust gauge setting.";
                return false;
            }

            m_arrLGauge[1][4].ref_GaugeAngle = -90;
            m_arrLGauge[1][4].ref_GaugeTransType = 1;
            m_arrLGauge[1][4].ref_GaugeTolerance = Math.Max(5, m_objEBlobs_Top.ref_arrWidth[1] / 2);
            m_arrLGauge[1][4].ref_GaugeLength = Math.Max(5, m_objEBlobs_Top.ref_arrHeight[1]);

            m_arrLGauge[1][4].SetGaugeCenter(m_arrROIs[1].ref_ROIPositionX + (m_objEBlobs_Top.ref_arrLimitCenterX[1] - (m_objEBlobs_Top.ref_arrWidth[1] / 2)), m_arrLGauge[1][3].ref_ObjectCenterY + 5);

            m_arrLGauge[1][4].Measure(m_arrROIs[1]);

            if (m_arrLGauge[1][4].ref_ObjectScore == 0)
            {
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Top ROI point - Please adjust gauge setting.";
                return false;
            }

            m_arrLGauge[1][5].ref_GaugeAngle = 90;
            m_arrLGauge[1][5].ref_GaugeTransType = 1;
            m_arrLGauge[1][5].ref_GaugeTolerance = Math.Max(5, m_objEBlobs_Top.ref_arrWidth[1] / 2);
            m_arrLGauge[1][5].ref_GaugeLength = Math.Max(5, m_objEBlobs_Top.ref_arrHeight[1]);

            m_arrLGauge[1][5].SetGaugeCenter(m_arrROIs[1].ref_ROIPositionX + (m_objEBlobs_Top.ref_arrLimitCenterX[1] + (m_objEBlobs_Top.ref_arrWidth[1] / 2)), m_arrLGauge[1][3].ref_ObjectCenterY + 5);

            m_arrLGauge[1][5].Measure(m_arrROIs[1]);

            if (m_arrLGauge[1][5].ref_ObjectScore == 0)
            {
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Top ROI point - Please adjust gauge setting.";
                return false;
            }

            if (m_arrLGauge[1].Count == 6)
                m_arrLGauge[1].Add(new LGauge());//Largest blob center

            m_arrLGauge[1][6].ref_GaugeAngle = 0;
            m_arrLGauge[1][6].ref_GaugeTransType = 1;
            m_arrLGauge[1][6].ref_GaugeTolerance = Math.Max(5, m_objEBlobs_Top.ref_arrHeight[2]);
            m_arrLGauge[1][6].ref_GaugeLength = Math.Max(5, m_objEBlobs_Top.ref_arrWidth[2] / 2);

            m_arrLGauge[1][6].SetGaugeCenter(m_arrROIs[1].ref_ROIPositionX + m_objEBlobs_Top.ref_arrLimitCenterX[2], m_arrROIs[1].ref_ROIPositionY + (m_objEBlobs_Top.ref_arrLimitCenterY[2] - (m_objEBlobs_Top.ref_arrHeight[2] / 2)));

            m_arrLGauge[1][6].Measure(m_arrROIs[1]);

            if (m_arrLGauge[1][6].ref_ObjectScore == 0)
            {
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Top ROI point - Please adjust gauge setting.";
                return false;
            }

            if (blnUpdateData)
            {
                if (m_fRotateAngleSide != 0)
                    m_f3DTopPixelCount2 = FixAngle(m_fRotateAngleSide, 1, 5, 4, true);
                else
                    m_f3DTopPixelCount2 = m_arrLGauge[1][5].ref_ObjectCenterX - m_arrLGauge[1][4].ref_ObjectCenterX;

                m_fCalibration3DTop = m_f3DTopPixelCount2 / m_fSize3DT2;
            }
            //m_fTopDistance = m_arrLGauge[1][0].ref_ObjectCenterY - m_arrLGauge[1][6].ref_ObjectCenterY;

            //m_fTopDistance = m_fTopDistance * (m_fSize3DT1 / m_f3DTopPixelCount1);

            //m_fTopAngle = (float)(Math.Asin(m_fTopDistance / m_fSize2DA1) * 180 / Math.PI);

            m_arrBlobTop.Clear();
            for (int i = 0; i < m_objEBlobs_Top.ref_intNumSelectedObject; i++)
            {

                m_stcBlobTop = new BlobsFeatures();

                m_stcBlobTop.fCenterX = m_arrROIs[1].ref_ROIPositionX + m_objEBlobs_Top.ref_arrLimitCenterX[i];
                m_stcBlobTop.fCenterY = m_arrROIs[1].ref_ROIPositionY + m_objEBlobs_Top.ref_arrLimitCenterY[i];
                m_stcBlobTop.fWidth = m_objEBlobs_Top.ref_arrWidth[i];
                m_stcBlobTop.fHeight = m_objEBlobs_Top.ref_arrHeight[i];
                m_arrBlobTop.Add(m_stcBlobTop);
            }
            objROI.Dispose();
            return true;
        }
        public bool MeasureBottomBlob(ImageDrawing objImage, bool blnUpdateData)
        {
            m_blnBlobOK_Bottom = false;
            m_objEBlobs_Bottom.CleanAllBlobs();

            m_objEBlobs_Bottom.BuildObjects_Filter_GetElement(m_arrROIs[3], true, true, 0, m_intThresholdBottom,
                20, m_arrROIs[3].ref_ROIWidth * m_arrROIs[3].ref_ROIHeight, false, 0xFF);
            //m_arrROIs[3].SaveImage("D:\\TS\\BottomROI.bmp");
            if (m_objEBlobs_Bottom.ref_intNumSelectedObject == 0)
            {
                m_strErrorMessage = "*Fail to build Bottom ROI object - Please adjust ROI or change threshold setting.";
                return false;
            }

            //int intParentBlobHeight = (int)Math.Round(m_objEBlobs_Bottom.ref_arrHeight[0]);
            //int intParentBlobCenterY = (int)Math.Round(m_objEBlobs_Bottom.ref_arrLimitCenterY[0]);

            //objROI.LoadROISetting(m_arrROIs[3].ref_ROIPositionX, 
            //                      m_arrROIs[3].ref_ROIPositionY + (int)Math.Round((m_objEBlobs_Bottom.ref_arrLimitCenterY[0] + (m_objEBlobs_Bottom.ref_arrHeight[0] / 2)) - 10), 
            //                      m_arrROIs[3].ref_ROIWidth, 
            //                      m_arrROIs[3].ref_ROIHeight -  (int)Math.Round(m_objEBlobs_Bottom.ref_arrHeight[0] - 10));
            //objROI.AttachImage(objImage);
            //objROI.SaveImage("D:\\TS\\objROI.bmp");
            //m_objEBlobs_Bottom.CleanAllBlobs();
            //m_objEBlobs_Bottom.BuildObjects_Filter_GetElement_SortByX(objROI, true, true, 0, m_intThresholdBottom,
            //    20, objROI.ref_ROIWidth * objROI.ref_ROIHeight, false, 0xFF, false);

            //if (m_objEBlobs_Bottom.ref_intNumSelectedObject == 0)
            //{
            //    objROI.LoadROISetting(objROI.ref_ROIPositionX, objROI.ref_ROIPositionY, objROI.ref_ROIWidth, objROI.ref_ROIHeight - 10);
            //    objROI.SaveImage("D:\\TS\\objROI2.bmp");
            //    m_objEBlobs_Bottom.CleanAllBlobs();
            //    m_objEBlobs_Bottom.BuildObjects_Filter_GetElement_SortByX(objROI, true, true, 0, m_intThresholdBottom,
            //        20, objROI.ref_ROIWidth * objROI.ref_ROIHeight, false, 0xFF, false);
            //}

            //if (m_objEBlobs_Bottom.ref_intNumSelectedObject != 3)
            //{
            //    objROI.Dispose();
            //    m_strErrorMessage = "*Fail to build Bottom ROI object - Please adjust ROI or change threshold setting.";
            //    return false;
            //}

            // Scan blob to find 3 tip blob.
            bool blnFoundTip = false;
            float fParentBlobHeight = m_objEBlobs_Bottom.ref_arrHeight[0];
            float fParentBlobCenterY = m_objEBlobs_Bottom.ref_arrLimitCenterY[0];
            ROI objROI = new ROI();
            for (int intThickness = 15; intThickness < fParentBlobHeight; intThickness++)// 2021-07-16 ZJYEOH : Change Max build pixel from 50 to fParentBlobHeight
            {
                objROI.LoadROISetting(m_arrROIs[3].ref_ROIPositionX,
                      m_arrROIs[3].ref_ROIPositionY + (int)Math.Round((fParentBlobCenterY + (fParentBlobHeight / 2)) - intThickness),
                      m_arrROIs[3].ref_ROIWidth,
                      m_arrROIs[3].ref_ROIHeight - (int)Math.Round(fParentBlobHeight - intThickness));
                objROI.AttachImage(objImage);
                m_objEBlobs_Bottom.CleanAllBlobs();
                m_objEBlobs_Bottom.BuildObjects_Filter_GetElement_SortByX(objROI, true, true, 0, m_intThresholdBottom,
                    20, objROI.ref_ROIWidth * objROI.ref_ROIHeight, false, 0xFF, false);

                if (m_objEBlobs_Bottom.ref_intNumSelectedObject >= 3)
                {
                    blnFoundTip = true;
                    break;
                }
            }

            if (!blnFoundTip)
            {
                objROI.Dispose();
                m_strErrorMessage = "*Fail to build Bottom ROI object - Please adjust ROI or change threshold setting.";
                return false;
            }

            m_blnBlobOK_Bottom = true;
            //m_arrROIs[3].ThresholdTo_ROIToROISamePosition(ref objImage, m_intThresholdBottom);

            if (m_arrLGauge[3].Count == 0)
                m_arrLGauge[3].Add(new LGauge());//Smallest blob center
            if (m_arrLGauge[3].Count == 1)
                m_arrLGauge[3].Add(new LGauge());//Smallest blob left
            if (m_arrLGauge[3].Count == 2)
                m_arrLGauge[3].Add(new LGauge());//Smallest blob right

            m_arrLGauge[3][0].ref_GaugeAngle = 180;
            m_arrLGauge[3][0].ref_GaugeTransType = 1;
            //m_arrLGauge[3][0].ref_GaugeTransChoice =  m_intGaugeTransitionChoice;
            //m_arrLGauge[3][0].ref_GaugeThickness =  m_intGaugeThickness;
            //m_arrLGauge[3][0].ref_GaugeThreshold = m_intGaugeThreshold;
            //m_arrLGauge[3][0].ref_GaugeMinAmplitude = m_intGaugeMinAmplitude;
            //m_arrLGauge[3][0].ref_GaugeSamplingStep = m_intGaugeSamplingSteps;
            //m_arrLGauge[3][0].ref_GaugeMinArea = m_intGaugeMinArea;
            //m_arrLGauge[3][0].ref_GaugeFilter = m_intGaugeFilter;
            //m_arrLGauge[3][0].ref_GaugeFilterThreshold = m_intGaugeFilterThreshold;
            //m_arrLGauge[3][0].ref_GaugeFilterPasses = m_intGaugeFilterPass;
            m_arrLGauge[3][0].ref_GaugeTolerance = Math.Max(10, m_objEBlobs_Bottom.ref_arrHeight[0]);
            m_arrLGauge[3][0].ref_GaugeLength = Math.Max(5, m_objEBlobs_Bottom.ref_arrWidth[0] / 2);

            m_arrLGauge[3][0].SetGaugeCenter(objROI.ref_ROIPositionX + m_objEBlobs_Bottom.ref_arrLimitCenterX[0], objROI.ref_ROIPositionY + (m_objEBlobs_Bottom.ref_arrLimitCenterY[0] + (m_objEBlobs_Bottom.ref_arrHeight[0] / 2)));
            
            m_arrLGauge[3][0].Measure(m_arrROIs[3]);

            if (m_arrLGauge[3][0].ref_ObjectScore == 0)
            {
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Bottom ROI point - Please adjust gauge setting.";
                return false;
            }
            
            m_arrLGauge[3][1].ref_GaugeAngle = -90;
            m_arrLGauge[3][1].ref_GaugeTransType = 1;
            m_arrLGauge[3][1].ref_GaugeTolerance = Math.Max(10, m_objEBlobs_Bottom.ref_arrWidth[0] / 2);
            m_arrLGauge[3][1].ref_GaugeLength = Math.Max(5, m_objEBlobs_Bottom.ref_arrHeight[0]);

            m_arrLGauge[3][1].SetGaugeCenter(objROI.ref_ROIPositionX + (m_objEBlobs_Bottom.ref_arrLimitCenterX[0] - (m_objEBlobs_Bottom.ref_arrWidth[0] / 2)), m_arrLGauge[3][0].ref_ObjectCenterY - 5);

            m_arrLGauge[3][1].Measure(m_arrROIs[3]);

            if (m_arrLGauge[3][1].ref_ObjectScore == 0)
            {
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Bottom ROI point - Please adjust gauge setting.";
                return false;
            }

            m_arrLGauge[3][2].ref_GaugeAngle = 90;
            m_arrLGauge[3][2].ref_GaugeTransType = 1;
            m_arrLGauge[3][2].ref_GaugeTolerance = Math.Max(10, m_objEBlobs_Bottom.ref_arrWidth[0] / 2);
            m_arrLGauge[3][2].ref_GaugeLength = Math.Max(5, m_objEBlobs_Bottom.ref_arrHeight[0]);

            m_arrLGauge[3][2].SetGaugeCenter(objROI.ref_ROIPositionX + (m_objEBlobs_Bottom.ref_arrLimitCenterX[0] + (m_objEBlobs_Bottom.ref_arrWidth[0] / 2)), m_arrLGauge[3][0].ref_ObjectCenterY - 5);

            m_arrLGauge[3][2].Measure(m_arrROIs[3]);

            if (m_arrLGauge[3][2].ref_ObjectScore == 0)
            {
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Bottom ROI point - Please adjust gauge setting.";
                return false;
            }

            if (blnUpdateData)
            {
                if (m_fRotateAngleSide != 0)
                    m_f3DBottomPixelCount1 = FixAngle(m_fRotateAngleSide, 3, 2, 1, true);
                else
                    m_f3DBottomPixelCount1 = m_arrLGauge[3][2].ref_ObjectCenterX - m_arrLGauge[3][1].ref_ObjectCenterX;
            }

            if (m_arrLGauge[3].Count == 3)
                m_arrLGauge[3].Add(new LGauge());//Middle blob center
            if (m_arrLGauge[3].Count == 4)
                m_arrLGauge[3].Add(new LGauge());//Middle blob left
            if (m_arrLGauge[3].Count == 5)
                m_arrLGauge[3].Add(new LGauge());//Middle blob right

            m_arrLGauge[3][3].ref_GaugeAngle = 180;
            m_arrLGauge[3][3].ref_GaugeTransType = 1;
            //m_arrLGauge[3][3].ref_GaugeTransChoice =  m_intGaugeTransitionChoice;
            //m_arrLGauge[3][3].ref_GaugeThickness =  m_intGaugeThickness;
            //m_arrLGauge[3][3].ref_GaugeThreshold = m_intGaugeThreshold;
            //m_arrLGauge[3][3].ref_GaugeMinAmplitude = m_intGaugeMinAmplitude;
            //m_arrLGauge[3][3].ref_GaugeSamplingStep = m_intGaugeSamplingSteps;
            //m_arrLGauge[3][3].ref_GaugeMinArea = m_intGaugeMinArea;
            //m_arrLGauge[3][3].ref_GaugeFilter = m_intGaugeFilter;
            //m_arrLGauge[3][3].ref_GaugeFilterThreshold = m_intGaugeFilterThreshold;
            //m_arrLGauge[3][3].ref_GaugeFilterPasses = m_intGaugeFilterPass;
            m_arrLGauge[3][3].ref_GaugeTolerance = Math.Max(10, m_objEBlobs_Bottom.ref_arrHeight[1]);
            m_arrLGauge[3][3].ref_GaugeLength = Math.Max(5, m_objEBlobs_Bottom.ref_arrWidth[1] / 2);

            m_arrLGauge[3][3].SetGaugeCenter(objROI.ref_ROIPositionX + m_objEBlobs_Bottom.ref_arrLimitCenterX[1], objROI.ref_ROIPositionY + (m_objEBlobs_Bottom.ref_arrLimitCenterY[1] + (m_objEBlobs_Bottom.ref_arrHeight[1] / 2)));

            m_arrLGauge[3][3].Measure(m_arrROIs[3]);

            if (m_arrLGauge[3][3].ref_ObjectScore == 0)
            {
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Bottom ROI point - Please adjust gauge setting.";
                return false;
            }

            m_arrLGauge[3][4].ref_GaugeAngle = -90;
            m_arrLGauge[3][4].ref_GaugeTransType = 1;
            m_arrLGauge[3][4].ref_GaugeTolerance = Math.Max(10, m_objEBlobs_Bottom.ref_arrWidth[1] / 2);
            m_arrLGauge[3][4].ref_GaugeLength = Math.Max(5, m_objEBlobs_Bottom.ref_arrHeight[1]);

            m_arrLGauge[3][4].SetGaugeCenter(objROI.ref_ROIPositionX + (m_objEBlobs_Bottom.ref_arrLimitCenterX[1] - (m_objEBlobs_Bottom.ref_arrWidth[1] / 2)), m_arrLGauge[3][3].ref_ObjectCenterY - 5);

            m_arrLGauge[3][4].Measure(m_arrROIs[3]);

            if (m_arrLGauge[3][4].ref_ObjectScore == 0)
            {
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Bottom ROI point - Please adjust gauge setting.";
                return false;
            }

            m_arrLGauge[3][5].ref_GaugeAngle = 90;
            m_arrLGauge[3][5].ref_GaugeTransType = 1;
            m_arrLGauge[3][5].ref_GaugeTolerance = Math.Max(10, m_objEBlobs_Bottom.ref_arrWidth[1] / 2);
            m_arrLGauge[3][5].ref_GaugeLength = Math.Max(5, m_objEBlobs_Bottom.ref_arrHeight[1]);

            m_arrLGauge[3][5].SetGaugeCenter(objROI.ref_ROIPositionX + (m_objEBlobs_Bottom.ref_arrLimitCenterX[1] + (m_objEBlobs_Bottom.ref_arrWidth[1] / 2)), m_arrLGauge[3][3].ref_ObjectCenterY - 5);

            m_arrLGauge[3][5].Measure(m_arrROIs[3]);

            if (m_arrLGauge[3][5].ref_ObjectScore == 0)
            {
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Bottom ROI point - Please adjust gauge setting.";
                return false;
            }

            if (m_arrLGauge[3].Count == 6)
                m_arrLGauge[3].Add(new LGauge());//Largest blob center

            m_arrLGauge[3][6].ref_GaugeAngle = 180;
            m_arrLGauge[3][6].ref_GaugeTransType = 1;
            m_arrLGauge[3][6].ref_GaugeTolerance = Math.Max(10, m_objEBlobs_Bottom.ref_arrHeight[2]);
            m_arrLGauge[3][6].ref_GaugeLength = Math.Max(5, m_objEBlobs_Bottom.ref_arrWidth[2] / 2);

            m_arrLGauge[3][6].SetGaugeCenter(objROI.ref_ROIPositionX + m_objEBlobs_Bottom.ref_arrLimitCenterX[2], objROI.ref_ROIPositionY + (m_objEBlobs_Bottom.ref_arrLimitCenterY[2] + (m_objEBlobs_Bottom.ref_arrHeight[2] / 2)));

            m_arrLGauge[3][6].Measure(m_arrROIs[3]);

            if (m_arrLGauge[3][6].ref_ObjectScore == 0)
            {
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Bottom ROI point - Please adjust gauge setting.";
                return false;
            }

            if (blnUpdateData)
            {
                if (m_fRotateAngleSide != 0)
                    m_f3DBottomPixelCount2 = FixAngle(m_fRotateAngleSide, 3, 5, 4, true);
                else
                    m_f3DBottomPixelCount2 = m_arrLGauge[3][5].ref_ObjectCenterX - m_arrLGauge[3][4].ref_ObjectCenterX;

                m_fCalibration3DBottom = m_f3DBottomPixelCount2 / m_fSize3DB2;
            }

            //m_fBottomDistance = m_arrLGauge[3][6].ref_ObjectCenterY - m_arrLGauge[3][0].ref_ObjectCenterY;

            //m_fBottomDistance = m_fBottomDistance * (m_fSize3DB1 / m_f3DBottomPixelCount1);

            //m_fBottomAngle = (float)(Math.Asin(m_fBottomDistance / m_fSize2DA2) * 180 / Math.PI);

            m_arrBlobBottom.Clear();
            for (int i = 0; i < m_objEBlobs_Bottom.ref_intNumSelectedObject; i++)
            {

                m_stcBlobBottom = new BlobsFeatures();

                m_stcBlobBottom.fCenterX = objROI.ref_ROIPositionX + m_objEBlobs_Bottom.ref_arrLimitCenterX[i];
                m_stcBlobBottom.fCenterY = objROI.ref_ROIPositionY + m_objEBlobs_Bottom.ref_arrLimitCenterY[i];
                m_stcBlobBottom.fWidth = m_objEBlobs_Bottom.ref_arrWidth[i];
                m_stcBlobBottom.fHeight = m_objEBlobs_Bottom.ref_arrHeight[i];
                m_arrBlobBottom.Add(m_stcBlobBottom);
            }
            objROI.Dispose();
            return true;
        }

        public bool MeasureLeftBlob(ImageDrawing objImage, bool blnUpdateData)
        {
            m_blnBlobOK_Left = false;
            m_objEBlobs_Left.CleanAllBlobs();

            m_objEBlobs_Left.BuildObjects_Filter_GetElement(m_arrROIs[4], true, true, 0, m_intThresholdLeft,
                20, m_arrROIs[4].ref_ROIWidth * m_arrROIs[4].ref_ROIHeight, false, 0xFF);
            //m_arrROIs[4].SaveImage("D:\\TS\\LeftROI.bmp");
            if (m_objEBlobs_Left.ref_intNumSelectedObject == 0)
            {
                m_strErrorMessage = "*Fail to build Left ROI object - Please adjust ROI or change threshold setting.";
                return false;
            }

            //int intParentBlobWidth = (int)Math.Round(m_objEBlobs_Left.ref_arrWidth[0]);
            //int intParentBlobCenterX = (int)Math.Round(m_objEBlobs_Left.ref_arrLimitCenterX[0]);

            //ROI objROI = new ROI();
            //objROI.LoadROISetting(m_arrROIs[4].ref_ROIPositionX, m_arrROIs[4].ref_ROIPositionY, m_arrROIs[4].ref_ROIWidth -  (int)Math.Round(m_objEBlobs_Left.ref_arrWidth[0] - 10), m_arrROIs[4].ref_ROIHeight);
            //objROI.AttachImage(objImage);
            ////objROI.SaveImage("D:\\TS\\objROI.bmp");
            //m_objEBlobs_Left.CleanAllBlobs();
            //m_objEBlobs_Left.BuildObjects_Filter_GetElement_SortByY(objROI, true, true, 0, m_intThresholdLeft,
            //    20, objROI.ref_ROIWidth * objROI.ref_ROIHeight, false, 0xFF, true);

            //if (m_objEBlobs_Left.ref_intNumSelectedObject == 0)
            //{
            //    objROI.LoadROISetting(objROI.ref_ROIPositionX, objROI.ref_ROIPositionY, objROI.ref_ROIWidth + 10, objROI.ref_ROIHeight);

            //    m_objEBlobs_Left.CleanAllBlobs();
            //    m_objEBlobs_Left.BuildObjects_Filter_GetElement_SortByY(objROI, true, true, 0, m_intThresholdLeft,
            //        20, objROI.ref_ROIWidth * objROI.ref_ROIHeight, false, 0xFF, true);
            //}

            //if (m_objEBlobs_Left.ref_intNumSelectedObject != 3)
            //{
            //    objROI.Dispose();
            //    m_strErrorMessage = "*Fail to build Left ROI object - Please adjust ROI or change threshold setting.";
            //    return false;
            //}

            bool blnFoundTip = false;
            float fParentBlobWidth = m_objEBlobs_Left.ref_arrWidth[0];
            ROI objROI = new ROI();
            for (int intThickness = 15; intThickness < fParentBlobWidth; intThickness++)// 2021-07-16 ZJYEOH : Change Max build pixel from 50 to fParentBlobWidth
            {
                objROI.LoadROISetting(m_arrROIs[4].ref_ROIPositionX, 
                                      m_arrROIs[4].ref_ROIPositionY, 
                                      m_arrROIs[4].ref_ROIWidth - (int)Math.Round(fParentBlobWidth - intThickness), 
                                      m_arrROIs[4].ref_ROIHeight);

                objROI.AttachImage(objImage);
                    m_objEBlobs_Left.CleanAllBlobs();
                m_objEBlobs_Left.BuildObjects_Filter_GetElement_SortByY(objROI, true, true, 0, m_intThresholdLeft,
                    20, objROI.ref_ROIWidth * objROI.ref_ROIHeight, false, 0xFF, true);

                if (m_objEBlobs_Left.ref_intNumSelectedObject >= 3)
                {
                    blnFoundTip = true;
                    break;
                }
            }

            if (!blnFoundTip)
            {
                objROI.Dispose();
                m_strErrorMessage = "*Fail to build Left ROI object - Please adjust ROI or change threshold setting.";
                return false;
            }

            m_blnBlobOK_Left = true;

            //m_arrROIs[4].ThresholdTo_ROIToROISamePosition(ref objImage, m_intThresholdLeft);

            if (m_arrLGauge[4].Count == 0)
                m_arrLGauge[4].Add(new LGauge());//Smallest blob center
            if (m_arrLGauge[4].Count == 1)
                m_arrLGauge[4].Add(new LGauge());//Smallest blob bottom
            if (m_arrLGauge[4].Count == 2)
                m_arrLGauge[4].Add(new LGauge());//Smallest blob top

            m_arrLGauge[4][0].ref_GaugeAngle = 270;
            m_arrLGauge[4][0].ref_GaugeTransType = 1;
            //m_arrLGauge[4][0].ref_GaugeTransChoice =  m_intGaugeTransitionChoice;
            //m_arrLGauge[4][0].ref_GaugeThickness =  m_intGaugeThickness;
            //m_arrLGauge[4][0].ref_GaugeThreshold = m_intGaugeThreshold;
            //m_arrLGauge[4][0].ref_GaugeMinAmplitude = m_intGaugeMinAmplitude;
            //m_arrLGauge[4][0].ref_GaugeSamplingStep = m_intGaugeSamplingSteps;
            //m_arrLGauge[4][0].ref_GaugeMinArea = m_intGaugeMinArea;
            //m_arrLGauge[4][0].ref_GaugeFilter = m_intGaugeFilter;
            //m_arrLGauge[4][0].ref_GaugeFilterThreshold = m_intGaugeFilterThreshold;
            //m_arrLGauge[4][0].ref_GaugeFilterPasses = m_intGaugeFilterPass;
            m_arrLGauge[4][0].ref_GaugeTolerance = Math.Max(5, m_objEBlobs_Left.ref_arrWidth[2]);
            m_arrLGauge[4][0].ref_GaugeLength = Math.Max(5, m_objEBlobs_Left.ref_arrHeight[2] / 2);

            m_arrLGauge[4][0].SetGaugeCenter(m_arrROIs[4].ref_ROIPositionX + (m_objEBlobs_Left.ref_arrLimitCenterX[2] - (m_objEBlobs_Left.ref_arrWidth[2] / 2)), m_arrROIs[4].ref_ROIPositionY + m_objEBlobs_Left.ref_arrLimitCenterY[2]);
            
            m_arrLGauge[4][0].Measure(m_arrROIs[4]);

            if (m_arrLGauge[4][0].ref_ObjectScore == 0)
            {
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Left ROI point - Please adjust gauge setting.";
                return false;
            }

            m_arrLGauge[4][1].ref_GaugeAngle = 180;
            m_arrLGauge[4][1].ref_GaugeTransType = 1;
            m_arrLGauge[4][1].ref_GaugeTolerance = Math.Max(5, m_objEBlobs_Left.ref_arrHeight[2] / 2);
            m_arrLGauge[4][1].ref_GaugeLength = Math.Max(5, m_objEBlobs_Left.ref_arrWidth[2]);

            m_arrLGauge[4][1].SetGaugeCenter(m_arrLGauge[4][0].ref_ObjectCenterX + 5, m_arrROIs[4].ref_ROIPositionY + (m_objEBlobs_Left.ref_arrLimitCenterY[2] + (m_objEBlobs_Left.ref_arrRectHeight[2] / 2)));

            m_arrLGauge[4][1].Measure(m_arrROIs[4]);

            if (m_arrLGauge[4][1].ref_ObjectScore == 0)
            {
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Left ROI point - Please adjust gauge setting.";
                return false;
            }

            m_arrLGauge[4][2].ref_GaugeAngle = 0;
            m_arrLGauge[4][2].ref_GaugeTransType = 1;
            m_arrLGauge[4][2].ref_GaugeTolerance = Math.Max(5, m_objEBlobs_Left.ref_arrHeight[2] / 2);
            m_arrLGauge[4][2].ref_GaugeLength = Math.Max(5, m_objEBlobs_Left.ref_arrWidth[2]);

            m_arrLGauge[4][2].SetGaugeCenter(m_arrLGauge[4][0].ref_ObjectCenterX + 5, m_arrROIs[4].ref_ROIPositionY + (m_objEBlobs_Left.ref_arrLimitCenterY[2] - (m_objEBlobs_Left.ref_arrRectHeight[2] / 2)));

            m_arrLGauge[4][2].Measure(m_arrROIs[4]);

            if (m_arrLGauge[4][2].ref_ObjectScore == 0)
            {
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Left ROI point - Please adjust gauge setting.";
                return false;
            }

            if (blnUpdateData)
            {
                if (m_fRotateAngleSide != 0)
                    m_f3DLeftPixelCount1 = FixAngle(m_fRotateAngleSide, 4, 1, 2, false);
                else
                    m_f3DLeftPixelCount1 = m_arrLGauge[4][1].ref_ObjectCenterY - m_arrLGauge[4][2].ref_ObjectCenterY;
            }

            if (m_arrLGauge[4].Count == 3)
                m_arrLGauge[4].Add(new LGauge());//Middle blob center
            if (m_arrLGauge[4].Count == 4)
                m_arrLGauge[4].Add(new LGauge());//Middle blob bottom
            if (m_arrLGauge[4].Count == 5)
                m_arrLGauge[4].Add(new LGauge());//Middle blob top

            m_arrLGauge[4][3].ref_GaugeAngle = 270;
            m_arrLGauge[4][3].ref_GaugeTransType = 1;
            //m_arrLGauge[4][3].ref_GaugeTransChoice =  m_intGaugeTransitionChoice;
            //m_arrLGauge[4][3].ref_GaugeThickness =  m_intGaugeThickness;
            //m_arrLGauge[4][3].ref_GaugeThreshold = m_intGaugeThreshold;
            //m_arrLGauge[4][3].ref_GaugeMinAmplitude = m_intGaugeMinAmplitude;
            //m_arrLGauge[4][3].ref_GaugeSamplingStep = m_intGaugeSamplingSteps;
            //m_arrLGauge[4][3].ref_GaugeMinArea = m_intGaugeMinArea;
            //m_arrLGauge[4][3].ref_GaugeFilter = m_intGaugeFilter;
            //m_arrLGauge[4][3].ref_GaugeFilterThreshold = m_intGaugeFilterThreshold;
            //m_arrLGauge[4][3].ref_GaugeFilterPasses = m_intGaugeFilterPass;
            m_arrLGauge[4][3].ref_GaugeTolerance = Math.Max(5, m_objEBlobs_Left.ref_arrWidth[1]);
            m_arrLGauge[4][3].ref_GaugeLength = Math.Max(5, m_objEBlobs_Left.ref_arrHeight[1] / 2);

            m_arrLGauge[4][3].SetGaugeCenter(m_arrROIs[4].ref_ROIPositionX + (m_objEBlobs_Left.ref_arrLimitCenterX[1] - (m_objEBlobs_Left.ref_arrWidth[1] / 2)), m_arrROIs[4].ref_ROIPositionY + m_objEBlobs_Left.ref_arrLimitCenterY[1]);

            m_arrLGauge[4][3].Measure(m_arrROIs[4]);

            if (m_arrLGauge[4][3].ref_ObjectScore == 0)
            {
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Left ROI point - Please adjust gauge setting.";
                return false;
            }

            m_arrLGauge[4][4].ref_GaugeAngle = 180;
            m_arrLGauge[4][4].ref_GaugeTransType = 1;
            m_arrLGauge[4][4].ref_GaugeTolerance = Math.Max(5, m_objEBlobs_Left.ref_arrHeight[1] / 2);
            m_arrLGauge[4][4].ref_GaugeLength = Math.Max(5, m_objEBlobs_Left.ref_arrWidth[1]);

            m_arrLGauge[4][4].SetGaugeCenter(m_arrLGauge[4][3].ref_ObjectCenterX + 5, m_arrROIs[4].ref_ROIPositionY + (m_objEBlobs_Left.ref_arrLimitCenterY[1] + (m_objEBlobs_Left.ref_arrRectHeight[1] / 2)));

            m_arrLGauge[4][4].Measure(m_arrROIs[4]);

            if (m_arrLGauge[4][4].ref_ObjectScore == 0)
            {
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Left ROI point - Please adjust gauge setting.";
                return false;
            }

            m_arrLGauge[4][5].ref_GaugeAngle = 0;
            m_arrLGauge[4][5].ref_GaugeTransType = 1;
            m_arrLGauge[4][5].ref_GaugeTolerance = Math.Max(5, m_objEBlobs_Left.ref_arrHeight[1] / 2);
            m_arrLGauge[4][5].ref_GaugeLength = Math.Max(5, m_objEBlobs_Left.ref_arrWidth[1]);

            m_arrLGauge[4][5].SetGaugeCenter(m_arrLGauge[4][3].ref_ObjectCenterX + 5, m_arrROIs[4].ref_ROIPositionY + (m_objEBlobs_Left.ref_arrLimitCenterY[1] - (m_objEBlobs_Left.ref_arrRectHeight[1] / 2)));

            m_arrLGauge[4][5].Measure(m_arrROIs[4]);

            if (m_arrLGauge[4][5].ref_ObjectScore == 0)
            {
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Left ROI point - Please adjust gauge setting.";
                return false;
            }

            if (m_arrLGauge[4].Count == 6)
                m_arrLGauge[4].Add(new LGauge());//Largest blob center

            m_arrLGauge[4][6].ref_GaugeAngle = 270;
            m_arrLGauge[4][6].ref_GaugeTransType = 1;
            m_arrLGauge[4][6].ref_GaugeTolerance = Math.Max(5, m_objEBlobs_Left.ref_arrWidth[0]);
            m_arrLGauge[4][6].ref_GaugeLength = Math.Max(5, m_objEBlobs_Left.ref_arrHeight[0] / 2);

            m_arrLGauge[4][6].SetGaugeCenter(m_arrROIs[4].ref_ROIPositionX + (m_objEBlobs_Left.ref_arrLimitCenterX[0] - (m_objEBlobs_Left.ref_arrWidth[0] / 2)), m_arrROIs[4].ref_ROIPositionY + m_objEBlobs_Left.ref_arrLimitCenterY[0]);

            m_arrLGauge[4][6].Measure(m_arrROIs[4]);

            if (m_arrLGauge[4][6].ref_ObjectScore == 0)
            {
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Left ROI point - Please adjust gauge setting.";
                return false;
            }

            if (blnUpdateData)
            {
                if (m_fRotateAngleSide != 0)
                    m_f3DLeftPixelCount2 = FixAngle(m_fRotateAngleSide, 4, 4, 5, false);
                else
                    m_f3DLeftPixelCount2 = m_arrLGauge[4][4].ref_ObjectCenterY - m_arrLGauge[4][5].ref_ObjectCenterY;

                m_fCalibration3DLeft = m_f3DLeftPixelCount2 / m_fSize3DL2;
            }

            //m_fLeftDistance = m_arrLGauge[4][0].ref_ObjectCenterX - m_arrLGauge[4][6].ref_ObjectCenterX;

            //m_fLeftDistance = m_fLeftDistance * (m_fSize3DL1 / m_f3DLeftPixelCount1);

            //m_fLeftAngle = (float)(Math.Asin(m_fLeftDistance / m_fSize2DA1) * 180 / Math.PI);

            m_arrBlobLeft.Clear();
            for (int i = 0; i < m_objEBlobs_Left.ref_intNumSelectedObject; i++)
            {

                m_stcBlobLeft = new BlobsFeatures();

                m_stcBlobLeft.fCenterX = m_arrROIs[4].ref_ROIPositionX + m_objEBlobs_Left.ref_arrLimitCenterX[i];
                m_stcBlobLeft.fCenterY = m_arrROIs[4].ref_ROIPositionY + m_objEBlobs_Left.ref_arrLimitCenterY[i];
                m_stcBlobLeft.fWidth = m_objEBlobs_Left.ref_arrWidth[i];
                m_stcBlobLeft.fHeight = m_objEBlobs_Left.ref_arrHeight[i];
                m_arrBlobLeft.Add(m_stcBlobLeft);
            }
            objROI.Dispose();
            return true;
        }

        public bool MeasureRightBlob(ImageDrawing objImage, bool blnUpdateData)
        {
            m_blnBlobOK_Right = false;
            m_objEBlobs_Right.CleanAllBlobs();

            m_objEBlobs_Right.BuildObjects_Filter_GetElement(m_arrROIs[2], true, true, 0, m_intThresholdRight,
                20, m_arrROIs[2].ref_ROIWidth * m_arrROIs[2].ref_ROIHeight, false, 0xFF);
            //m_arrROIs[2].SaveImage("D:\\TS\\RightROI.bmp");
            if (m_objEBlobs_Right.ref_intNumSelectedObject == 0)
            {
                m_strErrorMessage = "*Fail to build Right ROI object - Please adjust ROI or change threshold setting.";
                return false;
            }

            //int intParentBlobWidth = (int)Math.Round(m_objEBlobs_Right.ref_arrWidth[0]);
            //int intParentBlobCenterX = (int)Math.Round(m_objEBlobs_Right.ref_arrLimitCenterX[0]);

            //ROI objROI = new ROI();
            //objROI.LoadROISetting(m_arrROIs[2].ref_ROIPositionX + (int)Math.Round((m_objEBlobs_Right.ref_arrLimitCenterX[0] + (m_objEBlobs_Right.ref_arrWidth[0] / 2)) - 10), m_arrROIs[2].ref_ROIPositionY, (int)Math.Round(m_objEBlobs_Right.ref_arrWidth[0] - 10), m_arrROIs[2].ref_ROIHeight);
            //objROI.AttachImage(objImage);
            ////objROI.SaveImage("D:\\TS\\objROI.bmp");
            //m_objEBlobs_Right.CleanAllBlobs();
            //m_objEBlobs_Right.BuildObjects_Filter_GetElement_SortByY(objROI, true, true, 0, m_intThresholdRight,
            //    20, objROI.ref_ROIWidth * objROI.ref_ROIHeight, false, 0xFF, false);

            //if (m_objEBlobs_Right.ref_intNumSelectedObject == 0)
            //{
            //    objROI.LoadROISetting(objROI.ref_ROIPositionX, objROI.ref_ROIPositionY, objROI.ref_ROIWidth - 10, objROI.ref_ROIHeight);

            //    m_objEBlobs_Right.CleanAllBlobs();
            //    m_objEBlobs_Right.BuildObjects_Filter_GetElement_SortByY(objROI, true, true, 0, m_intThresholdRight,
            //        20, objROI.ref_ROIWidth * objROI.ref_ROIHeight, false, 0xFF, false);
            //}

            //if (m_objEBlobs_Right.ref_intNumSelectedObject != 3)
            //{
            //    objROI.Dispose();
            //    m_strErrorMessage = "*Fail to build Right ROI object - Please adjust ROI or change threshold setting.";
            //    return false;
            //}

            bool blnFoundTip = false;
            float fParentBlobWidth = m_objEBlobs_Right.ref_arrWidth[0];
            float fParentBlobCenterX = m_objEBlobs_Right.ref_arrLimitCenterX[0];
            ROI objROI = new ROI();
            for (int intThickness = 15; intThickness < fParentBlobWidth; intThickness++)// 2021-07-16 ZJYEOH : Change Max build pixel from 50 to fParentBlobWidth
            {
                objROI.LoadROISetting(m_arrROIs[2].ref_ROIPositionX + (int)Math.Round((fParentBlobCenterX + (fParentBlobWidth / 2)) - intThickness), 
                                      m_arrROIs[2].ref_ROIPositionY,
                                      m_arrROIs[2].ref_ROIWidth - (int)Math.Round(fParentBlobWidth - intThickness), 
                                      m_arrROIs[2].ref_ROIHeight);

                objROI.AttachImage(objImage);
                m_objEBlobs_Right.CleanAllBlobs();
                m_objEBlobs_Right.BuildObjects_Filter_GetElement_SortByY(objROI, true, true, 0, m_intThresholdRight,
                    20, objROI.ref_ROIWidth * objROI.ref_ROIHeight, false, 0xFF, false);
               
                if (m_objEBlobs_Right.ref_intNumSelectedObject >= 3)
                {
                    blnFoundTip = true;
                    break;
                }
            }

            if (!blnFoundTip)
            {
                objROI.Dispose();
                m_strErrorMessage = "*Fail to build Right ROI object - Please adjust ROI or change threshold setting.";
                return false;
            }

            m_blnBlobOK_Right = true;
            //m_arrROIs[2].ThresholdTo_ROIToROISamePosition(ref objImage, m_intThresholdRight);

            if (m_arrLGauge[2].Count == 0)
                m_arrLGauge[2].Add(new LGauge());//Smallest blob center
            if (m_arrLGauge[2].Count == 1)
                m_arrLGauge[2].Add(new LGauge());//Smallest blob bottom
            if (m_arrLGauge[2].Count == 2)
                m_arrLGauge[2].Add(new LGauge());//Smallest blob top

            m_arrLGauge[2][0].ref_GaugeAngle = 90;
            m_arrLGauge[2][0].ref_GaugeTransType = 1;
            //m_arrLGauge[2][0].ref_GaugeTransChoice =  m_intGaugeTransitionChoice;
            //m_arrLGauge[2][0].ref_GaugeThickness =  m_intGaugeThickness;
            //m_arrLGauge[2][0].ref_GaugeThreshold = m_intGaugeThreshold;
            //m_arrLGauge[2][0].ref_GaugeMinAmplitude = m_intGaugeMinAmplitude;
            //m_arrLGauge[2][0].ref_GaugeSamplingStep = m_intGaugeSamplingSteps;
            //m_arrLGauge[2][0].ref_GaugeMinArea = m_intGaugeMinArea;
            //m_arrLGauge[2][0].ref_GaugeFilter = m_intGaugeFilter;
            //m_arrLGauge[2][0].ref_GaugeFilterThreshold = m_intGaugeFilterThreshold;
            //m_arrLGauge[2][0].ref_GaugeFilterPasses = m_intGaugeFilterPass;
            m_arrLGauge[2][0].ref_GaugeTolerance = Math.Max(5, m_objEBlobs_Right.ref_arrWidth[2]);
            m_arrLGauge[2][0].ref_GaugeLength = Math.Max(5, m_objEBlobs_Right.ref_arrHeight[2] / 2);

            m_arrLGauge[2][0].SetGaugeCenter(objROI.ref_ROIPositionX + (m_objEBlobs_Right.ref_arrLimitCenterX[2] + (m_objEBlobs_Right.ref_arrWidth[2] / 2)), objROI.ref_ROIPositionY + m_objEBlobs_Right.ref_arrLimitCenterY[2]);

            m_arrLGauge[2][0].Measure(m_arrROIs[2]);

            if (m_arrLGauge[2][0].ref_ObjectScore == 0)
            {
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Right ROI point - Please adjust gauge setting.";
                return false;
            }

            m_arrLGauge[2][1].ref_GaugeAngle = 180;
            m_arrLGauge[2][1].ref_GaugeTransType = 1;
            m_arrLGauge[2][1].ref_GaugeTolerance = Math.Max(5, m_objEBlobs_Right.ref_arrHeight[2] / 2);
            m_arrLGauge[2][1].ref_GaugeLength = Math.Max(5, m_objEBlobs_Right.ref_arrWidth[2]);

            m_arrLGauge[2][1].SetGaugeCenter(m_arrLGauge[2][0].ref_ObjectCenterX - 5, objROI.ref_ROIPositionY + (m_objEBlobs_Right.ref_arrLimitCenterY[2] + (m_objEBlobs_Right.ref_arrRectHeight[2] / 2)));

            m_arrLGauge[2][1].Measure(m_arrROIs[2]);

            if (m_arrLGauge[2][1].ref_ObjectScore == 0)
            {
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Right ROI point - Please adjust gauge setting.";
                return false;
            }

            m_arrLGauge[2][2].ref_GaugeAngle = 0;
            m_arrLGauge[2][2].ref_GaugeTransType = 1;
            m_arrLGauge[2][2].ref_GaugeTolerance = Math.Max(5, m_objEBlobs_Right.ref_arrHeight[2] / 2);
            m_arrLGauge[2][2].ref_GaugeLength = Math.Max(5, m_objEBlobs_Right.ref_arrWidth[2]);

            m_arrLGauge[2][2].SetGaugeCenter(m_arrLGauge[2][0].ref_ObjectCenterX - 5, objROI.ref_ROIPositionY + (m_objEBlobs_Right.ref_arrLimitCenterY[2] - (m_objEBlobs_Right.ref_arrRectHeight[2] / 2)));

            m_arrLGauge[2][2].Measure(m_arrROIs[2]);

            if (m_arrLGauge[4][2].ref_ObjectScore == 0)
            {
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Right ROI point - Please adjust gauge setting.";
                return false;
            }

            if (blnUpdateData)
            {
                if (m_fRotateAngleSide != 0)
                    m_f3DRightPixelCount1 = FixAngle(m_fRotateAngleSide, 2, 1, 2, false);
                else
                    m_f3DRightPixelCount1 = m_arrLGauge[2][1].ref_ObjectCenterY - m_arrLGauge[2][2].ref_ObjectCenterY;
            }

            if (m_arrLGauge[2].Count == 3)
                m_arrLGauge[2].Add(new LGauge());//Middle blob center
            if (m_arrLGauge[2].Count == 4)
                m_arrLGauge[2].Add(new LGauge());//Middle blob bottom
            if (m_arrLGauge[2].Count == 5)
                m_arrLGauge[2].Add(new LGauge());//Middle blob top

            m_arrLGauge[2][3].ref_GaugeAngle = 90;
            m_arrLGauge[2][3].ref_GaugeTransType = 1;
            //m_arrLGauge[2][3].ref_GaugeTransChoice =  m_intGaugeTransitionChoice;
            //m_arrLGauge[2][3].ref_GaugeThickness =  m_intGaugeThickness;
            //m_arrLGauge[2][3].ref_GaugeThreshold = m_intGaugeThreshold;
            //m_arrLGauge[2][3].ref_GaugeMinAmplitude = m_intGaugeMinAmplitude;
            //m_arrLGauge[2][3].ref_GaugeSamplingStep = m_intGaugeSamplingSteps;
            //m_arrLGauge[2][3].ref_GaugeMinArea = m_intGaugeMinArea;
            //m_arrLGauge[2][3].ref_GaugeFilter = m_intGaugeFilter;
            //m_arrLGauge[2][3].ref_GaugeFilterThreshold = m_intGaugeFilterThreshold;
            //m_arrLGauge[2][3].ref_GaugeFilterPasses = m_intGaugeFilterPass;
            m_arrLGauge[2][3].ref_GaugeTolerance = Math.Max(5, m_objEBlobs_Right.ref_arrWidth[1]);
            m_arrLGauge[2][3].ref_GaugeLength = Math.Max(5, m_objEBlobs_Right.ref_arrHeight[1] / 2);

            m_arrLGauge[2][3].SetGaugeCenter(objROI.ref_ROIPositionX + (m_objEBlobs_Right.ref_arrLimitCenterX[1] + (m_objEBlobs_Right.ref_arrWidth[1] / 2)), objROI.ref_ROIPositionY + m_objEBlobs_Right.ref_arrLimitCenterY[1]);

            m_arrLGauge[2][3].Measure(m_arrROIs[2]);

            if (m_arrLGauge[2][3].ref_ObjectScore == 0)
            {
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Right ROI point - Please adjust gauge setting.";
                return false;
            }

            m_arrLGauge[2][4].ref_GaugeAngle = 180;
            m_arrLGauge[2][4].ref_GaugeTransType = 1;
            m_arrLGauge[2][4].ref_GaugeTolerance = Math.Max(5, m_objEBlobs_Right.ref_arrHeight[1] / 2);
            m_arrLGauge[2][4].ref_GaugeLength = Math.Max(5, m_objEBlobs_Right.ref_arrWidth[1]);

            m_arrLGauge[2][4].SetGaugeCenter(m_arrLGauge[2][3].ref_ObjectCenterX - 5, objROI.ref_ROIPositionY + (m_objEBlobs_Right.ref_arrLimitCenterY[1] + (m_objEBlobs_Right.ref_arrRectHeight[1] / 2)));

            m_arrLGauge[2][4].Measure(m_arrROIs[2]);

            if (m_arrLGauge[2][4].ref_ObjectScore == 0)
            {
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Right ROI point - Please adjust gauge setting.";
                return false;
            }

            m_arrLGauge[2][5].ref_GaugeAngle = 0;
            m_arrLGauge[2][5].ref_GaugeTransType = 1;
            m_arrLGauge[2][5].ref_GaugeTolerance = Math.Max(5, m_objEBlobs_Right.ref_arrHeight[1] / 2);
            m_arrLGauge[2][5].ref_GaugeLength = Math.Max(5, m_objEBlobs_Right.ref_arrWidth[1]);

            m_arrLGauge[2][5].SetGaugeCenter(m_arrLGauge[2][3].ref_ObjectCenterX - 5, objROI.ref_ROIPositionY + (m_objEBlobs_Right.ref_arrLimitCenterY[1] - (m_objEBlobs_Right.ref_arrRectHeight[1] / 2)));

            m_arrLGauge[2][5].Measure(m_arrROIs[2]);

            if (m_arrLGauge[2][5].ref_ObjectScore == 0)
            {
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Right ROI point - Please adjust gauge setting.";
                return false;
            }

            if (m_arrLGauge[2].Count == 6)
                m_arrLGauge[2].Add(new LGauge());//Largest blob center

            m_arrLGauge[2][6].ref_GaugeAngle = 90;
            m_arrLGauge[2][6].ref_GaugeTransType = 1;
            m_arrLGauge[2][6].ref_GaugeTolerance = Math.Max(5, m_objEBlobs_Right.ref_arrWidth[0]);
            m_arrLGauge[2][6].ref_GaugeLength = Math.Max(5, m_objEBlobs_Right.ref_arrHeight[0] / 2);

            m_arrLGauge[2][6].SetGaugeCenter(objROI.ref_ROIPositionX + (m_objEBlobs_Right.ref_arrLimitCenterX[0] + (m_objEBlobs_Right.ref_arrWidth[0] / 2)), objROI.ref_ROIPositionY + m_objEBlobs_Right.ref_arrLimitCenterY[0]);

            m_arrLGauge[2][6].Measure(m_arrROIs[2]);

            if (m_arrLGauge[2][6].ref_ObjectScore == 0)
            {
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Right ROI point - Please adjust gauge setting.";
                return false;
            }

            if (blnUpdateData)
            {
                if (m_fRotateAngleSide != 0)
                    m_f3DRightPixelCount2 = FixAngle(m_fRotateAngleSide, 2, 4, 5, false);
                else
                    m_f3DRightPixelCount2 = m_arrLGauge[2][4].ref_ObjectCenterY - m_arrLGauge[2][5].ref_ObjectCenterY;

                m_fCalibration3DRight = m_f3DRightPixelCount2 / m_fSize3DR2;
            }

            //m_fRightDistance = m_arrLGauge[2][6].ref_ObjectCenterX - m_arrLGauge[2][0].ref_ObjectCenterX;

            //m_fRightDistance = m_fRightDistance * (m_fSize3DR1 / m_f3DRightPixelCount1);

            //m_fRightAngle = (float)(Math.Asin(m_fRightDistance / m_fSize2DA2) * 180 / Math.PI);

            m_arrBlobRight.Clear();
            for (int i = 0; i < m_objEBlobs_Right.ref_intNumSelectedObject; i++)
            {

                m_stcBlobRight = new BlobsFeatures();

                m_stcBlobRight.fCenterX = objROI.ref_ROIPositionX + m_objEBlobs_Right.ref_arrLimitCenterX[i];
                m_stcBlobRight.fCenterY = objROI.ref_ROIPositionY + m_objEBlobs_Right.ref_arrLimitCenterY[i];
                m_stcBlobRight.fWidth = m_objEBlobs_Right.ref_arrWidth[i];
                m_stcBlobRight.fHeight = m_objEBlobs_Right.ref_arrHeight[i];
                m_arrBlobRight.Add(m_stcBlobRight);
            }
            objROI.Dispose();
            return true;
        }

        public bool MeasureCenterHorizontalTop(ImageDrawing objImage, bool blnUpdateData)
        {
            if (m_arrBlobTop.Count != 3)
                return false;

            ImageDrawing objImgTemp = new ImageDrawing(true, objImage.ref_intCameraResolutionWidth, objImage.ref_intCameraResolutionHeight);
            objImage.CopyTo(objImgTemp);
            ROI objROI = new ROI();
            objROI.LoadROISetting(m_arrROIs[0].ref_ROIPositionX, m_arrROIs[0].ref_ROIPositionY, m_arrROIs[0].ref_ROIWidth, m_arrROIs[0].ref_ROIHeight);
            objROI.AttachImage(objImgTemp);
            //EasyImage.Threshold(m_arrROIs[0].ref_ROI, objROI.ref_ROI, m_intThresholdCenter);
            
            // Smallest bar
            if (m_arrLGauge[0].Count == 0)
                m_arrLGauge[0].Add(new LGauge());//left
            if (m_arrLGauge[0].Count == 1)
                m_arrLGauge[0].Add(new LGauge());//center
            if (m_arrLGauge[0].Count == 2)
                m_arrLGauge[0].Add(new LGauge());//right
        
            m_arrLGauge[0][1].ref_GaugeAngle = 0;
            m_arrLGauge[0][1].ref_GaugeTransType = 0;
            //m_arrLGauge[0][1].ref_GaugeTransChoice =  m_intGaugeTransitionChoice;
            //m_arrLGauge[0][1].ref_GaugeThickness =  m_intGaugeThickness;
            //m_arrLGauge[0][1].ref_GaugeThreshold = m_intGaugeThreshold;
            //m_arrLGauge[0][1].ref_GaugeMinAmplitude = m_intGaugeMinAmplitude;
            //m_arrLGauge[0][1].ref_GaugeSamplingStep = m_intGaugeSamplingSteps;
            //m_arrLGauge[0][1].ref_GaugeMinArea = m_intGaugeMinArea;
            //m_arrLGauge[0][1].ref_GaugeFilter = m_intGaugeFilter;
            //m_arrLGauge[0][1].ref_GaugeFilterThreshold = m_intGaugeFilterThreshold;
            //m_arrLGauge[0][1].ref_GaugeFilterPasses = m_intGaugeFilterPass;
            m_arrLGauge[0][1].ref_GaugeTolerance = Math.Max(5, m_arrROIs[0].ref_ROIHeight / 4);
            m_arrLGauge[0][1].ref_GaugeLength = Math.Max(5, ((BlobsFeatures)m_arrBlobTop[0]).fWidth / 2);
            m_arrLGauge[0][1].SetGaugeCenter(((BlobsFeatures)m_arrBlobTop[0]).fCenterX , m_arrROIs[0].ref_ROIPositionY + (m_arrROIs[0].ref_ROIHeight / 4));
            m_arrLGauge[0][1].Measure(objROI);

            if (m_arrLGauge[0][1].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][1].ref_GaugeTolerance = Math.Max(10, m_arrROIs[0].ref_ROIHeight / 2);
                m_arrLGauge[0][1].Measure(objROI);
            }

            if (m_arrLGauge[0][1].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Top Side point - Please adjust gauge setting.";
                return false;
            }

            m_arrLGauge[0][0].ref_GaugeAngle = 270;
            m_arrLGauge[0][0].ref_GaugeTransType = 0;
            m_arrLGauge[0][0].ref_GaugeTolerance = Math.Max(5, ((BlobsFeatures)m_arrBlobTop[0]).fWidth / 4);
            m_arrLGauge[0][0].ref_GaugeLength = 10;
            m_arrLGauge[0][0].SetGaugeCenter(((BlobsFeatures)m_arrBlobTop[0]).fCenterX - (((BlobsFeatures)m_arrBlobTop[0]).fWidth / 2), m_arrLGauge[0][1].ref_ObjectCenterY + 15);
            m_arrLGauge[0][0].Measure(objROI);

            if (m_arrLGauge[0][0].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][0].ref_GaugeTolerance = Math.Max(10, ((BlobsFeatures)m_arrBlobTop[0]).fWidth / 2);
                m_arrLGauge[0][0].Measure(objROI);
            }

            if (m_arrLGauge[0][0].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Top Side point - Please adjust gauge setting.";
                return false;
            }

            m_arrLGauge[0][2].ref_GaugeAngle = 90;
            m_arrLGauge[0][2].ref_GaugeTransType = 0;
            m_arrLGauge[0][2].ref_GaugeTolerance = Math.Max(5, ((BlobsFeatures)m_arrBlobTop[0]).fWidth / 4);
            m_arrLGauge[0][2].ref_GaugeLength = 10;
            m_arrLGauge[0][2].SetGaugeCenter(((BlobsFeatures)m_arrBlobTop[0]).fCenterX + (((BlobsFeatures)m_arrBlobTop[0]).fWidth / 2), m_arrLGauge[0][1].ref_ObjectCenterY + 15);
            m_arrLGauge[0][2].Measure(objROI);

            if (m_arrLGauge[0][2].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][2].ref_GaugeTolerance = Math.Max(10, ((BlobsFeatures)m_arrBlobTop[0]).fWidth / 2);
                m_arrLGauge[0][2].Measure(objROI);
            }

            if (m_arrLGauge[0][2].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Top Side point - Please adjust gauge setting.";
                return false;
            }

            //2020-12-14 ZJYEOH : measure again because side and center bar got offset
            m_arrLGauge[0][1].ref_GaugeTolerance = Math.Max(5, m_arrROIs[0].ref_ROIHeight / 4);
            m_arrLGauge[0][1].SetGaugeCenter((m_arrLGauge[0][0].ref_ObjectCenterX + m_arrLGauge[0][2].ref_ObjectCenterX) / 2, m_arrROIs[0].ref_ROIPositionY + (m_arrROIs[0].ref_ROIHeight / 4));
            m_arrLGauge[0][1].Measure(objROI);

            if (m_arrLGauge[0][1].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][1].ref_GaugeTolerance = Math.Max(10, m_arrROIs[0].ref_ROIHeight / 2);
                m_arrLGauge[0][1].Measure(objROI);
            }

            if (m_arrLGauge[0][1].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Top Side point - Please adjust gauge setting.";
                return false;
            }

            // Middle bar
            if (m_arrLGauge[0].Count == 3)
                m_arrLGauge[0].Add(new LGauge());//left
            if (m_arrLGauge[0].Count == 4)
                m_arrLGauge[0].Add(new LGauge());//center
            if (m_arrLGauge[0].Count == 5)
                m_arrLGauge[0].Add(new LGauge());//right

            m_arrLGauge[0][4].ref_GaugeAngle = 0;
            m_arrLGauge[0][4].ref_GaugeTransType = 0;
            m_arrLGauge[0][4].ref_GaugeTolerance = Math.Max(5, m_arrROIs[0].ref_ROIHeight / 4);
            m_arrLGauge[0][4].ref_GaugeLength = Math.Max(5, ((BlobsFeatures)m_arrBlobTop[1]).fWidth / 2);
            m_arrLGauge[0][4].SetGaugeCenter(((BlobsFeatures)m_arrBlobTop[1]).fCenterX, m_arrROIs[0].ref_ROIPositionY + (m_arrROIs[0].ref_ROIHeight / 4));
            m_arrLGauge[0][4].Measure(objROI);

            if (m_arrLGauge[0][4].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][4].ref_GaugeTolerance = Math.Max(10, m_arrROIs[0].ref_ROIHeight / 2);
                m_arrLGauge[0][4].Measure(objROI);
            }

            if (m_arrLGauge[0][4].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Top Side point - Please adjust gauge setting.";
                return false;
            }

            m_arrLGauge[0][3].ref_GaugeAngle = 270;
            m_arrLGauge[0][3].ref_GaugeTransType = 0;
            m_arrLGauge[0][3].ref_GaugeTolerance = Math.Max(5, ((BlobsFeatures)m_arrBlobTop[1]).fWidth / 4);
            m_arrLGauge[0][3].ref_GaugeLength = 10;
            m_arrLGauge[0][3].SetGaugeCenter(((BlobsFeatures)m_arrBlobTop[1]).fCenterX - (((BlobsFeatures)m_arrBlobTop[1]).fWidth / 2), m_arrLGauge[0][4].ref_ObjectCenterY + 15);
            m_arrLGauge[0][3].Measure(objROI);

            if (m_arrLGauge[0][3].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][3].ref_GaugeTolerance = Math.Max(10, ((BlobsFeatures)m_arrBlobTop[1]).fWidth / 2);
                m_arrLGauge[0][3].Measure(objROI);
            }

            if (m_arrLGauge[0][3].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Top Side point - Please adjust gauge setting.";
                return false;
            }

            m_arrLGauge[0][5].ref_GaugeAngle = 90;
            m_arrLGauge[0][5].ref_GaugeTransType = 0;
            m_arrLGauge[0][5].ref_GaugeTolerance = Math.Max(5, ((BlobsFeatures)m_arrBlobTop[1]).fWidth / 4);
            m_arrLGauge[0][5].ref_GaugeLength = 10;
            m_arrLGauge[0][5].SetGaugeCenter(((BlobsFeatures)m_arrBlobTop[1]).fCenterX + (((BlobsFeatures)m_arrBlobTop[1]).fWidth / 2), m_arrLGauge[0][4].ref_ObjectCenterY + 15);
            m_arrLGauge[0][5].Measure(objROI);

            if (m_arrLGauge[0][5].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][5].ref_GaugeTolerance = Math.Max(10, ((BlobsFeatures)m_arrBlobTop[1]).fWidth / 2);
                m_arrLGauge[0][5].Measure(objROI);
            }

            if (m_arrLGauge[0][5].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Top Side point - Please adjust gauge setting.";
                return false;
            }

            //2020-12-14 ZJYEOH : measure again because side and center bar got offset
            m_arrLGauge[0][4].ref_GaugeTolerance = Math.Max(5, m_arrROIs[0].ref_ROIHeight / 4);
            m_arrLGauge[0][4].SetGaugeCenter((m_arrLGauge[0][3].ref_ObjectCenterX + m_arrLGauge[0][5].ref_ObjectCenterX) / 2, m_arrROIs[0].ref_ROIPositionY + (m_arrROIs[0].ref_ROIHeight / 4));
            m_arrLGauge[0][4].Measure(objROI);

            if (m_arrLGauge[0][4].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][4].ref_GaugeTolerance = Math.Max(10, m_arrROIs[0].ref_ROIHeight / 2);
                m_arrLGauge[0][4].Measure(objROI);
            }

            if (m_arrLGauge[0][4].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Top Side point - Please adjust gauge setting.";
                return false;
            }

            // Largest bar
            if (m_arrLGauge[0].Count == 6)
                m_arrLGauge[0].Add(new LGauge());//left
            if (m_arrLGauge[0].Count == 7)
                m_arrLGauge[0].Add(new LGauge());//center
            if (m_arrLGauge[0].Count == 8)
                m_arrLGauge[0].Add(new LGauge());//right
            
            m_arrLGauge[0][7].ref_GaugeAngle = 0;
            m_arrLGauge[0][7].ref_GaugeTransType = 0;
            m_arrLGauge[0][7].ref_GaugeTolerance = Math.Max(5, m_arrROIs[0].ref_ROIHeight / 4);
            m_arrLGauge[0][7].ref_GaugeLength = Math.Max(5, ((BlobsFeatures)m_arrBlobTop[2]).fWidth / 2);
            m_arrLGauge[0][7].SetGaugeCenter(((BlobsFeatures)m_arrBlobTop[2]).fCenterX, m_arrROIs[0].ref_ROIPositionY + (m_arrROIs[0].ref_ROIHeight / 4));
            m_arrLGauge[0][7].Measure(objROI);

            if (m_arrLGauge[0][7].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][7].ref_GaugeTolerance = Math.Max(10, m_arrROIs[0].ref_ROIHeight / 2);
                m_arrLGauge[0][7].Measure(objROI);
            }

            if (m_arrLGauge[0][7].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Top Side point - Please adjust gauge setting.";
                return false;
            }

            m_arrLGauge[0][6].ref_GaugeAngle = 270;
            m_arrLGauge[0][6].ref_GaugeTransType = 0;
            m_arrLGauge[0][6].ref_GaugeTolerance = Math.Max(5, ((BlobsFeatures)m_arrBlobTop[2]).fWidth / 4);
            m_arrLGauge[0][6].ref_GaugeLength = 10;
            m_arrLGauge[0][6].SetGaugeCenter(((BlobsFeatures)m_arrBlobTop[2]).fCenterX - (((BlobsFeatures)m_arrBlobTop[2]).fWidth / 2), m_arrLGauge[0][7].ref_ObjectCenterY + 15);
            m_arrLGauge[0][6].Measure(objROI);

            if (m_arrLGauge[0][6].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][6].ref_GaugeTolerance = Math.Max(10, ((BlobsFeatures)m_arrBlobTop[2]).fWidth / 2);
                m_arrLGauge[0][6].Measure(objROI);
            }

            if (m_arrLGauge[0][6].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Top Side point - Please adjust gauge setting.";
                return false;
            }

            m_arrLGauge[0][8].ref_GaugeAngle = 90;
            m_arrLGauge[0][8].ref_GaugeTransType = 0;
            m_arrLGauge[0][8].ref_GaugeTolerance = Math.Max(5, ((BlobsFeatures)m_arrBlobTop[2]).fWidth / 4);
            m_arrLGauge[0][8].ref_GaugeLength = 10;
            m_arrLGauge[0][8].SetGaugeCenter(((BlobsFeatures)m_arrBlobTop[2]).fCenterX + (((BlobsFeatures)m_arrBlobTop[2]).fWidth / 2), m_arrLGauge[0][7].ref_ObjectCenterY + 15);
            m_arrLGauge[0][8].Measure(objROI);

            if (m_arrLGauge[0][8].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][8].ref_GaugeTolerance = Math.Max(10, ((BlobsFeatures)m_arrBlobTop[2]).fWidth / 2);
                m_arrLGauge[0][8].Measure(objROI);
            }

            if (m_arrLGauge[0][8].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Top Side point - Please adjust gauge setting.";
                return false;
            }

            //2020-12-14 ZJYEOH : measure again because side and center bar got offset
            m_arrLGauge[0][7].ref_GaugeTolerance = Math.Max(5, m_arrROIs[0].ref_ROIHeight / 4);
            m_arrLGauge[0][7].SetGaugeCenter((m_arrLGauge[0][6].ref_ObjectCenterX + m_arrLGauge[0][8].ref_ObjectCenterX) / 2, m_arrROIs[0].ref_ROIPositionY + (m_arrROIs[0].ref_ROIHeight / 4));
            m_arrLGauge[0][7].Measure(objROI);

            if (m_arrLGauge[0][7].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][7].ref_GaugeTolerance = Math.Max(10, m_arrROIs[0].ref_ROIHeight / 2);
                m_arrLGauge[0][7].Measure(objROI);
            }

            if (m_arrLGauge[0][7].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Top Side point - Please adjust gauge setting.";
                return false;
            }

            if (blnUpdateData)
            {
                if (m_fRotateAngle != 0)
                    m_f2DCenterPixelCountX1 = FixAngle(m_fRotateAngle, 0, 2, 0, true);
                else
                    m_f2DCenterPixelCountX1 = m_arrLGauge[0][2].ref_ObjectCenterX - m_arrLGauge[0][0].ref_ObjectCenterX;

                if (m_fRotateAngle != 0)
                    m_f2DCenterPixelCountX2 = FixAngle(m_fRotateAngle, 0, 5, 3, true);
                else
                    m_f2DCenterPixelCountX2 = m_arrLGauge[0][5].ref_ObjectCenterX - m_arrLGauge[0][3].ref_ObjectCenterX;

                if (m_fRotateAngle != 0)
                    m_f2DCenterPixelCountX3 = FixAngle(m_fRotateAngle, 0, 8, 6, true);
                else
                    m_f2DCenterPixelCountX3 = m_arrLGauge[0][8].ref_ObjectCenterX - m_arrLGauge[0][6].ref_ObjectCenterX;

                m_fCalibration2DX = m_f2DCenterPixelCountX3 / m_fSize2DC3;

                if (m_fRotateAngle != 0)
                    m_f2DCenterPixelCountX4 = FixAngle(m_fRotateAngle, 0, 5, 0, true);
                else
                    m_f2DCenterPixelCountX4 = m_arrLGauge[0][5].ref_ObjectCenterX - m_arrLGauge[0][0].ref_ObjectCenterX;

                if (m_fRotateAngle != 0)
                    m_f2DCenterPixelCountX5 = FixAngle(m_fRotateAngle, 0, 8, 0, true);
                else
                    m_f2DCenterPixelCountX5 = m_arrLGauge[0][8].ref_ObjectCenterX - m_arrLGauge[0][0].ref_ObjectCenterX;

                float fDistanceT3 = m_fSize3DT3 * (m_f3DTopPixelCount1 / m_fSize3DT1);

                if (m_fRotateAngleSide != 0)
                    m_fTopDistance = FixAngle(m_fRotateAngleSide, 1, 0, 6, false) - fDistanceT3;
                else
                    m_fTopDistance = m_arrLGauge[1][0].ref_ObjectCenterY - m_arrLGauge[1][6].ref_ObjectCenterY - fDistanceT3;

                float DistanceA1 = 0;

                if (m_fRotateAngle != 0)
                    DistanceA1 = FixAngle(m_fRotateAngle, 0, 1, 7, false);
                else
                    DistanceA1 = m_arrLGauge[0][1].ref_ObjectCenterY - m_arrLGauge[0][7].ref_ObjectCenterY;

                m_fTopAngle = (float)(Math.Asin(m_fTopDistance / DistanceA1) * 180 / Math.PI);
            }

            if (float.IsNaN(m_fTopAngle) || float.IsInfinity(m_fTopAngle))
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Top Angle - Please adjust gauge or threshold setting.";
                return false;
            }

            objImgTemp.Dispose();
            objROI.Dispose();
            return true;
        }
        public bool MeasureCenterHorizontalBottom(ImageDrawing objImage, bool blnUpdateData)
        {
            if (m_arrBlobBottom.Count != 3)
                return false;

            ImageDrawing objImgTemp = new ImageDrawing(true, objImage.ref_intCameraResolutionWidth, objImage.ref_intCameraResolutionHeight);
            objImage.CopyTo(objImgTemp);
            ROI objROI = new ROI();
            objROI.LoadROISetting(m_arrROIs[0].ref_ROIPositionX, m_arrROIs[0].ref_ROIPositionY, m_arrROIs[0].ref_ROIWidth, m_arrROIs[0].ref_ROIHeight);
            objROI.AttachImage(objImgTemp);
            //EasyImage.Threshold(m_arrROIs[0].ref_ROI, objROI.ref_ROI, m_intThresholdCenter);


            // Smallest bar
            if (m_arrLGauge[0].Count == 9)
                m_arrLGauge[0].Add(new LGauge());//left
            if (m_arrLGauge[0].Count == 10)
                m_arrLGauge[0].Add(new LGauge());//center
            if (m_arrLGauge[0].Count == 11)
                m_arrLGauge[0].Add(new LGauge());//right

            m_arrLGauge[0][10].ref_GaugeAngle = 180;
            m_arrLGauge[0][10].ref_GaugeTransType = 0;
            m_arrLGauge[0][10].ref_GaugeTolerance = Math.Max(5, m_arrROIs[0].ref_ROIHeight / 4);
            m_arrLGauge[0][10].ref_GaugeLength = Math.Max(5, Math.Max(5, ((BlobsFeatures)m_arrBlobBottom[0]).fWidth / 2));
            m_arrLGauge[0][10].SetGaugeCenter(((BlobsFeatures)m_arrBlobBottom[0]).fCenterX, m_arrROIs[0].ref_ROIPositionY + (m_arrROIs[0].ref_ROIHeight * 3 / 4));
            m_arrLGauge[0][10].Measure(objROI);

            if (m_arrLGauge[0][10].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][10].ref_GaugeTolerance = Math.Max(10, m_arrROIs[0].ref_ROIHeight / 2);
                m_arrLGauge[0][10].Measure(objROI);
            }

            if (m_arrLGauge[0][10].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Bottom Side point - Please adjust gauge setting.";
                return false;
            }

            m_arrLGauge[0][9].ref_GaugeAngle = 270;
            m_arrLGauge[0][9].ref_GaugeTransType = 0;
            m_arrLGauge[0][9].ref_GaugeTolerance = Math.Max(5, ((BlobsFeatures)m_arrBlobBottom[0]).fWidth / 4);
            m_arrLGauge[0][9].ref_GaugeLength = 10;
            m_arrLGauge[0][9].SetGaugeCenter(((BlobsFeatures)m_arrBlobBottom[0]).fCenterX - (((BlobsFeatures)m_arrBlobBottom[0]).fWidth / 2), m_arrLGauge[0][10].ref_ObjectCenterY - 15);
            m_arrLGauge[0][9].Measure(objROI);

            if (m_arrLGauge[0][9].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][9].ref_GaugeTolerance = Math.Max(10, ((BlobsFeatures)m_arrBlobBottom[0]).fWidth / 2);
                m_arrLGauge[0][9].Measure(objROI);
            }

            if (m_arrLGauge[0][9].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Bottom Side point - Please adjust gauge setting.";
                return false;
            }

            m_arrLGauge[0][11].ref_GaugeAngle = 90;
            m_arrLGauge[0][11].ref_GaugeTransType = 0;
            m_arrLGauge[0][11].ref_GaugeTolerance = Math.Max(5, ((BlobsFeatures)m_arrBlobBottom[0]).fWidth / 4);
            m_arrLGauge[0][11].ref_GaugeLength = 10;
            m_arrLGauge[0][11].SetGaugeCenter(((BlobsFeatures)m_arrBlobBottom[0]).fCenterX + (((BlobsFeatures)m_arrBlobBottom[0]).fWidth / 2), m_arrLGauge[0][10].ref_ObjectCenterY - 15);
            m_arrLGauge[0][11].Measure(objROI);

            if (m_arrLGauge[0][11].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][11].ref_GaugeTolerance = Math.Max(10, ((BlobsFeatures)m_arrBlobBottom[0]).fWidth / 2);
                m_arrLGauge[0][11].Measure(objROI);
            }

            if (m_arrLGauge[0][11].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Bottom Side point - Please adjust gauge setting.";
                return false;
            }

            //2020-12-14 ZJYEOH : measure again because side and center bar got offset
            m_arrLGauge[0][10].ref_GaugeTolerance = Math.Max(5, m_arrROIs[0].ref_ROIHeight / 4);
            m_arrLGauge[0][10].SetGaugeCenter((m_arrLGauge[0][9].ref_ObjectCenterX + m_arrLGauge[0][11].ref_ObjectCenterX) / 2, m_arrROIs[0].ref_ROIPositionY + (m_arrROIs[0].ref_ROIHeight * 3 / 4));
            m_arrLGauge[0][10].Measure(objROI);

            if (m_arrLGauge[0][10].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][10].ref_GaugeTolerance = Math.Max(10, m_arrROIs[0].ref_ROIHeight / 2);
                m_arrLGauge[0][10].Measure(objROI);
            }

            if (m_arrLGauge[0][10].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Bottom Side point - Please adjust gauge setting.";
                return false;
            }

            // Middle bar
            if (m_arrLGauge[0].Count == 12)
                m_arrLGauge[0].Add(new LGauge());//left
            if (m_arrLGauge[0].Count == 13)
                m_arrLGauge[0].Add(new LGauge());//center
            if (m_arrLGauge[0].Count == 14)
                m_arrLGauge[0].Add(new LGauge());//right

            m_arrLGauge[0][13].ref_GaugeAngle = 180;
            m_arrLGauge[0][13].ref_GaugeTransType = 0;
            m_arrLGauge[0][13].ref_GaugeTolerance = Math.Max(5, m_arrROIs[0].ref_ROIHeight / 4);
            m_arrLGauge[0][13].ref_GaugeLength = Math.Max(5, ((BlobsFeatures)m_arrBlobBottom[1]).fWidth / 2);
            m_arrLGauge[0][13].SetGaugeCenter(((BlobsFeatures)m_arrBlobBottom[1]).fCenterX, m_arrROIs[0].ref_ROIPositionY + (m_arrROIs[0].ref_ROIHeight * 3 / 4));
            m_arrLGauge[0][13].Measure(objROI);

            if (m_arrLGauge[0][13].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][13].ref_GaugeTolerance = Math.Max(10, m_arrROIs[0].ref_ROIHeight / 2);
                m_arrLGauge[0][13].Measure(objROI);
            }

            if (m_arrLGauge[0][13].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Bottom Side point - Please adjust gauge setting.";
                return false;
            }

            m_arrLGauge[0][12].ref_GaugeAngle = 270;
            m_arrLGauge[0][12].ref_GaugeTransType = 0;
            m_arrLGauge[0][12].ref_GaugeTolerance = Math.Max(5, ((BlobsFeatures)m_arrBlobBottom[1]).fWidth / 4);
            m_arrLGauge[0][12].ref_GaugeLength = 10;
            m_arrLGauge[0][12].SetGaugeCenter(((BlobsFeatures)m_arrBlobBottom[1]).fCenterX - (((BlobsFeatures)m_arrBlobBottom[1]).fWidth / 2), m_arrLGauge[0][13].ref_ObjectCenterY - 15);
            m_arrLGauge[0][12].Measure(objROI);

            if (m_arrLGauge[0][12].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][12].ref_GaugeTolerance = Math.Max(10, ((BlobsFeatures)m_arrBlobBottom[1]).fWidth / 2);
                m_arrLGauge[0][12].Measure(objROI);
            }

            if (m_arrLGauge[0][12].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Bottom Side point - Please adjust gauge setting.";
                return false;
            }

            m_arrLGauge[0][14].ref_GaugeAngle = 90;
            m_arrLGauge[0][14].ref_GaugeTransType = 0;
            m_arrLGauge[0][14].ref_GaugeTolerance = Math.Max(5, ((BlobsFeatures)m_arrBlobBottom[1]).fWidth / 4);
            m_arrLGauge[0][14].ref_GaugeLength = 10;
            m_arrLGauge[0][14].SetGaugeCenter(((BlobsFeatures)m_arrBlobBottom[1]).fCenterX + (((BlobsFeatures)m_arrBlobBottom[1]).fWidth / 2), m_arrLGauge[0][13].ref_ObjectCenterY - 15);
            m_arrLGauge[0][14].Measure(objROI);

            if (m_arrLGauge[0][14].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][14].ref_GaugeTolerance = Math.Max(10, ((BlobsFeatures)m_arrBlobBottom[1]).fWidth / 2);
                m_arrLGauge[0][14].Measure(objROI);
            }

            if (m_arrLGauge[0][14].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Bottom Side point - Please adjust gauge setting.";
                return false;
            }

            //2020-12-14 ZJYEOH : measure again because side and center bar got offset
            m_arrLGauge[0][13].ref_GaugeTolerance = Math.Max(5, m_arrROIs[0].ref_ROIHeight / 4);
            m_arrLGauge[0][13].SetGaugeCenter((m_arrLGauge[0][12].ref_ObjectCenterX + m_arrLGauge[0][14].ref_ObjectCenterX) / 2, m_arrROIs[0].ref_ROIPositionY + (m_arrROIs[0].ref_ROIHeight * 3 / 4));
            m_arrLGauge[0][13].Measure(objROI);

            if (m_arrLGauge[0][13].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][13].ref_GaugeTolerance = Math.Max(10, m_arrROIs[0].ref_ROIHeight / 2);
                m_arrLGauge[0][13].Measure(objROI);
            }

            if (m_arrLGauge[0][13].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Bottom Side point - Please adjust gauge setting.";
                return false;
            }

            // Largest bar
            if (m_arrLGauge[0].Count == 15)
                m_arrLGauge[0].Add(new LGauge());//left
            if (m_arrLGauge[0].Count == 16)
                m_arrLGauge[0].Add(new LGauge());//center
            if (m_arrLGauge[0].Count == 17)
                m_arrLGauge[0].Add(new LGauge());//right
            
            m_arrLGauge[0][16].ref_GaugeAngle = 180;
            m_arrLGauge[0][16].ref_GaugeTransType = 0;
            m_arrLGauge[0][16].ref_GaugeTolerance = Math.Max(5, m_arrROIs[0].ref_ROIHeight / 4);
            m_arrLGauge[0][16].ref_GaugeLength = Math.Max(5, ((BlobsFeatures)m_arrBlobBottom[2]).fWidth / 2);
            m_arrLGauge[0][16].SetGaugeCenter(((BlobsFeatures)m_arrBlobBottom[2]).fCenterX, m_arrROIs[0].ref_ROIPositionY + (m_arrROIs[0].ref_ROIHeight * 3 / 4));
            m_arrLGauge[0][16].Measure(objROI);

            if (m_arrLGauge[0][16].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][16].ref_GaugeTolerance = Math.Max(10, m_arrROIs[0].ref_ROIHeight / 2);
                m_arrLGauge[0][16].Measure(objROI);
            }

            if (m_arrLGauge[0][16].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Bottom Side point - Please adjust gauge setting.";
                return false;
            }

            m_arrLGauge[0][15].ref_GaugeAngle = 270;
            m_arrLGauge[0][15].ref_GaugeTransType = 0;
            m_arrLGauge[0][15].ref_GaugeTolerance = Math.Max(5, ((BlobsFeatures)m_arrBlobBottom[2]).fWidth / 4);
            m_arrLGauge[0][15].ref_GaugeLength = 10;
            m_arrLGauge[0][15].SetGaugeCenter(((BlobsFeatures)m_arrBlobBottom[2]).fCenterX - (((BlobsFeatures)m_arrBlobBottom[2]).fWidth / 2), m_arrLGauge[0][16].ref_ObjectCenterY - 15);
            m_arrLGauge[0][15].Measure(objROI);

            if (m_arrLGauge[0][15].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][15].ref_GaugeTolerance = Math.Max(10, ((BlobsFeatures)m_arrBlobBottom[2]).fWidth / 2);
                m_arrLGauge[0][15].Measure(objROI);
            }

            if (m_arrLGauge[0][15].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Bottom Side point - Please adjust gauge setting.";
                return false;
            }

            m_arrLGauge[0][17].ref_GaugeAngle = 90;
            m_arrLGauge[0][17].ref_GaugeTransType = 0;
            m_arrLGauge[0][17].ref_GaugeTolerance = Math.Max(5, ((BlobsFeatures)m_arrBlobBottom[2]).fWidth / 4);
            m_arrLGauge[0][17].ref_GaugeLength = 10;
            m_arrLGauge[0][17].SetGaugeCenter(((BlobsFeatures)m_arrBlobBottom[2]).fCenterX + (((BlobsFeatures)m_arrBlobBottom[2]).fWidth / 2), m_arrLGauge[0][16].ref_ObjectCenterY - 15);
            m_arrLGauge[0][17].Measure(objROI);

            if (m_arrLGauge[0][17].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][17].ref_GaugeTolerance = Math.Max(10, ((BlobsFeatures)m_arrBlobBottom[2]).fWidth / 2);
                m_arrLGauge[0][17].Measure(objROI);
            }

            if (m_arrLGauge[0][17].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Bottom Side point - Please adjust gauge setting.";
                return false;
            }

            //2020-12-14 ZJYEOH : measure again because side and center bar got offset
            m_arrLGauge[0][16].ref_GaugeTolerance = Math.Max(5, m_arrROIs[0].ref_ROIHeight / 4);
            m_arrLGauge[0][16].SetGaugeCenter((m_arrLGauge[0][15].ref_ObjectCenterX + m_arrLGauge[0][17].ref_ObjectCenterX) / 2, m_arrROIs[0].ref_ROIPositionY + (m_arrROIs[0].ref_ROIHeight * 3 / 4));
            m_arrLGauge[0][16].Measure(objROI);

            if (m_arrLGauge[0][16].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][16].ref_GaugeTolerance = Math.Max(10, m_arrROIs[0].ref_ROIHeight / 2);
                m_arrLGauge[0][16].Measure(objROI);
            }

            if (m_arrLGauge[0][16].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Bottom Side point - Please adjust gauge setting.";
                return false;
            }

            if (blnUpdateData)
            {
                //m_f2DCenterPixelCount1 = m_arrLGauge[0][11].ref_ObjectCenterX - m_arrLGauge[0][9].ref_ObjectCenterX;

                //m_f2DCenterPixelCount2 = m_arrLGauge[0][14].ref_ObjectCenterX - m_arrLGauge[0][12].ref_ObjectCenterX;

                //m_f2DCenterPixelCount3 = m_arrLGauge[0][17].ref_ObjectCenterX - m_arrLGauge[0][15].ref_ObjectCenterX;

                //m_f2DCenterPixelCount4 = m_arrLGauge[0][14].ref_ObjectCenterX - m_arrLGauge[0][9].ref_ObjectCenterX;

                //m_f2DCenterPixelCount5 = m_arrLGauge[0][17].ref_ObjectCenterX - m_arrLGauge[0][9].ref_ObjectCenterX;

                float fDistanceB3 = m_fSize3DB3 * (m_f3DBottomPixelCount1 / m_fSize3DB1);

                if (m_fRotateAngleSide != 0)
                    m_fBottomDistance = FixAngle(m_fRotateAngleSide, 3, 6, 0, false) - fDistanceB3;
                else
                    m_fBottomDistance = m_arrLGauge[3][6].ref_ObjectCenterY - m_arrLGauge[3][0].ref_ObjectCenterY - fDistanceB3;

                float fDistanceA2 = 0;

                if (m_fRotateAngle != 0)
                    fDistanceA2 = FixAngle(m_fRotateAngle, 0, 16, 10, false);
                else
                    fDistanceA2 = m_arrLGauge[0][16].ref_ObjectCenterY - m_arrLGauge[0][10].ref_ObjectCenterY;

                m_fBottomAngle = (float)(Math.Asin(m_fBottomDistance / fDistanceA2) * 180 / Math.PI);
            }

            if (float.IsNaN(m_fBottomAngle) || float.IsInfinity(m_fBottomAngle))
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Bottom Angle - Please adjust gauge or threshold setting.";
                return false;
            }

            objImgTemp.Dispose();
            objROI.Dispose();
            return true;
        }
        public bool MeasureCenterVerticalLeft(ImageDrawing objImage, bool blnUpdateData)
        {
            if (m_arrBlobLeft.Count != 3)
                return false;

            ImageDrawing objImgTemp = new ImageDrawing(true, objImage.ref_intCameraResolutionWidth, objImage.ref_intCameraResolutionHeight);
            objImage.CopyTo(objImgTemp);
            ROI objROI = new ROI();
            objROI.LoadROISetting(m_arrROIs[0].ref_ROIPositionX, m_arrROIs[0].ref_ROIPositionY, m_arrROIs[0].ref_ROIWidth, m_arrROIs[0].ref_ROIHeight);
            objROI.AttachImage(objImgTemp);
#if (Debug_2_12 || Release_2_12)
            EasyImage.Threshold(m_arrROIs[0].ref_ROI, objROI.ref_ROI, (uint)m_intThresholdCenter);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            EasyImage.Threshold(m_arrROIs[0].ref_ROI, objROI.ref_ROI, m_intThresholdCenter);
#endif



            // Largest bar
            if (m_arrLGauge[0].Count == 0)
                m_arrLGauge[0].Add(new LGauge());//top
            if (m_arrLGauge[0].Count == 1)
                m_arrLGauge[0].Add(new LGauge());//center
            if (m_arrLGauge[0].Count == 2)
                m_arrLGauge[0].Add(new LGauge());//bottom

            m_arrLGauge[0][1].ref_GaugeAngle = 270;
            m_arrLGauge[0][1].ref_GaugeTransType = 0;
            m_arrLGauge[0][1].ref_GaugeTolerance = Math.Max(5, m_arrROIs[0].ref_ROIWidth / 4);
            m_arrLGauge[0][1].ref_GaugeLength = Math.Max(5, ((BlobsFeatures)m_arrBlobLeft[0]).fHeight / 2);
            m_arrLGauge[0][1].SetGaugeCenter(m_arrROIs[0].ref_ROIPositionX + (m_arrROIs[0].ref_ROIWidth / 4), ((BlobsFeatures)m_arrBlobLeft[0]).fCenterY);
            m_arrLGauge[0][1].Measure(objROI);

            if (m_arrLGauge[0][1].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][1].ref_GaugeTolerance = Math.Max(10, m_arrROIs[0].ref_ROIWidth / 2);
                m_arrLGauge[0][1].Measure(objROI);
            }

            if (m_arrLGauge[0][1].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Left Side point - Please adjust gauge setting.";
                return false;
            }

            m_arrLGauge[0][0].ref_GaugeAngle = 0;
            m_arrLGauge[0][0].ref_GaugeTransType = 0;
            m_arrLGauge[0][0].ref_GaugeTolerance = Math.Max(5, ((BlobsFeatures)m_arrBlobLeft[0]).fHeight / 4);
            m_arrLGauge[0][0].ref_GaugeLength = 10;
            m_arrLGauge[0][0].SetGaugeCenter(m_arrLGauge[0][1].ref_ObjectCenterX + 15, ((BlobsFeatures)m_arrBlobLeft[0]).fCenterY - (((BlobsFeatures)m_arrBlobLeft[0]).fHeight / 2));
            m_arrLGauge[0][0].Measure(objROI);

            if (m_arrLGauge[0][0].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][0].ref_GaugeTolerance = Math.Max(10, ((BlobsFeatures)m_arrBlobLeft[0]).fHeight / 2);
                m_arrLGauge[0][0].Measure(objROI);
            }

            if (m_arrLGauge[0][0].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Left Side point - Please adjust gauge setting.";
                return false;
            }

            m_arrLGauge[0][2].ref_GaugeAngle = 180;
            m_arrLGauge[0][2].ref_GaugeTransType = 0;
            m_arrLGauge[0][2].ref_GaugeTolerance = Math.Max(5, ((BlobsFeatures)m_arrBlobLeft[0]).fHeight / 4);
            m_arrLGauge[0][2].ref_GaugeLength = 10;
            m_arrLGauge[0][2].SetGaugeCenter(m_arrLGauge[0][1].ref_ObjectCenterX + 15, ((BlobsFeatures)m_arrBlobLeft[0]).fCenterY + (((BlobsFeatures)m_arrBlobLeft[0]).fHeight / 2));
            m_arrLGauge[0][2].Measure(objROI);

            if (m_arrLGauge[0][2].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][2].ref_GaugeTolerance = Math.Max(10, ((BlobsFeatures)m_arrBlobLeft[0]).fHeight / 2);
                m_arrLGauge[0][2].Measure(objROI);
            }

            if (m_arrLGauge[0][2].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Left Side point - Please adjust gauge setting.";
                return false;
            }

            //2020-12-14 ZJYEOH : measure again because side and center bar got offset
            m_arrLGauge[0][1].ref_GaugeTolerance = Math.Max(5, m_arrROIs[0].ref_ROIWidth / 4);
            m_arrLGauge[0][1].SetGaugeCenter(m_arrROIs[0].ref_ROIPositionX + (m_arrROIs[0].ref_ROIWidth / 4), (m_arrLGauge[0][0].ref_ObjectCenterY + m_arrLGauge[0][2].ref_ObjectCenterY) / 2);
            m_arrLGauge[0][1].Measure(objROI);

            if (m_arrLGauge[0][1].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][1].ref_GaugeTolerance = Math.Max(10, m_arrROIs[0].ref_ROIWidth / 2);
                m_arrLGauge[0][1].Measure(objROI);
            }

            if (m_arrLGauge[0][1].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Left Side point - Please adjust gauge setting.";
                return false;
            }

            // Middle bar
            if (m_arrLGauge[0].Count == 3)
                m_arrLGauge[0].Add(new LGauge());//top
            if (m_arrLGauge[0].Count == 4)
                m_arrLGauge[0].Add(new LGauge());//center
            if (m_arrLGauge[0].Count == 5)
                m_arrLGauge[0].Add(new LGauge());//bottom

            m_arrLGauge[0][4].ref_GaugeAngle = 270;
            m_arrLGauge[0][4].ref_GaugeTransType = 0;
            m_arrLGauge[0][4].ref_GaugeTolerance = Math.Max(5, m_arrROIs[0].ref_ROIWidth / 4);
            m_arrLGauge[0][4].ref_GaugeLength = Math.Max(5, ((BlobsFeatures)m_arrBlobLeft[1]).fHeight / 2);
            m_arrLGauge[0][4].SetGaugeCenter(m_arrROIs[0].ref_ROIPositionX + (m_arrROIs[0].ref_ROIWidth / 4), ((BlobsFeatures)m_arrBlobLeft[1]).fCenterY);
            m_arrLGauge[0][4].Measure(objROI);

            if (m_arrLGauge[0][4].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][4].ref_GaugeTolerance = Math.Max(10, m_arrROIs[0].ref_ROIWidth / 2);
                m_arrLGauge[0][4].Measure(objROI);
            }

            if (m_arrLGauge[0][4].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Left Side point - Please adjust gauge setting.";
                return false;
            }

            m_arrLGauge[0][3].ref_GaugeAngle = 0;
            m_arrLGauge[0][3].ref_GaugeTransType = 0;
            m_arrLGauge[0][3].ref_GaugeTolerance = Math.Max(5, ((BlobsFeatures)m_arrBlobLeft[1]).fHeight / 4);
            m_arrLGauge[0][3].ref_GaugeLength = 10;
            m_arrLGauge[0][3].SetGaugeCenter(m_arrLGauge[0][4].ref_ObjectCenterX + 15, ((BlobsFeatures)m_arrBlobLeft[1]).fCenterY - (((BlobsFeatures)m_arrBlobLeft[1]).fHeight / 2));
            m_arrLGauge[0][3].Measure(objROI);

            if (m_arrLGauge[0][3].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][3].ref_GaugeTolerance = Math.Max(10, ((BlobsFeatures)m_arrBlobLeft[1]).fHeight / 2);
                m_arrLGauge[0][3].Measure(objROI);
            }

            if (m_arrLGauge[0][3].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Left Side point - Please adjust gauge setting.";
                return false;
            }

            m_arrLGauge[0][5].ref_GaugeAngle = 180;
            m_arrLGauge[0][5].ref_GaugeTransType = 0;
            m_arrLGauge[0][5].ref_GaugeTolerance = Math.Max(5, ((BlobsFeatures)m_arrBlobLeft[1]).fHeight / 4);
            m_arrLGauge[0][5].ref_GaugeLength = 10;
            m_arrLGauge[0][5].SetGaugeCenter(m_arrLGauge[0][4].ref_ObjectCenterX + 15, ((BlobsFeatures)m_arrBlobLeft[1]).fCenterY + (((BlobsFeatures)m_arrBlobLeft[1]).fHeight / 2));
            m_arrLGauge[0][5].Measure(objROI);

            if (m_arrLGauge[0][5].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][5].ref_GaugeTolerance = Math.Max(10, ((BlobsFeatures)m_arrBlobLeft[1]).fHeight / 2);
                m_arrLGauge[0][5].Measure(objROI);
            }

            if (m_arrLGauge[0][5].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Left Side point - Please adjust gauge setting.";
                return false;
            }

            //2020-12-14 ZJYEOH : measure again because side and center bar got offset
            m_arrLGauge[0][4].ref_GaugeTolerance = Math.Max(5, m_arrROIs[0].ref_ROIWidth / 4);
            m_arrLGauge[0][4].SetGaugeCenter(m_arrROIs[0].ref_ROIPositionX + (m_arrROIs[0].ref_ROIWidth / 4), (m_arrLGauge[0][3].ref_ObjectCenterY + m_arrLGauge[0][5].ref_ObjectCenterY) / 2);
            m_arrLGauge[0][4].Measure(objROI);

            if (m_arrLGauge[0][4].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][4].ref_GaugeTolerance = Math.Max(10, m_arrROIs[0].ref_ROIWidth / 2);
                m_arrLGauge[0][4].Measure(objROI);
            }

            if (m_arrLGauge[0][4].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Left Side point - Please adjust gauge setting.";
                return false;
            }

            // Smallest bar
            if (m_arrLGauge[0].Count == 6)
                m_arrLGauge[0].Add(new LGauge());//top
            if (m_arrLGauge[0].Count == 7)
                m_arrLGauge[0].Add(new LGauge());//center
            if (m_arrLGauge[0].Count == 8)
                m_arrLGauge[0].Add(new LGauge());//bottom


            m_arrLGauge[0][7].ref_GaugeAngle = 270;
            m_arrLGauge[0][7].ref_GaugeTransType = 0;
            m_arrLGauge[0][7].ref_GaugeTolerance = Math.Max(5, m_arrROIs[0].ref_ROIWidth / 4);
            m_arrLGauge[0][7].ref_GaugeLength = Math.Max(5, ((BlobsFeatures)m_arrBlobLeft[2]).fHeight / 2);
            m_arrLGauge[0][7].SetGaugeCenter(m_arrROIs[0].ref_ROIPositionX + (m_arrROIs[0].ref_ROIWidth / 4), ((BlobsFeatures)m_arrBlobLeft[2]).fCenterY);
            m_arrLGauge[0][7].Measure(objROI);

            if (m_arrLGauge[0][7].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][7].ref_GaugeTolerance = Math.Max(10, m_arrROIs[0].ref_ROIWidth / 2);
                m_arrLGauge[0][7].Measure(objROI);
            }

            if (m_arrLGauge[0][7].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Left Side point - Please adjust gauge setting.";
                return false;
            }

            m_arrLGauge[0][6].ref_GaugeAngle = 0;
            m_arrLGauge[0][6].ref_GaugeTransType = 0;
            m_arrLGauge[0][6].ref_GaugeTolerance = Math.Max(5, ((BlobsFeatures)m_arrBlobLeft[2]).fHeight / 4);
            m_arrLGauge[0][6].ref_GaugeLength = 10;
            m_arrLGauge[0][6].SetGaugeCenter(m_arrLGauge[0][7].ref_ObjectCenterX + 15, ((BlobsFeatures)m_arrBlobLeft[2]).fCenterY - (((BlobsFeatures)m_arrBlobLeft[2]).fHeight / 2));
            m_arrLGauge[0][6].Measure(objROI);

            if (m_arrLGauge[0][6].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][6].ref_GaugeTolerance = Math.Max(10, ((BlobsFeatures)m_arrBlobLeft[2]).fHeight / 2);
                m_arrLGauge[0][6].Measure(objROI);
            }

            if (m_arrLGauge[0][6].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Left Side point - Please adjust gauge setting.";
                return false;
            }

            m_arrLGauge[0][8].ref_GaugeAngle = 180;
            m_arrLGauge[0][8].ref_GaugeTransType = 0;
            m_arrLGauge[0][8].ref_GaugeTolerance = Math.Max(5, ((BlobsFeatures)m_arrBlobLeft[2]).fHeight / 4);
            m_arrLGauge[0][8].ref_GaugeLength = 10;
            m_arrLGauge[0][8].SetGaugeCenter(m_arrLGauge[0][7].ref_ObjectCenterX + 15, ((BlobsFeatures)m_arrBlobLeft[2]).fCenterY + (((BlobsFeatures)m_arrBlobLeft[2]).fHeight / 2));
            m_arrLGauge[0][8].Measure(objROI);

            if (m_arrLGauge[0][8].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][8].ref_GaugeTolerance = Math.Max(10, ((BlobsFeatures)m_arrBlobLeft[2]).fHeight / 2);
                m_arrLGauge[0][8].Measure(objROI);
            }

            if (m_arrLGauge[0][8].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Left Side point - Please adjust gauge setting.";
                return false;
            }

            //2020-12-14 ZJYEOH : measure again because side and center bar got offset
            m_arrLGauge[0][7].ref_GaugeTolerance = Math.Max(5, m_arrROIs[0].ref_ROIWidth / 4);
            m_arrLGauge[0][7].SetGaugeCenter(m_arrROIs[0].ref_ROIPositionX + (m_arrROIs[0].ref_ROIWidth / 4), (m_arrLGauge[0][6].ref_ObjectCenterY + m_arrLGauge[0][8].ref_ObjectCenterY) / 2);
            m_arrLGauge[0][7].Measure(objROI);

            if (m_arrLGauge[0][7].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][7].ref_GaugeTolerance = Math.Max(10, m_arrROIs[0].ref_ROIWidth / 2);
                m_arrLGauge[0][7].Measure(objROI);
            }

            if (m_arrLGauge[0][7].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Left Side point - Please adjust gauge setting.";
                return false;
            }

            if (blnUpdateData)
            {
                if (m_fRotateAngle != 0)
                    m_f2DCenterPixelCountY1 = FixAngle(m_fRotateAngle, 0, 8, 6, false);
                else
                    m_f2DCenterPixelCountY1 = m_arrLGauge[0][8].ref_ObjectCenterY - m_arrLGauge[0][6].ref_ObjectCenterY;

                if (m_fRotateAngle != 0)
                    m_f2DCenterPixelCountY2 = FixAngle(m_fRotateAngle, 0, 5, 3, false);
                else
                    m_f2DCenterPixelCountY2 = m_arrLGauge[0][5].ref_ObjectCenterY - m_arrLGauge[0][3].ref_ObjectCenterY;

                if (m_fRotateAngle != 0)
                    m_f2DCenterPixelCountY3 = FixAngle(m_fRotateAngle, 0, 2, 0, false);
                else
                    m_f2DCenterPixelCountY3 = m_arrLGauge[0][2].ref_ObjectCenterY - m_arrLGauge[0][0].ref_ObjectCenterY;

                m_fCalibration2DY = m_f2DCenterPixelCountY3 / m_fSize2DC3;

                if (m_fRotateAngle != 0)
                    m_f2DCenterPixelCountY4 = FixAngle(m_fRotateAngle, 0, 8, 3, false);
                else
                    m_f2DCenterPixelCountY4 = m_arrLGauge[0][8].ref_ObjectCenterY - m_arrLGauge[0][3].ref_ObjectCenterY;

                if (m_fRotateAngle != 0)
                    m_f2DCenterPixelCountY5 = FixAngle(m_fRotateAngle, 0, 8, 0, false);
                else
                    m_f2DCenterPixelCountY5 = m_arrLGauge[0][8].ref_ObjectCenterY - m_arrLGauge[0][0].ref_ObjectCenterY;

                float fDistanceL3 = m_fSize3DL3 * (m_f3DLeftPixelCount1 / m_fSize3DL1);

                if (m_fRotateAngleSide != 0)
                    m_fLeftDistance = FixAngle(m_fRotateAngleSide, 4, 0, 6, true) - fDistanceL3;
                else
                    m_fLeftDistance = m_arrLGauge[4][0].ref_ObjectCenterX - m_arrLGauge[4][6].ref_ObjectCenterX - fDistanceL3;

                float fDistanceA1 = 0;

                if (m_fRotateAngle != 0)
                    fDistanceA1 = FixAngle(m_fRotateAngle, 0, 7, 1, true);
                else
                    fDistanceA1 = m_arrLGauge[0][7].ref_ObjectCenterX - m_arrLGauge[0][1].ref_ObjectCenterX;

                m_fLeftAngle = (float)(Math.Asin(m_fLeftDistance / fDistanceA1) * 180 / Math.PI);
            }

            if (float.IsNaN(m_fLeftAngle) || float.IsInfinity(m_fLeftAngle))
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Left Angle - Please adjust gauge or threshold setting.";
                return false;
            }

            objImgTemp.Dispose();
            objROI.Dispose();
            return true;
        }
        public bool MeasureCenterVerticalRight(ImageDrawing objImage, bool blnUpdateData)
        {
            if (m_arrBlobRight.Count != 3)
                return false;

            ImageDrawing objImgTemp = new ImageDrawing(true, objImage.ref_intCameraResolutionWidth, objImage.ref_intCameraResolutionHeight);
            objImage.CopyTo(objImgTemp);
            ROI objROI = new ROI();
            objROI.LoadROISetting(m_arrROIs[0].ref_ROIPositionX, m_arrROIs[0].ref_ROIPositionY, m_arrROIs[0].ref_ROIWidth, m_arrROIs[0].ref_ROIHeight);
            objROI.AttachImage(objImgTemp);
#if (Debug_2_12 || Release_2_12)
            EasyImage.Threshold(m_arrROIs[0].ref_ROI, objROI.ref_ROI, (uint)m_intThresholdCenter);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            EasyImage.Threshold(m_arrROIs[0].ref_ROI, objROI.ref_ROI, m_intThresholdCenter);
#endif
            
            // Largest bar
            if (m_arrLGauge[0].Count == 9)
                m_arrLGauge[0].Add(new LGauge());//top
            if (m_arrLGauge[0].Count == 10)
                m_arrLGauge[0].Add(new LGauge());//center
            if (m_arrLGauge[0].Count == 11)
                m_arrLGauge[0].Add(new LGauge());//bottom

            m_arrLGauge[0][10].ref_GaugeAngle = 90;
            m_arrLGauge[0][10].ref_GaugeTransType = 0;
            m_arrLGauge[0][10].ref_GaugeTolerance = Math.Max(5, m_arrROIs[0].ref_ROIWidth / 4);
            m_arrLGauge[0][10].ref_GaugeLength = Math.Max(5, ((BlobsFeatures)m_arrBlobLeft[0]).fHeight / 2);
            m_arrLGauge[0][10].SetGaugeCenter(m_arrROIs[0].ref_ROIPositionX + (m_arrROIs[0].ref_ROIWidth * 3 / 4), ((BlobsFeatures)m_arrBlobLeft[0]).fCenterY);
            m_arrLGauge[0][10].Measure(objROI);

            if (m_arrLGauge[0][10].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][10].ref_GaugeTolerance = Math.Max(10, m_arrROIs[0].ref_ROIWidth / 2);
                m_arrLGauge[0][10].Measure(objROI);
            }

            if (m_arrLGauge[0][10].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Right Side point - Please adjust gauge setting.";
                return false;
            }

            m_arrLGauge[0][9].ref_GaugeAngle = 0;
            m_arrLGauge[0][9].ref_GaugeTransType = 0;
            m_arrLGauge[0][9].ref_GaugeTolerance = Math.Max(5, ((BlobsFeatures)m_arrBlobLeft[0]).fHeight / 4);
            m_arrLGauge[0][9].ref_GaugeLength = 10;
            m_arrLGauge[0][9].SetGaugeCenter(m_arrLGauge[0][10].ref_ObjectCenterX - 15, ((BlobsFeatures)m_arrBlobLeft[0]).fCenterY - (((BlobsFeatures)m_arrBlobLeft[0]).fHeight / 2));
            m_arrLGauge[0][9].Measure(objROI);

            if (m_arrLGauge[0][9].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][9].ref_GaugeTolerance = Math.Max(10, ((BlobsFeatures)m_arrBlobLeft[0]).fHeight / 2);
                m_arrLGauge[0][9].Measure(objROI);
            }

            if (m_arrLGauge[0][9].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Right Side point - Please adjust gauge setting.";
                return false;
            }

            m_arrLGauge[0][11].ref_GaugeAngle = 180;
            m_arrLGauge[0][11].ref_GaugeTransType = 0;
            m_arrLGauge[0][11].ref_GaugeTolerance = Math.Max(5, ((BlobsFeatures)m_arrBlobLeft[0]).fHeight / 4);
            m_arrLGauge[0][11].ref_GaugeLength = 10;
            m_arrLGauge[0][11].SetGaugeCenter(m_arrLGauge[0][10].ref_ObjectCenterX - 15, ((BlobsFeatures)m_arrBlobLeft[0]).fCenterY + (((BlobsFeatures)m_arrBlobLeft[0]).fHeight / 2));
            m_arrLGauge[0][11].Measure(objROI);

            if (m_arrLGauge[0][11].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][11].ref_GaugeTolerance = Math.Max(10, ((BlobsFeatures)m_arrBlobLeft[0]).fHeight / 2);
                m_arrLGauge[0][11].Measure(objROI);
            }

            if (m_arrLGauge[0][11].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Right Side point - Please adjust gauge setting.";
                return false;
            }

            //2020-12-14 ZJYEOH : measure again because side and center bar got offset
            m_arrLGauge[0][10].ref_GaugeTolerance = Math.Max(5, m_arrROIs[0].ref_ROIWidth / 4);
            m_arrLGauge[0][10].SetGaugeCenter(m_arrROIs[0].ref_ROIPositionX + (m_arrROIs[0].ref_ROIWidth * 3 / 4), (m_arrLGauge[0][9].ref_ObjectCenterY + m_arrLGauge[0][11].ref_ObjectCenterY) / 2);
            m_arrLGauge[0][10].Measure(objROI);

            if (m_arrLGauge[0][10].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][10].ref_GaugeTolerance = Math.Max(10, m_arrROIs[0].ref_ROIWidth / 2);
                m_arrLGauge[0][10].Measure(objROI);
            }

            if (m_arrLGauge[0][10].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Right Side point - Please adjust gauge setting.";
                return false;
            }

            // Middle bar
            if (m_arrLGauge[0].Count == 12)
                m_arrLGauge[0].Add(new LGauge());//top
            if (m_arrLGauge[0].Count == 13)
                m_arrLGauge[0].Add(new LGauge());//center
            if (m_arrLGauge[0].Count == 14)
                m_arrLGauge[0].Add(new LGauge());//bottom

            m_arrLGauge[0][13].ref_GaugeAngle = 90;
            m_arrLGauge[0][13].ref_GaugeTransType = 0;
            m_arrLGauge[0][13].ref_GaugeTolerance = Math.Max(5, m_arrROIs[0].ref_ROIWidth / 4);
            m_arrLGauge[0][13].ref_GaugeLength = Math.Max(5, ((BlobsFeatures)m_arrBlobLeft[1]).fHeight / 2);
            m_arrLGauge[0][13].SetGaugeCenter(m_arrROIs[0].ref_ROIPositionX + (m_arrROIs[0].ref_ROIWidth * 3 / 4), ((BlobsFeatures)m_arrBlobLeft[1]).fCenterY);
            m_arrLGauge[0][13].Measure(objROI);

            if (m_arrLGauge[0][13].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][13].ref_GaugeTolerance = Math.Max(10, m_arrROIs[0].ref_ROIWidth / 2);
                m_arrLGauge[0][13].Measure(objROI);
            }

            if (m_arrLGauge[0][13].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Right Side point - Please adjust gauge setting.";
                return false;
            }

            m_arrLGauge[0][12].ref_GaugeAngle = 0;
            m_arrLGauge[0][12].ref_GaugeTransType = 0;
            m_arrLGauge[0][12].ref_GaugeTolerance = Math.Max(5, ((BlobsFeatures)m_arrBlobLeft[1]).fHeight / 4);
            m_arrLGauge[0][12].ref_GaugeLength = 10;
            m_arrLGauge[0][12].SetGaugeCenter(m_arrLGauge[0][13].ref_ObjectCenterX - 15, ((BlobsFeatures)m_arrBlobLeft[1]).fCenterY - (((BlobsFeatures)m_arrBlobLeft[1]).fHeight / 2));
            m_arrLGauge[0][12].Measure(objROI);

            if (m_arrLGauge[0][12].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][12].ref_GaugeTolerance = Math.Max(10, ((BlobsFeatures)m_arrBlobLeft[1]).fHeight / 2);
                m_arrLGauge[0][12].Measure(objROI);
            }

            if (m_arrLGauge[0][12].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Right Side point - Please adjust gauge setting.";
                return false;
            }

            m_arrLGauge[0][14].ref_GaugeAngle = 180;
            m_arrLGauge[0][14].ref_GaugeTransType = 0;
            m_arrLGauge[0][14].ref_GaugeTolerance = Math.Max(5, ((BlobsFeatures)m_arrBlobLeft[1]).fHeight / 4);
            m_arrLGauge[0][14].ref_GaugeLength = 10;
            m_arrLGauge[0][14].SetGaugeCenter(m_arrLGauge[0][13].ref_ObjectCenterX - 15, ((BlobsFeatures)m_arrBlobLeft[1]).fCenterY + (((BlobsFeatures)m_arrBlobLeft[1]).fHeight / 2));
            m_arrLGauge[0][14].Measure(objROI);

            if (m_arrLGauge[0][14].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][14].ref_GaugeTolerance = Math.Max(10, ((BlobsFeatures)m_arrBlobLeft[1]).fHeight / 2);
                m_arrLGauge[0][14].Measure(objROI);
            }

            if (m_arrLGauge[0][14].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Right Side point - Please adjust gauge setting.";
                return false;
            }

            //2020-12-14 ZJYEOH : measure again because side and center bar got offset
            m_arrLGauge[0][13].ref_GaugeTolerance = Math.Max(5, m_arrROIs[0].ref_ROIWidth / 4);
            m_arrLGauge[0][13].SetGaugeCenter(m_arrROIs[0].ref_ROIPositionX + (m_arrROIs[0].ref_ROIWidth * 3 / 4), (m_arrLGauge[0][12].ref_ObjectCenterY + m_arrLGauge[0][14].ref_ObjectCenterY) / 2);
            m_arrLGauge[0][13].Measure(objROI);

            if (m_arrLGauge[0][13].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][13].ref_GaugeTolerance = Math.Max(10, m_arrROIs[0].ref_ROIWidth / 2);
                m_arrLGauge[0][13].Measure(objROI);
            }

            if (m_arrLGauge[0][13].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Right Side point - Please adjust gauge setting.";
                return false;
            }

            // Smallest bar
            if (m_arrLGauge[0].Count == 15)
                m_arrLGauge[0].Add(new LGauge());//top
            if (m_arrLGauge[0].Count == 16)
                m_arrLGauge[0].Add(new LGauge());//center
            if (m_arrLGauge[0].Count == 17)
                m_arrLGauge[0].Add(new LGauge());//bottom


            m_arrLGauge[0][16].ref_GaugeAngle = 90;
            m_arrLGauge[0][16].ref_GaugeTransType = 0;
            m_arrLGauge[0][16].ref_GaugeTolerance = Math.Max(5, m_arrROIs[0].ref_ROIWidth / 4);
            m_arrLGauge[0][16].ref_GaugeLength = Math.Max(5, ((BlobsFeatures)m_arrBlobLeft[2]).fHeight / 2);
            m_arrLGauge[0][16].SetGaugeCenter(m_arrROIs[0].ref_ROIPositionX + (m_arrROIs[0].ref_ROIWidth * 3 / 4), ((BlobsFeatures)m_arrBlobLeft[2]).fCenterY);
            m_arrLGauge[0][16].Measure(objROI);

            if (m_arrLGauge[0][16].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][16].ref_GaugeTolerance = Math.Max(10, m_arrROIs[0].ref_ROIWidth / 2);
                m_arrLGauge[0][16].Measure(objROI);
            }

            if (m_arrLGauge[0][16].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Right Side point - Please adjust gauge setting.";
                return false;
            }

            m_arrLGauge[0][15].ref_GaugeAngle = 0;
            m_arrLGauge[0][15].ref_GaugeTransType = 0;
            m_arrLGauge[0][15].ref_GaugeTolerance = Math.Max(5, ((BlobsFeatures)m_arrBlobLeft[2]).fHeight / 4);
            m_arrLGauge[0][15].ref_GaugeLength = 10;
            m_arrLGauge[0][15].SetGaugeCenter(m_arrLGauge[0][16].ref_ObjectCenterX - 15, ((BlobsFeatures)m_arrBlobLeft[2]).fCenterY - (((BlobsFeatures)m_arrBlobLeft[2]).fHeight / 2));
            m_arrLGauge[0][15].Measure(objROI);

            if (m_arrLGauge[0][15].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][15].ref_GaugeTolerance = Math.Max(10, ((BlobsFeatures)m_arrBlobLeft[2]).fHeight / 2);
                m_arrLGauge[0][15].Measure(objROI);
            }

            if (m_arrLGauge[0][15].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Right Side point - Please adjust gauge setting.";
                return false;
            }

            m_arrLGauge[0][17].ref_GaugeAngle = 180;
            m_arrLGauge[0][17].ref_GaugeTransType = 0;
            m_arrLGauge[0][17].ref_GaugeTolerance = Math.Max(5, ((BlobsFeatures)m_arrBlobLeft[2]).fHeight / 4);
            m_arrLGauge[0][17].ref_GaugeLength = 10;
            m_arrLGauge[0][17].SetGaugeCenter(m_arrLGauge[0][16].ref_ObjectCenterX - 15, ((BlobsFeatures)m_arrBlobLeft[2]).fCenterY + (((BlobsFeatures)m_arrBlobLeft[2]).fHeight / 2));
            m_arrLGauge[0][17].Measure(objROI);

            if (m_arrLGauge[0][17].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][17].ref_GaugeTolerance = Math.Max(10, ((BlobsFeatures)m_arrBlobLeft[2]).fHeight / 2);
                m_arrLGauge[0][17].Measure(objROI);
            }

            if (m_arrLGauge[0][17].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Right Side point - Please adjust gauge setting.";
                return false;
            }

            //2020-12-14 ZJYEOH : measure again because side and center bar got offset
            m_arrLGauge[0][16].ref_GaugeTolerance = Math.Max(5, m_arrROIs[0].ref_ROIWidth / 4);
            m_arrLGauge[0][16].SetGaugeCenter(m_arrROIs[0].ref_ROIPositionX + (m_arrROIs[0].ref_ROIWidth * 3 / 4), (m_arrLGauge[0][15].ref_ObjectCenterY + m_arrLGauge[0][17].ref_ObjectCenterY) / 2);
            m_arrLGauge[0][16].Measure(objROI);

            if (m_arrLGauge[0][16].ref_ObjectScore == 0)
            {
                m_arrLGauge[0][16].ref_GaugeTolerance = Math.Max(10, m_arrROIs[0].ref_ROIWidth / 2);
                m_arrLGauge[0][16].Measure(objROI);
            }

            if (m_arrLGauge[0][16].ref_ObjectScore == 0)
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Center ROI Right Side point - Please adjust gauge setting.";
                return false;
            }

            if (blnUpdateData)
            {
                //m_f2DCenterPixelCount1 = m_arrLGauge[0][17].ref_ObjectCenterY - m_arrLGauge[0][15].ref_ObjectCenterY;

                //m_f2DCenterPixelCount2 = m_arrLGauge[0][14].ref_ObjectCenterY - m_arrLGauge[0][12].ref_ObjectCenterY;

                //m_f2DCenterPixelCount3 = m_arrLGauge[0][11].ref_ObjectCenterY - m_arrLGauge[0][9].ref_ObjectCenterY;

                //m_f2DCenterPixelCount4 = m_arrLGauge[0][17].ref_ObjectCenterY - m_arrLGauge[0][12].ref_ObjectCenterY;

                //m_f2DCenterPixelCount5 = m_arrLGauge[0][17].ref_ObjectCenterY - m_arrLGauge[0][9].ref_ObjectCenterY;

                float fDistanceR3 = m_fSize3DR3 * (m_f3DRightPixelCount1 / m_fSize3DR1);

                if (m_fRotateAngleSide != 0)
                    m_fRightDistance = FixAngle(m_fRotateAngleSide, 2, 6, 0, true) - fDistanceR3;
                else
                    m_fRightDistance = m_arrLGauge[2][6].ref_ObjectCenterX - m_arrLGauge[2][0].ref_ObjectCenterX - fDistanceR3;

                float fDistanceA2 = 0;

                if (m_fRotateAngle != 0)
                    fDistanceA2 = FixAngle(m_fRotateAngle, 0, 10, 16, true);
                else
                    fDistanceA2 = m_arrLGauge[0][10].ref_ObjectCenterX - m_arrLGauge[0][16].ref_ObjectCenterX;

                m_fRightAngle = (float)(Math.Asin(m_fRightDistance / fDistanceA2) * 180 / Math.PI);
            }

            if (float.IsNaN(m_fRightAngle) || float.IsInfinity(m_fRightAngle))
            {
                objImgTemp.Dispose();
                objROI.Dispose();
                m_strErrorMessage = "*Fail to find Right Angle - Please adjust gauge or threshold setting.";
                return false;
            }

            objImgTemp.Dispose();
            objROI.Dispose();
            return true;
        }
        public void DrawCalibrateResult(Graphics g, float fScaleX, float fScaleY)
        {
            if (m_blnHorizontal)
            {
                if (m_blnBlobOK_Top)
                    m_objEBlobs_Top.DrawSelectedBlob(g, fScaleX, fScaleY, Color.Yellow);
                else
                    m_objEBlobs_Top.DrawSelectedBlob(g, fScaleX, fScaleY, Color.Red);


                if (m_blnBlobOK_Bottom)
                    m_objEBlobs_Bottom.DrawSelectedBlob(g, fScaleX, fScaleY, Color.Yellow);
                else
                    m_objEBlobs_Bottom.DrawSelectedBlob(g, fScaleX, fScaleY, Color.Red);

                if (m_arrBlobTop.Count < 3 || m_arrBlobBottom.Count < 3)
                    return;

                //Top 

                ////Blob 1
                //g.DrawLine(m_PenYellow, ((BlobsFeatures)m_arrBlobTop[0]).fCenterX * fScaleX - 5, (((BlobsFeatures)m_arrBlobTop[0]).fCenterY - (((BlobsFeatures)m_arrBlobTop[0]).fHeight / 2)) * fScaleY - 5, ((BlobsFeatures)m_arrBlobTop[0]).fCenterX * fScaleX + 5, (((BlobsFeatures)m_arrBlobTop[0]).fCenterY - (((BlobsFeatures)m_arrBlobTop[0]).fHeight / 2)) * fScaleY + 5);
                //g.DrawLine(m_PenYellow, ((BlobsFeatures)m_arrBlobTop[0]).fCenterX * fScaleX - 5, (((BlobsFeatures)m_arrBlobTop[0]).fCenterY - (((BlobsFeatures)m_arrBlobTop[0]).fHeight / 2)) * fScaleY + 5, ((BlobsFeatures)m_arrBlobTop[0]).fCenterX * fScaleX + 5, (((BlobsFeatures)m_arrBlobTop[0]).fCenterY - (((BlobsFeatures)m_arrBlobTop[0]).fHeight / 2)) * fScaleY - 5);

                ////Blob 2
                //g.DrawLine(m_PenYellow, ((BlobsFeatures)m_arrBlobTop[1]).fCenterX * fScaleX - 5, (((BlobsFeatures)m_arrBlobTop[1]).fCenterY - (((BlobsFeatures)m_arrBlobTop[1]).fHeight / 2)) * fScaleY - 5, ((BlobsFeatures)m_arrBlobTop[1]).fCenterX * fScaleX + 5, (((BlobsFeatures)m_arrBlobTop[1]).fCenterY - (((BlobsFeatures)m_arrBlobTop[1]).fHeight / 2)) * fScaleY + 5);
                //g.DrawLine(m_PenYellow, ((BlobsFeatures)m_arrBlobTop[1]).fCenterX * fScaleX - 5, (((BlobsFeatures)m_arrBlobTop[1]).fCenterY - (((BlobsFeatures)m_arrBlobTop[1]).fHeight / 2)) * fScaleY + 5, ((BlobsFeatures)m_arrBlobTop[1]).fCenterX * fScaleX + 5, (((BlobsFeatures)m_arrBlobTop[1]).fCenterY - (((BlobsFeatures)m_arrBlobTop[1]).fHeight / 2)) * fScaleY - 5);

                ////Blob 3
                //g.DrawLine(m_PenYellow, ((BlobsFeatures)m_arrBlobTop[2]).fCenterX * fScaleX - 5, (((BlobsFeatures)m_arrBlobTop[2]).fCenterY - (((BlobsFeatures)m_arrBlobTop[2]).fHeight / 2)) * fScaleY - 5, ((BlobsFeatures)m_arrBlobTop[2]).fCenterX * fScaleX + 5, (((BlobsFeatures)m_arrBlobTop[2]).fCenterY - (((BlobsFeatures)m_arrBlobTop[2]).fHeight / 2)) * fScaleY + 5);
                //g.DrawLine(m_PenYellow, ((BlobsFeatures)m_arrBlobTop[2]).fCenterX * fScaleX - 5, (((BlobsFeatures)m_arrBlobTop[2]).fCenterY - (((BlobsFeatures)m_arrBlobTop[2]).fHeight / 2)) * fScaleY + 5, ((BlobsFeatures)m_arrBlobTop[2]).fCenterX * fScaleX + 5, (((BlobsFeatures)m_arrBlobTop[2]).fCenterY - (((BlobsFeatures)m_arrBlobTop[2]).fHeight / 2)) * fScaleY - 5);

                //Line 1
                if (m_arrLGauge[1][0].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[1][0].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[1][0].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[1][0].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[1][0].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[1][0].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[1][0].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[1][0].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[1][0].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenCyan, m_arrLGauge[1][0].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[1][0].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[1][0].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[1][0].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenCyan, m_arrLGauge[1][0].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[1][0].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[1][0].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[1][0].ref_ObjectCenterY * fScaleY - 5);
                }

                //Line 2
                if (m_arrLGauge[1][1].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[1][1].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[1][1].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[1][1].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[1][1].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[1][1].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[1][1].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[1][1].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[1][1].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenCyan, m_arrLGauge[1][1].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[1][1].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[1][1].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[1][1].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenCyan, m_arrLGauge[1][1].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[1][1].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[1][1].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[1][1].ref_ObjectCenterY * fScaleY - 5);
                }

                //Line 3
                if (m_arrLGauge[1][2].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[1][2].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[1][2].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[1][2].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[1][2].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[1][2].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[1][2].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[1][2].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[1][2].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenCyan, m_arrLGauge[1][2].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[1][2].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[1][2].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[1][2].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenCyan, m_arrLGauge[1][2].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[1][2].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[1][2].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[1][2].ref_ObjectCenterY * fScaleY - 5);
                }

                //Line 4
                if (m_arrLGauge[1][3].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[1][3].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[1][3].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[1][3].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[1][3].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[1][3].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[1][3].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[1][3].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[1][3].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenCyan, m_arrLGauge[1][3].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[1][3].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[1][3].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[1][3].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenCyan, m_arrLGauge[1][3].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[1][3].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[1][3].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[1][3].ref_ObjectCenterY * fScaleY - 5);
                }

                //Line 5
                if (m_arrLGauge[1][4].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[1][4].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[1][4].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[1][4].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[1][4].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[1][4].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[1][4].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[1][4].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[1][4].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenCyan, m_arrLGauge[1][4].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[1][4].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[1][4].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[1][4].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenCyan, m_arrLGauge[1][4].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[1][4].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[1][4].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[1][4].ref_ObjectCenterY * fScaleY - 5);
                }

                //Line 6
                if (m_arrLGauge[1][5].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[1][5].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[1][5].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[1][5].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[1][5].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[1][5].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[1][5].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[1][5].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[1][5].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenCyan, m_arrLGauge[1][5].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[1][5].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[1][5].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[1][5].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenCyan, m_arrLGauge[1][5].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[1][5].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[1][5].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[1][5].ref_ObjectCenterY * fScaleY - 5);
                }

                //Line 7
                if (m_arrLGauge[1][6].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[1][6].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[1][6].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[1][6].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[1][6].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[1][6].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[1][6].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[1][6].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[1][6].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenCyan, m_arrLGauge[1][6].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[1][6].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[1][6].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[1][6].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenCyan, m_arrLGauge[1][6].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[1][6].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[1][6].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[1][6].ref_ObjectCenterY * fScaleY - 5);
                }

                //Bottom 

                ////Blob 1
                //g.DrawLine(m_PenYellow, ((BlobsFeatures)m_arrBlobBottom[0]).fCenterX * fScaleX - 5, (((BlobsFeatures)m_arrBlobBottom[0]).fCenterY + (((BlobsFeatures)m_arrBlobBottom[0]).fHeight / 2)) * fScaleY - 5, ((BlobsFeatures)m_arrBlobBottom[0]).fCenterX * fScaleX + 5, (((BlobsFeatures)m_arrBlobBottom[0]).fCenterY + (((BlobsFeatures)m_arrBlobBottom[0]).fHeight / 2)) * fScaleY + 5);
                //g.DrawLine(m_PenYellow, ((BlobsFeatures)m_arrBlobBottom[0]).fCenterX * fScaleX - 5, (((BlobsFeatures)m_arrBlobBottom[0]).fCenterY + (((BlobsFeatures)m_arrBlobBottom[0]).fHeight / 2)) * fScaleY + 5, ((BlobsFeatures)m_arrBlobBottom[0]).fCenterX * fScaleX + 5, (((BlobsFeatures)m_arrBlobBottom[0]).fCenterY + (((BlobsFeatures)m_arrBlobBottom[0]).fHeight / 2)) * fScaleY - 5);

                ////Blob 2
                //g.DrawLine(m_PenYellow, ((BlobsFeatures)m_arrBlobBottom[1]).fCenterX * fScaleX - 5, (((BlobsFeatures)m_arrBlobBottom[1]).fCenterY + (((BlobsFeatures)m_arrBlobBottom[1]).fHeight / 2)) * fScaleY - 5, ((BlobsFeatures)m_arrBlobBottom[1]).fCenterX * fScaleX + 5, (((BlobsFeatures)m_arrBlobBottom[1]).fCenterY + (((BlobsFeatures)m_arrBlobBottom[1]).fHeight / 2)) * fScaleY + 5);
                //g.DrawLine(m_PenYellow, ((BlobsFeatures)m_arrBlobBottom[1]).fCenterX * fScaleX - 5, (((BlobsFeatures)m_arrBlobBottom[1]).fCenterY + (((BlobsFeatures)m_arrBlobBottom[1]).fHeight / 2)) * fScaleY + 5, ((BlobsFeatures)m_arrBlobBottom[1]).fCenterX * fScaleX + 5, (((BlobsFeatures)m_arrBlobBottom[1]).fCenterY + (((BlobsFeatures)m_arrBlobBottom[1]).fHeight / 2)) * fScaleY - 5);

                ////Blob 3
                //g.DrawLine(m_PenYellow, ((BlobsFeatures)m_arrBlobBottom[2]).fCenterX * fScaleX - 5, (((BlobsFeatures)m_arrBlobBottom[2]).fCenterY + (((BlobsFeatures)m_arrBlobBottom[2]).fHeight / 2)) * fScaleY - 5, ((BlobsFeatures)m_arrBlobBottom[2]).fCenterX * fScaleX + 5, (((BlobsFeatures)m_arrBlobBottom[2]).fCenterY + (((BlobsFeatures)m_arrBlobBottom[2]).fHeight / 2)) * fScaleY + 5);
                //g.DrawLine(m_PenYellow, ((BlobsFeatures)m_arrBlobBottom[2]).fCenterX * fScaleX - 5, (((BlobsFeatures)m_arrBlobBottom[2]).fCenterY + (((BlobsFeatures)m_arrBlobBottom[2]).fHeight / 2)) * fScaleY + 5, ((BlobsFeatures)m_arrBlobBottom[2]).fCenterX * fScaleX + 5, (((BlobsFeatures)m_arrBlobBottom[2]).fCenterY + (((BlobsFeatures)m_arrBlobBottom[2]).fHeight / 2)) * fScaleY - 5);

                //Line 1
                if (m_arrLGauge[3][0].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[3][0].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[3][0].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[3][0].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[3][0].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[3][0].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[3][0].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[3][0].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[3][0].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenCyan, m_arrLGauge[3][0].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[3][0].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[3][0].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[3][0].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenCyan, m_arrLGauge[3][0].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[3][0].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[3][0].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[3][0].ref_ObjectCenterY * fScaleY - 5);
                }

                //Line 2
                if (m_arrLGauge[3][1].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[3][1].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[3][1].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[3][1].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[3][1].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[3][1].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[3][1].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[3][1].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[3][1].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenCyan, m_arrLGauge[3][1].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[3][1].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[3][1].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[3][1].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenCyan, m_arrLGauge[3][1].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[3][1].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[3][1].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[3][1].ref_ObjectCenterY * fScaleY - 5);
                }

                //Line 3
                if (m_arrLGauge[3][2].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[3][2].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[3][2].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[3][2].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[3][2].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[3][2].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[3][2].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[3][2].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[3][2].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenCyan, m_arrLGauge[3][2].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[3][2].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[3][2].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[3][2].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenCyan, m_arrLGauge[3][2].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[3][2].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[3][2].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[3][2].ref_ObjectCenterY * fScaleY - 5);
                }

                //Line 4
                if (m_arrLGauge[3][3].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[3][3].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[3][3].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[3][3].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[3][3].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[3][3].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[3][3].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[3][3].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[3][3].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenCyan, m_arrLGauge[3][3].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[3][3].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[3][3].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[3][3].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenCyan, m_arrLGauge[3][3].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[3][3].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[3][3].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[3][3].ref_ObjectCenterY * fScaleY - 5);
                }

                //Line 5
                if (m_arrLGauge[3][4].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[3][4].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[3][4].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[3][4].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[3][4].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[3][4].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[3][4].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[3][4].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[3][4].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenCyan, m_arrLGauge[3][4].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[3][4].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[3][4].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[3][4].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenCyan, m_arrLGauge[3][4].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[3][4].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[3][4].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[3][4].ref_ObjectCenterY * fScaleY - 5);
                }

                //Line 6
                if (m_arrLGauge[3][5].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[3][5].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[3][5].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[3][5].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[3][5].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[3][5].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[3][5].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[3][5].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[3][5].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenCyan, m_arrLGauge[3][5].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[3][5].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[3][5].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[3][5].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenCyan, m_arrLGauge[3][5].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[3][5].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[3][5].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[3][5].ref_ObjectCenterY * fScaleY - 5);
                }

                //Line 7
                if (m_arrLGauge[3][6].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[3][6].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[3][6].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[3][6].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[3][6].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[3][6].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[3][6].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[3][6].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[3][6].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenCyan, m_arrLGauge[3][6].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[3][6].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[3][6].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[3][6].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenCyan, m_arrLGauge[3][6].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[3][6].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[3][6].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[3][6].ref_ObjectCenterY * fScaleY - 5);
                }

                //Center
                if (m_arrLGauge[0][0].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[0][0].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][0].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][0].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][0].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[0][0].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][0].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][0].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][0].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenLime, m_arrLGauge[0][0].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][0].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][0].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][0].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenLime, m_arrLGauge[0][0].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][0].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][0].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][0].ref_ObjectCenterY * fScaleY - 5);
                }

                if (m_arrLGauge[0][1].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[0][1].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][1].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][1].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][1].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[0][1].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][1].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][1].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][1].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenLime, m_arrLGauge[0][1].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][1].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][1].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][1].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenLime, m_arrLGauge[0][1].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][1].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][1].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][1].ref_ObjectCenterY * fScaleY - 5);
                }

                if (m_arrLGauge[0][2].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[0][2].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][2].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][2].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][2].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[0][2].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][2].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][2].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][2].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenLime, m_arrLGauge[0][2].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][2].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][2].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][2].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenLime, m_arrLGauge[0][2].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][2].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][2].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][2].ref_ObjectCenterY * fScaleY - 5);
                }

                if (m_arrLGauge[0][3].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[0][3].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][3].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][3].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][3].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[0][3].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][3].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][3].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][3].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenLime, m_arrLGauge[0][3].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][3].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][3].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][3].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenLime, m_arrLGauge[0][3].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][3].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][3].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][3].ref_ObjectCenterY * fScaleY - 5);
                }

                if (m_arrLGauge[0][4].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[0][4].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][4].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][4].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][4].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[0][4].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][4].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][4].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][4].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenLime, m_arrLGauge[0][4].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][4].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][4].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][4].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenLime, m_arrLGauge[0][4].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][4].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][4].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][4].ref_ObjectCenterY * fScaleY - 5);
                }

                if (m_arrLGauge[0][5].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[0][5].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][5].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][5].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][5].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[0][5].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][5].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][5].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][5].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenLime, m_arrLGauge[0][5].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][5].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][5].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][5].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenLime, m_arrLGauge[0][5].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][5].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][5].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][5].ref_ObjectCenterY * fScaleY - 5);
                }

                if (m_arrLGauge[0][6].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[0][6].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][6].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][6].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][6].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[0][6].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][6].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][6].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][6].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenLime, m_arrLGauge[0][6].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][6].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][6].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][6].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenLime, m_arrLGauge[0][6].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][6].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][6].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][6].ref_ObjectCenterY * fScaleY - 5);
                }

                if (m_arrLGauge[0][7].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[0][7].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][7].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][7].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][7].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[0][7].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][7].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][7].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][7].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenLime, m_arrLGauge[0][7].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][7].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][7].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][7].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenLime, m_arrLGauge[0][7].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][7].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][7].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][7].ref_ObjectCenterY * fScaleY - 5);
                }
                

                if (m_arrLGauge[0][8].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[0][8].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][8].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][8].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][8].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[0][8].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][8].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][8].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][8].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenLime, m_arrLGauge[0][8].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][8].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][8].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][8].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenLime, m_arrLGauge[0][8].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][8].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][8].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][8].ref_ObjectCenterY * fScaleY - 5);
                }

                if (m_arrLGauge[0][9].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[0][9].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][9].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][9].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][9].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[0][9].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][9].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][9].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][9].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenLime, m_arrLGauge[0][9].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][9].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][9].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][9].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenLime, m_arrLGauge[0][9].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][9].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][9].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][9].ref_ObjectCenterY * fScaleY - 5);
                }

                if (m_arrLGauge[0][10].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[0][10].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][10].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][10].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][10].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[0][10].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][10].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][10].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][10].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenLime, m_arrLGauge[0][10].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][10].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][10].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][10].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenLime, m_arrLGauge[0][10].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][10].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][10].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][10].ref_ObjectCenterY * fScaleY - 5);
                }

                if (m_arrLGauge[0][11].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[0][11].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][11].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][11].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][11].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[0][11].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][11].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][11].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][11].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenLime, m_arrLGauge[0][11].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][11].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][11].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][11].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenLime, m_arrLGauge[0][11].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][11].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][11].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][11].ref_ObjectCenterY * fScaleY - 5);
                }

                if (m_arrLGauge[0][12].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[0][12].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][12].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][12].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][12].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[0][12].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][12].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][12].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][12].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenLime, m_arrLGauge[0][12].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][12].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][12].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][12].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenLime, m_arrLGauge[0][12].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][12].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][12].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][12].ref_ObjectCenterY * fScaleY - 5);
                }

                if (m_arrLGauge[0][13].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[0][13].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][13].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][13].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][13].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[0][13].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][13].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][13].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][13].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenLime, m_arrLGauge[0][13].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][13].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][13].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][13].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenLime, m_arrLGauge[0][13].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][13].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][13].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][13].ref_ObjectCenterY * fScaleY - 5);
                }

                if (m_arrLGauge[0][14].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[0][14].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][14].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][14].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][14].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[0][14].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][14].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][14].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][14].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenLime, m_arrLGauge[0][14].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][14].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][14].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][14].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenLime, m_arrLGauge[0][14].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][14].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][14].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][14].ref_ObjectCenterY * fScaleY - 5);
                }

                if (m_arrLGauge[0][15].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[0][15].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][15].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][15].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][15].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[0][15].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][15].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][15].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][15].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenLime, m_arrLGauge[0][15].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][15].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][15].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][15].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenLime, m_arrLGauge[0][15].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][15].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][15].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][15].ref_ObjectCenterY * fScaleY - 5);
                }

                if (m_arrLGauge[0][16].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[0][16].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][16].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][16].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][16].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[0][16].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][16].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][16].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][16].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenLime, m_arrLGauge[0][16].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][16].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][16].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][16].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenLime, m_arrLGauge[0][16].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][16].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][16].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][16].ref_ObjectCenterY * fScaleY - 5);
                }

                if (m_arrLGauge[0][17].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[0][17].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][17].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][17].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][17].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[0][17].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][17].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][17].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][17].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenLime, m_arrLGauge[0][17].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][17].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][17].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][17].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenLime, m_arrLGauge[0][17].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][17].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][17].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][17].ref_ObjectCenterY * fScaleY - 5);
                }
                
            }
            else
            {
                if (m_blnBlobOK_Left)
                    m_objEBlobs_Left.DrawSelectedBlob(g, fScaleX, fScaleY, Color.Yellow);
                else
                    m_objEBlobs_Left.DrawSelectedBlob(g, fScaleX, fScaleY, Color.Red);

                if (m_blnBlobOK_Right)
                    m_objEBlobs_Right.DrawSelectedBlob(g, fScaleX, fScaleY, Color.Yellow);
                else
                    m_objEBlobs_Right.DrawSelectedBlob(g, fScaleX, fScaleY, Color.Red);

                if (m_arrBlobLeft.Count < 3 || m_arrBlobRight.Count < 3)
                    return;

                //Left 

                ////Blob 1
                //g.DrawLine(m_PenYellow, (((BlobsFeatures)m_arrBlobLeft[0]).fCenterX - (((BlobsFeatures)m_arrBlobLeft[0]).fWidth / 2)) * fScaleX - 5, ((BlobsFeatures)m_arrBlobLeft[0]).fCenterY * fScaleY - 5, (((BlobsFeatures)m_arrBlobLeft[0]).fCenterX - (((BlobsFeatures)m_arrBlobLeft[0]).fWidth / 2)) * fScaleX + 5, ((BlobsFeatures)m_arrBlobLeft[0]).fCenterY * fScaleY + 5);
                //g.DrawLine(m_PenYellow, (((BlobsFeatures)m_arrBlobLeft[0]).fCenterX - (((BlobsFeatures)m_arrBlobLeft[0]).fWidth / 2)) * fScaleX - 5, ((BlobsFeatures)m_arrBlobLeft[0]).fCenterY * fScaleY + 5, (((BlobsFeatures)m_arrBlobLeft[0]).fCenterX - (((BlobsFeatures)m_arrBlobLeft[0]).fWidth / 2)) * fScaleX + 5, ((BlobsFeatures)m_arrBlobLeft[0]).fCenterY * fScaleY - 5);

                ////Blob 2
                //g.DrawLine(m_PenYellow, (((BlobsFeatures)m_arrBlobLeft[1]).fCenterX - (((BlobsFeatures)m_arrBlobLeft[0]).fWidth / 2)) * fScaleX - 5, ((BlobsFeatures)m_arrBlobLeft[1]).fCenterY * fScaleY - 5, (((BlobsFeatures)m_arrBlobLeft[1]).fCenterX - (((BlobsFeatures)m_arrBlobLeft[0]).fWidth / 2)) * fScaleX + 5, ((BlobsFeatures)m_arrBlobLeft[1]).fCenterY * fScaleY + 5);
                //g.DrawLine(m_PenYellow, (((BlobsFeatures)m_arrBlobLeft[1]).fCenterX - (((BlobsFeatures)m_arrBlobLeft[0]).fWidth / 2)) * fScaleX - 5, ((BlobsFeatures)m_arrBlobLeft[1]).fCenterY * fScaleY + 5, (((BlobsFeatures)m_arrBlobLeft[1]).fCenterX - (((BlobsFeatures)m_arrBlobLeft[0]).fWidth / 2)) * fScaleX + 5, ((BlobsFeatures)m_arrBlobLeft[1]).fCenterY * fScaleY - 5);

                ////Blob 3
                //g.DrawLine(m_PenYellow, (((BlobsFeatures)m_arrBlobLeft[2]).fCenterX - (((BlobsFeatures)m_arrBlobLeft[0]).fWidth / 2)) * fScaleX - 5, ((BlobsFeatures)m_arrBlobLeft[2]).fCenterY * fScaleY - 5, (((BlobsFeatures)m_arrBlobLeft[2]).fCenterX - (((BlobsFeatures)m_arrBlobLeft[0]).fWidth / 2)) * fScaleX + 5, ((BlobsFeatures)m_arrBlobLeft[2]).fCenterY * fScaleY + 5);
                //g.DrawLine(m_PenYellow, (((BlobsFeatures)m_arrBlobLeft[2]).fCenterX - (((BlobsFeatures)m_arrBlobLeft[0]).fWidth / 2)) * fScaleX - 5, ((BlobsFeatures)m_arrBlobLeft[2]).fCenterY * fScaleY + 5, (((BlobsFeatures)m_arrBlobLeft[2]).fCenterX - (((BlobsFeatures)m_arrBlobLeft[0]).fWidth / 2)) * fScaleX + 5, ((BlobsFeatures)m_arrBlobLeft[2]).fCenterY * fScaleY - 5);

                //Line 1
                if (m_arrLGauge[4][0].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[4][0].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[4][0].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[4][0].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[4][0].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[4][0].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[4][0].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[4][0].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[4][0].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenCyan, m_arrLGauge[4][0].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[4][0].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[4][0].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[4][0].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenCyan, m_arrLGauge[4][0].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[4][0].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[4][0].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[4][0].ref_ObjectCenterY * fScaleY - 5);
                }

                //Line 2
                if (m_arrLGauge[4][1].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[4][1].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[4][1].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[4][1].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[4][1].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[4][1].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[4][1].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[4][1].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[4][1].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenCyan, m_arrLGauge[4][1].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[4][1].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[4][1].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[4][1].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenCyan, m_arrLGauge[4][1].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[4][1].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[4][1].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[4][1].ref_ObjectCenterY * fScaleY - 5);
                }

                //Line 3
                if (m_arrLGauge[4][2].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[4][2].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[4][2].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[4][2].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[4][2].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[4][2].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[4][2].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[4][2].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[4][2].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenCyan, m_arrLGauge[4][2].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[4][2].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[4][2].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[4][2].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenCyan, m_arrLGauge[4][2].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[4][2].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[4][2].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[4][2].ref_ObjectCenterY * fScaleY - 5);
                }

                //Line 4
                if (m_arrLGauge[4][3].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[4][3].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[4][3].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[4][3].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[4][3].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[4][3].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[4][3].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[4][3].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[4][3].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenCyan, m_arrLGauge[4][3].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[4][3].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[4][3].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[4][3].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenCyan, m_arrLGauge[4][3].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[4][3].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[4][3].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[4][3].ref_ObjectCenterY * fScaleY - 5);
                }

                //Line 5
                if (m_arrLGauge[4][4].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[4][4].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[4][4].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[4][4].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[4][4].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[4][4].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[4][4].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[4][4].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[4][4].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenCyan, m_arrLGauge[4][4].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[4][4].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[4][4].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[4][4].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenCyan, m_arrLGauge[4][4].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[4][4].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[4][4].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[4][4].ref_ObjectCenterY * fScaleY - 5);
                }

                //Line 6
                if (m_arrLGauge[4][5].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[4][5].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[4][5].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[4][5].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[4][5].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[4][5].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[4][5].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[4][5].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[4][5].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenCyan, m_arrLGauge[4][5].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[4][5].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[4][5].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[4][5].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenCyan, m_arrLGauge[4][5].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[4][5].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[4][5].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[4][5].ref_ObjectCenterY * fScaleY - 5);
                }

                //Line 7
                if (m_arrLGauge[4][6].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[4][6].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[4][6].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[4][6].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[4][6].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[4][6].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[4][6].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[4][6].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[4][6].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenCyan, m_arrLGauge[4][6].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[4][6].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[4][6].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[4][6].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenCyan, m_arrLGauge[4][6].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[4][6].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[4][6].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[4][6].ref_ObjectCenterY * fScaleY - 5);
                }


                //Right 

                ////Blob 1
                //g.DrawLine(m_PenYellow, (((BlobsFeatures)m_arrBlobRight[0]).fCenterX + (((BlobsFeatures)m_arrBlobLeft[0]).fWidth / 2)) * fScaleX - 5, ((BlobsFeatures)m_arrBlobRight[0]).fCenterY * fScaleY - 5, (((BlobsFeatures)m_arrBlobRight[0]).fCenterX + (((BlobsFeatures)m_arrBlobLeft[0]).fWidth / 2)) * fScaleX + 5, ((BlobsFeatures)m_arrBlobRight[0]).fCenterY * fScaleY + 5);
                //g.DrawLine(m_PenYellow, (((BlobsFeatures)m_arrBlobRight[0]).fCenterX + (((BlobsFeatures)m_arrBlobLeft[0]).fWidth / 2)) * fScaleX - 5, ((BlobsFeatures)m_arrBlobRight[0]).fCenterY * fScaleY + 5, (((BlobsFeatures)m_arrBlobRight[0]).fCenterX + (((BlobsFeatures)m_arrBlobLeft[0]).fWidth / 2)) * fScaleX + 5, ((BlobsFeatures)m_arrBlobRight[0]).fCenterY * fScaleY - 5);

                ////Blob 2
                //g.DrawLine(m_PenYellow, (((BlobsFeatures)m_arrBlobRight[1]).fCenterX + (((BlobsFeatures)m_arrBlobLeft[0]).fWidth / 2)) * fScaleX - 5, ((BlobsFeatures)m_arrBlobRight[1]).fCenterY * fScaleY - 5, (((BlobsFeatures)m_arrBlobRight[1]).fCenterX + (((BlobsFeatures)m_arrBlobLeft[0]).fWidth / 2)) * fScaleX + 5, ((BlobsFeatures)m_arrBlobRight[1]).fCenterY * fScaleY + 5);
                //g.DrawLine(m_PenYellow, (((BlobsFeatures)m_arrBlobRight[1]).fCenterX + (((BlobsFeatures)m_arrBlobLeft[0]).fWidth / 2)) * fScaleX - 5, ((BlobsFeatures)m_arrBlobRight[1]).fCenterY * fScaleY + 5, (((BlobsFeatures)m_arrBlobRight[1]).fCenterX + (((BlobsFeatures)m_arrBlobLeft[0]).fWidth / 2)) * fScaleX + 5, ((BlobsFeatures)m_arrBlobRight[1]).fCenterY * fScaleY - 5);

                ////Blob 3
                //g.DrawLine(m_PenYellow, (((BlobsFeatures)m_arrBlobRight[2]).fCenterX + (((BlobsFeatures)m_arrBlobLeft[0]).fWidth / 2)) * fScaleX - 5, ((BlobsFeatures)m_arrBlobRight[2]).fCenterY * fScaleY - 5, (((BlobsFeatures)m_arrBlobRight[2]).fCenterX + (((BlobsFeatures)m_arrBlobLeft[0]).fWidth / 2)) * fScaleX + 5, ((BlobsFeatures)m_arrBlobRight[2]).fCenterY * fScaleY + 5);
                //g.DrawLine(m_PenYellow, (((BlobsFeatures)m_arrBlobRight[2]).fCenterX + (((BlobsFeatures)m_arrBlobLeft[0]).fWidth / 2)) * fScaleX - 5, ((BlobsFeatures)m_arrBlobRight[2]).fCenterY * fScaleY + 5, (((BlobsFeatures)m_arrBlobRight[2]).fCenterX + (((BlobsFeatures)m_arrBlobLeft[0]).fWidth / 2)) * fScaleX + 5, ((BlobsFeatures)m_arrBlobRight[2]).fCenterY * fScaleY - 5);

                //Line 1
                if (m_arrLGauge[2][0].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[2][0].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[2][0].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[2][0].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[2][0].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[2][0].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[2][0].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[2][0].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[2][0].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenCyan, m_arrLGauge[2][0].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[2][0].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[2][0].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[2][0].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenCyan, m_arrLGauge[2][0].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[2][0].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[2][0].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[2][0].ref_ObjectCenterY * fScaleY - 5);
                }

                //Line 2
                if (m_arrLGauge[2][1].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[2][1].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[2][1].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[2][1].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[2][1].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[2][1].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[2][1].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[2][1].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[2][1].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenCyan, m_arrLGauge[2][1].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[2][1].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[2][1].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[2][1].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenCyan, m_arrLGauge[2][1].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[2][1].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[2][1].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[2][1].ref_ObjectCenterY * fScaleY - 5);
                }

                //Line 3
                if (m_arrLGauge[2][2].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[2][2].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[2][2].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[2][2].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[2][2].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[2][2].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[2][2].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[2][2].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[2][2].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenCyan, m_arrLGauge[2][2].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[2][2].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[2][2].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[2][2].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenCyan, m_arrLGauge[2][2].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[2][2].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[2][2].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[2][2].ref_ObjectCenterY * fScaleY - 5);
                }

                //Line 4
                if (m_arrLGauge[2][3].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[2][3].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[2][3].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[2][3].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[2][3].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[2][3].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[2][3].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[2][3].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[2][3].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenCyan, m_arrLGauge[2][3].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[2][3].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[2][3].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[2][3].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenCyan, m_arrLGauge[2][3].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[2][3].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[2][3].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[2][3].ref_ObjectCenterY * fScaleY - 5);
                }

                //Line 5
                if (m_arrLGauge[2][4].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[2][4].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[2][4].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[2][4].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[2][4].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[2][4].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[2][4].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[2][4].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[2][4].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenCyan, m_arrLGauge[2][4].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[2][4].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[2][4].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[2][4].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenCyan, m_arrLGauge[2][4].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[2][4].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[2][4].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[2][4].ref_ObjectCenterY * fScaleY - 5);
                }

                //Line 6
                if (m_arrLGauge[2][5].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[2][5].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[2][5].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[2][5].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[2][5].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[2][5].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[2][5].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[2][5].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[2][5].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenCyan, m_arrLGauge[2][5].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[2][5].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[2][5].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[2][5].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenCyan, m_arrLGauge[2][5].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[2][5].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[2][5].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[2][5].ref_ObjectCenterY * fScaleY - 5);
                }

                //Line 7
                if (m_arrLGauge[2][6].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[2][6].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[2][6].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[2][6].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[2][6].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[2][6].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[2][6].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[2][6].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[2][6].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenCyan, m_arrLGauge[2][6].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[2][6].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[2][6].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[2][6].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenCyan, m_arrLGauge[2][6].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[2][6].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[2][6].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[2][6].ref_ObjectCenterY * fScaleY - 5);
                }


                //Center
                if (m_arrLGauge[0][0].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[0][0].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][0].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][0].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][0].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[0][0].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][0].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][0].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][0].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenLime, m_arrLGauge[0][0].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][0].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][0].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][0].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenLime, m_arrLGauge[0][0].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][0].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][0].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][0].ref_ObjectCenterY * fScaleY - 5);
                }

                if (m_arrLGauge[0][1].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[0][1].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][1].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][1].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][1].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[0][1].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][1].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][1].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][1].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenLime, m_arrLGauge[0][1].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][1].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][1].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][1].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenLime, m_arrLGauge[0][1].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][1].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][1].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][1].ref_ObjectCenterY * fScaleY - 5);
                }

                if (m_arrLGauge[0][2].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[0][2].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][2].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][2].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][2].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[0][2].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][2].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][2].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][2].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenLime, m_arrLGauge[0][2].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][2].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][2].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][2].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenLime, m_arrLGauge[0][2].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][2].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][2].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][2].ref_ObjectCenterY * fScaleY - 5);
                }

                if (m_arrLGauge[0][3].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[0][3].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][3].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][3].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][3].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[0][3].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][3].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][3].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][3].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenLime, m_arrLGauge[0][3].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][3].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][3].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][3].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenLime, m_arrLGauge[0][3].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][3].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][3].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][3].ref_ObjectCenterY * fScaleY - 5);
                }

                if (m_arrLGauge[0][4].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[0][4].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][4].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][4].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][4].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[0][4].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][4].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][4].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][4].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenLime, m_arrLGauge[0][4].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][4].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][4].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][4].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenLime, m_arrLGauge[0][4].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][4].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][4].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][4].ref_ObjectCenterY * fScaleY - 5);
                }

                if (m_arrLGauge[0][5].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[0][5].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][5].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][5].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][5].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[0][5].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][5].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][5].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][5].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenLime, m_arrLGauge[0][5].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][5].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][5].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][5].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenLime, m_arrLGauge[0][5].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][5].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][5].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][5].ref_ObjectCenterY * fScaleY - 5);
                }

                if (m_arrLGauge[0][6].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[0][6].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][6].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][6].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][6].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[0][6].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][6].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][6].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][6].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenLime, m_arrLGauge[0][6].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][6].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][6].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][6].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenLime, m_arrLGauge[0][6].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][6].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][6].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][6].ref_ObjectCenterY * fScaleY - 5);
                }

                if (m_arrLGauge[0][7].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[0][7].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][7].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][7].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][7].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[0][7].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][7].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][7].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][7].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenLime, m_arrLGauge[0][7].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][7].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][7].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][7].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenLime, m_arrLGauge[0][7].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][7].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][7].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][7].ref_ObjectCenterY * fScaleY - 5);
                }

                if (m_arrLGauge[0][8].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[0][8].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][8].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][8].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][8].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[0][8].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][8].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][8].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][8].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenLime, m_arrLGauge[0][8].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][8].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][8].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][8].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenLime, m_arrLGauge[0][8].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][8].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][8].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][8].ref_ObjectCenterY * fScaleY - 5);
                }

                if (m_arrLGauge[0][9].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[0][9].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][9].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][9].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][9].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[0][9].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][9].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][9].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][9].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenLime, m_arrLGauge[0][9].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][9].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][9].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][9].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenLime, m_arrLGauge[0][9].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][9].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][9].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][9].ref_ObjectCenterY * fScaleY - 5);
                }

                if (m_arrLGauge[0][10].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[0][10].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][10].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][10].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][10].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[0][10].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][10].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][10].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][10].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenLime, m_arrLGauge[0][10].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][10].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][10].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][10].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenLime, m_arrLGauge[0][10].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][10].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][10].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][10].ref_ObjectCenterY * fScaleY - 5);
                }

                if (m_arrLGauge[0][11].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[0][11].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][11].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][11].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][11].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[0][11].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][11].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][11].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][11].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenLime, m_arrLGauge[0][11].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][11].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][11].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][11].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenLime, m_arrLGauge[0][11].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][11].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][11].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][11].ref_ObjectCenterY * fScaleY - 5);
                }

                if (m_arrLGauge[0][12].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[0][12].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][12].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][12].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][12].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[0][12].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][12].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][12].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][12].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenLime, m_arrLGauge[0][12].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][12].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][12].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][12].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenLime, m_arrLGauge[0][12].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][12].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][12].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][12].ref_ObjectCenterY * fScaleY - 5);
                }

                if (m_arrLGauge[0][13].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[0][13].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][13].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][13].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][13].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[0][13].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][13].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][13].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][13].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenLime, m_arrLGauge[0][13].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][13].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][13].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][13].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenLime, m_arrLGauge[0][13].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][13].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][13].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][13].ref_ObjectCenterY * fScaleY - 5);
                }

                if (m_arrLGauge[0][14].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[0][14].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][14].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][14].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][14].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[0][14].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][14].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][14].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][14].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenLime, m_arrLGauge[0][14].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][14].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][14].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][14].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenLime, m_arrLGauge[0][14].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][14].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][14].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][14].ref_ObjectCenterY * fScaleY - 5);
                }

                if (m_arrLGauge[0][15].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[0][15].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][15].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][15].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][15].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[0][15].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][15].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][15].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][15].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenLime, m_arrLGauge[0][15].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][15].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][15].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][15].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenLime, m_arrLGauge[0][15].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][15].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][15].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][15].ref_ObjectCenterY * fScaleY - 5);
                }

                if (m_arrLGauge[0][16].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[0][16].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][16].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][16].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][16].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[0][16].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][16].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][16].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][16].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenLime, m_arrLGauge[0][16].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][16].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][16].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][16].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenLime, m_arrLGauge[0][16].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][16].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][16].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][16].ref_ObjectCenterY * fScaleY - 5);
                }

                if (m_arrLGauge[0][17].ref_ObjectScore == 0)
                {
                    g.DrawLine(m_PenRed, m_arrLGauge[0][17].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][17].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][17].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][17].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenRed, m_arrLGauge[0][17].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][17].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][17].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][17].ref_ObjectCenterY * fScaleY - 5);
                }
                else
                {
                    g.DrawLine(m_PenLime, m_arrLGauge[0][17].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][17].ref_ObjectCenterY * fScaleY - 5, m_arrLGauge[0][17].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][17].ref_ObjectCenterY * fScaleY + 5);
                    g.DrawLine(m_PenLime, m_arrLGauge[0][17].ref_ObjectCenterX * fScaleX - 5, m_arrLGauge[0][17].ref_ObjectCenterY * fScaleY + 5, m_arrLGauge[0][17].ref_ObjectCenterX * fScaleX + 5, m_arrLGauge[0][17].ref_ObjectCenterY * fScaleY - 5);
                }

            }
        }

        public void SaveGauge(string strPath, bool blnNewFile, string strSectionName, bool blnNewSection, bool blnSaveMeasurementAsTemplate)
        {
            XmlParser objFile = new XmlParser(strPath, blnNewFile);

            objFile.WriteSectionElement(strSectionName, blnNewSection);
            
            // gauge measurement setting
            objFile.WriteElement1Value("TransChoice", m_intGaugeTransitionChoice, "Gauge Setting Transition Choice", true);
            objFile.WriteElement1Value("Threshold", m_intGaugeThreshold, "Gauge Setting Threshold", true);
            objFile.WriteElement1Value("Thickness", m_intGaugeThickness, "Gauge Setting Thickness", true);
            objFile.WriteElement1Value("MinAmp", m_intGaugeMinAmplitude, "Gauge Setting Minimum Amplitude", true);
            objFile.WriteElement1Value("MinArea", m_intGaugeMinArea, "Gauge Setting Minimum Area", true);
            objFile.WriteElement1Value("Filter", m_intGaugeFilter, "Gauge Setting Filter/Smoothing", true);
            objFile.WriteElement1Value("FilterPass", m_intGaugeFilterPass, "Gauge Setting FilterPass", true);
            objFile.WriteElement1Value("FilterThreshold", m_fGaugeFilterThreshold, "Gauge Setting FilterThreshold", true);
            //objFile.WriteElement1Value("MinScore", m_intMinScore, "Gauge Min Score", true);

            objFile.WriteEndElement();
            UpdateLGaugeSetting();
        }

        public void LoadGauge(string strPath, string strSectionName)
        {
            XmlParser objFile = new XmlParser(strPath);
            objFile.GetFirstSection(strSectionName);

            // gauge measurement setting

            m_intGaugeTransitionChoice = objFile.GetValueAsInt("TransChoice", 2);
            m_intGaugeThreshold = objFile.GetValueAsInt("Threshold", 20);
            m_intGaugeThickness = objFile.GetValueAsInt("Thickness", 20);
            m_intGaugeMinAmplitude = objFile.GetValueAsInt("MinAmp", 10);
            m_intGaugeMinArea = objFile.GetValueAsInt("MinArea", 0);
            m_intGaugeFilter = objFile.GetValueAsInt("Filter", 1);
            m_intGaugeFilterPass = objFile.GetValueAsInt("FilterPass", 3);
            m_fGaugeFilterThreshold = objFile.GetValueAsFloat("FilterThreshold", 3);
            //m_intMinScore = objFile.GetValueAsInt("MinScore", 0);

            UpdateLGaugeSetting();
        }

        public void calaculateAngleAverage(int direction, bool blnUpdateData)
        {
            if(direction == 0)
            {
                //float length1 = m_arrLGauge[1][0].ref_GaugeLength / 2;
                //float length2 = m_arrLGauge[3][0].ref_GaugeLength / 2;
                //float length3 = m_arrLGauge[1][6].ref_GaugeLength / 2;
                //float length4 = m_arrLGauge[3][6].ref_GaugeLength / 2;

                PointF xSide = new PointF(m_arrLGauge[1][1].ref_ObjectCenterX, m_arrLGauge[1][1].ref_ObjectCenterY);
                PointF ySide = new PointF(m_arrLGauge[3][1].ref_ObjectCenterX, m_arrLGauge[3][1].ref_ObjectCenterY);
                PointF x1Side = new PointF(m_arrLGauge[1][5].ref_ObjectCenterX, m_arrLGauge[1][5].ref_ObjectCenterY);
                PointF y1Side = new PointF(m_arrLGauge[3][5].ref_ObjectCenterX, m_arrLGauge[3][5].ref_ObjectCenterY);

                PointF x = new PointF (m_arrLGauge[0][6].ref_ObjectCenterX, m_arrLGauge[0][6].ref_ObjectCenterY);
                PointF y = new PointF (m_arrLGauge[0][15].ref_ObjectCenterX, m_arrLGauge[0][15].ref_ObjectCenterY);
                PointF x1 = new PointF(m_arrLGauge[0][8].ref_ObjectCenterX, m_arrLGauge[0][8].ref_ObjectCenterY);
                PointF y1 = new PointF(m_arrLGauge[0][17].ref_ObjectCenterX, m_arrLGauge[0][17].ref_ObjectCenterY);

                Line newLineSide = new Line();
                Line newLine2Side = new Line();
                newLineSide.CalculateStraightLine(xSide, ySide);
                newLine2Side.CalculateStraightLine(x1Side, y1Side);

                float AngleSide = (float)newLineSide.ref_dAngle;
                float Angle2Side = (float)newLine2Side.ref_dAngle;

                m_fRotateAngleSide = ((AngleSide + Angle2Side) / 2);

                Line newLine = new Line();
                Line newLine2 = new Line();
                newLine.CalculateStraightLine(x, y);
                newLine2.CalculateStraightLine(x1, y1);

                float Angle = (float)newLine.ref_dAngle;
                float Angle2 = (float)newLine2.ref_dAngle;
           
                //double distance = Math.Sqrt(Math.Pow(m_arrLGauge[1][6].ref_ObjectCenterY - m_arrLGauge[1][0].ref_ObjectCenterY, 2) + Math.Pow(m_arrLGauge[1][0].ref_ObjectCenterX - m_arrLGauge[1][0].ref_ObjectCenterX, 2));
                //double distance2 = Math.Sqrt(Math.Pow(m_arrLGauge[3][6].ref_ObjectCenterY - m_arrLGauge[3][0].ref_ObjectCenterY, 2) + Math.Pow(m_arrLGauge[3][0].ref_ObjectCenterX - m_arrLGauge[3][0].ref_ObjectCenterX, 2));

                m_fRotateAngle = ((Angle + Angle2) / 2);

                if (blnUpdateData && (Math.Abs(m_fRotateAngle) < 1.5))
                {
                    if (m_fRotateAngleSide != 0)
                        m_f3DTopPixelCount1 = FixAngle(m_fRotateAngleSide, 1, 2, 1, true);
                    else
                        m_f3DTopPixelCount1 = m_arrLGauge[1][2].ref_ObjectCenterX - m_arrLGauge[1][1].ref_ObjectCenterX;

                    if (m_fRotateAngleSide != 0)
                        m_f3DTopPixelCount2 = FixAngle(m_fRotateAngleSide, 1, 5, 4, true);
                    else
                        m_f3DTopPixelCount2 = m_arrLGauge[1][5].ref_ObjectCenterX - m_arrLGauge[1][4].ref_ObjectCenterX;

                    m_fCalibration3DTop = m_f3DTopPixelCount2 / m_fSize3DT2;
                    ////////////////
                    if (m_fRotateAngleSide != 0)
                        m_f3DBottomPixelCount1 = FixAngle(m_fRotateAngleSide, 3, 2, 1, true);
                    else
                        m_f3DBottomPixelCount1 = m_arrLGauge[3][2].ref_ObjectCenterX - m_arrLGauge[3][1].ref_ObjectCenterX;

                    if (m_fRotateAngleSide != 0)
                        m_f3DBottomPixelCount2 = FixAngle(m_fRotateAngleSide, 3, 5, 4, true);
                    else
                        m_f3DBottomPixelCount2 = m_arrLGauge[3][5].ref_ObjectCenterX - m_arrLGauge[3][4].ref_ObjectCenterX;

                    m_fCalibration3DBottom = m_f3DBottomPixelCount2 / m_fSize3DB2;
                    ////////////

                    if (m_fRotateAngle != 0)
                        m_f2DCenterPixelCountX1 = FixAngle(m_fRotateAngle, 0, 2, 0, true);
                    else
                        m_f2DCenterPixelCountX1 = m_arrLGauge[0][2].ref_ObjectCenterX - m_arrLGauge[0][0].ref_ObjectCenterX;

                    if (m_fRotateAngle != 0)
                        m_f2DCenterPixelCountX2 = FixAngle(m_fRotateAngle, 0, 5, 3, true);
                    else
                        m_f2DCenterPixelCountX2 = m_arrLGauge[0][5].ref_ObjectCenterX - m_arrLGauge[0][3].ref_ObjectCenterX;

                    if (m_fRotateAngle != 0)
                        m_f2DCenterPixelCountX3 = FixAngle(m_fRotateAngle, 0, 8, 6, true);
                    else
                        m_f2DCenterPixelCountX3 = m_arrLGauge[0][8].ref_ObjectCenterX - m_arrLGauge[0][6].ref_ObjectCenterX;

                    m_fCalibration2DX = m_f2DCenterPixelCountX3 / m_fSize2DC3;

                    if (m_fRotateAngle != 0)
                        m_f2DCenterPixelCountX4 = FixAngle(m_fRotateAngle, 0, 5, 0, true);
                    else
                        m_f2DCenterPixelCountX4 = m_arrLGauge[0][5].ref_ObjectCenterX - m_arrLGauge[0][0].ref_ObjectCenterX;

                    if (m_fRotateAngle != 0)
                        m_f2DCenterPixelCountX5 = FixAngle(m_fRotateAngle, 0, 8, 0, true);
                    else
                        m_f2DCenterPixelCountX5 = m_arrLGauge[0][8].ref_ObjectCenterX - m_arrLGauge[0][0].ref_ObjectCenterX;

                    float fDistanceT3 = m_fSize3DT3 * (m_f3DTopPixelCount1 / m_fSize3DT1);

                    if (m_fRotateAngleSide != 0)
                        m_fTopDistance = FixAngle(m_fRotateAngleSide, 1, 0, 6, false) - fDistanceT3;
                    else
                        m_fTopDistance = m_arrLGauge[1][0].ref_ObjectCenterY - m_arrLGauge[1][6].ref_ObjectCenterY - fDistanceT3;

                    float DistanceA1 = 0;

                    if (m_fRotateAngle != 0)
                        DistanceA1 = FixAngle(m_fRotateAngle, 0, 1, 7, false);
                    else
                        DistanceA1 = m_arrLGauge[0][1].ref_ObjectCenterY - m_arrLGauge[0][7].ref_ObjectCenterY;

                    m_fTopAngle = (float)(Math.Asin(m_fTopDistance / DistanceA1) * 180 / Math.PI);

                    ///////////////////////////

                    float fDistanceB3 = m_fSize3DB3 * (m_f3DBottomPixelCount1 / m_fSize3DB1);

                    if (m_fRotateAngleSide != 0)
                        m_fBottomDistance = FixAngle(m_fRotateAngleSide, 3, 6, 0, false) - fDistanceB3;
                    else
                        m_fBottomDistance = m_arrLGauge[3][6].ref_ObjectCenterY - m_arrLGauge[3][0].ref_ObjectCenterY - fDistanceB3;

                    float fDistanceA2 = 0;

                    if (m_fRotateAngle != 0)
                        fDistanceA2 = FixAngle(m_fRotateAngle, 0, 16, 10, false);
                    else
                        fDistanceA2 = m_arrLGauge[0][16].ref_ObjectCenterY - m_arrLGauge[0][10].ref_ObjectCenterY;

                    m_fBottomAngle = (float)(Math.Asin(m_fBottomDistance / fDistanceA2) * 180 / Math.PI);
                }
            }
            else if (direction == 1)
            {
                //float length1 = m_arrLGauge[4][0].ref_GaugeLength / 2;
                //float length2 = m_arrLGauge[2][0].ref_GaugeLength / 2;
                //float length3 = m_arrLGauge[4][6].ref_GaugeLength / 2;
                //float length4 = m_arrLGauge[2][6].ref_GaugeLength / 2;

                PointF xSide = new PointF(m_arrLGauge[4][1].ref_ObjectCenterX, m_arrLGauge[4][1].ref_ObjectCenterY);
                PointF ySide = new PointF(m_arrLGauge[2][1].ref_ObjectCenterX, m_arrLGauge[2][1].ref_ObjectCenterY);
                PointF x1Side = new PointF(m_arrLGauge[4][5].ref_ObjectCenterX, m_arrLGauge[4][5].ref_ObjectCenterY);
                PointF y1Side = new PointF(m_arrLGauge[2][5].ref_ObjectCenterX, m_arrLGauge[2][5].ref_ObjectCenterY);

                PointF x = new PointF(m_arrLGauge[0][0].ref_ObjectCenterX, m_arrLGauge[0][0].ref_ObjectCenterY);
                PointF y = new PointF(m_arrLGauge[0][9].ref_ObjectCenterX, m_arrLGauge[0][9].ref_ObjectCenterY);
                PointF x1 = new PointF(m_arrLGauge[0][2].ref_ObjectCenterX, m_arrLGauge[0][2].ref_ObjectCenterY);
                PointF y1 = new PointF(m_arrLGauge[0][11].ref_ObjectCenterX, m_arrLGauge[0][11].ref_ObjectCenterY);

                Line newLineSide = new Line();
                Line newLine2Side = new Line();
                newLineSide.CalculateStraightLine(xSide, ySide);
                newLine2Side.CalculateStraightLine(x1Side, y1Side);

                float AngleSide = (float)newLineSide.ref_dAngle;
                float Angle2Side = (float)newLine2Side.ref_dAngle;

                if (AngleSide < 0)
                {
                    AngleSide = AngleSide + 90;
                }
                else
                {
                    AngleSide = AngleSide - 90;
                }

                if (Angle2Side < 0)
                {
                    Angle2Side = Angle2Side + 90;
                }
                else
                {
                    Angle2Side = Angle2Side - 90;
                }

                m_fRotateAngleSide = ((AngleSide + Angle2Side) / 2);

                Line newLine = new Line();
                Line newLine2 = new Line();
                newLine.CalculateStraightLine(x, y);
                newLine2.CalculateStraightLine(x1, y1);

                float Angle = (float)newLine.ref_dAngle;
                float Angle2 = (float)newLine2.ref_dAngle;

                if (Angle < 0)
                {
                    Angle = Angle + 90;
                }
                else
                {
                    Angle = Angle - 90;
                }

                if (Angle2 < 0)
                {
                    Angle2 = Angle2 + 90;
                }
                else
                {
                    Angle2 = Angle2 - 90;
                }

                //double distance = Math.Sqrt(Math.Pow(m_arrLGauge[4][6].ref_ObjectCenterX - m_arrLGauge[4][0].ref_ObjectCenterX, 2) + Math.Pow(m_arrLGauge[4][0].ref_ObjectCenterY - m_arrLGauge[4][0].ref_ObjectCenterY, 2));
                //double distance2 = Math.Sqrt(Math.Pow(m_arrLGauge[2][6].ref_ObjectCenterX - m_arrLGauge[2][0].ref_ObjectCenterX, 2) + Math.Pow(m_arrLGauge[2][0].ref_ObjectCenterY - m_arrLGauge[2][0].ref_ObjectCenterY, 2));

                m_fRotateAngle = ((Angle + Angle2) / 2);

                if (blnUpdateData && (Math.Abs(m_fRotateAngle) < 1.5))
                {
                    if (m_fRotateAngleSide != 0)
                        m_f3DLeftPixelCount1 = FixAngle(m_fRotateAngleSide, 4, 1, 2, false);
                    else
                        m_f3DLeftPixelCount1 = m_arrLGauge[4][1].ref_ObjectCenterY - m_arrLGauge[4][2].ref_ObjectCenterY;

                    if (m_fRotateAngleSide != 0)
                        m_f3DLeftPixelCount2 = FixAngle(m_fRotateAngleSide, 4, 4, 5, false);
                    else
                        m_f3DLeftPixelCount2 = m_arrLGauge[4][4].ref_ObjectCenterY - m_arrLGauge[4][5].ref_ObjectCenterY;

                    m_fCalibration3DLeft = m_f3DLeftPixelCount2 / m_fSize3DL2;
                    //////////////////
                    if (m_fRotateAngleSide != 0)
                        m_f3DRightPixelCount1 = FixAngle(m_fRotateAngleSide, 2, 1, 2, false);
                    else
                        m_f3DRightPixelCount1 = m_arrLGauge[2][1].ref_ObjectCenterY - m_arrLGauge[2][2].ref_ObjectCenterY;

                    if (m_fRotateAngleSide != 0)
                        m_f3DRightPixelCount2 = FixAngle(m_fRotateAngleSide, 2, 4, 5, false);
                    else
                        m_f3DRightPixelCount2 = m_arrLGauge[2][4].ref_ObjectCenterY - m_arrLGauge[2][5].ref_ObjectCenterY;

                    m_fCalibration3DRight = m_f3DRightPixelCount2 / m_fSize3DR2;
                    ///////////////////////////

                    if (m_fRotateAngle != 0)
                        m_f2DCenterPixelCountY1 = FixAngle(m_fRotateAngle, 0, 8, 6, false);
                    else
                        m_f2DCenterPixelCountY1 = m_arrLGauge[0][8].ref_ObjectCenterY - m_arrLGauge[0][6].ref_ObjectCenterY;

                    if (m_fRotateAngle != 0)
                        m_f2DCenterPixelCountY2 = FixAngle(m_fRotateAngle, 0, 5, 3, false);
                    else
                        m_f2DCenterPixelCountY2 = m_arrLGauge[0][5].ref_ObjectCenterY - m_arrLGauge[0][3].ref_ObjectCenterY;

                    if (m_fRotateAngle != 0)
                        m_f2DCenterPixelCountY3 = FixAngle(m_fRotateAngle, 0, 2, 0, false);
                    else
                        m_f2DCenterPixelCountY3 = m_arrLGauge[0][2].ref_ObjectCenterY - m_arrLGauge[0][0].ref_ObjectCenterY;

                    m_fCalibration2DY = m_f2DCenterPixelCountY3 / m_fSize2DC3;

                    if (m_fRotateAngle != 0)
                        m_f2DCenterPixelCountY4 = FixAngle(m_fRotateAngle, 0, 8, 3, false);
                    else
                        m_f2DCenterPixelCountY4 = m_arrLGauge[0][8].ref_ObjectCenterY - m_arrLGauge[0][3].ref_ObjectCenterY;

                    if (m_fRotateAngle != 0)
                        m_f2DCenterPixelCountY5 = FixAngle(m_fRotateAngle, 0, 8, 0, false);
                    else
                        m_f2DCenterPixelCountY5 = m_arrLGauge[0][8].ref_ObjectCenterY - m_arrLGauge[0][0].ref_ObjectCenterY;

                    float fDistanceL3 = m_fSize3DL3 * (m_f3DLeftPixelCount1 / m_fSize3DL1);

                    if (m_fRotateAngleSide != 0)
                        m_fLeftDistance = FixAngle(m_fRotateAngleSide, 4, 0, 6, true) - fDistanceL3;
                    else
                        m_fLeftDistance = m_arrLGauge[4][0].ref_ObjectCenterX - m_arrLGauge[4][6].ref_ObjectCenterX - fDistanceL3;

                    float fDistanceA1 = 0;

                    if (m_fRotateAngle != 0)
                        fDistanceA1 = FixAngle(m_fRotateAngle, 0, 7, 1, true);
                    else
                        fDistanceA1 = m_arrLGauge[0][7].ref_ObjectCenterX - m_arrLGauge[0][1].ref_ObjectCenterX;

                    m_fLeftAngle = (float)(Math.Asin(m_fLeftDistance / fDistanceA1) * 180 / Math.PI);

                    ////////////////////

                    float fDistanceR3 = m_fSize3DR3 * (m_f3DRightPixelCount1 / m_fSize3DR1);

                    if (m_fRotateAngleSide != 0)
                        m_fRightDistance = FixAngle(m_fRotateAngleSide, 2, 6, 0, true) - fDistanceR3;
                    else
                        m_fRightDistance = m_arrLGauge[2][6].ref_ObjectCenterX - m_arrLGauge[2][0].ref_ObjectCenterX - fDistanceR3;

                    float fDistanceA2 = 0;

                    if (m_fRotateAngle != 0)
                        fDistanceA2 = FixAngle(m_fRotateAngle, 0, 10, 16, true);
                    else
                        fDistanceA2 = m_arrLGauge[0][10].ref_ObjectCenterX - m_arrLGauge[0][16].ref_ObjectCenterX;

                    m_fRightAngle = (float)(Math.Asin(m_fRightDistance / fDistanceA2) * 180 / Math.PI);
                }
            }
        }

        public float FixAngle(float angle, int position, int intIndex1, int intIndex2, bool blnWantX)
        {
            //angle = (float)((-angle / 180) * Math.PI); //convert to theta
            angle = -angle;
            /*************************************
             Formula Calculate
             X = xcos(θ) - ysin(θ) 
             Y = ycos(θ) + xsin(θ)

             θ = angle nid to rotate
            ***************************************/

            PointF CenterPoint = new PointF((m_arrLGauge[position][intIndex1].ref_ObjectCenterX + m_arrLGauge[position][intIndex2].ref_ObjectCenterX) / 2,
                                             (m_arrLGauge[position][intIndex1].ref_ObjectCenterY + m_arrLGauge[position][intIndex2].ref_ObjectCenterY) / 2);
            float newX1 = 0, newY1 = 0;
            float newX2 = 0, newY2 = 0;
            Math2.RotateWithAngleAccordingToReferencePoint(CenterPoint.X, CenterPoint.Y, m_arrLGauge[position][intIndex1].ref_ObjectCenterX, m_arrLGauge[position][intIndex1].ref_ObjectCenterY, angle, ref newX1, ref newY1);
            Math2.RotateWithAngleAccordingToReferencePoint(CenterPoint.X, CenterPoint.Y, m_arrLGauge[position][intIndex2].ref_ObjectCenterX, m_arrLGauge[position][intIndex2].ref_ObjectCenterY, angle, ref newX2, ref newY2);

            //newX1 = ((float)(m_arrLGauge[position][intIndex1].ref_ObjectCenterX * Math.Cos(angle) - m_arrLGauge[position][intIndex1].ref_ObjectCenterY * Math.Sin(angle)));
            //newY1 = ((float)(m_arrLGauge[position][intIndex1].ref_ObjectCenterY * Math.Cos(angle) + m_arrLGauge[position][intIndex1].ref_ObjectCenterX * Math.Sin(angle)));

            //newX2 = ((float)(m_arrLGauge[position][intIndex2].ref_ObjectCenterX * Math.Cos(angle) - m_arrLGauge[position][intIndex2].ref_ObjectCenterY * Math.Sin(angle)));
            //newY2 = ((float)(m_arrLGauge[position][intIndex2].ref_ObjectCenterY * Math.Cos(angle) + m_arrLGauge[position][intIndex2].ref_ObjectCenterX * Math.Sin(angle)));

            if (blnWantX)
                return newX1 - newX2;
            else
                return newY1 - newY2;

        }
        //public float FixAngle(float angle, int direction, int position)
        //{
        //    if (direction == 0)
        //    {
        //        angle = (float)((-angle / 180) * Math.PI); //convert to theta

        //        /*************************************
        //         Formula Calculate
        //         X = xcos(θ) - ysin(θ) 
        //         Y = ycos(θ) + xsin(θ)

        //         θ = angle nid to rotate
        //        ***************************************/
        //        if (position == 0)
        //        {
        //            float Top1X = ((float)(m_arrLGauge[1][5].ref_ObjectCenterX * Math.Cos(angle) - m_arrLGauge[1][5].ref_ObjectCenterY * Math.Sin(angle)));
        //            float Top1Y = ((float)(m_arrLGauge[1][5].ref_ObjectCenterY * Math.Cos(angle) + m_arrLGauge[1][5].ref_ObjectCenterX * Math.Sin(angle)));

        //            float Top2X = ((float)(m_arrLGauge[1][4].ref_ObjectCenterX * Math.Cos(angle) - m_arrLGauge[1][4].ref_ObjectCenterY * Math.Sin(angle)));
        //            float Top2Y = ((float)(m_arrLGauge[1][4].ref_ObjectCenterY * Math.Cos(angle) + m_arrLGauge[1][4].ref_ObjectCenterX * Math.Sin(angle)));

        //            return Top1X - Top2X;
        //        }
        //        else if (position == 1)
        //        {
        //            float BottomX = ((float)(m_arrLGauge[3][5].ref_ObjectCenterX * Math.Cos(angle) - m_arrLGauge[3][5].ref_ObjectCenterY * Math.Sin(angle)));
        //            float BottomY = ((float)(m_arrLGauge[3][5].ref_ObjectCenterY * Math.Cos(angle) + m_arrLGauge[3][5].ref_ObjectCenterX * Math.Sin(angle)));

        //            float Bottom2X = ((float)(m_arrLGauge[3][4].ref_ObjectCenterX * Math.Cos(angle) - m_arrLGauge[3][4].ref_ObjectCenterY * Math.Sin(angle)));
        //            float Bottom2Y = ((float)(m_arrLGauge[3][4].ref_ObjectCenterY * Math.Cos(angle) + m_arrLGauge[3][4].ref_ObjectCenterX * Math.Sin(angle)));

        //            return BottomX - Bottom2X;
        //        }
        //        else if (position == 2)
        //        {
        //            float CenterX = ((float)(m_arrLGauge[0][8].ref_ObjectCenterX * Math.Cos(angle) - m_arrLGauge[0][8].ref_ObjectCenterY * Math.Sin(angle)));
        //            float CenterY = ((float)(m_arrLGauge[0][8].ref_ObjectCenterY * Math.Cos(angle) + m_arrLGauge[0][8].ref_ObjectCenterX * Math.Sin(angle)));

        //            float Center2X = ((float)(m_arrLGauge[0][6].ref_ObjectCenterX * Math.Cos(angle) - m_arrLGauge[0][6].ref_ObjectCenterY * Math.Sin(angle)));
        //            float Center2Y = ((float)(m_arrLGauge[0][6].ref_ObjectCenterY * Math.Cos(angle) + m_arrLGauge[0][6].ref_ObjectCenterX * Math.Sin(angle)));

        //            return CenterX - Center2X;
        //        }
        //        else if (position == 3)
        //        {
        //            float TopPixelX = ((float)(m_arrLGauge[1][2].ref_ObjectCenterX * Math.Cos(angle) - m_arrLGauge[1][2].ref_ObjectCenterY * Math.Sin(angle)));
        //            float TopPixelY = ((float)(m_arrLGauge[1][2].ref_ObjectCenterY * Math.Cos(angle) + m_arrLGauge[1][2].ref_ObjectCenterX * Math.Sin(angle)));

        //            float TopPixel2X = ((float)(m_arrLGauge[1][1].ref_ObjectCenterX * Math.Cos(angle) - m_arrLGauge[1][1].ref_ObjectCenterY * Math.Sin(angle)));
        //            float TopPixel2Y = ((float)(m_arrLGauge[1][1].ref_ObjectCenterY * Math.Cos(angle) + m_arrLGauge[1][1].ref_ObjectCenterX * Math.Sin(angle)));

        //            return TopPixelX - TopPixel2X;
        //        }
        //        else if (position == 4)
        //        {
        //            float TopDistanceX = ((float)(m_arrLGauge[1][0].ref_ObjectCenterX * Math.Cos(angle) - m_arrLGauge[1][0].ref_ObjectCenterY * Math.Sin(angle)));
        //            float TopDistanceY = ((float)(m_arrLGauge[1][0].ref_ObjectCenterY * Math.Cos(angle) + m_arrLGauge[1][0].ref_ObjectCenterX * Math.Sin(angle)));

        //            float TopDistance2X = ((float)(m_arrLGauge[1][6].ref_ObjectCenterX * Math.Cos(angle) - m_arrLGauge[1][6].ref_ObjectCenterY * Math.Sin(angle)));
        //            float TopDistance2Y = ((float)(m_arrLGauge[1][6].ref_ObjectCenterY * Math.Cos(angle) + m_arrLGauge[1][6].ref_ObjectCenterX * Math.Sin(angle)));

        //            return TopDistanceY - TopDistance2Y;
        //        }
        //        else if (position == 5)
        //        {
        //            float A1X = ((float)(m_arrLGauge[0][1].ref_ObjectCenterX * Math.Cos(angle) - m_arrLGauge[0][1].ref_ObjectCenterY * Math.Sin(angle)));
        //            float A1Y = ((float)(m_arrLGauge[0][1].ref_ObjectCenterY * Math.Cos(angle) + m_arrLGauge[0][1].ref_ObjectCenterX * Math.Sin(angle)));

        //            float A2X = ((float)(m_arrLGauge[0][7].ref_ObjectCenterX * Math.Cos(angle) - m_arrLGauge[0][7].ref_ObjectCenterY * Math.Sin(angle)));
        //            float A2Y = ((float)(m_arrLGauge[0][7].ref_ObjectCenterY * Math.Cos(angle) + m_arrLGauge[0][7].ref_ObjectCenterX * Math.Sin(angle)));

        //            return A1Y - A2Y;
        //        }
        //        else if (position == 6)
        //        {
        //            float BDistanceX = ((float)(m_arrLGauge[3][6].ref_ObjectCenterX * Math.Cos(angle) - m_arrLGauge[3][6].ref_ObjectCenterY * Math.Sin(angle)));
        //            float BDistanceY = ((float)(m_arrLGauge[3][6].ref_ObjectCenterY * Math.Cos(angle) + m_arrLGauge[3][6].ref_ObjectCenterX * Math.Sin(angle)));

        //            float BDistance2X = ((float)(m_arrLGauge[3][0].ref_ObjectCenterX * Math.Cos(angle) - m_arrLGauge[3][0].ref_ObjectCenterY * Math.Sin(angle)));
        //            float BDistance2Y = ((float)(m_arrLGauge[3][0].ref_ObjectCenterY * Math.Cos(angle) + m_arrLGauge[3][0].ref_ObjectCenterX * Math.Sin(angle)));

        //            return BDistanceY - BDistance2Y;
        //        }
        //        else
        //        {
        //            float B1X = ((float)(m_arrLGauge[0][16].ref_ObjectCenterX * Math.Cos(angle) - m_arrLGauge[0][16].ref_ObjectCenterY * Math.Sin(angle)));
        //            float B1Y = ((float)(m_arrLGauge[0][16].ref_ObjectCenterY * Math.Cos(angle) + m_arrLGauge[0][16].ref_ObjectCenterX * Math.Sin(angle)));

        //            float B2X = ((float)(m_arrLGauge[0][10].ref_ObjectCenterX * Math.Cos(angle) - m_arrLGauge[0][10].ref_ObjectCenterY * Math.Sin(angle)));
        //            float B2Y = ((float)(m_arrLGauge[0][10].ref_ObjectCenterY * Math.Cos(angle) + m_arrLGauge[0][10].ref_ObjectCenterX * Math.Sin(angle)));

        //            return B1Y - B2Y;
        //        }
        //    }
        //    else
        //    {
        //        angle = (float)(-(angle / 180) * Math.PI); //convert to theta

        //        /*************************************
        //         Formula Calculate
        //         X = xcos(θ) - ysin(θ) 
        //         Y = ycos(θ) + xsin(θ)

        //         θ = angle nid to rotate
        //        ***************************************/

        //        if (position == 0)
        //        {
        //            float LeftX = ((float)(m_arrLGauge[4][5].ref_ObjectCenterX * Math.Cos(angle) - m_arrLGauge[4][5].ref_ObjectCenterY * Math.Sin(angle)));
        //            float LeftY = ((float)(m_arrLGauge[4][5].ref_ObjectCenterY * Math.Cos(angle) + m_arrLGauge[4][5].ref_ObjectCenterX * Math.Sin(angle)));

        //            float Left2X = ((float)(m_arrLGauge[4][4].ref_ObjectCenterX * Math.Cos(angle) - m_arrLGauge[4][4].ref_ObjectCenterY * Math.Sin(angle)));
        //            float Left2Y = ((float)(m_arrLGauge[4][4].ref_ObjectCenterY * Math.Cos(angle) + m_arrLGauge[4][4].ref_ObjectCenterX * Math.Sin(angle)));

        //            return Left2Y - LeftY;
        //        }
        //        else if (position == 1)
        //        {
        //            float RightX = ((float)(m_arrLGauge[2][5].ref_ObjectCenterX * Math.Cos(angle) - m_arrLGauge[2][5].ref_ObjectCenterY * Math.Sin(angle)));
        //            float RightY = ((float)(m_arrLGauge[2][5].ref_ObjectCenterY * Math.Cos(angle) + m_arrLGauge[2][5].ref_ObjectCenterX * Math.Sin(angle)));

        //            float Right2X = ((float)(m_arrLGauge[2][4].ref_ObjectCenterX * Math.Cos(angle) - m_arrLGauge[2][4].ref_ObjectCenterY * Math.Sin(angle)));
        //            float Right2Y = ((float)(m_arrLGauge[2][4].ref_ObjectCenterY * Math.Cos(angle) + m_arrLGauge[2][4].ref_ObjectCenterX * Math.Sin(angle)));

        //            return Right2Y - RightY;
        //        }
        //        else if (position == 2)
        //        {
        //            float CenterX = ((float)(m_arrLGauge[0][2].ref_ObjectCenterX * Math.Cos(angle) - m_arrLGauge[0][2].ref_ObjectCenterY * Math.Sin(angle)));
        //            float CenterY = ((float)(m_arrLGauge[0][2].ref_ObjectCenterY * Math.Cos(angle) + m_arrLGauge[0][2].ref_ObjectCenterX * Math.Sin(angle)));

        //            float Center2X = ((float)(m_arrLGauge[0][0].ref_ObjectCenterX * Math.Cos(angle) - m_arrLGauge[0][0].ref_ObjectCenterY * Math.Sin(angle)));
        //            float Center2Y = ((float)(m_arrLGauge[0][0].ref_ObjectCenterY * Math.Cos(angle) + m_arrLGauge[0][0].ref_ObjectCenterX * Math.Sin(angle)));

        //            return CenterY - Center2Y;
        //        }
        //        else if (position == 3)
        //        {
        //            float LeftPixelX = ((float)(m_arrLGauge[4][1].ref_ObjectCenterX * Math.Cos(angle) - m_arrLGauge[4][1].ref_ObjectCenterY * Math.Sin(angle)));
        //            float LeftPixelY = ((float)(m_arrLGauge[4][1].ref_ObjectCenterY * Math.Cos(angle) + m_arrLGauge[4][1].ref_ObjectCenterX * Math.Sin(angle)));

        //            float LeftPixel2X = ((float)(m_arrLGauge[4][2].ref_ObjectCenterX * Math.Cos(angle) - m_arrLGauge[4][2].ref_ObjectCenterY * Math.Sin(angle)));
        //            float LeftPixel2Y = ((float)(m_arrLGauge[4][2].ref_ObjectCenterY * Math.Cos(angle) + m_arrLGauge[4][2].ref_ObjectCenterX * Math.Sin(angle)));

        //            return LeftPixelY - LeftPixel2Y;
        //        }
        //        else if (position == 4)
        //        {
        //            float LeftDistanceX = ((float)(m_arrLGauge[4][0].ref_ObjectCenterX * Math.Cos(angle) - m_arrLGauge[4][0].ref_ObjectCenterY * Math.Sin(angle)));
        //            float LeftDistanceY = ((float)(m_arrLGauge[4][0].ref_ObjectCenterY * Math.Cos(angle) + m_arrLGauge[4][0].ref_ObjectCenterX * Math.Sin(angle)));

        //            float LeftDistance2X = ((float)(m_arrLGauge[4][6].ref_ObjectCenterX * Math.Cos(angle) - m_arrLGauge[4][6].ref_ObjectCenterY * Math.Sin(angle)));
        //            float LeftDistance2Y = ((float)(m_arrLGauge[4][6].ref_ObjectCenterY * Math.Cos(angle) + m_arrLGauge[4][6].ref_ObjectCenterX * Math.Sin(angle)));

        //            return LeftDistanceX - LeftDistance2X;
        //        }
        //        else if (position == 5)
        //        {
        //            float A1X = ((float)(m_arrLGauge[0][1].ref_ObjectCenterX * Math.Cos(angle) - m_arrLGauge[0][1].ref_ObjectCenterY * Math.Sin(angle)));
        //            float A1Y = ((float)(m_arrLGauge[0][1].ref_ObjectCenterY * Math.Cos(angle) + m_arrLGauge[0][1].ref_ObjectCenterX * Math.Sin(angle)));

        //            float A2X = ((float)(m_arrLGauge[0][7].ref_ObjectCenterX * Math.Cos(angle) - m_arrLGauge[0][7].ref_ObjectCenterY * Math.Sin(angle)));
        //            float A2Y = ((float)(m_arrLGauge[0][7].ref_ObjectCenterY * Math.Cos(angle) + m_arrLGauge[0][7].ref_ObjectCenterX * Math.Sin(angle)));

        //            return A2X - A1X;
        //        }
        //        else if (position == 6)
        //        {
        //            float RightPixelX = ((float)(m_arrLGauge[2][1].ref_ObjectCenterX * Math.Cos(angle) - m_arrLGauge[2][1].ref_ObjectCenterY * Math.Sin(angle)));
        //            float RightPixelY = ((float)(m_arrLGauge[2][1].ref_ObjectCenterY * Math.Cos(angle) + m_arrLGauge[2][1].ref_ObjectCenterX * Math.Sin(angle)));

        //            float RightPixel2X = ((float)(m_arrLGauge[2][2].ref_ObjectCenterX * Math.Cos(angle) - m_arrLGauge[2][2].ref_ObjectCenterY * Math.Sin(angle)));
        //            float Rightixel2Y = ((float)(m_arrLGauge[2][2].ref_ObjectCenterY * Math.Cos(angle) + m_arrLGauge[2][2].ref_ObjectCenterX * Math.Sin(angle)));

        //            return RightPixelY - Rightixel2Y;
        //        }
        //        else if (position == 7)
        //        {
        //            float RightDistanceX = ((float)(m_arrLGauge[2][0].ref_ObjectCenterX * Math.Cos(angle) - m_arrLGauge[2][0].ref_ObjectCenterY * Math.Sin(angle)));
        //            float RightDistanceY = ((float)(m_arrLGauge[2][0].ref_ObjectCenterY * Math.Cos(angle) + m_arrLGauge[2][0].ref_ObjectCenterX * Math.Sin(angle)));

        //            float RightDistance2X = ((float)(m_arrLGauge[2][6].ref_ObjectCenterX * Math.Cos(angle) - m_arrLGauge[2][6].ref_ObjectCenterY * Math.Sin(angle)));
        //            float RightDistance2Y = ((float)(m_arrLGauge[2][6].ref_ObjectCenterY * Math.Cos(angle) + m_arrLGauge[2][6].ref_ObjectCenterX * Math.Sin(angle)));

        //            return RightDistance2X - RightDistanceX;
        //        }
        //        else
        //        {
        //            float B1X = ((float)(m_arrLGauge[0][16].ref_ObjectCenterX * Math.Cos(angle) - m_arrLGauge[0][16].ref_ObjectCenterY * Math.Sin(angle)));
        //            float B1Y = ((float)(m_arrLGauge[0][16].ref_ObjectCenterY * Math.Cos(angle) + m_arrLGauge[0][16].ref_ObjectCenterX * Math.Sin(angle)));

        //            float B2X = ((float)(m_arrLGauge[0][10].ref_ObjectCenterX * Math.Cos(angle) - m_arrLGauge[0][10].ref_ObjectCenterY * Math.Sin(angle)));
        //            float B2Y = ((float)(m_arrLGauge[0][10].ref_ObjectCenterY * Math.Cos(angle) + m_arrLGauge[0][10].ref_ObjectCenterX * Math.Sin(angle)));

        //            return B2X - B1X;
        //        }
        //    }
        //}
    }
}
