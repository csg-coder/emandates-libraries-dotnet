using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Diagnostics;
using System.IO;
using System.Web.Hosting;
using System.Globalization;
using eMandates.Merchant.Library;
using eMandates.Merchant.Library.Configuration;

namespace eMandates.Merchant.Website
{
    class CustomTraceListener : TraceListener
    {
        public override void Write(string message)
        {
            lock(this)
            {
                using (var sw = new StreamWriter(Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "logs/info-log.txt"), true))
                {
                    sw.WriteLine(message);
                }
            }
        }

        public override void WriteLine(string message)
        {
            lock (this)
            {
                using (var sw = new StreamWriter(Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "logs/info-log.txt"), true))
                {
                    sw.WriteLine(message);
                }
            }
        }
    }

    public class DecimalModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            string val = valueProviderResult.AttemptedValue;

            val = val.Replace(",", CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator);
            val = val.Replace(".", CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator);

            decimal d;
            Decimal.TryParse(val, out d);

            return d == 0 ? base.BindModel(controllerContext, bindingContext) : d;
        }
    }

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            ModelBinders.Binders.Add(typeof(decimal), new DecimalModelBinder());

            Configuration.Load();

            Trace.Listeners.Add(new CustomTraceListener());
        }
    }
}
