
var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5294);
});

var app =builder.Build();

app.Use(async (context, next) => {
   var isSecureRequest=  context.Request.Query["secure"]=="true";

    if (!isSecureRequest)
    {
        Console.WriteLine("Security Event: Unsecured HTTP request blocked. Missing 'secure=true' query parameter.");
        context.Response.StatusCode=400;
        await context.Response.WriteAsync("Simulated HTTPS is required.");
        return;
    }
    await next();
});

app.Use(async (context,next) =>{

    var userInput = context.Request.Query["input"].ToString();

    if (userInput.Contains("<script>"))
    {
        Console.WriteLine("Security Event: Unsafe input detected and blocked.");
        context.Response.StatusCode=400;
        await context.Response.WriteAsync("Invalid input");
        return;
    }

    await next();


});

app.Use(async(context, next)=>
{
    var IsAuthenticatedPAyh=context.Request.Path=="/unauthorized";

    if (IsAuthenticatedPAyh)
    {
        Console.WriteLine("Path is not authorized");
        context.Response.StatusCode=400;
        await  context.Response.WriteAsync("Unauthorized Access");
        return;

    }
    await next();
});


app.Use(async (context,next) =>
{
    bool isAuthenticated=context.Request.Query["authenticated"]=="true";

    if (!isAuthenticated)
    {
        Console.WriteLine("Security Event: Unauthorized access attempt blocked.");
        context.Response.StatusCode=403;
        await context.Response.WriteAsync("Access Denied");
        return;
    }

    await next();
});

app.Use(async (context,next) =>
{
    await Task.Delay(100);

    await context.Response.WriteAsync("Processed Asynchronously\n");
    await next();
});

app.Run(async context =>
{
    await context.Response.WriteAsync("Final Response from Application");
    
});

app.Run();