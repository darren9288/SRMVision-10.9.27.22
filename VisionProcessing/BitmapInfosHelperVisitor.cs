using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Euresys.Open_eVision_2_12;

namespace Ocr2Studio
{
  public class BitmapInfosHelperVisitor : IEOCR2TextVisitor
  {
    private int currentLineWidth_;
    private int width_;
    private int lineCount_;


    public int LineHeight
    {
      get;
      private set;
    }


    public int Width
    {
      get
      {
        return Math.Max(width_, currentLineWidth_);
      }
    }


    public int Height
    {
      get
      {
        return lineCount_ * LineHeight;
      }
    }


    public BitmapInfosHelperVisitor()
    {
      currentLineWidth_ = 0;
      width_ = 0;
      LineHeight = 0;
      lineCount_ = 1;
    }


    public void Visit(EOCR2Text t)
    {
      lineCount_ = t.Lines.Count();

      foreach (EOCR2Line l in t.Lines)
      {
        currentLineWidth_ = 0;
        LineHeight = 0;

        Visit(l);
        l.Dispose();

        if (currentLineWidth_ > width_)
          width_ = currentLineWidth_;
      }
    }


    public void Visit(EOCR2Line l)
    {
      foreach (EOCR2Word w in l.Words)
      { 
        Visit(w);
        w.Dispose();
      }
    }


    public void Visit(EOCR2Word w)
    {
      foreach(EOCR2Char c in w.Characters)
      {
        Visit(c);
        c.Dispose();
      }
    }


    public void Visit(EOCR2Char c)
    {
      EROIBW8 bitmap = c.Bitmap;

      if (bitmap.Height > LineHeight)
        LineHeight = bitmap.Height;

      currentLineWidth_ += bitmap.Width;

      bitmap.Dispose();
    }
  }
}