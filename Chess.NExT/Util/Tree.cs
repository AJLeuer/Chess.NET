using System;
using System.Collections;
using System.Collections.Generic;

namespace Chess.Util
{
	public class TreeNode<T>
	{
		public T Datum { get; set; }
		public TreeNode<T> Parent { get; private set; } = null;
		public List<TreeNode<T>> Children { get; } = new List<TreeNode<T>>();
		
		public TreeNode(T datum)
		{
			this.Datum = datum;
		}
		
		public void AddChildren(params TreeNode<T>[] nodes)
		{
			this.Children.AddRange(nodes);
			this.Children.ForEach((TreeNode<T> node) => { node.Parent = this; });
		}
		
		public void AddChildren(params T[] data)
		{
			TreeNode<T>[] nodes = new TreeNode<T>[data.Length];
			
			for(uint i = 0; i < data.Length; i++)
			{
				TreeNode<T> node = new TreeNode<T>(data[i]);
				nodes[i] = node;
			}
			
			AddChildren(nodes);
		}
	}
}


