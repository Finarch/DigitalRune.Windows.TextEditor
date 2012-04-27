using System;
using System.Collections.Generic;
using System.Diagnostics;

using DigitalRune.Windows.TextEditor.Undo;
using DigitalRune.Windows.TextEditor.Bookmarks;
using DigitalRune.Windows.TextEditor.CustomLines;
using DigitalRune.Windows.TextEditor.Folding;
using DigitalRune.Windows.TextEditor.Formatting;
using DigitalRune.Windows.TextEditor.Highlighting;
using DigitalRune.Windows.TextEditor.Markers;
using DigitalRune.Windows.TextEditor.TextBuffer;
using DigitalRune.Windows.TextEditor.Properties;


namespace DigitalRune.Windows.TextEditor.Document
{
  /// <summary>
  /// The default <see cref="IDocument"/> implementation.
  /// </summary>
  internal class DefaultDocument : IDocument
  {
    bool readOnly = false;

    /// <summary>
    /// 
    /// </summary>
    LineManager lineTrackingStrategy;
    ICustomLineManager customLineManager;
    BookmarkManager bookmarkManager;
    ITextBufferStrategy textBufferStrategy;
    IFormattingStrategy formattingStrategy;
    FoldingManager foldingManager;
    UndoStack undoStack = new UndoStack();
    ITextEditorProperties textEditorProperties = new DefaultTextEditorProperties();
    MarkerStrategy markerStrategy;


    /// <summary>
    /// Gets or sets the line manager.
    /// </summary>
    /// <value>The line manager.</value>
    public LineManager LineManager
    {
      get { return lineTrackingStrategy; }
      set { lineTrackingStrategy = value; }
    }


    public event EventHandler<LineLengthChangeEventArgs> LineLengthChanged
    {
      add { lineTrackingStrategy.LineLengthChanged += value; }
      remove { lineTrackingStrategy.LineLengthChanged -= value; }
    }


    public event EventHandler<LineCountChangeEventArgs> LineCountChanged
    {
      add { lineTrackingStrategy.LineCountChanged += value; }
      remove { lineTrackingStrategy.LineCountChanged -= value; }
    }


    public event EventHandler<LineEventArgs> LineDeleted
    {
      add { lineTrackingStrategy.LineDeleted += value; }
      remove { lineTrackingStrategy.LineDeleted -= value; }
    }


    /// <summary>
    /// Gets the marker strategy.
    /// </summary>
    /// <value>The marker strategy.</value>
    public MarkerStrategy MarkerStrategy
    {
      get { return markerStrategy; }
      set { markerStrategy = value; }
    }


    /// <summary>
    /// Gets or sets the text editor properties.
    /// </summary>
    /// <value>The text editor properties.</value>
    public ITextEditorProperties TextEditorProperties
    {
      get { return textEditorProperties; }
      set { textEditorProperties = value; }
    }


    /// <summary>
    /// Gets the undo stack.
    /// </summary>
    /// <value>The undo stack.</value>
    public UndoStack UndoStack
    {
      get { return undoStack; }
    }


