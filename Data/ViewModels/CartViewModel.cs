using Data.Enums;
using Data.MongoCollections;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.ViewModels
{

    public  class GetCartViewModel
    {
        public List<OrderProduct> Products { get; set; }
    }

    public class CartOrderModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public OrderStatus Status { get; set; } = OrderStatus.NewOrder;
        public List<OrderProduct> Products { get; set; }  

        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateUpdated { get; set; } = DateTime.Now;
    }

        public class CartCreateModel
        {

            [Required]
              public OrderProduct Product { get; set; } 

        }

    }
