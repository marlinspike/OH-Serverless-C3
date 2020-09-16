namespace com.cleetus.models
{

    public class Product {
        public string productId { get; set; }
        public string productName { get; set; }
        public string productDescription { get; set; }
    }

    public class UserRating {
        public string id { get; set; }
        public string userId { get; set; }
        public string productId { get; set; }
        public string timestamp { get; set; }
        public string locationName { get; set; }
        public string rating { get; set; }
        public string userNotes { get; set; }
        public string userName { get; set; }
        public string fullName { get; set; }
    }
}