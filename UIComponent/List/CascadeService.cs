using System.Linq.Expressions;
using Caspian.Common.Extension;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public interface ICascadeService
    {
        void Update(bool isParent);
    }
    public interface ICascadeService<TEntity> where TEntity:class
    {
        void Initialize(IComboBox<TEntity> cmbParent);
    }

    public class CascadeService<TParent>:ICascadeService<TParent> where TParent: class     
    {
        protected IComboBox<TParent> cmbParent;
        protected object parentValue;
        protected ICascadeService ChildService;

        public void Initialize(IComboBox<TParent> cmbParent)
        {
            this.cmbParent = cmbParent;
            ///When the parent combobox is created, the child combobox has not yet been created
            ///So we set "parentValue" to be used for the filter when creating the Child Combobox
            cmbParent.OnInternalValueChanged = EventCallback.Factory.Create<object>(this, value => 
            {
                parentValue = value;
                ChildService?.Update(true);
            });
        }
    }

    public class CascadeService<TParent, TChild> : CascadeService<TParent>, ICascadeService, ICascadeService<TChild> where TParent :class where TChild : class
    {
        protected IComboBox<TChild> cmbChild;
        protected object childValue;
        protected ICascadeService GrandChildService;


        public void Initialize(IComboBox<TChild> cmbChild)
        {
            this.cmbChild = cmbChild;
            SetInternalExpression(parentValue);
            base.ChildService = this;
            ///When the Child Combobox is created, the GrandChild Combobox has not yet been created
            ///So we set "childValue" to be used for the filter when creating the GrandChild Combobox
            cmbChild.OnInternalValueChanged = EventCallback.Factory.Create<object>(this, value => 
            {
                childValue = value;
                GrandChildService?.Update(false);
            });
        }

        public virtual void Update(bool isParent)
        {
            SetInternalExpression(parentValue);
            cmbChild.UpdateCascadeComboBox();
        }

        protected void SetInternalExpression(object value)
        {
            var parameter = Expression.Parameter(typeof(TChild), "t");
            var info = typeof(TChild).GetForeignKey(typeof(TParent));
            Expression expr = Expression.Property(parameter, info);
            if (info.PropertyType.IsNullableType())
                expr = Expression.Property(expr, "Value");
            else
                value = value ?? 0;
            value = Convert.ChangeType(value, info.PropertyType);
            expr = Expression.Equal(expr, Expression.Constant(value));
            expr = Expression.Lambda(expr, parameter);
            cmbChild.InternalConditionExpression = expr as Expression<Func<TChild, bool>>;
        }
    }

    public class CascadeService<TParent, TChild, TGrandChild>: CascadeService<TParent, TChild>, ICascadeService<TGrandChild> 
        where TParent : class where TChild : class where TGrandChild : class
    {
        IComboBox<TGrandChild> cmbGrandChild;
        public void Initialize(IComboBox<TGrandChild> cmbGrandChild)
        {
            base.GrandChildService = this;
            this.cmbGrandChild = cmbGrandChild;
            SetInternalExpression();
        }

        void SetInternalExpression()
        {
            var value = childValue;
            var parameter = Expression.Parameter(typeof(TGrandChild), "t");
            var info = typeof(TGrandChild).GetForeignKey(typeof(TChild));
            Expression expr = Expression.Property(parameter, info);
            if (info.PropertyType.IsNullableType())
                expr = Expression.Property(expr, "Value");
            value = value ?? 0;
            value = Convert.ChangeType(value, info.PropertyType.GetUnderlyingType());
            expr = Expression.Equal(expr, Expression.Constant(value));
            expr = Expression.Lambda(expr, parameter);
            cmbGrandChild.InternalConditionExpression = expr as Expression<Func<TGrandChild, bool>>;
        }

        public override  async void Update(bool isParent)
        {
            if (isParent)
            {
                base.SetInternalExpression(parentValue);
                var value = await cmbChild.UpdateCascadeComboBox();
                if (value == null)
                    cmbGrandChild.Clear();
                else
                {
                    childValue = value;
                    SetInternalExpression();
                    await cmbGrandChild.UpdateCascadeComboBox();
                }
            }
            else
            {
                SetInternalExpression();
                await cmbGrandChild.UpdateCascadeComboBox();
            }

        }
    }

    public interface IComboBox<TEntity> where TEntity:class 
    {
        EventCallback<object> OnInternalValueChanged { get; set; }

        Expression<Func<TEntity, bool>> InternalConditionExpression { get; set; }

        Task<int?> UpdateCascadeComboBox();

        void Clear();
    }
}
