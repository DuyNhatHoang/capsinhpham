using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.ViewModels
{
    public class GetProductModel
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
    }
    public class CreateProductModel
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Code { get; set; }
        public int Quantity { get; set; }
        public string? UserGuide1 { get; set; }
        public string? UserGuide2 { get; set; }
        public string? Image { get; set; }
    }

    public class UpdateProductModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public string? UserGuide1 { get; set; }
        public string? UserGuide2 { get; set; }
        public string? Image { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; } = DateTime.Now;
    }
}
