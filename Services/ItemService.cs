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

        //public async Task<object> Save(ItemDTO text)
        //{
        //    return await Task.FromResult<object>($"Inside ItemService Save method '{text}'.");
        //}

        public string Save(ItemDTO text)
        {
            return $"Inside ItemService Save method '{text.Text}'.";
        }
    }
}
