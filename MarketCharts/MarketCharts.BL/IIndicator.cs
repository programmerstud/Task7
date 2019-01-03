using MarketCharts.Data;
using System.Collections.Generic;

namespace MarketCharts.BL
{
    public interface IIndicator
    {
        Queue<double?> BBLower { get; set; }
        Queue<double?> BBMiddle { get; set; }
        Queue<double?> BBUpper { get; set; }

        void Calculator(Queue<Candle> Candles);
    }
}
