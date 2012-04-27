using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using DigitalRune.Windows.TextEditor.Document;

namespace DigitalRune.Windows.TextEditor.Selections
{
  /// <summary>
  /// Manages the selections in a document.
  /// </summary>
  public class SelectionManager
  {
    internal TextLocation selectionStart;

    internal TextLocation SelectionStart
    {
      get { return selectionStart; }
      set
      {
        DefaultDocument.ValidatePosition(document, value);
        selectionStart = value;
      }
    }

    IDocument document;
    TextArea textArea;
    internal SelectFrom selectFrom = new SelectFrom();
    List<ISelection> selectionCollection = new List<ISelection>();


    /// <summary>
    /// Gets the selection collection.
    /// </summary>
    /// <value>A collection containing all selections.</value>
    public List<ISelection> SelectionCollection
    {
      get { return selectionCollection; }
    }


    /// <summary>
    /// Gets a value indicating whether this instance has something selected.
    /// </summary>
    /// <value>
    /// <c>true</c> if the <see cref="SelectionCollection"/> is not empty, 
    /// <c>false</c> otherwise.
    /// </value>
    public bool HasSomethingSelected
    {
      get { return selectionCollection.Count > 0; }
    }


    /// <summary>
    /// Gets a value indicating whether the selections are read-only.
    /// </summary>
    /// <value>
    /// <c>true</c> if the selections are read-only; otherwise, <c>false</c>.
    /// </value>
    public bool SelectionIsReadonly
    {
      get
      {
        if (document.ReadOnly)
          return true;

        if (document.TextEditorProperties.UseCustomLine)
        {
          foreach (ISelection sel in selectionCollection)
          {
            if (document.CustomLineManager.IsReadOnly(sel, false))
              return true;
          }
        }
        return false;
      }
    }


    /// <summary>
    /// Gets the selected text.
    /// </summary>
    /// <value>The text that is currently selected.</value>
    /// <remarks>
    /// If multiple non-consecutive texts are selected, then these texts are 
    /// returned as a single string.
    /// </remarks>
    public string SelectedText
    {
      get
      {
        StringBuilder builder = new StringBuilder();

        foreach (ISelection s in selectionCollection)
          builder.Append(s.SelectedText);

        return builder.ToString();
      }
    }


    /// <summary>
    /// Creates a new instance of <see cref="SelectionManager"/>
    /// </summary>
    /// <param name="document">The document.</param>
    public SelectionManager(IDocument document)
    {
      this.document = document;
    }


    /// <summary>
    /// Creates a new instance of <see cref="SelectionManager"/>
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="textArea">The text area.</param>
    public SelectionManager(IDocument document, TextArea textArea)
    {
      this.document = document;
      this.textArea = textArea;
    }


