using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
#if (Debug_2_12 || Release_2_12)
using Euresys.Open_eVision_2_12;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
using Euresys.Open_eVision_1_2;
#endif

namespace VisionProcessing
{
    public class ColorProcessing
    {
        #region Member Variables
        EColorLookup m_objColorLookup = new EColorLookup();
        //EROIC24
        
        #endregion

        #region Properties
        #endregion

        public ColorProcessing()
        {

        }



        /// <summary>
        /// Gain or offset color image
        /// </summary>
        /// <param name="objSourceCROI">source ROI</param>
        /// <param name="objDestinationCROI">destination ROI</param>
        /// <param name="fRedGain">red gain</param>
        /// <param name="fRedOffSet">red offset</param>
        /// <param name="fGreenGain">green gain</param>
        /// <param name="fGreenOffSet">green offset</param>
        /// <param name="fBlueGain">blue gain</param>
        /// <param name="fBlueOffSet">blue offset</param>
        public void AdjustImage(CROI objSourceCROI, CROI objDestinationCROI, float fRedGain, float fRedOffSet, float fGreenGain, float fGreenOffSet, float fBlueGain, float fBlueOffSet)
        {
            m_objColorLookup.AdjustGainOffset(EColorSystem.Rgb, fRedGain, fRedOffSet, fGreenGain, fGreenOffSet, fBlueGain, fBlueOffSet);
            m_objColorLookup.Transform(objSourceCROI.ref_CROI, objDestinationCROI.ref_CROI);
        }

        /// <summary>
        /// Color threshold - change color image to gray scale image 
        /// </summary>
        /// <param name="objSourceCROI">source ROI</param>
        /// <param name="objDestinationCROI">destination ROI</param>
        /// <param name="objMinColor">min color</param>
        /// <param name="objMaxColor">max color</param>
        public void ThresholdImageToLSH(CROI objSourceCROI, ROI objDestinationCROI, Color24 objMinColor, Color24 objMaxColor)
        {
            EasyImage.Threshold(objSourceCROI.ref_CROI, objDestinationCROI.ref_ROI, objMinColor.ref_Color24, objMaxColor.ref_Color24, m_objColorLookup);
        }

        /// <summary>
        /// White balance color image. RGB based on source image.
        /// </summary>
        /// <param name="objSourceCROI">source ROI</param>
        /// <param name="objDestinationCROI">destination ROI</param>
        public void WhiteBalance(CROI objSourceCROI, CROI objDestinationCROI)
        {
            float fAvgRed, fAvgGreen, fAvgBlue;
            fAvgRed = fAvgGreen = fAvgBlue = 255;
            EasyImage.PixelAverage(objSourceCROI.ref_CROI, out fAvgRed, out fAvgGreen, out fAvgBlue);
            WhiteBalance(objSourceCROI, objDestinationCROI, 1f, 1f, fAvgRed, fAvgGreen, fAvgBlue);
        }

        /// <summary>
        /// White balance color image.
        /// </summary>
        /// <param name="objSourceCROI">source ROI</param>
        /// <param name="objDestinationCROI">destination ROI</param>
        /// <param name="fGain">gain</param>
        /// <param name="fGamma">gamma</param>
        /// <param name="fAvgRed">balance red</param>
        /// <param name="fAvgGreen">balance green</param>
        /// <param name="fAvgBlue">balance blue</param>
        public void WhiteBalance(CROI objSourceCROI, CROI objDestinationCROI, float fGain, float fGamma, float fAvgRed, float fAvgGreen, float fAvgBlue)
        {
            m_objColorLookup.WhiteBalance(fGain, fGamma, fAvgRed, fAvgGreen, fAvgBlue);
            m_objColorLookup.Transform(objSourceCROI.ref_CROI, objDestinationCROI.ref_CROI);
        }


        /// <summary>
        /// Convert rgb value to lsh value
        /// </summary>
        /// <param name="intR">red</param>
        /// <param name="intG">green</param>
        /// <param name="intB">blue</param>
        /// <returns>lsh value</returns>
        public static int[] ConvertRGBToLSH(int intR, int intG, int intB)
        {
            int[] intLSH = new int[3];

            EC24 objLSH = new EC24();
            EC24 objRGB = new EC24();
            objRGB.C0 = (byte)intR;
            objRGB.C1 = (byte)intG;
            objRGB.C2 = (byte)intB;
            EasyColor.RgbToLsh(objRGB, out objLSH);

            intLSH[0] = objLSH.C0;
            intLSH[1] = objLSH.C1;
            intLSH[2] = objLSH.C2;

            return intLSH;
        }

