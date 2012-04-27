using System;
using System.Collections.Generic;

namespace DigitalRune.Windows.TextEditor.Highlighting
{
  /// <summary>
  /// A stack of Span instances. 
  /// </summary>
  /// <remarks>
  /// Works like Stack&lt;Span&gt;, but can be cloned quickly because it is 
  /// implemented as linked list.
  /// </remarks>
  public sealed class SpanStack : ICloneable, IEnumerable<Span>
  {
    internal sealed class StackNode
    {
      public readonly StackNode Previous;
      public readonly Span Data;

      public StackNode(StackNode previous, Span data)
      {
        this.Previous = previous;
        this.Data = data;
      }
    }


    StackNode top = null;


    /// <summary>
    /// Removes a span from the top of the stack.
    /// </summary>
    /// <returns>The span on top of the stack.</returns>
    public Span Pop()
    {
      Span s = top.Data;
      top = top.Previous;
      return s;
    }


    /// <summary>
    /// Returns the span on top of the stack (but does not remove it from the stack).
    /// </summary>
    /// <returns>The span on top of the stack.</returns>
    public Span Peek()
    {
      return top.Data;
    }


    /// <summary>
    /// Puts a span on top of the stack.
    /// </summary>
    /// <param name="s">The span.</param>
    public void Push(Span s)
    {
      top = new StackNode(top, s);
    }


    /// <summary>
    /// Gets a value indicating whether stack is empty.
    /// </summary>
    /// <value><c>true</c> if the stack is empty; otherwise, <c>false</c>.</value>
    public bool IsEmpty
    {
      get { return top == null; }
    }


    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns>A clone of this stack.</returns>
    public SpanStack Clone()
    {
      SpanStack n = new SpanStack();
      n.top = this.top;
      return n;
    }


    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>
    /// A new object that is a copy of this instance.
    /// </returns>
    object ICloneable.Clone()
    {
      return this.Clone();
    }


    /// <summary>
    /// Gets the enumerator for this stack.
    /// </summary>
    /// <returns>The enumerator.</returns>
    public Enumerator GetEnumerator()
    {
      return new Enumerator(new StackNode(top, null));
    }


    IEnumerator<Span> IEnumerable<Span>.GetEnumerator()
    {
      return this.GetEnumerator();
    }


    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }


    /// <summary>
    /// Enumerates a <see cref="SpanStack"/> from top to bottom of the stack.
    /// </summary>
    public struct Enumerator : IEnumerator<Span>
    {
      StackNode c;

      internal Enumerator(StackNode node)
      {
        c = node;
      }


      /// <summary>
      /// Gets the current span.
      /// </summary>
      /// <value>The current.</value>
      public Span Current
      {
        get { return c.Data; }
      }


      object System.Collections.IEnumerator.Current
      {
        get { return c.Data; }
      }


      /// <summary>
      /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
      /// </summary>
      public void Dispose()
      {
        c = null;
      }


      /// <summary>
      /// Advances the enumerator to the next element of the stack (top-down).
      /// </summary>
      /// <returns>
      /// <c>true</c> if the enumerator was successfully advanced to the next element; <c>false</c> if the enumerator has passed the end of the collection.
      /// </returns>
      /// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created.</exception>
      public bool MoveNext()
      {
        c = c.Previous;
        return c != null;
      }


      /// <summary>
      /// Sets the enumerator to its initial position, which is before the first element in the collection.
      /// </summary>
      /// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created. </exception>
      public void Reset()
      {
        throw new NotSupportedException();
      }
    }
  }
}
