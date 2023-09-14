using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Windows.Forms;
using Common;
using SharedMemory;

namespace IOMode
{
    public class VisionIO
    {
        // General
        public InputIO AutoShutDown;

        // Same IO function among visions
        public InputIO IOStartVision;
        public OutputIO IOEndVision;
        public OutputIO IOGrabbing;

        // Rotary Machine
        public InputIO InPocketCheckUnit1;
        public InputIO InPocketReTest;
        public InputIO InPocketReCheckUnit2;
        public InputIO EndOfReTest;
        public InputIO CheckEmpty;
        public InputIO RotatorSignal1;
        public InputIO RotatorSignal2;
        public OutputIO IOPass1;
        public OutputIO IOPass2;
        public OutputIO OrientResult2;
        public OutputIO OrientResult1;
        public OutputIO UnitPresent;
        public OutputIO PackageFail;        // Fail criteria: Package
        public OutputIO EmptyUnit;          // Fail criteria: Empty Unit           
        public OutputIO WrongOrientation;   // Fail criteria: Wrong Orientation
        public OutputIO PositionReject;     // Fail criteria: Position Reject
        public OutputIO MarkFail;           // Fail criteria: Mark
        public OutputIO FailLead;           // Fail criteria in Mark Orient : Trigger this IO when fail criteria is not mark
        public OutputIO FailNoMark;         // Fail criteria: no Mark

        // Zyro Machine IO
        public OutputIO ResultBit1;
        public OutputIO ResultBit2;
        public OutputIO ResultBit3;
        public OutputIO ResultBit4;
        public OutputIO ResultBit5;
        public OutputIO ResultBit6;

        // Grab Image IO
        public OutputIO Grab1;
        public OutputIO Grab2;
        public OutputIO Grab3;

        // Specific for Seal
        public InputIO CheckPresentQA;
        public InputIO CheckPresent;
        public InputIO Retest;


        //for local use in assign io
        private DataSet ioDataSet = new DataSet();
        private DataSet ioMapDataSet = new DataSet();
        private string message = "";
        private DataRow[] ioList;

        //New Added 

        // Reserved
        public OutputIO LaserDone;
        public OutputIO PCBFail;
        public OutputIO NewFile;
        public OutputIO UnknownOrientation;

        //MarkPkg
        public InputIO Data1;
        public InputIO Data2;
        public InputIO Data4;

        //InPocketPkg
        public OutputIO FailOffset;
        public OutputIO OffsetDone;
        public InputIO RollbackRetest;

        //PadPkg
        public InputIO CheckPH;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="strVisionModule"></param>
        /// <param name="strVisionName"></param>
        /// <param name="intVisionIndex"></param>
        /// <param name="intVisionSameCount"></param>
        /// <param name="strVisionNameRemark"></param>
        /// <param name="intEnableGrabIOMask">0x01=Enable Grab1, 0x02=Enable Grab2, 0x04=Enable Grab3</param>
        public VisionIO(string strVisionModule, string strVisionName, int intVisionIndex, int intVisionSameCount, string strVisionNameRemark, int intEnableGrabIOMask)
        {
            if (strVisionName != "")
                strVisionName += " ";

            if (strVisionNameRemark != "")
                strVisionNameRemark += " ";

            switch (strVisionModule)
            {
                case "Barcode":
                    MarkFail = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Fail");
                    IOPass1 = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Pass");
                    IOEndVision = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Ready");

                    break;
                case "Orient":
                case "BottomOrient":
                case "BottomPosition":
                case "MarkOrient":
                case "MOLi":
                    FailLead = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Fail Lead");
                    IOStartVision = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Start of Vision");
                    IOPass1 = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Pass");
                    IOGrabbing = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Grabbing");
                    IOEndVision = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "End of Vision");

                    OrientResult1 = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Result 1");
                    OrientResult2 = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Result 2");

                    Data1 = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Data 1");
                    Data2 = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Data 2");
                    Data4 = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Data 4");

                    FailNoMark = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Fail No Mark");
                    break;

                case "Mark":
                    IOStartVision = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Start of Vision");
                    IOPass1 = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Pass");
                    IOGrabbing = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Grabbing");
                    IOEndVision = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "End of Vision");

