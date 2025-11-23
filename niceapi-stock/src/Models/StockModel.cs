using Amazon.DynamoDBv2.DataModel;

namespace NiceApiStock.Models
{
    [DynamoDBTable("Stock")]
    public class StockModel
    {
        [DynamoDBHashKey]
        public required string UserId { get; set; }

        [DynamoDBRangeKey]
        public required string Id { get; set; }

        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Image { get; set; }
        public decimal Price { get; set; }

        [DynamoDBGlobalSecondaryIndexHashKey("TypeIndex")]
        public required string Type { get; set; }

        public int Count { get; set; }
    }
}