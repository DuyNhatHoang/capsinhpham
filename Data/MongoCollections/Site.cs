﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.MongoCollections
{
    public class Site
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
    }
}
