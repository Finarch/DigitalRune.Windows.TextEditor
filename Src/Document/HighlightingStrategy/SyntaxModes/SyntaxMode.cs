using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace DigitalRune.Windows.TextEditor.Highlighting
{
  /// <summary>
  /// Describes a syntax highlighting mode.
  /// </summary>
  public class SyntaxMode
  {
    string fileName;
    string name;
    string[] extensions;


    /// <summary>
    /// Gets or sets the file name of the syntax highlighting definition file.
    /// </summary>
    /// <value>The file name of the syntax highlighting definition file.</value>
    public string FileName
    {
      get { return fileName; }
      set { fileName = value; }
    }


    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name
    {
      get { return name; }
      set { name = value; }
    }


    /// <summary>
    /// Gets or sets the file extensions of files of files that have this syntax.
    /// </summary>
    /// <value>The file extensions.</value>
    /// <remarks>
    /// For example: C++ file extension are: ".c;.h;.cc;.C;.cpp;.hpp"
    /// </remarks>
    public string[] Extensions
    {
      get { return extensions; }
      set { extensions = value; }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="SyntaxMode"/> class.
    /// </summary>
    /// <param name="fileName">Name of the syntax highlighting definition file.</param>
    /// <param name="name">The name of the syntax.</param>
    /// <param name="extensions">
    /// The file extensions (extensions need to be seperated by ';', '|' or ','). 
    /// For example: <c>".htm;.html"</c>
    /// </param>
    public SyntaxMode(string fileName, string name, string extensions)
    {
      this.fileName = fileName;
      this.name = name;
      this.extensions = extensions.Split(';', '|', ',');
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="SyntaxMode"/> class.
    /// </summary>
    /// <param name="fileName">Name of the syntax highlighting definition file.</param>
    /// <param name="name">The name of the syntax.</param>
    /// <param name="extensions">The file extensions (for example: <c>".html"</c>, <c>".htm"</c>, etc.)</param>
    public SyntaxMode(string fileName, string name, string[] extensions)
    {
      this.fileName = fileName;
      this.name = name;
      this.extensions = extensions;
    }


    /// <summary>
    /// Gets a list of provided syntax modes.
    /// </summary>
    /// <param name="xmlSyntaxModeStream">The XML syntax mode stream.</param>
    /// <returns>A list of provided syntax modes.</returns>
    /// <remarks>
    /// Have a look at <c>Resources/SyntaxModes.xml</c> in this project.
    /// </remarks>
    public static List<SyntaxMode> GetSyntaxModes(Stream xmlSyntaxModeStream)
    {
      XmlTextReader reader = new XmlTextReader(xmlSyntaxModeStream);
      List<SyntaxMode> syntaxModes = new List<SyntaxMode>();
      while (reader.Read())
      {
        switch (reader.NodeType)
        {
          case XmlNodeType.Element:
            switch (reader.Name)
            {
              case "SyntaxModes":
                string version = reader.GetAttribute("version");
                if (version != "1.0")
                {
                  throw new HighlightingDefinitionInvalidException("Unknown syntax mode file defininition with version " + version);
                }
                break;
              case "Mode":
                syntaxModes.Add(new SyntaxMode(reader.GetAttribute("file"), reader.GetAttribute("name"), reader.GetAttribute("extensions")));
                break;
              default:
                throw new HighlightingDefinitionInvalidException("Unknown node in syntax mode file :" + reader.Name);
            }
            break;
        }
      }
      reader.Close();
      return syntaxModes;
    }


    /// <summary>
    /// Returns a <see cref="String"></see> that represents the current <see cref="Object"></see>.
    /// </summary>
    /// <returns>
    /// A <see cref="String"></see> that represents the current <see cref="Object"></see>.
    /// </returns>
    public override string ToString()
    {
      return String.Format("[SyntaxMode: FileName={0}, Name={1}, Extensions=({2})]", fileName, name, String.Join(",", extensions));
    }
  }
}
