using System.Drawing;
using System.Drawing.Text;
using System.Text;
using DigitalRune.Windows.TextEditor.Highlighting;

namespace DigitalRune.Windows.TextEditor.Properties
{
  /// <summary>
  /// Describes the caret marker
  /// </summary>
  public enum LineViewerStyle
  {
    /// <summary>No line viewer will be displayed.</summary>
    None,
    /// <summary>The row in which the caret is will be marked.</summary>
    FullRow
  }


  /// <summary>
  /// Describes the indent style
  /// </summary>
  public enum IndentStyle
  {
    /// <summary>No indentation occurs.</summary>
    None,
    /// <summary>The indentation from the line above will be taken to indent the current line. </summary>
    Auto,
    /// <summary>Intelligent, context sensitive indentation will occur.</summary>
    Smart
  }


  /// <summary>
  /// Describes the selection mode of the text area
  /// </summary>
  public enum DocumentSelectionMode
  {
    /// <summary>The 'normal' selection mode.</summary>
    Normal,
    /// <summary>
    /// Selections will be added to the current selection or new  ones will 
    /// be created (multi-select mode)
    /// </summary>
    Additive
  }

  
  /// <summary>
  /// The bracket matching style.
  /// </summary>
  public enum BracketMatchingStyle
  {
    /// <summary>Cursor on bracket: Highlight brackets on cursor position.</summary>
    OnBracket,
    /// <summary> Cursor after bracket: Highlight brackets before cursor position.</summary>
    After
  }


  /// <summary>
  /// Stores the properties of a text editor.
  /// </summary>
  public interface ITextEditorProperties
  {
    //bool CaretLine
    //{
    //  get;
    //  set;
    //}


