using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Data.Enums;
using Data.MongoCollections;
using MongoDB.Bson.Serialization.Attributes;

namespace Data.ViewModels
{
    public class OrderItemModel
    {
        public Guid Id { get; set; }
        public bool IsDelete { get; set; }
        public int Status { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public Customer Customer { get; set; }

        public List<OrderProduct> Products { get; set; }
        public string? Address { get; set; }
        public string? ProvinceCode { get; set; }
        public string? DistrictCode { get; set; }
        public string? WardCode { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? DateExpected { get; set; }


    }

    public class OrderProductModel
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public OrderStatus Status { get; set; } = 0;
        public Guid Id { get; set; }
        public Guid OrderItemId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }

        public DateTime? DateExpected { get; set; }
    }

    public class GetOrderItemModel
    {
        public Guid Id { get; set; }

        public int Status { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public Customer Customer { get; set; }

        public DateTime? DateExpected { get; set; }
    }

    public class OrderItemUpdateModel
    {
        public Guid Id { get; set; }
        public bool IsDelete { get; set; }
        public int Status { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public Customer Customer { get; set; }
        public List<OrderProduct> Products { get; set; }
        public string? Address { get; set; }
        public string? ProvinceCode { get; set; }
        public string? PhoneNumber { get; set; }
        public string? DistrictCode { get; set; }
        public string? WardCode { get; set; }
        public DateTime? DateExpected { get; set; }
        public Site? Site { get; set; }
    }

    public class UpdateOrderStatusModel
    {
        public Guid OrderItemId { get; set; }
        public OrderStatus Status { get; set; }


    }

    public class UpdateOrderProductStatusModel
    {
        public Guid OrderItemId { get; set; }
        public List<ProductStatus> ProductsStatus { get; set; }
        
    }

    public class ProductStatus
    {
        public Guid ProductId { get; set; }
        public OrderStatus Status { get; set; }
    }

    public class OrderItemCreateModel
    {
        [Required]
        public Customer Customer { get; set; }

        [Required]
        public List<OrderProduct> Products { get; set; }

        public string? Address { get; set; }
        public string? ProvinceCode { get; set; }
        public string? DistrictCode { get; set; }
        public string? WardCode { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? DateExpected { get; set; }
        public Site? Site { get; set; }

    }

    public class OrderItemViewModel : OrderItemModel
    {
        public Site? Site { get; set; }
    }




    public class ItemModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
    }


    public class Customer
    {
        public Guid Id { get; set; }
        public string Fullname { get; set; }
      //  public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool? Gender { get; set; }
        public string? ProvinceCode { get; set; }
        public string? DistrictCode { get; set; }
        public string? WardCode { get; set; }
        //public string IC { get; set; }
        public string? Nation { get; set; }
        public string? PassportNumber { get; set; }
        //public string VaccinationCode { get; set; }
        //public string ExternalId { get; set; }
    }
}
