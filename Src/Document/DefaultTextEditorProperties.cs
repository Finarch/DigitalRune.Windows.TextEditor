using System.Drawing;
using System.Drawing.Text;
using System.Text;
using DigitalRune.Windows.TextEditor.Highlighting;

namespace DigitalRune.Windows.TextEditor.Properties
{
  /// <summary>
  /// The default properties of a text editor.
  /// </summary>
  public class DefaultTextEditorProperties : ITextEditorProperties
  {
    int tabIndent = 4;
    int indentationSize = 4;
    IndentStyle indentStyle = IndentStyle.Smart;
    DocumentSelectionMode documentSelectionMode = DocumentSelectionMode.Normal;
    Encoding encoding = System.Text.Encoding.UTF8;
    BracketMatchingStyle bracketMatchingStyle = BracketMatchingStyle.After;
    FontContainer fontContainer;
    static Font DefaultFont;


    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultTextEditorProperties"/> class.
    /// </summary>
    public DefaultTextEditorProperties()
    {
      if (DefaultFont == null)
        DefaultFont = new Font("Courier New", 10);

      this.fontContainer = new FontContainer(DefaultFont);
    }


    bool allowCaretBeyondEOL = false;
    bool showMatchingBracket = true;
    bool showLineNumbers = true;
    bool showSpaces = false;
    bool showTabs = false;
    bool showEOLMarker = false;
    bool showInvalidLines = false;
    bool isIconBarVisible = false;
    bool enableFolding = true;
    bool showHorizontalRuler = false;
    bool showVerticalRuler = true;
    bool convertTabsToSpaces = false;
    TextRenderingHint textRenderingHint = TextRenderingHint.SystemDefault;
    bool mouseWheelScrollDown = true;
    bool mouseWheelTextZoom = true;
    bool hideMouseCursor = false;
    bool cutCopyWholeLine = true;
    int verticalRulerRow = 80;
    LineViewerStyle lineViewerStyle = LineViewerStyle.None;
    string lineTerminator = "\r\n";
    bool autoInsertCurlyBracket = true;
    bool useCustomLine = false;
    bool _showScrollBars = true;
    bool _enableCompletion = true;
    bool _enableMethodInsight = true;


    /// <summary>
    /// Gets or sets the width of a tab character.
    /// </summary>
    /// <value>The width in spaces of a tab character.</value>
    public int TabIndent
    {
      get { return tabIndent; }
      set { tabIndent = value; }
    }


    /// <summary>
    /// The amount of spaces a tab is converted to if <see cref="ConvertTabsToSpaces"/> is true.
    /// </summary>
    /// <value></value>
    public int IndentationSize
    {
      get { return indentationSize; }
      set { indentationSize = value; }
    }

    
    /// <summary>
    /// Gets or sets the indent style.
    /// </summary>
    /// <value>The indent style.</value>
    public IndentStyle IndentStyle
    {
      get { return indentStyle; }
      set { indentStyle = value; }
    }


