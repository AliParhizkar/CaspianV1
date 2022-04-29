namespace Caspian.Engine
{
    /// <summary>
    /// مشخصات روابط بین دو State را مشخص می کند
    /// </summary>
    public class Relation
    {
        /// <summary>
        /// کد State شروع
        /// </summary>
        public int FromStateId { get; set; }

        /// <summary>
        /// کد State مقصد
        /// </summary>
        public int ToStateId { get; set; }

        /// <summary>
        /// نوع توکن
        /// </summary>
        public TokenKind TokenKind { get; set; }
    }
}
