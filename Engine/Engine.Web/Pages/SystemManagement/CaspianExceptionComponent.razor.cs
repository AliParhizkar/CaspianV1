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
        bool isDevelopment;
        bool showDetail;
        string saveErrorMessage;
        Exception exception;

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

        (string fileName, int? lineNumber) GetExceptionData(Exception exception)
        {
            var frame = GetFrame(exception);
            if (frame != null)
            {
                 var fileName = Path.GetFileName(frame.GetFileName());
                var fileLineNumber = frame.GetFileLineNumber();
                return (fileName, fileLineNumber);
            }
            return (null, null);
        }

        protected override async Task OnErrorAsync(Exception exception)
        {
            saveErrorMessage = null;
            try
            {
                if (isDevelopment)
                    this.exception = exception;
                else 
                    await SaveError(exception);
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    saveErrorMessage += ex.Message + "</br>";
                    ex = ex.InnerException;
                }
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
            
            using var scope = factory.CreateScope();
            if (PageData != null)
                scope.GetService<CaspianDataService>().UserId = PageData.UserId;
            var service = scope.GetService<ExceptionDataService>();
            var detailService = scope.GetService<ExceptionDetailService>();
            var ex = exception;
            (string fileName, int? lineNumber) exceptionData = default;
            while (ex != null)
            {
                var temp =  GetExceptionData(ex);
                if (temp.lineNumber.HasValue)
                    exceptionData = temp;
                ex = ex.InnerException;
            }
            var path = $"{host.ContentRootPath}\\Errors";
            string errorFileName;
            if (exceptionData.lineNumber.HasValue)
            {
                var old = await service.GetAll().SingleOrDefaultAsync(t => t.SourceCodeFileName == exceptionData.fileName &&
                    t.LineNumber == exceptionData.lineNumber);
                if (old == null)
                {
                    old = new ExceptionData()
                    {
                        LineNumber = (short)exceptionData.lineNumber,
                        ErrorFileName = Path.GetRandomFileName(),
                        RegisterDate = DateTime.Now,
                        RepetitionTimes = 1,
                        SubSystemKind = SystemKind,
                        SourceCodeFileName = exceptionData.fileName,
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
                            UserId = UserId.Value,
                            RegisterDate = DateTime.Now
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
                            UserId = UserId.Value,
                            RegisterDate = DateTime.Now
                        };
                        await detailService.AddAsync(detail);
                    }
                    await service.SaveChangesAsync();
                }
                errorFileName = old.ErrorFileName;
            }
            else
            {
                path = $"{path}\\Public";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                errorFileName = Path.GetRandomFileName();
            }

            path += $"\\{errorFileName}.xml";
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

        [Parameter]
        public SubsystemKind SystemKind { get; set; }

        [Parameter]
        public int? UserId { get; set; }
    }
}
