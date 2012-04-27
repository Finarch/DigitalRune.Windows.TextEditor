using System;
using System.Collections.Generic;
using System.Drawing;
using DigitalRune.Windows.TextEditor.Document;
using DigitalRune.Windows.TextEditor.Selections;

namespace DigitalRune.Windows.TextEditor.CustomLines
{
  /// <summary>
  /// Describes a custom line (custom background color, read-only flag).
  /// </summary>
  public class CustomLine
  {
    /// <summary>
    /// The start line number.
    /// </summary>
    public int StartLineNr;


    /// <summary>
    /// The end line number.
    /// </summary>
    public int EndLineNr;


    /// <summary>
    /// The custom color.
    /// </summary>
    public Color Color;


    /// <summary>
    /// <c>true</c> if custom line is read-only.
    /// </summary>
    public bool ReadOnly;


    /// <summary>
    /// Initializes a new instance of the <see cref="CustomLine"/> class.
    /// </summary>
    /// <param name="lineNr">The line number.</param>
    /// <param name="customColor">The custom color.</param>
    /// <param name="readOnly"><c>true</c> if custom line is read-only.</param>
    public CustomLine(int lineNr, Color customColor, bool readOnly)
    {
      this.StartLineNr = this.EndLineNr = lineNr;
      this.Color = customColor;
      this.ReadOnly = readOnly;
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="CustomLine"/> class.
    /// </summary>
    /// <param name="startLineNr">The start line number.</param>
    /// <param name="endLineNr">The end line number.</param>
    /// <param name="customColor">The custom color.</param>
    /// <param name="readOnly"><c>true</c> if custom linen is read-only.</param>
    public CustomLine(int startLineNr, int endLineNr, Color customColor, bool readOnly)
    {
      this.StartLineNr = startLineNr;
      this.EndLineNr = endLineNr;
      this.Color = customColor;
      this.ReadOnly = readOnly;
    }
  }


  /// <summary>
  /// Handles the custom lines (custom background color, read-only flag) for a buffer.
  /// </summary>
  internal class CustomLineManager : ICustomLineManager
  {
    List<CustomLine> lines = new List<CustomLine>();

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomLineManager"/> class.
    /// </summary>
    /// <param name="lineTracker">The line tracker.</param>
    public CustomLineManager(LineManager lineTracker)
    {
      lineTracker.LineCountChanged += MoveIndices;
    }


    /// <summary>
    /// Gets a list of the custom lines.
    /// </summary>
    /// <value>Contains all custom lines</value>
    public List<CustomLine> CustomLines
    {
      get { return lines; }
    }


    /// <summary>
    /// Gets the background color of the line.
    /// </summary>
    /// <param name="lineNr">The line number.</param>
    /// <param name="defaultColor">The default background color.</param>
    /// <returns>
    /// The color if the line <c>lineNr</c> has custom background color;
    /// otherwise returns <c>defaultColor</c>
    /// </returns>
    public Color GetCustomColor(int lineNr, Color defaultColor)
    {
      foreach (CustomLine line in lines)
        if (line.StartLineNr <= lineNr && line.EndLineNr >= lineNr)
          return line.Color;
      return defaultColor;
    }


    /// <summary>
    /// Determines whether a line is read-only.
    /// </summary>
    /// <param name="lineNr">The line number.</param>
    /// <param name="defaultReadOnly">The default status of the lines (<c>true</c> = read-only).</param>
    /// <returns>
    /// 	<c>true</c> if the line is read-only; otherwise, <c>false</c>.
    /// </returns>
    public bool IsReadOnly(int lineNr, bool defaultReadOnly)
    {
      foreach (CustomLine line in lines)
        if (line.StartLineNr <= lineNr && line.EndLineNr >= lineNr)
          return line.ReadOnly;
      return defaultReadOnly;
    }


