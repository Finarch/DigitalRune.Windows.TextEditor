using System;
using System.Xml;

namespace DigitalRune.Windows.TextEditor.Highlighting
{
  /// <summary>
  /// Used to mark next token.
  /// </summary>
  public class NextMarker
  {
    string what;
    HighlightColor color;
    bool markMarker = false;


    /// <summary>
    /// Gets the string that indicates that the next token should be marked.
    /// </summary>
    /// <value>The string that indicates that the next token should be marked.</value>
    public string What
    {
      get { return what; }
    }


    /// <summary>
    /// Gets the highlighting color for the next token.
    /// </summary>
    /// <value>Color for marking next token.</value>
    public HighlightColor Color
    {
      get { return color; }
    }


    /// <summary>
    /// Gets a value indicating whether indication text (<see cref="What"/>) should 
    /// marked with the same color.
    /// </summary>
    /// <value>
    /// <c>true</c> if the indication text (<see cref="What"/>) should 
    /// marked with the same color.
    /// </value>
    public bool MarkMarker
    {
      get { return markMarker; }
    }


    /// <summary>
    /// Creates a new instance of <see cref="NextMarker"/>
    /// </summary>
    /// <param name="mark">The XML element that defines this <see cref="NextMarker"/>.</param>
    public NextMarker(XmlElement mark)
    {
      color = new HighlightColor(mark);
      what = mark.InnerText;
      if (mark.Attributes["markmarker"] != null)
        markMarker = Boolean.Parse(mark.Attributes["markmarker"].InnerText);
    }
  }

}
