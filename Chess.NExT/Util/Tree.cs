using System;
using System.Collections;
using System.Collections.Generic;

namespace Chess.Utility
{
	public class TreeNode<T> : ICollection<TreeNode<T>>, IComparable<TreeNode<T>> where T : IComparable<T>
	{
		private static ulong IDs = 0;

		public ulong ID { get; } = IDs++;
		public T Datum { get; set; } = default;
		public TreeNode<T> Parent { get; private set; } = null;
		public SynchronizedCollection<TreeNode<T>> Children { get; } = new SynchronizedCollection<TreeNode<T>>();

		public TreeNode(T datum)
		{
			this.Datum = datum;
		}

		public TreeNode() : this(default)
		{
			
		}

		public void Add(TreeNode<T> node)
		{
			AddChildren(node);
		}
		
		public void Add(T item)
		{
			AddChildren(item);
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
		
		public static bool operator == (TreeNode<T> left, TreeNode<T> right)
		{
			if (ReferenceEquals(left, null))
			{
				return ReferenceEquals(right, null);
			}
			else if (ReferenceEquals(right, null))
			{
				return false;
			}
			else
			{
				return (dynamic) left.Datum == (dynamic) right.Datum;
			}
		}
		
		public static bool operator != (TreeNode<T> left, TreeNode<T> right)
		{
			return (left == right) == false;
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

		public IEnumerator<TreeNode<T>> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Clear()
		{
			foreach (var child in Children)
			{
				child.Clear();
			}
			
			Children.Clear();

			this.Datum = default;
		}

		public bool Contains(TreeNode<T> node)
		{
				if (node != null)
				{
					if ((dynamic) this.Datum == (dynamic) node.Datum) 
					{
						return true;
					}
				}
				foreach (var child in Children)
				{
					if (child == null)
					{
						if (node == null)
						{
							return true;
						}
					}
					else if (child.Contains(node))
					{
						return true;
					}
				}
	
				return false;
		}
		
		public bool Contains(T item)
		{
			return Contains(new TreeNode<T>(item));
		}

		public void CopyTo(TreeNode<T>[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public bool Remove(TreeNode<T> node)
		{
			bool removed = false;

			if (this == node)
			{
				Clear();
				removed = true;
			}
			else
			{
				foreach (var child in Children)
				{
					removed = child.Remove(node);
					if (removed) break;
				}
			}

			return removed;
		}

		public bool Remove(T item)
		{
			return Remove(new TreeNode<T>(item));
		}

		public int Count
		{
			get 
			{
				int count = 1;
				
				if (Children.Count == 0)
				{
					return count;
				}
				else
				{
					foreach (var child in Children)
					{
						count += child.Count;
					}

					return count;
				}
			}	
		}

		public bool IsReadOnly { get; } = false;

		#endregion
	}
}


