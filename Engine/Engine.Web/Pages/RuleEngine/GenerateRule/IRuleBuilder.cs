namespace Caspian.Engine
{
    internal interface IRuleBuilder<TEntity>
    {
        void AddRuleData(string propertyPath, string title);
    }
}
