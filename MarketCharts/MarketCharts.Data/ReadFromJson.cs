using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketCharts.Data
{
    public class ReadFromJson : ILoadData
    {
        private string path;
        public ReadFromJson(string path)
        {
            this.path = path;
        }
        public Queue<Candle> Candles()
        {
            Queue<Candle> Candles = new Queue<Candle>();

            dynamic jsonObject = JsonConvert.DeserializeObject(File.ReadAllText(path));

            for (int i = 0; i < ((JArray)jsonObject.t).Count; i++)
            {
                Candle candle = new Candle()
                {
                    High = (decimal)((JArray)jsonObject.h)[i],
                    Low = (decimal)((JArray)jsonObject.l)[i],
                    Open = (decimal)((JArray)jsonObject.o)[i],
                    Close = (decimal)((JArray)jsonObject.c)[i],
                    Time = DateTimeOffset.FromUnixTimeSeconds((long)((JArray)jsonObject.t)[i]).UtcDateTime,
                    TimeStamp = (long)((JArray)jsonObject.t)[i],
                };
                Candles.Enqueue(candle);
            }
            return Candles;
        }
    }
}
