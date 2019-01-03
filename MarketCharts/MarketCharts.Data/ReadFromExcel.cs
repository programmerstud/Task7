using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MarketCharts.Data
{
    public class ReadFromExcel : ILoadData
    {
        private string path;

        public ReadFromExcel(string path)
        {
            this.path = path;
        }

        public Queue<Candle> Candles()
        {
            Queue<Candle> Candles = new Queue<Candle>();

            using (ExcelPackage xlPackage = new ExcelPackage(new FileInfo(path)))
             {
                 var myWorksheet = xlPackage.Workbook.Worksheets[1];
                 var totalRows = myWorksheet. Dimension.End.Row;
                 var totalColumns = myWorksheet.Dimension.End.Column;

                 for (int rowNum = 2; rowNum <= totalRows; rowNum++)
                 {
                     var row = myWorksheet.Cells[rowNum, 1, rowNum, totalColumns]
                         .Select(c => c.Value == null ? string.Empty : c.Value.ToString()).ToArray();

                     string rawDateTime = row[2] + " " + row[3];

                     DateTime dateTime = DateTime.ParseExact(rawDateTime, "yyyyMMdd HHmmss", null);

                     Candle candle = new Candle()
                     {
                         High = int.Parse(row[5]),
                         Low = int.Parse(row[6]),
                         Open = int.Parse(row[4]),
                         Close = int.Parse(row[7]),
                         Time = dateTime,
                         TimeStamp = ((DateTimeOffset)dateTime).ToUnixTimeMilliseconds(),
                     };

                     Candles.Enqueue(candle); 
                 }
            }
            return Candles;

        }
    }
}
