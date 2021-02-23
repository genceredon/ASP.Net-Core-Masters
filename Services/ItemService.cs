using Microsoft.AspNetCore.Mvc;
using Services.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class ItemService
    {
        public int Get(int userId)
        {
            Console.WriteLine($"Inside Get by id funtion {userId}");
            return userId;
        }

        public string GetAll()
        {
            return $"Inside GetAll() function";
        }

        public string GetByFilters(Dictionary<string, string> filters)
        {
            string result = string.Empty;
            foreach (var items in filters)
            {
                result = $"Key: {items.Key}, Value: {items.Value}";
            }

            return result;
        }

        public string Post(ItemDTO text)
        {
            var result = text.Text;
            return result;
        }

        public string Put(int id, ItemDTO text)
        {
            var result = $"{id}, { text.Text}";
            return result;
        }

        public string Delete(int id)
        {
            var result = $"{id}";

            return result;
        }

        public string Save(ItemDTO text)
        {
            var itemObj = new ItemDTO();
            var result = itemObj.MappedObj(text);

            return $"Inside ItemService Save method '{result.Items}'.";
        }
    }
}
