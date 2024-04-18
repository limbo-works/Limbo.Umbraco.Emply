using System.Collections.Generic;
using System.Text;
using Limbo.Umbraco.Emply.Models.Import;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Umbraco.Extensions;

namespace Limbo.Umbraco.Emply.Extensions;

public static class HtmlHelperExtensions {

    public static IHtmlContent RenderTasks(this IHtmlHelper helper, IEnumerable<ImportTask> tasks) {

        StringBuilder sb = new();

        sb.AppendLine("<div class=\"emply-tasks\">");
        sb.AppendLine("<ul>");

        foreach (var task in tasks) {
            sb.Append(helper.Partial("~/Views/Partials/Emply/ImportTask.cshtml", task).ToHtmlString());
        }

        sb.AppendLine("</ul>");
        sb.AppendLine("</div>");

        return new HtmlString(sb.ToString());

    }

    public static IHtmlContent RenderSingleTask(this IHtmlHelper helper, ImportTask task) {

        StringBuilder sb = new();

        sb.AppendLine("<div class=\"emply-tasks\">");
        sb.AppendLine("<ul>");

        sb.Append(helper.Partial("~/Views/Partials/Emply/ImportTask.cshtml", task).ToHtmlString());

        sb.AppendLine("</ul>");
        sb.AppendLine("</div>");

        return new HtmlString(sb.ToString());

    }

    public static IHtmlContent RenderTask(this IHtmlHelper helper, ImportTask task) {
        return helper.Partial("~/Views/Partials/Emply/ImportTask.cshtml", task);
    }

}