using System;
using System.Linq;
using System.Reflection;
using Caspian.Common.Extension;
using System.Collections.Generic;

namespace Caspian.UI
{
    public class HierarchyTree<TEntity>
    {
        public IList<NodeView> CreateTree(IList<TEntity> entities, bool selectable, Func<TEntity, bool> selectableFunc = null)
        {
            var nodes = new List<NodeView>();
            var info = typeof(TEntity).GetProperties().SingleOrDefault(t => t.PropertyType.IsCollectionType() && t.PropertyType.GenericTypeArguments.Any() && t.PropertyType.GenericTypeArguments[0] == typeof(TEntity));
            var key = typeof(TEntity).GetPrimaryKey();
            foreach (var entity in entities)
                nodes.Add(CreateNode(entity, info, key, selectable, selectableFunc));
            return nodes;
        }

        public NodeView FindNodeByValue(string value, IList<NodeView> nodes)
        {
            foreach (var node in nodes)
            {
                var targetNode = FindNodeByValue(value, node);
                if (targetNode != null)
                    return targetNode;
            }
            return null;
        }

        public void UpdateSelectedState(IList<NodeView> nodes, IList<string> selectedNodesvalue)
        {
            foreach (var node in nodes)
            {
                UpdateSelectedState(node, selectedNodesvalue);
            }
        }

        void UpdateSelectedState(NodeView node, IList<string> selectedNodesvalue)
        {
            node.Selected = selectedNodesvalue.Contains(node.Value);
            if (node.Children != null)
            {
                foreach(var child in node.Children)
                    UpdateSelectedState(child, selectedNodesvalue);
            }
        }

        NodeView FindNodeByValue(string value, NodeView node)
        {
            if (node.Value == value)
                return node;
            if (node.Children != null)
            {
                foreach(var child in node.Children)
                {
                    var targetNode = FindNodeByValue(value, child);
                    if (targetNode != null)
                        return targetNode;
                }
            }
            return null;
        }

        public Func<TEntity, bool> FilterFunc { get; set; }

        public IList<NodeView> FilterTree(IList<TEntity> entities, bool selectable, Func<TEntity, bool> selectableFunc)
        {
            var list = new List<NodeView>();
            var type = typeof(TEntity);
            var info = typeof(TEntity).GetProperties().SingleOrDefault(t => t.PropertyType.IsCollectionType() && t.PropertyType.GenericTypeArguments.Any() && t.PropertyType.GenericTypeArguments[0] == typeof(TEntity));
            var key = type.GetPrimaryKey();
            foreach (var entity in entities)
            {
                var node = CreateFilterNode(entity, info, key, selectable, selectableFunc);
                if (node != null)
                    list.Add(node);
            }
            return list;
        }

        NodeView CreateFilterNode(TEntity entity, PropertyInfo info, PropertyInfo key, bool selectable, Func<TEntity, bool> selectableFunc)
        {
            var items = info.GetValue(entity) as IEnumerable<TEntity>;
            if ((items == null || items.Count() == 0))
            {
                if (FilterFunc(entity))
                {
                    var node = new NodeView(key.GetValue(entity).ToString(), TextFunc.Invoke(entity));
                    node.Collabsable = false;
                    if (selectableFunc == null)
                        node.Selectable = selectable;
                    else
                        node.Selectable = selectableFunc(entity);
                    return node;
                }
                return null;
            }
            var children = new List<NodeView>();
            foreach (var item in items)
            {
                var child = CreateFilterNode(item, info, key, selectable, selectableFunc);
                if (child != null)
                    children.Add(child);
            }
            if (children.Count > 0 || FilterFunc(entity))
            {
                var node = new NodeView();
                node.Expanded = children.Count > 0;
                node.Collabsable = true;
                node.Text = TextFunc.Invoke(entity);
                node.Value = key.GetValue(entity).ToString();
                if (selectableFunc == null)
                    node.Selectable = selectable;
                else
                    node.Selectable = selectableFunc(entity);
                node.Children = children;
                return node;
            }
            return null;
        }

        public Func<TEntity, string> TextFunc { get; set; }

        NodeView CreateNode(TEntity entity, PropertyInfo info, PropertyInfo key, bool selectable, Func<TEntity, bool> selectableFunc = null)
        {
            var node = new NodeView();
            node.Expanded = true;
            if (selectableFunc != null)
                node.Selectable = selectableFunc.Invoke(entity);
            else
                node.Selectable = selectable;
            node.Collabsable = node.Children != null && node.Children.Any();
            node.Text = TextFunc.Invoke(entity);
            node.Value = key.GetValue(entity).ToString();
            var items = info.GetValue(entity) as IEnumerable<TEntity>;
            if (items != null)
            {
                node.Children = new List<NodeView>();
                foreach (var child in items)
                    node.Children.Add(CreateNode(child, info, key, selectable, selectableFunc));
            }
            return node;
        }
    }
}
