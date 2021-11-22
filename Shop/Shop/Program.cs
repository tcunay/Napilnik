using System;
using System.Collections.Generic;
using System.Linq;

namespace Shop
{
    public class Program
    {
        private readonly Shop _shop = new Shop(new GoodCellContainer(), new GoodCellContainer());
        private readonly Good _iPhone12 = new Good("IPhone 12");
        private readonly Good _iPhone11 = new Good("IPhone 11");

        private void Start()
        {
            _shop.AddToWarehouse(new GoodCell(_iPhone12, 10), new GoodCell(_iPhone11, 1));

            foreach (var goodCell in _shop.Warehouse.Goods)
            {
                Console.WriteLine($"Товар: {goodCell.Good.Name}, Кол-во: {goodCell.Count}");
            }
            ShowGoods(_shop.Warehouse.Goods);

            _shop.AddToCart(new GoodCell(_iPhone12, 4), new GoodCell(_iPhone11, 3));
            
            ShowGoods(_shop.Cart.Goods);
        }

        private void ShowGoods(IEnumerable<GoodCell> cells)
        {
            foreach (var goodCell in cells)
            {
                Console.WriteLine($"Товар: {goodCell.Good.Name}, Кол-во: {goodCell.Count}");
            }
        }
    }

    public class Shop
    {
        public Shop(GoodCellContainer warehouse, GoodCellContainer cart)
        {
            Warehouse = warehouse ?? throw new InvalidOperationException();
            Cart = cart ?? throw new InvalidOperationException();
        }

        public GoodCellContainer Warehouse { get; private set; }
        public GoodCellContainer Cart { get; private set; }

        public void AddToWarehouse(params GoodCell[] cells)
        {
            foreach (var cell in cells)
            {
                Warehouse.Add(cell);
            }
        }

        public void AddToCart(params GoodCell[] cells)
        {
            foreach (var cell in cells)
            {
                try
                {
                    Warehouse.Remove(cell);
                    Cart.Add(cell);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
        }
    }

    public class GoodCellContainer
    {
        private readonly List<GoodCell> _goods = new List<GoodCell>();
        public IEnumerable<GoodCell> Goods => _goods;

        public void Add(GoodCell goodCell)
        {
            GoodCell currentCell = _goods.FirstOrDefault(cell => cell.Good.Name == goodCell.Good.Name);

            if (currentCell.Good == null)
            {
                _goods.Add(goodCell);
            }
            else
            {
                currentCell.Merge(goodCell);
            }
        }

        public void Remove(GoodCell goodCell)
        {
            GoodCell currentCell = _goods.FirstOrDefault(cell => cell.Good.Name == goodCell.Good.Name);

            if (currentCell.Good == null)
                throw new Exception("Товара не существует");

            currentCell.Remove(goodCell);
        }
    }

    public struct GoodCell
    {
        public GoodCell(Good good, int count)
        {
            if (count < 0)
                throw new InvalidOperationException();
            if (good == null)
                throw new InvalidOperationException();

            Good = good;
            Count = count;
        }

        public Good Good { get; private set; }
        public int Count { get; private set; }

        public void Merge(GoodCell cell)
        {
            if (cell.Good.Name != Good.Name)
                throw new InvalidOperationException();

            Count += cell.Count;
        }

        public void Remove(GoodCell cell)
        {
            if (cell.Good.Name != Good.Name)
                throw new InvalidOperationException();
            if (cell.Count > Count)
                throw new InvalidOperationException();

            Count -= cell.Count;
        }
    }

    public class Good
    {
        public Good(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}