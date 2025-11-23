using Amazon.DynamoDBv2.DataModel;

namespace NiceAppApi.Utils.Auth
{
    [DynamoDBTable("Users")]
    public class UserModel
    {
        [DynamoDBHashKey]
        public string UserId { get; set; }  // primary key (Phone number)

        public string Name { get; set; }

        public string Password { get; set; }
        
        public string Type { get; set; }  // "user" or "admin"

        [DynamoDBRangeKey] // sort key
        public string Team { get; set; }
        
        public string Selfie { get; set; }  // URL image

        public short Status { get; set; }  // 0: Active, 1: Inactive
    }
}
