//using NUnit.Framework;

//namespace ForTemSdk
//{
//    /// <summary>
//    /// Tests for data models and response types.
//    /// </summary>
//    public class ModelsTests
//    {
//        [Test]
//        public void ApiCallResultGeneric_CanSetAndGetProperties()
//        {
//            var result = new ApiCallResult<User>
//            {
//                Success = true,
//                Data = new User { Nickname = "TestUser", WalletAddress = "0x123" },
//                StatusCode = 200
//            };

//            Assert.IsTrue(result.Success);
//            Assert.AreEqual("TestUser", result.Data.Nickname);
//            Assert.AreEqual("0x123", result.Data.WalletAddress);
//            Assert.AreEqual(200, result.StatusCode);
//        }

//        [Test]
//        public void ApiCallResultGeneric_CanSetError()
//        {
//            var result = new ApiCallResult<User>
//            {
//                Success = false,
//                Error = "Not found",
//                StatusCode = 404
//            };

//            Assert.IsFalse(result.Success);
//            Assert.AreEqual("Not found", result.Error);
//            Assert.AreEqual(404, result.StatusCode);
//        }

//        [Test]
//        public void NonceResponse_CanSetAndGetNonce()
//        {
//            var response = new NonceResponse { nonce = "test-nonce-123" };

//            Assert.AreEqual("test-nonce-123", response.nonce);
//        }

//        [Test]
//        public void AccessTokenResponse_CanSetAndGetToken()
//        {
//            var response = new AccessTokenResponse { AccessToken = "test-token-abc" };

//            Assert.AreEqual("test-token-abc", response.AccessToken);
//        }

//        [Test]
//        public void User_CanSetAllProperties()
//        {
//            var user = new User
//            {
//                IsUser = true,
//                Nickname = "TestUser",
//                ProfileImage = "https://example.com/image.png",
//                WalletAddress = "0xabc123"
//            };

//            Assert.IsTrue(user.IsUser);
//            Assert.AreEqual("TestUser", user.Nickname);
//            Assert.AreEqual("https://example.com/image.png", user.ProfileImage);
//            Assert.AreEqual("0xabc123", user.WalletAddress);
//        }

//        [Test]
//        public void Collection_CanSetAllProperties()
//        {
//            var collection = new Collection
//            {
//                ID = 1,
//                Name = "TestCollection",
//                Description = "A test collection"
//            };

//            Assert.AreEqual(1, collection.ID);
//            Assert.AreEqual("TestCollection", collection.Name);
//            Assert.AreEqual("A test collection", collection.Description);
//        }

//        [Test]
//        public void CollectionItem_CanSetAllProperties()
//        {
//            var item = new CollectionItem
//            {
//                ID = 40,
//                Name = "TestItem",
//                Quantity = 10,
//                ItemImage = "https://example.com/item.png"
//            };

//            Assert.AreEqual(40, item.ID);
//            Assert.AreEqual("TestItem", item.Name);
//            Assert.AreEqual(10, item.Quantity);
//            Assert.AreEqual("https://example.com/item.png", item.ItemImage);
//        }

//        [Test]
//        public void ItemCreationResponse_CanSetAllProperties()
//        {
//            var response = new ItemCreationResponse
//            {
//                ItemID = 2,
//                Name = "NewItem",
//                Quantity = 5
//            };

//            Assert.AreEqual(2, response.ItemID);
//            Assert.AreEqual("NewItem", response.Name);
//            Assert.AreEqual(5, response.Quantity);
//        }

//        [Test]
//        public void ImageUploadResponse_CanSetCid()
//        {
//            var response = new ImageUploadResponse { ItemImage = "QmTest123" };

//            Assert.AreEqual("QmTest123", response.ItemImage);
//        }

//        [Test]
//        public void ItemAttribute_CanSetAllProperties()
//        {
//            var attr = new ItemAttribute
//            {
//                Name = "Rarity",
//                Value = "Legendary"
//            };

//            Assert.AreEqual("Rarity", attr.Name);
//            Assert.AreEqual("Legendary", attr.Value);
//        }
//    }
//}
