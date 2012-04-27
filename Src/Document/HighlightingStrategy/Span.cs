using System;
using System.Xml;

namespace DigitalRune.Windows.TextEditor.Highlighting
{
  /// <summary>
  /// Defines a span (for example: strings, line comments, block comments, etc.).
  /// </summary>
  public sealed class Span
  {
    bool stopEOL;
    HighlightColor color;
    HighlightColor beginColor;
    HighlightColor endColor;
    char[] begin;
    char[] end;
    string name;
    string rule;
    HighlightRuleSet ruleSet;
    char escapeCharacter;
    bool ignoreCase;
    bool isBeginSingleWord;
    bool? isBeginStartOfLine;
    bool isEndSingleWord;


    /// <summary>
    /// Gets or sets the rule set.
    /// </summary>
    /// <value>The rule set.</value>
    internal HighlightRuleSet RuleSet
    {
      get { return ruleSet; }
      set { ruleSet = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to ignore the case.
    /// </summary>
    /// <value><c>true</c> to ignore the case; otherwise, <c>false</c>.</value>
    public bool IgnoreCase
    {
      get { return ignoreCase; }
      set { ignoreCase = value; }
    }


    /// <summary>
    /// Gets a value indicating whether stop the span at the end of the line.
    /// </summary>
    /// <value><c>true</c> if the span stops at the end of the line; otherwise, <c>false</c>.</value>
    public bool StopEOL
    {
      get { return stopEOL; }
    }


    /// <summary>
    /// Gets a value indicating whether this span starts with the begin of a line.
    /// </summary>
    /// <value>A value indicating whether this span starts with the begin of a line.</value>
    public bool? IsBeginStartOfLine
    {
      get { return isBeginStartOfLine; }
    }


    /// <summary>
    /// Gets a value indicating whether this span begins with a single word.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this span begins with a single word; otherwise, <c>false</c>.
    /// </value>
    public bool IsBeginSingleWord
    {
      get { return isBeginSingleWord; }
    }


    /// <summary>
    /// Gets a value indicating whether this span ends with a single word.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this span ends with a single word; otherwise, <c>false</c>.
    /// </value>
    public bool IsEndSingleWord
    {
      get { return isEndSingleWord; }
    }


    /// <summary>
    /// Gets the color.
    /// </summary>
    /// <value>The color.</value>
    public HighlightColor Color
    {
      get { return color; }
    }


    /// <summary>
    /// Gets the color of the begin of the span.
    /// </summary>
    /// <value>The color of the begin of the span.</value>
    public HighlightColor BeginColor
    {
      get
      {
        if (beginColor != null)
          return beginColor;
        else
          return color;
      }
    }


    /// <summary>
    /// Gets the end color of the span.
    /// </summary>
    /// <value>The end color of the span.</value>
    public HighlightColor EndColor
    {
      get { return endColor != null ? endColor : color; }
    }


    /// <summary>
    /// Gets the expression that defines the begin of the span.
    /// </summary>
    /// <value>The expression that defines the begin of the span.</value>
    public char[] Begin
    {
      get { return begin; }
    }


    /// <summary>
    /// Gets the expression that defines the end of the span.
    /// </summary>
    /// <value>The expression that defines the end of the span.</value>
    public char[] End
    {
      get { return end; }
    }


    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name
    {
      get { return name; }
    }


    /// <summary>
    /// Gets the rule applied to this span.
    /// </summary>
    /// <value>The rule applied to this span.</value>
    public string Rule
    {
      get { return rule; }
    }


    /// <summary>
    /// Gets the escape character of the span. The escape character is a character that can be used in front
    /// of the span end to make it not end the span. The escape character followed by another escape character
    /// means the escape character was escaped like in @"a "" b" literals in C#.
    /// The default value '\0' means no escape character is allowed.
    /// </summary>
    public char EscapeCharacter
    {
      get { return escapeCharacter; }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="Span"/> class.
    /// </summary>
    /// <param name="span">The XML element that describes the span.</param>
    public Span(XmlElement span)
    {
      color = new HighlightColor(span);

      if (span.HasAttribute("rule"))
        rule = span.GetAttribute("rule");

      if (span.HasAttribute("escapecharacter"))
        escapeCharacter = span.GetAttribute("escapecharacter")[0];

      name = span.GetAttribute("name");
      if (span.HasAttribute("stopateol"))
        stopEOL = Boolean.Parse(span.GetAttribute("stopateol"));

      begin = span["Begin"].InnerText.ToCharArray();
      beginColor = new HighlightColor(span["Begin"], color);

      if (span["Begin"].HasAttribute("singleword"))
        this.isBeginSingleWord = Boolean.Parse(span["Begin"].GetAttribute("singleword"));

      if (span["Begin"].HasAttribute("startofline"))
        this.isBeginStartOfLine = Boolean.Parse(span["Begin"].GetAttribute("startofline"));

      if (span["End"] != null)
      {
        end = span["End"].InnerText.ToCharArray();
        endColor = new HighlightColor(span["End"], color);
        if (span["End"].HasAttribute("singleword"))
          this.isEndSingleWord = Boolean.Parse(span["End"].GetAttribute("singleword"));
      }
    }
  }
}
