using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Remoting.Messaging;

namespace Chess.Util
{
	public class TreeNode<T> : IComparable<TreeNode<T>> where T : IComparable<T>
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

		#region InterfaceImplementation
		
		public int CompareTo(TreeNode<T> other)
		{
			return this.Datum.CompareTo(other.Datum);
		}

		public static bool operator < (TreeNode<T> left, TreeNode<T> right)
		{
			return left.CompareTo(right) < 0;
		}

		public static bool operator > (TreeNode<T> left, TreeNode<T> right)
		{
			return left.CompareTo(right) > 0;
		}

		public static bool operator <= (TreeNode<T> left, TreeNode<T> right)
		{
			return left.CompareTo(right) <= 0;
		}

		public static bool operator >= (TreeNode<T> left, TreeNode<T> right)
		{
			return left.CompareTo(right) >= 0;
		}
		
		#endregion
	}
}


