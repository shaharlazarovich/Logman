using System.Linq;
using System.Text;
using System.Web.Mvc;
using Logman.Web.Models.Shared;
using WebGrease.Css.Extensions;

namespace Logman.Web.Code.Classes
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString RenderGauges(this HtmlHelper instance)
        {
            var sbuilder = new StringBuilder();
            ViewStartViewModel viewModel = Util.GetLayoutViewModel();
            if (viewModel != null)
            {
                if (viewModel.Gauges.Any())
                {
                    sbuilder.AppendLine(
                        "var gaugeOptions = {width: 300,height: 100,redFrom: 80,redTo: 100,yellowFrom: 50,yellowTo: 79};");

                    int index = 1;
                    foreach (GaugeData gauge in viewModel.Gauges)
                    {
                        sbuilder.AppendLine(
                            string.Format("var gdata{0} = google.visualization.arrayToDataTable([['Label', 'Value'],",
                                index));


                        sbuilder.AppendLine((string.Format("['{0}',{1}]]);", gauge.Label, gauge.Value)));

                        sbuilder.AppendLine(
                            string.Format(
                                "var gchart{1} = new google.visualization.Gauge(document.getElementById('{0}'));",
                                gauge.ContainerName, index));
                        sbuilder.AppendLine(string.Format("gchart{0}.draw(gdata{0}, gaugeOptions); ", index));

                        index++;
                    }
                }
            }
            return new MvcHtmlString(sbuilder.ToString());
        }

        public static MvcHtmlString RenderLines(this HtmlHelper instance)
        {
            var sbuilder = new StringBuilder();
            ViewStartViewModel viewModel = Util.GetLayoutViewModel();
            if (viewModel != null)
            {
                if (viewModel.Lines.Any())
                {
                    int index = 1;
                    foreach (LineData line in viewModel.Lines)
                    {
                        sbuilder.AppendLine(string.Format("var ldata{0} = google.visualization.arrayToDataTable([",
                            index));
                        sbuilder.Append(string.Format("['{0}','{1}'],", line.XAxisName, line.YAxisName));
                        line.Data.ForEach(l => { sbuilder.AppendLine(string.Format("['{0}',{1}],", l.Key, l.Value)); });
                        sbuilder.AppendLine("]);");

                        sbuilder.AppendLine(string.Format("var loptions{0} = {{title: '{1}'}};", index, line.ChartTitle));

                        sbuilder.AppendLine(
                            string.Format(
                                "var lchart{0} = new google.visualization.LineChart(document.getElementById('{1}'));",
                                index, line.ContainerName));
                        sbuilder.AppendLine(
                            string.Format("lchart{0}.draw(ldata{0}, loptions{0});", index));


                        index++;
                    }
                }
            }
            return new MvcHtmlString(sbuilder.ToString());
        }
        
    }
}