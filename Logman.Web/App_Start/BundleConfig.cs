using System.Web.Optimization;

namespace Logman.Web.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/Content/Scripts/").Include(
                "~/Content/Scripts/jquery.js",
                "~/Content/Scripts/jsapi.js",
                "~/Content/Scripts/site.js",
                "~/Content/Scripts/jquery.modal.js",
                "~/Content/Scripts/Base64.js",
                "~/Content/Scripts/date.js",
                "~/Content/Scripts/jquery.validate.js",
                "~/Content/Scripts/additional-methods.js",
                "~/Content/Scripts/bootstrap-switch.js"));

            bundles.Add(new StyleBundle("~/Content/Styles/").Include("~/Content/Styles/site.css", "~/Content/Styles/jquery.modal.css",
                "~/Content/Styles/bootstrap-switch.css"));
            bundles.Add(new StyleBundle("~/Content/Bootstrap/css/").Include("~/Content/Bootstrap/css/bootstrap.css").Include("~/Content/Bootstrap/css/bootswatch.less"));
            bundles.Add(new ScriptBundle("~/Content/Bootstrap/scripts/").Include("~/Content/Bootstrap/js/bootstrap.js"));
        }
    }
}