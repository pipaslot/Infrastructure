# ASP.NET Core MVC Tools

Allows to structure controllers like example below instead of separated controllers and views folders.

- Controllers
  - Home
    - Views
      - Index.cshtml
    - HomeController.cs 
    - HomeModel.cs
  - _Layout.cshtml
  - _ViewImports.cshtml
  - _ViewStart.cshtml

Requires to be configured as RazorViewEngineOptions
```
//Startup.cs configuration: 
services.Configure<RazorViewEngineOptions>(options => {
    options.ViewLocationExpanders.Add(new ControllerViewLocationExpander("Controllers"));
});
```