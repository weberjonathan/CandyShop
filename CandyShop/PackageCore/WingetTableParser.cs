using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Unicode;

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

    public class WingetTableParser
    {
        public string[] Columns { get; private set; } = [];

        public string[][] Items { get; private set; } = [];

        public bool HasTable => Columns.Length > 0 && Items.Length > 0;

        public WingetTableParser(string output)
        {
            var _output = new Queue<string>(output.Split(Environment.NewLine));

            // monitor occurence of unicode chars
            if (output.Any(c => UnicodeRanges.BlockElements.Contains(c)))
                Log.Debug($"WingetParser: Encountered unicode block characters in output.");

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

        /// <summary>
        /// Discards rows from the queue until a table is identified by its divider
        /// and returns a new empty table with column names and offsets
        /// </summary>
        private WingetTable LookForTable(Queue<string> output)
        {
            if (!output.TryDequeue(out string tableHead))
                return null;

            // current row is table head if next is divider
            while (output.TryDequeue(out string divider))
            {
                if (!string.IsNullOrEmpty(divider) && divider.All(c => c.Equals('-')))
                {
                    tableHead = WingetParser.LeftTrimProgressChars(tableHead);
                    WingetColumn[] columns = ParseTableHead(tableHead);
                    return new WingetTable(columns);
                }

                tableHead = divider;
            }

            return null;
        }

        /// <summary>
        /// Reads rows from the queue and adds them to the table
        /// until the format changes or end of queue.
        /// </summary>
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

        /// <summary>
        /// Reads column names and column offsets from a winget table heading
        /// </summary>
        private WingetColumn[] ParseTableHead(string tableHead)
        {
            List<WingetColumn> columns = [];

            StringBuilder currentColName = new();
            int currentColOffset = 0;
            bool isReadingName = true;
            for (int i = currentColOffset; i < tableHead.Length; i++)
            {
                if (isReadingName)
                {
                    if (char.IsLetterOrDigit(tableHead[i]))
                    {
                        // continue reading col name
                        currentColName.Append(tableHead[i]);
                    }
                    else if (char.IsWhiteSpace(tableHead[i]))
                    {
                        // finish reading col name
                        columns.Add(new WingetColumn(currentColName.ToString(), currentColOffset));
                        isReadingName = false;
                    }
                }
                else
                {
                    if (char.IsLetterOrDigit(tableHead[i]))
                    {
                        // begin reading col name
                        currentColName = new();
                        currentColName.Append(tableHead[i]);
                        currentColOffset = i;
                        isReadingName = true;
                    }
                }
            }

            if (isReadingName)
                columns.Add(new WingetColumn(currentColName.ToString(), currentColOffset));

            return columns.ToArray();
        }

        /// <summary>
        /// Reads items in row based on column offsets
        /// </summary>
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
    }
}
