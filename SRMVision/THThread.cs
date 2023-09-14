using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Win32;
using SharedMemory;
using Common;

namespace SRMVision
{
    public class THThread
    {
        #region Member Variables
        // Thread handle
        private readonly object m_objStopLock = new object();
        private bool m_blnStopping = false;
        private bool m_blnStopped = false;

        private int[] m_intVSPassTotal = new int[10];
        private int[] m_intVSPassTotalPrev = new int[10];

        private int[] m_intVSPassImageCount = new int[10];
        private int[] m_intVSPassImageCountPrev = new int[10];

        private int[] m_intVSFailImageCount = new int[10];
        private int[] m_intVSFailImageCountPrev = new int[10];

        private int[] m_intVSTotalImageCount = new int[10];
        private int[] m_intVSTotalImageCountPrev = new int[10];

        private int[] m_intVSLowYieldUnitCount = new int[10];
        private int[] m_intVSLowYieldUnitCountPrev = new int[10];

        private int[] m_intVSContinuousPassUnitCount = new int[10];
        private int[] m_intVSContinuousPassUnitCountPrev = new int[10];

        private int[] m_intVSContinuousFailUnitCount = new int[10];
        private int[] m_intVSContinuousFailUnitCountPrev = new int[10];

        // ------------------ Fail variables -------------------------------------
        private int[] m_intVSOrientFailTotal = new int[10];
        private int[] m_intVSOrientFailTotalPrev = new int[10];

        private int[] m_intVSPin1FailTotal = new int[10];
        private int[] m_intVSPin1FailTotalPrev = new int[10];

        private int[] m_intVSMarkFailTotal = new int[10];
        private int[] m_intVSMarkFailTotalPrev = new int[10];
        
        private int[] m_intVSLeadFailTotal = new int[10];
        private int[] m_intVSLeadFailTotalPrev = new int[10];

        private int[] m_intVSPackageFailTotal = new int[10];
        private int[] m_intVSPackageFailTotalPrev = new int[10];

        private int[] m_intVSPadFailTotal = new int[10];
        private int[] m_intVSPadFailTotalPrev = new int[10];

        private int[] m_intVSBarcodeFailTotal = new int[10];
        private int[] m_intVSBarcodeFailTotalPrev = new int[10];

        private int[] m_intVSSealFailTotal = new int[10];
        private int[] m_intVSSealFailTotalPrev = new int[10];

        private int[] m_intVSSealDistanceFailTotal = new int[10];
        private int[] m_intVSSealDistanceFailTotalPrev = new int[10];

        private int[] m_intVSSealOverHeatFailTotal = new int[10];
        private int[] m_intVSSealOverHeatFailTotalPrev = new int[10];

        private int[] m_intVSSealBrokenAreaFailTotal = new int[10];
        private int[] m_intVSSealBrokenAreaFailTotalPrev = new int[10];

        private int[] m_intVSSealBrokenGapFailTotal = new int[10];
        private int[] m_intVSSealBrokenGapFailTotalPrev = new int[10];

        private int[] m_intVSSealSprocketHoleFailTotal = new int[10];
        private int[] m_intVSSealSprocketHoleFailTotalPrev = new int[10];

        private int[] m_intVSSealSprocketHoleDiameterFailTotal = new int[10];
        private int[] m_intVSSealSprocketHoleDiameterFailTotalPrev = new int[10];

        private int[] m_intVSSealSprocketHoleDefectFailTotal = new int[10];
        private int[] m_intVSSealSprocketHoleDefectFailTotalPrev = new int[10];

        private int[] m_intVSSealSprocketHoleBrokenFailTotal = new int[10];
        private int[] m_intVSSealSprocketHoleBrokenFailTotalPrev = new int[10];

        private int[] m_intVSSealSprocketHoleRoundnessFailTotal = new int[10];
        private int[] m_intVSSealSprocketHoleRoundnessFailTotalPrev = new int[10];

        private int[] m_intVSSealEdgeStraightnessFailTotal = new int[10];
        private int[] m_intVSSealEdgeStraightnessFailTotalPrev = new int[10];

        private int[] m_intVSCheckPresenceFailTotal = new int[10];
        private int[] m_intVSCheckPresenceFailTotalPrev = new int[10];

        private int[] m_intVSPositionFailTotal = new int[10];
        private int[] m_intVSPositionFailTotalPrev = new int[10];

        private int[] m_intVSNoTemplateFailTotal = new int[10];
        private int[] m_intVSNoTemplateFailTotalPrev = new int[10];

        private int[] m_intVSAngleFailTotal = new int[10];
        private int[] m_intVSAngleFailTotalPrev = new int[10];

        private int[] m_intVSEdgeNotFoundFailTotal = new int[10];
        private int[] m_intVSEdgeNotFoundFailTotalPrev = new int[10];

        private int[] m_intVSPkgDefectFailTotal = new int[10];
        private int[] m_intVSPkgDefectFailTotalPrev = new int[10];

        private int[] m_intVSPkgColorDefectFailTotal = new int[10];
        private int[] m_intVSPkgColorDefectFailTotalPrev = new int[10];

        private int[] m_intVSCenterPadOffsetFailTotal = new int[10];
        private int[] m_intVSCenterPadOffsetFailTotalPrev = new int[10];

        private int[] m_intVSCenterPadAreaFailTotal = new int[10];
        private int[] m_intVSCenterPadAreaFailTotalPrev = new int[10];

        private int[] m_intVSCenterPadDimensionFailTotal = new int[10];
        private int[] m_intVSCenterPadDimensionFailTotalPrev = new int[10];

        private int[] m_intVSCenterPadPitchGapFailTotal = new int[10];
        private int[] m_intVSCenterPadPitchGapFailTotalPrev = new int[10];

        private int[] m_intVSCenterPadBrokenFailTotal = new int[10];
        private int[] m_intVSCenterPadBrokenFailTotalPrev = new int[10];

        private int[] m_intVSCenterPadExcessFailTotal = new int[10];
        private int[] m_intVSCenterPadExcessFailTotalPrev = new int[10];

        private int[] m_intVSCenterPadSmearFailTotal = new int[10];
        private int[] m_intVSCenterPadSmearFailTotalPrev = new int[10];

        private int[] m_intVSCenterPadEdgeLimitFailTotal = new int[10];
        private int[] m_intVSCenterPadEdgeLimitFailTotalPrev = new int[10];

        private int[] m_intVSCenterPadStandOffFailTotal = new int[10];
        private int[] m_intVSCenterPadStandOffFailTotalPrev = new int[10];

        private int[] m_intVSCenterPadEdgeDistanceFailTotal = new int[10];
        private int[] m_intVSCenterPadEdgeDistanceFailTotalPrev = new int[10];

        private int[] m_intVSCenterPadSpanFailTotal = new int[10];
        private int[] m_intVSCenterPadSpanFailTotalPrev = new int[10];

        private int[] m_intVSCenterPadContaminationFailTotal = new int[10];
        private int[] m_intVSCenterPadContaminationFailTotalPrev = new int[10];

        private int[] m_intVSCenterPadMissingFailTotal = new int[10];
        private int[] m_intVSCenterPadMissingFailTotalPrev = new int[10];

        private int[] m_intVSCenterPadColorDefectFailTotal = new int[10];
        private int[] m_intVSCenterPadColorDefectFailTotalPrev = new int[10];

        private int[] m_intVSSidePadOffsetFailTotal = new int[10];
        private int[] m_intVSSidePadOffsetFailTotalPrev = new int[10];

        private int[] m_intVSSidePadAreaFailTotal = new int[10];
        private int[] m_intVSSidePadAreaFailTotalPrev = new int[10];

        private int[] m_intVSSidePadDimensionFailTotal = new int[10];
        private int[] m_intVSSidePadDimensionFailTotalPrev = new int[10];

        private int[] m_intVSSidePadPitchGapFailTotal = new int[10];
        private int[] m_intVSSidePadPitchGapFailTotalPrev = new int[10];

