using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace DigitalRune.Windows.TextEditor.Highlighting
{
  /// <summary>
  /// Manages syntax highlighting.
  /// </summary>
  public class HighlightingManager
  {
    List<ISyntaxModeFileProvider> syntaxModeFileProviders = new List<ISyntaxModeFileProvider>();
    static HighlightingManager highlightingManager;

    Hashtable highlightingDefs = new Hashtable();
    Hashtable extensionsToName = new Hashtable();

    /// <summary>
    /// Gets the syntax highlighting definitions.
    /// </summary>
    /// <value>The syntax highlighting definitions.</value>
    /// <remarks>
    /// This is a hash table from extension name to highlighting definition, 
    /// OR from extension name to pair SyntaxMode,ISyntaxModeFileProvider
    /// </remarks>
    public Hashtable HighlightingDefinitions
    {
      get { return highlightingDefs; }
    }


    /// <summary>
    /// Gets the manager (singleton).
    /// </summary>
    /// <value>The manager.</value>
    public static HighlightingManager Manager
    {
      get { return highlightingManager; }
    }


    static HighlightingManager()
    {
      highlightingManager = new HighlightingManager();
      highlightingManager.AddSyntaxModeFileProvider(new ResourceSyntaxModeProvider());
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="HighlightingManager"/> class.
    /// </summary>
    public HighlightingManager()
    {
      CreateDefaultHighlightingStrategy();
    }


    /// <summary>
    /// Adds the syntax mode provider.
    /// </summary>
    /// <param name="syntaxModeFileProvider">The syntax mode file provider.</param>
    public void AddSyntaxModeFileProvider(ISyntaxModeFileProvider syntaxModeFileProvider)
    {
      foreach (SyntaxMode syntaxMode in syntaxModeFileProvider.SyntaxModes)
      {
        highlightingDefs[syntaxMode.Name] = new DictionaryEntry(syntaxMode, syntaxModeFileProvider);
        foreach (string extension in syntaxMode.Extensions)
          extensionsToName[extension.ToUpperInvariant()] = syntaxMode.Name;
      }

      if (!syntaxModeFileProviders.Contains(syntaxModeFileProvider))
        syntaxModeFileProviders.Add(syntaxModeFileProvider);
    }


    /// <summary>
    /// Adds the highlighting strategy.
    /// </summary>
    /// <param name="highlightingStrategy">The highlighting strategy.</param>
    public void AddHighlightingStrategy(IHighlightingStrategy highlightingStrategy)
    {
      highlightingDefs[highlightingStrategy.Name] = highlightingStrategy;
      foreach (string extension in highlightingStrategy.Extensions)
      {
        extensionsToName[extension.ToUpperInvariant()] = highlightingStrategy.Name;
      }
    }


    /// <summary>
    /// Reloads the syntax modes.
    /// </summary>
    public void ReloadSyntaxModes()
    {
      highlightingDefs.Clear();
      extensionsToName.Clear();
      CreateDefaultHighlightingStrategy();
      foreach (ISyntaxModeFileProvider provider in syntaxModeFileProviders)
      {
        provider.UpdateSyntaxModeList();
        AddSyntaxModeFileProvider(provider);
      }
      OnReloadSyntaxHighlighting(EventArgs.Empty);
    }


    void CreateDefaultHighlightingStrategy()
    {
      DefaultHighlightingStrategy defaultHighlightingStrategy = new DefaultHighlightingStrategy();
      defaultHighlightingStrategy.Extensions = new string[] { };
      defaultHighlightingStrategy.Rules.Add(new HighlightRuleSet());
      highlightingDefs["Default"] = defaultHighlightingStrategy;
    }


    IHighlightingStrategy LoadDefinition(DictionaryEntry entry)
    {
      SyntaxMode syntaxMode = (SyntaxMode) entry.Key;
      ISyntaxModeFileProvider syntaxModeFileProvider = (ISyntaxModeFileProvider) entry.Value;

			DefaultHighlightingStrategy highlightingStrategy = null;
      try
      {
        highlightingStrategy = HighlightingDefinitionParser.Parse(syntaxMode, syntaxModeFileProvider.GetSyntaxModeFile(syntaxMode));
        if (highlightingStrategy.Name != syntaxMode.Name)
        {
          throw new HighlightingDefinitionInvalidException("The name specified in the .xshd '" + highlightingStrategy.Name + "' must be equal the syntax mode name '" + syntaxMode.Name + "'");
        }
      }
      finally
      {
        if (highlightingStrategy == null)
        {
          highlightingStrategy = DefaultHighlighting;
        }
        highlightingDefs[syntaxMode.Name] = highlightingStrategy;
        highlightingStrategy.ResolveReferences();
      }
      return highlightingStrategy;
    }


    /// <summary>
    /// Gets the default highlighting strategy.
    /// </summary>
    /// <value>The default highlighting strategy.</value>
    public DefaultHighlightingStrategy DefaultHighlighting
    {
      get { return (DefaultHighlightingStrategy) highlightingDefs["Default"]; }
    }


    internal KeyValuePair<SyntaxMode, ISyntaxModeFileProvider> FindHighlighterEntry(string name)
    {
      foreach (ISyntaxModeFileProvider provider in syntaxModeFileProviders)
      {
        foreach (SyntaxMode mode in provider.SyntaxModes)
        {
          if (mode.Name == name)
          {
            return new KeyValuePair<SyntaxMode, ISyntaxModeFileProvider>(mode, provider);
          }
        }
      }
      return new KeyValuePair<SyntaxMode, ISyntaxModeFileProvider>(null, null);
    }


    /// <summary>
    /// Finds the syntax highlighting stategy.
    /// </summary>
    /// <param name="name">The name of the syntax highlighting strategy.</param>
    /// <returns>The syntax highlighting strategy.</returns>
    public IHighlightingStrategy FindHighlighter(string name)
    {
      object def = highlightingDefs[name];
      if (def is DictionaryEntry)
        return LoadDefinition((DictionaryEntry) def);

      return def == null ? DefaultHighlighting : (IHighlightingStrategy) def;
    }


    /// <summary>
    /// Finds the syntax highlighting strategy for file.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <returns>The syntax highlighting strategy.</returns>
    public IHighlightingStrategy FindHighlighterForFile(string fileName)
    {
      string highlighterName = (string) extensionsToName[Path.GetExtension(fileName).ToUpperInvariant()];
      if (highlighterName != null)
      {
        object def = highlightingDefs[highlighterName];
        if (def is DictionaryEntry)
          return LoadDefinition((DictionaryEntry) def);

        return def == null ? DefaultHighlighting : (IHighlightingStrategy) def;
      }
      else
      {
        return DefaultHighlighting;
      }
    }


    /// <summary>
    /// Raises the <see cref="ReloadSyntaxHighlighting"/> event.
    /// </summary>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected virtual void OnReloadSyntaxHighlighting(EventArgs e)
    {
      if (ReloadSyntaxHighlighting != null)
      {
        ReloadSyntaxHighlighting(this, e);
      }
    }


    /// <summary>
    /// Occurs when syntax highlighting definitions are reloaded.
    /// </summary>
    public event EventHandler ReloadSyntaxHighlighting;
  }
}
