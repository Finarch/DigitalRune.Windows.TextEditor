using System;
using DigitalRune.Windows.TextEditor.Document;

namespace DigitalRune.Windows.TextEditor.Folding
{
  /// <summary>
  /// Describes a folding.
  /// </summary>
  public class FoldMarker : ISegment, IComparable<FoldMarker>
  {
    private int offset = -1;
    private int length = -1;
    bool isFolded = false;
    string foldText = "...";
    IDocument document = null;
    private int startLine;
    private int startColumn;
    private int endLine;
    private int endColumn;


    /// <summary>
    /// Gets or sets the offset.
    /// </summary>
    /// <value>The offset where the span begins</value>
    public int Offset
    {
      get { return offset; }
      set
      {
        offset = value; 
        Update();
      }
    }


    /// <summary>
    /// Gets or sets the length.
    /// </summary>
    /// <value>The length of the span</value>
    public int Length
    {
      get { return length; }
      set
      {
        length = value; 
        Update();
      }
    }


    /// <summary>
    /// Gets the start line.
    /// </summary>
    /// <value>The start line.</value>
    public int StartLine
    {
      get { return startLine; }
    }


    /// <summary>
    /// Gets the start column.
    /// </summary>
    /// <value>The start column.</value>
    public int StartColumn
    {
      get { return startColumn; }
    }


    /// <summary>
    /// Gets the end line.
    /// </summary>
    /// <value>The end line.</value>
    public int EndLine
    {
      get { return endLine; }
    }


    /// <summary>
    /// Gets the end column.
    /// </summary>
    /// <value>The end column.</value>
    public int EndColumn
    {
      get { return endColumn; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether this instance is folded.
    /// </summary>
    /// <value><c>true</c> if this instance is folded; otherwise, <c>false</c>.</value>
    public bool IsFolded
    {
      get { return isFolded; }
      set
      {
        if (isFolded != value)
        {
          document.FoldingManager.ClearCache();
          isFolded = value;
        }
      }
    }


    /// <summary>
    /// Gets the label of the folding (shown when folded).
    /// </summary>
    /// <value>The label of the folding.</value>
    public string FoldText
    {
      get { return foldText; }
    }


    /// <summary>
    /// Gets the inner text.
    /// </summary>
    /// <value>The inner text.</value>
    public string InnerText
    {
      get { return document.GetText(offset, length); }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="FoldMarker"/> class.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="offset">The Offset.</param>
    /// <param name="length">The Length.</param>
    /// <param name="foldText">The label of the folding (shown when folded).</param>
    /// <param name="isFolded">Flag that defines whether the text is folded or not.</param>
    public FoldMarker(IDocument document, int offset, int length, string foldText, bool isFolded)
    {
      this.document = document;
      this.offset = offset;
      this.length = length;
      this.foldText = foldText;
      this.isFolded = isFolded;
      Update();
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="FoldMarker"/> class.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="startLine">The start line.</param>
    /// <param name="startColumn">The start column.</param>
    /// <param name="endLine">The end line.</param>
    /// <param name="endColumn">The end column.</param>
    public FoldMarker(IDocument document, int startLine, int startColumn, int endLine, int endColumn)
      : this(document, startLine, startColumn, endLine, endColumn, "...")
    {
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="FoldMarker"/> class.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="startLine">The start line.</param>
    /// <param name="startColumn">The start column.</param>
    /// <param name="endLine">The end line.</param>
    /// <param name="endColumn">The end column.</param>
    /// <param name="foldText">The label of the folding (shown when folded).</param>
    public FoldMarker(IDocument document, int startLine, int startColumn, int endLine, int endColumn, string foldText)
      : this(document, startLine, startColumn, endLine, endColumn, foldText, false)
    {
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="FoldMarker"/> class.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="startLine">The start line.</param>
    /// <param name="startColumn">The start column.</param>
    /// <param name="endLine">The end line.</param>
    /// <param name="endColumn">The end column.</param>
    /// <param name="foldText">The label of the folding (shown when folded).</param>
    /// <param name="isFolded">Flag that defines whether the text is folded or not.</param>
    public FoldMarker(IDocument document, int startLine, int startColumn, int endLine, int endColumn, string foldText, bool isFolded)
    {
      this.document = document;

      startLine = Math.Min(document.TotalNumberOfLines - 1, Math.Max(startLine, 0));
      ISegment startLineSegment = document.GetLineSegment(startLine);

      endLine = Math.Min(document.TotalNumberOfLines - 1, Math.Max(endLine, 0));
      ISegment endLineSegment = document.GetLineSegment(endLine);

      // Prevent the region from completely disappearing
      if (string.IsNullOrEmpty(foldText))
      {
        foldText = "...";
      }

      this.foldText = foldText;
      this.offset = startLineSegment.Offset + Math.Min(startColumn, startLineSegment.Length);
      this.length = (endLineSegment.Offset + Math.Min(endColumn, endLineSegment.Length)) - this.Offset;
      this.isFolded = isFolded;
      Update();
    }


    private void Update()
    {
      TextLocation location = GetPointForOffset(document, offset);
      startLine = location.Line;
      startColumn = location.Column;
      location = GetPointForOffset(document, offset + Length);
      endLine = location.Line;
      endColumn = location.Column;
    }


    static TextLocation GetPointForOffset(IDocument document, int offset)
    {
      if (offset > document.TextLength)
      {
        return new TextLocation(1, document.TotalNumberOfLines + 1);
      }
      else if (offset < 0)
      {
        return new TextLocation(-1, -1);
      }
      else
      {
        int line = document.GetLineNumberForOffset(offset);
        int column = offset - document.GetLineSegment(line).Offset;
        return new TextLocation(column, line);
      }
    }

    
    /// <summary>
    /// Compares the current object with another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the other parameter.Zero This object is equal to other. Greater than zero This object is greater than other.
    /// </returns>
    public int CompareTo(FoldMarker other)
    {
      if (offset != other.Offset)
        return offset.CompareTo(other.Offset);

      return length.CompareTo(other.Length);
    }


    /// <summary>
    /// Returns a <see cref="String"></see> that represents the current <see cref="Object"></see>.
    /// </summary>
    /// <returns>
    /// A <see cref="String"></see> that represents the current <see cref="Object"></see>.
    /// </returns>
    public override string ToString()
    {
      return String.Format("[FoldMarker: Offset = {0}, Length = {1}, Text = {3}, IsFolded = {4}]", Offset, Length, foldText, isFolded);
    }
  }
}
