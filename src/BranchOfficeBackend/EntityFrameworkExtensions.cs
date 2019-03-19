using Microsoft.EntityFrameworkCore;

namespace BranchOfficeBackend
{
    public static class EntityFrameworkExtensions
    {
        /// <summary>
        /// Remove all rows from a table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Clear<T>(this DbSet<T> dbSet) where T : class
        {
            dbSet.RemoveRange(dbSet);
        }
    }
}