using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Pipaslot.Infrastructure.Mvc
{
    /// <inheritdoc />
    /// <summary>
    /// Allows to structure controllers like example below instead of separated controllers and views folders.
    /// Controllers
    ///     - Home
    ///         - Views
    ///             - Index.cshtml
    ///         - HomeController.cs 
    ///         - HomeModel.cs
    ///     - _Layout.cshtml
    ///     - _ViewImports.cshtml
    ///     - _ViewStart.cshtml
    /// 
    /// Require to be configured as RazorViewEngineOptions
    /// </summary>
    /// <code> 
    ///     //Startup.cs configuration: 
    ///     services.Configure<RazorViewEngineOptions>(options => {
    ///            options.ViewLocationExpanders.Add(new ViewLocationExpander("Controllers"));
    ///        });
    /// </code>
    public class ControllerViewLocationExpander : IViewLocationExpander
    {
        private readonly string _controllersFolder;

        public ControllerViewLocationExpander(string controllersFolder = "Controllers")
        {
            _controllersFolder = controllersFolder;
        }

        /// <inheritdoc />
        /// <summary>
        /// Used to specify the locations that the view engine should search to 
        /// locate views.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="viewLocations"></param>
        /// <returns></returns>
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            //{2} is area, {1} is controller,{0} is the action
            string[] locations = new string[]
            {
                "/"+_controllersFolder+"/{1}/Views/{0}.cshtml",
                "/"+_controllersFolder+"/{0}.cshtml"
            };
            return locations.Union(viewLocations);
        }


        public void PopulateValues(ViewLocationExpanderContext context)
        {
            context.Values["customviewlocation"] = nameof(ControllerViewLocationExpander);
        }
    }
}