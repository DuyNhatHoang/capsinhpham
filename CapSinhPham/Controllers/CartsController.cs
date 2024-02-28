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
using Data.MongoCollections;

namespace Booking_Service_App.Controllers
{
    [Route("api/carts")]
   // [Authorize(AuthenticationSchemes = "Bearer")]

    [ApiController]
    public class CartsController : ControllerBase
    {
        private ICartService _service;

        public CartsController(ICartService orderItemService)
        {
            _service = orderItemService;
        }

       
        [HttpPost]
        public async Task<IActionResult> Add(CartCreateModel model)
        {
            try
            {
                var username = User.Claims.GetUserName();
                var result = await _service.AddToCart(username, model);
                if (result.Succeed) return Ok(result.Data);
                return BadRequest(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var username = User.Claims.GetUserName();
                var result = await _service.GetCart(username);
                    if (result.Succeed) return Ok(result.Data);
                return BadRequest(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(GetCartViewModel item)
        {
            try
            {
                var username = User.Claims.GetUserName();
                var result = await _service.Update(username, item);
                if (result.Succeed) return Ok(result.Data);
                return BadRequest(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(List<Guid> id)
        {
            try
            {
                var username = User.Claims.GetUserName();
                var result = await _service.Delete(username, id);
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
