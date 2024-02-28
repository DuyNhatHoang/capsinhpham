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
using Data.Enums;

namespace Booking_Service_App.Controllers
{
    [Route("api/orderItems")]
   // [Authorize(AuthenticationSchemes = "Bearer")]

    [ApiController]
    public class OrderItemController : ControllerBase
    {
        private IOrderItemService _orderItemService;

        public OrderItemController(IOrderItemService orderItemService)
        {
            _orderItemService = orderItemService;
        }

        //[HttpGet]
        //public async Task<IActionResult> Get(Guid? cboId,Guid? customerId, int? pageIndex, int? pageSize)
        //{
        //    try
        //    {
        //        var result = await _orderItemService.GetOrderItem(cboId, customerId, pageIndex, pageSize);
        //        if (result.Succeed) return Ok(result.Data);
        //        return BadRequest(result);
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(e.Message);
        //    }
        //}

        [HttpGet("GetByAdmin")]
        public async Task<IActionResult> GetByAdmin( Guid? customerId, OrderStatus? orderStatus, String? proviceCode, String? districtCode, String? wardCode, String? address, int? pageIndex, int? pageSize)
        {
            try
            {
                var username = User.Claims.GetUserName();
                var result = await _orderItemService.GetOrderItemByAdmin( customerId, orderStatus, username, proviceCode, districtCode, wardCode, address, pageIndex, pageSize);
                if (result.Succeed) return Ok(result.Data);
                return BadRequest(result);
            }
            catch (Exception e)
            {   
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetBySite")]
        public async Task<IActionResult> GetBySite(Guid? customerId, OrderStatus? orderStatus, String? proviceCode, String? districtCode, String? wardCode, String? address, int? pageIndex, int? pageSize)
        {
            try
            {
                var username = User.Claims.GetUserName();
                var result = await _orderItemService.GetOrderItemBySite(customerId, orderStatus, username, proviceCode, districtCode, wardCode, address, pageIndex, pageSize);
                if (result.Succeed) return Ok(result.Data);
                return BadRequest(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetByCustomer")]
        public async Task<IActionResult> GetByCustomer(OrderStatus? orderStatus,String? proviceCode, String? districtCode, String? wardCode, String? address, int? pageIndex, int? pageSize)
        {
            try
            {
                var username = User.Claims.GetUserName();
                var result = await _orderItemService.GetOrderItemByCustomer(username, orderStatus, username, proviceCode, districtCode, wardCode, address, pageIndex, pageSize);
                if (result.Succeed) return Ok(result.Data);
                return BadRequest(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetCompletedProductStatistic")]
        public async Task<IActionResult> GetOrderItemStatistic(DateTime? fromdate, DateTime? todate)
        {
            try
            {
                var username = User.Claims.GetUserName();
                var result = await _orderItemService.GetOrderItemStatistic(fromdate, todate);
                if (result.Succeed) return Ok(result.Data);
                return BadRequest(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var result = await _orderItemService.GetById(id);
                if (result.Succeed) return Ok(result.Data);
                return BadRequest(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(OrderItemCreateModel model)
        {
            try
            {
                var username = User.Claims.GetUserName();
                var result = await _orderItemService.CreateOrder(username, model);
                if (result.Succeed) return Ok(result.Data);
                return BadRequest(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpPut("UpdateStatus")]   
        public async Task<IActionResult> UpdateStatus(UpdateOrderStatusModel model)
        {
            try
            {
                var result = await _orderItemService.UpdateOrderStatus(model);
                if (result.Succeed) return Ok(result.Data);
                return BadRequest(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("UpdateProductsStatus")]
        public async Task<IActionResult> UpdateProductsStatus(UpdateOrderProductStatusModel model)
        {
            try
            {
                var result = await _orderItemService.UpdateOrderProductStatus(model);
                if (result.Succeed) return Ok(result.Data);
                return BadRequest(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("UpdateAllProductsStatus")]
        public async Task<IActionResult> UpdateAllProductsStatus(UpdateOrderStatusModel model)
        {
            try
            {
                var result = await _orderItemService.UpdateAllProductsStatus(model);
                if (result.Succeed) return Ok(result.Data);
                return BadRequest(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateOrderItem(OrderItemUpdateModel model)
        {
            try
            {
                var result = await _orderItemService.UpdateOrder(model);
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
                var result = await _orderItemService.DeleteOrder(id);
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