        /// <summary>
        /// Convert lsh value to rgb value
        /// </summary>
        /// <param name="intL">lightness</param>
        /// <param name="intS">saturation</param>
        /// <param name="intH">hue</param>
        /// <returns>rgb value</returns>
        public static int[] ConvertLSHToRGB(int intL, int intS, int intH)
        {
            int[] intRGB = new int[3];

            EC24 objLSH = new EC24();
            EC24 objRGB = new EC24();
            objLSH.C0 = (byte)intL;
            objLSH.C1 = (byte)intS;
            objLSH.C2 = (byte)intH;
            EasyColor.LshToRgb(objLSH, out objRGB);

            intRGB[0] = objRGB.C0;
            intRGB[1] = objRGB.C1;
            intRGB[2] = objRGB.C2;

            return intRGB;
        }

        /// <summary>
        /// Define RGB value based on different mode
        /// </summary>
        /// <param name="intPredefineMode">0 = None, 1 = Incandescent, 2 = Print, 3 = Photo, 4 = DayLight, 5 = Fluorescent</param>
        /// <returns>rgb value</returns>
        public static float[] PredefineGRB(int intPredefineMode)
        {
            float[] fRGB = new float[3];
            switch (intPredefineMode)
            {
                case 0: // None
                    fRGB[0] = 255;
                    fRGB[1] = 255;
                    fRGB[2] = 255;
                    break;
                case 1: // A (Incandescent)
                    fRGB[0] = 141.6f;
                    fRGB[1] = 90.7f;
                    fRGB[2] = 26.5f;
                    break;
                case 2: // D50 (Print)
                    fRGB[0] = 106.9f;
                    fRGB[1] = 102.7f;
                    fRGB[2] = 67.8f;
                    break;
                case 3: // D55 (Photo)
                    fRGB[0] = 102.7f;
                    fRGB[1] = 103.2f;
                    fRGB[2] = 76.4f;
                    break;
                case 4: // D65 (DayLight)
                    fRGB[0] = 96.7f;
                    fRGB[1] = 103.3f;
                    fRGB[2] = 91.4f;
                    break;
                case 5: // F (Fluorescent)
                    fRGB[0] = 105.4f;
                    fRGB[1] = 104.7f;
                    fRGB[2] = 61.6f;
                    break;
                default:
                    fRGB[0] = 255;
                    fRGB[1] = 255;
                    fRGB[2] = 255;
                    break;
            }

            return fRGB;
        }


        /// <summary>
        /// Search Minimum Color Threshold 
        /// </summary>
        /// <param name="intValue">Color Threshold value in array RGB or LSH</param>
        /// <param name="intTolerance">Color Threshold tolerance value in array RGB or LSH</param>
        /// <returns>Min Color Threshold value in RGB format or LSH format: if input is RGB format, output is RGB</returns>
        public static Color24 CalculateMinColor(int[] intValue, int[] intTolerance)
        {         
            int[] intColorThreshold = new int[3];
            intColorThreshold[0] = intValue[0] - intTolerance[0];   // Lightness
            if (intColorThreshold[0] < 0)
                intColorThreshold[0] = 0;

            intColorThreshold[1] = intValue[1] - intTolerance[1];   //Saturation
            if (intColorThreshold[1] < 0)
                intColorThreshold[1] = 0;

            intColorThreshold[2] = intValue[2] - intTolerance[2];   // Hue
            if (intColorThreshold[2] < 0)
                intColorThreshold[2] = 0;

            Color24 objColor = new Color24(intColorThreshold[0], intColorThreshold[1], intColorThreshold[2]);
            return objColor;
        }

