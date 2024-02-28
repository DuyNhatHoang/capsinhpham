using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.MongoCollections
{
    public class UserFirebaseToken
    {
        public Guid UserId { get; set; }
        public Guid Id { get; set; } = new Guid();
        public string Token { get; set; }
    }
}
