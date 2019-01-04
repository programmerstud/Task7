using MarketCharts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MarketCharts.BL
{
    public class MarketLogic
    {
        public delegate void ReceiveCandleEvent(Candle candle, double? indicatorMiddleValue1, double? indicatorUpperValue2, double? indicatorLowerValue3);
        public event ReceiveCandleEvent OnReceiveCandle;
        private Thread Thread;

        private Queue<Candle> Candles;
        private Queue<double?> BBMiddle;
        private Queue<double?> BBUpper;
        private Queue<double?> BBLower;

        public MarketLogic(ILoadData dataInfo, IIndicator indicator)
        {
            Candles = dataInfo.Candles();
            indicator.Calculator(Candles);
            BBMiddle = indicator.BBMiddle;
            BBUpper = indicator.BBUpper;
            BBLower = indicator.BBLower;
        }
        public void RunThread()
        {
            Thread = new Thread(new ThreadStart(UpdateThread));
            Thread.Start();
        }
        public void DisposeThread()
        {
            Thread.Abort();
        }
        protected void UpdateThread()
        {
            while (Candles.Count != 0)
            {
                OnReceiveCandle?.Invoke(Candles.Dequeue(), BBMiddle.Dequeue(), BBUpper.Dequeue(), BBLower.Dequeue());

                Thread.Sleep(1000);
            }
        }
    }
}