        private int[] m_intVSSidePadBrokenFailTotal = new int[10];
        private int[] m_intVSSidePadBrokenFailTotalPrev = new int[10];

        private int[] m_intVSSidePadExcessFailTotal = new int[10];
        private int[] m_intVSSidePadExcessFailTotalPrev = new int[10];

        private int[] m_intVSSidePadSmearFailTotal = new int[10];
        private int[] m_intVSSidePadSmearFailTotalPrev = new int[10];

        private int[] m_intVSSidePadEdgeLimitFailTotal = new int[10];
        private int[] m_intVSSidePadEdgeLimitFailTotalPrev = new int[10];

        private int[] m_intVSSidePadStandOffFailTotal = new int[10];
        private int[] m_intVSSidePadStandOffFailTotalPrev = new int[10];

        private int[] m_intVSSidePadEdgeDistanceFailTotal = new int[10];
        private int[] m_intVSSidePadEdgeDistanceFailTotalPrev = new int[10];

        private int[] m_intVSSidePadSpanFailTotal = new int[10];
        private int[] m_intVSSidePadSpanFailTotalPrev = new int[10];

        private int[] m_intVSSidePadContaminationFailTotal = new int[10];
        private int[] m_intVSSidePadContaminationFailTotalPrev = new int[10];

        private int[] m_intVSSidePadMissingFailTotal = new int[10];
        private int[] m_intVSSidePadMissingFailTotalPrev = new int[10];

        private int[] m_intVSSidePadColorDefectFailTotal = new int[10];
        private int[] m_intVSSidePadColorDefectFailTotalPrev = new int[10];

        private int[] m_intVSCenterPkgDefectFailTotal = new int[10];
        private int[] m_intVSCenterPkgDefectFailTotalPrev = new int[10];

        private int[] m_intVSCenterPkgDimensionFailTotal = new int[10];
        private int[] m_intVSCenterPkgDimensionFailTotalPrev = new int[10];

        private int[] m_intVSSidePkgDefectFailTotal = new int[10];
        private int[] m_intVSSidePkgDefectFailTotalPrev = new int[10];

        private int[] m_intVSSidePkgDimensionFailTotal = new int[10];
        private int[] m_intVSSidePkgDimensionFailTotalPrev = new int[10];

        private int[] m_intVSLeadOffsetFailTotal = new int[10];
        private int[] m_intVSLeadOffsetFailTotalPrev = new int[10];

        private int[] m_intVSLeadTipOffsetFailTotal = new int[10];
        private int[] m_intVSLeadTipOffsetFailTotalPrev = new int[10];

        private int[] m_intVSLeadWidthFailTotal = new int[10];
        private int[] m_intVSLeadWidthFailTotalPrev = new int[10];

        private int[] m_intVSLeadLengthFailTotal = new int[10];
        private int[] m_intVSLeadLengthFailTotalPrev = new int[10];

        private int[] m_intVSLeadLengthVarianceFailTotal = new int[10];
        private int[] m_intVSLeadLengthVarianceFailTotalPrev = new int[10];

        private int[] m_intVSLeadPitchGapFailTotal = new int[10];
        private int[] m_intVSLeadPitchGapFailTotalPrev = new int[10];

        private int[] m_intVSLeadPitchVarianceFailTotal = new int[10];
        private int[] m_intVSLeadPitchVarianceFailTotalPrev = new int[10];

        private int[] m_intVSLeadStandOffFailTotal = new int[10];
        private int[] m_intVSLeadStandOffFailTotalPrev = new int[10];

        private int[] m_intVSLeadStandOffVarianceFailTotal = new int[10];
        private int[] m_intVSLeadStandOffVarianceFailTotalPrev = new int[10];

        private int[] m_intVSLeadCoplanFailTotal = new int[10];
        private int[] m_intVSLeadCoplanFailTotalPrev = new int[10];

        private int[] m_intVSLeadAGVFailTotal = new int[10];
        private int[] m_intVSLeadAGVFailTotalPrev = new int[10];

        private int[] m_intVSLeadSpanFailTotal = new int[10];
        private int[] m_intVSLeadSpanFailTotalPrev = new int[10];

        private int[] m_intVSLeadSweepsFailTotal = new int[10];
        private int[] m_intVSLeadSweepsFailTotalPrev = new int[10];

        private int[] m_intVSLeadUnCutTiebarFailTotal = new int[10];
        private int[] m_intVSLeadUnCutTiebarFailTotalPrev = new int[10];

        private int[] m_intVSLeadMissingFailTotal = new int[10];
        private int[] m_intVSLeadMissingFailTotalPrev = new int[10];

        private int[] m_intVSLeadContaminationFailTotal = new int[10];
        private int[] m_intVSLeadContaminationFailTotalPrev = new int[10];

        private int[] m_intVSLeadPkgDefectFailTotal = new int[10];
        private int[] m_intVSLeadPkgDefectFailTotalPrev = new int[10];

        private int[] m_intVSLeadPkgDimensionFailTotal = new int[10];
        private int[] m_intVSLeadPkgDimensionFailTotalPrev = new int[10];

        private bool[] m_blnVSPocket1Pass = new bool[10];
        private bool[] m_blnVSPocket1PassPrev = new bool[10];

        private bool[] m_blnVSPocket2Pass = new bool[10];
        private bool[] m_blnVSPocket2PassPrev = new bool[10];

        private int[] m_intVSScenario = new int[10];
        private int[] m_intVSScenarioPrev = new int[10];

        private Thread m_thThread;
        private ProductionInfo m_smProductionInfo;
        private VisionInfo[] m_smVSInfo;

        #endregion

