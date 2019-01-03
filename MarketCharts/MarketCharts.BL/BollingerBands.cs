using MarketCharts.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MarketCharts.BL
{
    public class BollingerBands : IIndicator
    {
        int BBLength = 20;
        double BBMultiplier = 2;
        public Queue<double?> BBMiddle { get; set; } = new Queue<double?>();
        public Queue<double?> BBUpper { get; set; } = new Queue<double?>();
        public Queue<double?> BBLower { get; set; } = new Queue<double?>();

        public void Calculator(Queue<Candle> Candles)
        {
            foreach (Candle c in Candles)
            {
                int PCC = Candles.Where(a => a.TimeStamp < c.TimeStamp).Count();
                if (PCC > BBLength)
                {
                    double? bbM = (double?)Candles.Where(a => a.TimeStamp <= c.TimeStamp).OrderByDescending(a => a.TimeStamp).Take(BBLength).Average(a => a.Close);
                    BBMiddle.Enqueue(bbM);
                    double total_squared = 0;
                    double total_for_average = Convert.ToDouble(Candles.Where(a => a.TimeStamp <= c.TimeStamp).OrderByDescending(a => a.TimeStamp).Take(BBLength).Sum(a => a.Close));
                    foreach (Candle cb in Candles.Where(a => a.TimeStamp <= c.TimeStamp).OrderByDescending(a => a.TimeStamp).Take(BBLength).ToList())
                    {
                        total_squared += Math.Pow(Convert.ToDouble(cb.Close), 2);
                    }
                    double stdev = Math.Sqrt((total_squared-Math.Pow(total_for_average, 2)/BBLength)/BBLength);
                    BBUpper.Enqueue(bbM + (stdev * BBMultiplier));
                    BBLower.Enqueue(bbM - (stdev * BBMultiplier));

                }
            }
        }
    }
}
