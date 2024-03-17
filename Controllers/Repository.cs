namespace LinkedinScraping.Controllers
{
	public class Repository: IRepository
	{
		private readonly JobContext jobContext;

		public Repository(JobContext jobContext)
		{
			this.jobContext = jobContext;
		}
		public void AddRange(List<Job> jobs)
		{
			jobContext.Jobs.AddRange(jobs);
			jobContext.SaveChanges();
		}
	}
}
