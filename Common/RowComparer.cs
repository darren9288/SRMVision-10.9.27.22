// (c) Vasian Cepa 2005
// Version 2

using System;
using System.Collections; // required for RowComparer : IComparer only
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Common
{

	public class RowComparer : IComparer
	{
        #region DllImport

        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        public static extern int StrCmpLogicalW(string psz1, string psz2);

        #endregion

        private static int m_intSortOrderModifier = 1;
        private static int m_intCellNo = 0;
        public RowComparer(SortOrder sortOrder, int intCellNo)
        {
            m_intCellNo = intCellNo;

            if (sortOrder == SortOrder.Descending)
            {
                m_intSortOrderModifier = -1;
            }
            else if (sortOrder == SortOrder.Ascending)
            {
                m_intSortOrderModifier = 1;
            }
        }

        public int Compare(object x, object y)
        {
            if ((x is DataGridViewRow) && (y is DataGridViewRow))
            {
                DataGridViewRow DataGridViewRow1 = (DataGridViewRow)x;
                DataGridViewRow DataGridViewRow2 = (DataGridViewRow)y;

                //if the cell value are the same, compare other cell value
                int intCellNo = m_intCellNo;
                if (DataGridViewRow1.Cells[m_intCellNo].Value.ToString() == DataGridViewRow2.Cells[m_intCellNo].Value.ToString())
                {
                    for (int i = 0; i < DataGridViewRow1.Cells.Count; i++)
                    {
                        if (i == m_intCellNo)
                            continue;

                        if(DataGridViewRow1.Cells[i].Value != DataGridViewRow2.Cells[i].Value)
                        {
                            intCellNo = i;
                            break;
                        }
                    }
                }

                return StrCmpLogicalW(DataGridViewRow1.Cells[intCellNo].Value.ToString(), DataGridViewRow2.Cells[intCellNo].Value.ToString()) * m_intSortOrderModifier;
            }
            return -1;
        }

    }//EOC

}