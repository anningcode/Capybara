using Capybara.IService;
using Capybara.Service;
using Microsoft.AspNetCore.Rewrite;

class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // 1. 注册CORS服务
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()    // 生产环境替换为你的前端域名
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddScoped<IAccoutService, AccoutService>();
        builder.Services.AddScoped<IGeneralService, GeneralService>();
        builder.Services.AddScoped<IModelService, ModelService>();
        builder.Services.AddScoped<IPromptService, PromptService>();
        builder.Services.AddScoped<IRoleService, RoleService>();
        builder.Services.AddScoped<ISkillService, SkillService>();
        builder.Services.AddScoped<IToolService, ToolService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IWebUserService, WebUserService>();

        var app = builder.Build();
        app.UseRouting();
        app.UseCors("AllowAll");
        app.UseAuthorization();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        // 指向静态文件目录
        app.UseStaticFiles();
        app.UseRewriter(new RewriteOptions().AddRedirect("^$", "account/index"));
        app.UseHttpsRedirection();
        app.UseWebSockets();
        app.MapControllers();

        app.Run();
    }
}