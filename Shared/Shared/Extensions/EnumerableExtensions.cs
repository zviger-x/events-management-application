namespace Shared.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Splits the source sequence into batches (chunks) of the specified size.
        /// Each batch is returned as a separate IEnumerable&lt;T&gt;.
        /// </summary>
        /// <typeparam name="T">The type of elements in the source sequence.</typeparam>
        /// <param name="source">The source sequence to split into batches.</param>
        /// <param name="size">The maximum number of elements in each batch. Must be greater than 0.</param>
        /// <returns>An IEnumerable of IEnumerable&lt;T&gt;, where each inner sequence is a batch of the specified size.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the source is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the size is less than or equal to 0.</exception>
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int size)
        {
            ArgumentNullException.ThrowIfNull(source, nameof(source));
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(size, 0, nameof(size));

            var bucket = default(List<T>);
            var count = 0;

            foreach (var item in source)
            {
                bucket ??= new List<T>(size);
                bucket.Add(item);
                count++;

                if (count == size)
                {
                    yield return bucket;
                    bucket = null;
                    count = 0;
                }
            }

            if (bucket != null && bucket.Count > 0)
                yield return bucket;
        }
    }
}
