using Caspian.Common;
using Caspian.Engine.Model;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Caspian.Engine.Service
{
    public class UserService: SimpleService<User>
    {
        public UserService(IServiceScope scope):
            base(scope)
        {
            RuleFor(t => t.UserName).Required().UniqAsync("نام کاربری با این عنوان در سیستم وجود دارد");

            RuleFor(t => t.LName).Required();

            RuleFor(t => t.Password).Required().CustomValue(t => t.HasValue() && t.Length < 8, "کلمه عبور باید 8 کاراکتر باشد")
                .CustomValue(t =>
                {
                    if (t != null)
                    {
                        foreach (var chr in t)
                            if (!Char.IsAscii(chr))
                                return true;
                    }
                    return false;
                }, "تنها از حروف لاتین و اعداد و کاراکترهای خاص برای کلمه ی عبور استفاده نمائید").
                CustomValue(t => 
                {
                    bool hasUppercase = false, haseLowercase = false, hasNumber = false;
                    if (t != null)
                    {
                        foreach (var chr in t)
                        {
                            if (char.IsDigit(chr))
                                hasNumber = true;
                            if (chr >= 'A' && chr < 'Z')
                                hasUppercase = true;
                            if (chr > 'a' && chr < 'z')
                                haseLowercase = true;
                        }
                        return !(hasUppercase && haseLowercase && hasNumber);
                    }
                    return false;
                }, "کلمه عبور باید شامل حروف کوچک و بزرگ لاتین و عدد باشد");

            RuleFor(t => t.Email).UniqAsync("کاربری با این پست الکترونیکی در سیستم ثبت شده است");

            RuleFor(t => t.MobileNumber).UniqAsync("کاربری با این شماره همراه در سیستم ثبت شده است");
        }

        public async override Task<User> AddAsync(User entity)
        {
            ///Md5 Code
            return await base.AddAsync(entity);
        }

        public async override Task UpdateAsync(User entity)
        {
            var old = await SingleAsync(entity.Id);
            entity.Password = old.Password;
            await base.UpdateAsync(entity);
        }

        public async Task<User> UserIsvalidAsync(string userName, string password)
        {
            ///Md5 Code
            var md5Password = password;
            var query = new UserService(ServiceScope).GetAll();
            return await query.SingleOrDefaultAsync(t => t.UserName == userName && t.Password == md5Password);
        }
    }
}
