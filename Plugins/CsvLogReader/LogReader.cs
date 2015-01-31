using System;
using System.Collections.Generic;
using System.IO;

namespace CsvLogReader
{
	public class LogReader
	{
		public class CsvRow : List<string>
		{
			public string LineText { get; set; }
		}
		public class CsvFileReader : StreamReader
		{
			public CsvFileReader(Stream stream)
				:base(stream)
			{
			}
			public bool ReadRow(CsvRow row)
			{
				row.LineText = ReadLine();
				if (String.IsNullOrEmpty(row.LineText))
					return false;
				int pos = 0;
				int rows = 0;

				while(pos<row.LineText.Length)
				{
					string value;

					if(row.LineText[pos]=='"')
					{
						pos++;
						int start = pos;
						while (pos < row.LineText.Length)
						{
							if (row.LineText[pos] == '"')
							{
								pos++;

								if (pos >= row.LineText.Length || row.LineText[pos] != '"')
								{
									pos--;
									break;
								}
							}
							pos++;
						}
						value = row.LineText.Substring(start, pos - start);
						value = value.Replace("\"\"", "\"");
					}
					else
					{
						int start = pos;
						while (pos < row.LineText.Length && row.LineText[pos] != ',')
							pos++;
						value = row.LineText.Substring(start, pos - start);
					}

					if (rows < row.Count)
						row[rows] = value;
					else
						row.Add(value);
					rows++;

					while (pos < row.LineText.Length && row.LineText[pos] != ',')
						pos++;
					if (pos < row.LineText.Length)
						pos++;
				}
				while (row.Count < rows)
					row.RemoveAt(rows);

				return (row.Count > 0);
			}
		}
	}
}