                    FailNoMark = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Fail No Mark");
                    break;
                case "BottomOrientPad":
                case "BottomOPadPkg":
                case "Pad":
                case "PadPos":
                case "Pad5S":
                case "Pad5SPos":
                case "Li3D":
                    if (strVisionModule == "BottomOrientPad" || strVisionModule == "BottomOPadPkg")
                    {
                        OrientResult1 = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Result 1");
                        OrientResult2 = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Result 2");
                    }
                    IOStartVision = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Start of Vision");
                    IOPass1 = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Pass");
                    IOGrabbing = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Grabbing");
                    IOEndVision = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "End of Vision");

                    EmptyUnit = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Empty Unit");
                    PositionReject = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Position Reject");
                    CheckPH = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Check PH");
                    break;
                case "BottomPositionOrient":
                    IOStartVision = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Start of Vision");
                    IOEndVision = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "End of Vision");
                    IOGrabbing = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Grabbing");
                    IOPass1 = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Pass");
                    if (strVisionNameRemark == "After Sing ")
                        UnitPresent = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Unit Present");

                    OrientResult1 = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Result 1");
                    OrientResult2 = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Result 2");
                    break;

                case "TapePocketPosition":
                    IOStartVision = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Start of Vision");
                    IOEndVision = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "End of Vision");
                    IOGrabbing = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Grabbing");
                    break;

                case "MarkPkg":
                    IOStartVision = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Start of Vision");
                    IOPass1 = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Pass");
                    IOGrabbing = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Grabbing");
                    IOEndVision = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "End of Vision");
                    WrongOrientation = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Wrong Orientation");
                    PackageFail = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Package Fail");
                    OrientResult1 = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Result 1");
                    OrientResult2 = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Result 2");
                    Data1 = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Data 1");
                    Data2 = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Data 2");
                    Data4 = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Data 4");
                    FailNoMark = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Fail No Mark");
                    break;
                case "MOPkg":
                case "MOLiPkg":
                    FailLead = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Fail Lead");
                    IOStartVision = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Start of Vision");
                    IOPass1 = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Pass");
                    IOGrabbing = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Grabbing");
                    IOEndVision = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "End of Vision");

                    OrientResult1 = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Result 1");
                    OrientResult2 = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Result 2");
                    WrongOrientation = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Wrong Orientation");
                    PackageFail = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Package Fail");
                    MarkFail = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Mark Fail");
                    Data1 = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Data 1");
                    Data2 = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Data 2");
                    Data4 = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Data 4");
                    FailNoMark = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Fail No Mark");
                    UnknownOrientation = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Unknown Orientation");
                    NewFile = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "New File");
                    break;
                case "Package":
                    IOStartVision = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Start of Vision");
                    IOPass1 = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Pass");
                    IOGrabbing = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Grabbing");
                    IOEndVision = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "End of Vision");

                    WrongOrientation = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Wrong Orientation");
                    //PackageFail = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Package Fail");
                    break;
                case "Li3DPkg":
                case "PadPkg":
                case "PadPkgPos":
                case "Pad5SPkg":
                case "Pad5SPkgPos":
                    IOStartVision = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Start of Vision");
                    IOPass1 = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Pass");
                    IOGrabbing = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Grabbing");
                    IOEndVision = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "End of Vision");

                    EmptyUnit = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Empty Unit");
                    PositionReject = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Position Reject");
                    CheckPH = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Check PH");
                    break;
                case "InPocket":
                case "IPMLi":
                    IOStartVision = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Start of Vision");
                    InPocketReTest = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Re-Test");
                    EndOfReTest = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "End Of Re-Test");
                    InPocketReCheckUnit2 = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Re-Check Unit 2");
                    InPocketCheckUnit1 = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Check Unit 1");

                    ///    17-07-2019 ZJYEOH : Added checkEmpty and ROllbackRetest so that it will compatible with InPocket Vision without package inspection
                    CheckEmpty = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Check Empty");
                    RollbackRetest = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Rollback Retest");
                    ///

                    IOEndVision = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "End of Vision");
                    IOPass2 = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Pass 2");
                    IOPass1 = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Pass 1");
                    break;
                case "InPocketPkg":
                case "IPMLiPkg":
                    IOStartVision = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Start of Vision");
                    InPocketReTest = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Re-Test");
                    EndOfReTest = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "End Of Re-Test");
                    CheckEmpty = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Check Empty");
                    InPocketReCheckUnit2 = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Re-Check Unit 2");
                    InPocketCheckUnit1 = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Check Unit 1");
                    IOPass2 = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Pass 2");

                    IOEndVision = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "End of Vision");
                    IOPass1 = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Pass 1");
                    IOGrabbing = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Grabbing");
                    WrongOrientation = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Wrong Orientation");
                    PackageFail = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Package Fail");
                    RollbackRetest = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Rollback Retest");
                    FailOffset = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Fail Offset");
                    OffsetDone = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Offset Done");

                    //////IOStartVision = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Start of Vision");
                    //////IOEndVision = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "End of Vision");

                    //////InPocketReTest = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Re-Test");
                    ////////InPocketReCheckUnit2 = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Re-Check Unit 2");
                    ////////InPocketCheckUnit1 = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Check Unit 1");
                    //////InPocketCheckUnit1 = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "End Of Re-Test");

                    ////////IOPass2 = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Pass 2");
                    //////IOPass1 = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Pass 1");

                    //////CheckEmpty = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Check Empty");

                    ////////RotatorSignal1 = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Rotator Signal 1");   // Use in carsem customer
                    ////////RotatorSignal2 = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Rotator Signal 2");   // Use in carsem customer

                    ////////UnitPresent = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Unit Present");
                    ////////PackageFail = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Package Fail");
                    break;
                case "InPocketPkgPos":  // For T&R with auto replace feature
                    IOStartVision = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Start of Vision");
                    InPocketReTest = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Re-Test");
                    EndOfReTest = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "End Of Re-Test");
                    CheckEmpty = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Check Empty");

                    IOEndVision = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "End of Vision");
                    IOPass1 = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Pass 1");
                    IOGrabbing = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Grabbing");
                    WrongOrientation = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Wrong Orientation");
                    PackageFail = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Package Fail");
                    break;
                //Post Seal
                case "Seal":
                    IOStartVision = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Start of Vision");
                    IOPass1 = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Pass");
                    IOEndVision = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "End of Vision");

                    CheckPresentQA = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Check Present QA Mode");
                    CheckPresent = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Check Present");
                    Retest = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Retest");
                    break;
                case "UnitPresent":
                    IOStartVision = new InputIO(strVisionName + "Vision " + strVisionNameRemark + "Start of Vision");

                    IOEndVision = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "End of Vision");
                    IOGrabbing = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Grabbing");
                    ResultBit1 = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Result Bit 1");
                    ResultBit2 = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Result Bit 2");
                    ResultBit3 = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Result Bit 3");
                    ResultBit4 = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Result Bit 4");
                    ResultBit5 = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Result Bit 5");
                    ResultBit6 = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Result Bit 6");
                    break;
                case "General":
                    // create object for general purpose
                    AutoShutDown = new InputIO("Auto Shut Down");
                    break;
                default:
                    SRMMessageBox.Show("VisionIO() --> There is no such vision module name " + strVisionModule + " in this SRMVision software version.");
                    break;
            }

            if ((intEnableGrabIOMask & 0x01) > 0)
                Grab1 = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Image Grab1");
            if ((intEnableGrabIOMask & 0x02) > 0)
                Grab2 = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Image Grab2");
            if ((intEnableGrabIOMask & 0x04) > 0)
                Grab3 = new OutputIO(strVisionName + "Vision " + strVisionNameRemark + "Image Grab3");


            GetVisionIOInfo();
        }

        public void GetVisionIOInfo()
        {
            //get io dataset
            GetDataSet();

            if (IOStartVision != null && InputExistInIOMap(IOStartVision, ioMapDataSet))
                AssignInput(IOStartVision, ioDataSet);
            if (InPocketReTest != null && InputExistInIOMap(InPocketReTest, ioMapDataSet))
                AssignInput(InPocketReTest, ioDataSet);
            if (EndOfReTest != null && InputExistInIOMap(EndOfReTest, ioMapDataSet))
                AssignInput(EndOfReTest, ioDataSet);
            if (InPocketCheckUnit1 != null && InputExistInIOMap(InPocketCheckUnit1, ioMapDataSet))
                AssignInput(InPocketCheckUnit1, ioDataSet);
            if (InPocketReCheckUnit2 != null && InputExistInIOMap(InPocketReCheckUnit2, ioMapDataSet))
                AssignInput(InPocketReCheckUnit2, ioDataSet);
            if (CheckEmpty != null && InputExistInIOMap(CheckEmpty, ioMapDataSet))
                AssignInput(CheckEmpty, ioDataSet);
            if (RotatorSignal1 != null && InputExistInIOMap(RotatorSignal1, ioMapDataSet))
                AssignInput(RotatorSignal1, ioDataSet);
            if (RotatorSignal2 != null && InputExistInIOMap(RotatorSignal2, ioMapDataSet))
                AssignInput(RotatorSignal2, ioDataSet);
            if (RollbackRetest != null && InputExistInIOMap(RollbackRetest, ioMapDataSet))
                AssignInput(RollbackRetest, ioDataSet);
            if (Data1 != null && InputExistInIOMap(Data1, ioMapDataSet))
                AssignInput(Data1, ioDataSet);
            if (Data2 != null && InputExistInIOMap(Data2, ioMapDataSet))
                AssignInput(Data2, ioDataSet);
            if (Data4 != null && InputExistInIOMap(Data4, ioMapDataSet))
                AssignInput(Data4, ioDataSet);

            if (IOEndVision != null && OutputExistInIOMap(IOEndVision, ioMapDataSet))
                AssignOutput(IOEndVision, ioDataSet);
            if (IOGrabbing != null && OutputExistInIOMap(IOGrabbing, ioMapDataSet))
                AssignOutput(IOGrabbing, ioDataSet);
            if (PackageFail != null && OutputExistInIOMap(PackageFail, ioMapDataSet))
                AssignOutput(PackageFail, ioDataSet);
            if (MarkFail != null && OutputExistInIOMap(MarkFail, ioMapDataSet))
                AssignOutput(MarkFail, ioDataSet);
            if (OrientResult1 != null && OutputExistInIOMap(OrientResult1, ioMapDataSet))
                AssignOutput(OrientResult1, ioDataSet);
            if (OrientResult2 != null && OutputExistInIOMap(OrientResult2, ioMapDataSet))
                AssignOutput(OrientResult2, ioDataSet);
            if (IOPass1 != null && OutputExistInIOMap(IOPass1, ioMapDataSet))
                AssignOutput(IOPass1, ioDataSet);
            if (IOPass2 != null && OutputExistInIOMap(IOPass2, ioMapDataSet))
                AssignOutput(IOPass2, ioDataSet);
            if (UnitPresent != null && OutputExistInIOMap(UnitPresent, ioMapDataSet))
                AssignOutput(UnitPresent, ioDataSet);
            if (WrongOrientation != null && OutputExistInIOMap(WrongOrientation, ioMapDataSet))
                AssignOutput(WrongOrientation, ioDataSet);
            if (EmptyUnit != null && OutputExistInIOMap(EmptyUnit, ioMapDataSet))
                AssignOutput(EmptyUnit, ioDataSet);
            if (PositionReject != null && OutputExistInIOMap(PositionReject, ioMapDataSet))
                AssignOutput(PositionReject, ioDataSet);
            if (FailOffset != null && OutputExistInIOMap(FailOffset, ioMapDataSet))
                AssignOutput(FailOffset, ioDataSet);
            if (OffsetDone != null && OutputExistInIOMap(OffsetDone, ioMapDataSet))
                AssignOutput(OffsetDone, ioDataSet);
            if (FailLead != null && OutputExistInIOMap(FailLead, ioMapDataSet))
                AssignOutput(FailLead, ioDataSet);
            if (FailNoMark != null && OutputExistInIOMap(FailNoMark, ioMapDataSet))
                AssignOutput(FailNoMark, ioDataSet);
            if (UnknownOrientation != null && OutputExistInIOMap(UnknownOrientation, ioMapDataSet))
                AssignOutput(UnknownOrientation, ioDataSet);
            if (NewFile != null && OutputExistInIOMap(NewFile, ioMapDataSet))
                AssignOutput(NewFile, ioDataSet);


            if (AutoShutDown != null && InputExistInIOMap(AutoShutDown, ioMapDataSet))
                AssignInput(AutoShutDown, ioDataSet);

            if (CheckPresentQA != null && InputExistInIOMap(CheckPresentQA, ioMapDataSet))
                AssignInput(CheckPresentQA, ioDataSet);
            if (CheckPresent != null && InputExistInIOMap(CheckPresent, ioMapDataSet))
                AssignInput(CheckPresent, ioDataSet);
            if (Retest != null && InputExistInIOMap(Retest, ioMapDataSet))
                AssignInput(Retest, ioDataSet);

            if (ResultBit1 != null && OutputExistInIOMap(ResultBit1, ioMapDataSet))
                AssignOutput(ResultBit1, ioDataSet);
            if (ResultBit2 != null && OutputExistInIOMap(ResultBit2, ioMapDataSet))
                AssignOutput(ResultBit2, ioDataSet);
            if (ResultBit3 != null && OutputExistInIOMap(ResultBit3, ioMapDataSet))
                AssignOutput(ResultBit3, ioDataSet);
            if (ResultBit4 != null && OutputExistInIOMap(ResultBit4, ioMapDataSet))
                AssignOutput(ResultBit4, ioDataSet);
            if (ResultBit5 != null && OutputExistInIOMap(ResultBit5, ioMapDataSet))
                AssignOutput(ResultBit5, ioDataSet);
            if (ResultBit6 != null && OutputExistInIOMap(ResultBit6, ioMapDataSet))
                AssignOutput(ResultBit6, ioDataSet);

            if (Grab1 != null && OutputExistInIOMap(Grab1, ioMapDataSet))
                AssignOutput(Grab1, ioDataSet);
            if (Grab2 != null && OutputExistInIOMap(Grab2, ioMapDataSet))
                AssignOutput(Grab2, ioDataSet);
            if (Grab3 != null && OutputExistInIOMap(Grab3, ioMapDataSet))
                AssignOutput(Grab3, ioDataSet);

            if (CheckPH != null && InputExistInIOMap(CheckPH, ioMapDataSet))
                AssignInput(CheckPH, ioDataSet);


            if (message != "")
            {
                message = "The following IO(s) NOT assigned:\n" + message;
                SRMMessageBox.Show(message, "Get Vision IO", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }

        public void GetDataSet()
        {
            DBCall dbCall = new DBCall("\\access\\simeca.mdb");
            //set select criteria
            string sqlSelect = "SELECT IOMap.*, IO.* FROM IOMap, IO WHERE IOMap.Name = IO.Description";

            string sqlMapSelect = "SELECT IOMap.* FROM IOMap";

            //fill dataset
            dbCall.Select(sqlSelect, ioDataSet);
            dbCall.Select(sqlMapSelect, ioMapDataSet);
        }

        public bool InputExistInIOMap(InputIO inputIO, DataSet ds)
        {
            ioList = ds.Tables[0].Select("Name = '" + inputIO.Name + "'");

            if (ioList.Length == 0)
            {
                STTrackLog.WriteLine(inputIO.Name + " - No Exist in Input IO Map");
            }

            return (ioList.Length > 0);
        }

        public bool OutputExistInIOMap(OutputIO outputIO, DataSet ds)
        {
            ioList = ds.Tables[0].Select("Name = '" + outputIO.Name + "'");

            if (ioList.Length == 0)
            {
                STTrackLog.WriteLine(outputIO.Name + " - No Exist in Output IO Map");
            }

            return (ioList.Length > 0);
        }

        public void AssignOutput(OutputIO outputIO, DataSet ds)
        {
            ioList = ds.Tables[0].Select("Name = '" + outputIO.Name + "'");
            if (ioList.Length > 0)
            {
                outputIO.CardNo = Convert.ToInt32(ioList[0]["Card Number"]);
                outputIO.Channel = Convert.ToInt32(ioList[0]["Channel Number"]);
                outputIO.Bit = Convert.ToInt32(ioList[0]["Bit"]);
            }
            else
                message += "- " + outputIO.Name + "\n";

            STTrackLog.WriteLine("IO Output [" + outputIO.Name + "]=" + outputIO.CardNo.ToString() + ", " + outputIO.Channel.ToString() + ", " + outputIO.Bit.ToString());
        }

        public void AssignInput(InputIO inputIO, DataSet ds)
        {
            ioList = ds.Tables[0].Select("Name = '" + inputIO.Name + "'");
            if (ioList.Length > 0)
            {
                inputIO.CardNo = Convert.ToInt32(ioList[0]["Card Number"]);
                inputIO.Channel = Convert.ToInt32(ioList[0]["Channel Number"]);
                inputIO.Bit = Convert.ToInt32(ioList[0]["Bit"]);
            }
            else
                message += "- " + inputIO.Name + "\n";

            STTrackLog.WriteLine("IO Input [" + inputIO.Name + "]=" + inputIO.CardNo.ToString() + ", " + inputIO.Channel.ToString() + ", " + inputIO.Bit.ToString());
        }
    }
}
