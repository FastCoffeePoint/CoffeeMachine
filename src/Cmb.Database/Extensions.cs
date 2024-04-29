namespace Cmb.Database;

public static class Extensions
{
    public static IQueryable<T> ExcludeDeleted<T>(this IQueryable<T> query) where T : DbEntity =>
        query.Where(u => u.DeleteDate == null);
}