using Caspian.Common;
using System.Xml.Linq;
using System.Diagnostics;
using Caspian.Engine.Model;
using Caspian.Engine.Service;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Caspian.Engine.Shared
{
    public partial class CaspianExceptionComponent: ErrorBoundary
    {
        string fileName;
        short? fileLineNumber;
        string errorMessage;
        string stackTraceMessage;
        bool isDevelopment;
        bool showDetail;
        string saveErrorMessage;

        protected override void OnInitialized()
        {
            isDevelopment = host.IsDevelopment();

            base.OnInitialized();
        }

        public new void Recover()
        {
            showDetail = false;
            saveErrorMessage = null;
            base.Recover();
        }

        void ShowDetails()
        {
            showDetail = true;
        }

        protected override async Task OnErrorAsync(Exception exception)
        {
            saveErrorMessage = null;
            while (exception.InnerException != null)
                exception = exception.InnerException;
            try
            {
                errorMessage = exception.Message;
                stackTraceMessage = exception.StackTrace;
                if (isDevelopment)
                {
                    var frame = GetFrame(exception);
                    if (frame != null)
                    {
                        fileName = Path.GetFileName(frame.GetFileName());
                        fileLineNumber = (short)frame.GetFileLineNumber();
                    }
                }
                else
                    await SaveError(exception);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                saveErrorMessage = ex.Message;
                StateHasChanged();
            }
            await base.OnErrorAsync(exception);
        }

        StackFrame GetFrame(Exception exception)
        {
            var st = new StackTrace(exception, true);
            return st.GetFrames().FirstOrDefault(t => t.HasSource());
        }

        [CascadingParameter]
        public PageData PageData { get; set; }

        [Parameter]
        public bool IsWebAssembly { get; set; }

        async Task SaveError(Exception exception)
        {
            if (IsWebAssembly)
                return;
            var ex = exception;
            while (ex.InnerException != null)
                ex = ex.InnerException;
            var frame = GetFrame(ex);
            using var scope = factory.CreateScope();
            if (PageData != null)
                scope.GetService<CaspianDataService>().UserId = PageData.UserId;
            var service = scope.GetService<ExceptionDataService>();
            var detailService = scope.GetService<ExceptionDetailService>();
            ExceptionData old = null;
            if (frame != null)
            {
                fileName = Path.GetFileName(frame.GetFileName());
                fileLineNumber = (short)frame.GetFileLineNumber();
                old = await service.GetAll().SingleOrDefaultAsync(t => t.SourceCodeFileName == fileName &&
                    t.LineNumber == fileLineNumber);
            }
            if (old == null)
            {
                old = new ExceptionData()
                {
                    LineNumber = fileLineNumber,
                    ErrorFileName = Path.GetRandomFileName(),
                    RepetitionTimes = 1,
                    SubSystemKind = SystemKind,
                    SourceCodeFileName = fileName,
                    Version = "1.1.1.1"
                };
                var transaction = await service.Context.Database.BeginTransactionAsync();
                await service.AddAsync(old);
                await service.SaveChangesAsync();
                if (UserId.HasValue)
                {
                    var detail = new ExceptionDetail()
                    {
                        ExceptionDataId = old.Id,
                        UserId = UserId.Value
                    };
                    await detailService.AddAsync(detail);
                    await service.SaveChangesAsync();
                }
                await transaction.CommitAsync();
            }
            else
            {
                old.RepetitionTimes = Convert.ToInt16(old.RepetitionTimes + 1);
                if (UserId.HasValue)
                {
                    var detail = new ExceptionDetail()
                    {
                        ExceptionDataId = old.Id,
                        UserId = UserId.Value
                    };
                    await detailService.AddAsync(detail);
                }
                await service.SaveChangesAsync();
            }
            var path = host.ContentRootPath + "\\Errors\\" + old.ErrorFileName + ".xml";
            if (!File.Exists(path))
            {
                ex = exception;
                var doc = new XElement("Errors");
                while (ex != null)
                {
                    var xElement = new XElement("Error").AddElement("Message", exception.Message).AddElement("StackTrace", exception.StackTrace);
                    doc.AddElement(xElement);
                    ex = ex.InnerException;
                }
                doc.Save(path);
            }
        }

        [Parameter]
        public SubSystemKind SystemKind { get; set; }

        [Parameter]
        public int? UserId { get; set; }
    }
}
