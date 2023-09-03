using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

var wheelsPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "wheels");
if(Directory.GetFiles(wheelsPath).Length == 0)
    throw new InvalidOperationException(".\\wwwroot\\wheels is empty, macrobond-data-api package is missing!");

builder.Services.AddHttpClient();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

var srcPath = Path.Combine(builder.Environment.ContentRootPath, "..", "src");

app.UseDefaultFiles(new DefaultFilesOptions
{
    FileProvider = new PhysicalFileProvider(srcPath)
});
app.UseStaticFiles(new StaticFileOptions { ServeUnknownFileTypes = true });
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(srcPath),
    ServeUnknownFileTypes = true
});

app.UseMiddleware<ProxyMiddleware>();

app.Run();
