using System;
using System.Collections.Generic;
using System.Text;

namespace OSECore.Text
{
    public class LineFormatter
    {
        private LineMargins _margins;
        private TabStops _tabStops;
        public LineFormatter(LineMargins margins = null, TabStops tabStops = null)
        {
            _margins = margins ?? new LineMargins();
            _tabStops = tabStops ?? new TabStops();
        }
        public LineMargins Margins { get => _margins; set => _margins = value; }
        public TabStops TabStops { get => _tabStops; set => _tabStops = value; }
        public string[] FormatLine(string line)
        {
            List<string> formatted = new List<string>();
            string[] lines = line.SplitLines();
            foreach(var l in lines)
            {
                string m = Margins.GetLeftPadding();
                int c = Margins.LeftMargin;
                string[] columns = l.Split('\t');
                int columnIndex = 0;
                foreach(var column in columns)
                {
                    if(columnIndex > 0)
                    {
                        m += TabStops.GetPadding(columnIndex, c);
                        c = m.Length + 1;
                    }
                    if(c + column.Length < Margins.RightMargin)
                    {
                        m += column;
                        c += column.Length;
                    }
                    else
                    {
                        string[] fields = column.SplitLine();
                        bool first = true;
                        foreach(var field in fields)
                        {
                            if(c + field.Length < Margins.RightMargin)
                            {
                                if (!first)
                                {
                                    m += ' ';
                                    c++;
                                }
                                first = false;
                                m += field;
                                c += field.Length;
                            }
                            else
                            {
                                formatted.Add(m);
                                if(columnIndex == 0)
                                {
                                    c = Margins.HangingIndent;
                                    m = Margins.GetHangingIndentPadding() + field;
                                }
                                else
                                {
                                    c = TabStops.GetTabColumn(columnIndex);
                                    m = TabStops.GetPadding(columnIndex) + field;
                                }
                            }
                        }
                    }
                    ++columnIndex;
                }
                if (c > Margins.LeftMargin)
                    formatted.Add(m);
            }
            return formatted.ToArray();
        }
    }
}
