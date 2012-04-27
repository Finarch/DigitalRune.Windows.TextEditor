using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using DigitalRune.Windows.TextEditor.Highlighting;


namespace DigitalRune.Windows.TextEditor.Document
{
  /// <summary>
  /// Describes a line of a document.
  /// </summary>
  public sealed class LineSegment : ISegment
  {
    internal LineSegmentTree.Enumerator treeEntry;
    int totalLength, delimiterLength;

    List<TextWord> words;
    SpanStack highlightSpanStack;


    /// <summary>
    /// Gets the word at a certain column.
    /// </summary>
    /// <param name="column">The column.</param>
    /// <returns>The word at the specified column.</returns>
    public TextWord GetWord(int column)
    {
      int curColumn = 0;
      foreach (TextWord word in words)
      {
        if (column < curColumn + word.Length)
        {
          return word;
        }
        curColumn += word.Length;
      }
      return null;
    }


    /// <summary>
    /// Gets a value indicating whether this segment has been deleted.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is deleted; otherwise, <c>false</c>.
    /// </value>
    public bool IsDeleted
    {
      get { return !treeEntry.IsValid; }
    }


    /// <summary>
    /// Gets the line number.
    /// </summary>
    /// <value>The line number.</value>
    public int LineNumber
    {
      get { return treeEntry.CurrentIndex; }
    }


    /// <summary>
    /// Gets the offset.
    /// </summary>
    /// <value>The offset where the span begins</value>
    public int Offset
    {
      get { return treeEntry.CurrentOffset; }
    }


    /// <summary>
    /// Gets the length of the line (without newline delimiter).
    /// </summary>
    /// <value>The length of the line (without the newline delimiter).</value>
    public int Length
    {
      get { return totalLength - delimiterLength; }
    }


    /// <summary>
    /// Gets or sets the offset.
    /// </summary>
    /// <value>The offset where the span begins</value>
    int ISegment.Offset
    {
      get { return Offset; }
      set { throw new NotSupportedException(); }
    }


    /// <summary>
    /// Gets or sets the length.
    /// </summary>
    /// <value>The length of the span.</value>
    int ISegment.Length
    {
      get { return Length; }
      set { throw new NotSupportedException(); }
    }


    /// <summary>
    /// Gets (or sets) the length of the line (including newline delimiter).
    /// </summary>
    /// <value>The length of the line (include newline delimiter).</value>
    public int TotalLength
    {
      get { return totalLength; }
      internal set { totalLength = value; }
    }


    /// <summary>
    /// Gets (or sets) the length of the newline delimiter.
    /// </summary>
    /// <value>The length of the newline delimiter.</value>
    public int DelimiterLength
    {
      get { return delimiterLength; }
      internal set { delimiterLength = value; }
    }


    /// <summary>
    /// Gets or sets the words in the line.
    /// </summary>
    /// <value>The words in this line.</value>
    public List<TextWord> Words
    {
      get { return words; }
      set { words = value; }
    }


    /// <summary>
    /// Gets the highlight color for a position.
    /// </summary>
    /// <param name="x">The position (column).</param>
    /// <returns></returns>
    public HighlightColor GetColorForPosition(int x)
    {
      if (Words != null)
      {
        int xPos = 0;
        foreach (TextWord word in Words)
        {
          if (x < xPos + word.Length)
          {
            return word.SyntaxColor;
          }
          xPos += word.Length;
        }
      }
      return new HighlightColor(Color.Black, false, false);
    }


    /// <summary>
    /// Gets or sets the spans in the line with the same highlighting.
    /// </summary>
    /// <value>The spans with same highlighting.</value>
    public SpanStack HighlightSpanStack
    {
      get { return highlightSpanStack; }
      set { highlightSpanStack = value; }
    }


    /// <summary>
    /// Converts a <see cref="LineSegment"/> instance to string (for debug purposes)
    /// </summary>
    public override string ToString()
    {
      if (IsDeleted)
        return "[LineSegment: (deleted) Length = " + Length + ", TotalLength = " + TotalLength + ", DelimiterLength = " + delimiterLength + "]";
      else
        return "[LineSegment: LineNumber=" + LineNumber + ", Offset = " + Offset + ", Length = " + Length + ", TotalLength = " + TotalLength + ", DelimiterLength = " + delimiterLength + "]";
    }



