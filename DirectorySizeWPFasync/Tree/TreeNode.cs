using System.Collections;
using System.Collections.Generic;

namespace DirectorySizeWPFasync.Tree
{
    internal class TreeNode<T> : IEnumerable<TreeNode<T>>
    {
        public T Data { get; private set; }
        public List<TreeNode<T>> Children { get; private set; }

        public TreeNode(T data)
        {
            Data = data;
            Children = new List<TreeNode<T>>();
        }

        public TreeNode<T> AddChild(T data)
        {
            TreeNode<T> childNode = new TreeNode<T>(data);
            Children.Add(childNode);

            return childNode;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<TreeNode<T>> GetEnumerator()
        {
            yield return this;
            foreach (var directChild in Children)
            {
                foreach (var anyChild in directChild)
                    yield return anyChild;
            }
        }
    }
}