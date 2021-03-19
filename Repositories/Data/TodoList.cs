using System;
using System.Collections.Generic;

namespace Repositories.Data
{
    public class TodoList
    {
        public int Id { get; set; }
        public string Todo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
