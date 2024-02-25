using System.Collections.Generic;

namespace Mamboo.Purchase.Scripts
{
    public class ProductsConstants
    {
        public static List<ProductItem> products = new List<ProductItem>
        {
            new ProductItem { productId = "com.example.noads", priceAndroid = 5.99, priceIos = 9.99 },
        };

        public class ProductItem
        {
            public double priceIos { get; set; }

            public double priceAndroid { get; set; }

            public string productId { get; set; }
        }
    }
}