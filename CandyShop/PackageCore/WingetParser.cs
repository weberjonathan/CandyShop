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
        public string[] Columns { get; private set; }

        public string[][] Items { get; private set; }

        public WingetParser(string output)
        {
            output = TrimProgressChars(output);
            var _output = new Queue<string>(output.Split(Environment.NewLine));

            // read all output; may contain multiple tables
            List<WingetTable> tables = [];
            while (_output.Count > 0)
            {
                var table = LookForTable(_output);
                if (table != null)
                {
                    ReadTableContent(_output, table);
                    tables.Add(table);
                }
            }

            // disregard tables from the first different column layout
            for (int i = 0; i < tables.Count - 1; i++)
            {
                if (!tables[i].HasMatchingColumns(tables[i + 1]))
                {
                    tables = tables.GetRange(0, i + 1);
                    break;
                }
            }

            // create output
            if (tables.Count > 0)
            {
                Columns = tables[0].Columns.Select(col => col.Name).ToArray();
                Items = tables.SelectMany((table) => table.Items).ToArray();
            }
        }

        private WingetTable LookForTable(Queue<string> output)
        {
            if (!output.TryDequeue(out string tableHead))
                return null;

            // current row is table head if next is divider
            while (output.TryDequeue(out string divider))
            {
                if (string.IsNullOrEmpty(divider) || divider.All(c => c.Equals('-')))
                {
                    var columns = ParseTableHead(tableHead);
                    return new WingetTable(columns);
                }

                tableHead = divider;
            }

            return null;
        }

        private WingetTable ReadTableContent(Queue<string> output, WingetTable table)
        {
            while (output.TryDequeue(out string tableRow))
            {
                // check end of content
                if (string.IsNullOrEmpty(tableRow) || tableRow.Length < table.Columns[^1].Offset)
                {
                    // some tables end with a status message like '39 upgrades available.'
                    // and some with a blank line; there is no reliable way to determine
                    // the end, so let's assume the status line is shorter than the table
                    // content; this seems to work for now
                    break;
                }

                // parse content
                var items = ParseTableRow(table.Columns, tableRow);
                table.Items.Add(items);
            }

            return table;
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

        private string[] ParseTableRow(WingetColumn[] columns, string row)
        {
            string[] items = new string[columns.Length];

            for (int i = 0; i < columns.Length; i++)
            {
                int start = columns[i].Offset;
                int end = i + 1 < columns.Length ? columns[i + 1].Offset : row.Length;
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
