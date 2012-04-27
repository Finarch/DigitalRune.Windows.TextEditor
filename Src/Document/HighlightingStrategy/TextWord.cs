using System;
using System.Diagnostics;
using System.Drawing;
using DigitalRune.Windows.TextEditor.Document;

namespace DigitalRune.Windows.TextEditor.Highlighting
{
  /// <summary>
  /// Types of words in a line.
  /// </summary>
  public enum TextWordType
  {
    /// <summary>A word.</summary>
    Word,
    /// <summary>A space.</summary>
    Space,
    /// <summary>A tab.</summary>
    Tab
  }


  /// <summary>
  /// This class represents single words with color information, two special versions of a word are
  /// spaces and tabs.
  /// </summary>
  public class TextWord
  {
    HighlightColor color;
    LineSegment line;
    private readonly Span _span;
    IDocument document;
    int offset;
    int length;

    /// <summary>
    /// A space (special type of <see cref="TextWord"/>).
    /// </summary>
    public sealed class SpaceTextWord : TextWord
    {
      /// <summary>
      /// Initializes a new instance of the <see cref="TextWord.SpaceTextWord"/> class.
      /// </summary>
      public SpaceTextWord()
      {
        length = 1;
      }


      /// <summary>
      /// Initializes a new instance of the <see cref="TextWord.SpaceTextWord"/> class.
      /// </summary>
      /// <param name="color">The color.</param>
      public SpaceTextWord(HighlightColor color)
      {
        length = 1;
        base.SyntaxColor = color;
      }


      /// <summary>
      /// Gets the font.
      /// </summary>
      /// <param name="fontContainer">The font container.</param>
      /// <returns>Always <c>null</c>.</returns>
      public override Font GetFont(FontContainer fontContainer)
      {
        return null;
      }


      /// <summary>
      /// Gets the type.
      /// </summary>
      /// <value>The type (<see cref="TextWordType.Space"/>).</value>
      public override TextWordType Type
      {
        get { return TextWordType.Space; }
      }


      /// <summary>
      /// Gets a value indicating whether this instance is a whitespace.
      /// </summary>
      /// <value>
      /// 	<c>true</c>.
      /// </value>
      public override bool IsWhiteSpace
      {
        get { return true; }
      }
    }


    /// <summary>
    /// A tab (special type of <see cref="TextWord"/>).
    /// </summary>
    public sealed class TabTextWord : TextWord
    {
      /// <summary>
      /// Initializes a new instance of the <see cref="TextWord.TabTextWord"/> class.
      /// </summary>
      public TabTextWord()
      {
        length = 1;
      }


      /// <summary>
      /// Initializes a new instance of the <see cref="TextWord.TabTextWord"/> class.
      /// </summary>
      /// <param name="color">The color.</param>
      public TabTextWord(HighlightColor color)
      {
        length = 1;
        base.SyntaxColor = color;
      }

      /// <summary>
      /// Gets the font.
      /// </summary>
      /// <param name="fontContainer">The font container.</param>
      /// <returns>Always <c>null</c>.</returns>
      public override Font GetFont(FontContainer fontContainer)
      {
        return null;
      }


      /// <summary>
      /// Gets the type.
      /// </summary>
      /// <value>The type (<see cref="TextWordType.Tab"/>).</value>
      public override TextWordType Type
      {
        get { return TextWordType.Tab; }
      }


      /// <summary>
      /// Gets a value indicating whether this instance is a whitespace.
      /// </summary>
      /// <value>
      /// 	<c>true</c>.
      /// </value>
      public override bool IsWhiteSpace
      {
        get { return true; }
      }
    }


    static TextWord spaceWord = new SpaceTextWord();
    static TextWord tabWord = new TabTextWord();
    bool hasDefaultColor;


    /// <summary>
    /// Gets a space (special type of <see cref="TextWord"/>).
    /// </summary>
    /// <value>The space.</value>
    public static TextWord Space
    {
      get { return spaceWord; }
    }


    /// <summary>
    /// Gets a tab (special type of <see cref="TextWord"/>).
    /// </summary>
    /// <value>The tab.</value>
    public static TextWord Tab
    {
      get { return tabWord; }
    }


    /// <summary>
    /// Gets the offset of the word in the text buffer.
    /// </summary>
    /// <value>The offset of the word in the text buffer.</value>
    public int Offset
    {
      get { return offset; }
    }


    /// <summary>
    /// Gets the length of the word.
    /// </summary>
    /// <value>The length of the word.</value>
    public int Length
    {
      get { return length; }
    }


    /// <summary>
    /// Gets the <see cref="Span"/>.
    /// </summary>
    /// <value>The <see cref="Span"/>.</value>
    public Span Span
    {
      get { return _span; }
    }

