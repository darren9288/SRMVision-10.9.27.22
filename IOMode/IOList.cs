using System;

namespace IOMode
{
    public class VisionIO
    {
        public InputIO MarkOrientStartVision;
        public OutputIO MarkOrientPass;
        public OutputIO MarkOrientResult1;
        public OutputIO MarkOrientResult2;
        public OutputIO MarkOrientGrabDone;
        public OutputIO MarkOrientEndVision;

        public VisionIO()
        {
            MarkOrientStartVision = new InputIO("Mark/Orient Vision Start Vision");
            MarkOrientPass = new OutputIO("Mark Orient Vision Pass");
            MarkOrientResult1 = new OutputIO("Mark Orient Vision Result 1");
            MarkOrientResult2 = new OutputIO("Mark Orient Vision Result 2");
            MarkOrientGrabDone = new OutputIO("Mark Orient Vision Grab Done");
            MarkOrientEndVision = new OutputIO("Mark Orient Vision End of Vision");
        }
    }
}
