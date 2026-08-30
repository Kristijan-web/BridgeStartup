namespace Implementation.ExtensionMethods
{
    public static class QueryableExtensions
    {

        // Kako bi na primer izledao poziv ove metode?
        // Koje metoda kako se ona zove?
        // - Filter

        // Kako bi izgledalo pozivanje ove emtoda

        // IQueryable<Post> = _context.Posts.asQueryable().Filter(); 
        // Pozvao sam filter metodu ali sta ona filtrira kada svaki entitet ima drugacije parametre po kojima se filtira, ovde bi morao delegat da se prosledi

        // Metoda filter mora da prima delegat
        public static IQueryable<T> Filter<T>(
            this IQueryable<T> query)
        {
            // filtriranje


            return query;
        }
    }
}