    /// <summary>
    /// Gets or sets a value indicating whether to automatically insert curly 
    /// brackets.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if automatically inserting curly brackets; otherwise, <c>false</c>.
    /// </value>
    bool AutoInsertCurlyBracket
    { 
      get;
      set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether to hide the mouse cursor while typing.
    /// </summary>
    /// <value><c>true</c> to hide the mouse cursor; otherwise, <c>false</c>.</value>
    bool HideMouseCursor
    { 
      get;
      set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether the icon bar visible.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if the icon bar visible; otherwise, <c>false</c>.
    /// </value>
    bool IsIconBarVisible
    { 
      get;
      set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether allow placing a caret beyond
    /// the end of the line (often called virtual space).
    /// </summary>
    /// <value>
    /// <c>true</c> if placing the caret beyond the end of line is allowed; 
    /// otherwise, <c>false</c>.
    /// </value>
    bool AllowCaretBeyondEOL
    {
      get;
      set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether to highlight matching brackets.
    /// </summary>
    /// <value>
    /// <c>true</c> if matching brackets are highlighted; otherwise, <c>false</c>.
    /// </value>
    bool ShowMatchingBracket
    { 
      get;
      set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether to cut/copy the whole line when
    /// nothing is selected.
    /// </summary>
    /// <value><c>true</c> to cut/copy the whole line; otherwise, <c>false</c>.</value>
    bool CutCopyWholeLine
    {
      get;
      set;
    }


    /// <summary>
    /// Gets or sets the <see cref="System.Drawing.Text.TextRenderingHint"/> for rendering the text.
    /// </summary>
    /// <value>The <see cref="System.Drawing.Text.TextRenderingHint"/>.</value>
    TextRenderingHint TextRenderingHint
    {
      get;
      set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether to scroll the text down or up
    /// when rotating the mouse wheel (default is 'down').
    /// </summary>
    /// <value>
    /// 	<c>true</c> to scroll down; <c>false</c> false to scroll up.
    /// </value>
    bool MouseWheelScrollDown
    {
      get;
      set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether to zoom the text with the mouse
    /// wheel.
    /// </summary>
    /// <value><c>true</c> to zoom the text with the mouse wheel; otherwise, <c>false</c>.</value>
    bool MouseWheelTextZoom
    {
      get;
      set;
    }


    /// <summary>
    /// Gets or sets the line terminator.
    /// </summary>
    /// <value>The line terminator.</value>
    string LineTerminator
    {
      get;
      set;
    }


    /// <summary>
    /// Gets or sets the style the current line is highlighted.
    /// </summary>
    /// <value>The line viewer style.</value>
    LineViewerStyle LineViewerStyle
    { 
      get;
      set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether to show invalid lines.
    /// </summary>
    /// <value><c>true</c> to show invalid lines; otherwise, <c>false</c>.</value>
    bool ShowInvalidLines
    { 
      get;
      set;
    }


    /// <summary>
    /// Gets or sets the column of the vertical ruler.
    /// </summary>
    /// <value>The column of vertical ruler.</value>
    int VerticalRulerColumn
    { 
      get;
      set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether to display spaces in the text area.
    /// </summary>
    /// <value><c>true</c> to display spaces in the text area; otherwise, <c>false</c>.</value>
    bool ShowSpaces
    { 
      get;
      set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether to display tabs.
    /// </summary>
    /// <value><c>true</c> to display tabs; otherwise, <c>false</c>.</value>
    bool ShowTabs
    { 
      get;
      set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether display the end-of-line marker.
    /// </summary>
    /// <value><c>true</c> to display the end-of-line marker; otherwise, <c>false</c>.</value>
    bool ShowEOLMarker
    { 
      get;
      set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether to convert tabs to spaces.
    /// </summary>
    /// <value>
    /// 	<c>true</c> to convert tabs to spaces; otherwise, <c>false</c>.
    /// </value>
    bool ConvertTabsToSpaces
    { 
      get;
      set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether to show horizontal ruler (columns).
    /// </summary>
    /// <value><c>true</c> to show the horizontal ruler; otherwise, <c>false</c>.</value>
    bool ShowHorizontalRuler
    { 
      get;
      set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether to show the vertical ruler (vertical guide).
    /// </summary>
    /// <value><c>true</c> to show the vertical ruler; otherwise, <c>false</c>.</value>
    bool ShowVerticalRuler
    { 
      get;
      set;
    }


    /// <summary>
    /// Gets or sets the encoding.
    /// </summary>
    /// <value>The encoding.</value>
    Encoding Encoding
    {
      get;
      set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether to enable folding.
    /// </summary>
    /// <value><c>true</c> if folding is enabled; otherwise, <c>false</c>.</value>
    bool EnableFolding
    { 
      get;
      set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether to show line numbers.
    /// </summary>
    /// <value><c>true</c> if line numbers are shown; otherwise, <c>false</c>.</value>
    bool ShowLineNumbers
    { 
      get;
      set;
    }


    /// <summary>
    /// The width of a tab.
    /// </summary>
    int TabIndent
    {
      get;
      set;
    }


    /// <summary>
    /// The amount of spaces a tab is converted to if <see cref="ConvertTabsToSpaces"/> is true.
    /// </summary>
    int IndentationSize
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the indent style.
    /// </summary>
    /// <value>The indent style.</value>
    IndentStyle IndentStyle
    { 
      get;
      set;
    }


    /// <summary>
    /// Gets or sets the selection mode.
    /// </summary>
    /// <value>The selection mode.</value>
    DocumentSelectionMode DocumentSelectionMode
    {
      get;
      set;
    }


    /// <summary>
    /// Gets or sets the font.
    /// </summary>
    /// <value>The font.</value>
    Font Font
    { 
      get;
      set;
    }


    /// <summary>
    /// Gets the font container.
    /// </summary>
    /// <value>The font container.</value>
    FontContainer FontContainer
    {
      get;
    }


    /// <summary>
    /// Gets or sets the bracket matching style.
    /// </summary>
    /// <value>The bracket matching style.</value>
    BracketMatchingStyle BracketMatchingStyle
    { 
      get;
      set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether this document supports read-only segments.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if the document supports read-only segments; otherwise, <c>false</c>.
    /// </value>
    bool SupportsReadOnlySegments
    {
      get;
      set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether to show scroll bars.
    /// </summary>
    /// <value><c>true</c> if to show scroll bars; otherwise, <c>false</c>.</value>
    bool ShowScrollBars
    {
      get;
      set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether to enable code completion.
    /// </summary>
    /// <value><c>true</c> if code completion is enabled; otherwise, <c>false</c>.</value>
    bool EnableCompletion
    {
      get;
      set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether to enable method insight.
    /// </summary>
    /// <value><c>true</c> if method insight is enabled; otherwise, <c>false</c>.</value>
    bool EnableInsight
    {
      get;
      set;
    }
  }
}
