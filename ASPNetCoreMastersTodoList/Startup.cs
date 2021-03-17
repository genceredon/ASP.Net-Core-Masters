using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Repositories;
using Services;
using ASPNetCoreMastersTodoList.Api.Models;
using ASPNetCoreMastersTodoList.Api.Filters;
using ASPNetCoreMastersTodoList.Api.Data;
using Microsoft.EntityFrameworkCore;
using ASPNetCoreMastersTodoList.Api.Areas.Identity.Data;

namespace ASPNetCoreMastersTodoList
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options => 
            {
                options.Filters.Add(new GlobalTimeElapsedAsyncFilter());          
            });

            services.AddSingleton<DataContext>();
            services.AddScoped<IItemRepository, ItemRepository>();
            services.AddScoped<IUserService, UserService>();

            services.Configure<Settings>(Configuration.GetSection("Authentication:JWT:SecurityKey"));

            services.AddDbContext<ASPNetCoreMastersTodoListApiContext>(options =>
                    options.UseSqlServer(
                        Configuration.GetConnectionString("ASPNetCoreMastersTodoListApiContextConnection")));

            services.AddDefaultIdentity<ASPNetCoreMastersTodoListApiUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ASPNetCoreMastersTodoListApiContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            env.EnvironmentName = "Production";

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();                  
            });
        }
    }
}
