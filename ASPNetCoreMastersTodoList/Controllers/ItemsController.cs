using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Services;
using Microsoft.Extensions.Logging;
using System;
using ASPNetCoreMastersTodoList.Api.ApiModels;
using Services.DTO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ASPNetCoreMastersTodoList.Api.Controllers
{
    
    public class ItemsController : ControllerBase
    {
        private readonly ILogger<ItemsController> _logger;

        public ItemsController(ILogger<ItemsController> logger)
        {
            _logger = logger;
        }

        // 3. Change return type of Get action method to int.
        [HttpGet("{id:int}")]
        public int Get(int id)
        {
            //throw new Exception(); 

            var service = new ItemService();
            var result = service.GetAll(id);

            return result;
        }

        public void Post(ItemCreateApiModel itemCreateApiModel)
        {

        }

        //5. Create an action method in the items controller that accepts ItemCreateApiModel 
        //object and is mapped to an ItemDTO object for the ItemService Save method to consume

        public IActionResult ItemsCreate(ItemCreateApiModel itemCreateApiModel)
        {
            var mappedObj = new ItemDTO
            {
                Text = itemCreateApiModel.Text
            };

            var service = new ItemService();
            var result = service.Save(mappedObj);
           
            return Ok(result);
        }
    }
}
