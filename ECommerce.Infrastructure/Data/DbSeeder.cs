using ECommerce.Core.Models;

namespace ECommerce.Infrastructure.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext context)
    {
        if (context.Products.Any()) return; // already seeded

        var electronics = new Category { Name = "Electronics" };
        var apparel = new Category { Name = "Apparel" };
        var home = new Category { Name = "Home & Kitchen" };
        var sports = new Category { Name = "Sports & Outdoors" };

        context.Categories.AddRange(electronics, apparel, home, sports);

        var products = new List<Product>
        {
            new() { Name = "Wireless Over-Ear Headphones", Description = "Noise-cancelling bluetooth headphones with 30-hour battery life.", Price = 129.99m, Tags = "wireless,bluetooth,audio,headphones", Category = electronics, ImageUrl = "https://pixabay.com/images/download/infinitefantasy-headphones-8310796_1920.jpg" },
            new() { Name = "Bluetooth Earbuds", Description = "Compact true-wireless earbuds with charging case.", Price = 59.99m, Tags = "wireless,bluetooth,audio,earbuds", Category = electronics, ImageUrl = "https://pisces.bbystatic.com/image2/BestBuy_US/images/products/6533/6533939_sd.jpg" },
            new() { Name = "Portable Bluetooth Speaker", Description = "Waterproof speaker with rich bass, perfect for outdoors.", Price = 44.99m, Tags = "wireless,bluetooth,audio,speaker,outdoor", Category = electronics, ImageUrl = "https://m.media-amazon.com/images/I/718yxonHN8L._AC_.jpg" },
            new() { Name = "Smartwatch Series 5", Description = "Fitness tracking smartwatch with heart-rate monitor.", Price = 199.99m, Tags = "wearable,fitness,bluetooth", Category = electronics, ImageUrl = "https://tse1.mm.bing.net/th/id/OIP.3ToNhz4zx_gb8Z_Vy97A4wHaI0?r=0&rs=1&pid=ImgDetMain&o=7&rm=3" },
            new() { Name = "4K Action Camera", Description = "Rugged waterproof camera for adventure sports.", Price = 149.99m, Tags = "camera,outdoor,adventure", Category = electronics, ImageUrl = "https://tse1.mm.bing.net/th/id/OIP.2RGOY_sbB4LTSiEoc2cVhwHaHa?r=0&rs=1&pid=ImgDetMain&o=7&rm=3" },

            new() { Name = "Men's Running Jacket", Description = "Lightweight windproof jacket for running in any weather.", Price = 79.99m, Tags = "running,outdoor,jacket,men", Category = apparel, ImageUrl = "https://i5.walmartimages.com/seo/SSAAVKUY-Men-s-Running-Jacket-Lightweight-Water-Resistant-Windbreaker-Hoodie-Zip-Up-Workout-Sportswear-Activewear-Blue-14_e4f2874d-98aa-4cf8-b4cf-10844838774b.4844fdb0713670dbc59dd5c8c1c10dbf.jpeg" },
            new() { Name = "Women's Yoga Leggings", Description = "High-waisted, moisture-wicking leggings for yoga and gym.", Price = 39.99m, Tags = "yoga,fitness,leggings,women", Category = apparel, ImageUrl = "https://th.bing.com/th/id/OIP.gAZNxyUbp1T_1r2c97g_ewHaJk?r=0&o=7rm=3&rs=1&pid=ImgDetMain&o=7&rm=3" },
            new() { Name = "Merino Wool Beanie", Description = "Warm, breathable beanie for cold-weather outdoor activities.", Price = 24.99m, Tags = "outdoor,winter,accessory", Category = apparel, ImageUrl = "https://th.bing.com/th/id/OIP.39Tjvphekixq7SqBWpztpwHaF7?r=0&o=7rm=3&rs=1&pid=ImgDetMain&o=7&rm=3" },
            new() { Name = "Trail Running Shoes", Description = "Grippy, durable shoes built for rugged trails.", Price = 109.99m, Tags = "running,outdoor,shoes,trail", Category = apparel, ImageUrl = "https://th.bing.com/th/id/OIP.OriKoOX5Lgn3CTP-OMx0YQHaE8?r=0&o=7rm=3&rs=1&pid=ImgDetMain&o=7&rm=3" },

            new() { Name = "Stainless Steel French Press", Description = "12-cup french press for rich, full-bodied coffee.", Price = 34.99m, Tags = "kitchen,coffee,home", Category = home, ImageUrl = "https://tse1.mm.bing.net/th/id/OIP.1IcQaS2_Nm032iboi7E63AHaHa?r=0&rs=1&pid=ImgDetMain&o=7&rm=3" },
            new() { Name = "Electric Kettle", Description = "Fast-boil kettle with auto shut-off, 1.7L capacity.", Price = 29.99m, Tags = "kitchen,coffee,home,appliance", Category = home, ImageUrl = "https://tse2.mm.bing.net/th/id/OIP.DduaJWN_CP_utU4atToyugHaHa?r=0&rs=1&pid=ImgDetMain&o=7&rm=3" },
            new() { Name = "Ceramic Non-Stick Cookware Set", Description = "10-piece non-stick cookware set, oven safe.", Price = 149.99m, Tags = "kitchen,cookware,home", Category = home, ImageUrl = "https://tse4.mm.bing.net/th/id/OIP.4Y5D4y2Zpsz8jxwotvD9ygHaEf?r=0&rs=1&pid=ImgDetMain&o=7&rm=3" },

            new() { Name = "Insulated Water Bottle", Description = "24oz stainless steel bottle, keeps drinks cold 24 hours.", Price = 27.99m, Tags = "outdoor,hydration,fitness", Category = sports, ImageUrl = "https://th.bing.com/th/id/R.7fd76747c9e009f77ad622b06ffcbb2f?rik=bvKXmm9mjGAsqg&riu=http%3a%2f%2fsungowaterbottles.com%2fcdn%2fshop%2fproducts%2fDoubleWallInsulatedStainlessSteelWaterBottle.png%3fv%3d1710072425&ehk=Bfx7GeeB2n5hHJULLt7eza%2bJ5kSrDXRGj083N2kTciM%3d&risl=&pid=ImgRaw&r=0" },
            new() { Name = "Adjustable Dumbbell Set", Description = "Space-saving dumbbells adjustable from 5-52.5 lbs.", Price = 249.99m, Tags = "fitness,gym,strength", Category = sports, ImageUrl = "https://i5.walmartimages.com/seo/UPGO-Adjustable-Dumbbells-Set-25LB-A-Pair-Weights-5-1-Free-Weights-5-10-15-20-25lb-50lb-Dumbbell-Anti-Slip-Handle-Suitable-Home-Gym-Exercise-Equipmen_95097a3a-10ad-4324-a1b0-986f0d7f87a3.d368c076431f65e2a95b64b651fedf78.jpeg" },
            new() { Name = "Yoga Mat", Description = "Extra-thick non-slip mat for yoga and floor exercises.", Price = 32.99m, Tags = "yoga,fitness,mat", Category = sports, ImageUrl = "https://static.vecteezy.com/system/resources/previews/047/826/797/original/yoga-mat-against-transparent-background-free-png.png" },
            new() { Name = "2-Person Camping Tent", Description = "Lightweight, weatherproof tent for backpacking trips.", Price = 89.99m, Tags = "outdoor,camping,adventure", Category = sports, ImageUrl = "https://th.bing.com/th/id/OIP.xK1GPO7LkfnAuwumTCts5AHaHa?r=0&o=7rm=3&rs=1&pid=ImgDetMain&o=7&rm=3" },
        };

        context.Products.AddRange(products);
        context.SaveChanges();
    }
}