    /// <remarks>
    /// Clears the selection and sets a new selection
    /// using the given <see cref="ISelection"/> object.
    /// </remarks>
    public void SetSelection(ISelection selection)
    {
      if (selection != null)
      {
        if (SelectionCollection.Count == 1 &&
            selection.StartPosition == SelectionCollection[0].StartPosition &&
            selection.EndPosition == SelectionCollection[0].EndPosition)
        {
          return;
        }
        ClearWithoutUpdate();
        selectionCollection.Add(selection);
        document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.LinesBetween, selection.StartPosition.Y, selection.EndPosition.Y));
        document.CommitUpdate();
        OnSelectionChanged(EventArgs.Empty);
      }
      else
      {
        ClearSelection();
      }
    }


    /// <summary>
    /// Sets a selection.
    /// </summary>
    /// <param name="startPosition">The start position.</param>
    /// <param name="endPosition">The end position.</param>
    public void SetSelection(TextLocation startPosition, TextLocation endPosition)
    {
      SetSelection(new DefaultSelection(document, startPosition, endPosition));
    }


    /// <summary>
    /// Determines whether a text position is greater than or equal to another 
    /// text position.
    /// </summary>
    /// <param name="position1">The first position.</param>
    /// <param name="position2">The second position.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="position1"/> is greater than or equal to 
    /// <paramref name="position2"/>; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsPositionGreaterOrEqual(TextLocation position1, TextLocation position2)
    {
      return position1.Y > position2.Y || position1.Y == position2.Y && position1.X >= position2.X;
    }


    /// <summary>
    /// Extends the selection.
    /// </summary>
    /// <param name="oldPosition">The old position.</param>
    /// <param name="newPosition">The new position.</param>
    public void ExtendSelection(TextLocation oldPosition, TextLocation newPosition)
    {
      // where oldposition is where the cursor was,
      // and newposition is where it has ended up from a click (both zero based)

      if (oldPosition == newPosition)
      {
        return;
      }

      TextLocation min;
      TextLocation max;
      int oldnewX = newPosition.X;
      bool oldIsGreater = IsPositionGreaterOrEqual(oldPosition, newPosition);
      if (oldIsGreater)
      {
        min = newPosition;
        max = oldPosition;
      }
      else
      {
        min = oldPosition;
        max = newPosition;
      }

      if (min == max)
      {
        return;
      }

      if (!HasSomethingSelected)
      {
        SetSelection(new DefaultSelection(document, min, max));
        // initialise selectFrom for a cursor selection
        if (selectFrom.where == WhereFrom.None)
          SelectionStart = oldPosition; //textArea.Caret.Position;
        return;
      }

      ISelection selection = this.selectionCollection[0];

      if (min == max)
      {
        //selection.StartPosition = newPosition;
        return;
      }
      else
      {
        // changed selection via gutter
        if (selectFrom.where == WhereFrom.Gutter)
        {
          // selection new position is always at the left edge for gutter selections
          newPosition.X = 0;
        }

        if (IsPositionGreaterOrEqual(newPosition, SelectionStart)) // selecting forward
        {
          selection.StartPosition = SelectionStart;
          // this handles last line selection
          if (selectFrom.where == WhereFrom.Gutter) //&& newPosition.Y != oldPosition.Y)
            selection.EndPosition = new TextLocation(textArea.Caret.Column, textArea.Caret.Line);
          else
          {
            newPosition.X = oldnewX;
            selection.EndPosition = newPosition;
          }
        }
        else
        { // selecting back
          if (selectFrom.where == WhereFrom.Gutter && selectFrom.first == WhereFrom.Gutter)
          { // gutter selection
            selection.EndPosition = NextValidPosition(SelectionStart.Y);
          }
          else
          { // internal text selection
            selection.EndPosition = SelectionStart; //selection.StartPosition;
          }
          selection.StartPosition = newPosition;
        }
      }

      document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.LinesBetween, min.Y, max.Y));
      document.CommitUpdate();
      OnSelectionChanged(EventArgs.Empty);
    }


    /// <summary>
    /// Returns the next valid position.
    /// </summary>
    /// <param name="line">The line.</param>
    /// <returns>The next valid position after the given line.</returns>
    /// <remarks>
    /// This methods checks that there are more lines available after current one.
    /// If there are then the next line is returned. Otherwise, the last position 
    /// on the given line is returned.
    /// </remarks>
    public TextLocation NextValidPosition(int line)
    {
      if (line < document.TotalNumberOfLines - 1)
        return new TextLocation(0, line + 1);
      else
        return new TextLocation(document.GetLineSegment(document.TotalNumberOfLines - 1).Length + 1, line);
    }


    /// <summary>
    /// Clears the without update.
    /// </summary>
    void ClearWithoutUpdate()
    {
      while (selectionCollection.Count > 0)
      {
        ISelection selection = selectionCollection[selectionCollection.Count - 1];
        selectionCollection.RemoveAt(selectionCollection.Count - 1);
        document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.LinesBetween, selection.StartPosition.Y, selection.EndPosition.Y));
        OnSelectionChanged(EventArgs.Empty);
      }
    }


    /// <remarks>
    /// Clears the selection.
    /// </remarks>
    public void ClearSelection()
    {
      Point mousepos;
      mousepos = textArea.mousepos;
      // this is the most logical place to reset selection starting
      // positions because it is always called before a new selection
      selectFrom.first = selectFrom.where;
      TextLocation newSelectionStart = textArea.TextView.GetLogicalPosition(mousepos.X - textArea.TextView.DrawingPosition.X, mousepos.Y - textArea.TextView.DrawingPosition.Y);
      if (selectFrom.where == WhereFrom.Gutter)
        newSelectionStart.X = 0;

      if (newSelectionStart.Line >= document.TotalNumberOfLines)
      {
        newSelectionStart.Line = document.TotalNumberOfLines - 1;
        newSelectionStart.Column = document.GetLineSegment(document.TotalNumberOfLines - 1).Length;
      }
      this.SelectionStart = newSelectionStart;

      ClearWithoutUpdate();
      document.CommitUpdate();
    }


    /// <remarks>
    /// Removes the selected text from the buffer and clears
    /// the selection.
    /// </remarks>
    public void RemoveSelectedText()
    {
      List<int> lines = new List<int>();
      int offset = -1;
      bool oneLine = true;
      foreach (ISelection s in selectionCollection)
      {
        if (oneLine)
        {
          int lineBegin = s.StartPosition.Y;
          if (lineBegin != s.EndPosition.Y)
          {
            oneLine = false;
          }
          else
          {
            lines.Add(lineBegin);
          }
        }
        offset = s.Offset;
        document.Remove(s.Offset, s.Length);
      }
      ClearSelection();
      if (offset >= 0)
      {
        // TODO:
        //   document.Caret.Offset = offset;
      }
      if (offset != -1)
      {
        if (oneLine)
        {
          foreach (int i in lines)
            document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.SingleLine, i));
        }
        else
        {
          document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
        }
        document.CommitUpdate();
      }
    }


    bool SelectionsOverlap(ISelection s1, ISelection s2)
    {
      return (s1.Offset <= s2.Offset && s2.Offset <= s1.Offset + s1.Length) ||
        (s1.Offset <= s2.Offset + s2.Length && s2.Offset + s2.Length <= s1.Offset + s1.Length) ||
        (s1.Offset >= s2.Offset && s1.Offset + s1.Length <= s2.Offset + s2.Length);
    }


    /// <summary>
    /// Determines whether the specified offset points to a section which is
    /// selected.
    /// </summary>
    /// <param name="offset">The offset.</param>
    /// <returns>
    /// 	<c>true</c> if the specified offset is selected; otherwise, <c>false</c>.
    /// </returns>
    public bool IsSelected(int offset)
    {
      return GetSelectionAt(offset) != null;
    }


    /// <summary>
    /// Returns a <see cref="ISelection"/> object giving the selection in which
    /// the offset points to.
    /// </summary>
    /// <param name="offset">The offset.</param>
    /// <returns>
    /// 	<c>null</c> if the offset doesn't point to a selection
    /// </returns>
    public ISelection GetSelectionAt(int offset)
    {
      foreach (ISelection s in selectionCollection)
      {
        if (s.ContainsOffset(offset))
          return s;
      }
      return null;
    }


    /// <summary>
    /// Gets the selection at a given line.
    /// </summary>
    /// <param name="lineNumber">The line number.</param>
    /// <returns>The range of selected columns.</returns>
    public ColumnRange GetSelectionAtLine(int lineNumber)
    {
      foreach (ISelection selection in selectionCollection)
      {
        int startLine = selection.StartPosition.Y;
        int endLine = selection.EndPosition.Y;
        if (startLine < lineNumber && lineNumber < endLine)
        {
          return ColumnRange.WholeColumn;
        }

        if (startLine == lineNumber)
        {
          LineSegment line = document.GetLineSegment(startLine);
          int startColumn = selection.StartPosition.X;
          int endColumn = endLine == lineNumber ? selection.EndPosition.X : line.Length + 1;
          return new ColumnRange(startColumn, endColumn);
        }

        if (endLine == lineNumber)
        {
          int endColumn = selection.EndPosition.X;
          return new ColumnRange(0, endColumn);
        }
      }

      return ColumnRange.NoColumn;
    }


    /// <summary>
    /// Fires the selection changed.
    /// </summary>
    public void FireSelectionChanged()
    {
      OnSelectionChanged(EventArgs.Empty);
    }


    /// <summary>
    /// Raises the <see cref="SelectionChanged"/> event.
    /// </summary>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected virtual void OnSelectionChanged(EventArgs e)
    {
      if (SelectionChanged != null)
        SelectionChanged(this, e);
    }


    /// <summary>
    /// Occurs when the selection is changed.
    /// </summary>
    public event EventHandler SelectionChanged;
  }


  // selection initiated from...
  internal class SelectFrom
  {
    public int where = WhereFrom.None; // last selection initiator
    public int first = WhereFrom.None; // first selection initiator

    public SelectFrom()
    {
    }
  }

  // selection initiated from type...
  internal class WhereFrom
  {
    public const int None = 0;
    public const int Gutter = 1;
    public const int TArea = 2;
  }
}