        /// <summary>
        /// Search Maximum Color Threshold 
        /// </summary>
        /// <param name="intValue">Color Threshold value in array RGB or LSH</param>
        /// <param name="intTolerance">Color Threshold tolerance value in array RGB or LSH</param>
        /// <returns>Max Color Threshold value in RGB format or LSH format: if input is RGB format, output is RGB</returns>
        public static Color24 CalculateMaxColor(int[] intValue, int[] intTolerance)
        {
            int[] intColorThreshold = new int[3];
            intColorThreshold[0] = intValue[0] + intTolerance[0];   // Lightness
            if (intColorThreshold[0] > 255)
                intColorThreshold[0] = 255;

            intColorThreshold[1] = intValue[1] + intTolerance[1];   //Saturation
            if (intColorThreshold[1] > 255)
                intColorThreshold[1] = 255;

            intColorThreshold[2] = intValue[2] + intTolerance[2];    // Hue
            if (intColorThreshold[2] > 255)
                intColorThreshold[2] = 255;

            Color24 objColor = new Color24(intColorThreshold[0], intColorThreshold[1], intColorThreshold[2]);
            return objColor;
        }

        /// <summary>
        /// Draw Red color with length 255 pixels
        /// </summary>
        /// <param name="intHeight">height</param>
        /// <returns>destination bitmap image</returns>
        public static Image DrawRedColor(int intMinHue, int intMaxHue, int intHeight)
        {
            Bitmap bm = new Bitmap(256, intHeight);
            Graphics g = Graphics.FromImage(bm);
            Pen pen = new Pen(Color.Red);           

            // Draw Red to yellow
            for (int x = 0; x < 255;x++)
            {
                pen.Color = Color.FromArgb(x, 0, 0);
                g.DrawLine(pen, x, 0, x, intHeight);            
            }

            g.DrawLine(new Pen(Color.Black), intMinHue, 0, intMinHue, intHeight);
            g.DrawLine(new Pen(Color.Black), intMaxHue, 0, intMaxHue, intHeight);
          
            return bm;
        }

        /// <summary>
        /// Draw green color with length 255 pixels
        /// </summary>
        /// <param name="intHeight">height</param>
        /// <returns>destination bitmap image</returns>
        public static Image DrawGreenColor(int intMinHue, int intMaxHue, int intHeight)
        {
            Bitmap bm = new Bitmap(256, intHeight);
            Graphics g = Graphics.FromImage(bm);
            Pen pen = new Pen(Color.Blue);

            // Draw Red to yellow
            for (int x = 0; x < 255; x++)
            {
                pen.Color = Color.FromArgb(0, x, 0);
                g.DrawLine(pen, x, 0, x, intHeight);
            }

            g.DrawLine(new Pen(Color.Black), intMinHue, 0, intMinHue, intHeight);
            g.DrawLine(new Pen(Color.Black), intMaxHue, 0, intMaxHue, intHeight);

            return bm;
        }

        /// <summary>
        /// Draw blue color with length 255 pixels
        /// </summary>
        /// <param name="intHeight">height</param>
        /// <returns>destination bitmap image</returns>
        public static Image DrawBlueColor(int intMinHue, int intMaxHue, int intHeight)
        {
            Bitmap bm = new Bitmap(256, intHeight);
            Graphics g = Graphics.FromImage(bm);
            Pen pen = new Pen(Color.Green);

            // Draw Red to yellow
            for (int x = 0; x < 255; x++)
            {
                pen.Color = Color.FromArgb(0, 0, x);
                g.DrawLine(pen, x, 0, x, intHeight);
            }

            g.DrawLine(new Pen(Color.Black), intMinHue, 0, intMinHue, intHeight);
            g.DrawLine(new Pen(Color.Black), intMaxHue, 0, intMaxHue, intHeight);

            return bm;
        }
   
