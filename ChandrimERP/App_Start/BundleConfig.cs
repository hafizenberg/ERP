using System.Web.Optimization;
using WebHelpers.Mvc5;

namespace ChandrimERP.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Bundles/css")
                .Include("~/Content/css/bootstrap.min.css")
                .Include("~/Content/css/bootstrap-select.css")
                .Include("~/Content/css/bootstrap-datepicker3.min.css")
                .Include("~/Content/css/font-awesome.min.css")
                .Include("~/Content/css/icheck/blue.min.css")
                .Include("~/Content/css/AdminLTE.css")
                .Include("~/Content/css/skins/skin-blue.css")
                .Include("~/Content/css/chosen.css")
                .Include("~/Content/css/jquery-ui.min.css")   
                .Include("~/Content/css/jquery-ui.structure.min.css")
                .Include("~/Content/css/style.css")
                .Include("~/Content/jsTree/themes/default/style.css",
                    new CssRewriteUrlTransform()));

            bundles.Add(new ScriptBundle("~/Bundles/js")
                .Include("~/Content/js/plugins/jquery/jquery-3.3.1.js")
                .Include("~/Content/js/plugins/bootstrap/bootstrap.js")
                .Include("~/Content/js/plugins/fastclick/fastclick.js")
                .Include("~/Content/js/plugins/slimscroll/jquery.slimscroll.js")
                .Include("~/Content/js/plugins/bootstrap-select/bootstrap-select.js")
                .Include("~/Content/js/plugins/moment/moment.js")
                .Include("~/Content/js/plugins/datepicker/bootstrap-datepicker.js")
                .Include("~/Content/js/plugins/icheck/icheck.js")
                .Include("~/Content/js/plugins/validator.js")
                .Include("~/Content/js/plugins/inputmask/jquery.inputmask.bundle.js")
                .Include("~/Content/js/adminlte.js")
                .Include("~/Content/js/chosen.jquery.js")
                .Include("~/Content/js/chosen.jquery-ui.min.js")
                .Include("~/Content/js/init.js")
                .Include("~/Scripts/MvcGrid/mvc-grid.js")
                .Include("~/Scripts/gridmvc.min.js")
                .Include("~/Scripts/jsTree3/jstree.min.js")
                .Include("~/Scripts/customall.js")
                );


#if DEBUG
            BundleTable.EnableOptimizations = false;
#else
            BundleTable.EnableOptimizations = true;
#endif
        }
    }
}
