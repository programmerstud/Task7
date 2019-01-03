using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketCharts.Data
{
    public interface ILoadData
    {
        Queue<Candle> Candles();
    }
}
