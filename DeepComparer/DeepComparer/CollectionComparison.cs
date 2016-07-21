namespace DeepComparison
{
    /// <summary>There are many ways to compare collections</summary>
    public enum CollectionComparison
    {
        /// <summary>Collections are equal when they 
        /// all the same items in the same order</summary>
        Sequential,
        /// <summary>Collections are equal when all items present in one
        /// collection present in the other </summary>
        Equivalency
    }
}