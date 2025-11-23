using System.ComponentModel.DataAnnotations;
using Amazon.DynamoDBv2.DataModel;

namespace NiceAppApi.Products
{
    [DynamoDBTable("Products")]
    public class ProductModel
    {
        [DynamoDBHashKey]
        public string IdProduct { get; set; }
        
        public string Type { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public string Image { get; set; }
        
        public decimal Price { get; set; }
    }
}
