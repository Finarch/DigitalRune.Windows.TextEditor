using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Xml;

namespace DigitalRune.Windows.TextEditor.Highlighting
{
  /// <summary>
  /// A color/style used for highlighting.
  /// </summary>
  public class HighlightColor
  {
    bool systemColor = false;
    string systemColorName = null;

    bool systemBgColor = false;
    string systemBgColorName = null;

    Color color;
    Color backgroundcolor = System.Drawing.Color.WhiteSmoke;

    bool bold = false;
    bool italic = false;
    bool hasForeground = false;
    bool hasBackground = false;


    /// <summary>
    /// Gets a value indicating whether <see cref="HighlightColor"/> has foreground color.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this <see cref="HighlightColor"/> has foreground color; otherwise, <c>false</c>.
    /// </value>
    public bool HasForeground
    {
      get { return hasForeground; }
    }


    /// <summary>
    /// Gets a value indicating whether <see cref="HighlightColor"/> has background color.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this <see cref="HighlightColor"/> has background color; otherwise, <c>false</c>.
    /// </value>
    public bool HasBackground
    {
      get { return hasBackground; }
    }


    /// <summary>
    /// Gets a value indicating whether the font will be displayed bold style.
    /// </summary>
    /// <value>If <c>true</c> the font will be displayed bold style.</value>
    public bool Bold
    {
      get { return bold; }
    }


    /// <summary>
    /// Gets a value indicating whether the font will be displayed italic style.
    /// </summary>
    /// <value>If <c>true</c> the font will be displayed italic style.</value>
    public bool Italic
    {
      get { return italic; }
    }


    /// <summary>
    /// Gets the background color.
    /// </summary>
    /// <value>The background color used</value>
    public Color BackgroundColor
    {
      get
      {
        if (!systemBgColor)
          return backgroundcolor;

        return ParseColorString(systemBgColorName);
      }
    }


    /// <summary>
    /// Gets foreground color.
    /// </summary>
    /// <value>The foreground color used.</value>
    public Color Color
    {
      get
      {
        if (!systemColor)
          return color;

        return ParseColorString(systemColorName);
      }
    }


    /// <summary>
    /// Gets the font.
    /// </summary>
    /// <param name="fontContainer">The font container.</param>
    /// <returns>The font.</returns>
    public Font GetFont(FontContainer fontContainer)
    {
      if (Bold)
        return Italic ? fontContainer.BoldItalicFont : fontContainer.BoldFont;

      return Italic ? fontContainer.ItalicFont : fontContainer.RegularFont;
    }


    Color ParseColorString(string colorName)
    {
      string[] cNames = colorName.Split('*');
      PropertyInfo myPropInfo = typeof(System.Drawing.SystemColors).GetProperty(cNames[0], BindingFlags.Public |
                                                                                BindingFlags.Instance |
                                                                                BindingFlags.Static);
      Color c = (Color) myPropInfo.GetValue(null, null);

      if (cNames.Length == 2)
      {
        // hack : can't figure out how to parse doubles with '.' (culture info might set the '.' to ',')
        double factor = Double.Parse(cNames[1]) / 100;
        c = Color.FromArgb((int) ((double) c.R * factor), (int) ((double) c.G * factor), (int) ((double) c.B * factor));
      }

      return c;
    }

    /// <summary>
    /// Creates a new instance of <see cref="HighlightColor"/>
    /// </summary>
    /// <param name="el">The XML element that describes the highlighting color.</param>
    public HighlightColor(XmlElement el)
    {
      Debug.Assert(el != null, "DigitalRune.Windows.TextEditor.Document.SyntaxColor(XmlElement el) : el == null");
      if (el.Attributes["bold"] != null)
        bold = Boolean.Parse(el.Attributes["bold"].InnerText);

      if (el.Attributes["italic"] != null)
        italic = Boolean.Parse(el.Attributes["italic"].InnerText);

      if (el.Attributes["color"] != null)
      {
        string c = el.Attributes["color"].InnerText;
        if (c[0] == '#')
        {
          color = ParseColor(c);
        }
        else if (c.StartsWith("SystemColors."))
        {
          systemColor = true;
          systemColorName = c.Substring("SystemColors.".Length);
        }
        else
        {
          color = (Color) (Color.GetType()).InvokeMember(c, BindingFlags.GetProperty, null, Color, new object[0]);
        }
        hasForeground = true;
      }
      else
      {
        color = Color.Transparent; // to set it to the default value.
      }

      if (el.Attributes["bgcolor"] != null)
      {
        string c = el.Attributes["bgcolor"].InnerText;
        if (c[0] == '#')
        {
          backgroundcolor = ParseColor(c);
        }
        else if (c.StartsWith("SystemColors."))
        {
          systemBgColor = true;
          systemBgColorName = c.Substring("SystemColors.".Length);
        }
        else
        {
          backgroundcolor = (Color) (Color.GetType()).InvokeMember(c, BindingFlags.GetProperty, null, Color, new object[0]);
        }
        hasBackground = true;
      }
    }


