using System.Web;
using System.Web.Mvc;

namespace Horrible_Charades_ASP
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
