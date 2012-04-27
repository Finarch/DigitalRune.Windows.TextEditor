using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace DigitalRune.Windows.TextEditor.Highlighting
{
  /// <summary>
  /// Provides that syntax highlighting defintions which are stored as resources
  /// in this assembly.
  /// </summary>
  public class ResourceSyntaxModeProvider : ISyntaxModeFileProvider
  {
    List<SyntaxMode> syntaxModes = null;


    /// <summary>
    /// Gets the provided syntax highlighting modes.
    /// </summary>
    /// <value>The syntax highlighting modes.</value>
    public ICollection<SyntaxMode> SyntaxModes
    {
      get { return syntaxModes; }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceSyntaxModeProvider"/> class.
    /// </summary>
    public ResourceSyntaxModeProvider()
    {
      Assembly assembly = typeof(SyntaxMode).Assembly;
      Stream syntaxModeStream = assembly.GetManifestResourceStream("DigitalRune.Windows.TextEditor.Resources.SyntaxModes.xml");
      if (syntaxModeStream != null)
        syntaxModes = SyntaxMode.GetSyntaxModes(syntaxModeStream);
      else
        syntaxModes = new List<SyntaxMode>();
    }


    /// <summary>
    /// Gets the syntax highlighting definition for a certain syntax.
    /// </summary>
    /// <param name="syntaxMode">The syntax.</param>
    /// <returns>The syntax highlighting definition.</returns>
    public XmlTextReader GetSyntaxModeFile(SyntaxMode syntaxMode)
    {
      Assembly assembly = typeof(SyntaxMode).Assembly;
      return new XmlTextReader(assembly.GetManifestResourceStream("DigitalRune.Windows.TextEditor.Resources." + syntaxMode.FileName));
    }


    /// <summary>
    /// Updates the list of syntax highlighting modes.
    /// </summary>
    /// <remarks>
    /// Has no effect in this case, because the resources cannot change during
    /// runtime.
    /// </remarks>
    public void UpdateSyntaxModeList()
    {
      // resources don't change during runtime
    }
  }
}
