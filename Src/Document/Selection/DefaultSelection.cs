using System;
using System.Diagnostics;
using DigitalRune.Windows.TextEditor.Document;


namespace DigitalRune.Windows.TextEditor.Selections
{
  /// <summary>
  /// Default implementation of an <see cref="ISelection"/> interface.
  /// </summary>
  public class DefaultSelection : ISelection
  {
    IDocument document;
    bool isRectangularSelection;
    TextLocation startPosition;
    TextLocation endPosition;


    /// <summary>
    /// Gets or sets the start position of the selection.
    /// </summary>
    /// <value>The start position of the selection.</value>
    public TextLocation StartPosition
    {
      get
      {
        return startPosition;
      }
      set
      {
        DefaultDocument.ValidatePosition(document, value);
        startPosition = value;
      }
    }


    /// <summary>
    /// Gets or sets the end position of the selection.
    /// </summary>
    /// <value>The end position of the selection.</value>
    public TextLocation EndPosition
    {
      get
      {
        return endPosition;
      }
      set
      {
        DefaultDocument.ValidatePosition(document, value);
        endPosition = value;
      }
    }


    /// <summary>
    /// Gets the offset of the selection.
    /// </summary>
    /// <value>The offset of the selection.</value>
    public int Offset
    {
      get { return document.PositionToOffset(startPosition); }
    }


    /// <summary>
    /// Gets the end offset of the selection.
    /// </summary>
    /// <value>The end offset of the selection.</value>
    public int EndOffset
    {
      get { return document.PositionToOffset(endPosition); }
    }


    /// <summary>
    /// Gets the length of the selection.
    /// </summary>
    /// <value>The length of the selection.</value>
    public int Length
    {
      get { return EndOffset - Offset; }
    }


    /// <summary>
    /// Gets a value indicating whether this selection is empty.
    /// </summary>
    /// <value>Returns true, if the selection is empty</value>
    public bool IsEmpty
    {
      get { return startPosition == endPosition; }
    }


    /// <value>
    /// Returns true, if the selection is rectangular
    /// </value>
    // TODO : make this unused property used.
    public bool IsRectangularSelection
    {
      get { return isRectangularSelection; }
      set { isRectangularSelection = value; }
    }


    /// <summary>
    /// Gets the selected text.
    /// </summary>
    /// <value>The text which is selected by this selection.</value>
    public string SelectedText
    {
      get
      {
        if (document != null)
        {
          if (Length < 0)
            return null;

          return document.GetText(Offset, Length);
        }
        return null;
      }
    }


    /// <summary>
    /// Creates a new instance of <see cref="DefaultSelection"/>
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="startPosition">The start position.</param>
    /// <param name="endPosition">The end position.</param>
    public DefaultSelection(IDocument document, TextLocation startPosition, TextLocation endPosition)
    {
      DefaultDocument.ValidatePosition(document, startPosition);
      DefaultDocument.ValidatePosition(document, endPosition);
      Debug.Assert(startPosition <= endPosition);
      this.document = document;
      this.startPosition = startPosition;
      this.endPosition = endPosition;
    }


    /// <summary>
    /// Converts a <see cref="DefaultSelection"/> instance to string 
    /// (only for debugging purposes).
    /// </summary>
    public override string ToString()
    {
      return String.Format("[DefaultSelection : StartPosition={0}, EndPosition={1}]", startPosition, endPosition);
    }


    /// <summary>
    /// Determines whether this selection contains the specified position.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <returns>
    /// 	<c>true</c> if this selection contains the specified position; otherwise, <c>false</c>.
    /// </returns>
    public bool ContainsPosition(TextLocation position)
    {
      if (this.IsEmpty)
        return false;
      return startPosition.Y < position.Y && position.Y < endPosition.Y ||
        startPosition.Y == position.Y && startPosition.X <= position.X && (startPosition.Y != endPosition.Y || position.X <= endPosition.X) ||
        endPosition.Y == position.Y && startPosition.Y != endPosition.Y && position.X <= endPosition.X;
    }


    /// <summary>
    /// Determines whether this selection contains the specified offset.
    /// </summary>
    /// <param name="offset">The specified offset.</param>
    /// <returns>
    /// <c>true</c> if this selection contains the specified offset; otherwise,
    /// <c>false</c>.
    /// </returns>
    public bool ContainsOffset(int offset)
    {
      return Offset <= offset && offset <= EndOffset;
    }
  }
}