    #region Anchor management
    Utilities.WeakCollection<TextAnchor> anchors;

    /// <summary>
    /// Creates a new anchor in the current line.
    /// </summary>
    /// <param name="column">The column.</param>
    /// <returns>The <see cref="TextAnchor"/>.</returns>
    public TextAnchor CreateAnchor(int column)
    {
      TextAnchor anchor = new TextAnchor(this, column);
      AddAnchor(anchor);
      return anchor;
    }

    void AddAnchor(TextAnchor anchor)
    {
      Debug.Assert(anchor.Line == this);

      if (anchors == null)
        anchors = new Utilities.WeakCollection<TextAnchor>();

      anchors.Add(anchor);
    }

    /// <summary>
    /// Is called when the LineSegment is deleted.
    /// </summary>
    internal void Deleted()
    {
      treeEntry = LineSegmentTree.Enumerator.Invalid;
      if (anchors != null)
      {
        foreach (TextAnchor a in anchors)
        {
          a.Deleted();
        }
        anchors = null;
      }
    }

    /// <summary>
    /// Is called when a part of the line is removed.
    /// </summary>
    internal void RemovedLinePart(int startColumn, int length)
    {
      if (length == 0)
        return;
      Debug.Assert(length > 0);

      if (anchors != null)
      {
        List<TextAnchor> deletedAnchors = null;
        foreach (TextAnchor a in anchors)
        {
          if (a.ColumnNumber > startColumn)
          {
            if (a.ColumnNumber >= startColumn + length)
            {
              a.ColumnNumber -= length;
            }
            else
            {
              if (deletedAnchors == null)
                deletedAnchors = new List<TextAnchor>();
              a.Deleted();
              deletedAnchors.Add(a);
            }
          }
        }
        if (deletedAnchors != null)
        {
          foreach (TextAnchor a in deletedAnchors)
          {
            anchors.Remove(a);
          }
        }
      }
    }

    /// <summary>
    /// Is called when a part of the line is inserted.
    /// </summary>
    internal void InsertedLinePart(int startColumn, int length)
    {
      if (length == 0)
        return;
      Debug.Assert(length > 0);

      if (anchors != null)
      {
        foreach (TextAnchor a in anchors)
        {
          if (a.ColumnNumber >= startColumn)
          {
            a.ColumnNumber += length;
          }
        }
      }
    }

    /// <summary>
    /// Is called after another line's content is appended to this line because the newline in between
    /// was deleted.
    /// The DefaultLineManager will call Deleted() on the deletedLine after the MergedWith call.
    /// 
    /// firstLineLength: the length of the line before the merge.
    /// </summary>
    internal void MergedWith(LineSegment deletedLine, int firstLineLength)
    {
      if (deletedLine.anchors != null)
      {
        foreach (TextAnchor a in deletedLine.anchors)
        {
          a.Line = this;
          AddAnchor(a);
          a.ColumnNumber += firstLineLength;
        }
        deletedLine.anchors = null;
      }
    }

    /// <summary>
    /// Is called after a newline was inserted into this line, splitting it into this and followingLine.
    /// </summary>
    internal void SplitTo(LineSegment followingLine)
    {
      if (anchors != null)
      {
        List<TextAnchor> movedAnchors = null;
        foreach (TextAnchor a in anchors)
        {
          if (a.ColumnNumber > this.Length)
          {
            a.Line = followingLine;
            followingLine.AddAnchor(a);
            a.ColumnNumber -= this.Length;

            if (movedAnchors == null)
              movedAnchors = new List<TextAnchor>();
            movedAnchors.Add(a);
          }
        }
        if (movedAnchors != null)
        {
          foreach (TextAnchor a in movedAnchors)
          {
            anchors.Remove(a);
          }
        }
      }
    }
    #endregion
  }
}
