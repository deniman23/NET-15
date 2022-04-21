// See https://aka.ms/new-console-template for more information
using System.Text;
using System.Collections.Generic;


Console.WriteLine("Домашнее задание. Создать список продуктов.");

Console.WriteLine("-------------------------------------------");
Console.WriteLine("Введите количество продуктов:");
var prodNumber = int.Parse(Console.ReadLine());
List<int> price = new List<int>();
List<int> height = new List<int>();
for (int i = 0; i < prodNumber; i++)
{
    Console.WriteLine("Введите цену продукта:");
    price.Add(Convert.ToInt32(Console.ReadLine()));
    Console.WriteLine("Введите количество продукта:");
    height.Add(Convert.ToInt32(Console.ReadLine()));

}
Console.WriteLine("-------------------------------------------");
Console.WriteLine("Список цен");

foreach (var Prices in price)
{
    Console.WriteLine(Prices);
    
}

Console.WriteLine("Средняя сумма:");

var average = price.Average();
Console.WriteLine(average);



public class Product
{
    //void ShowProd();
}


