using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace TradingSignalGenerator
{
    public class StockCandle
    {
        public string Date { get; set; }
        public double Price { get; set; }
        public string Signal { get; set; }
    }

    class Program
    {
        private static readonly string filePath = "trading_signals.csv";

        static async Task Main(string[] args)
        {
            Console.WriteLine("=== АВТОМАТИЧЕСКИЙ ТРЕЙДИНГ-МОНИТОР ===");
            List<StockCandle> history = LoadHistoryFromCsv(filePath);
            StockCandle todayCandle = FetchTodayData();
            if (history.Any(c => c.Date == todayCandle.Date))
            {
                Console.WriteLine($"[!] Данные за {todayCandle.Date} уже записаны в таблицу.");
                return;
            }
            if (history.Count > 0)
            {
                double prevPrice = history.Last().Price;
                double currentPrice = todayCandle.Price;
                double changePercent = ((currentPrice - prevPrice) / prevPrice) * 100;

                if (changePercent > 1.5)
                    todayCandle.Signal = "BUY (Покупка)";
                else if (changePercent < -1.5)
                    todayCandle.Signal = "SELL (Продажа)";
                else
                    todayCandle.Signal = "HOLD (Удержание)";

                Console.WriteLine($"Предыдущая цена: {prevPrice}$ | Сегодня: {currentPrice}$ (Изменение: {changePercent:F2}%)");
            }
            else
            {
                todayCandle.Signal = "HOLD (Базовый день)";
            }
            AppendToCsv(filePath, todayCandle);

            Console.WriteLine($"[Успешно] Новые данные за {todayCandle.Date} добавлены в {filePath}!");
            Console.WriteLine($"Сигнал на сегодня: {todayCandle.Signal}");
        }
        static List<StockCandle> LoadHistoryFromCsv(string path)
        {
            List<StockCandle> list = new List<StockCandle>();
            if (!File.Exists(path)) return list;

            string[] lines = File.ReadAllLines(path);
            for (int i = 1; i < lines.Length; i++)
            {
                var parts = lines[i].Split(';');
                if (parts.Length >= 3 && double.TryParse(parts[1], out double price))
                {
                    list.Add(new StockCandle { Date = parts[0], Price = price, Signal = parts[2] });
                }
            }
            return list;
        }
        static void AppendToCsv(string path, StockCandle candle)
        {
            bool fileExists = File.Exists(path);
            using (StreamWriter writer = new StreamWriter(path, append: true))
            {
                if (!fileExists)
                {
                    writer.WriteLine("Дата;Цена Закрытия;Сигнал");
                }
                writer.WriteLine($"{candle.Date};{candle.Price};{candle.Signal}");
            }
        }
        static StockCandle FetchTodayData()
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            Random rand = new Random();
            double randomPrice = Math.Round(180.0 + (rand.NextDouble() * 10 - 5), 2);

            return new StockCandle
            {
                Date = today,
                Price = randomPrice
            };
        }
    }
}