    /// <summary>
    /// Gets or sets the selection mode.
    /// </summary>
    /// <value>The selection mode.</value>
    public DocumentSelectionMode DocumentSelectionMode
    {
      get { return documentSelectionMode; }
      set { documentSelectionMode = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether allow placing a caret beyond
    /// the end of the line (often called virtual space).
    /// </summary>
    /// <value>
    /// 	<c>true</c> if placing the caret beyond the end of line is allowed;
    /// otherwise, <c>false</c>.
    /// </value>
    public bool AllowCaretBeyondEOL
    {
      get { return allowCaretBeyondEOL; }
      set { allowCaretBeyondEOL = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to highlight matching brackets.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if matching brackets are highlighted; otherwise, <c>false</c>.
    /// </value>
    public bool ShowMatchingBracket
    {
      get { return showMatchingBracket; }
      set { showMatchingBracket = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to show line numbers.
    /// </summary>
    /// <value><c>true</c> if line numbers are shown; otherwise, <c>false</c>.</value>
    public bool ShowLineNumbers
    {
      get { return showLineNumbers; }
      set { showLineNumbers = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to display spaces in the text area.
    /// </summary>
    /// <value>
    /// 	<c>true</c> to display spaces in the text area; otherwise, <c>false</c>.
    /// </value>
    public bool ShowSpaces
    {
      get { return showSpaces; }
      set { showSpaces = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to display tabs.
    /// </summary>
    /// <value><c>true</c> to display tabs; otherwise, <c>false</c>.</value>
    public bool ShowTabs
    {
      get { return showTabs; }
      set { showTabs = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether display the end-of-line marker.
    /// </summary>
    /// <value>
    /// 	<c>true</c> to display the end-of-line marker; otherwise, <c>false</c>.
    /// </value>
    public bool ShowEOLMarker
    {
      get { return showEOLMarker; }
      set { showEOLMarker = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to show invalid lines.
    /// </summary>
    /// <value><c>true</c> to show invalid lines; otherwise, <c>false</c>.</value>
    public bool ShowInvalidLines
    {
      get { return showInvalidLines; }
      set { showInvalidLines = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether the icon bar visible.
    /// </summary>
    /// <value><c>true</c> if the icon bar visible; otherwise, <c>false</c>.</value>
    public bool IsIconBarVisible
    {
      get { return isIconBarVisible; }
      set { isIconBarVisible = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to enable folding.
    /// </summary>
    /// <value><c>true</c> if folding is enabled; otherwise, <c>false</c>.</value>
    public bool EnableFolding
    {
      get { return enableFolding; }
      set { enableFolding = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to show horizontal ruler (columns).
    /// </summary>
    /// <value>
    /// 	<c>true</c> to show the horizontal ruler; otherwise, <c>false</c>.
    /// </value>
    public bool ShowHorizontalRuler
    {
      get { return showHorizontalRuler; }
      set { showHorizontalRuler = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to show the vertical ruler (vertical guide).
    /// </summary>
    /// <value><c>true</c> to show the vertical ruler; otherwise, <c>false</c>.</value>
    public bool ShowVerticalRuler
    {
      get { return showVerticalRuler; }
      set { showVerticalRuler = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to convert tabs to spaces.
    /// </summary>
    /// <value><c>true</c> to convert tabs to spaces; otherwise, <c>false</c>.</value>
    public bool ConvertTabsToSpaces
    {
      get { return convertTabsToSpaces; }
      set { convertTabsToSpaces = value; }
    }


    /// <summary>
    /// Gets or sets the <see cref="System.Drawing.Text.TextRenderingHint"/> for rendering the text.
    /// </summary>
    /// <value>The <see cref="System.Drawing.Text.TextRenderingHint"/>.</value>
    public TextRenderingHint TextRenderingHint
    {
      get { return textRenderingHint; }
      set { textRenderingHint = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to scroll the text down or up
    /// when rotating the mouse wheel (default is 'down').
    /// </summary>
    /// <value><c>true</c> to scroll down; <c>false</c> false to scroll up.</value>
    public bool MouseWheelScrollDown
    {
      get { return mouseWheelScrollDown; }
      set { mouseWheelScrollDown = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to zoom the text with the mouse
    /// wheel.
    /// </summary>
    /// <value>
    /// 	<c>true</c> to zoom the text with the mouse wheel; otherwise, <c>false</c>.
    /// </value>
    public bool MouseWheelTextZoom
    {
      get { return mouseWheelTextZoom; }
      set { mouseWheelTextZoom = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to hide the mouse cursor while typing.
    /// </summary>
    /// <value><c>true</c> to hide the mouse cursor; otherwise, <c>false</c>.</value>
    public bool HideMouseCursor
    {
      get { return hideMouseCursor; }
      set { hideMouseCursor = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to cut/copy the whole line when
    /// nothing is selected.
    /// </summary>
    /// <value><c>true</c> to cut/copy the whole line; otherwise, <c>false</c>.</value>
    public bool CutCopyWholeLine
    {
      get { return cutCopyWholeLine; }
      set { cutCopyWholeLine = value; }
    }


    /// <summary>
    /// Gets or sets the encoding.
    /// </summary>
    /// <value>The encoding.</value>
    public Encoding Encoding
    {
      get { return encoding; }
      set { encoding = value; }
    }


    /// <summary>
    /// Gets or sets the column of the vertical ruler.
    /// </summary>
    /// <value>The column of vertical ruler.</value>
    public int VerticalRulerColumn
    {
      get { return verticalRulerRow; }
      set { verticalRulerRow = value; }
    }


    /// <summary>
    /// Gets or sets the style the current line is highlighted.
    /// </summary>
    /// <value>The line viewer style.</value>
    public LineViewerStyle LineViewerStyle
    {
      get { return lineViewerStyle; }
      set { lineViewerStyle = value; }
    }


    /// <summary>
    /// Gets or sets the line terminator.
    /// </summary>
    /// <value>The line terminator.</value>
    public string LineTerminator
    {
      get { return lineTerminator; }
      set { lineTerminator = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to automatically insert curly
    /// brackets.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if automatically inserting curly brackets; otherwise, <c>false</c>.
    /// </value>
    public bool AutoInsertCurlyBracket
    {
      get { return autoInsertCurlyBracket; }
      set { autoInsertCurlyBracket = value; }
    }


    /// <summary>
    /// Gets or sets the font.
    /// </summary>
    /// <value>The font.</value>
    public Font Font
    {
      get { return fontContainer.DefaultFont; }
      set { fontContainer.DefaultFont = value; }
    }


    /// <summary>
    /// Gets the font container.
    /// </summary>
    /// <value>The font container.</value>
    public FontContainer FontContainer
    {
      get { return fontContainer; }
    }


    /// <summary>
    /// Gets or sets the bracket matching style.
    /// </summary>
    /// <value>The bracket matching style.</value>
    public BracketMatchingStyle BracketMatchingStyle
    {
      get { return bracketMatchingStyle; }
      set { bracketMatchingStyle = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to use custom lines.
    /// </summary>
    /// <value><c>true</c> if custom lines are used; otherwise, <c>false</c>.</value>
    public bool UseCustomLine
    {
      get { return useCustomLine; }
      set { useCustomLine = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to show scroll bars.
    /// </summary>
    /// <value><c>true</c> if to show scroll bars; otherwise, <c>false</c>.</value>
    public bool ShowScrollBars
    {
      get { return _showScrollBars; }
      set { _showScrollBars = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to enable code completion.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if code completion is enabled; otherwise, <c>false</c>.
    /// </value>
    public bool EnableCompletion
    {
      get { return _enableCompletion; }
      set { _enableCompletion = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to enable method insight.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if method insight is enabled; otherwise, <c>false</c>.
    /// </value>
    public bool EnableInsight
    {
      get { return _enableMethodInsight; }
      set { _enableMethodInsight = value; }
    }
  }
}
