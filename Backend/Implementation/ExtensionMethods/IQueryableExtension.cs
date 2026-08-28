namespace Implementation.ExtensionMethods
{
    public static class QueryableExtensions
    {

        // Kako bi na primer izledao poziv ove metode?
        // Koje metoda kako se ona zove?
        // - Filter
        public static IQueryable<T> Filter<T>(
            this IQueryable<T> query)
        {
            // filtriranje

            return query;
        }
    }
}
