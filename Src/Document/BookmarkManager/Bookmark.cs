using System;
using System.Drawing;
using System.Windows.Forms;
using DigitalRune.Windows.TextEditor.Document;

namespace DigitalRune.Windows.TextEditor.Bookmarks
{
  /// <summary>
  /// Description of Bookmark.
  /// </summary>
  public class Bookmark
  {
    IDocument document;
    LineSegment line;
    int lineNumber;
    bool isEnabled = true;

    /// <summary>
    /// Gets or sets the document.
    /// </summary>
    /// <value>The document.</value>
    public IDocument Document
    {
      get
      {
        return document;
      }
      set
      {
        if (document != value)
        {
          if (line != null)
          {
            lineNumber = line.LineNumber;
            line = null;
          }
          document = value;
          if (document != null)
          {
            line = document.GetLineSegment(Math.Min(lineNumber, document.TotalNumberOfLines - 1));
          }
          OnDocumentChanged(EventArgs.Empty);
        }
      }
    }


    /// <summary>
    /// Occurs when the document is has been changed.
    /// </summary>
    public event EventHandler DocumentChanged;


    /// <summary>
    /// Raises the <see cref="DocumentChanged"/> event.
    /// </summary>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected virtual void OnDocumentChanged(EventArgs e)
    {
      if (DocumentChanged != null)
        DocumentChanged(this, e);
    }


    /// <summary>
    /// Gets or sets a value indicating whether this bookmark is enabled.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this bookmark is enabled; otherwise, <c>false</c>.
    /// </value>
    public bool IsEnabled
    {
      get
      {
        return isEnabled;
      }
      set
      {
        if (isEnabled != value)
        {
          isEnabled = value;
          if (document != null)
          {
            document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.SingleLine, lineNumber));
            document.CommitUpdate();
          }
          OnIsEnabledChanged(EventArgs.Empty);
        }
      }
    }


    /// <summary>
    /// Occurs when the <see cref="IsEnabled"/> property has been changed.
    /// </summary>
    public event EventHandler IsEnabledChanged;


    /// <summary>
    /// Raises the <see cref="IsEnabledChanged"/> event.
    /// </summary>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected virtual void OnIsEnabledChanged(EventArgs e)
    {
      if (IsEnabledChanged != null)
        IsEnabledChanged(this, e);
    }


    /// <summary>
    /// Gets the line the bookmark belongs to.
    /// Is <c>null</c> if the bookmark is not connected to a document.
    /// </summary>
    public LineSegment Line
    {
      get { return line; }
    }


    /// <summary>
    /// Gets or sets the line number.
    /// </summary>
    /// <value>The line number.</value>
    public int LineNumber
    {
      get
      {
        if (line != null)
          return line.LineNumber;
        else
          return lineNumber;
      }
      set
      {
        if (value < 0)
          throw new ArgumentOutOfRangeException("value", value, "line number must be >= 0");
        if (document == null)
        {
          lineNumber = value;
        }
        else
        {
          line = document.GetLineSegment(value);
        }
      }
    }

    /// <summary>
    /// Gets if the bookmark can be toggled off using the 'set/unset bookmark' command.
    /// </summary>
    public virtual bool CanToggle
    {
      get { return true; }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="Bookmark"/> class.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="lineNumber">The line number.</param>
    public Bookmark(IDocument document, int lineNumber)
      : this(document, lineNumber, true)
    {
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="Bookmark"/> class.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="lineNumber">The line number.</param>
    /// <param name="isEnabled">if set to <c>true</c> [is enabled].</param>
    public Bookmark(IDocument document, int lineNumber, bool isEnabled)
    {
      this.document = document;
      this.isEnabled = isEnabled;
      this.LineNumber = lineNumber;
    }


    /// <summary>
    /// Handles a click on this bookmark on the <see cref="IconBarMargin"/>.
    /// </summary>
    /// <param name="parent">The parent control.</param>
    /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
    /// <returns><c>true</c> if mouse click has been handled and the bookmark is removed.</returns>
    /// <remarks>
    /// Per default a left-click on the bookmark removes it from the icon margin.
    /// </remarks>
    public virtual bool Click(Control parent, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left && CanToggle)
      {
        document.BookmarkManager.RemoveMark(this);
        return true;
      }
      return false;
    }


    /// <summary>
    /// Draws the bookmark on the <see cref="IconBarMargin"/>.
    /// </summary>
    /// <param name="g">The <see cref="Graphics"/> context.</param>
    /// <param name="rectangle">The bounding rectangle.</param>
    public virtual void Draw(Graphics g, Rectangle rectangle)
    {
      BookmarkRenderer.DrawBookmark(g, rectangle, isEnabled);
    }
  }
}
