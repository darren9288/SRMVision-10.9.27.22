using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Euresys.Open_eVision_2_12;

namespace Ocr2Studio
{
  public class BitmapCreatorVisitor : IEOCR2TextVisitor
  {
    private EImageBW8 img_;

    private int x_;
    private int y_;

    private BitmapInfosHelperVisitor infos_;


    public BitmapCreatorVisitor(EImageBW8 img)
    {
      img_ = img;

      x_ = 0;
      y_ = 0;

      infos_ = new BitmapInfosHelperVisitor();
    }


    private void SetupImage(int width, int height)
    {
      img_.SetSize(width, height);
      EasyImage.Copy(new EBW8(0), img_);
    }


    public void Visit(EOCR2Text t)
    {
      infos_.Visit(t);
      SetupImage(infos_.Width, infos_.Height);

      InternalVisit(t);
    }


    private void InternalVisit(EOCR2Text t)
    {
      foreach (EOCR2Line l in t.Lines)
      {
        InternalVisit(l);

        x_ = 0;
        y_ += infos_.LineHeight;

        l.Dispose();
      }
    }


    public void Visit(EOCR2Line l)
    {
      infos_.Visit(l);
      SetupImage(infos_.Width, infos_.Height);

      InternalVisit(l);
    }


    private void InternalVisit(EOCR2Line l)
    {
      foreach(EOCR2Word w in l.Words)
      {
        InternalVisit(w);

        w.Dispose();
      }
    }


    public void Visit(EOCR2Word w)
    {
      infos_.Visit(w);
      SetupImage(infos_.Width, infos_.Height);

      InternalVisit(w);
    }


    private void InternalVisit(EOCR2Word w)
    {
      foreach(EOCR2Char c in w.Characters)
      {
        InternalVisit(c);

        EROIBW8 bitmap = c.Bitmap;
        x_ += bitmap.Width;
        bitmap.Dispose();

        c.Dispose();
      }
    }


    public void Visit(EOCR2Char c)
    {
      infos_.Visit(c);
      SetupImage(infos_.Width, infos_.Height);

      InternalVisit(c);
    }


    private void InternalVisit(EOCR2Char c)
    {
      IntPtr hdc = Easy.OpenImageGraphicContext(img_);

      EROIBW8 bitmap = c.Bitmap;
      bitmap.Draw(hdc, 1, 1, x_, y_);
      bitmap.Dispose();

      Easy.CloseImageGraphicContext(img_, hdc);
    }
  }
}
