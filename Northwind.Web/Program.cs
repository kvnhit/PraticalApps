using Northwind.EntityModels;

var builder = WebApplication.CreateBuilder(args);

#region Configure the web servers host and services
builder.Services.AddRazorPages();

builder.Services.AddNorthwindContext();

var app = builder.Build();
#endregion

#region Configure the HTTP pipeline and routes
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.Use(async (HttpContext context, Func<Task> next) =>
{
    RouteEndpoint? rep = context.GetEndpoint() as RouteEndpoint;

    if(rep is not null)
    {
        WriteLine($"EndPoint Name: {rep.DisplayName}");
        WriteLine($"EndPoint Route Pattern: {rep.RoutePattern.RawText}");
    }
    if(context.Request.Path == "/bonjour")
    {
        await context.Response.WriteAsync("Bonjour Monde!");
        return;
    }
    await next();
});

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();


app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapGet("/hello", () => $"Environment is {app.Environment.EnvironmentName}");

#endregion

app.Run();
WriteLine("This executes after the web server has stopped!");
