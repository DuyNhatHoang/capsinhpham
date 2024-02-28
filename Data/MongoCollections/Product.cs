using Balo.Data.MongoCollections;
using Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.MongoCollections
{
    public class Product : BaseMongoCollection
    { 
        public string Name { get; set; }
        public string Code { get; set; }
        public int Quantity { get; set; }
        public string UserGuide1 { get; set; }
        public string UserGuide2 { get; set; }
        public string Image { get; set; }
    }
}
