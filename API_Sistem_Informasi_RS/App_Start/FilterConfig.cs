using System.Web;
using System.Web.Mvc;

namespace API_Sistem_Informasi_RS
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
