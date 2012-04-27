using System;
using System.Collections.Generic;
using System.Drawing;
using DigitalRune.Windows.TextEditor.Document;
using DigitalRune.Windows.TextEditor.Selections;

namespace DigitalRune.Windows.TextEditor.CustomLines
{
  /// <summary>
  /// Handles the custom lines (custom background color, read-only flag) for a buffer.
  /// </summary>
  public interface ICustomLineManager
  {
    /// <summary>
    /// Gets a list of the custom lines.
    /// </summary>
    /// <value>Contains all custom lines</value>
    List<CustomLine> CustomLines { get; }

    /// <summary>
    /// Gets the background color of the line.
    /// </summary>
    /// <param name="lineNr">The line number.</param>
    /// <param name="defaultColor">The default background color.</param>
    /// <returns>The color if the line <c>lineNr</c> has custom background color;
    /// otherwise returns <c>defaultColor</c></returns>
    Color GetCustomColor(int lineNr, Color defaultColor);

    /// <summary>
    /// Determines whether a line is read-only.
    /// </summary>
    /// <param name="lineNr">The line number.</param>
    /// <param name="defaultReadOnly">
    /// The default status of the lines (<c>true</c> = read-only).
    /// </param>
    /// <returns>
    /// <c>true</c> if the line is read-only; otherwise, <c>false</c>.
    /// </returns>
    bool IsReadOnly(int lineNr, bool defaultReadOnly);

    /// <summary>
    /// Determines whether a selection is read-only.
    /// </summary>
    /// <param name="selection">The selection.</param>
    /// <param name="defaultReadOnly">
    /// The default status of the lines (<c>true</c> = read-only).
    /// </param>
    /// <returns>
    /// <c>true</c> if the line is read-only; otherwise, <c>false</c>.
    /// </returns>
    bool IsReadOnly(ISelection selection, bool defaultReadOnly);

    /// <summary>
    /// Adds a custom line.
    /// </summary>
    /// <param name="lineNr">The line number.</param>
    /// <param name="customColor">The background color of the line.</param>
    /// <param name="readOnly">If set to <c>true</c> the line is marked as read-only.</param>
    void AddCustomLine(int lineNr, Color customColor, bool readOnly);

    /// <summary>
    /// Adds a range of custom lines.
    /// </summary>
    /// <param name="startLineNr">The start line number.</param>
    /// <param name="endLineNr">The end line number.</param>
    /// <param name="customColor">The background color of these custom lines.</param>
    /// <param name="readOnly">If set to <c>true</c> the lines are marked as read-only.</param>
    void AddCustomLine(int startLineNr, int endLineNr, Color customColor, bool readOnly);

    /// <summary>
    /// Removes the custom line.
    /// </summary>
    /// <param name="lineNr">The line number.</param>
    void RemoveCustomLine(int lineNr);

    /// <summary>
    /// Clears all custom lines.
    /// </summary>
    void Clear();

    /// <summary>
    /// Occurs before a change.
    /// </summary>
    event EventHandler BeforeChanged;

    /// <summary>
    /// Occurs after a change.
    /// </summary>
    event EventHandler Changed;
  }
}
