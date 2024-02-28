using System;
using System.Collections.Generic;
using System.Text;
using Balo.Data.MongoCollections;
using Data.Enums;
using Data.ViewModels;
using MongoDB.Bson.Serialization.Attributes;

namespace Data.MongoCollections
{
    public class OrderItem : BaseMongoCollection
    {
        public OrderStatus Status { get; set; } = OrderStatus.NewOrder;
      
        public Customer Customer { get; set; }
        public List<OrderProduct> Products { get; set; }
        public string? Address { get; set; }
        public string? ProvinceCode { get; set; }
        public string? DistrictCode { get; set; }
        public string? WardCode { get; set; }
        public Site? Site { get; set; }
        public DateTime? DateExpected { get; set; }

        public string? PhoneNumber { get; set; }

    }


    public class OrderProduct
    {
        public Guid Id { get; set; } = new Guid();
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string? Image { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.NewOrder;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateUpdated { get; set; } = DateTime.Now;
    }

    public class ProductSource
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
    }

}
