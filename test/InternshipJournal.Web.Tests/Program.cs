using Microsoft.AspNetCore.Builder;
using InternshipJournal;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();

builder.Environment.ContentRootPath = GetWebProjectContentRootPathHelper.Get("InternshipJournal.Web.csproj");
await builder.RunAbpModuleAsync<InternshipJournalWebTestModule>(applicationName: "InternshipJournal.Web" );

public partial class Program
{
}
