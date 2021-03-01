using DomainModels;
using System;
using System.Collections.Generic;

namespace Repositories
{
    public class DataContext
    {
        public IList<Item> itemList = new List<Item>
        {
            new Item { Id = 1, Text = "Item1"},
            new Item { Id = 2, Text = "Item2"},
            new Item { Id = 3, Text = "Item3"}
        };
    }
}
