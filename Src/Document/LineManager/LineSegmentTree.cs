using System;
using System.Collections.Generic;
using System.Diagnostics;
using DigitalRune.Windows.TextEditor.Utilities;


namespace DigitalRune.Windows.TextEditor.Document
{
	/// <summary>
	/// Data structure for efficient management of the line segments (most operations are O(lg n)).
	/// </summary>
	/// <remarks>
	/// <para>
  /// This implements an augmented red-black tree where each node has fields for the number of
  /// nodes in its subtree (like an order statistics tree) for access by index(=line number).
  /// Additionally, each node knows the total length of all segments in its subtree.
  /// This means we can find nodes by offset in O(lg n) time. Since the offset itself is not stored in
  /// the line segment but computed from the lengths stored in the tree, we adjusting the offsets when
  /// text is inserted in one line means we just have to increment the totalLength of the affected line and
  /// its parent nodes - an O(lg n) operation.
  /// However this means getting the line number or offset from a LineSegment is not a constant time
  /// operation, but takes O(lg n).
  /// </para>
  /// <para>
  /// Note: The tree is never empty, <see cref="Clear"/> causes it to contain an empty segment.
  /// </para>
  /// </remarks>
	sealed class LineSegmentTree : IList<LineSegment>
	{
		internal struct RBNode
		{
			internal LineSegment lineSegment;
			internal int count;
			internal int totalLength;
			
			public RBNode(LineSegment lineSegment)
			{
				this.lineSegment = lineSegment;
				this.count = 1;
				this.totalLength = lineSegment.TotalLength;
			}
			
			public override string ToString()
			{
				return "[RBNode count=" + count + " totalLength="+totalLength
					+ " lineSegment.LineNumber=" + lineSegment.LineNumber
					+ " lineSegment.Offset=" + lineSegment.Offset
					+ " lineSegment.TotalLength=" + lineSegment.TotalLength
					+ " lineSegment.DelimiterLength=" + lineSegment.DelimiterLength + "]";
			}
		}
		
		struct MyHost : IRedBlackTreeHost<RBNode>
		{
			public int Compare(RBNode x, RBNode y)
			{
				throw new NotImplementedException();
			}
			
			public bool Equals(RBNode a, RBNode b)
			{
				throw new NotImplementedException();
			}
			
			public void UpdateAfterChildrenChange(RedBlackTreeNode<RBNode> node)
			{
				int count = 1;
				int totalLength = node.Value.lineSegment.TotalLength;
				if (node.Left != null) {
					count += node.Left.Value.count;
					totalLength += node.Left.Value.totalLength;
				}
				if (node.Right != null) {
					count += node.Right.Value.count;
					totalLength += node.Right.Value.totalLength;
				}
				if (count != node.Value.count || totalLength != node.Value.totalLength) {
					node.Value.count = count;
					node.Value.totalLength = totalLength;
					if (node.Parent != null) UpdateAfterChildrenChange(node.Parent);
				}
			}
			
			public void UpdateAfterRotateLeft(RedBlackTreeNode<RBNode> node)
			{
				UpdateAfterChildrenChange(node);
				UpdateAfterChildrenChange(node.Parent);
			}
			
			public void UpdateAfterRotateRight(RedBlackTreeNode<RBNode> node)
			{
				UpdateAfterChildrenChange(node);
				UpdateAfterChildrenChange(node.Parent);
			}
		}
		
		readonly AugmentableRedBlackTree<RBNode, MyHost> tree = new AugmentableRedBlackTree<RBNode, MyHost>(new MyHost());
		
		RedBlackTreeNode<RBNode> GetNode(int index)
		{
			if (index < 0 || index >= tree.Count)
				throw new ArgumentOutOfRangeException("index", index, "index should be between 0 and " + (tree.Count-1));
			RedBlackTreeNode<RBNode> node = tree.Root;
			while (true) {
				if (node.Left != null && index < node.Left.Value.count) {
					node = node.Left;
				} else {
					if (node.Left != null) {
						index -= node.Left.Value.count;
					}
					if (index == 0)
						return node;
					index--;
					node = node.Right;
				}
			}
		}
		
		static int GetIndexFromNode(RedBlackTreeNode<RBNode> node)
		{
			int index = (node.Left != null) ? node.Left.Value.count : 0;
			while (node.Parent != null) {
				if (node == node.Parent.Right) {
					if (node.Parent.Left != null)
						index += node.Parent.Left.Value.count;
					index++;
				}
				node = node.Parent;
			}
			return index;
		}
		
