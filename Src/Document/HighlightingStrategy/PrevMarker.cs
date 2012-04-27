using System;
using System.Xml;

namespace DigitalRune.Windows.TextEditor.Highlighting
{
  /// <summary>
  /// Used to mark previous token.
  /// </summary>
  public class PrevMarker
  {
    string what;
    HighlightColor color;
    bool markMarker = false;


    /// <summary>
    /// Gets the string that indicates that the previous token should be marked.
    /// </summary>
    /// <value>The string that indicates that the previous token should be marked.</value>
    public string What
    {
      get { return what; }
    }


    /// <summary>
    /// Gets the highlighting color for the previous token.
    /// </summary>
    /// <value>Color for marking previous token.</value>
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
    /// Creates a new instance of <see cref="PrevMarker"/>
    /// </summary>
    /// <param name="mark">The XML element that defines this <see cref="PrevMarker"/>.</param>
    public PrevMarker(XmlElement mark)
    {
      color = new HighlightColor(mark);
      what = mark.InnerText;
      if (mark.Attributes["markmarker"] != null)
        markMarker = Boolean.Parse(mark.Attributes["markmarker"].InnerText);
    }
  }

}