        public THThread(ProductionInfo smProductionInfo, VisionInfo[] smVSInfo)
        {
            m_smProductionInfo = smProductionInfo;
            m_smVSInfo = smVSInfo;

            for (int i = 0; i < 10; i++)
            {
                m_intVSPassTotal[i] = 0;
                m_intVSPassTotalPrev[i] = -1;

                m_intVSPassImageCount[i] = 0;
                m_intVSPassImageCountPrev[i] = -1;

                m_intVSFailImageCount[i] = 0;
                m_intVSFailImageCountPrev[i] = -1;

                m_intVSLowYieldUnitCount[i] = 0;
                m_intVSLowYieldUnitCountPrev[i] = -1;

                // ----- Fail Variables -----
                m_intVSOrientFailTotal[i] = 0;
                m_intVSOrientFailTotalPrev[i] = -1;

                m_intVSMarkFailTotal[i] = 0;
                m_intVSMarkFailTotalPrev[i] = -1;

                m_intVSLeadFailTotal[i] = 0;
                m_intVSLeadFailTotalPrev[i] = -1;

                m_intVSPackageFailTotal[i] = 0;
                m_intVSPackageFailTotalPrev[i] = -1;

                m_intVSPadFailTotal[i] = 0;
                m_intVSPadFailTotalPrev[i] = -1;
                
                m_intVSBarcodeFailTotal[i] = 0;
                m_intVSBarcodeFailTotalPrev[i] = -1;

                m_intVSSealFailTotal[i] = 0;
                m_intVSSealFailTotalPrev[i] = -1;

                m_intVSSealDistanceFailTotal[i] = 0;
                m_intVSSealDistanceFailTotalPrev[i] = -1;

                m_intVSSealOverHeatFailTotal[i] = 0;
                m_intVSSealOverHeatFailTotalPrev[i] = -1;

                m_intVSSealBrokenAreaFailTotal[i] = 0;
                m_intVSSealBrokenAreaFailTotalPrev[i] = -1;

                m_intVSSealBrokenGapFailTotal[i] = 0;
                m_intVSSealBrokenGapFailTotalPrev[i] = -1;

                m_intVSSealSprocketHoleFailTotal[i] = 0;
                m_intVSSealSprocketHoleFailTotalPrev[i] = -1;

                m_intVSSealSprocketHoleDiameterFailTotal[i] = 0;
                m_intVSSealSprocketHoleDiameterFailTotalPrev[i] = -1;

                m_intVSSealSprocketHoleDefectFailTotal[i] = 0;
                m_intVSSealSprocketHoleDefectFailTotalPrev[i] = -1;

                m_intVSSealSprocketHoleBrokenFailTotal[i] = 0;
                m_intVSSealSprocketHoleBrokenFailTotalPrev[i] = -1;

                m_intVSSealSprocketHoleRoundnessFailTotal[i] = 0;
                m_intVSSealSprocketHoleRoundnessFailTotalPrev[i] = -1;

                m_intVSSealEdgeStraightnessFailTotal[i] = 0;
                m_intVSSealEdgeStraightnessFailTotalPrev[i] = -1;

                m_intVSCheckPresenceFailTotal[i] = 0;
                m_intVSCheckPresenceFailTotalPrev[i] = -1;

                m_intVSPositionFailTotal[i] = 0;
                m_intVSPositionFailTotalPrev[i] = -1;

                m_intVSNoTemplateFailTotal[i] = 0;
                m_intVSNoTemplateFailTotalPrev[i] = -1;

                m_intVSAngleFailTotal[i] = 0;
                m_intVSAngleFailTotalPrev[i] = -1;

                m_intVSEdgeNotFoundFailTotal[i] = 0;
                m_intVSEdgeNotFoundFailTotalPrev[i] = -1;

                m_intVSPkgDefectFailTotal[i] = 0;
                m_intVSPkgDefectFailTotalPrev[i] = -1;

                m_intVSPkgColorDefectFailTotal[i] = 0;
                m_intVSPkgColorDefectFailTotalPrev[i] = -1;

                m_intVSCenterPadOffsetFailTotal[i] = 0;
                m_intVSCenterPadOffsetFailTotalPrev[i] = -1;

                m_intVSCenterPadAreaFailTotal[i] = 0;
                m_intVSCenterPadAreaFailTotalPrev[i] = -1;

                m_intVSCenterPadDimensionFailTotal[i] = 0;
                m_intVSCenterPadDimensionFailTotalPrev[i] = -1;

                m_intVSCenterPadPitchGapFailTotal[i] = 0;
                m_intVSCenterPadPitchGapFailTotalPrev[i] = -1;

                m_intVSCenterPadBrokenFailTotal[i] = 0;
                m_intVSCenterPadBrokenFailTotalPrev[i] = -1;

                m_intVSCenterPadExcessFailTotal[i] = 0;
                m_intVSCenterPadExcessFailTotalPrev[i] = -1;

                m_intVSCenterPadSmearFailTotal[i] = 0;
                m_intVSCenterPadSmearFailTotalPrev[i] = -1;

                m_intVSCenterPadEdgeLimitFailTotal[i] = 0;
                m_intVSCenterPadEdgeLimitFailTotalPrev[i] = -1;

                m_intVSCenterPadStandOffFailTotal[i] = 0;
                m_intVSCenterPadStandOffFailTotalPrev[i] = -1;

                m_intVSCenterPadEdgeDistanceFailTotal[i] = 0;
                m_intVSCenterPadEdgeDistanceFailTotalPrev[i] = -1;

                m_intVSCenterPadSpanFailTotal[i] = 0;
                m_intVSCenterPadSpanFailTotalPrev[i] = -1;

                m_intVSCenterPadContaminationFailTotal[i] = 0;
                m_intVSCenterPadContaminationFailTotalPrev[i] = -1;

                m_intVSCenterPadMissingFailTotal[i] = 0;
                m_intVSCenterPadMissingFailTotalPrev[i] = -1;

                m_intVSCenterPadColorDefectFailTotal[i] = 0;
                m_intVSCenterPadColorDefectFailTotalPrev[i] = -1;

                m_intVSSidePadOffsetFailTotal[i] = 0;
                m_intVSSidePadOffsetFailTotalPrev[i] = -1;

                m_intVSSidePadAreaFailTotal[i] = 0;
                m_intVSSidePadAreaFailTotalPrev[i] = -1;

                m_intVSSidePadDimensionFailTotal[i] = 0;
                m_intVSSidePadDimensionFailTotalPrev[i] = -1;

                m_intVSSidePadPitchGapFailTotal[i] = 0;
                m_intVSSidePadPitchGapFailTotalPrev[i] = -1;

                m_intVSSidePadBrokenFailTotal[i] = 0;
                m_intVSSidePadBrokenFailTotalPrev[i] = -1;

                m_intVSSidePadExcessFailTotal[i] = 0;
                m_intVSSidePadExcessFailTotalPrev[i] = -1;

                m_intVSSidePadSmearFailTotal[i] = 0;
                m_intVSSidePadSmearFailTotalPrev[i] = -1;

                m_intVSSidePadEdgeLimitFailTotal[i] = 0;
                m_intVSSidePadEdgeLimitFailTotalPrev[i] = -1;

                m_intVSSidePadStandOffFailTotal[i] = 0;
                m_intVSSidePadStandOffFailTotalPrev[i] = -1;

                m_intVSSidePadEdgeDistanceFailTotal[i] = 0;
                m_intVSSidePadEdgeDistanceFailTotalPrev[i] = -1;

                m_intVSSidePadSpanFailTotal[i] = 0;
                m_intVSSidePadSpanFailTotalPrev[i] = -1;

                m_intVSSidePadContaminationFailTotal[i] = 0;
                m_intVSSidePadContaminationFailTotalPrev[i] = -1;

                m_intVSSidePadMissingFailTotal[i] = 0;
                m_intVSSidePadMissingFailTotalPrev[i] = -1;

                m_intVSSidePadColorDefectFailTotal[i] = 0;
                m_intVSSidePadColorDefectFailTotalPrev[i] = -1;

                m_intVSCenterPkgDefectFailTotal[i] = 0;
                m_intVSCenterPkgDefectFailTotalPrev[i] = -1;

                m_intVSCenterPkgDimensionFailTotal[i] = 0;
                m_intVSCenterPkgDimensionFailTotalPrev[i] = -1;

                m_intVSSidePkgDefectFailTotal[i] = 0;
                m_intVSSidePkgDefectFailTotalPrev[i] = -1;

                m_intVSSidePkgDimensionFailTotal[i] = 0;
                m_intVSSidePkgDimensionFailTotalPrev[i] = -1;

                m_intVSLeadOffsetFailTotal[i] = 0;
                m_intVSLeadOffsetFailTotalPrev[i] = -1;

                m_intVSLeadTipOffsetFailTotal[i] = 0;
                m_intVSLeadTipOffsetFailTotalPrev[i] = 0;

                m_intVSLeadWidthFailTotal[i] = 0;
                m_intVSLeadWidthFailTotalPrev[i] = -1;

                m_intVSLeadLengthFailTotal[i] = 0;
                m_intVSLeadLengthFailTotalPrev[i] = -1;

                m_intVSLeadLengthVarianceFailTotal[i] = 0;
                m_intVSLeadLengthVarianceFailTotalPrev[i] = -1;

                m_intVSLeadPitchGapFailTotal[i] = 0;
                m_intVSLeadPitchGapFailTotalPrev[i] = -1;

                m_intVSLeadPitchVarianceFailTotal[i] = 0;
                m_intVSLeadPitchVarianceFailTotalPrev[i] = -1;

                m_intVSLeadStandOffFailTotal[i] = 0;
                m_intVSLeadStandOffFailTotalPrev[i] = -1;

                m_intVSLeadStandOffVarianceFailTotal[i] = 0;
                m_intVSLeadStandOffVarianceFailTotalPrev[i] = -1;

                m_intVSLeadCoplanFailTotal[i] = 0;
                m_intVSLeadCoplanFailTotalPrev[i] = -1;

                m_intVSLeadAGVFailTotal[i] = 0;
                m_intVSLeadAGVFailTotalPrev[i] = -1;

                m_intVSLeadSpanFailTotal[i] = 0;
                m_intVSLeadSpanFailTotalPrev[i] = -1;

                m_intVSLeadSweepsFailTotal[i] = 0;
                m_intVSLeadSweepsFailTotalPrev[i] = -1;

                m_intVSLeadUnCutTiebarFailTotal[i] = 0;
                m_intVSLeadUnCutTiebarFailTotalPrev[i] = -1;

                m_intVSLeadMissingFailTotal[i] = 0;
                m_intVSLeadMissingFailTotalPrev[i] = -1;

                m_intVSLeadContaminationFailTotal[i] = 0;
                m_intVSLeadContaminationFailTotalPrev[i] = -1;

                m_intVSLeadPkgDefectFailTotal[i] = 0;
                m_intVSLeadPkgDefectFailTotalPrev[i] = -1;

                m_intVSLeadPkgDimensionFailTotal[i] = 0;
                m_intVSLeadPkgDimensionFailTotalPrev[i] = -1;

                m_blnVSPocket1Pass[i] = false;
                m_blnVSPocket1PassPrev[i] = false;

                m_blnVSPocket2Pass[i] = false;
                m_blnVSPocket2PassPrev[i] = false;

                m_intVSScenario[i] = 0;
                m_intVSScenarioPrev[i] = -1;
            }

            //List<string> arrThreadNameBF = new List<string>();
            //List<string> arrThreadNameAF = new List<string>();
            //arrThreadNameBF = ProcessTh.GetThreadsName("SRMVision");

            m_thThread = new Thread(new ThreadStart(UpdateProgress));
            m_thThread.IsBackground = true;
            m_thThread.Priority = ThreadPriority.Lowest;
            m_thThread.Start();

            //Thread.Sleep(500);
            //arrThreadNameAF = ProcessTh.GetThreadsName("SRMVision");
            //ProcessTh.GetDifferentThreadsName(arrThreadNameAF, arrThreadNameBF, "6", 0x02);
        }


