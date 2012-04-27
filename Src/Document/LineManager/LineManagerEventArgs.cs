using System;

namespace DigitalRune.Windows.TextEditor.Document
{
  /// <summary>
  /// Event arguments for events raised by an <see cref="LineManager"/>.
  /// </summary>
  public class LineCountChangeEventArgs : EventArgs
  {
    IDocument document;
    int start;
    int moved;


    /// <summary>
    /// Always a valid Document which is related to the event.
    /// </summary>
    /// <value>The document.</value>
    public IDocument Document
    {
      get { return document; }
    }


    /// <summary>
    /// Gets the line start (-1 if no offset was specified for this event).
    /// </summary>
    /// <value>The line start (-1 if no offset was specified for this event).</value>
    public int LineStart
    {
      get { return start; }
    }


    /// <summary>
    /// Gets the lines moved (-1 if no length was specified for this event).
    /// </summary>
    /// <value>The lines moved (-1 if no length was specified for this event).</value>
    public int LinesMoved
    {
      get { return moved; }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="LineCountChangeEventArgs"/> class.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="lineStart">The line start.</param>
    /// <param name="linesMoved">The lines moved.</param>
    public LineCountChangeEventArgs(IDocument document, int lineStart, int linesMoved)
    {
      this.document = document;
      this.start = lineStart;
      this.moved = linesMoved;
    }
  }


  /// <summary>
  /// Event arguments for events raised by an <see cref="LineManager"/>.
  /// </summary>
  public class LineEventArgs : EventArgs
  {
    IDocument document;
    LineSegment lineSegment;


    /// <summary>
    /// Gets the document.
    /// </summary>
    /// <value>The document.</value>
    public IDocument Document
    {
      get { return document; }
    }


    /// <summary>
    /// Gets the line segment.
    /// </summary>
    /// <value>The line segment.</value>
    public LineSegment LineSegment
    {
      get { return lineSegment; }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="LineEventArgs"/> class.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="lineSegment">The line segment.</param>
    public LineEventArgs(IDocument document, LineSegment lineSegment)
    {
      this.document = document;
      this.lineSegment = lineSegment;
    }


    /// <summary>
    /// Returns a <see cref="String"></see> that represents the current <see cref="Object"></see>.
    /// </summary>
    /// <returns>
    /// A <see cref="String"></see> that represents the current <see cref="Object"></see>.
    /// </returns>
    public override string ToString()
    {
      return string.Format("[LineEventArgs Document={0} LineSegment={1}]", this.document, this.lineSegment);
    }
  }


  /// <summary>
  /// Event arguments for events raised when the length of lines changes.
  /// </summary>
	public class LineLengthChangeEventArgs : LineEventArgs
	{
		int lengthDelta;


    /// <summary>
    /// Gets the length delta.
    /// </summary>
    /// <value>The length delta.</value>
		public int LengthDelta 
    {
			get { return lengthDelta; }
		}
		

    /// <summary>
    /// Initializes a new instance of the <see cref="LineLengthChangeEventArgs"/> class.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="lineSegment">The line segment.</param>
    /// <param name="moved">The moved.</param>
		public LineLengthChangeEventArgs(IDocument document, LineSegment lineSegment, int moved)
			: base(document, lineSegment)
		{
			this.lengthDelta = moved;
		}
		

    /// <summary>
    /// Returns a <see cref="String"></see> that represents the current <see cref="Object"></see>.
    /// </summary>
    /// <returns>
    /// A <see cref="String"></see> that represents the current <see cref="Object"></see>.
    /// </returns>
		public override string ToString()
		{
			return string.Format("[LineLengthEventArgs Document={0} LineSegment={1} LengthDelta={2}]", this.Document, this.LineSegment, this.lengthDelta);
		}
  }
}
