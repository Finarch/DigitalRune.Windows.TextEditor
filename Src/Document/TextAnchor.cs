using System;


namespace DigitalRune.Windows.TextEditor.Document
{
	/// <summary>
	/// References a certain location in a document.
	/// </summary>
	public sealed class TextAnchor
	{
		static Exception AnchorDeletedError()
		{
			return new InvalidOperationException("The text containing the anchor was deleted");
		}
		
		LineSegment lineSegment;
		int columnNumber;

    /// <summary>
    /// Gets (or sets) the line.
    /// </summary>
    /// <value>The line.</value>
		public LineSegment Line {
			get {
				if (lineSegment == null) throw AnchorDeletedError();
				return lineSegment;
			}
			internal set {
				lineSegment = value;
			}
		}


    /// <summary>
    /// Gets a value indicating whether this instance is deleted.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is deleted; otherwise, <c>false</c>.
    /// </value>
		public bool IsDeleted {
			get {
				return lineSegment == null;
			}
		}


    /// <summary>
    /// Gets the line number.
    /// </summary>
    /// <value>The line number.</value>
		public int LineNumber {
			get {
				return Line.LineNumber;
			}
		}


    /// <summary>
    /// Gets or sets the column number.
    /// </summary>
    /// <value>The column number.</value>
		public int ColumnNumber {
			get {
				if (lineSegment == null) throw AnchorDeletedError();
				return columnNumber;
			}
			internal set {
				columnNumber = value;
			}
		}


    /// <summary>
    /// Gets the location.
    /// </summary>
    /// <value>The location.</value>
		public TextLocation Location {
			get {
				return new TextLocation(this.ColumnNumber, this.LineNumber);
			}
		}


    /// <summary>
    /// Gets the offset.
    /// </summary>
    /// <value>The offset.</value>
		public int Offset {
			get {
				return this.Line.Offset + columnNumber;
			}
		}


		internal void Deleted()
		{
			lineSegment = null;
		}


    internal TextAnchor(LineSegment lineSegment, int columnNumber)
		{
			this.lineSegment = lineSegment;
			this.columnNumber = columnNumber;
		}


    /// <summary>
    /// Returns a <see cref="String"></see> that represents the current <see cref="TextAnchor"></see>.
    /// </summary>
    /// <returns>
    /// A <see cref="String"></see> that represents the current <see cref="TextAnchor"></see>.
    /// </returns>
    public override string ToString()
		{
			if (this.IsDeleted)
				return "[TextAnchor (deleted)]";
			else
				return "[TextAnchor " + this.Location.ToString() + "]";
		}
	}
}
