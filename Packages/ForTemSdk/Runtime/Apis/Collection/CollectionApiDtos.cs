using System;
using UnityEngine;

namespace ForTemSdk
{
    /// <summary>
    /// Request to create a collection.
    /// </summary>
    [Serializable]
    public sealed class CreateCollectionRequest
    {
        [SerializeField] private string name;
        [SerializeField] private string description;
        [SerializeField] private CollectionLink link;

        /// <summary>
        /// Name of the collection (required).
        /// </summary>
        /// <remarks>
        /// Must be less than or equal to 40 characters.<br/>
        /// Should not be an empty string.
        /// </remarks>
        public string Name { get => name; set => name = value; }

        /// <summary>
        /// Description of the collection (required).
        /// </summary>
        /// <remarks>
        /// Should not be an empty string.<br/>
        /// Must be less than or equal to 1000 characters.
        /// </remarks>
        public string Description { get => description; set => description = value; }

        /// <summary>
        /// Link information for the collection (optional).
        /// </summary>
        public CollectionLink Link { get => link; set => link = value; }
    }

    /// <summary>
    /// Game collection information.
    /// </summary>
    [Serializable]
    public sealed class CollectionResponse
    {
        [SerializeField] private int id;
        [SerializeField] private string objectId;
        [SerializeField] private string name;
        [SerializeField] private string description;
        [SerializeField] private string tradeVolume;
        [SerializeField] private int itemCount;
        [SerializeField] private CollectionLink link;
        [SerializeField] private long createdAt;
        [SerializeField] private long updatedAt;

        /// <summary>
        /// Unique identifier for the collection.
        /// </summary>
        public int ID => id;

        /// <summary>
        /// Object ID for the collection.
        /// </summary>
        public string ObjectID => objectId;

        /// <summary>
        /// Name of the collection.
        /// </summary>
        public string Name => name;

        /// <summary>
        /// Description of the collection.
        /// </summary>
        public string Description => description;

        /// <summary>
        /// Total trade volume of the collection.
        /// </summary>
        public string TradeVolume => tradeVolume;

        /// <summary>
        /// Number of items in the collection.
        /// </summary>
        public int ItemCount => itemCount;

        /// <summary>
        /// Link information for the collection.
        /// </summary>
        public CollectionLink Link => link;

        /// <summary>
        /// Timestamp when the collection was created.
        /// </summary>
        public long CreatedAt => createdAt;

        /// <summary>
        /// Timestamp when the collection was last updated.
        /// </summary>
        public long UpdatedAt => updatedAt;
    }

    /// <summary>
    /// Link information for collections.
    /// </summary>
    [Serializable]
    public sealed class CollectionLink
    {
        [SerializeField] private string website;

        /// <summary>
        /// Website URL for the collection.
        /// </summary>
        public string Website { get => website; set => website = value; }
    }
}
