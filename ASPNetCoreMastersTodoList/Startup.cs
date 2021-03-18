using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Repositories;
using Services;
using ASPNetCoreMastersTodoList.Api.ApiModels;
using ASPNetCoreMastersTodoList.Api.Filters;
using Repositories.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ASPNetCoreMastersTodoList.Api.Authorization;
using Microsoft.AspNetCore.Authorization;

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

            //services.AddSingleton<DataContext>();
            services.AddScoped<IItemRepository, ItemRepository>();
            services.AddScoped<IItemService, ItemService>();

            services.Configure<Settings>(Configuration.GetSection("Authentication:JWT:SecurityKey"));
           
            // For Entity Framework
            services.AddDbContext<ASPNetCoreMastersTodoListApiContext>(options =>
                    options.UseSqlServer(
                        Configuration.GetConnectionString("ASPNetCoreMastersTodoListApiContextConnection")));

            // For Identity
            services.AddDefaultIdentity<ASPNetCoreMastersTodoListApiUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ASPNetCoreMastersTodoListApiContext>()
                .AddDefaultTokenProviders();                

            // Adding Authentication  
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                // Adding Jwt Bearer  
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidAudience = Configuration.GetSection("Authentication:JWT:Audience").Value,
                        ValidIssuer = Configuration.GetSection("Authentication:JWT:Issuer").Value,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("Authentication:JWT:SecurityKey").Value))
                    };
                });

            //Adding Authorization
            services.AddAuthorization(options => {
                options.AddPolicy("CanEditTodoItems",
                    policy => policy.Requirements.Add(new IsTodoListOwnerRequirement()));
            });

            services.AddScoped<IAuthorizationHandler, IsTodoListOwnerHandler>();
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
