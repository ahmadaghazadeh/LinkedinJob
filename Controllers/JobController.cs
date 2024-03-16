using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace LinkedinScraping.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class JobController : ControllerBase
	{
		private readonly IHttpClientFactory _httpClientFactory;

		private string path = "https://api.scrapingdog.com/linkedinjobs/?api_key=65f5604effbbac3174b4d31b";
		private JobContext _dbContext;
		List<string> Fields=new List<string>();
		List<string> geoids = new List<string>();
		int pageFetch =1;

		public JobController(IConfiguration configuration, JobContext dbContext, IHttpClientFactory httpClientFactory)
		{
			_dbContext = dbContext;

			Fields = configuration.GetSection("Fileds").Get<List<string>>();
			geoids = configuration.GetSection("geoids").Get<List<string>>();
			pageFetch = configuration.GetSection("pageFetch").Get<int>();
			_httpClientFactory = httpClientFactory;
		}

		[HttpGet]
		public async Task<List<Job>> GetAllContacts()
		{
			

			var jobs=new List<Job>();

			Parallel.ForEach(geoids, geoid =>
			{
				Parallel.ForEach(Fields, field =>
				{
					Parallel.For(0, pageFetch, async page =>
					{
						var tempJobs = await TempJobs(field, geoid, page);
						jobs.AddRange(tempJobs);
					});
				});

			});
 
			return jobs;
		}

		private async Task<List<Job>?> TempJobs(string field, string geoid, int page)
		{
			var jobs = new List<Job>();
			var httpClient = _httpClientFactory.CreateClient();
			var response = await httpClient.GetAsync($"{path}&field={field}&geoid={geoid}&page={page}");
			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStreamAsync();
				var tempJobs = await JsonSerializer.DeserializeAsync<List<Job>>(content);
				foreach (var job in tempJobs)
				{
					var jobNew =await getJobDescriptionTask(job);
					jobs.Add(jobNew);
				}
			}
			return jobs;
		}

		private async Task<Job> getJobDescriptionTask(Job job)
		{
			var httpClient = _httpClientFactory.CreateClient();
			var response = await httpClient.GetAsync($"{job.JobLink}");
			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStreamAsync();
				var jobDesc = await JsonSerializer.DeserializeAsync<string>(content);
				if (jobDesc.Contains("relocat"))
				{
					job.IsRelocation = true;
				}
				if (jobDesc.Contains("remote"))
				{
					job.IsRemote = true;
				}
			}
			
			return job;
		}
	}
}