		RedBlackTreeNode<RBNode> GetNodeByOffset(int offset)
		{
			if (offset < 0 || offset > this.TotalLength)
				throw new ArgumentOutOfRangeException("offset", offset, "offset should be between 0 and " + this.TotalLength);
			if (offset == this.TotalLength) {
				if (tree.Root == null)
					throw new InvalidOperationException("Cannot call GetNodeByOffset while tree is empty.");
				return tree.Root.RightMost;
			}
			RedBlackTreeNode<RBNode> node = tree.Root;
			while (true) {
				if (node.Left != null && offset < node.Left.Value.totalLength) {
					node = node.Left;
				} else {
					if (node.Left != null) {
						offset -= node.Left.Value.totalLength;
					}
					offset -= node.Value.lineSegment.TotalLength;
					if (offset < 0)
						return node;
					node = node.Right;
				}
			}
		}
		
		static int GetOffsetFromNode(RedBlackTreeNode<RBNode> node)
		{
			int offset = (node.Left != null) ? node.Left.Value.totalLength : 0;
			while (node.Parent != null) {
				if (node == node.Parent.Right) {
					if (node.Parent.Left != null)
						offset += node.Parent.Left.Value.totalLength;
					offset += node.Parent.Value.lineSegment.TotalLength;
				}
				node = node.Parent;
			}
			return offset;
		}
		
		public LineSegment GetByOffset(int offset)
		{
			return GetNodeByOffset(offset).Value.lineSegment;
		}
		
		/// <summary>
		/// Gets the total length of all line segments. Runs in O(1).
		/// </summary>
		public int TotalLength {
			get {
				if (tree.Root == null)
					return 0;
				else
					return tree.Root.Value.totalLength;
			}
		}
		
		/// <summary>
		/// Updates the length of a line segment. Runs in O(lg n).
		/// </summary>
		public void SetSegmentLength(LineSegment segment, int newTotalLength)
		{
			if (segment == null)
				throw new ArgumentNullException("segment");
			RedBlackTreeNode<RBNode> node = segment.treeEntry.it.Node;
			segment.TotalLength = newTotalLength;
			default(MyHost).UpdateAfterChildrenChange(node);
			#if DEBUG
			CheckProperties();
			#endif
		}
		
		public void RemoveSegment(LineSegment segment)
		{
			tree.RemoveAt(segment.treeEntry.it);
			#if DEBUG
			CheckProperties();
			#endif
		}
		
		public LineSegment InsertSegmentAfter(LineSegment segment, int length)
		{
			LineSegment newSegment = new LineSegment();
			newSegment.TotalLength = length;
			newSegment.DelimiterLength = segment.DelimiterLength;
			
			newSegment.treeEntry = InsertAfter(segment.treeEntry.it.Node, newSegment);
			return newSegment;
		}
		
		Enumerator InsertAfter(RedBlackTreeNode<RBNode> node, LineSegment newSegment)
		{
			RedBlackTreeNode<RBNode> newNode = new RedBlackTreeNode<RBNode>(new RBNode(newSegment));
			if (node.Right == null) {
				tree.InsertAsRight(node, newNode);
			} else {
				tree.InsertAsLeft(node.Right.LeftMost, newNode);
			}
			#if DEBUG
			CheckProperties();
			#endif
			return new Enumerator(new RedBlackTreeIterator<RBNode>(newNode));
		}
		
		/// <summary>
		/// Gets the number of items in the collections. Runs in O(1).
		/// </summary>
		public int Count {
			get { return tree.Count; }
		}
		
		/// <summary>
		/// Gets or sets an item by index. Runs in O(lg n).
		/// </summary>
		public LineSegment this[int index] {
			get {
				return GetNode(index).Value.lineSegment;
			}
			set {
				throw new NotSupportedException();
			}
		}
		
		bool ICollection<LineSegment>.IsReadOnly {
			get { return true; }
		}
		
		/// <summary>
		/// Gets the index of an item. Runs in O(lg n).
		/// </summary>
		public int IndexOf(LineSegment item)
		{
			int index = item.LineNumber;
			if (index < 0 || index >= this.Count)
				return -1;
			if (item != this[index])
				return -1;
			return index;
		}
		
		void IList<LineSegment>.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}
		
		#if DEBUG
		[Conditional("DATACONSISTENCYTEST")]
		void CheckProperties()
		{
			if (tree.Root == null) {
				Debug.Assert(this.Count == 0);
			} else {
				Debug.Assert(tree.Root.Value.count == this.Count);
				CheckProperties(tree.Root);
			}
		}
		
