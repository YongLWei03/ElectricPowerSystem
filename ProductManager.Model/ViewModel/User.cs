﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManager.Model.ViewModel
{
    [Serializable]
    public class User
    {
        public string Account { get; set; }
        public string Password { get; set; }
    }
}
