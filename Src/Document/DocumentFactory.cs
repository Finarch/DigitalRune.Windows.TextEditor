using System;
using System.Text;
using DigitalRune.Windows.TextEditor.Bookmarks;
using DigitalRune.Windows.TextEditor.CustomLines;
using DigitalRune.Windows.TextEditor.Formatting;
using DigitalRune.Windows.TextEditor.Folding;
using DigitalRune.Windows.TextEditor.Markers;
using DigitalRune.Windows.TextEditor.TextBuffer;

namespace DigitalRune.Windows.TextEditor.Document
{
  /// <summary>
  /// This interface represents a container which holds a text sequence and
  /// all necessary information about it. It is used as the base for a text editor.
  /// </summary>
  public class DocumentFactory
  {
    /// <summary>
    /// Creates a new document.
    /// </summary>
    /// <returns>The new document.</returns>
    public IDocument CreateDocument()
    {
      DefaultDocument doc = new DefaultDocument();
      doc.TextBufferStrategy = new GapTextBufferStrategy();
      doc.FormattingStrategy = new DefaultFormattingStrategy();
      doc.LineManager = new LineManager(doc, null);
      doc.FoldingManager = new FoldingManager(doc, doc.LineManager);
      doc.FoldingManager.FoldingStrategy = null; //new ParserFoldingStrategy();
      doc.MarkerStrategy = new MarkerStrategy(doc);
      doc.BookmarkManager = new BookmarkManager(doc, doc.LineManager);
      doc.CustomLineManager = new CustomLineManager(doc.LineManager);
      return doc;
    }


    /// <summary>
    /// Creates a new document and loads the given file.
    /// </summary>
    /// <param name="textBuffer">The text buffer.</param>
    /// <returns>The document.</returns>
    public IDocument CreateFromTextBuffer(ITextBufferStrategy textBuffer)
    {
      DefaultDocument doc = (DefaultDocument) CreateDocument();
      doc.TextContent = textBuffer.GetText(0, textBuffer.Length);
      doc.TextBufferStrategy = textBuffer;
      return doc;
    }


    /// <summary>
    /// Creates a new document and loads the given file
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <returns>The document.</returns>
    public IDocument CreateFromFile(string fileName)
    {
      IDocument document = CreateDocument();
      Encoding encoding = Encoding.Default;
      document.TextContent = Utilities.FileReader.ReadFileContent(fileName, encoding);
      return document;
    }
  }
}
