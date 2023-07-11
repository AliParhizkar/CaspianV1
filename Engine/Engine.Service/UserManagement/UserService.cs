using Caspian.Common;
using Caspian.Engine.Model;
using Caspian.Common.Service;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace Caspian.Engine.Service
{
    public class UserService: BaseService<User>
    {
        public string OldPassword { get; set; }

        public UserService(IServiceProvider provider) :
            base(provider)
        {
            RuleFor(t => t.UserName).Required().UniqAsync("نام کاربری با این عنوان در سیستم وجود دارد");

            RuleFor(t => t.LName).Required();

            RuleFor(t => t.Password).Required(t => t.Id == 0)
                .Custom(t => t.Id == 0 && t.Password.HasValue() && t.Password.Length < 8, "کلمه عبور باید 8 کاراکتر باشد")
                .Custom(t =>
                {
                    if (t.Id > 0)
                        return false;
                    if (t.Password != null)
                    {
                        foreach (var chr in t.Password)
                            if (!Char.IsAscii(chr))
                                return true;
                    }
                    return false;
                }, "تنها از حروف لاتین و اعداد و کاراکترهای خاص برای کلمه ی عبور استفاده نمائید").
                Custom(t => 
                {
                    if (t.Id > 0)
                        return false;
                    bool hasUppercase = false, haseLowercase = false, hasNumber = false;
                    if (t.Password != null)
                    {
                        foreach (var chr in t.Password)
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

        public async Task<User> GetGuest()
        {
            return await GetAll().SingleOrDefaultAsync(t => t.UserName == "Guest");
        }

        public async override Task<User> AddAsync(User entity)
        {
            entity.Password = CreateMD5(entity.Password);
            return await base.AddAsync(entity);
        }

        public async override Task UpdateAsync(User entity)
        {
            var old = await SingleAsync(entity.Id);
            entity.Password = old.Password;
            entity.UserName = old.UserName;
            await base.UpdateAsync(entity);
        }

        public async Task<User> UserIsvalidAsync(string userName, string password)
        {
            var md5Password = CreateMD5(password);
            var query = new UserService(ServiceProvider).GetAll();
            return await query.SingleOrDefaultAsync(t => t.UserName == userName && t.Password == md5Password);
        }

        public static string CreateMD5(string input)
        {
            using var md5 = MD5.Create();
            var inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            return Convert.ToHexString(hashBytes);
        }
    }
}
