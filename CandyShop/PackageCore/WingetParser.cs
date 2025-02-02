using System;
using System.Collections.Generic;
using System.Linq;

namespace CandyShop.PackageCore
{
    internal record WingetColumn(string Name, int Offset);

    internal class WingetTable(WingetColumn[] columns)
    {
        public WingetColumn[] Columns { get; set; } = columns;
        public List<string[]> Items { get; set; } = [];

        public bool HasMatchingColumns(WingetTable other)
        {
            if (Columns.Length != other.Columns.Length)
                return false;

            for (int i = 0; i < Columns.Length; i++)
                if (!Columns[i].Name.Equals(other.Columns[i].Name))
                    return false;

            return true;
        }
    }

    internal class WingetParser
    {
        enum State { LookingForTable, BeginTableContentRead, ReadingTableContent };

        private Queue<string> _Output;

        private State _CurrentState;

        private List<WingetTable> Tables = [];

        private WingetTable CurrentTable => Tables[^1];

        public string[] Columns { get; private set; }

        public string[][] Items { get; private set; }

        public WingetParser(string output)
        {
            output = TrimProgressChars(output);
            _Output = new(output.Split(Environment.NewLine));

            // read all output; may contain multiple tables
            while (_Output.Count > 0)
            {
                var line = _Output.Dequeue();
                switch (_CurrentState)
                {
                    case State.LookingForTable:
                        _CurrentState = LookForTable(line);
                        break;
                    case State.BeginTableContentRead:
                        _CurrentState = State.ReadingTableContent;
                        break;
                    case State.ReadingTableContent:
                        _CurrentState = ReadTableContent(line);
                        break;
                    default:
                        break;
                }
            }

            // disregard tables from the first different column layout
            for (int i = 0; i < Tables.Count - 1; i++)
            {
                if (!Tables[i].HasMatchingColumns(Tables[i + 1]))
                {
                    Tables = Tables.GetRange(0, i + 1);
                    break;
                }
            }

            // create output
            if (Tables.Count > 0)
            {
                Columns = Tables[0].Columns.Select(col => col.Name).ToArray();
                Items = Tables.SelectMany((table) => table.Items).ToArray();
            }
        }

        private State LookForTable(string tableHead)
        {
            // current line is table head if next is divider
            if (_Output.TryPeek(out string divider))
                if (string.IsNullOrEmpty(divider) || divider.All(c => c.Equals('-')))
                {
                    var columns = ParseTableHead(tableHead);
                    Tables.Add(new WingetTable(columns));
                    return State.BeginTableContentRead;
                }

            return State.LookingForTable;
        }

        private State ReadTableContent(string row)
        {
            // check end of content
            if (string.IsNullOrEmpty(row) || row.Length < CurrentTable.Columns[^1].Offset)
            {
                // some tables end with a status message like '39 upgrades available.'
                // and some with a blank line; there is no reliable way to determine
                // the end, so let's assume the status line is shorter than the table
                // content; this seems to work for now
                return State.LookingForTable;
            }

            // parse items from row
            var items = ParseTableRow(row);
            CurrentTable.Items.Add(items);
            return State.ReadingTableContent;
        }

        private WingetColumn[] ParseTableHead(string tableHead)
        {
            List<WingetColumn> columns = [];
            int i = 0;
            while (i < tableHead.Length)
            {
                int columnStart = i;

                // get column name
                while (i < tableHead.Length && char.IsLetterOrDigit(tableHead[i]))
                    i++;

                string name = tableHead[columnStart..i];
                columns.Add(new WingetColumn(name, columnStart));

                // skip whitespaces until next column starts
                while (i < tableHead.Length && char.IsWhiteSpace(tableHead[i]))
                    i++;
            }

            return columns.ToArray();
        }

        private string[] ParseTableRow(string row)
        {
            WingetColumn[] cols = CurrentTable.Columns;
            string[] items = new string[cols.Length];

            for (int i = 0; i < cols.Length; i++)
            {
                int start = cols[i].Offset;
                int end = i + 1 < cols.Length ? cols[i + 1].Offset : row.Length;
                if (start < row.Length && end <= row.Length)
                {
                    items[i] = row[start..end].Trim();
                }
                else
                {
                    items[i] = string.Empty;
                }
            }

            return items;
        }

        public static string TrimProgressChars(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            int i = 0;
            while (i < value.Length && !char.IsLetterOrDigit(value[i]))
            {
                i++;
            }
            return value[i..];
        }
    }
}
