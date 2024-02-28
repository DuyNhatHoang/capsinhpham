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
    public interface IOrderItemService
    {
        Task<ResultModel> AddToCart(string username, OrderItemCreateModel model);
        Task<ResultModel> CreateOrder(string username, OrderItemCreateModel model);
        Task<ResultModel> DeleteOrder(Guid id);
        Task<ResultModel> GetOrderItem( Guid? customerId, int? pageIndex, int? pageSize);
        Task<ResultModel> GetOrderItemByAdmin( Guid? customerId, OrderStatus? orderStatus, string? siteUsername, string? proviceCode, string? districtCode, string? wardCode, string? address, int? pageIndex, int? pageSize);
        Task<ResultModel> GetOrderItemBySite( Guid? customerId, OrderStatus? orderStatus, string? siteUsername, string? proviceCode, string? districtCode, string? wardCode, string? address, int? pageIndex, int? pageSize);
        Task<ResultModel> GetById(Guid id);
        Task<ResultModel> GetOrderItemByCustomer( string username,OrderStatus? orderStatus, string? siteUsername, string? proviceCode, string? districtCode, string? wardCode, string? address, int? pageIndex, int? pageSize);
        Task<ResultModel> UpdateOrder(OrderItemUpdateModel model);
        Task<ResultModel> UpdateOrderStatus(UpdateOrderStatusModel status);
        Task<ResultModel> UpdateAllProductsStatus(UpdateOrderStatusModel status);
        Task<ResultModel> UpdateOrderProductStatus(UpdateOrderProductStatusModel status);
        Task<ResultModel> GetOrderItemStatistic(DateTime? fromdate, DateTime? todate);
    }


    public class OrderItemService: IOrderItemService
    {
        private AppDbContext _context;
        private readonly IDistributedCache _cache;
        private readonly IConfiguration _configuration;
        public OrderItemService(AppDbContext context, IConfiguration configuration, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
            _configuration = configuration;


        }

        public async Task<ResultModel> CreateOrder(string username, OrderItemCreateModel model)
        {
            var result = new ResultModel();

            try
            {
                model.Customer.Username = username;
                if (!model.Products.Any())
                {
                    result.ErrorMessage = ErrorMessages.ITEMSOURCE_NOT_NULL + " AND " + ErrorMessages.ITEM_NOT_NULL;
                    return result;
                }

                var itemOrder = model.Adapt<OrderItem>();
                itemOrder.Status = OrderStatus.NewOrder;
                await _context.OrderItem.InsertOneAsync(itemOrder);
    



                result.Data = itemOrder;
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

        public async Task<ResultModel> DeleteOrder(Guid id)
        {
            var resultModel = new ResultModel();
            try
            {
                var filter = Builders<OrderItem>.Filter.Eq(x => x.Id, id);
                var isExisted = await
                    _context.OrderItem.Find(filter).FirstOrDefaultAsync();


                if (isExisted == null)
                {
                    throw new Exception(ErrorMessages.ID_NOT_FOUND);
                }

                var delete = Builders<OrderItem>
                    .Update.Set(x => x.IsDeleted, true);
                await _context.OrderItem.UpdateOneAsync(filter, delete);
                resultModel.Succeed = true;
            }
            catch (Exception e)
            {
                resultModel.Succeed = false;
                resultModel.ErrorMessage = e.Message;
            }
            return resultModel;
        }

        public async Task<ResultModel> GetById(Guid id)
        {
            var resultModel = new ResultModel();
            try
            {
                var filter = Builders<OrderItem>.Filter.Eq(x => x.Id, id);
                var isExisted = await
                    _context.OrderItem.Find(filter).FirstOrDefaultAsync();


                if (isExisted == null)
                {
                    throw new Exception(ErrorMessages.ID_NOT_FOUND);
                }

                resultModel.Succeed = true;
                resultModel.Data = isExisted;
            }
            catch (Exception e)
            {
                resultModel.Succeed = false;
                resultModel.ErrorMessage = e.Message;
            }
            return resultModel;
        }

        public async Task<ResultModel> GetOrderItem(Guid? customerId,int? pageIndex, int? pageSize)
        {
            var result = new ResultModel();

            try
            {
                var basefilter = Builders<OrderItem>.Filter.Empty;

        
                if (customerId.HasValue)
                {
                    basefilter = basefilter &
                                 Builders<OrderItem>.Filter.Eq(x => x.Customer.Id, customerId);
                }

                var data = _context.OrderItem.Find(basefilter);
                PagingModel paging = new PagingModel(pageIndex ?? 0, pageSize ?? 0, data.CountDocuments());

                var list = await data
                    .Skip((paging.PageIndex - 1) * paging.PageSize)
                    .Limit(paging.PageSize)
                    .ToListAsync();

                paging.Data = list.Adapt < List <GetOrderItemModel>>();
                result.Data = paging;
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

        public async Task<ResultModel> GetOrderItemByAdmin(Guid? customerId, OrderStatus? orderStatus, string? siteUsername, string? proviceCode, string? districtCode, string? wardCode, string? address, int? pageIndex, int? pageSize)
        {
            var result = new ResultModel();

            try
            {
                var basefilter = Builders<OrderItem>.Filter.Empty;
                basefilter = basefilter &
                                Builders<OrderItem>.Filter.Eq(x => x.IsDeleted, false);

                //if (!string.IsNullOrEmpty(siteUsername))
                //{
                //    basefilter = basefilter &
                //                 Builders<OrderItem>.Filter.Eq(x => x.Site.Username, siteUsername);
                //}
                if (customerId.HasValue)
                {
                    basefilter = basefilter &
                                 Builders<OrderItem>.Filter.Eq(x => x.Customer.Id, customerId);
                }
                if (orderStatus.HasValue)
                {
                    basefilter = basefilter &
                                 Builders<OrderItem>.Filter.Eq(x => x.Status, orderStatus);
                }
                if (!string.IsNullOrEmpty(proviceCode))
                {
                    basefilter = basefilter &
                                 Builders<OrderItem>.Filter.Eq(x => x.ProvinceCode, proviceCode);
                }
                if (!string.IsNullOrEmpty(districtCode))
                {
                    basefilter = basefilter &
                                 Builders<OrderItem>.Filter.Eq(x => x.DistrictCode, districtCode);
                }
                if (!string.IsNullOrEmpty(wardCode))
                {
                    basefilter = basefilter &
                                 Builders<OrderItem>.Filter.Eq(x => x.WardCode, wardCode);
                }
                if (!string.IsNullOrEmpty(address))
                {
                    basefilter = basefilter &
                                Builders<OrderItem>.Filter.Regex(x => x.Address, new BsonRegularExpression(".*" + address + ".*", "i"));
                }

                var data = _context.OrderItem.Find(basefilter);
                PagingModel paging = new PagingModel(pageIndex ?? 0, pageSize ?? 10, data.CountDocuments());

                var list = await data
                    .Skip((paging.PageIndex - 1) * paging.PageSize)
                    .Limit(paging.PageSize)
                    .ToListAsync();

                paging.Data = list.Adapt<List<OrderItemViewModel>>();
                result.Data = paging;
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

        public async Task<ResultModel> GetOrderItemByCustomer( string username, OrderStatus? orderStatus, string? siteUsername, string? proviceCode, string? districtCode, string? wardCode, string? address, int? pageIndex, int? pageSize)
        {
            var result = new ResultModel();

            try
            {
                var basefilter = Builders<OrderItem>.Filter.Empty;
                basefilter = basefilter &
                              Builders<OrderItem>.Filter.Eq(x => x.IsDeleted, false);

                //if (cboId.HasValue)
                //{
                //    basefilter = basefilter &
                //                 Builders<OrderItem>.Filter.Eq(x => x.CDO_Employee.EmployeeId, cboId.Tostring());
                //}
                basefilter = basefilter &
                             Builders<OrderItem>.Filter.Eq(x => x.Customer.Username, username);
                if (orderStatus.HasValue)
                {
                    basefilter = basefilter &
                                 Builders<OrderItem>.Filter.Eq(x => x.Status, orderStatus);
                }
                if (!string.IsNullOrEmpty(proviceCode))
                {
                    basefilter = basefilter &
                                 Builders<OrderItem>.Filter.Eq(x => x.ProvinceCode, proviceCode);
                }
                if (!string.IsNullOrEmpty(districtCode))
                {
                    basefilter = basefilter &
                                 Builders<OrderItem>.Filter.Eq(x => x.DistrictCode, districtCode);
                }
                if (!string.IsNullOrEmpty(wardCode))
                {
                    basefilter = basefilter &
                                 Builders<OrderItem>.Filter.Eq(x => x.WardCode, wardCode);
                }
                if (!string.IsNullOrEmpty(address))
                {
                    basefilter = basefilter &
                                Builders<OrderItem>.Filter.Regex(x => x.Address, new BsonRegularExpression(".*" + address + ".*", "i"));
                }

                var data = _context.OrderItem.Find(basefilter);
                PagingModel paging = new PagingModel(pageIndex ?? 0, pageSize ?? 10, data.CountDocuments());

                var list = await data
                    .Skip((paging.PageIndex - 1) * paging.PageSize)
                    .Limit(paging.PageSize)
                    .ToListAsync();

                paging.Data = list.Adapt<List<OrderItemViewModel>>();
                result.Data = paging;
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

        public async Task<ResultModel> GetOrderItemStatistic(DateTime? fromdate, DateTime? todate)
        {
            var resultModel = new ResultModel();
            try
            {
                var filters = Builders<OrderItem>.Filter.Empty;
                List<OrderProduct> products = new List<OrderProduct>();
                if (fromdate.HasValue)
                {
                    filters &= Builders<OrderItem>.Filter.Gte(x => x.DateCreated, fromdate.Value);
                }
                if (todate.HasValue)
                {
                    filters &= Builders<OrderItem>.Filter.Lte(x => x.DateCreated, todate.Value);
                }
                var existedItem = await _context.OrderItem.Find(filters).ToListAsync();
                if (existedItem == null)
                {
                    throw new Exception("The OrderItem not existed");
                }

                foreach(var i in existedItem)
                {
                    foreach(var j in i.Products)
                    {
                        if(j.Status == OrderStatus.OrderCompleted)
                        {
                            var tempProduct = j.Adapt<OrderProduct>();
                            //tempProduct.OrderItemId = i.Id;
                            products.Add(tempProduct);
                        }
                        
                    }    
                }

                resultModel.Succeed = true;
                resultModel.Data = products;
            }
            catch (Exception ex)
            {
                resultModel.Succeed = false;
                resultModel.ErrorMessage = ex.Message;

            }
            return resultModel;
        }

        public async Task<ResultModel> UpdateAllProductsStatus(UpdateOrderStatusModel status)
        {
            var resultModel = new ResultModel();
            try
            {
                var existedItem = await _context.OrderItem.Find(x => x.Id == status.OrderItemId).FirstOrDefaultAsync();
                if (existedItem == null)
                {
                    throw new Exception("The OrderItem not existed");
                }

                foreach (var i in existedItem.Products)
                {
                    i.Status = status.Status;
                    i.DateUpdated = DateTime.Now;
                }
                existedItem.DateUpdated = DateTime.Now;
                await _context.OrderItem.ReplaceOneAsync(x => x.Id == status.OrderItemId, existedItem);
                resultModel.Succeed = true;
                resultModel.Data = existedItem;
            }
            catch (Exception ex)
            {
                resultModel.Succeed = false;
                resultModel.ErrorMessage = ex.Message;

            }
            return resultModel;
        }

        public async Task<ResultModel> UpdateOrder(OrderItemUpdateModel model)
        {
            var resultModel = new ResultModel();
            try
            {
                var existedItem = await _context.OrderItem.Find(x => x.Id == model.Id).FirstOrDefaultAsync();
                if (existedItem == null)
                {
                    throw new Exception("The OrderItem not existed");
                }

                var newOrderItem = model.Adapt(existedItem);
                newOrderItem.DateUpdated = DateTime.Now;

                await _context.OrderItem.ReplaceOneAsync(x => x.Id == model.Id, newOrderItem);
                resultModel.Succeed = true;
                resultModel.Data = newOrderItem;
            }
            catch (Exception ex)
            {
                resultModel.Succeed = false;
                resultModel.ErrorMessage = ex.Message;

            }
            return resultModel;
        }

        public async Task<ResultModel> UpdateOrderProductStatus(UpdateOrderProductStatusModel status)
        {
            var resultModel = new ResultModel();
            try
            {
                var existedItem = await _context.OrderItem.Find(x => x.Id == status.OrderItemId).FirstOrDefaultAsync();
                if (existedItem == null)
                {
                    throw new Exception("The OrderItem not existed");
                }

                foreach(var i in existedItem.Products)
                {
                    foreach(var j in status.ProductsStatus)
                    {
                        if(i.Id == j.ProductId)
                        {
                            i.Status = j.Status;
                        }
                    }
                }
                existedItem.DateUpdated = DateTime.Now;
                await _context.OrderItem.ReplaceOneAsync(x => x.Id == status.OrderItemId, existedItem);
                resultModel.Succeed = true;
                resultModel.Data = existedItem;
            }
            catch (Exception ex)
            {
                resultModel.Succeed = false;
                resultModel.ErrorMessage = ex.Message;

            }
            return resultModel;
        }
    
        public async Task<ResultModel> UpdateOrderStatus(UpdateOrderStatusModel status)
        {
            var resultModel = new ResultModel();
            try
            {
                var existedItem = await _context.OrderItem.Find(x => x.Id == status.OrderItemId).FirstOrDefaultAsync();
                if (existedItem == null)
                {
                    throw new Exception("The OrderItem not existed");
                }
                //switch (existedItem.Status)
                //{
                //    case OrderStatus.NewOrder:
                //        if (status.Status != OrderStatus.OrderReceived && status.Status != OrderStatus.Cancelled && status.Status != OrderStatus.Other)
                //        {
                //            throw new Exception("This status not accepted");
                //        }
                //        break;
                //    case OrderStatus.OrderReceived:
                //        if (status.Status != OrderStatus.OrderDelivered && status.Status != OrderStatus.Cancelled && status.Status != OrderStatus.Other)
                //        {
                //            throw new Exception("This status not accepted");
                //        }
                //        break;
                //    case OrderStatus.OrderDelivered:
                //        if (status.Status != OrderStatus.OrderCompleted && status.Status != OrderStatus.CustomerReceived && status.Status != OrderStatus.Cancelled && status.Status != OrderStatus.Other)
                //        {
                //            throw new Exception("This status not accepted");
                //        }
                //        break;
                //    case OrderStatus.OrderCompleted:
                //        if (status.Status != OrderStatus.OrderReceived && status.Status != OrderStatus.Cancelled && status.Status != OrderStatus.Other)
                //        {
                //            throw new Exception("This status can not update anymore");
                //        }
                //        break;
                //}
                existedItem.Status = status.Status;
                existedItem.DateUpdated = DateTime.Now;
                await _context.OrderItem.ReplaceOneAsync(x => x.Id == status.OrderItemId, existedItem);
                resultModel.Succeed = true;
                resultModel.Data = existedItem;
            }
            catch (Exception ex)
            {
                resultModel.Succeed = false;
                resultModel.ErrorMessage = ex.Message;

            }
            return resultModel;
        }

        public async Task<ResultModel> AddToCart(string username, OrderItemCreateModel model)
        {
            var result = new ResultModel();

            try
            {
                model.Customer.Username = username;
                if (!model.Products.Any())
                {
                    result.ErrorMessage = ErrorMessages.ITEMSOURCE_NOT_NULL + " AND " + ErrorMessages.ITEM_NOT_NULL;
                    return result;
                }

                var itemOrder = model.Adapt<OrderItem>();
                itemOrder.Status = OrderStatus.NewOrder;
                await _cache.SetRecordAsync("Cart", itemOrder);
                result.Data = itemOrder;
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

        public async Task<ResultModel> GetOrderItemBySite(Guid? customerId, OrderStatus? orderStatus, string? siteUsername, string? proviceCode, string? districtCode, string? wardCode, string? address, int? pageIndex, int? pageSize)
        {
            var result = new ResultModel();

            try
            {
                var basefilter = Builders<OrderItem>.Filter.Empty;
                basefilter = basefilter &
                                Builders<OrderItem>.Filter.Eq(x => x.IsDeleted, false);

                if (!string.IsNullOrEmpty(siteUsername))
                {
                    basefilter = basefilter &
                                 Builders<OrderItem>.Filter.Eq(x => x.Site.Username, siteUsername);
                }
                if (customerId.HasValue)
                {
                    basefilter = basefilter &
                                 Builders<OrderItem>.Filter.Eq(x => x.Customer.Id, customerId);
                }
                if (orderStatus.HasValue)
                {
                    basefilter = basefilter &
                                 Builders<OrderItem>.Filter.Eq(x => x.Status, orderStatus);
                }
                if (!string.IsNullOrEmpty(proviceCode))
                {
                    basefilter = basefilter &
                                 Builders<OrderItem>.Filter.Eq(x => x.ProvinceCode, proviceCode);
                }
                if (!string.IsNullOrEmpty(districtCode))
                {
                    basefilter = basefilter &
                                 Builders<OrderItem>.Filter.Eq(x => x.DistrictCode, districtCode);
                }
                if (!string.IsNullOrEmpty(wardCode))
                {
                    basefilter = basefilter &
                                 Builders<OrderItem>.Filter.Eq(x => x.WardCode, wardCode);
                }
                if (!string.IsNullOrEmpty(address))
                {
                    basefilter = basefilter &
                                Builders<OrderItem>.Filter.Regex(x => x.Address, new BsonRegularExpression(".*" + address + ".*", "i"));
                }

                var data = _context.OrderItem.Find(basefilter);
                PagingModel paging = new PagingModel(pageIndex ?? 0, pageSize ?? 10, data.CountDocuments());

                var list = await data
                    .Skip((paging.PageIndex - 1) * paging.PageSize)
                    .Limit(paging.PageSize)
                    .ToListAsync();

                paging.Data = list.Adapt<List<OrderItemViewModel>>();
                result.Data = paging;
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
    }
}
