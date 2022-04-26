namespace Caspian.UI
{
    public enum SearchInitializedState
    {

        ParentInitialized,

        /// <summary>
        /// 
        /// </summary>
        ChildParameterInitialized,

        /// <summary>
        /// 
        /// </summary>
        DataFetched
    }

    public enum InitializedState
    {
        /// <summary>
        /// قبل از آنکه ستونهای گرید مقداردهی شوند
        /// </summary>
        GridColumnsNotInitialized,

        /// <summary>
        /// پس از آنکه فیلدهای گرید مقداردهی شدند
        /// </summary>
        GridFieldsIinitialized,

        /// <summary>
        /// بعد از اولین بازیابی دیتا
        /// </summary>
        FirstFetchData
    }
}
