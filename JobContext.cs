using Microsoft.EntityFrameworkCore;

namespace LinkedinScraping
{
	public class JobContext : DbContext
	{
		public DbSet<Job> Jobs { get; set; }
	}
}
