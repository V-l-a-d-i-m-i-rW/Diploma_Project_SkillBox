using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkillProfi_WebAPI.Classes;

namespace SkillProfi_WebAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {

            //������ ����������� � �� � �������
            string data_connection = Configuration.GetConnectionString("DataConnection");
            services.AddDbContext<SkillProfiContext>(options => options.UseSqlServer(data_connection));
            services.AddControllers(); // ���������� ����������� ��� �������������
        }
        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); // ���������� ������������� �� �����������
            });
        }
    }
}
