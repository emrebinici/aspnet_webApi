using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace FlowDMApi.Core.Extentions
{
    public static  class ExportToHtml
    {
        public static string ToHtmlTable<T>(this List<T> listOfClassObjects, List<string> headerList, string title, Expression<Func<T, object>> excludedProperties)
        {
            var ret = new StringBuilder();
            if (listOfClassObjects == null || !listOfClassObjects.Any())
            {
                return ret.ToString();
            }

            ret.Append("<html><header><meta charset='utf-8'>");
            ret.Append(@"<style>
                        .row{
                        background-color:#FBAA00;
                        -webkit-print-color-adjust: exact;
                            }
                        #customers {
                          font-family: 'Trebuchet MS', Arial, Helvetica, sans-serif;
                          border-collapse: collapse;
                            font-size: 13px;
                            width:100% !important;
                        }
                        #customers td, #customers th {
                          border: 1px solid #ddd;
                          padding: 6px;
                          text-align: center;
                        }
                        #customers tr:nth-child(even){background-color: #FBAA00;}
                        #customers tr:hover {background-color: #ddd;}
                        #customers th {
                          padding-top: 10px;
                          padding-bottom: 10px;
                          text-align: center;
                          background-color: #0087CC;
                          color: white;
                          height: 20px;
                        }
                        </style> ");
            ret.Append("</header>");
            ret.Append("<body>");
            ret.Append("<table id='customers'> ");
            ret.Append(ToColumnHeaders(headerList, title));
            ret.Append(ToHtmlTableRow(listOfClassObjects, excludedProperties));
            ret.Append("</table>");
            ret.Append("</body></html>");
            return ret.ToString();
        }

        public static string ToColumnHeaders<T>(this List<T> listOfProperties, string title="")
        {
            var ret = string.Empty;

            return listOfProperties == null || !listOfProperties.Any()
                ? ret
                : "<tr> <th colspan='" + listOfProperties.Count + "'>   <center><h3>" + title + "</h3></center></th> " + "<tr>" +
                  listOfProperties.Aggregate(ret,
                      (current, propValue) =>
                          current +
                          ("<th>" + Convert.ToString(propValue) + "</th>")) +
                  "</tr>";
        }

        

        public static string ToHtmlTableRow<T>(List<T> listOfClassObjects, Expression<Func<T, object>> excludedProperties)
        {
            var html = new StringBuilder();
            var excludedFields = new List<string>();
            if (excludedProperties != null)
            {
                var memberExpr = excludedProperties.Body as MemberExpression;
                if (memberExpr == null)
                {
                    var properties = excludedProperties.Body.Type.GetProperties();
                    excludedFields.AddRange(properties.Select(propertyInfo => propertyInfo.Name));
                }
                else if (memberExpr.Member.MemberType == MemberTypes.Property)
                    excludedFields.Add(memberExpr.Member.Name);
            }

            foreach (var item in listOfClassObjects)
            {
                var columns = typeof(T).GetProperties().Select(p => p.Name);
                html.Append("<tr>");
                foreach (var column in columns)
                {
                    if (excludedFields.Contains(column)) continue;
                    var value = item.GetType().GetProperty(column)?.GetValue(item, null);
                    if (value != null && value.ToString().ToLower() == "true")
                    {
                        value = "Aktif";
                    }
                    else if(value != null && value.ToString().ToLower() == "false")
                    {
                        value = "Pasif";
                    }
                  
                    html.Append("<td>&nbsp;" + value + " </td>");
                }
                html.Append("</tr>");

            }
            return html.ToString();
        }
       
    }
}
