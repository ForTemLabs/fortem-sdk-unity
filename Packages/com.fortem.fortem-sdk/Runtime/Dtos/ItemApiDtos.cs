using System;
using System.Collections.Generic;
using UnityEngine;

namespace ForTemSdk
{
    /// <summary>
    /// Request to create a collection item.
    /// </summary>
    [System.Serializable]
    public sealed class CreateItemRequest
    {
        [SerializeField] private string name;
        [SerializeField] private int quantity;
        [SerializeField] private string redeemCode;
        [SerializeField] private string redeemUrl;
        [SerializeField] private string description;
        [SerializeField] private string itemImage;
        [SerializeField] private List<ItemAttribute> attributes;
        [SerializeField] private string recipientAddress;

        /// <summary>
        /// Name of the item (required).
        /// </summary>
        /// <remarks>
        /// Must be less than or equal to 40 characters.<br/>
        /// Should not be an empty string.
        /// </remarks>
        public string Name { get => name; set => name = value; }

        /// <summary>
        /// Number of items to create (required).
        /// </summary>
        /// <remarks>
        /// Must be a positive number.
        /// </remarks>
        public int Quantity { get => quantity; set => quantity = value; }

        /// <summary>
        /// Developer-managed unique item code (required).
        /// </summary>
        /// <remarks>Must not contain any whitespace characters</remarks>
        public string RedeemCode { get => redeemCode; set => redeemCode = value; }

        /// <summary>
        /// Developer unique item code import site (optional).
        /// </summary>
        public string RedeemUrl { get => redeemUrl; set => redeemUrl = value; }

        /// <summary>
        /// Description of the item (required).
        /// </summary>
        /// <remarks>
        /// Should not be an empty string.<br/>
        /// Must be less than or equal to 1000 characters.
        /// </remarks>
        public string Description { get => description; set => description = value; }

        /// <summary>
        /// CID value returned from image-upload endpoint (optional).
        /// </summary>
        public string ItemImage { get => itemImage; set => itemImage = value; }

        /// <summary>
        /// Custom attributes (optional).
        /// </summary>
        public List<ItemAttribute> Attributes { get => attributes; set => attributes = value; }

        /// <summary>
        /// Recipient wallet address for minting the item (required).<br/>
        /// Must belong to a ForTem user.
        /// </summary>
        public string RecipientAddress { get => recipientAddress; set => recipientAddress = value; }
    }

    /// <summary>
    /// Item creation response.
    /// </summary>
    [Serializable]
    public sealed class CreateItemResponse
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

        /// <summary>
        /// Unique identifier for the created item.
        /// </summary>
        public int ItemID => itemId;

        /// <summary>
        /// Name of the created item.
        /// </summary>
        public string Name => name;

        /// <summary>
        /// Description of the created item.
        /// </summary>
        public string Description => description;

        /// <summary>
        /// Collection ID to which the item belongs.
        /// </summary>
        public int CollectionID => collectionID;

        /// <summary>
        /// NFT number of the created item.
        /// </summary>
        public int NftNumber => nftNumber;

        /// <summary>
        /// Image URL of the created item.
        /// </summary>
        public string ItemImage => itemImage;

        /// <summary>
        /// Quantity of the created item.
        /// </summary>
        public int Quantity => quantity;

        /// <summary>
        /// Developer-managed unique item code.
        /// </summary>
        public string RedeemCode => redeemCode;

        /// <summary>
        /// Developer unique item code import site.
        /// </summary>
        public string RedeemUrl => redeemUrl;

        /// <summary>
        /// Custom attributes of the created item.
        /// </summary>
        public List<ItemAttribute> Attributes => attributes;

        /// <summary>
        /// Status of the created item.
        /// </summary>
        /// <remarks>
        /// PROCESSING: The item is currently being processed as an NFT. No additional action is required from the developer.<br/>
        /// MINTED: The item has been successfully minted as an NFT.<br/>
        /// OFFER_PENDING: The item is pending an exchange offer with another item.<br/>
        /// KIOSK_LISTED: The item is listed for sale to other users.<br/>
        /// REDEEMED: The NFT has been burned for in-game use (redemption completed).
        /// </remarks>
        public string RawStatus => status;

