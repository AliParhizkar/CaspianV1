namespace Caspian.common
{
    public static class PublicExtenssion
    {
        public static void AddRange<TEntity>(this ICollection<TEntity> entities, params TEntity[] items)
        {
            foreach (var item in items)
                entities.Add(item);
        }
    }
}