    /// <summary>
    /// Gets the line segment collection.
    /// </summary>
    /// <value>A collection of all line segments</value>
    /// <remarks>
    /// The collection should only be used if you're aware
    /// of the 'last line ends with a delimiter problem'. Otherwise
    /// the <see cref="GetLineSegment"/> method should be used.
    /// </remarks>
    public IList<LineSegment> LineSegmentCollection
    {
      get { return lineTrackingStrategy.LineSegmentCollection; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether the document is read-only.
    /// </summary>
    /// <value>If <c>true</c> the document can't be altered</value>
    public bool ReadOnly
    {
      get { return readOnly; }
      set { readOnly = value; }
    }




    /// <summary>
    /// The <see cref="ITextBufferStrategy"/> attached to the <see cref="IDocument"/> instance
    /// </summary>
    /// <value>The text buffer strategy.</value>
    public ITextBufferStrategy TextBufferStrategy
    {
      get { return textBufferStrategy; }
      set { textBufferStrategy = value; }
    }


    /// <summary>
    /// The <see cref="IFormattingStrategy"/> attached to the <see cref="IDocument"/> instance
    /// </summary>
    /// <value>The formatting strategy.</value>
    public IFormattingStrategy FormattingStrategy
    {
      get { return formattingStrategy; }
      set { formattingStrategy = value; }
    }


    /// <summary>
    /// The <see cref="FoldingManager"/> attached to the <see cref="IDocument"/> instance
    /// </summary>
    /// <value>The folding manager.</value>
    public FoldingManager FoldingManager
    {
      get { return foldingManager; }
      set { foldingManager = value; }
    }


    /// <summary>
    /// The <see cref="IHighlightingStrategy"/> attached to the <see cref="IDocument"/> instance
    /// </summary>
    /// <value>The highlighting strategy.</value>
    public IHighlightingStrategy HighlightingStrategy
    {
      get { return lineTrackingStrategy.HighlightingStrategy; }
      set { lineTrackingStrategy.HighlightingStrategy = value; }
    }


    /// <summary>
    /// Gets the length of the text.
    /// </summary>
    /// <value>
    /// The current length of the sequence of characters that can be edited.
    /// </value>
    public int TextLength
    {
      get { return textBufferStrategy.Length; }
    }


    /// <summary>
    /// The <see cref="BookmarkManager"/> attached to the <see cref="IDocument"/> instance
    /// </summary>
    /// <value>The bookmark manager.</value>
    public BookmarkManager BookmarkManager
    {
      get { return bookmarkManager; }
      set { bookmarkManager = value; }
    }


    /// <summary>
    /// The <see cref="ICustomLineManager"/> attached to the <see cref="IDocument"/> instance
    /// </summary>
    /// <value>The custom line manager.</value>
    public ICustomLineManager CustomLineManager
    {
      get { return customLineManager; }
      set { customLineManager = value; }
    }


    /// <summary>
    /// Gets or sets the whole text as string.
    /// </summary>
    /// <value>The whole text as string.</value>
    /// <remarks>
    /// When setting the text using the TextContent property, the undo stack is cleared.
    /// Set TextContent only for actions such as loading a file; if you want to change the current document
    /// use the Replace method instead.
    /// </remarks>
    public string TextContent
    {
      get { return GetText(0, textBufferStrategy.Length); }
      set
      {
        Debug.Assert(textBufferStrategy != null);
        Debug.Assert(lineTrackingStrategy != null);
        OnDocumentAboutToBeChanged(new DocumentEventArgs(this, 0, 0, value));
        textBufferStrategy.SetContent(value);
        lineTrackingStrategy.SetContent(value);
        undoStack.ClearAll();

        OnDocumentChanged(new DocumentEventArgs(this, 0, 0, value));
        OnTextContentChanged(EventArgs.Empty);
      }
    }


    /// <summary>
    /// Inserts a string of characters into the sequence.
    /// </summary>
    /// <param name="offset">Offset where to insert the string.</param>
    /// <param name="text">Text to be inserted.</param>
    public void Insert(int offset, string text)
    {
      if (readOnly)
        return;

      OnDocumentAboutToBeChanged(new DocumentEventArgs(this, offset, -1, text));
      textBufferStrategy.Insert(offset, text);
      lineTrackingStrategy.Insert(offset, text);
      undoStack.Push(new UndoableInsert(this, offset, text));
      OnDocumentChanged(new DocumentEventArgs(this, offset, -1, text));
    }


    /// <summary>
    /// Removes some portion of the sequence.
    /// </summary>
    /// <param name="offset">Offset of the remove.</param>
    /// <param name="length">Number of characters to remove.</param>
    public void Remove(int offset, int length)
    {
      if (readOnly)
        return;

      OnDocumentAboutToBeChanged(new DocumentEventArgs(this, offset, length));
      undoStack.Push(new UndoableDelete(this, offset, GetText(offset, length)));

      textBufferStrategy.Remove(offset, length);
      lineTrackingStrategy.Remove(offset, length);

      OnDocumentChanged(new DocumentEventArgs(this, offset, length));
    }


    /// <summary>
    /// Replace some portion of the sequence.
    /// </summary>
    /// <param name="offset">The offset.</param>
    /// <param name="length">The number of characters to replace.</param>
    /// <param name="text">The text to be replaced with.</param>
    public void Replace(int offset, int length, string text)
    {
      if (readOnly)
        return;

      OnDocumentAboutToBeChanged(new DocumentEventArgs(this, offset, length, text));
      undoStack.Push(new UndoableReplace(this, offset, GetText(offset, length), text));

      textBufferStrategy.Replace(offset, length, text);
      lineTrackingStrategy.Replace(offset, length, text);

      OnDocumentChanged(new DocumentEventArgs(this, offset, length, text));
    }


    /// <summary>
    /// Returns a specific char of the sequence.
    /// </summary>
    /// <param name="offset">Offset of the char to get.</param>
    /// <returns>The character.</returns>
    public char GetCharAt(int offset)
    {
      return textBufferStrategy.GetCharAt(offset);
    }


    /// <summary>
    /// Fetches a string of characters contained in the sequence.
    /// </summary>
    /// <param name="offset">Offset into the sequence to fetch</param>
    /// <param name="length">The number of characters to copy.</param>
    /// <returns>
    /// The text at the <paramref name="offset"/>.
    /// </returns>
    public string GetText(int offset, int length)
    {
#if DEBUG
      if (length < 0) throw new ArgumentOutOfRangeException("length", length, "length < 0");
#endif
      return textBufferStrategy.GetText(offset, length);
    }


    /// <summary>
    /// Gets the text of a certain segment.
    /// </summary>
    /// <param name="segment">The segment.</param>
    /// <returns>The text in the segment.</returns>
    public string GetText(ISegment segment)
    {
      return GetText(segment.Offset, segment.Length);
    }


    /// <summary>
    /// Gets the total number of lines.
    /// </summary>
    /// <value>
    /// The total number of lines, this may be != <c>LineSegmentCollection.Count</c>
    /// if the last line ends with a delimiter.
    /// </value>
    public int TotalNumberOfLines
    {
      get { return lineTrackingStrategy.TotalNumberOfLines; }
    }


    /// <summary>
    /// Gets the line number for the given offset.
    /// </summary>
    /// <param name="offset">A offset which points to a character in the line which line number is
    /// returned.</param>
    /// <returns>
    /// An <c>int</c> which value is the line number.
    /// </returns>
    /// <remarks>
    /// Returns a valid line number for the given offset.
    /// </remarks>
    /// <exception cref="System.ArgumentException">If offset points not to a valid position</exception>
    public int GetLineNumberForOffset(int offset)
    {
      return lineTrackingStrategy.GetLineNumberForOffset(offset);
    }


    /// <summary>
    /// Gets the line segment for a given offset.
    /// </summary>
    /// <param name="offset">A offset which points to a character in the line which
    /// is returned.</param>
    /// <returns>A <see cref="LineSegment"/> object.</returns>
    /// <remarks>
    /// Returns a <see cref="LineSegment"/> for the given offset.
    /// </remarks>
    /// <exception cref="System.ArgumentException">If offset points not to a valid position</exception>
    public LineSegment GetLineSegmentForOffset(int offset)
    {
      return lineTrackingStrategy.GetLineSegmentForOffset(offset);
    }


    /// <summary>
    /// Gets the line segment.
    /// </summary>
    /// <param name="lineNumber">The line number which is requested.</param>
    /// <returns>A <see cref="LineSegment"/> object.</returns>
    /// <remarks>
    /// Returns a <see cref="LineSegment"/> for the given line number.
    /// This function should be used to get a line instead of getting the
    /// line using the <see cref="LineSegmentCollection"/>.
    /// </remarks>
    /// <exception cref="System.ArgumentException">If offset points not to a valid position</exception>
    public LineSegment GetLineSegment(int lineNumber)
    {
      return lineTrackingStrategy.GetLineSegment(lineNumber);
    }


    /// <summary>
    /// Gets the first logical line for a given visible line.
    /// </summary>
    /// <param name="visibleLineNumber">The line number (visible lines).</param>
    /// <returns>The logical line number.</returns>
    /// <remarks>
    /// <para>
    /// Example: <c>lineNumber == 100</c>, foldings are in the <see cref="LineManager"/>
    /// between 0..1 (2 folded, invisible lines). This method returns 102 as
    /// the 'logical' line number.
    /// </para>
    /// <para>
    /// A visible line can contain several logical lines when it contains a (folded)
    /// folding. This method returns the <b>first</b> logical line that belongs to the 
    /// visible line.
    /// </para>
    /// </remarks>
    public int GetFirstLogicalLine(int visibleLineNumber)
    {
      return lineTrackingStrategy.GetFirstLogicalLine(visibleLineNumber);
    }


    /// <summary>
    /// Get the last logical line for a given visible line.
    /// </summary>
    /// <param name="visibleLineNumber">The line number (visible lines).</param>
    /// <returns>The logical line number.</returns>
    /// <remarks>
    /// <para>
    /// A visible line can contain several logical lines when it contains a (folded)
    /// folding. This method returns the <b>last</b> logical line that belongs to the 
    /// visible line.
    /// </para>
    /// </remarks>
    public int GetLastLogicalLine(int visibleLineNumber)
    {
      return lineTrackingStrategy.GetLastLogicalLine(visibleLineNumber);
    }


    /// <summary>
    /// Get the visible line for a given logical line.
    /// </summary>
    /// <param name="logicalLineNumber">The logical line number.</param>
    /// <returns>The 'visible' line number.</returns>
    /// <remarks>
    /// Example : <c>lineNumber == 100</c>, foldings are in the <see cref="LineManager"/>
    /// between 0..1 (2 folded, invisible lines). This method returns 98 as
    /// the 'visible' line number.
    /// </remarks>
    public int GetVisibleLine(int logicalLineNumber)
    {
      return lineTrackingStrategy.GetVisibleLine(logicalLineNumber);
    }


    /// <summary>
    /// Skips a certain number of visible lines forwards and returns the line 
    /// number of the next visible line.
    /// </summary>
    /// <param name="logicalLineNumber">The current logical line number.</param>
    /// <param name="visibleLineCount">The number of visible lines to skip.</param>
    /// <returns>The logical line number of the the next visible line.</returns>
    public int GetNextVisibleLineAfter(int logicalLineNumber, int visibleLineCount)
    {
      return lineTrackingStrategy.GetNextVisibleLineAfter(logicalLineNumber, visibleLineCount);
    }


    /// <summary>
    /// Skips a certain number of visible lines backwards and returns the line
    /// number of the visible line before.
    /// </summary>
    /// <param name="logicalLineNumber">The current logical line number.</param>
    /// <param name="visibleLineCount">The number of visible lines to skip.</param>
    /// <returns>The next visible line before the skipped block of lines.</returns>
    public int GetNextVisibleLineBefore(int logicalLineNumber, int visibleLineCount)
    {
      return lineTrackingStrategy.GetNextVisibleLineBefore(logicalLineNumber, visibleLineCount);
    }


    /// <summary>
    /// Returns the logical line/column position from an offset
    /// </summary>
    /// <param name="offset">The offset.</param>
    /// <returns>The position.</returns>
    public TextLocation OffsetToPosition(int offset)
    {
      int lineNr = GetLineNumberForOffset(offset);
      LineSegment line = GetLineSegment(lineNr);
      return new TextLocation(offset - line.Offset, lineNr);
    }


    /// <summary>
    /// Returns the offset from a logical line/column position
    /// </summary>
    /// <param name="p">The position.</param>
    /// <returns>The offset.</returns>
    public int PositionToOffset(TextLocation p)
    {
      if (p.Y >= this.TotalNumberOfLines)
      {
        return 0;
      }
      LineSegment line = GetLineSegment(p.Y);
      return Math.Min(this.TextLength, line.Offset + Math.Min(line.Length, p.X));
    }


    /// <summary>
    /// Moves, Resizes, Removes a list of segments on insert/remove/replace events.
    /// </summary>
    /// <typeparam name="T">A type of segment.</typeparam>
    /// <param name="list">The list.</param>
    /// <param name="e">The <see cref="DigitalRune.Windows.TextEditor.Document.DocumentEventArgs"/> instance containing the event data.</param>
    public void UpdateSegmentListOnDocumentChange<T>(List<T> list, DocumentEventArgs e) where T : ISegment
    {
      for (int i = 0; i < list.Count; ++i)
      {
        ISegment fm = list[i];

        if (e.Offset <= fm.Offset && fm.Offset <= e.Offset + e.Length ||
            e.Offset <= fm.Offset + fm.Length && fm.Offset + fm.Length <= e.Offset + e.Length)
        {
          list.RemoveAt(i);
          --i;
          continue;
        }

        if (fm.Offset <= e.Offset && e.Offset <= fm.Offset + fm.Length)
        {
          if (e.Text != null)
          {
            fm.Length += e.Text.Length;
          }
          if (e.Length > 0)
          {
            fm.Length -= e.Length;
          }
          continue;
        }

        if (fm.Offset >= e.Offset)
        {
          if (e.Text != null)
          {
            fm.Offset += e.Text.Length;
          }
          if (e.Length > 0)
          {
            fm.Offset -= e.Length;
          }
        }
      }
    }


    /// <summary>
    /// Raises the <see cref="DocumentAboutToBeChanged"/> event.
    /// </summary>
    /// <param name="e">The <see cref="DigitalRune.Windows.TextEditor.Document.DocumentEventArgs"/> instance containing the event data.</param>
    protected void OnDocumentAboutToBeChanged(DocumentEventArgs e)
    {
      if (DocumentAboutToBeChanged != null)
      {
        DocumentAboutToBeChanged(this, e);
      }
    }


    /// <summary>
    /// Raises the <see cref="DocumentChanged"/> event.
    /// </summary>
    /// <param name="e">The <see cref="DigitalRune.Windows.TextEditor.Document.DocumentEventArgs"/> instance containing the event data.</param>
    protected void OnDocumentChanged(DocumentEventArgs e)
    {
      if (DocumentChanged != null)
      {
        DocumentChanged(this, e);
      }
    }


    /// <summary>
    /// Occurs when a document is about to be changed.
    /// </summary>
    public event EventHandler<DocumentEventArgs> DocumentAboutToBeChanged;


    /// <summary>
    /// Occurs when a document has been changed.
    /// </summary>
    public event EventHandler<DocumentEventArgs> DocumentChanged;


    // UPDATE STUFF
    List<TextAreaUpdate> updateQueue = new List<TextAreaUpdate>();


    /// <summary>
    /// Gets the update queue.
    /// </summary>
    /// <value>A container where all TextAreaUpdate objects get stored</value>
    public List<TextAreaUpdate> UpdateQueue
    {
      get { return updateQueue; }
    }


    /// <summary>
    /// Requests an update.
    /// </summary>
    /// <param name="update">The update.</param>
    /// <remarks>
    /// Requests an update of the text area.
    /// </remarks>
    public void RequestUpdate(TextAreaUpdate update)
    {
      if (updateQueue.Count == 1 && updateQueue[0].TextAreaUpdateType == TextAreaUpdateType.WholeTextArea)
      {
        // if we're going to update the whole text area, we don't need to store detail updates
        return;
      }
      if (update.TextAreaUpdateType == TextAreaUpdateType.WholeTextArea)
      {
        // if we're going to update the whole text area, we don't need to store detail updates
        updateQueue.Clear();
      }
      updateQueue.Add(update);
    }


    /// <summary>
    /// Commits the update.
    /// </summary>
    /// <remarks>
    /// Commits all updates in the queue to the text area (the
    /// text area will be painted)
    /// </remarks>
    public void CommitUpdate()
    {
      if (UpdateCommited != null)
        UpdateCommited(this, EventArgs.Empty);
    }


    /// <summary>
    /// Raises the <see cref="TextContentChanged"/> event.
    /// </summary>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected virtual void OnTextContentChanged(EventArgs e)
    {
      if (TextContentChanged != null)
        TextContentChanged(this, e);
    }


    /// <summary>
    /// Is fired when CommitUpdate is called
    /// </summary>
    public event EventHandler UpdateCommited;


    /// <summary>
    /// Occurs when the text content has changed.
    /// </summary>
    public event EventHandler TextContentChanged;


    [Conditional("DEBUG")]
    internal static void ValidatePosition(IDocument document, TextLocation position)
    {
      document.GetLineSegment(position.Line);
    }
  }
}
