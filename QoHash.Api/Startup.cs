using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QoHash.Core;

namespace QoHash.Api
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddScoped<IScanAgent, ScanAgent>();
			services.AddControllers().AddNewtonsoftJson();

			services.Configure<ApiBehaviorOptions>(options =>
			{
				options.InvalidModelStateResponseFactory = actionContext =>
				{
					var actionExecutingContext =
						actionContext as Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext;

					// if there are modelState errors & all keys were correctly
					// found/parsed we're dealing with validation errors
					if (actionContext.ModelState.ErrorCount > 0
					    && actionExecutingContext?.ActionArguments.Count == actionContext.ActionDescriptor.Parameters.Count)
					{
						return new UnprocessableEntityObjectResult(actionContext.ModelState);
					}

					// if one of the keys wasn't correctly found / couldn't be parsed
					// we're dealing with null/unparsable input
					return new BadRequestObjectResult(actionContext.ModelState);
				};
			});

			services.AddVersionedApiExplorer(setupAction =>
			{
				setupAction.GroupNameFormat = "'v'VV";
			});

			services.AddApiVersioning(opt =>
			{
				opt.UseApiBehavior = false;
				opt.AssumeDefaultVersionWhenUnspecified = true;
				opt.DefaultApiVersion = new ApiVersion(1, 0);
				opt.ReportApiVersions = true;
				opt.ApiVersionReader = new HeaderApiVersionReader("X-Version");
			});

			var apiVersionDescriptionProvider =
			#pragma warning disable ASP0000 
				services.BuildServiceProvider().GetService<IApiVersionDescriptionProvider>();
			#pragma warning restore ASP0000 

			services.AddSwaggerGen(setupAction =>
			{
				foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
				{
					setupAction.SwaggerDoc(
						$"DiskScannerSpecification{description.GroupName}",
						new Microsoft.OpenApi.Models.OpenApiInfo
						{
							Title = "Disk Scanner",
							Version = description.ApiVersion.ToString(),
							Description = "Through this API you can scan a given folder from remote computer.",
							Contact = new Microsoft.OpenApi.Models.OpenApiContact
							{
								Email = "gaetan.gingras@usherbrooke.ca",
								Name = "Gaetan Gingras",
							},
							License = new Microsoft.OpenApi.Models.OpenApiLicense
							{
								Name = "MIT License",
								Url = new Uri("https://opensource.org/licenses/MIT")
							}
						});
				}

				setupAction.DocInclusionPredicate((documentName, apiDescription) =>
				{
					var actionApiVersionModel = apiDescription.ActionDescriptor
						.GetApiVersionModel(ApiVersionMapping.Explicit | ApiVersionMapping.Implicit);

					if (actionApiVersionModel == null)
						return true;

					if (actionApiVersionModel.DeclaredApiVersions.Any())
						return actionApiVersionModel.DeclaredApiVersions.Any(v => $"DiskScannerSpecificationv{v.ToString()}" == documentName);

					return actionApiVersionModel.ImplementedApiVersions.Any(v => $"DiskScannerSpecificationv{v.ToString()}" == documentName);
				});

				var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);

				setupAction.IncludeXmlComments(xmlCommentsFullPath);
			});
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider apiVersionDescriptionProvider)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseSwagger();

			app.UseSwaggerUI(setupAction =>
			{
				foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
				{
					setupAction.SwaggerEndpoint($"/swagger/DiskScannerSpecification{description.GroupName}/swagger.json",
						description.GroupName.ToUpperInvariant());
				}

				setupAction.RoutePrefix = "swagger";

				setupAction.DefaultModelExpandDepth(2);
				setupAction.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model);
				setupAction.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
				setupAction.EnableDeepLinking();
				setupAction.DisplayOperationId();
			});

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
