using System;
using System.Drawing;
using DigitalRune.Windows.TextEditor.Document;

namespace DigitalRune.Windows.TextEditor.Markers
{
  /// <summary>
  /// Defines the type of a <see cref="TextMarker"/>.
  /// </summary>
  public enum TextMarkerType
  {
    /// <summary>
    /// A solid colored block in the background of the text.
    /// </summary>
    SolidBlock,
    /// <summary>
    /// An underline.
    /// </summary>
    Underlined,
    /// <summary>
    /// A zigzag line below the text.
    /// </summary>
    WaveLine
  }


  /// <summary>
  /// Marks a part of a document.
  /// </summary>
  public class TextMarker : ISegment
  {
    private int offset = -1;
    private int length = -1;
    TextMarkerType textMarkerType;
    Color color;
    Color foreColor;
    string toolTip = null;
    bool overrideForeColor = false;


    /// <summary>
    /// Gets or sets the offset.
    /// </summary>
    /// <value>The offset where the span begins</value>
    public int Offset
    {
      get { return offset; }
      set { offset = value; }
    }


    /// <summary>
    /// Gets or sets the length.
    /// </summary>
    /// <value>The length of the span</value>
    public int Length
    {
      get { return length; }
      set { length = value; }
    }


    /// <summary>
    /// Gets the type of the text marker.
    /// </summary>
    /// <value>The type of the text marker.</value>
    public TextMarkerType TextMarkerType
    {
      get { return textMarkerType; }
    }


    /// <summary>
    /// Gets the color.
    /// </summary>
    /// <value>The color.</value>
    public Color Color
    {
      get { return color; }
    }


    /// <summary>
    /// Gets the forecolor of the text.
    /// </summary>
    /// <value>The forecolor of the text.</value>
    /// <remarks>
    /// Only relevant when <see cref="TextMarkerType"/> is <c>SolidBlock</c>.
    /// </remarks>
    public Color ForeColor
    {
      get { return foreColor; }
    }


    /// <summary>
    /// Gets a value indicating whether to override the forecolor of the text.
    /// </summary>
    /// <value>
    /// <c>true</c> if the forecolor of the text shall be overriden; otherwise, 
    /// <c>false</c>.
    /// </value>
    /// <remarks>
    /// Only relevant when <see cref="TextMarkerType"/> is <c>SolidBlock</c>.
    /// </remarks>
    public bool OverrideForeColor
    {
      get { return overrideForeColor; }
    }


    /// <summary>
    /// Gets or sets the tool tip of the text marker.
    /// </summary>
    /// <value>The tool tip.</value>
    public string ToolTip
    {
      get { return toolTip; }
      set { toolTip = value; }
    }


    /// <summary>
    /// Gets the last offset that is inside the marker region.
    /// </summary>
    /// <value>The end offset.</value>
    public int EndOffset
    {
      get { return Offset + Length - 1; }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="TextMarker"/> class.
    /// </summary>
    /// <param name="offset">The offset of the marked region.</param>
    /// <param name="length">The length of the marked region.</param>
    /// <param name="textMarkerType">Type of the text marker.</param>
    public TextMarker(int offset, int length, TextMarkerType textMarkerType)
      : this(offset, length, textMarkerType, Color.Red)
    {
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="TextMarker"/> class.
    /// </summary>
    /// <param name="offset">The offset of the marked region.</param>
    /// <param name="length">The length of the marked region.</param>
    /// <param name="textMarkerType">Type of the text marker.</param>
    /// <param name="color">The color of the text marker.</param>
    public TextMarker(int offset, int length, TextMarkerType textMarkerType, Color color)
    {
      if (length < 1) length = 1;
      this.offset = offset;
      this.length = length;
      this.textMarkerType = textMarkerType;
      this.color = color;
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="TextMarker"/> class.
    /// </summary>
    /// <param name="offset">The offset of the marked region.</param>
    /// <param name="length">The length of the marked region.</param>
    /// <param name="textMarkerType">Type of the text marker.</param>
    /// <param name="color">The color of the text marker.</param>
    /// <param name="foreColor">The foreground color of the text.</param>
    public TextMarker(int offset, int length, TextMarkerType textMarkerType, Color color, Color foreColor)
    {
      if (length < 1) length = 1;
      this.offset = offset;
      this.length = length;
      this.textMarkerType = textMarkerType;
      this.color = color;
      this.foreColor = foreColor;
      this.overrideForeColor = true;
    }


    /// <summary>
    /// Returns a <see cref="String"></see> that represents the current <see cref="Object"></see>.
    /// </summary>
    /// <returns>
    /// A <see cref="String"></see> that represents the current <see cref="Object"></see>.
    /// </returns>
    public override string ToString()
    {
      return String.Format("[TextMarker: Offset = {0}, Length = {1}]", Offset, Length);
    }
  }
}
