using System;
using System.Collections.Generic;

namespace Services
{
    public class ItemService
    {
        public IEnumerable<string> GetAll(int userId)
        {
            return new string[] { $"Inside ItemService: value1 '{userId}'", "value2" };
        }
    }
}
