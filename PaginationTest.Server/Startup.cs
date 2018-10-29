using System;
using Microsoft.AspNetCore.Blazor.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using System.Linq;
using System.Net.Mime;
using Cosmonaut;
using Cosmonaut.Extensions.Microsoft.DependencyInjection;
using PaginationTest.Shared;

namespace PaginationTest.Server
{
    public class Startup
    {
        readonly CosmosStoreSettings _cosmosStoreSettings = new CosmosStoreSettings("paginationdb", "https://localhost:8081",
            "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddResponseCompression(options =>
            {
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                    MediaTypeNames.Application.Octet,
                    WasmMediaTypeNames.Application.Wasm,
                });
            });

            services.AddCosmosStore<Student>(_cosmosStoreSettings);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc(routes =>
            {
                routes.MapRoute(name: "default", template: "{controller}/{action}/{id?}");
            });

            var studentsStore = serviceProvider.GetRequiredService<ICosmosStore<Student>>();

            //studentsStore.RemoveAsync(student => true).GetAwaiter().GetResult();

            for (var i = 0; i < 500; i++)
            {
                studentsStore.UpsertAsync(new Student
                {
                    Id = i.ToString(),
                    FirstName = $"FirstName {i}",
                    LastName = $"LastName {i}"
                }).GetAwaiter().GetResult();
            }

            app.UseBlazor<Client.Startup>();
        }
    }
}