    /// <summary>
    /// Creates a new instance of <see cref="HighlightColor"/>
    /// </summary>
    /// <param name="el">The XML element that describes the highlighting color.</param>
    /// <param name="defaultColor">The default highlighting color.</param>
    /// <remarks>
    /// Attributes that are not explicitly set in the XML element will be inherited
    /// from <paramref name="defaultColor"/>.
    /// </remarks>
    public HighlightColor(XmlElement el, HighlightColor defaultColor)
    {
      Debug.Assert(el != null, "DigitalRune.Windows.TextEditor.Document.SyntaxColor(XmlElement el) : el == null");
      if (el.Attributes["bold"] != null)
        bold = Boolean.Parse(el.Attributes["bold"].InnerText);
      else
        bold = defaultColor.Bold;

      if (el.Attributes["italic"] != null)
        italic = Boolean.Parse(el.Attributes["italic"].InnerText);
      else
        italic = defaultColor.Italic;

      if (el.Attributes["color"] != null)
      {
        string c = el.Attributes["color"].InnerText;
        if (c[0] == '#')
        {
          color = ParseColor(c);
        }
        else if (c.StartsWith("SystemColors."))
        {
          systemColor = true;
          systemColorName = c.Substring("SystemColors.".Length);
        }
        else
        {
          color = (Color) (Color.GetType()).InvokeMember(c, BindingFlags.GetProperty, null, Color, new object[0]);
        }
        hasForeground = true;
      }
      else
      {
        color = defaultColor.color;
      }

      if (el.Attributes["bgcolor"] != null)
      {
        string c = el.Attributes["bgcolor"].InnerText;
        if (c[0] == '#')
        {
          backgroundcolor = ParseColor(c);
        }
        else if (c.StartsWith("SystemColors."))
        {
          systemBgColor = true;
          systemBgColorName = c.Substring("SystemColors.".Length);
        }
        else
        {
          backgroundcolor = (Color) (Color.GetType()).InvokeMember(c, BindingFlags.GetProperty, null, Color, new object[0]);
        }
        hasBackground = true;
      }
      else
      {
        backgroundcolor = defaultColor.BackgroundColor;
      }
    }


    /// <summary>
    /// Creates a new instance of <see cref="HighlightColor"/>
    /// </summary>
    /// <param name="color">The foreground color.</param>
    /// <param name="bold">If set to <c>true</c> use bold style.</param>
    /// <param name="italic">If set to <c>true</c> use italic style.</param>
    public HighlightColor(Color color, bool bold, bool italic)
    {
      hasForeground = true;
      this.color = color;
      this.bold = bold;
      this.italic = italic;
    }


    /// <summary>
    /// Creates a new instance of <see cref="HighlightColor"/>
    /// </summary>
    /// <param name="color">The foreground color.</param>
    /// <param name="backgroundColor">The background color.</param>
    /// <param name="bold">If set to <c>true</c> use bold style.</param>
    /// <param name="italic">If set to <c>true</c> use italic style.</param>
    public HighlightColor(Color color, Color backgroundColor, bool bold, bool italic)
    {
      hasForeground = true;
      hasBackground = true;
      this.color = color;
      this.backgroundcolor = backgroundColor;
      this.bold = bold;
      this.italic = italic;
    }


    /// <summary>
    /// Creates a new instance of <see cref="HighlightColor"/>
    /// </summary>
    /// <param name="systemColor">The foreground color (name of a color in <see cref="System.Drawing.SystemColors"/>).</param>
    /// <param name="systemBackgroundColor">The background color (name of a color in <see cref="System.Drawing.SystemColors"/>).</param>
    /// <param name="bold">If set to <c>true</c> use bold style.</param>
    /// <param name="italic">If set to <c>true</c> use italic style.</param>
    public HighlightColor(string systemColor, string systemBackgroundColor, bool bold, bool italic)
    {
      hasForeground = true;
      hasBackground = true;

      this.systemColor = true;
      systemColorName = systemColor;

      systemBgColor = true;
      systemBgColorName = systemBackgroundColor;

      this.bold = bold;
      this.italic = italic;
    }


    static Color ParseColor(string c)
    {
      int a = 255;
      int offset = 0;
      if (c.Length > 7)
      {
        offset = 2;
        a = Int32.Parse(c.Substring(1, 2), NumberStyles.HexNumber);
      }

      int r = Int32.Parse(c.Substring(1 + offset, 2), NumberStyles.HexNumber);
      int g = Int32.Parse(c.Substring(3 + offset, 2), NumberStyles.HexNumber);
      int b = Int32.Parse(c.Substring(5 + offset, 2), NumberStyles.HexNumber);
      return Color.FromArgb(a, r, g, b);
    }


    /// <summary>
    /// Converts a <see cref="HighlightColor"/> instance to string (for debug purposes)
    /// </summary>
    public override string ToString()
    {
      return "[HighlightColor: Bold = " + Bold +
        ", Italic = " + Italic +
        ", Color = " + Color +
        ", BackgroundColor = " + BackgroundColor + "]";
    }
  }
}
