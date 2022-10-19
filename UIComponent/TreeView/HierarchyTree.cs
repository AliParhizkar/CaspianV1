using System;
using System.Linq;
using System.Reflection;
using Caspian.Common.Extension;
using System.Collections.Generic;

namespace Caspian.UI
{
    public class HierarchyTree<TEntity>
    {
        public IList<TreeViewItem> CreateTree(IList<TEntity> entities, bool selectable)
        {
            var nodes = new List<TreeViewItem>();
            var info = typeof(TEntity).GetProperties().SingleOrDefault(t => t.PropertyType.IsCollectionType() && t.PropertyType.GenericTypeArguments.Any() && t.PropertyType.GenericTypeArguments[0] == typeof(TEntity));
            var key = typeof(TEntity).GetPrimaryKey();
            foreach (var entity in entities)
                nodes.Add(CreateNode(entity, info, key, selectable));
            return nodes;
        }

        public TreeViewItem FindNodeByValue(string value, IList<TreeViewItem> nodes)
        {
            foreach (var node in nodes)
            {
                var targetNode = FindNodeByValue(value, node);
                if (targetNode != null)
                    return targetNode;
            }
            return null;
        }

        public void UpdateSelectedState(IList<TreeViewItem> nodes, IList<object> selectedNodesvalue)
        {
            var items = selectedNodesvalue.Select(t => t.ToString()).ToList();
            foreach (var node in nodes)
            {
                UpdateSelectedState(node, items);
            }
        }

        void UpdateSelectedState(TreeViewItem node, IList<string> selectedNodesvalue)
        {
            node.Selected = selectedNodesvalue.Contains(node.Value);
            if (node.Items != null)
            {
                foreach(var item in node.Items)
                    UpdateSelectedState(item, selectedNodesvalue);
            }
        }

        TreeViewItem FindNodeByValue(string value, TreeViewItem node)
        {
            if (node.Value == value)
                return node;
            if (node.Items != null)
            {
                foreach(var item in node.Items)
                {
                    var targetNode = FindNodeByValue(value, item);
                    if (targetNode != null)
                        return targetNode;
                }
            }
            return null;
        }

        public Func<TEntity, bool> FilterFunc { get; set; }

        public IList<TreeViewItem> FilterTree(IList<TEntity> entities, bool selectable)
        {
            var list = new List<TreeViewItem>();
            var type = typeof(TEntity);
            var info = typeof(TEntity).GetProperties().SingleOrDefault(t => t.PropertyType.IsCollectionType() && t.PropertyType.GenericTypeArguments.Any() && t.PropertyType.GenericTypeArguments[0] == typeof(TEntity));
            var key = type.GetPrimaryKey();
            foreach (var entity in entities)
            {
                var node = CreateFilterNode(entity, info, key, selectable);
                if (node != null)
                    list.Add(node);
            }
            return list;
        }

        TreeViewItem CreateFilterNode(TEntity entity, PropertyInfo info, PropertyInfo key, bool selectable)
        {
            var items = info.GetValue(entity) as IEnumerable<TEntity>;
            if ((items == null || items.Count() == 0))
            {
                if (FilterFunc(entity))
                {
                    var node = new TreeViewItem();
                    node.Collabsable = true;
                    node.ShowTemplate = true;
                    node.Selectable = selectable;
                    node.Text = TextFunc.Invoke(entity);
                    node.Value = key.GetValue(entity).ToString();
                    return node;
                }
                return null;
            }
            var children = new List<TreeViewItem>();
            foreach (var item in items)
            {
                var child = CreateFilterNode(item, info, key, selectable);
                if (child != null)
                    children.Add(child);
            }
            if (children.Count > 0 || FilterFunc(entity))
            {
                var node = new TreeViewItem();
                node.Expanded = children.Count > 0;
                node.Collabsable = true;
                node.Text = TextFunc.Invoke(entity);
                node.Value = key.GetValue(entity).ToString();
                node.ShowTemplate = true;
                node.Selectable = selectable;
                node.Items = children;
                return node;
            }
            return null;
        }

        public Func<TEntity, string> TextFunc { get; set; }

        TreeViewItem CreateNode(TEntity entity, PropertyInfo info, PropertyInfo key, bool selectable)
        {
            var node = new TreeViewItem();
            node.Expanded = true;
            node.ShowTemplate = true;
            node.Selectable = selectable;
            node.Text = TextFunc.Invoke(entity);
            node.Value = key.GetValue(entity).ToString();
            var items = info.GetValue(entity) as IEnumerable<TEntity>;
            if (items != null)
            {
                node.Items = new List<TreeViewItem>();
                foreach (var item in items)
                    node.Items.Add(CreateNode(item, info, key, selectable));
            }
            return node;
        }
    }
}
