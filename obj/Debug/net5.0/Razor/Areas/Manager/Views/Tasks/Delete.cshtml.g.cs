#pragma checksum "D:\KRU\Areas\Manager\Views\Tasks\Delete.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "c41a2290b6e9fb73e4c5ee1c052de2f7eb6fa35f"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Areas_Manager_Views_Tasks_Delete), @"mvc.1.0.view", @"/Areas/Manager/Views/Tasks/Delete.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "D:\KRU\Areas\Manager\Views\_ViewImports.cshtml"
using KRU;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\KRU\Areas\Manager\Views\_ViewImports.cshtml"
using KRU.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c41a2290b6e9fb73e4c5ee1c052de2f7eb6fa35f", @"/Areas/Manager/Views/Tasks/Delete.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"0e29578f1c1cd1ba9ebbd71298adc0a93a63721f", @"/Areas/Manager/Views/_ViewImports.cshtml")]
    public class Areas_Manager_Views_Tasks_Delete : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<KRU.Models.Tasks>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-area", "Manager", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-controller", "FileHistories", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "Download_ConfigEnd", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("btn btn-outline-primary mr-3 no-outline"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_4 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "Index", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_5 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("type", "hidden", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_6 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "Delete", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.InputTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 3 "D:\KRU\Areas\Manager\Views\Tasks\Delete.cshtml"
  
    ViewData["Title"] = "Delete";

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
<div class=""card box-shadow card-simple-form"">
    <h1>Ўчириш</h1>

    <h3>Ҳақиқатан ҳам ўчиришни хоҳлайсизми?</h3>
    <div>
        <h4>Топшириқ</h4>
      
        <div class=""row"">
            <table class=""table table-hover"">
                <tbody>
                    <tr>
                        <td>Масъул ходим</td>
                        <td>
                           
                                ");
#nullable restore
#line 21 "D:\KRU\Areas\Manager\Views\Tasks\Delete.cshtml"
                           Write(Html.DisplayFor(modelItem => modelItem.Employee.User.FullName));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                             \r\n                        </td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td>Бажарувчи ходим</td>\r\n                        <td>\r\n");
#nullable restore
#line 28 "D:\KRU\Areas\Manager\Views\Tasks\Delete.cshtml"
                             foreach (var i in Model.Task_Emples)
                            {
                                

#line default
#line hidden
#nullable disable
#nullable restore
#line 30 "D:\KRU\Areas\Manager\Views\Tasks\Delete.cshtml"
                           Write(Html.DisplayFor(modelItem => i.Employee.User.FullName));

#line default
#line hidden
#nullable disable
            WriteLiteral("                                <br />\r\n");
#nullable restore
#line 32 "D:\KRU\Areas\Manager\Views\Tasks\Delete.cshtml"
                            }

#line default
#line hidden
#nullable disable
            WriteLiteral("                        </td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td>Ёъқотилган маблағ</td>\r\n                        <td>");
#nullable restore
#line 37 "D:\KRU\Areas\Manager\Views\Tasks\Delete.cshtml"
                       Write(Html.DisplayFor(model => model.SumLost));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td>Ортирилган маблағ</td>\r\n                        <td>");
#nullable restore
#line 41 "D:\KRU\Areas\Manager\Views\Tasks\Delete.cshtml"
                       Write(Html.DisplayFor(model => model.SumGain));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td>Изоҳ</td>\r\n                        <td>");
#nullable restore
#line 45 "D:\KRU\Areas\Manager\Views\Tasks\Delete.cshtml"
                       Write(Html.DisplayFor(model => model.Comment));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td>Якунланган</td>\r\n                        <td>\r\n                            ");
#nullable restore
#line 50 "D:\KRU\Areas\Manager\Views\Tasks\Delete.cshtml"
                       Write(Html.DisplayFor(model => model.Finished));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
#nullable restore
#line 51 "D:\KRU\Areas\Manager\Views\Tasks\Delete.cshtml"
                             if (Model.File == "" || Model.File == null)
                            {

                            }
                            else
                            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                                ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "c41a2290b6e9fb73e4c5ee1c052de2f7eb6fa35f9447", async() => {
                WriteLiteral("\r\n                                    ");
#nullable restore
#line 59 "D:\KRU\Areas\Manager\Views\Tasks\Delete.cshtml"
                               Write(Html.DisplayFor(model => model.File));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n                                ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Area = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Controller = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_2.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_2);
            if (__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues == null)
            {
                throw new InvalidOperationException(InvalidTagHelperIndexerAssignment("asp-route-TaskId", "Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper", "RouteValues"));
            }
            BeginWriteTagHelperAttribute();
#nullable restore
#line 58 "D:\KRU\Areas\Manager\Views\Tasks\Delete.cshtml"
                                         WriteLiteral(Model.TaskId);

#line default
#line hidden
#nullable disable
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["TaskId"] = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-route-TaskId", __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["TaskId"], global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n");
#nullable restore
#line 61 "D:\KRU\Areas\Manager\Views\Tasks\Delete.cshtml"
                            }

