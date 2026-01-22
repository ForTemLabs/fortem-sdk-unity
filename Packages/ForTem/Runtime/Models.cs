using System;
using System.Collections.Generic;
using UnityEngine;

namespace ForTemSdk
{
    /// <summary>
    /// Standard response wrapper for all ForTem API requests.
    /// </summary>
    [Serializable]
    public class ApiResponse<T>
    {
        public int statusCode;
        public T data;
        //"metadata":{"pagination":{"totalItems":1}}
    }

    /// <summary>
    /// Request to exchange nonce for access token.
    /// </summary>
    [Serializable]
    public class TokenExchangeRequest
    {
        [SerializeField] private string nonce;

        public TokenExchangeRequest(string nonceValue)
        {
            nonce = nonceValue;
        }

        public string Nonce => nonce;
    }

    /// <summary>
    /// Request to create a collection.
    /// </summary>
    [Serializable]
    public class CreateCollectionRequest
    {
        // TODO: Underscore naming convention is these stay private
        [SerializeField] private string name;
        [SerializeField] private string description;
        [SerializeField] private Link link;

        public CreateCollectionRequest(string collectionName, string collectionDescription, Link link = null)
        {
            name = collectionName;
            description = collectionDescription;
            this.link = link;
        }

        public string Name => name;
        public string Description => description;
        public Link Link => link;
    }

    /// <summary>
    /// Request to create a collection item.
    /// </summary>
    [System.Serializable]
    public class CreateCollectionItemRequest
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

        public CreateCollectionItemRequest(
            string itemName,
            int itemQuantity,
            string itemRedeemCode,
            string itemRedeemUrl = "",
            string itemDescription = "",
            string itemImageCid = "",
            List<ItemAttribute> itemAttributes = null,
            string itemRecipientAddress = "")
        {
            name = itemName;
            quantity = itemQuantity;
            redeemCode = itemRedeemCode;
            redeemUrl = itemRedeemUrl;
            description = itemDescription;
            itemImage = itemImageCid;
            attributes = itemAttributes;
            recipientAddress = itemRecipientAddress;
        }
    }

    /// <summary>
    /// Nonce response for authentication initiation.
    /// </summary>
    [Serializable]
    public class NonceResponse
    {
        public string nonce;
    }

    /// <summary>
    /// Access token response from authentication.
    /// </summary>
    [Serializable]
    public class AccessTokenResponse
    {
        [SerializeField] private string accessToken;
        public string AccessToken => accessToken;
    }

    /// <summary>
    /// User information from ForTem API.
    /// </summary>
    [Serializable]
    public class User
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
    public class Collection
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
        public string Website => website;
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
    public class CollectionItem
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
        public int ItemID => itemId;

        [SerializeField] private string name;
        public string Name => name;

        [SerializeField] private string description;
        public string Description => description;

        [SerializeField] private int collectionID;
        public int CollectionID => collectionID;

        [SerializeField] private int nftNumber;
        public int NftNumber => nftNumber;

        [SerializeField] private string itemImage;
        public string ItemImage => itemImage;

        [SerializeField] private int quantity;
        public int Quantity => quantity;

        [SerializeField] private string redeemCode;
        public string RedeemCode => redeemCode;

        [SerializeField] private string redeemUrl;
        public string RedeemUrl => redeemUrl;

        [SerializeField] private List<ItemAttribute> attributes;
        public List<ItemAttribute> Attributes => attributes;

        [SerializeField] private string status;
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