    /// <summary>
    /// Determines whether a selection is read-only.
    /// </summary>
    /// <param name="selection"></param>
    /// <param name="defaultReadOnly">The default status of the lines (<c>true</c> = read-only).</param>
    /// <returns>
    /// 	<c>true</c> if the line is read-only; otherwise, <c>false</c>.
    /// </returns>
    public bool IsReadOnly(ISelection selection, bool defaultReadOnly)
    {
      int startLine = selection.StartPosition.Y;
      int endLine = selection.EndPosition.Y;
      foreach (CustomLine customLine in lines)
      {
        if (customLine.ReadOnly == false)
          continue;
        if (startLine < customLine.StartLineNr && endLine < customLine.StartLineNr)
          continue;
        if (startLine > customLine.EndLineNr && endLine > customLine.EndLineNr)
          continue;
        return true;
      }
      return defaultReadOnly;
    }


    /// <summary>
    /// Clears all custom lines.
    /// </summary>
    public void Clear()
    {
      OnBeforeChanged();
      lines.Clear();
      OnChanged();
    }


    /// <summary>
    /// Occurs when before a change.
    /// </summary>
    public event EventHandler BeforeChanged;


    /// <summary>
    /// Occurs after a change.
    /// </summary>
    public event EventHandler Changed;


    void OnChanged()
    {
      if (Changed != null)
      {
        Changed(this, null);
      }
    }


    void OnBeforeChanged()
    {
      if (BeforeChanged != null)
      {
        BeforeChanged(this, null);
      }
    }


    /// <summary>
    /// Adds a custom line.
    /// </summary>
    /// <param name="lineNr">The line number.</param>
    /// <param name="customColor">The background color of the line.</param>
    /// <param name="readOnly">If set to <c>true</c> the line is marked as read-only.</param>
    public void AddCustomLine(int lineNr, Color customColor, bool readOnly)
    {
      OnBeforeChanged();
      lines.Add(new CustomLine(lineNr, customColor, readOnly));
      OnChanged();
    }


    /// <summary>
    /// Adds a range of custom lines.
    /// </summary>
    /// <param name="startLineNr">The start line number.</param>
    /// <param name="endLineNr">The end line number.</param>
    /// <param name="customColor">The background color of these custom lines.</param>
    /// <param name="readOnly">If set to <c>true</c> the lines are marked as read-only.</param>
    public void AddCustomLine(int startLineNr, int endLineNr, Color customColor, bool readOnly)
    {
      OnBeforeChanged();
      lines.Add(new CustomLine(startLineNr, endLineNr, customColor, readOnly));
      OnChanged();
    }


    /// <summary>
    /// Removes the custom line.
    /// </summary>
    /// <param name="lineNr">The line number.</param>
    public void RemoveCustomLine(int lineNr)
    {
      for (int i = 0; i < lines.Count; ++i)
      {
        if (((CustomLine) lines[i]).StartLineNr <= lineNr && ((CustomLine) lines[i]).EndLineNr >= lineNr)
        {
          OnBeforeChanged();
          lines.RemoveAt(i);
          OnChanged();
          return;
        }
      }
    }


    /// <summary>
    /// This method moves all indices from index upward count lines
    /// (useful for deletion/insertion of text)
    /// </summary>
    void MoveIndices(object sender, LineCountChangeEventArgs e)
    {
      bool changed = false;
      OnBeforeChanged();
      for (int i = 0; i < lines.Count; ++i)
      {
        int startLineNr = ((CustomLine) lines[i]).StartLineNr;
        int endLineNr = ((CustomLine) lines[i]).EndLineNr;
        if (e.LineStart >= startLineNr && e.LineStart < endLineNr)
        {
          changed = true;
          ((CustomLine) lines[i]).EndLineNr += e.LinesMoved;
        }
        else if (e.LineStart < startLineNr)
        {
          ((CustomLine) lines[i]).StartLineNr += e.LinesMoved;
          ((CustomLine) lines[i]).EndLineNr += e.LinesMoved;
        }
        else
        {
        }
      }

      if (changed)
        OnChanged();
    }
  }
}
