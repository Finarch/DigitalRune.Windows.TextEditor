using System;
using System.Drawing;

namespace DigitalRune.Windows.TextEditor.Highlighting
{
  /// <summary>
  /// This class is used to generate bold, italic and bold/italic fonts out
  /// of a base font.
  /// </summary>
  public class FontContainer
  {
    Font defaultFont;
    Font regularfont, boldfont, italicfont, bolditalicfont;


    /// <summary>
    /// Gets the regular font.
    /// </summary>
    /// <value>The regular version of the base font.</value>
    public Font RegularFont
    {
      get { return regularfont; }
    }


    /// <summary>
    /// Gets the bold font.
    /// </summary>
    /// <value>The scaled, bold version of the base font</value>
    public Font BoldFont
    {
      get { return boldfont; }
    }


    /// <summary>
    /// Gets the italic font.
    /// </summary>
    /// <value>The scaled, italic version of the base font</value>
    public Font ItalicFont
    {
      get { return italicfont; }
    }


    /// <summary>
    /// Gets the bold/italic font.
    /// </summary>
    /// <value>The scaled, bold/italic version of the base font</value>
    public Font BoldItalicFont
    {
      get { return bolditalicfont; }
    }


    static float twipsPerPixelY;

    /// <summary>
    /// Gets the twips per pixel in y direction.
    /// </summary>
    /// <value>The twips per pixel in y direction.</value>
    public static float TwipsPerPixelY
    {
      get
      {
        if (twipsPerPixelY == 0)
          using (Bitmap bmp = new Bitmap(1, 1))
            using (Graphics g = Graphics.FromImage(bmp))
              twipsPerPixelY = 1440 / g.DpiY;

        return twipsPerPixelY;
      }
    }


    /// <summary>
    /// Gets or sets the default font.
    /// </summary>
    /// <value>The base font.</value>
    public Font DefaultFont
    {
      get { return defaultFont; }
      set
      {
        // 1440 twips is one inch
        int pixelSize = (int) (value.SizeInPoints * 20 / TwipsPerPixelY);

        defaultFont = value;
        regularfont = new Font(value.FontFamily, pixelSize * TwipsPerPixelY / 20f, FontStyle.Regular);
        boldfont = new Font(regularfont, FontStyle.Bold);
        italicfont = new Font(regularfont, FontStyle.Italic);
        bolditalicfont = new Font(regularfont, FontStyle.Bold | FontStyle.Italic);
      }
    }


    /// <summary>
    /// Converts a string to <see cref="Font"/> object.
    /// </summary>
    /// <param name="font">The font.</param>
    /// <returns>The font.</returns>
    public static Font ParseFont(string font)
    {
      string[] descr = font.Split(new char[] { ',', '=' });
      return new Font(descr[1], Single.Parse(descr[3]));
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="FontContainer"/> class.
    /// </summary>
    /// <param name="defaultFont">The default font.</param>
    public FontContainer(Font defaultFont)
    {
      this.DefaultFont = defaultFont;
    }
  }
}