        /// <summary>
        /// Returns whether the worker thread has stopped.
        /// </summary>
        public bool IsThreadStopped
        {
            get
            {
                lock (m_objStopLock)
                {
                    return m_blnStopped;
                }
            }
        }
        /// <summary>
        /// Tells the thread to stop, typically after completing its 
        /// current work item.
        /// </summary>
        public void StopThread()
        {
            lock (m_objStopLock)
            {
                m_blnStopping = true;
            }
        }


        /// <summary>
        /// Called by the thread to indicate when it has stopped.
        /// </summary>
        private void SetStopped()
        {
            lock (m_objStopLock)
            {
                m_blnStopped = true;
            }
        }

        private void UpdateProgress()
        {
            try
            {
                while (!m_blnStopping)
                {
                    if (m_smProductionInfo.VM_TH_UpdateCount)
                    {
                        WriteAutoModeRegistry();
                        m_smProductionInfo.VM_TH_UpdateCount = false;
                    }

                    Thread.Sleep(1);
                }
            }
            finally
            {
                SetStopped();
            }

        }

        private void WriteAutoModeRegistry()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey1 = key.CreateSubKey("SVG\\AutoMode");

            #region Save General Variables

            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null)
                    continue;
                //Save Pass Total
                m_intVSPassTotal[i] = m_smVSInfo[i].g_intPassTotal;
                if (m_intVSPassTotal[i] != m_intVSPassTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "Pass", m_intVSPassTotal[i]);
                    m_intVSPassTotalPrev[i] = m_intVSPassTotal[i];
                }
                // Save Pass Image Count
                m_intVSPassImageCount[i] = m_smVSInfo[i].g_intPassImageCount;
                if (m_intVSPassImageCount[i] != m_intVSPassImageCountPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "PassImageCount", m_intVSPassImageCount[i]);
                    m_intVSPassImageCountPrev[i] = m_intVSPassImageCount[i];
                }
                // Save Fail Image Count
                m_intVSFailImageCount[i] = m_smVSInfo[i].g_intFailImageCount;
                if (m_intVSFailImageCount[i] != m_intVSFailImageCountPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "FailImageCount", m_intVSFailImageCount[i]);
                    m_intVSFailImageCountPrev[i] = m_intVSFailImageCount[i];
                }

                // Save Total Image Count
                m_intVSTotalImageCount[i] = m_smVSInfo[i].g_intTotalImageCount;
                if (m_intVSTotalImageCount[i] != m_intVSTotalImageCountPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "TotalImageCount", m_intVSTotalImageCount[i]);
                    m_intVSTotalImageCountPrev[i] = m_intVSTotalImageCount[i];
                }

                // Save Low Yield Count
                m_intVSLowYieldUnitCount[i] = m_smVSInfo[i].g_intLowYieldUnitCount;
                if (m_intVSLowYieldUnitCount[i] != m_intVSLowYieldUnitCountPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "LowYieldUnit", m_intVSLowYieldUnitCount[i]);
                    m_intVSLowYieldUnitCountPrev[i] = m_intVSLowYieldUnitCount[i];
                }
                // Save Continuous Pass Count
                m_intVSContinuousPassUnitCount[i] = m_smVSInfo[i].g_intContinuousPassUnitCount;
                if (m_intVSContinuousPassUnitCount[i] != m_intVSContinuousPassUnitCountPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "ContinuousPassUnit", m_intVSContinuousPassUnitCount[i]);
                    m_intVSContinuousPassUnitCountPrev[i] = m_intVSContinuousPassUnitCount[i];
                }
                // Save Continuous Fail Count
                m_intVSContinuousFailUnitCount[i] = m_smVSInfo[i].g_intContinuousFailUnitCount;
                if (m_intVSContinuousFailUnitCount[i] != m_intVSContinuousFailUnitCountPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "ContinuousFailUnit", m_intVSContinuousFailUnitCount[i]);
                    m_intVSContinuousFailUnitCountPrev[i] = m_intVSContinuousFailUnitCount[i];
                }
                // ----------------- Save Fail Variables
                m_intVSOrientFailTotal[i] = m_smVSInfo[i].g_intOrientFailureTotal;
                if (m_intVSOrientFailTotal[i] != m_intVSOrientFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "OrientFail", m_intVSOrientFailTotal[i]);
                    m_intVSOrientFailTotalPrev[i] = m_intVSOrientFailTotal[i];
                }

                m_intVSPin1FailTotal[i] = m_smVSInfo[i].g_intPin1FailureTotal;
                if (m_intVSPin1FailTotal[i] != m_intVSPin1FailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "Pin1Fail", m_intVSPin1FailTotal[i]);
                    m_intVSPin1FailTotalPrev[i] = m_intVSPin1FailTotal[i];
                }

                m_intVSMarkFailTotal[i] = m_smVSInfo[i].g_intMarkFailureTotal;
                if (m_intVSMarkFailTotal[i] != m_intVSMarkFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "MarkFail", m_intVSMarkFailTotal[i]);
                    m_intVSMarkFailTotalPrev[i] = m_intVSMarkFailTotal[i];
                }

                m_intVSLeadFailTotal[i] = m_smVSInfo[i].g_intLeadFailureTotal;
                if (m_intVSLeadFailTotal[i] != m_intVSLeadFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "LeadFail", m_intVSLeadFailTotal[i]);
                    m_intVSLeadFailTotalPrev[i] = m_intVSLeadFailTotal[i];
                }

                m_intVSPackageFailTotal[i] = m_smVSInfo[i].g_intPackageFailureTotal;
                if (m_intVSPackageFailTotal[i] != m_intVSPackageFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "PackageFail", m_intVSPackageFailTotal[i]);
                    m_intVSPackageFailTotalPrev[i] = m_intVSPackageFailTotal[i];
                }

                m_intVSPadFailTotal[i] = m_smVSInfo[i].g_intPadFailureTotal;
                if (m_intVSPadFailTotal[i] != m_intVSPadFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "PadFail", m_intVSPadFailTotal[i]);
                    m_intVSPadFailTotalPrev[i] = m_intVSPadFailTotal[i];
                }

                m_intVSBarcodeFailTotal[i] = m_smVSInfo[i].g_intBarcodeFailureTotal;
                if (m_intVSBarcodeFailTotal[i] != m_intVSBarcodeFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "BarcodeFail", m_intVSBarcodeFailTotal[i]);
                    m_intVSBarcodeFailTotalPrev[i] = m_intVSBarcodeFailTotal[i];
                }

                m_intVSSealFailTotal[i] = m_smVSInfo[i].g_intSealFailureTotal;
                if (m_intVSSealFailTotal[i] != m_intVSSealFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "SealFail", m_intVSSealFailTotal[i]);
                    m_intVSSealFailTotalPrev[i] = m_intVSSealFailTotal[i];
                }

                m_intVSSealDistanceFailTotal[i] = m_smVSInfo[i].g_intSealDistanceFailureTotal;
                if (m_intVSSealDistanceFailTotal[i] != m_intVSSealDistanceFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "SealDistanceFail", m_intVSSealDistanceFailTotal[i]);
                    m_intVSSealDistanceFailTotalPrev[i] = m_intVSSealDistanceFailTotal[i];
                }

                m_intVSSealOverHeatFailTotal[i] = m_smVSInfo[i].g_intSealOverHeatFailureTotal;
                if (m_intVSSealOverHeatFailTotal[i] != m_intVSSealOverHeatFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "SealOverHeatFail", m_intVSSealOverHeatFailTotal[i]);
                    m_intVSSealOverHeatFailTotalPrev[i] = m_intVSSealOverHeatFailTotal[i];
                }

                m_intVSSealBrokenAreaFailTotal[i] = m_smVSInfo[i].g_intSealBrokenAreaFailureTotal;
                if (m_intVSSealBrokenAreaFailTotal[i] != m_intVSSealBrokenAreaFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "SealBrokenAreaFail", m_intVSSealBrokenAreaFailTotal[i]);
                    m_intVSSealBrokenAreaFailTotalPrev[i] = m_intVSSealBrokenAreaFailTotal[i];
                }

                m_intVSSealBrokenGapFailTotal[i] = m_smVSInfo[i].g_intSealBrokenGapFailureTotal;
                if (m_intVSSealBrokenGapFailTotal[i] != m_intVSSealBrokenGapFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "SealBrokenGapFail", m_intVSSealBrokenGapFailTotal[i]);
                    m_intVSSealBrokenGapFailTotalPrev[i] = m_intVSSealBrokenGapFailTotal[i];
                }

                m_intVSSealSprocketHoleFailTotal[i] = m_smVSInfo[i].g_intSealSprocketHoleFailureTotal;
                if (m_intVSSealSprocketHoleFailTotal[i] != m_intVSSealSprocketHoleFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "SealSprocketHoleFail", m_intVSSealSprocketHoleFailTotal[i]);
                    m_intVSSealSprocketHoleFailTotalPrev[i] = m_intVSSealSprocketHoleFailTotal[i];
                }

                m_intVSSealSprocketHoleDiameterFailTotal[i] = m_smVSInfo[i].g_intSealSprocketHoleDiameterFailureTotal;
                if (m_intVSSealSprocketHoleDiameterFailTotal[i] != m_intVSSealSprocketHoleDiameterFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "SealSprocketHoleDiameterFail", m_intVSSealSprocketHoleDiameterFailTotal[i]);
                    m_intVSSealSprocketHoleDiameterFailTotalPrev[i] = m_intVSSealSprocketHoleDiameterFailTotal[i];
                }

                m_intVSSealSprocketHoleDefectFailTotal[i] = m_smVSInfo[i].g_intSealSprocketHoleDefectFailureTotal;
                if (m_intVSSealSprocketHoleDefectFailTotal[i] != m_intVSSealSprocketHoleDefectFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "SealSprocketHoleDefectFail", m_intVSSealSprocketHoleDefectFailTotal[i]);
                    m_intVSSealSprocketHoleDefectFailTotalPrev[i] = m_intVSSealSprocketHoleDefectFailTotal[i];
                }

                m_intVSSealSprocketHoleBrokenFailTotal[i] = m_smVSInfo[i].g_intSealSprocketHoleBrokenFailureTotal;
                if (m_intVSSealSprocketHoleBrokenFailTotal[i] != m_intVSSealSprocketHoleBrokenFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "SealSprocketHoleBrokenFail", m_intVSSealSprocketHoleBrokenFailTotal[i]);
                    m_intVSSealSprocketHoleBrokenFailTotalPrev[i] = m_intVSSealSprocketHoleBrokenFailTotal[i];
                }

                m_intVSSealEdgeStraightnessFailTotal[i] = m_smVSInfo[i].g_intSealEdgeStraightnessFailureTotal;
                if (m_intVSSealEdgeStraightnessFailTotal[i] != m_intVSSealEdgeStraightnessFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "SealEdgeStraightnessFail", m_intVSSealEdgeStraightnessFailTotal[i]);
                    m_intVSSealEdgeStraightnessFailTotalPrev[i] = m_intVSSealEdgeStraightnessFailTotal[i];
                }

                m_intVSSealSprocketHoleRoundnessFailTotal[i] = m_smVSInfo[i].g_intSealSprocketHoleRoundnessFailureTotal;
                if (m_intVSSealSprocketHoleRoundnessFailTotal[i] != m_intVSSealSprocketHoleRoundnessFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "SealSprocketHoleRoundnessFail", m_intVSSealSprocketHoleRoundnessFailTotal[i]);
                    m_intVSSealSprocketHoleRoundnessFailTotalPrev[i] = m_intVSSealSprocketHoleRoundnessFailTotal[i];
                }

                m_intVSCheckPresenceFailTotal[i] = m_smVSInfo[i].g_intCheckPresenceFailureTotal;
                if (m_intVSCheckPresenceFailTotal[i] != m_intVSCheckPresenceFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "CheckPresenceFail", m_intVSCheckPresenceFailTotal[i]);
                    m_intVSCheckPresenceFailTotalPrev[i] = m_intVSCheckPresenceFailTotal[i];
                }

                m_intVSPositionFailTotal[i] = m_smVSInfo[i].g_intPositionFailureTotal;
                if (m_intVSPositionFailTotal[i] != m_intVSPositionFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "PositionFail", m_intVSPositionFailTotal[i]);
                    m_intVSPositionFailTotalPrev[i] = m_intVSPositionFailTotal[i];
                }

                m_intVSNoTemplateFailTotal[i] = m_smVSInfo[i].g_intNoTemplateFailureTotal;
                if (m_intVSNoTemplateFailTotal[i] != m_intVSNoTemplateFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "NoTemplateFail", m_intVSNoTemplateFailTotal[i]);
                    m_intVSNoTemplateFailTotalPrev[i] = m_intVSNoTemplateFailTotal[i];
                }

                m_intVSEdgeNotFoundFailTotal[i] = m_smVSInfo[i].g_intEdgeNotFoundFailureTotal;
                if (m_intVSEdgeNotFoundFailTotal[i] != m_intVSEdgeNotFoundFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "EdgeNotFoundFail", m_intVSEdgeNotFoundFailTotal[i]);
                    m_intVSEdgeNotFoundFailTotalPrev[i] = m_intVSEdgeNotFoundFailTotal[i];
                }

                m_intVSAngleFailTotal[i] = m_smVSInfo[i].g_intAngleFailureTotal;
                if (m_intVSAngleFailTotal[i] != m_intVSAngleFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "AngleFail", m_intVSAngleFailTotal[i]);
                    m_intVSAngleFailTotalPrev[i] = m_intVSAngleFailTotal[i];
                }

                m_intVSPkgDefectFailTotal[i] = m_smVSInfo[i].g_intPkgDefectFailureTotal;
                if (m_intVSPkgDefectFailTotal[i] != m_intVSPkgDefectFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "PkgDefectFail", m_intVSPkgDefectFailTotal[i]);
                    m_intVSPkgDefectFailTotalPrev[i] = m_intVSPkgDefectFailTotal[i];
                }

                m_intVSPkgColorDefectFailTotal[i] = m_smVSInfo[i].g_intPkgColorDefectFailureTotal;
                if (m_intVSPkgColorDefectFailTotal[i] != m_intVSPkgColorDefectFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "PkgColorDefectFail", m_intVSPkgColorDefectFailTotal[i]);
                    m_intVSPkgColorDefectFailTotalPrev[i] = m_intVSPkgColorDefectFailTotal[i];
                }

                m_intVSCenterPadOffsetFailTotal[i] = m_smVSInfo[i].g_intCenterPadOffsetFailureTotal;
                if (m_intVSCenterPadOffsetFailTotal[i] != m_intVSCenterPadOffsetFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "CenterPadOffsetFail", m_intVSCenterPadOffsetFailTotal[i]);
                    m_intVSCenterPadOffsetFailTotalPrev[i] = m_intVSCenterPadOffsetFailTotal[i];
                }

                m_intVSCenterPadAreaFailTotal[i] = m_smVSInfo[i].g_intCenterPadAreaFailureTotal;
                if (m_intVSCenterPadAreaFailTotal[i] != m_intVSCenterPadAreaFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "CenterPadAreaFail", m_intVSCenterPadAreaFailTotal[i]);
                    m_intVSCenterPadAreaFailTotalPrev[i] = m_intVSCenterPadAreaFailTotal[i];
                }

                m_intVSCenterPadDimensionFailTotal[i] = m_smVSInfo[i].g_intCenterPadDimensionFailureTotal;
                if (m_intVSCenterPadDimensionFailTotal[i] != m_intVSCenterPadDimensionFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "CenterPadDimensionFail", m_intVSCenterPadDimensionFailTotal[i]);
                    m_intVSCenterPadDimensionFailTotalPrev[i] = m_intVSCenterPadDimensionFailTotal[i];
                }

                m_intVSCenterPadPitchGapFailTotal[i] = m_smVSInfo[i].g_intCenterPadPitchGapFailureTotal;
                if (m_intVSCenterPadPitchGapFailTotal[i] != m_intVSCenterPadPitchGapFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "CenterPadPitchGapFail", m_intVSCenterPadPitchGapFailTotal[i]);
                    m_intVSCenterPadPitchGapFailTotalPrev[i] = m_intVSCenterPadPitchGapFailTotal[i];
                }

                m_intVSCenterPadBrokenFailTotal[i] = m_smVSInfo[i].g_intCenterPadBrokenFailureTotal;
                if (m_intVSCenterPadBrokenFailTotal[i] != m_intVSCenterPadBrokenFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "CenterPadBrokenFail", m_intVSCenterPadBrokenFailTotal[i]);
                    m_intVSCenterPadBrokenFailTotalPrev[i] = m_intVSCenterPadBrokenFailTotal[i];
                }

                m_intVSCenterPadExcessFailTotal[i] = m_smVSInfo[i].g_intCenterPadExcessFailureTotal;
                if (m_intVSCenterPadExcessFailTotal[i] != m_intVSCenterPadExcessFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "CenterPadExcessFail", m_intVSCenterPadExcessFailTotal[i]);
                    m_intVSCenterPadExcessFailTotalPrev[i] = m_intVSCenterPadExcessFailTotal[i];
                }

                m_intVSCenterPadSmearFailTotal[i] = m_smVSInfo[i].g_intCenterPadSmearFailureTotal;
                if (m_intVSCenterPadSmearFailTotal[i] != m_intVSCenterPadSmearFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "CenterPadSmearFail", m_intVSCenterPadSmearFailTotal[i]);
                    m_intVSCenterPadSmearFailTotalPrev[i] = m_intVSCenterPadSmearFailTotal[i];
                }

                m_intVSCenterPadEdgeLimitFailTotal[i] = m_smVSInfo[i].g_intCenterPadEdgeLimitFailureTotal;
                if (m_intVSCenterPadEdgeLimitFailTotal[i] != m_intVSCenterPadEdgeLimitFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "CenterPadEdgeLimitFail", m_intVSCenterPadEdgeLimitFailTotal[i]);
                    m_intVSCenterPadEdgeLimitFailTotalPrev[i] = m_intVSCenterPadEdgeLimitFailTotal[i];
                }

                m_intVSCenterPadStandOffFailTotal[i] = m_smVSInfo[i].g_intCenterPadStandOffFailureTotal;
                if (m_intVSCenterPadStandOffFailTotal[i] != m_intVSCenterPadStandOffFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "CenterPadStandOffFail", m_intVSCenterPadStandOffFailTotal[i]);
                    m_intVSCenterPadStandOffFailTotalPrev[i] = m_intVSCenterPadStandOffFailTotal[i];
                }

                m_intVSCenterPadEdgeDistanceFailTotal[i] = m_smVSInfo[i].g_intCenterPadEdgeDistanceFailureTotal;
                if (m_intVSCenterPadEdgeDistanceFailTotal[i] != m_intVSCenterPadEdgeDistanceFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "CenterPadEdgeDistanceFail", m_intVSCenterPadEdgeDistanceFailTotal[i]);
                    m_intVSCenterPadEdgeDistanceFailTotalPrev[i] = m_intVSCenterPadEdgeDistanceFailTotal[i];
                }

                m_intVSCenterPadSpanFailTotal[i] = m_smVSInfo[i].g_intCenterPadSpanFailureTotal;
                if (m_intVSCenterPadSpanFailTotal[i] != m_intVSCenterPadSpanFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "CenterPadSpanFail", m_intVSCenterPadSpanFailTotal[i]);
                    m_intVSCenterPadSpanFailTotalPrev[i] = m_intVSCenterPadSpanFailTotal[i];
                }

                m_intVSCenterPadContaminationFailTotal[i] = m_smVSInfo[i].g_intCenterPadContaminationFailureTotal;
                if (m_intVSCenterPadContaminationFailTotal[i] != m_intVSCenterPadContaminationFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "CenterPadContaminationFail", m_intVSCenterPadContaminationFailTotal[i]);
                    m_intVSCenterPadContaminationFailTotalPrev[i] = m_intVSCenterPadContaminationFailTotal[i];
                }

                m_intVSCenterPadMissingFailTotal[i] = m_smVSInfo[i].g_intCenterPadMissingFailureTotal;
                if (m_intVSCenterPadMissingFailTotal[i] != m_intVSCenterPadMissingFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "CenterPadMissingFail", m_intVSCenterPadMissingFailTotal[i]);
                    m_intVSCenterPadMissingFailTotalPrev[i] = m_intVSCenterPadMissingFailTotal[i];
                }

                m_intVSCenterPadColorDefectFailTotal[i] = m_smVSInfo[i].g_intCenterPadColorDefectFailureTotal;
                if (m_intVSCenterPadColorDefectFailTotal[i] != m_intVSCenterPadColorDefectFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "CenterPadColorDefectFail", m_intVSCenterPadColorDefectFailTotal[i]);
                    m_intVSCenterPadColorDefectFailTotalPrev[i] = m_intVSCenterPadColorDefectFailTotal[i];
                }

                m_intVSSidePadOffsetFailTotal[i] = m_smVSInfo[i].g_intSidePadOffsetFailureTotal;
                if (m_intVSSidePadOffsetFailTotal[i] != m_intVSSidePadOffsetFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "SidePadOffsetFail", m_intVSSidePadOffsetFailTotal[i]);
                    m_intVSSidePadOffsetFailTotalPrev[i] = m_intVSSidePadOffsetFailTotal[i];
                }

                m_intVSSidePadAreaFailTotal[i] = m_smVSInfo[i].g_intSidePadAreaFailureTotal;
                if (m_intVSSidePadAreaFailTotal[i] != m_intVSSidePadAreaFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "SidePadAreaFail", m_intVSSidePadAreaFailTotal[i]);
                    m_intVSSidePadAreaFailTotalPrev[i] = m_intVSSidePadAreaFailTotal[i];
                }

                m_intVSSidePadDimensionFailTotal[i] = m_smVSInfo[i].g_intSidePadDimensionFailureTotal;
                if (m_intVSSidePadDimensionFailTotal[i] != m_intVSSidePadDimensionFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "SidePadDimensionFail", m_intVSSidePadDimensionFailTotal[i]);
                    m_intVSSidePadDimensionFailTotalPrev[i] = m_intVSSidePadDimensionFailTotal[i];
                }

                m_intVSSidePadPitchGapFailTotal[i] = m_smVSInfo[i].g_intSidePadPitchGapFailureTotal;
                if (m_intVSSidePadPitchGapFailTotal[i] != m_intVSSidePadPitchGapFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "SidePadPitchGapFail", m_intVSSidePadPitchGapFailTotal[i]);
                    m_intVSSidePadPitchGapFailTotalPrev[i] = m_intVSSidePadPitchGapFailTotal[i];
                }

                m_intVSSidePadBrokenFailTotal[i] = m_smVSInfo[i].g_intSidePadBrokenFailureTotal;
                if (m_intVSSidePadBrokenFailTotal[i] != m_intVSSidePadBrokenFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "SidePadBrokenFail", m_intVSSidePadBrokenFailTotal[i]);
                    m_intVSSidePadBrokenFailTotalPrev[i] = m_intVSSidePadBrokenFailTotal[i];
                }

                m_intVSSidePadExcessFailTotal[i] = m_smVSInfo[i].g_intSidePadExcessFailureTotal;
                if (m_intVSSidePadExcessFailTotal[i] != m_intVSSidePadExcessFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "SidePadExcessFail", m_intVSSidePadExcessFailTotal[i]);
                    m_intVSSidePadExcessFailTotalPrev[i] = m_intVSSidePadExcessFailTotal[i];
                }

                m_intVSSidePadSmearFailTotal[i] = m_smVSInfo[i].g_intSidePadSmearFailureTotal;
                if (m_intVSSidePadSmearFailTotal[i] != m_intVSSidePadSmearFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "SidePadSmearFail", m_intVSSidePadSmearFailTotal[i]);
                    m_intVSSidePadSmearFailTotalPrev[i] = m_intVSSidePadSmearFailTotal[i];
                }

                m_intVSSidePadEdgeLimitFailTotal[i] = m_smVSInfo[i].g_intSidePadEdgeLimitFailureTotal;
                if (m_intVSSidePadEdgeLimitFailTotal[i] != m_intVSSidePadEdgeLimitFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "SidePadEdgeLimitFail", m_intVSSidePadEdgeLimitFailTotal[i]);
                    m_intVSSidePadEdgeLimitFailTotalPrev[i] = m_intVSSidePadEdgeLimitFailTotal[i];
                }

                m_intVSSidePadStandOffFailTotal[i] = m_smVSInfo[i].g_intSidePadStandOffFailureTotal;
                if (m_intVSSidePadStandOffFailTotal[i] != m_intVSSidePadStandOffFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "SidePadStandOffFail", m_intVSSidePadStandOffFailTotal[i]);
                    m_intVSSidePadStandOffFailTotalPrev[i] = m_intVSSidePadStandOffFailTotal[i];
                }

                m_intVSSidePadEdgeDistanceFailTotal[i] = m_smVSInfo[i].g_intSidePadEdgeDistanceFailureTotal;
                if (m_intVSSidePadEdgeDistanceFailTotal[i] != m_intVSSidePadEdgeDistanceFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "SidePadEdgeDistanceFail", m_intVSSidePadEdgeDistanceFailTotal[i]);
                    m_intVSSidePadEdgeDistanceFailTotalPrev[i] = m_intVSSidePadEdgeDistanceFailTotal[i];
                }

                m_intVSSidePadSpanFailTotal[i] = m_smVSInfo[i].g_intSidePadSpanFailureTotal;
                if (m_intVSSidePadSpanFailTotal[i] != m_intVSSidePadSpanFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "SidePadSpanFail", m_intVSSidePadSpanFailTotal[i]);
                    m_intVSSidePadSpanFailTotalPrev[i] = m_intVSSidePadSpanFailTotal[i];
                }

                m_intVSSidePadContaminationFailTotal[i] = m_smVSInfo[i].g_intSidePadContaminationFailureTotal;
                if (m_intVSSidePadContaminationFailTotal[i] != m_intVSSidePadContaminationFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "SidePadContaminationFail", m_intVSSidePadContaminationFailTotal[i]);
                    m_intVSSidePadContaminationFailTotalPrev[i] = m_intVSSidePadContaminationFailTotal[i];
                }

                m_intVSSidePadMissingFailTotal[i] = m_smVSInfo[i].g_intSidePadMissingFailureTotal;
                if (m_intVSSidePadMissingFailTotal[i] != m_intVSSidePadMissingFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "SidePadMissingFail", m_intVSSidePadMissingFailTotal[i]);
                    m_intVSSidePadMissingFailTotalPrev[i] = m_intVSSidePadMissingFailTotal[i];
                }

                m_intVSSidePadColorDefectFailTotal[i] = m_smVSInfo[i].g_intSidePadColorDefectFailureTotal;
                if (m_intVSSidePadColorDefectFailTotal[i] != m_intVSSidePadColorDefectFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "SidePadColorDefectFail", m_intVSSidePadColorDefectFailTotal[i]);
                    m_intVSSidePadColorDefectFailTotalPrev[i] = m_intVSSidePadColorDefectFailTotal[i];
                }

                m_intVSCenterPkgDefectFailTotal[i] = m_smVSInfo[i].g_intCenterPkgDefectFailureTotal;
                if (m_intVSCenterPkgDefectFailTotal[i] != m_intVSCenterPkgDefectFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "CenterPkgDefectFail", m_intVSCenterPkgDefectFailTotal[i]);
                    m_intVSCenterPkgDefectFailTotalPrev[i] = m_intVSCenterPkgDefectFailTotal[i];
                }

                m_intVSCenterPkgDimensionFailTotal[i] = m_smVSInfo[i].g_intCenterPkgDimensionFailureTotal;
                if (m_intVSCenterPkgDimensionFailTotal[i] != m_intVSCenterPkgDimensionFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "CenterPkgDimensionFail", m_intVSCenterPkgDimensionFailTotal[i]);
                    m_intVSCenterPkgDimensionFailTotalPrev[i] = m_intVSCenterPkgDimensionFailTotal[i];
                }

                m_intVSSidePkgDefectFailTotal[i] = m_smVSInfo[i].g_intSidePkgDefectFailureTotal;
                if (m_intVSSidePkgDefectFailTotal[i] != m_intVSSidePkgDefectFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "SidePkgDefectFail", m_intVSSidePkgDefectFailTotal[i]);
                    m_intVSSidePkgDefectFailTotalPrev[i] = m_intVSSidePkgDefectFailTotal[i];
                }

                m_intVSSidePkgDimensionFailTotal[i] = m_smVSInfo[i].g_intSidePkgDimensionFailureTotal;
                if (m_intVSSidePkgDimensionFailTotal[i] != m_intVSSidePkgDimensionFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "SidePkgDimensionFail", m_intVSSidePkgDimensionFailTotal[i]);
                    m_intVSSidePkgDimensionFailTotalPrev[i] = m_intVSSidePkgDimensionFailTotal[i];
                }

                m_intVSLeadOffsetFailTotal[i] = m_smVSInfo[i].g_intLeadOffsetFailureTotal;
                if (m_intVSLeadOffsetFailTotal[i] != m_intVSLeadOffsetFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "LeadOffsetFail", m_intVSLeadOffsetFailTotal[i]);
                    m_intVSLeadOffsetFailTotalPrev[i] = m_intVSLeadOffsetFailTotal[i];
                }

                m_intVSLeadTipOffsetFailTotal[i] = m_smVSInfo[i].g_intLeadSkewFailureTotal;
                if (m_intVSLeadTipOffsetFailTotal[i] != m_intVSLeadTipOffsetFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "LeadSkewFail", m_intVSLeadTipOffsetFailTotal[i]);
                    m_intVSLeadTipOffsetFailTotalPrev[i] = m_intVSLeadTipOffsetFailTotal[i];
                }

                m_intVSLeadWidthFailTotal[i] = m_smVSInfo[i].g_intLeadWidthFailureTotal;
                if (m_intVSLeadWidthFailTotal[i] != m_intVSLeadWidthFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "LeadWidthFail", m_intVSLeadWidthFailTotal[i]);
                    m_intVSLeadWidthFailTotalPrev[i] = m_intVSLeadWidthFailTotal[i];
                }

                m_intVSLeadLengthFailTotal[i] = m_smVSInfo[i].g_intLeadLengthFailureTotal;
                if (m_intVSLeadLengthFailTotal[i] != m_intVSLeadLengthFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "LeadLengthFail", m_intVSLeadLengthFailTotal[i]);
                    m_intVSLeadLengthFailTotalPrev[i] = m_intVSLeadLengthFailTotal[i];
                }

                m_intVSLeadLengthVarianceFailTotal[i] = m_smVSInfo[i].g_intLeadLengthVarianceFailureTotal;
                if (m_intVSLeadLengthVarianceFailTotal[i] != m_intVSLeadLengthVarianceFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "LeadLengthVarianceFail", m_intVSLeadLengthVarianceFailTotal[i]);
                    m_intVSLeadLengthVarianceFailTotalPrev[i] = m_intVSLeadLengthVarianceFailTotal[i];
                }

                m_intVSLeadPitchGapFailTotal[i] = m_smVSInfo[i].g_intLeadPitchGapFailureTotal;
                if (m_intVSLeadPitchGapFailTotal[i] != m_intVSLeadPitchGapFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "LeadPitchGapFail", m_intVSLeadPitchGapFailTotal[i]);
                    m_intVSLeadPitchGapFailTotalPrev[i] = m_intVSLeadPitchGapFailTotal[i];
                }

                m_intVSLeadPitchVarianceFailTotal[i] = m_smVSInfo[i].g_intLeadPitchVarianceFailureTotal;
                if (m_intVSLeadPitchVarianceFailTotal[i] != m_intVSLeadPitchVarianceFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "LeadPitchVarianceFail", m_intVSLeadPitchVarianceFailTotal[i]);
                    m_intVSLeadPitchVarianceFailTotalPrev[i] = m_intVSLeadPitchVarianceFailTotal[i];
                }

                m_intVSLeadStandOffFailTotal[i] = m_smVSInfo[i].g_intLeadStandOffFailureTotal;
                if (m_intVSLeadStandOffFailTotal[i] != m_intVSLeadStandOffFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "LeadStandOffFail", m_intVSLeadStandOffFailTotal[i]);
                    m_intVSLeadStandOffFailTotalPrev[i] = m_intVSLeadStandOffFailTotal[i];
                }

                m_intVSLeadStandOffVarianceFailTotal[i] = m_smVSInfo[i].g_intLeadStandOffVarianceFailureTotal;
                if (m_intVSLeadStandOffVarianceFailTotal[i] != m_intVSLeadStandOffVarianceFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "LeadStandOffVarianceFail", m_intVSLeadStandOffVarianceFailTotal[i]);
                    m_intVSLeadStandOffVarianceFailTotalPrev[i] = m_intVSLeadStandOffVarianceFailTotal[i];
                }

                m_intVSLeadCoplanFailTotal[i] = m_smVSInfo[i].g_intLeadCoplanFailureTotal;
                if (m_intVSLeadCoplanFailTotal[i] != m_intVSLeadCoplanFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "LeadCoplanFail", m_intVSLeadCoplanFailTotal[i]);
                    m_intVSLeadCoplanFailTotalPrev[i] = m_intVSLeadCoplanFailTotal[i];
                }

                m_intVSLeadAGVFailTotal[i] = m_smVSInfo[i].g_intLeadAGVFailureTotal;
                if (m_intVSLeadAGVFailTotal[i] != m_intVSLeadAGVFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "LeadAGVFail", m_intVSLeadAGVFailTotal[i]);
                    m_intVSLeadAGVFailTotalPrev[i] = m_intVSLeadAGVFailTotal[i];
                }

                m_intVSLeadSpanFailTotal[i] = m_smVSInfo[i].g_intLeadSpanFailureTotal;
                if (m_intVSLeadSpanFailTotal[i] != m_intVSLeadSpanFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "LeadSpanFail", m_intVSLeadSpanFailTotal[i]);
                    m_intVSLeadSpanFailTotalPrev[i] = m_intVSLeadSpanFailTotal[i];
                }

                m_intVSLeadSweepsFailTotal[i] = m_smVSInfo[i].g_intLeadSweepsFailureTotal;
                if (m_intVSLeadSweepsFailTotal[i] != m_intVSLeadSweepsFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "LeadSweepsFail", m_intVSLeadSweepsFailTotal[i]);
                    m_intVSLeadSweepsFailTotalPrev[i] = m_intVSLeadSweepsFailTotal[i];
                }

                m_intVSLeadUnCutTiebarFailTotal[i] = m_smVSInfo[i].g_intLeadUnCutTiebarFailureTotal;
                if (m_intVSLeadUnCutTiebarFailTotal[i] != m_intVSLeadUnCutTiebarFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "LeadUnCutTiebarFail", m_intVSLeadUnCutTiebarFailTotal[i]);
                    m_intVSLeadUnCutTiebarFailTotalPrev[i] = m_intVSLeadUnCutTiebarFailTotal[i];
                }

                m_intVSLeadMissingFailTotal[i] = m_smVSInfo[i].g_intLeadMissingFailureTotal;
                if (m_intVSLeadMissingFailTotal[i] != m_intVSLeadMissingFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "LeadMissingFail", m_intVSLeadMissingFailTotal[i]);
                    m_intVSLeadMissingFailTotalPrev[i] = m_intVSLeadMissingFailTotal[i];
                }

                m_intVSLeadContaminationFailTotal[i] = m_smVSInfo[i].g_intLeadContaminationFailureTotal;
                if (m_intVSLeadContaminationFailTotal[i] != m_intVSLeadContaminationFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "LeadContaminationFail", m_intVSLeadContaminationFailTotal[i]);
                    m_intVSLeadContaminationFailTotalPrev[i] = m_intVSLeadContaminationFailTotal[i];
                }

                m_intVSLeadPkgDefectFailTotal[i] = m_smVSInfo[i].g_intLeadPkgDefectFailureTotal;
                if (m_intVSLeadPkgDefectFailTotal[i] != m_intVSLeadPkgDefectFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "LeadPkgDefectFail", m_intVSLeadPkgDefectFailTotal[i]);
                    m_intVSLeadPkgDefectFailTotalPrev[i] = m_intVSLeadPkgDefectFailTotal[i];
                }

                m_intVSLeadPkgDimensionFailTotal[i] = m_smVSInfo[i].g_intLeadPkgDimensionFailureTotal;
                if (m_intVSLeadPkgDimensionFailTotal[i] != m_intVSLeadPkgDimensionFailTotalPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "LeadPkgDimensionFail", m_intVSLeadPkgDimensionFailTotal[i]);
                    m_intVSLeadPkgDimensionFailTotalPrev[i] = m_intVSLeadPkgDimensionFailTotal[i];
                }

                m_blnVSPocket1Pass[i] = m_smVSInfo[i].g_blnPocket1Pass;
                if (m_blnVSPocket1Pass[i] != m_blnVSPocket1PassPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "Pocket1", m_blnVSPocket1Pass[i]);
                    m_blnVSPocket1PassPrev[i] = m_blnVSPocket1Pass[i];
                }

                m_blnVSPocket2Pass[i] = m_smVSInfo[i].g_blnPocket2Pass;
                if (m_blnVSPocket2Pass[i] != m_blnVSPocket2PassPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "Pocket2", m_blnVSPocket2Pass[i]);
                    m_blnVSPocket2PassPrev[i] = m_blnVSPocket2Pass[i];
                }

                m_intVSScenario[i] = m_smVSInfo[i].g_intScenario;
                if (m_intVSScenario[i] != m_intVSScenarioPrev[i])
                {
                    subKey1.SetValue("VS" + (i + 1) + "Scenario", m_intVSScenario[i]);
                    m_intVSScenarioPrev[i] = m_intVSScenario[i];
                }
            }

            #endregion
        }

    }
}
