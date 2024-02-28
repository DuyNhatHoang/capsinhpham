using System;
using Data.Enums;
using Data.ViewModels;
using MongoDB.Bson.Serialization.Attributes;

namespace Data.MongoCollections
{
    public class WorkingSession
    {
        [BsonId()]
        public Guid Id { get; set; } = Guid.NewGuid();
        public bool IsDelete { get; set; }
        public DateTime DateCreate { get; set; } = DateTime.Now;
        public DateTime DateUpdate { get; set; }
        public Guid? ReferTicketId { get; set; }
        public Customer Customer { get; set; }
        public Session Session { get; set; }
        public SessionContent SessionContent { get; set; }

    }

    public class Session
    {
        public string WorkingLocation { get; set; }
        public DateTime WorkingDate { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
    }

    public class SessionContent
    {

        public bool IsConsulstation { get; set; }
        public SesstionType Type { get; set; }
        public string Surrogate { get; set; }
        public string Note { get; set; }
        public string Code { get; set; }
        public string ResultTestingId { get; set; }
        public Guid TestingHistoryId { get; set; }
        public string Result { get; set; }
        public string FeedbackFromHospital { get; set; }
        public string ToUnitId { get; set; }
        public string FormId { get; set; }
        public int ConsulstationType { get; set; }

        public int RequestCustomerStatus { get; set; }
        public int RequestFacilityStatus { get; set; }
        public Items Items { get; set; }


    }

    public class Items 
    {
        public List<Product> ListItem { get; set; }
        public List<ProductSource> ListItemSource { get; set; }

    }


}
