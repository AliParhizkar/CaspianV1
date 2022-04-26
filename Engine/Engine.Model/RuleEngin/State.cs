namespace Caspian.Engine
{
    /// <summary>
    /// مشخصات State
    /// </summary>
    public class State
    {
        /// <summary>
        /// کد State
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// وضعیت State
        /// </summary>
        public StateStatus Status { get; set; }
    }
}
