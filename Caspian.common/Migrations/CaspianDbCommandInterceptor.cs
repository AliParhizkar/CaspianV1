using Caspian.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caspian.common.Migrations
{
    internal class CaspianDbCommandInterceptor: DbCommandInterceptor
    {
        public override void CommandFailed(DbCommand command, CommandErrorEventData eventData)
        {
            eventData.Exception.Data.Add("SqlCommandText", command.CommandText);
            base.CommandFailed(command, eventData);
        }
    }
}
