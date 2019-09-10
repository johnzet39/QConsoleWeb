using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace QConsoleWeb.Components
{

    public class NavigationMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            //ViewBag.SelectedCategory = RouteData?.Values["controller"];
            return View();
        }
    }
}