        /// <summary>
        /// Status of the created item.
        /// </summary>
        public ItemStatus Status
        {
            get
            {
                return status switch
                {
                    "PROCESSING" => ItemStatus.Processing,
                    "MINTED" => ItemStatus.Minted,
                    "OFFER_PENDING" => ItemStatus.OfferPending,
                    "KIOSK_LISTED" => ItemStatus.KioskListed,
                    "REDEEMED" => ItemStatus.Redeemed,
                    _ => ItemStatus.Unknown,
                };
            }
        }
    }

    /// <summary>
    /// Collection item (NFT) information.
    /// </summary>
    [Serializable]
    public sealed class GetItemResponse
    {
        [SerializeField] private int id;
        [SerializeField] private string objectId;
        [SerializeField] private string name;
        [SerializeField] private string description;
        [SerializeField] private int nftNumber;
        [SerializeField] private string itemImage;
        [SerializeField] private int quantity;
        [SerializeField] private List<ItemAttribute> attributes;
        [SerializeField] private ItemOwner owner;
        [SerializeField] private string status;
        [SerializeField] private long createdAt;
        [SerializeField] private long updatedAt;

        /// <summary>
        /// Unique identifier for the item.
        /// </summary>
        public int ID => id;

        /// <summary>
        /// Object ID for the item.
        /// </summary>
        public string ObjectID => objectId;

        /// <summary>
        /// Name of the item.
        /// </summary>
        public string Name => name;

        /// <summary>
        /// Description of the item.
        /// </summary>
        public string Description => description;

        /// <summary>
        /// NFT number of the item.
        /// </summary>
        public int NftNumber => nftNumber;

        /// <summary>
        /// Image URL of the item.
        /// </summary>
        public string ItemImage => itemImage;

        /// <summary>
        /// Quantity of the item.
        /// </summary>
        public int Quantity => quantity;

        /// <summary>
        /// Custom attributes of the item.
        /// </summary>
        public List<ItemAttribute> Attributes => attributes;

        /// <summary>
        /// Owner information of the item.
        /// </summary>
        public ItemOwner Owner => owner;

        /// <summary>
        /// Status of the item.
        /// </summary>
        /// <remarks><inheritdoc cref="CreateItemResponse.RawStatus"/></remarks>"
        public string RawStatus => status;

        /// <summary>
        /// Timestamp when the item was created.
        /// </summary>
        public long CreatedAt => createdAt;

        /// <summary>
        /// Timestamp when the item was last updated.
        /// </summary>
        public long UpdatedAt => updatedAt;

        /// <summary>
        /// Status of the item.
        /// </summary>
        public ItemStatus Status
        {
            get
            {
                return status switch
                {
                    "PROCESSING" => ItemStatus.Processing,
                    "MINTED" => ItemStatus.Minted,
                    "OFFER_PENDING" => ItemStatus.OfferPending,
                    "KIOSK_LISTED" => ItemStatus.KioskListed,
                    "REDEEMED" => ItemStatus.Redeemed,
                    _ => ItemStatus.Unknown,
                };
            }
        }
    }

    /// <summary>
    /// Owner information for items.
    /// </summary>
    [Serializable]
    public sealed class ItemOwner
    {
        [SerializeField] private string nickname;
        [SerializeField] private string walletAddress;

        /// <summary>
        /// Owner's nickname.
        /// </summary>
        public string Nickname => nickname;

        /// <summary>
        /// Owner's wallet address.
        /// </summary>
        public string WalletAddress => walletAddress;
    }

    /// <summary>
    /// Attribute for collection items.
    /// </summary>
    [Serializable]
    public sealed class ItemAttribute
    {
        [SerializeField] private string name;
        [SerializeField] private string value;

        /// <summary>
        /// Name of the attribute.
        /// </summary>
        /// <remarks>Should not be an empty string.</remarks>
        public string Name { get => name; set => name = value; }

        /// <summary>
        /// Value of the attribute.
        /// </summary>
        /// <remarks>Should not be an empty string.</remarks>
        public string Value { get => this.value; set => this.value = value; }
    }

    /// <summary>
    /// Image upload response.
    /// </summary>
    [Serializable]
    public sealed class ImageUploadResponse
    {
        [SerializeField] private string itemImage;

        /// <summary>
        /// CID value for the uploaded image.
        /// </summary>
        public string ItemImage => itemImage;
    }

    /// <summary>
    /// Request to update an item.
    /// </summary>
    [System.Serializable]
    public sealed class UpdateItemRequest
    {
        [SerializeField] private string name;
        [SerializeField] private string description;
        [SerializeField] private string itemImage;
        [SerializeField] private List<ItemAttribute> attributes;

        /// <summary>
        /// Name of the item (optional).
        /// </summary>
        /// <remarks>
        /// Must be less than or equal to 40 characters.<br/>
        /// Should not be an empty string.
        /// </remarks>
        public string Name { get => name; set => name = value; }

        /// <summary>
        /// Description of the item (optional).
        /// </summary>
        /// <remarks>
        /// Should not be an empty string.<br/>
        /// Must be less than or equal to 1000 characters.
        /// </remarks>
        public string Description { get => description; set => description = value; }

        /// <summary>
        /// CID value returned from image-upload endpoint (optional).
        /// </summary>
        public string ItemImage { get => itemImage; set => itemImage = value; }

        /// <summary>
        /// Custom attributes (optional).
        /// </summary>
        public List<ItemAttribute> Attributes { get => attributes; set => attributes = value; }
    }

    /// <summary>
    /// Item update response.
    /// </summary>
    [Serializable]
    public sealed class UpdateItemResponse
    {
        [SerializeField] private int itemId;
        [SerializeField] private string name;
        [SerializeField] private string description;
        [SerializeField] private string itemImage;
        [SerializeField] private List<ItemAttribute> attributes;

        /// <summary>
        /// Unique identifier for the updated item.
        /// </summary>
        public int ItemID => itemId;

        /// <summary>
        /// Name of the updated item.
        /// </summary>
        public string Name => name;

        /// <summary>
        /// Description of the updated item.
        /// </summary>
        public string Description => description;

        /// <summary>
        /// Image URL of the updated item.
        /// </summary>
        public string ItemImage => itemImage;

        /// <summary>
        /// Custom attributes of the created item.
        /// </summary>
        public List<ItemAttribute> Attributes => attributes;
    }
}
