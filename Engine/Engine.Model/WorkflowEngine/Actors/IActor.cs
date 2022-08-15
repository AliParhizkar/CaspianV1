
namespace Caspian.Engine
{
    public interface IActor<T>
    {
        /// <summary>
        /// با گرفتن کد یک اکتور، کد اکتور مافوق وی را برمی گرداند
        /// </summary>
        /// <param name="userId">کد اکتور</param>
        /// <returns>کد اکتور مافوق</returns>
        int GetMasterUserId(int userId);

        /// <summary>
        /// با گرفتن کد یک اکتور، کد اکتور مافوق مافوق وی را برمی گرداند
        /// </summary>
        /// <param name="userId">کد اکتور</param>
        /// <returns>کد اکتور مافوق مافوق</returns>
        int GetMasterOfMasterUserId(int userId);

        /// <summary>
        /// تمامی پست های سیستم را برمی گرداند
        /// </summary>
        /// <returns>پست های سیستم که بصورت شماره و عنوان پست برگردانده می شوند.</returns>
        IDictionary<string, string> GetOrganPosts();

        /// <summary>
        /// با گرفتن شماره ی پست کد اکتور(متصدی پست) را برمی گرداند
        /// </summary>
        /// <param name="postNo">شماره ی پست</param>
        /// <returns>(کد اکتور(متصدی پست</returns>
        int GetUserId(string postNo);

        /// <summary>
        /// براساس داده هایی که اکتور(ها) وارد کرده اند کد اکتور بعدی را برمی گرداند
        /// </summary>
        /// <param name="userData">داده هایی که اکتور(ها) وارد کرده اند </param>
        /// <returns>کد اکتور</returns>
        int GetUserId(T userData);
    }
}