#line default
#line hidden
#nullable disable
            WriteLiteral("                        </td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td>Бошланиш санаси</td>\r\n                        <td>");
#nullable restore
#line 66 "D:\KRU\Areas\Manager\Views\Tasks\Delete.cshtml"
                       Write(Html.DisplayFor(model => model.TaskStarted));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td>Тугаш санаси</td>\r\n                        <td>");
#nullable restore
#line 70 "D:\KRU\Areas\Manager\Views\Tasks\Delete.cshtml"
                       Write(Html.DisplayFor(model => model.TaskEnd));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td>Департамент</td>\r\n                        <td>");
#nullable restore
#line 74 "D:\KRU\Areas\Manager\Views\Tasks\Delete.cshtml"
                       Write(Html.DisplayFor(model => model.Department.DepartmentName));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td>Вазифа тури</td>\r\n                        <td>");
#nullable restore
#line 78 "D:\KRU\Areas\Manager\Views\Tasks\Delete.cshtml"
                       Write(Html.DisplayFor(model => model.Task_Type.NameType));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"</td>
                    </tr>
                </tbody>
            </table>

            <table class=""mt-5 table bg-color-1 table-striped table-hover"">
                <thead>
                    <tr>
                        <th colspan=""3""><h5 class=""m-0"" align=""center"">Ҳисоботлар:</h5></th>
                    </tr>
                    <tr>
                        <th>
                           Сана
                        </th>
                        <th>
                            Ходим ҳолати
                        </th>
                        
                    </tr>
                </thead>
                <tbody>
");
#nullable restore
#line 99 "D:\KRU\Areas\Manager\Views\Tasks\Delete.cshtml"
                 if (Model.Reports == null)
                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                    <tr>\r\n                        Ҳеч нарса топилмади\r\n                    </tr>\r\n");
#nullable restore
#line 104 "D:\KRU\Areas\Manager\Views\Tasks\Delete.cshtml"
                }
                else
                {
                

#line default
#line hidden
#nullable disable
#nullable restore
#line 107 "D:\KRU\Areas\Manager\Views\Tasks\Delete.cshtml"
                 foreach (var r in Model.Reports)
                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                    <tr>\r\n                        <td>\r\n                            ");
#nullable restore
#line 111 "D:\KRU\Areas\Manager\Views\Tasks\Delete.cshtml"
                       Write(Html.DisplayFor(modelItem => r.ReportDate));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                        </td>\r\n                        <td>\r\n                            ");
#nullable restore
#line 114 "D:\KRU\Areas\Manager\Views\Tasks\Delete.cshtml"
                       Write(Html.DisplayFor(modelItem => r.State));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                        </td>\r\n                    </tr>\r\n");
#nullable restore
#line 117 "D:\KRU\Areas\Manager\Views\Tasks\Delete.cshtml"
                }

#line default
#line hidden
#nullable disable
#nullable restore
#line 117 "D:\KRU\Areas\Manager\Views\Tasks\Delete.cshtml"
                 }

