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
        public string GetPadding(int start, int end)
        {
            if (start < end)
                return new string(' ', end - start);
            else
                return "";
        }
        private (string, int) FormatField(string line, int pos, int index, string field, bool first, List<string> lines)
        {
            if (pos + field.Length < Margins.RightMargin)
            {
                if (!first)
                {
                    line += ' ';
                    pos++;
                }
                line += field;
                pos += field.Length;
            }
            else
            {
                lines.Add(line);
                if (index == 0)
                {
                    pos = Margins.HangingIndent;
                    line = Margins.GetHangingIndentPadding() + field;
                }
                else
                {
                    pos = TabStops.GetTabColumn(index);
                    line = TabStops.GetPadding(index) + field;
                }
            }
            return (line, pos);
        }
        private (string,int) FormatColumn(string line, int pos, int index, string column, List<string> lines)
        {
            if (index > 0)
            {
                line += TabStops.GetPadding(index, pos);
                pos = line.Length + 1;
            }
            if (pos + column.Length < Margins.RightMargin)
            {
                line += column;
                pos += column.Length;
            }
            else
            {
                string[] fields = column.SplitLine();
                bool first = true;
                foreach (var field in fields)
                {
                    (line, pos) = FormatField(line, pos, index, field, first, lines);
                    first = false;
                }
            }
            return (line, pos);
        }
        public string[] FormatLine(int startColumn, string line)
        {
            List<string> formatted = new List<string>();
            string m = GetPadding(1, startColumn);
            int c = startColumn;
            string[] columns = line.Split('\t');
            int columnIndex = 0;
            foreach (var column in columns)
            {
                (m, c) = FormatColumn(m, c, columnIndex, column, formatted);
                ++columnIndex;
            }
            if (c > startColumn)
                formatted.Add(m);
            return formatted.ToArray();
        }
        public string[] FormatLine(string line)
        {
            List<string> formatted = new List<string>();
            string[] lines = line.SplitLines();
            foreach(var l in lines)
            {
                string[] m = FormatLine(Margins.LeftMargin, l);
                formatted.AddRange(m);
            }
            return formatted.ToArray();
        }
        public string[] FormatTitleLine(string line)
        {
            List<string> formatted = new List<string>();
            formatted.AddRange(FormatLine(line));
            int columns = TabStops.Count;
            int i = Margins.LeftMargin;
            int j = 0;
            string outLine = "";
            int ts0 = 1;
            foreach(int ts in TabStops)
            {
                int l = ts - ts0 - 1;
                if (l > 0)
                {
                    (outLine, i) = FormatColumn(outLine, i, j, new string('-', l), formatted);
                }
                j++;
                ts0 = ts;
            }
            if(i < Margins.RightMargin)
            {
                (outLine, i) = FormatColumn(outLine, i, j, new string('-', Margins.RightMargin - ts0 - 1), formatted);
            }
            formatted.Add(outLine);
            return formatted.ToArray();
        }
    }
}
