using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Runtime.Internal;
using Data.Constants;
using Data.DataAccess;
using Data.Enums;
using Data.MongoCollections;
using Data.ViewModels;
using Google.Api.Gax;
using Mapster;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using Service.Utilities;
using SixLabors.ImageSharp;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Services.Core
{
    public interface ICartService
    {
        Task<ResultModel> AddToCart(string username, CartCreateModel model);
        Task<ResultModel> Update(string username, GetCartViewModel data);
        Task<ResultModel> Delete(string username, List<Guid> id);
        Task<ResultModel> GetCart(string username);

    }


    public class CartService : ICartService
    {
        private AppDbContext _context;
        private readonly IDistributedCache _cache;
        private readonly IConfiguration _configuration;
        private TimeSpan ttl;
        public CartService(AppDbContext context, IConfiguration configuration, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
            _configuration = configuration;
            ttl = TimeSpan.FromSeconds(double.Parse(_configuration["RedisTimeExpired"]));

        }


        public async Task<ResultModel> GetCart(string username)
        {
            var result = new ResultModel();

            try
            {
                var cacheData = await _cache.GetRecordAsync<GetCartViewModel>("Carts" + username);
                if (cacheData != null)
                {
                    result.Data = cacheData;
                } else
                {
                }
              
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null
                    ? e.InnerException.Message + "\n" + e.StackTrace
                    : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

    
        public async Task<ResultModel> AddToCart(string username, CartCreateModel model)
        {
            var result = new ResultModel();

            try
            {
            
                GetCartViewModel cart;
                var cacheData = await _cache.GetRecordAsync<GetCartViewModel>("Carts" + username);
                if(cacheData != null)
                {
                    cart = cacheData;

                    bool addMore = true;
                    foreach(var i in cart.Products)
                    {
                        if(i.Id == model.Product.Id)
                        {
                            i.Quantity += model.Product.Quantity;
                            addMore = false;
                        }
                    }
                    if (addMore)
                    {
                        cart.Products.Add(model.Product);
                    }
                } else
                {
                    cart = new GetCartViewModel();
               
                    cart.Products = new List<OrderProduct> { model.Product };
                }
               
               
                await _cache.SetRecordAsync("Carts" + username, cart, ttl);

                result.Data = cart;
                result.Succeed = true;

            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null
                    ? e.InnerException.Message + "\n" + e.StackTrace
                    : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> Update(string username, GetCartViewModel data)
        {
            var result = new ResultModel();
            try
            {
                var cacheData = await _cache.GetRecordAsync<GetCartViewModel>("Carts" + username);
                if (cacheData != null)
                {
                    await _cache.SetRecordAsync("Carts" + username, data);
                    result.Succeed = true;
                }
                else
                {
                    throw new Exception("The cart is empty");
                }


            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null
                    ? e.InnerException.Message + "\n" + e.StackTrace
                    : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }


        public async Task<ResultModel> Delete(string username, List<Guid> ids)
        {
            var result = new ResultModel();
            try
            {
                var cacheData = await _cache.GetRecordAsync<GetCartViewModel>("Carts" + username);
                if (cacheData != null)
                {
                    for (int i = 0; i < cacheData.Products.Count; i++)
                    {
                        foreach(var id in ids)
                        {
                            if (cacheData.Products[i].Id == id)
                            {

                                cacheData.Products.RemoveAt(i);
                            }
                        }
                       
                    }
                    await _cache.SetRecordAsync("Carts" + username, cacheData);
                    result.Succeed = true;
                }
                else
                {
                    throw new Exception("The cart is empty");
                }


            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null
                    ? e.InnerException.Message + "\n" + e.StackTrace
                    : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }
    }
}
