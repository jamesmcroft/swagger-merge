namespace SwaggerMerge.Common.Extensions;

/// <summary>
/// Defines a set of extension methods for collections.
/// </summary>
internal static class CollectionExtensions
{
    /// <summary>
    /// Adds or updates a value within a dictionary.
    /// </summary>
    /// <param name="dictionary">
    /// The dictionary to update.
    /// </param>
    /// <param name="key">
    /// The key of the value to add or update.
    /// </param>
    /// <param name="value">
    /// The value to add or update.
    /// </param>
    /// <typeparam name="TKey">
    /// The type of key item within the dictionary.
    /// </typeparam>
    /// <typeparam name="TValue">
    /// The type of value item within the dictionary.
    /// </typeparam>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="dictionary"/> or <paramref name="key"/> is <see langword="null"/>.</exception>
    public static void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(dictionary);

        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        if (dictionary.ContainsKey(key))
        {
            dictionary.Remove(key);
        }

        dictionary.Add(key, value);
    }

    /// <summary>
    /// Adds a collection of items to another.
    /// </summary>
    /// <param name="collection">
    /// The collection to add to.
    /// </param>
    /// <param name="itemsToAdd">
    /// The items to add.
    /// </param>
    /// <typeparam name="T">
    /// The type of items in the collection.
    /// </typeparam>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="collection"/> or <paramref name="itemsToAdd"/> is <see langword="null"/>.</exception>
    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> itemsToAdd)
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(itemsToAdd);

        foreach (var item in itemsToAdd)
        {
            collection.Add(item);
        }
    }

    /// <summary>
    /// Removes a collection of items from another.
    /// </summary>
    /// <param name="collection">
    /// The collection to remove from.
    /// </param>
    /// <param name="itemsToRemove">
    /// The items to remove from the collection.
    /// </param>
    /// <typeparam name="T">
    /// The type of items in the collection.
    /// </typeparam>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="collection"/> or <paramref name="itemsToRemove"/> is <see langword="null"/>.</exception>
    public static void RemoveRange<T>(this ICollection<T> collection, IEnumerable<T> itemsToRemove)
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(itemsToRemove);

        foreach (var item in itemsToRemove)
        {
            if (collection.Contains(item))
            {
                collection.Remove(item);
            }
        }
    }
}
