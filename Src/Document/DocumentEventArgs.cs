using System;

namespace DigitalRune.Windows.TextEditor.Document
{
  /// <summary>
  /// This class contains more information on a document event
  /// </summary>
  public class DocumentEventArgs : EventArgs
  {
    IDocument document;
    int offset;
    int length;
    string text;


    /// <summary>
    /// Gets the document which is related to the event.
    /// </summary>
    /// <value>The document which is related to the event.</value>
    public IDocument Document
    {
      get { return document; }
    }


    /// <summary>
    /// Gets the offset where the document was changed.
    /// </summary>
    /// <value>The offset of the change.</value>
    /// <remarks>
    /// Returns <c>-1</c> if no offset was specified for this event.
    /// </remarks>
    public int Offset
    {
      get { return offset; }
    }


    /// <summary>
    /// Gets the text that is inserted.
    /// </summary>
    /// <value>The text that is inserted.</value>
    /// <returns>
    /// Returns <c>null</c> if no text was inserted.
    /// </returns>
    public string Text
    {
      get { return text; }
    }


    /// <summary>
    /// Gets the number of characters removed at <see cref="Offset"/>.
    /// </summary>
    /// <value>The number of character removed.</value>
    /// <returns>
    /// <c>-1</c> if no characters were removed.
    /// </returns>
    public int Length
    {
      get { return length; }
    }


    /// <summary>
    /// Creates a new instance off <see cref="DocumentEventArgs"/>
    /// </summary>
    /// <param name="document">The document.</param>
    public DocumentEventArgs(IDocument document)
      : this(document, -1, -1, null)
    {
    }


    /// <summary>
    /// Creates a new instance off <see cref="DocumentEventArgs"/>
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="offset">The offset where the document was changed.</param>
    public DocumentEventArgs(IDocument document, int offset)
      : this(document, offset, -1, null)
    {
    }


    /// <summary>
    /// Creates a new instance off <see cref="DocumentEventArgs"/>
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="offset">The offset where the document was changed.</param>
    /// <param name="length">The number of characters removed at <paramref name="offset"/>.</param>
    public DocumentEventArgs(IDocument document, int offset, int length)
      : this(document, offset, length, null)
    {
    }


    /// <summary>
    /// Creates a new instance off <see cref="DocumentEventArgs"/>
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="offset">The offset where the document was changed.</param>
    /// <param name="length">The number of characters removed at <paramref name="offset"/>.</param>
    /// <param name="text">The text inserted at <paramref name="offset"/>.</param>
    public DocumentEventArgs(IDocument document, int offset, int length, string text)
    {
      this.document = document;
      this.offset = offset;
      this.length = length;
      this.text = text;
    }


    /// <summary>
    /// Returns a <see cref="String"></see> that represents the current <see cref="Object"></see>.
    /// </summary>
    /// <returns>
    /// A <see cref="String"></see> that represents the current <see cref="Object"></see>.
    /// </returns>
    public override string ToString()
    {
      return String.Format("[DocumentEventArgs: Document = {0}, Offset = {1}, Text = {2}, Length = {3}]",
                           Document,
                           Offset,
                           Text,
                           Length);
    }
  }
}
