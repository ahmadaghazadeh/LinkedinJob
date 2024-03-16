using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace LinkedinScraping.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class JobController : ControllerBase
	{
		HttpClient client = new HttpClient();
		private List<string> Fields;
		private List<string> geoids;
		private int pageFetch = 1;

		private string path = "https://api.scrapingdog.com/linkedinjobs/?api_key=65f5604effbbac3174b4d31b";

		[HttpGet]
		public async Task<List<Job>> GetAllContacts()
		{
			var jobs=new List<Job>();
			foreach (var geoid in geoids)
			{
				foreach (var field in Fields)
				{
					for (int page = 0; page < pageFetch; page++)
					{
						var response = await client.GetAsync($"{path}/&field={field}&geoid={geoid}&page={page}");
						var content = await response.Content.ReadAsStreamAsync();
						var tempJobs = await JsonSerializer.DeserializeAsync<List<Job>>(content);
						jobs.AddRange(tempJobs);
					}
					
				}
				 
			}
			return jobs;
		}
	}
}
