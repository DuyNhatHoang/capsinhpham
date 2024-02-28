using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Constants;
using Data.DataAccess;
using Data.Enums;
using Data.MongoCollections;
using Data.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Driver;
using Service.Utilities;
using static System.Net.Mime.MediaTypeNames;
using static Data.ViewModels.GetProductModel;

namespace Services.Core
{
    public interface IProductService
    {
        Task<ResultModel> Create(CreateProductModel model);
        Task<ResultModel> UploadImageFile(IFormFile file, Guid productId);
        Task<ResultModel> Delete(Guid id);
        Task<ResultModel> GetById(Guid id);
        Task<ResultModel> GetProducts(GetProductModel model, int? pageIndex, int? pageSize);
        Task<ResultModel> UpdateProduct(UpdateProductModel model);
    }


    public class ProductService : IProductService
    {
        private AppDbContext _context;


        public ProductService(AppDbContext context)
        {
            _context = context;


        }

        public async Task<ResultModel> Create(CreateProductModel model)
        {
            var result = new ResultModel();

            try
            {
                var filter = Builders<Product>.Filter.Eq(x => x.Code, model.Code);
                var isExisted = await
                    _context.Products.Find(filter).FirstOrDefaultAsync();
                if(isExisted != null)
                {
                    throw new Exception(ErrorMessages.ITEM_NOT_NULL);
                }

                var product = model.Adapt<Product>();
                await _context.Products.InsertOneAsync(product);


                result.Data = product;
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

        public async Task<ResultModel> Delete(Guid id)
        {
            var resultModel = new ResultModel();
            try
            {
                var filter = Builders<Product>.Filter.Eq(x => x.Id, id);
                var isExisted = await
                    _context.Products.Find(filter).FirstOrDefaultAsync();


                if (isExisted == null)
                {
                    throw new Exception(ErrorMessages.ID_NOT_FOUND);
                }

                var delete = Builders<Product>
                    .Update.Set(x => x.IsDeleted, true);
                await _context.Products.UpdateOneAsync(filter, delete);
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
                var filter = Builders<Product>.Filter.Eq(x => x.Id, id);
                var isExisted = await
                    _context.Products.Find(filter).FirstOrDefaultAsync();


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

        public async Task<ResultModel> GetProducts(GetProductModel model, int? pageIndex, int? pageSize)
        {
            var result = new ResultModel();

            try
            {
                var basefilter = Builders<Product>.Filter.Empty;


                if (!String.IsNullOrEmpty(model.Code))
                {
                    basefilter = basefilter &
                                 Builders<Product>.Filter.Regex(x => x.Name, new BsonRegularExpression(".*" + model.Code + ".*", "i"));
                }

                if (!String.IsNullOrEmpty(model.Name))
                {
                    basefilter = basefilter &
                                 Builders<Product>.Filter.Regex(x => x.Code, new BsonRegularExpression(".*" + model.Name + ".*", "i"));
                }

                basefilter = basefilter &
                               Builders<Product>.Filter.Eq(x => x.IsDeleted, false);
                var data = _context.Products.Find(basefilter);
                PagingModel paging = new PagingModel(pageIndex ?? 0, pageSize ?? 0, data.CountDocuments());

                var list = await data
                    .Skip((paging.PageIndex - 1) * paging.PageSize)
                    .Limit(paging.PageSize)
                    .ToListAsync();

                paging.Data = list.Adapt<List<Product>>();
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

        public async Task<ResultModel> UpdateProduct(UpdateProductModel model)
        {
            var resultModel = new ResultModel();
            try
            {
                var existedItem = await _context.Products.Find(x => x.Id == model.Id).FirstOrDefaultAsync();
                if (existedItem == null)
                {
                    throw new Exception("The OrderItem not existed");
                }

                var newItem = model.Adapt(existedItem);

                newItem.DateUpdated = DateTime.Now;
                await _context.Products.ReplaceOneAsync(x => x.Id == model.Id, newItem);
                resultModel.Succeed = true;
                resultModel.Data = newItem;
            }
            catch (Exception ex)
            {
                resultModel.Succeed = false;
                resultModel.ErrorMessage = ex.Message;

            }
            return resultModel;
        }

        public async Task<ResultModel> UploadImageFile(IFormFile image, Guid productId)
        {
            var resultModel = new ResultModel();
            int targetWidth = 450;
            try
            {
                var base64Image = FormFileImageProcessing.ResizeAndEncodeToBase64(image);
                var existedItem = await _context.Products.Find(x => x.Id == productId).FirstOrDefaultAsync();
                if (existedItem == null)
                {
                    throw new Exception("The OrderItem not existed");
                }
                existedItem.Image = base64Image;
                await _context.Products.ReplaceOneAsync(x => x.Id == productId, existedItem);

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
    }
}
