
using CorePush.Google;
using Data.DataAccess;
using Data.MongoCollections;
using Data.ViewModels;
using FirebaseAdmin.Messaging;
using Mapster;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Core.Servers;
using Nancy.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static Data.ViewModels.GoogleNotification;

namespace Services.Core
{
    public interface IFCMService
    {
        Task<ResponseModel> SendNotification(NotificationModel notificationModel);
        Task<ResponseModel> SendNotificationForWeb(WebNotificationModel notificationModel);
        Task<ResultModel> SetFirebaseToken(CreateUserFirebaseToken userFirebaseTokens);
    }

    public class FCMService : IFCMService
    {
        private readonly FcmNotificationSetting _fcmNotificationSetting;
        private AppDbContext _dbContext;
            public FCMService(IOptions<FcmNotificationSetting> settings, AppDbContext dbContext)
        {
            _fcmNotificationSetting = settings.Value;
                _dbContext = dbContext;
        }

        public async Task<ResponseModel> SendNotification(NotificationModel notificationModel)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                if (notificationModel.IsAndroiodDevice)
                {
                    /* FCM Sender (Android Device) */
                    FcmSettings settings = new FcmSettings()
                    {
                        SenderId = _fcmNotificationSetting.SenderId,
                        ServerKey = _fcmNotificationSetting.ServerKey
                    };
                    HttpClient httpClient = new HttpClient();

                    string authorizationKey = string.Format("keyy={0}", settings.ServerKey);
                    string deviceToken = notificationModel.DeviceId;

                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorizationKey);
                    httpClient.DefaultRequestHeaders.Accept
                            .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    DataPayload dataPayload = new DataPayload();
                    dataPayload.Title = notificationModel.Title;
                    dataPayload.Body = notificationModel.Body;

                    GoogleNotification notification = new GoogleNotification();
                    notification.Data = dataPayload;
                    notification.Notification = dataPayload;

                    var fcm = new FcmSender(settings, httpClient);
                    var fcmSendResponse = await fcm.SendAsync(deviceToken, notification);

                    if (fcmSendResponse.IsSuccess())
                    {
                        response.IsSuccess = true;
                        response.Message = "Notification sent successfully";
                        return response;
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = fcmSendResponse.Results[0].Error;
                        return response;
                    }
                }
                else
                {
                    /* Code here for APN Sender (iOS Device) */
                    //var apn = new ApnSender(apnSettings, httpClient);
                    //await apn.SendAsync(notification, deviceToken);
                }
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Something went wrong";
                return response;
            }
        }

        public async Task<ResponseModel> SendNotificationForWeb(WebNotificationModel notificationModel)
        {
        string serverKey = "AAAAi4wzZac:APA91bFTOhQpWG4noQVKN6THOtRcwfforwoGv7brZ87oO7hbABYilrL8O2oJUaZb_mIDwGqrluQidsPNyxzGEBOHmpUEY1FwQ-U_tGBMnV5_vjm8ukjBvfrB0G4Zo6uH8_TAkK3Ax1TQ";
        string senderId = "599352632743";
        string webAddr = "https://fcm.googleapis.com/fcm/send";
            var filter = Builders<UserFirebaseToken>.Filter.Eq(x => x.UserId, notificationModel.UserId);
            var userToken = await _dbContext.UserFirebaseTokens.Find(filter).FirstOrDefaultAsync();
            if(userToken == null)
            {
                throw new Exception("This user dont have the firebase token to send");
            }
        ResponseModel response = new ResponseModel();
            var result = "-1";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Headers.Add(string.Format("Authorization: key={0}", serverKey));
            httpWebRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
            httpWebRequest.Method = "POST";

            var payload = new
            {
                to = userToken.Token,
                priority = "high",
                content_available = true,
                notification = new
                {
                    body = notificationModel.Body,
                    title = notificationModel.Title
                },
            };
            var serializer = new JavaScriptSerializer();
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = serializer.Serialize(payload);
                streamWriter.Write(json);
                streamWriter.Flush();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
                response.IsSuccess = true;
                response.Message = result;

            return response;
        }

            public async Task<ResultModel> SetFirebaseToken(CreateUserFirebaseToken userFirebaseTokens)
            {
                ResultModel result = new ResultModel();
                try
                {
                var filter = Builders<UserFirebaseToken>.Filter.Eq(x => x.UserId, userFirebaseTokens.UserId);
                var userToken = await _dbContext.UserFirebaseTokens.Find(filter).FirstOrDefaultAsync();
                if (userToken != null)
                {
                    await _dbContext.UserFirebaseTokens.ReplaceOneAsync(x => x.UserId == userFirebaseTokens.UserId, userFirebaseTokens.Adapt(userToken));
                } else
                {
                    await _dbContext.UserFirebaseTokens.InsertOneAsync(userFirebaseTokens.Adapt<UserFirebaseToken>());
                }
             
                    result.Succeed = true;
                } catch(Exception ex)
                {
                    result.Succeed = false;
                    result.ErrorMessage = ex.Message;
                }

                return result;
            }
        }
}
