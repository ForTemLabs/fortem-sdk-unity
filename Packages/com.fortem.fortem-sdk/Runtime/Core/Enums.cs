namespace ForTemSdk
{
    /// <summary>
    /// Enum for specifying the ForTem environment.
    /// </summary>
    public enum ForTemEnvironment
    {
        Testnet,
        Mainnet
    }

    /// <summary>
    /// Status of an item within a collection.
    /// </summary>
    public enum ItemStatus
    {
        /// <summary>
        /// The item status is unknown.
        /// </summary>
        Unknown,

        /// <summary>
        /// The item is currently being processed as an NFT.
        /// </summary>
        Processing,

        /// <summary>
        /// The item has been successfully minted as an NFT.
        /// </summary>
        Minted,

        /// <summary>
        /// The item is pending an exchange offer with another item.
        /// </summary>
        OfferPending,

        /// <summary>
        /// The item is listed for sale to other users.
        /// </summary>
        KioskListed,

        /// <summary>
        /// The NFT has been burned for in-game use (redemption completed).
        /// </summary>
        Redeemed
    }
}
