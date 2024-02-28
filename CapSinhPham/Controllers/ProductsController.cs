using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Services.Core;
using CapSinhPham.Extensions;

namespace Booking_Service_App.Controllers
{
    [Route("api/products")]
    //[Authorize(AuthenticationSchemes = "Bearer")]

    [ApiController]
    public class ProductController : ControllerBase
    {
        private IProductService _service;

        public ProductController(IProductService productService)
        {
            _service = productService;
        }


        [HttpGet("GetProducts")]
        public async Task<IActionResult> GetProducts([FromQuery]GetProductModel model, int? pageIndex, int? pageSize)
        {
            try
            {
                var result = await _service.GetProducts(model, pageIndex, pageSize);
                if (result.Succeed) return Ok(result.Data);
                return BadRequest(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetProductById")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            try
            {
                var result = await _service.GetById(id);
                if (result.Succeed) return Ok(result.Data);
                return BadRequest(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        //[HttpGet("GetById")]
        //public async Task<IActionResult> GetById(Guid id)
        //{
        //    try
        //    {
        //        var result = await _service.get(cboId, username, pageIndex, pageSize);
        //        if (result.Succeed) return Ok(result.Data);
        //        return BadRequest(result);
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(e.Message);
        //    }
        //}


        [HttpPost]
        public async Task<IActionResult> Add(CreateProductModel model)
        {
            try
            {
                var username = User.Claims.GetUserName();
                var result = await _service.Create(model);
                if (result.Succeed) return Ok(result.Data);
                return BadRequest(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [RequestFormLimits(MultipartBodyLengthLimit = 200000000)]
        [DisableRequestSizeLimit]
        [Consumes("multipart/form-data")]
        [HttpPut("UploadFile")]
        public async Task<IActionResult> UploadProductImage(IFormFile file, Guid id)
        {
            try
            {
                var result = await _service.UploadImageFile(file, id);
                if (result.Succeed) return Ok(result.Data);
                return BadRequest(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpPut]
        public async Task<IActionResult> Update(UpdateProductModel model)
        {
            try
            {
                var result = await _service.UpdateProduct(model);
                if (result.Succeed) return Ok(result.Data);
                return BadRequest(result);
            }   
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _service.Delete(id);
                if (result.Succeed) return Ok(result.Data);
                return BadRequest(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