        /// <summary>
        /// Draw hue color with length 255 pixels
        /// </summary>
        /// <param name="intHeight">height</param>
        /// <returns>destination bitmap image</returns>
        public static Image DrawHueColor(int intHeight)
        {
            Bitmap bm = new Bitmap(256, intHeight);
            Graphics g = Graphics.FromImage(bm);
            Pen pen = new Pen(Color.Red);
            int R, G, B;
            int intCount = 0;

            // Draw Red to yellow
            for (G = 0; G < 255; G += 6)
            {
                if (G == 84 || G == 167 || G == 250)
                    G--;
                pen.Color = Color.FromArgb(255, G, 0);
                g.DrawLine(pen, intCount, 0, intCount, intHeight);
                intCount++;
            }
            // Draw yellow to green
            for (R = 255; R > 0; R -= 6)
            {
                pen.Color = Color.FromArgb(R, 255, 0);
                g.DrawLine(pen, intCount, 0, intCount, intHeight);
                intCount++;

                if (R == 255 || R == 170 || R == 85)
                    R--;
            }
            // Draw green to light blue
            for (B = 0; B < 255; B += 6)
            {
                if (B == 84 || B == 167 || B == 250)
                    B++;
                pen.Color = Color.FromArgb(0, 255, B);
                g.DrawLine(pen, intCount, 0, intCount, intHeight);
                intCount++;
            }
            // Draw light blue to blue
            for (G = 255; G > 0; G -= 6)
            {
                pen.Color = Color.FromArgb(0, G, 255);
                g.DrawLine(pen, intCount, 0, intCount, intHeight);
                intCount++;

                if (G == 255 || G == 170 || G == 85)
                    G--;
            }
            // Draw blue to pink
            for (R = 0; R < 255; R += 6)
            {
                if (R == 84 || R == 167 || R == 250)
                    R++;
                pen.Color = Color.FromArgb(R, 0, 255);
                g.DrawLine(pen, intCount, 0, intCount, intHeight);
                intCount++;
            }
            // Draw pink to red
            for (B = 255; B > 0; B -= 6)
            {
                pen.Color = Color.FromArgb(255, 0, B);
                g.DrawLine(pen, intCount, 0, intCount, intHeight);
                intCount++;

                if (B == 255 || B == 170 || B == 85)
                    B--;
            }

            return bm;
        }

        /// <summary>
        /// Draw hue color with length 255 pixels + 2 lines for min and max tolerance
        /// </summary>
        /// <param name="intMinHue">min hue</param>
        /// <param name="intMaxHue">max hue</param>
        /// <param name="intHeight">height</param>
        /// <returns>destination bitmap image</returns>
        public static Image DrawHueColor(int intMinHue, int intMaxHue, int intHeight)
        {
            Bitmap bm = (Bitmap)DrawHueColor(intHeight);
            Graphics g = Graphics.FromImage(bm);

            g.DrawLine(new Pen(Color.Black), intMinHue, 0, intMinHue, intHeight);
            g.DrawLine(new Pen(Color.Black), intMaxHue, 0, intMaxHue, intHeight);

            return bm;
        }

        /// <summary>
        /// Draw lightness color with length 255 pixels
        /// </summary>
        /// <param name="intHue">hue</param>
        /// <param name="intSaturation">saturation</param>
        /// <param name="intHeight">height</param>
        /// <returns>destination bitmap image</returns>
        public static Image DrawLightnessColor(int intHue, int intSaturation, int intHeight)
        {
            Bitmap bm = new Bitmap(256, intHeight);
            Graphics g = Graphics.FromImage(bm);
            Pen pen = new Pen(Color.Red);
            EC24 objLSH = new EC24();
            EC24 objRGB = new EC24();

            for (int x = 0; x < 256; x++)
            {
                objLSH.C0 = (byte)x;
                objLSH.C1 = (byte)intSaturation;
                objLSH.C2 = (byte)intHue;
                EasyColor.LshToRgb(objLSH, out objRGB);

                pen.Color = Color.FromArgb(objRGB.C0, objRGB.C1, objRGB.C2);
                g.DrawLine(pen, x, 0, x, intHeight);
            }
            return bm;
        }

