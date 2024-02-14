using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.WebEncoders;
using MyToDo.Models;
using System.Text.Unicode;


var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

//builder.Services.Add(new ServiceDescriptor(typeof(JoinService), new JoinService(builder.Configuration.GetConnectionString("MyToDoContext"))));
// Add services to the container.
services.AddControllersWithViews();

//// DB 연결 설정 (MyToDoContext 는 내가 직접 만드는 것)
services.AddDbContext<MyToDoContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("MyToDoContext")));

// ASP.NET Core 에서 한글 문자열이 인코딩되어 출력되는 문제 해결
services.Configure<WebEncoderOptions>(options =>
    {
        options.TextEncoderSettings = new System.Text.Encodings.Web.TextEncoderSettings(UnicodeRanges.All);
    });

// IDistributedCache의 기본 메모리 내 구현을 사용해 메모리 내 세션 공급자를 설정
services.AddDistributedMemoryCache();
services.AddAuthentication("Cookies").AddCookie("Cookies", config =>
{
    config.ExpireTimeSpan = TimeSpan.FromMinutes(1);
    config.LoginPath = "/Member/Login";
    config.LogoutPath = "/Member/Logout";
    config.Cookie.Name = "Test.Cookie";
});
services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(1); // 세션 만료시간
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
// 컨트롤러 외 기타 클래스에서 세션 값을 접근할 수 있도록 의존성 추가
services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
/// 다음과 같은 방법으로 세션 값 접근
/// using Microsoft.AspNetCore.Http;
/// ...
/// HttpContextAccessor _Ac = new HttpContextAccessor();
/// string Id = _Ac.HttpContext.Session.GetString("YourSessionName");




var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    try
    {
        var context = new MyToDoContext(
                serviceProvider.GetRequiredService<DbContextOptions<MyToDoContext>>());

        SeedData.Initialize(context);
    }
    catch (Exception e)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(e, "An error occurred creating the DB.");
    }
 

}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
// IDistributedCache의 기본 메모리 내 구현을 사용해 메모리 내 세션 공급자를 설정
app.UseSession();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=ToDo}/{action=Index}/{id?}");

app.Run();
