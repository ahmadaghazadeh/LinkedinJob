using System.Text.Json.Serialization;

public class Job
{
	[JsonPropertyName("job_position")]
	public string JobPosition { get; set; }

	[JsonPropertyName("job_link")]
	public Uri JobLink { get; set; }

	[JsonPropertyName("job_id")]
	public string JobId { get; set; }

	[JsonPropertyName("company_name")]
	public string CompanyName { get; set; }

	[JsonPropertyName("company_profile")]
	public string CompanyProfile { get; set; }

	[JsonPropertyName("job_location")]
	public string JobLocation { get; set; }

	[JsonPropertyName("job_posting_date")]
	public DateTimeOffset JobPostingDate { get; set; }

	public bool IsRelocation { get; set; }

	public bool IsRemote { get; set; }

	public string JobDescriptiob { get; set; }


}