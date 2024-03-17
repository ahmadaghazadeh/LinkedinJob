using Microsoft.EntityFrameworkCore;

namespace LinkedinScraping
{
	public static class EFCoreExtention
	{
		public static void AddOrUpdateRange<TEntity>(this DbSet<TEntity> set, IEnumerable<TEntity> entities)
			where TEntity : class
		{
			foreach (var entity in entities)
			{
				_ = !set.Any(e => e == entity) ? set.Add(entity) : set.Update(entity);
			}
		}
	}
}
