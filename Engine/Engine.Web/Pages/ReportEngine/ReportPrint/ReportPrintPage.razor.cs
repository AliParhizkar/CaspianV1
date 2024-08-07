﻿using Caspian.UI;
using Caspian.Common;
using ReportUiModels;
using System.Xml.Linq;
using System.Reflection;
using Microsoft.JSInterop;
using Caspian.Engine.Service;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using System.Text.Json;

namespace Caspian.Engine.ReportPrint
{
    public partial class ReportPrintPage
    {
        Bond Bond;
        string message;
        bool pageRendered;
        int? MinDataLevel;
        WindowStatus status;
        ReportSetting reportSetting;
        ReportWindowType? windowType;
        ReportUiModels.ReportPrintPage Page;
        PrintParam PrintParam = new PrintParam();

        [Parameter]
        public int ReportId { get; set; } = 1;

        protected async override Task OnInitializedAsync()
        {
            if (GuId.HasValue())
            {
                var path = Assembly.GetExecutingAssembly().GetMapPath() + "\\Data\\Report\\JsonFile\\" + GuId + ".json";
                var json = File.ReadAllText(path);
                Page = JsonSerializer.Deserialize<ReportUiModels.ReportPrintPage>(json);
                UnescapeDataString(Page);
            }
            using var scope = ServiceScopeFactory.CreateScope();
            if (ReportId > 0)
            {
                var report = await scope.GetService<ReportService>().SingleAsync(ReportId);
                var reportParams = scope.GetService<ReportParamService>().GetAll().Where(t => t.ReportId == ReportId);
                if (! await reportParams.AnyAsync())
                {
                    message = "Please specify the report parameters first and then create the report file";
                    await base.OnInitializedAsync();
                }
                if (report.PrintFileName.HasValue())
                {
                    var path = Host.ContentRootPath + "\\Data\\Report\\View\\" + report.PrintFileName + ".mrt";
                    var doc = XDocument.Load(path);
                    var pagesElement = doc.Element("StiSerializer").Element("Pages").Elements();
                    Page = new ReportUiModels.ReportPrintPage(pagesElement.First());
                    Page.Report = new ReportUiModels.ReportPrint();
                    Page.Report.Pages = pagesElement.Select(t => new ReportUiModels.ReportPrintPage(t)).ToList();
                }
                else
                {
                    int count = reportParams.Max(t => t.DataLevel);
                    Page = new ReportUiModels.ReportPrintPage();
                    Page.Bonds = new List<Bond>();
                    for (int i = 0; i < count; i++)
                    {
                        Page.Bonds.Add(new Bond()
                        {
                            Height = 1,
                            DataLevel = count - i,
                            BondType = BondType.DataBond
                        });
                    }
                    Page.Width = 21;
                    Page.Height = 29.7M;
                }
                if (report.SubReportLevel.HasValue)
                {
                    if (Page.Report == null)
                        Page.Report = new ReportUiModels.ReportPrint();
                    Page.Report.SubReportLevel = report.SubReportLevel;
                }
            }
            if (DataModelId == 1)
            {
                Page = new ReportUiModels.ReportPrintPage();
                Page.Bonds = new List<Bond>()
                {
                    new Bond()
                    {
                        Height = 10,
                        BondType= BondType.PageHeader
                    }
                };
                Page.Width = 21;
                Page.Height = 29.7M;
            }
            await base.OnInitializedAsync();
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (message.HasValue())
            {
                await jsRunTime.InvokeVoidAsync("$.report.showMessage", message);
                message = null;
            }
            if (!pageRendered && Page != null)
            {
                pageRendered = true;
                var json = Page.GetJson();
                await jsRunTime.InvokeVoidAsync("$.report.reportBind", DotNetObjectReference.Create(this), json);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        [JSInvokable]
        public void ShowWindow(PrintParam param)
        {
            status = WindowStatus.Open;
            switch (param.BondType)
            {
                case BondType.ReportTitle:
                    break;
            }
            param.ReportId = ReportId;
            PrintParam = param;
            if (DataModelId > 0)
                windowType = ReportWindowType.WorkflowPrint;
            else
                windowType = ReportWindowType.Text;
            StateHasChanged();
        }

        [JSInvokable]
        public void ShowColumnWindow(Bond bond, int? minDataLevel)
        {
            if (DataModelId == 0)
            {
                Bond = bond;
                status = WindowStatus.Open;
                MinDataLevel = minDataLevel;
                windowType = ReportWindowType.ColumnWindow;
                StateHasChanged();
            }
        }

        [JSInvokable]
        public void ShowSettingWindow(ReportSetting setting)
        {
            status = WindowStatus.Open;
            windowType = ReportWindowType.Setting;
            reportSetting = setting;
            StateHasChanged();
        }

        [JSInvokable]
        public void ShowPictureBoxWindow(PrintParam param)
        {
            status = WindowStatus.Open;
            windowType = ReportWindowType.PictureBox;
            PrintParam = param;
            PrintParam.ReportId = ReportId;
            StateHasChanged();
        }

        [JSInvokable]
        public async Task ShowSubreportWindow(ReportUiModels.ReportPrintPage page)
        {
            UnescapeDataString(page);
            using var scope = ServiceScopeFactory.CreateScope();
            var report = await new ReportService(scope.ServiceProvider).SingleAsync(ReportId);
            var maxDataLevel = report.ReportParams.Max(t => t.DataLevel);
            page.Report = new ReportUiModels.ReportPrint();
            page.IsSubReport = true;
            page.Report.SubReportLevel = report.SubReportLevel;
            if (page.Bonds == null)
            {
                page.Bonds = new List<Bond>();
                if (maxDataLevel == 3 && report.SubReportLevel == SubReportLevel.Level1)
                {
                    page.Bonds.Add(new Bond()
                    {
                        Height = 1,
                        DataLevel = 2,
                        BondType = BondType.DataBond
                    });
                }
                page.Bonds.Add(new Bond()
                {
                    Height = 1,
                    DataLevel = 1,
                    BondType = BondType.DataBond
                });
            }
            Page = page;
            status = WindowStatus.Open;
            windowType = ReportWindowType.Subreport;
            var fileName = Path.GetRandomFileName().Replace(".", "");
            var path = Host.ContentRootPath + "\\App_Data\\Report\\JsonFile\\" + fileName + ".json";
            GuId = fileName;
            var json = page.GetJson();
            await File.WriteAllTextAsync(path, json);
            StateHasChanged();
        }

        [JSInvokable]
        public void ShowFormulaWindow(PrintParam param)
        {
            status = WindowStatus.Open;
            windowType = ReportWindowType.Formula;
            param.ReportId = ReportId;
            PrintParam = param;
            StateHasChanged();
        }

        [JSInvokable]
        public void ShowDigitsFormatWindow(PrintParam param)
        {
            status = WindowStatus.Open;
            windowType = ReportWindowType.DigitsFormat;
            PrintParam = param;
            StateHasChanged();
        }

        [JSInvokable]
        public async Task SaveData(ReportUiModels.ReportPrintPage page)
        {
            try
            {
                using var scope = ServiceScopeFactory.CreateScope();
                var reportService = new ReportService(scope.ServiceProvider);
                reportService.Context.ChangeTracker.LazyLoadingEnabled = true;
                var report = await reportService.GetAll().Where(t => t.Id == ReportId).Include(t => t.ReportGroup).SingleAsync();
                page.GuId = "3f7f0c9730b145ee9132cfdedc3c8ccd";
                var mainType = new AssemblyInfo().GetReturnType(report.ReportGroup);
                var reportParams = new ReportParamService(scope.ServiceProvider).GetAll().Where(t => t.ReportId == ReportId).ToList();
                var selectReport = new SelectReport(mainType);
                //var type = selectReport.SimpleSelect(reportParams).Body.Type;
                var type = selectReport.GetEqualType(reportParams);
                type = new ReportPrintEngine(scope.ServiceProvider).GetTypeOf(reportParams, type, mainType.Name);
                var reportPrint = new ReportUiModels.ReportPrint(type);
                reportPrint.Pages.Add(page);
                var path = Host.ContentRootPath;
                foreach (var bond in page.Bonds)
                {
                    if (bond.Table != null)
                    {
                        foreach (var cell in bond.Table.Cells)
                        {
                            if (cell.Text.HasValue())
                                cell.Text = Uri.UnescapeDataString(cell.Text);
                            if (cell.Member.HasValue())
                                cell.Member = Uri.UnescapeDataString(cell.Member);
                        }
                    }

                    foreach (var control in bond.Controls)
                    {
                        if (control.Text.HasValue())
                            control.Text = Uri.UnescapeDataString(control.Text);
                        if (control.Member.HasValue())
                            control.Member = Uri.UnescapeDataString(control.Member);
                        control.SystemFiledType = control.SystemFiledType;
                        control.SystemVariable = control.SystemVariable;
                        if (control.ImageFileName.HasValue())
                            control.ImageFileName = path + "\\Data\\Report\\Images/" + control.ImageFileName;
                        if (control.Type == ReportControlType.SubReport && control.SubReportPage != null)
                        {
                            control.SubReportPage.IsSubReport = true;
                            control.SubReportPage.GuId = control.Guid;
                            UnescapeDataString(control.SubReportPage);
                            reportPrint.Pages.Add(control.SubReportPage);
                        }
                    }
                }
                string fileName = null;
                if (report.PrintFileName.HasValue())
                    fileName = report.PrintFileName;
                else
                    fileName = Path.GetRandomFileName().Replace(".", "");
                path = path + "\\Data\\Report\\";
                var doc = reportPrint.GetXmlElement(ReportType.View);
                doc.Save(path + "View/" + fileName + ".mrt");
                doc = reportPrint.GetXmlElement(ReportType.Report);
                doc.Save(path + "Print/" + fileName + ".mrt");
                if (!report.PrintFileName.HasValue())
                {
                    report.PrintFileName = fileName;
                    await reportService.SaveChangesAsync();
                }
            }
            catch(Exception ex)
            {
                await Alert(ex.Message);
            }
        }

        private void UnescapeDataString(ReportUiModels.ReportPrintPage page)
        {
            if (page.Bonds == null)
                return;
            foreach (var bond in page.Bonds)
            {
                if (bond.Table != null)
                {
                    foreach (var cell in bond.Table.Cells)
                    {
                        if (cell.Text.HasValue())
                            cell.Text = Uri.UnescapeDataString(cell.Text);
                        if (cell.Member.HasValue())
                            cell.Member = Uri.UnescapeDataString(cell.Member);
                    }
                }
                foreach (var control in bond.Controls)
                {
                    if (control.Text.HasValue())
                        control.Text = Uri.UnescapeDataString(control.Text);
                    if (control.Member.HasValue())
                        control.Member = Uri.UnescapeDataString(control.Member);
                }
            }
        }
    }
}
