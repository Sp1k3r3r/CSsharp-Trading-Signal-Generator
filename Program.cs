using System;
using System.Collections.Generic;
using System.IO;

namespace TradingSignalGenerator
{
    // Класс для хранения данных одного дня
    public class StockCandle
    {
        public string Date { get; set; }
        public double Price { get; set; }
        public string Signal { get; set; } // BUY, SELL или HOLD
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== АНАЛИЗАТОР ТОРГОВЫХ СИГНАЛОВ ===");

            // 1. Создаем тестовые данные (имитация цен акции Apple за неделю)
            List<StockCandle> candles = new List<StockCandle>
            {
                new StockCandle { Date = "2026-09-01", Price = 180.5 },
                new StockCandle { Date = "2026-09-02", Price = 178.2 },
                new StockCandle { Date = "2026-09-03", Price = 182.0 },
                new StockCandle { Date = "2026-09-04", Price = 185.4 },
                new StockCandle { Date = "2026-09-05", Price = 181.1 }
            };

            // 2. Алгоритм генерации сигналов: 
            // Если цена растет более чем на 2% за день — BUY, если падает — SELL
            for (int i = 0; i < candles.Count; i++)
            {
                if (i == 0)
                {
                    candles[i].Signal = "HOLD"; // Для первого дня нет предыдущей цены
                    continue;
                }

                double prevPrice = candles[i - 1].Price;
                double currentPrice = candles[i].Price;
                double changePercent = ((currentPrice - prevPrice) / prevPrice) * 100;

                if (changePercent > 1.5)
                {
                    candles[i].Signal = "BUY (Покупка)";
                }
                else if (changePercent < -1.5)
                {
                    candles[i].Signal = "SELL (Продажа)";
                }
                else
                {
                    candles[i].Signal = "HOLD (Удержание)";
                }
            }

            // 3. Сохраняем результат в CSV-файл
            string filePath = "trading_signals.csv";
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Дата;Цена Закрытия;Сигнал"); // Заголовок CSV

                foreach (var item in candles)
                {
                    writer.WriteLine($"{item.Date};{item.Price};{item.Signal}");
                    Console.WriteLine($"Дата: {item.Date} | Цена: {item.Price} | Сигнал: {item.Signal}");
                }
            }

            Console.WriteLine("\n-------------------------------------------");
            Console.WriteLine($"Успешно! Отчет сохранен в файл: {Path.GetFullPath(filePath)}");
            Console.WriteLine("Нажми Enter для выхода...");
            Console.ReadLine();
        }
    }
}