﻿using Demo.Model;
using Caspian.Common;
using FluentValidation;
using Caspian.Common.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Demo.Service
{
    public class OrganUnitService : BaseService<OrganUnit>, IBaseService<OrganUnit>
    {
        public OrganUnitService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqAsync("سازمانی با این عنوان در سیستم وجود دارد");
            RuleFor(t => t.ActiveType).CustomAsync(async t =>
            {
                if (t.ActiveType == ActiveType.Enable || t.Id == 0)
                    return false;
                var result = await GetAll().AnyAsync(u => u.ParentOrganId == t.Id && u.ActiveType == ActiveType.Enable);
                return result;
            }, "برای غیرفعال شدن واحد سازمانی تمامی زیرمجموعه های آن باید غیرفعال باشند");
        }
    }
}
