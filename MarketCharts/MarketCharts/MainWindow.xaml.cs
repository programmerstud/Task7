using MarketCharts.BL;
using MarketCharts.Data;
using Microsoft.Win32;
using SciChart.Charting.Model.DataSeries;
using SciChart.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MarketCharts
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private OhlcDataSeries<DateTime, double> ohlcSeries;
        private XyDataSeries<DateTime, double> BBMiddleSeries;
        private XyDataSeries<DateTime, double> BBUpperSeries;
        private XyDataSeries<DateTime, double> BBLowerSeries;
        private MarketLogic MonitorLogic;
        private int candlesCount = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainMenu_File_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    if (System.IO.Path.GetExtension(openFileDialog.FileName) == ".xlsx" || System.IO.Path.GetExtension(openFileDialog.FileName) == ".json")
                    {
                        if (System.IO.Path.GetExtension(openFileDialog.FileName) == ".xlsx")
                            LoadData(new ReadFromExcel(openFileDialog.FileName));
                        if (System.IO.Path.GetExtension(openFileDialog.FileName) == ".json")
                            LoadData(new ReadFromJson(openFileDialog.FileName));
                    }
                    else
                        throw new Exception();
                }
                catch (Exception error)
                {
                    MessageBox.Show("Ошибка", "Невозможно открыть файл.", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void StartNewChart()
        {
            ohlcSeries = new OhlcDataSeries<DateTime, double>() { SeriesName = "Candles", FifoCapacity = 100 };
            BBMiddleSeries = new XyDataSeries<DateTime, double>() { SeriesName = "BBMid", FifoCapacity = 100 };
            BBUpperSeries = new XyDataSeries<DateTime, double>() { SeriesName = "BBUpp", FifoCapacity = 100 };
            BBLowerSeries = new XyDataSeries<DateTime, double>() { SeriesName = "BBLow", FifoCapacity = 100 };

            CandleSeries.DataSeries = ohlcSeries;
            BBMiddle.DataSeries = BBMiddleSeries;
            BBUpper.DataSeries = BBUpperSeries;
            BBLower.DataSeries = BBLowerSeries;

            MonitorLogic.RunThread();
        }

        private void LoadData(ILoadData data)
        {
            if (this.MonitorLogic != null)
            {
                MonitorLogic.DisposeThread();
                MonitorLogic = null;
            }

            MonitorLogic = new MarketLogic(data, new BollingerBands());
            MonitorLogic.OnReceiveCandle += OnCandleReceived;

            StartNewChart();
        }

        private void OnCandleReceived(Candle candle, double? indicatorValue1, double? indicatorValue2, double? indicatorValue3)
        {
            StockChart.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { AddNewCandle(candle, indicatorValue1, indicatorValue2, indicatorValue3); }));
        }

        private void AddNewCandle(Candle candle, double? indicatorValue1, double? indicatorValue2, double? indicatorValue3)
        {
            using (ohlcSeries.SuspendUpdates())
            using (BBMiddleSeries.SuspendUpdates())
            using (BBUpperSeries.SuspendUpdates())
            using (BBLowerSeries.SuspendUpdates())
            {
                candlesCount++;
                ohlcSeries.Append(candle.Time, (double)candle.Open, (double)candle.High, (double)candle.Low, (double)candle.Close);

                if (indicatorValue1 != null)
                    BBMiddleSeries.Append(candle.Time, indicatorValue1.Value);
                if (indicatorValue2 != null)
                    BBUpperSeries.Append(candle.Time, indicatorValue2.Value);
                if (indicatorValue3 != null)
                    BBLowerSeries.Append(candle.Time, indicatorValue3.Value);

                StockChart.XAxis.VisibleRange = new IndexRange(candlesCount - 50, candlesCount);
            }
        }
    }
}
