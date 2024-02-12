using Microsoft.EntityFrameworkCore;
using MyToDo.Models;


var builder = WebApplication.CreateBuilder(args);

//builder.Services.Add(new ServiceDescriptor(typeof(JoinService), new JoinService(builder.Configuration.GetConnectionString("MyToDoContext"))));
// Add services to the container.
builder.Services.AddControllersWithViews();

//// MyToDoContext 는 내가 직접 만드는 것
builder.Services.AddDbContext<MyToDoContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("MyToDoContext")));


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = new MyToDoContext(
                services.GetRequiredService<DbContextOptions<MyToDoContext>>());

        SeedData.Initialize(context);
    }
    catch (Exception e)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Member}/{action=Login}/{id?}");

app.MapControllerRoute(
    name: "content",
    pattern: "{controller=ToDo}/{action=Index}/{id?}");

app.Run();
