using Microsoft.AspNetCore.Mvc;
using Services.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class ItemService
    {
        public int GetAll(int userId)
        {
            return userId;
        }

        public string Save(ItemDTO text)
        {
            var itemObj = new ItemDTO();
            var result = itemObj.MappedObj(text);

            return $"Inside ItemService Save method '{result.Items}'.";
        }
    }
}