		void CheckProperties(RedBlackTreeNode<RBNode> node)
		{
			int count = 1;
			int totalLength = node.Value.lineSegment.TotalLength;
			if (node.Left != null) {
				CheckProperties(node.Left);
				count += node.Left.Value.count;
				totalLength += node.Left.Value.totalLength;
			}
			if (node.Right != null) {
				CheckProperties(node.Right);
				count += node.Right.Value.count;
				totalLength += node.Right.Value.totalLength;
			}
			Debug.Assert(node.Value.count == count);
			Debug.Assert(node.Value.totalLength == totalLength);
		}
		
		public string GetTreeAsString()
		{
			return tree.GetTreeAsString();
		}
		#endif
		
		public LineSegmentTree()
		{
			Clear();
		}
		
		/// <summary>
		/// Clears the list. Runs in O(1).
		/// </summary>
		public void Clear()
		{
			tree.Clear();
			LineSegment emptySegment = new LineSegment();
			emptySegment.TotalLength = 0;
			emptySegment.DelimiterLength = 0;
			tree.Add(new RBNode(emptySegment));
			emptySegment.treeEntry = GetEnumeratorForIndex(0);
			#if DEBUG
			CheckProperties();
			#endif
		}
		
		/// <summary>
		/// Tests whether an item is in the list. Runs in O(n).
		/// </summary>
		public bool Contains(LineSegment item)
		{
			return IndexOf(item) >= 0;
		}
		
		/// <summary>
		/// Copies all elements from the list to the array.
		/// </summary>
		public void CopyTo(LineSegment[] array, int arrayIndex)
		{
			if (array == null) throw new ArgumentNullException("array");
			foreach (LineSegment val in this)
				array[arrayIndex++] = val;
		}
		
		IEnumerator<LineSegment> IEnumerable<LineSegment>.GetEnumerator()
		{
			return this.GetEnumerator();
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
		
		public Enumerator GetEnumerator()
		{
			return new Enumerator(tree.GetEnumerator());
		}
		
		public Enumerator GetEnumeratorForIndex(int index)
		{
			return new Enumerator(new RedBlackTreeIterator<RBNode>(GetNode(index)));
		}
		
		public Enumerator GetEnumeratorForOffset(int offset)
		{
			return new Enumerator(new RedBlackTreeIterator<RBNode>(GetNodeByOffset(offset)));
		}
		
		public struct Enumerator : IEnumerator<LineSegment>
		{
			/// <summary>
			/// An invalid enumerator value. Calling MoveNext on the invalid enumerator
			/// will always return false, accessing Current will throw an exception.
			/// </summary>
			public static readonly Enumerator Invalid = default(Enumerator);
			
			internal RedBlackTreeIterator<RBNode> it;
			
			internal Enumerator(RedBlackTreeIterator<RBNode> it)
			{
				this.it = it;
			}
			
			/// <summary>
			/// Gets the current value. Runs in O(1).
			/// </summary>
			public LineSegment Current {
				get {
					return it.Current.lineSegment;
				}
			}
			
			public bool IsValid {
				get {
					return it.IsValid;
				}
			}
			
			/// <summary>
			/// Gets the index of the current value. Runs in O(lg n).
			/// </summary>
			public int CurrentIndex {
				get {
					if (it.Node == null)
						throw new InvalidOperationException();
					return GetIndexFromNode(it.Node);
				}
			}
			
			/// <summary>
			/// Gets the offset of the current value. Runs in O(lg n).
			/// </summary>
			public int CurrentOffset {
				get {
					if (it.Node == null)
						throw new InvalidOperationException();
					return GetOffsetFromNode(it.Node);
				}
			}
			
			object System.Collections.IEnumerator.Current {
				get {
					return it.Current.lineSegment;
				}
			}
			
			public void Dispose()
			{
			}
			
			/// <summary>
			/// Moves to the next index. Runs in O(lg n), but for k calls, the combined time is only O(k+lg n).
			/// </summary>
			public bool MoveNext()
			{
				return it.MoveNext();
			}
			
			/// <summary>
			/// Moves to the previous index. Runs in O(lg n), but for k calls, the combined time is only O(k+lg n).
			/// </summary>
			public bool MoveBack()
			{
				return it.MoveBack();
			}
			
			void System.Collections.IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}
		}
		
		void IList<LineSegment>.Insert(int index, LineSegment item)
		{
			throw new NotSupportedException();
		}
		
		void ICollection<LineSegment>.Add(LineSegment item)
		{
			throw new NotSupportedException();
		}
		
		bool ICollection<LineSegment>.Remove(LineSegment item)
		{
			throw new NotSupportedException();
		}
	}
}