#line default
#line hidden
#nullable disable
            WriteLiteral(@"                </tbody>
            </table>
            <table class=""mt-5 table table-hover table-success table-striped"">
                <thead>
                    <tr>
                        <th colspan=""4""><h5 class=""m-0"" align=""center"">Текширувлар ҳисоботи:</h5></th>
                    </tr>
                    <tr>
                        <th>
                            Ходим ҳолати
                        </th>
                        <th>
                            Бошланиш санаси
                        </th>
                        <th>
                            Тугаш санаси
                        </th>
                       
                    </tr>
                </thead>
                <tbody>
");
#nullable restore
#line 139 "D:\KRU\Areas\Manager\Views\Tasks\Delete.cshtml"
                 if (Model.FinanceReports == null)
                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                    <tr>\r\n                        Ҳеч нарса топилмади\r\n                    </tr>\r\n");
#nullable restore
#line 144 "D:\KRU\Areas\Manager\Views\Tasks\Delete.cshtml"
                }
                else
                {
                

#line default
#line hidden
#nullable disable
#nullable restore
#line 147 "D:\KRU\Areas\Manager\Views\Tasks\Delete.cshtml"
                 foreach (var f in Model.FinanceReports)
                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                <tr>\r\n\r\n                    <td>\r\n                        ");
#nullable restore
#line 152 "D:\KRU\Areas\Manager\Views\Tasks\Delete.cshtml"
                   Write(Html.DisplayFor(modelItem => f.Status));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    </td>\r\n                    <td>\r\n                        ");
#nullable restore
#line 155 "D:\KRU\Areas\Manager\Views\Tasks\Delete.cshtml"
                   Write(Html.DisplayFor(modelItem => f.StartDate));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n\r\n                    </td>\r\n                    <td>\r\n                        ");
#nullable restore
#line 159 "D:\KRU\Areas\Manager\Views\Tasks\Delete.cshtml"
                   Write(Html.DisplayFor(modelItem => f.EndDate));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n\r\n                    </td>\r\n                </tr>\r\n");
#nullable restore
#line 163 "D:\KRU\Areas\Manager\Views\Tasks\Delete.cshtml"

                }

#line default
#line hidden
#nullable disable
#nullable restore
#line 164 "D:\KRU\Areas\Manager\Views\Tasks\Delete.cshtml"
                 }

#line default
#line hidden
#nullable disable
            WriteLiteral("                </tbody>\r\n            </table>\r\n        </div>\r\n        ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "c41a2290b6e9fb73e4c5ee1c052de2f7eb6fa35f19471", async() => {
                WriteLiteral("\r\n            <div class=\"form-button-container\">\r\n                ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "c41a2290b6e9fb73e4c5ee1c052de2f7eb6fa35f19799", async() => {
                    WriteLiteral("\r\n                    <i class=\"fas fa-arrow-left mr-2\" ></i>Ортга қайтиш\r\n                ");
                }
                );
                __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_3);
                __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_4.Value;
                __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_4);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n                ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("input", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "c41a2290b6e9fb73e4c5ee1c052de2f7eb6fa35f21213", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.InputTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper);
                __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.InputTypeName = (string)__tagHelperAttribute_5.Value;
                __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_5);
#nullable restore
#line 173 "D:\KRU\Areas\Manager\Views\Tasks\Delete.cshtml"
__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.For = ModelExpressionProvider.CreateModelExpression(ViewData, __model => __model.TaskId);

#line default
#line hidden
#nullable disable
                __tagHelperExecutionContext.AddTagHelperAttribute("asp-for", __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.For, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n");
#nullable restore
#line 174 "D:\KRU\Areas\Manager\Views\Tasks\Delete.cshtml"
                 if (Model.FinCount == 0 && Model.RepCount == 0)
                {

#line default
#line hidden
#nullable disable
                WriteLiteral("                <input type=\"submit\" value=\"Ўчириш\" class=\"btn btn-danger\"/> ");
#nullable restore
#line 176 "D:\KRU\Areas\Manager\Views\Tasks\Delete.cshtml"
                                                                             }

#line default
#line hidden
#nullable disable
                WriteLiteral("            </div>\r\n        ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Action = (string)__tagHelperAttribute_6.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_6);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n    </div>\r\n</div>");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<KRU.Models.Tasks> Html { get; private set; }
    }
}
#pragma warning restore 1591
