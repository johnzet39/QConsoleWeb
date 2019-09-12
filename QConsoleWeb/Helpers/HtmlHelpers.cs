using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QConsoleWeb.Helpers
{
    public static class HtmlHelpers
    {
        //https://stackoverflow.com/questions/20410623/how-to-add-active-class-to-html-actionlink-in-asp-net-mvc
        public static string IsActiveController(this IHtmlHelper htmlHelper, string controller)
        {
            var routeData = htmlHelper.ViewContext.RouteData;
            var routeController = routeData.Values["controller"].ToString();
            return (controller == routeController) ? "active" : "";
        }

        public static string IsActiveControllerAction(this IHtmlHelper htmlHelper, string controller, string action)
        {
            var routeData = htmlHelper.ViewContext.RouteData;

            var routeAction = routeData.Values["action"].ToString();
            var routeController = routeData.Values["controller"].ToString();

            return (controller == routeController && action == routeAction) ? "active" : "";
        }




        /// <summary>
        /// Gets an object containing a htmlAttributes collection for any Razor HTML helper component,
        /// supporting a static set (anonymous object) and/or a dynamic set (Dictionary)
        /// </summary>
        /// <param name="fixedHtmlAttributes">A fixed set of htmlAttributes (anonymous object)</param>
        /// <param name="dynamicHtmlAttributes">A dynamic set of htmlAttributes (Dictionary)</param>
        /// <returns>A collection of htmlAttributes including a merge of the given set(s)</returns>
        public static IDictionary<string, object> GetHtmlAttributes(
            object fixedHtmlAttributes = null,
            IDictionary<string, object> dynamicHtmlAttributes = null
            )
        {
            var rvd = (fixedHtmlAttributes == null)
                ? new RouteValueDictionary()
                : HtmlHelper.AnonymousObjectToHtmlAttributes(fixedHtmlAttributes);
            if (dynamicHtmlAttributes != null)
            {
                foreach (KeyValuePair<string, object> kvp in dynamicHtmlAttributes)
                    rvd[kvp.Key] = kvp.Value;
            }
            return rvd;
        }
    }
}