    /// <summary>
    /// Splits the word into two parts. 
    /// </summary>
    /// <param name="word">The word.</param>
    /// <param name="pos">The position, which lies in the range <c>[1, Length - 1]</c>.</param>
    /// <returns>The part after <paramref name="pos"/> is returned as a new <see cref="TextWord"/>.</returns>
    /// <remarks>
    /// The part before <paramref name="pos"/> is assigned to
    /// the reference parameter <paramref name="word"/>, the part after <paramref name="pos"/> is returned.
    /// </remarks>
    public static TextWord Split(ref TextWord word, int pos)
    {
#if DEBUG
      if (word.Type != TextWordType.Word)
        throw new ArgumentException("word.Type must be Word");

      if (pos <= 0)
        throw new ArgumentOutOfRangeException("pos", pos, "pos must be > 0");

      if (pos >= word.Length)
        throw new ArgumentOutOfRangeException("pos", pos, "pos must be < word.Length");
#endif
      TextWord after = new TextWord(word.document, word.Span, word.line, word.offset + pos, word.length - pos, word.color, word.hasDefaultColor);
      word = new TextWord(word.document, word.Span, word.line, word.offset, pos, word.color, word.hasDefaultColor);
      return after;
    }


    /// <summary>
    /// Gets a value indicating whether this instance has default color.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance has default color; otherwise, <c>false</c>.
    /// </value>
    public bool HasDefaultColor
    {
      get { return hasDefaultColor; }
    }


    /// <summary>
    /// Gets the type.
    /// </summary>
    /// <value>The type.</value>
    public virtual TextWordType Type
    {
      get { return TextWordType.Word; }
    }


    /// <summary>
    /// Gets the word.
    /// </summary>
    /// <value>The word.</value>
    public string Word
    {
      get
      {
        if (document == null)
          return String.Empty;

        return document.GetText(line.Offset + offset, length);
      }
    }


    /// <summary>
    /// Gets the font.
    /// </summary>
    /// <param name="fontContainer">The font container.</param>
    /// <returns>The font.</returns>
    public virtual Font GetFont(FontContainer fontContainer)
    {
      return color.GetFont(fontContainer);
    }


    /// <summary>
    /// Gets the color.
    /// </summary>
    /// <value>The color.</value>
    public Color Color
    {
      get
      {
        if (color == null)
          return Color.Black;
        else
          return color.Color;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="TextWord"/> is bold.
    /// </summary>
    /// <value><c>true</c> if bold; otherwise, <c>false</c>.</value>
    public bool Bold
    {
      get
      {
        if (color == null)
          return false;
        else
          return color.Bold;
      }
    }


    /// <summary>
    /// Gets a value indicating whether this <see cref="TextWord"/> is italic.
    /// </summary>
    /// <value><c>true</c> if italic; otherwise, <c>false</c>.</value>
    public bool Italic
    {
      get
      {
        if (color == null)
          return false;
        else
          return color.Italic;
      }
    }


    /// <summary>
    /// Gets or sets the <see cref="HighlightColor"/>.
    /// </summary>
    /// <value>The color for the syntax highlighting.</value>
    public HighlightColor SyntaxColor
    {
      get { return color; }
      set
      {
        Debug.Assert(value != null);
        color = value;
      }
    }


    /// <summary>
    /// Gets a value indicating whether this instance is a whitespace.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is whitespace; otherwise, <c>false</c>.
    /// </value>
    public virtual bool IsWhiteSpace
    {
      get { return false; }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="TextWord"/> class.
    /// </summary>
    protected TextWord()
    {
      // Needed by the nested classes SpaceTextWord and TabTextWord.
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="TextWord"/> class.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="span">The <see cref="Span"/>.</param>
    /// <param name="line">The line.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="length">The length.</param>
    /// <param name="color">The color.</param>
    /// <param name="hasDefaultColor">if set to <c>true</c> [has default color].</param>
    public TextWord(IDocument document, Span span, LineSegment line, int offset, int length, HighlightColor color, bool hasDefaultColor)
    {
      Debug.Assert(document != null);
      Debug.Assert(line != null);
      Debug.Assert(color != null);

      this.document = document;
      _span = span;
      this.line = line;
      this.offset = offset;
      this.length = length;
      this.color = color;
      this.hasDefaultColor = hasDefaultColor;
    }


    /// <summary>
    /// Converts a <see cref="TextWord"/> instance to string (for debug purposes)
    /// </summary>
    /// <returns>
    /// A <see cref="String"></see> that represents the current <see cref="Object"></see>.
    /// </returns>
    public override string ToString()
    {
      return "[TextWord: Word = " + Word + ", Color = " + Color + "]";
    }
  }
}
