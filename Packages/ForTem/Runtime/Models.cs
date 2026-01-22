using System;
using System.Collections.Generic;
using UnityEngine;

namespace ForTemSdk
{
    /// <summary>
    /// Standard response wrapper for all ForTem API requests.
    /// </summary>
    [Serializable]
    internal class ApiResponse<T>
    {
        public int statusCode;
        public T data;
        //"metadata":{"pagination":{"totalItems":1}}
    }

    /// <summary>
    /// Request to exchange nonce for access token.
    /// </summary>
    [Serializable]
    public class AccessTokenRequest
    {
        [SerializeField] private string nonce;
        public string Nonce { get => nonce; set => nonce = value; }
    }

    /// <summary>
    /// Request to create a collection.
    /// </summary>
    [Serializable]
    public class CreateCollectionRequest
    {
        [SerializeField] private string name;
        [SerializeField] private string description;
        [SerializeField] private Link link;

        public string Name { get => name; set => name = value; }
        public string Description { get => description; set => description = value; }
        public Link Link { get => link; set => link = value; }
    }

    /// <summary>
    /// Request to create a collection item.
    /// </summary>
    [System.Serializable]
    public class CreateItemRequest
    {
        [SerializeField] private string name;
        [SerializeField] private int quantity;
        [SerializeField] private string redeemCode;
        [SerializeField] private string redeemUrl;
        [SerializeField] private string description;
        [SerializeField] private string itemImage;
        [SerializeField] private List<ItemAttribute> attributes;

        /// <summary>
        /// Recipient wallet address for minting the item (ForTem user only; required).
        /// </summary>
        [SerializeField] private string recipientAddress;

        public string Name { get => name; set => name = value; }
        public int Quantity { get => quantity; set => quantity = value; }
        public string RedeemCode { get => redeemCode; set => redeemCode = value; }
        public string RedeemUrl { get => redeemUrl; set => redeemUrl = value; }
        public string Description { get => description; set => description = value; }
        public string ItemImage { get => itemImage; set => itemImage = value; }
        public List<ItemAttribute> Attributes { get => attributes; set => attributes = value; }
        public string RecipientAddress { get => recipientAddress; set => recipientAddress = value; }
    }

    /// <summary>
    /// Nonce response for authentication initiation.
    /// </summary>
    [Serializable]
    internal class NonceResponseData
    {
        [SerializeField] private string nonce;
        public string Nonce => nonce;
    }

    /// <summary>
    /// Access token response from authentication.
    /// </summary>
    [Serializable]
    internal class AccessTokenResponseData
    {
        [SerializeField] private string accessToken;
        public string AccessToken => accessToken;
    }

    /// <summary>
    /// User information from ForTem API.
    /// </summary>
    [Serializable]
    public class UserResponseData
    {
        [SerializeField] private bool isUser;
        [SerializeField] private string nickname;

        /// <summary>
        /// Default: profile/default.png
        /// </summary>
        [SerializeField] private string profileImage;
        [SerializeField] private string walletAddress;

        public bool IsUser => isUser;
        public string Nickname => nickname;
        public string ProfileImage => profileImage;
        public string WalletAddress => walletAddress;
    }

    /// <summary>
    /// Game collection information.
    /// </summary>
    [Serializable]
    public class CollectionResponseData
    {
        [SerializeField] private int id;
        [SerializeField] private string objectId;
        [SerializeField] private string name;
        [SerializeField] private string description;
        [SerializeField] private string tradeVolume;
        [SerializeField] private int itemCount;
        [SerializeField] private long createdAt;
        [SerializeField] private long updatedAt;
        [SerializeField] private Link link;

        public int ID => id;
        public string ObjectID => objectId;
        public string Name => name;
        public string Description => description;
        public string TradeVolume => tradeVolume;
        public int ItemCount => itemCount;
        public long CreatedAt => createdAt;
        public long UpdatedAt => updatedAt;
        public Link Link => link;
    }

    /// <summary>
    /// Link information for collections.
    /// </summary>
    [Serializable]
    public class Link
    {
        [SerializeField] private string website;
        public string Website { get => website; set => website = value; }
    }

    /// <summary>
    /// Owner information for items.
    /// </summary>
    [Serializable]
    public class Owner
    {
        [SerializeField] private string nickname;
        [SerializeField] private string walletAddress;

        public string Nickname => nickname;
        public string WalletAddress => walletAddress;
    }

    /// <summary>
    /// Attribute for collection items.
    /// </summary>
    [Serializable]
    public class ItemAttribute
    {
        [SerializeField] private string name;
        [SerializeField] private string value;

        public string Name { get => name; set => name = value; }
        public string Value { get => this.value; set => this.value = value; }
    }

    /// <summary>
    /// Collection item (NFT) information.
    /// </summary>
    [Serializable]
    public class ItemResponseData
    {
        [SerializeField] private int id;
        [SerializeField] private string objectId;
        [SerializeField] private string name;
        [SerializeField] private string description;
        [SerializeField] private int nftNumber;
        [SerializeField] private string itemImage;
        [SerializeField] private int quantity;
        [SerializeField] private List<ItemAttribute> attributes;
        [SerializeField] private Owner owner;
        [SerializeField] private string status;
        [SerializeField] private long createdAt;
        [SerializeField] private long updatedAt;

        public int ID => id;
        public string ObjectID => objectId;
        public string Name => name;
        public string Description => description;
        public int NftNumber => nftNumber;
        public string ItemImage => itemImage;
        public int Quantity => quantity;
        public List<ItemAttribute> Attributes => attributes;
        public Owner Owner => owner;
        public string Status => status;
        public long CreatedAt => createdAt;
        public long UpdatedAt => updatedAt;
    }

    /// <summary>
    /// Item creation response.
    /// </summary>
    [Serializable]
    public class ItemCreationResponse
    {
        [SerializeField] private int itemId;
        [SerializeField] private string name;
        [SerializeField] private string description;
        [SerializeField] private int collectionID;
        [SerializeField] private int nftNumber;
        [SerializeField] private string itemImage;
        [SerializeField] private int quantity;
        [SerializeField] private string redeemCode;
        [SerializeField] private string redeemUrl;
        [SerializeField] private List<ItemAttribute> attributes;
        [SerializeField] private string status;

        public int ItemID => itemId;
        public string Name => name;
        public string Description => description;
        public int CollectionID => collectionID;
        public int NftNumber => nftNumber;
        public string ItemImage => itemImage;
        public int Quantity => quantity;
        public string RedeemCode => redeemCode;
        public string RedeemUrl => redeemUrl;
        public List<ItemAttribute> Attributes => attributes;
        public string Status => status;
    }

    /// <summary>
    /// Image upload response.
    /// </summary>
    [Serializable]
    public class ImageUploadResponse
    {
        [SerializeField] private string itemImage;
        public string ItemImage => itemImage;
    }
}