        /// <summary>
        /// Draw lightness color with length 255 pixels + 2 lines for min and max tolerance
        /// </summary>
        /// <param name="intMinLightness">min lightness</param>
        /// <param name="intMaxLightness">max lightness</param>
        /// <param name="intHue">hue</param>
        /// <param name="intSaturation">saturation</param>  
        /// <param name="intHeight">height</param>
        /// <returns>destination bitmap image</returns>
        public static Image DrawLightnessColor(int intMinLightness, int intMaxLightness, int intHue, int intSaturation, int intHeight)
        {
            Bitmap bm = (Bitmap)DrawLightnessColor(intHue, intSaturation, intHeight);
            Graphics g = Graphics.FromImage(bm);

            EC24 objLSH = new EC24();
            EC24 objRGB = new EC24();
            objLSH.C0 = 127;
            objLSH.C1 = (byte)intSaturation;
            objLSH.C2 = (byte)intHue;
            EasyColor.LshToRgb(objLSH, out objRGB);
            objRGB.C0 = (byte)(~objRGB.C0 & 0xFF);
            objRGB.C1 = (byte)(~objRGB.C1 & 0xFF);
            objRGB.C2 = (byte)(~objRGB.C2 & 0xFF);

            Pen pen = new Pen(Color.FromArgb(objRGB.C0, objRGB.C1, objRGB.C2));
            g.DrawLine(pen, intMinLightness, 0, intMinLightness, intHeight);
            g.DrawLine(pen, intMaxLightness, 0, intMaxLightness, intHeight);

            return bm;
        }

        /// <summary>
        /// Draw saturation color with length 255 pixels
        /// </summary>
        /// <param name="intHue">hue</param>
        /// <param name="intHeight">height</param>
        /// <returns>destination bitmap image</returns>
        public static Image DrawSaturationColor(int intHue, int intHeight)
        {
            Bitmap bm = new Bitmap(256, intHeight);
            Graphics g = Graphics.FromImage(bm);
            Pen pen = new Pen(Color.Red);
            EC24 objLSH = new EC24();
            EC24 objRGB = new EC24();

            for (int x = 0; x < 256; x++)
            {
                objLSH.C0 = (byte)127;
                objLSH.C1 = (byte)x;
                objLSH.C2 = (byte)intHue;
                EasyColor.LshToRgb(objLSH, out objRGB);

                pen.Color = Color.FromArgb(objRGB.C0, objRGB.C1, objRGB.C2);
                g.DrawLine(pen, x, 0, x, intHeight);
            }
            return bm;
        }

        /// <summary>
        /// Draw saturation color with length 255 pixels + 2 lines for min and max tolerance
        /// </summary>
        /// <param name="intMinSaturation">min saturation</param>
        /// <param name="intMaxSaturation">max saturation</param>
        /// <param name="intHue">hue</param>
        /// <param name="intHeight">height</param>
        /// <returns>destination bitmap image</returns>
        public static Image DrawSaturationColor(int intMinSaturation, int intMaxSaturation, int intHue, int intHeight)
        {
            Bitmap bm = (Bitmap)DrawSaturationColor(intHue, intHeight);
            Graphics g = Graphics.FromImage(bm);

            EC24 objLSH = new EC24();
            EC24 objRGB = new EC24();
            objLSH.C0 = (byte)127;
            objLSH.C1 = (byte)255;
            objLSH.C2 = (byte)intHue;
            EasyColor.LshToRgb(objLSH, out objRGB);
            objRGB.C0 = (byte)(~objRGB.C0 & 0xFF);
            objRGB.C1 = (byte)(~objRGB.C1 & 0xFF);
            objRGB.C2 = (byte)(~objRGB.C2 & 0xFF);

            Pen pen = new Pen(Color.FromArgb(objRGB.C0, objRGB.C1, objRGB.C2));
            g.DrawLine(pen, intMinSaturation, 0, intMinSaturation, intHeight);
            g.DrawLine(pen, intMaxSaturation, 0, intMaxSaturation, intHeight);

            return bm;
        }

        /// <summary>
        /// Draw color threshold image
        /// </summary>
        /// <param name="objSourceCmage">source image</param>
        /// <param name="intColorThreshold">Color Threshold value in array RGB or LSH</param>
        /// <param name="intColorTolerance">Color Tolerance value in array RGB or LSH</param>
        /// <param name="blnRGBFormat">true = RGB format; false = LSH format</param>
        /// <returns>destination bitmap image</returns>
        public static Image DrawThresholdImage(CImageDrawing objSourceCmage, int[] intColorThreshold, int[] intColorTolerance, bool blnRGBFormat)
        {
            Bitmap bm = new Bitmap(objSourceCmage.ref_objMainCImage.Width, objSourceCmage.ref_objMainCImage.Height);
            Graphics g = Graphics.FromImage(bm);

            objSourceCmage.RedrawImage(g, intColorThreshold, intColorTolerance, blnRGBFormat);

            return bm;
        }

    }
}
