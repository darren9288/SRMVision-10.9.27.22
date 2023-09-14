using Euresys.Open_eVision_2_12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Ocr2Studio
{
  interface IEOCR2TextVisitor
  {
    void Visit(EOCR2Text t);
    void Visit(EOCR2Line l);
    void Visit(EOCR2Word w);
    void Visit(EOCR2Char c);
  }
}
