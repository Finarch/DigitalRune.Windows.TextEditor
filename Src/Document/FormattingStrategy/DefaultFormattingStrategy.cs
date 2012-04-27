using System;
using System.Text;
using DigitalRune.Windows.TextEditor.Document;
using DigitalRune.Windows.TextEditor.Utilities;
using DigitalRune.Windows.TextEditor.Properties;

namespace DigitalRune.Windows.TextEditor.Formatting
{
  /// <summary>
  /// This class handles the auto and smart indenting in the text buffer while
  /// you type.
  /// </summary>
  public class DefaultFormattingStrategy : IFormattingStrategy
  {
    /// <summary>
    /// Returns the whitespaces which are before a non white space character in the line
    /// as a string.
    /// </summary>
    /// <param name="textArea">The text area.</param>
    /// <param name="lineNumber">The line number.</param>
    /// <returns>
    /// The whitespaces which are before a non white space character in the line
    /// as a string.
    /// </returns>
    protected string GetIndentation(TextArea textArea, int lineNumber)
    {
      if (lineNumber < 0 || lineNumber > textArea.Document.TotalNumberOfLines)
        throw new ArgumentOutOfRangeException("lineNumber");

      string lineText = TextUtilities.GetLineAsString(textArea.Document, lineNumber);
      StringBuilder whitespaces = new StringBuilder();

      foreach (char ch in lineText)
      {
        if (Char.IsWhiteSpace(ch))
          whitespaces.Append(ch);
        else
          break;
      }
      return whitespaces.ToString();
    }


    /// <summary>
    /// Could be overwritten to define more complex indenting.
    /// </summary>
    protected virtual int AutoIndentLine(TextArea textArea, int lineNumber)
    {
      string indentation = lineNumber != 0 ? GetIndentation(textArea, lineNumber - 1) : "";
      if (indentation.Length > 0)
      {
        string newLineText = indentation + TextUtilities.GetLineAsString(textArea.Document, lineNumber).Trim();
        LineSegment oldLine = textArea.Document.GetLineSegment(lineNumber);
        textArea.Document.Replace(oldLine.Offset, oldLine.Length, newLineText);
      }
      return indentation.Length;
    }


    /// <summary>
    /// Could be overwritten to define more complex indenting.
    /// </summary>
    protected virtual int SmartIndentLine(TextArea textArea, int line)
    {
      return AutoIndentLine(textArea, line); // smart = autoindent in normal texts
    }


    /// <summary>
    /// This function formats a specific line after a character is typed.
    /// </summary>
    /// <param name="textArea">The text area.</param>
    /// <param name="line">The line.</param>
    /// <param name="caretOffset">The caret offset.</param>
    /// <param name="ch">The character typed.</param>
    public virtual void FormatLine(TextArea textArea, int line, int caretOffset, char ch)
    {
      if (ch == '\n')
        textArea.Caret.Column = IndentLine(textArea, line);
    }


    /// <summary>
    /// This function sets the indentation level in a specific line
    /// </summary>
    /// <param name="textArea">The text area.</param>
    /// <param name="line">The line.</param>
    /// <returns>The number of inserted characters.</returns>
    public int IndentLine(TextArea textArea, int line)
    {
      textArea.Document.UndoStack.StartUndoGroup();
      int result;
      switch (textArea.Document.TextEditorProperties.IndentStyle)
      {
        case IndentStyle.None:
          result = 0;
          break;
        case IndentStyle.Auto:
          result = AutoIndentLine(textArea, line);
          break;
        case IndentStyle.Smart:
          result = SmartIndentLine(textArea, line);
          break;
        default:
          throw new NotSupportedException("Unsupported value for IndentStyle: " + textArea.Document.TextEditorProperties.IndentStyle);
      }
      textArea.Document.UndoStack.EndUndoGroup();
      return result;
    }


    /// <summary>
    /// This function sets the indentation level in a range of lines.
    /// </summary>
    /// <param name="textArea">The text area.</param>
    /// <param name="begin">The begin.</param>
    /// <param name="end">The end.</param>
    public virtual void IndentLines(TextArea textArea, int begin, int end)
    {
      textArea.Document.UndoStack.StartUndoGroup();

      for (int i = begin; i <= end; ++i)
        IndentLine(textArea, i);

      textArea.Document.UndoStack.EndUndoGroup();
    }


    /// <summary>
    /// Finds the offset of the opening bracket in the block defined by offset skipping
    /// brackets in strings and comments.
    /// </summary>
    /// <param name="document">The document to search in.</param>
    /// <param name="offset">The offset of an position in the block or the offset of the closing bracket.</param>
    /// <param name="openBracket">The character for the opening bracket.</param>
    /// <param name="closingBracket">The character for the closing bracket.</param>
    /// <returns>
    /// Returns the offset of the opening bracket or -1 if no matching bracket was found.
    /// </returns>
    public virtual int SearchBracketBackward(IDocument document, int offset, char openBracket, char closingBracket)
    {
      int brackets = -1;

      // first try "quick find" - find the matching bracket if there is no string/comment in the way
      for (int i = offset; i >= 0; --i)
      {
        char ch = document.GetCharAt(i);
        if (ch == openBracket)
        {
          ++brackets;
          if (brackets == 0) return i;
        }
        else if (ch == closingBracket)
        {
          --brackets;
        }
        else if (ch == '"')
        {
          break;
        }
        else if (ch == '\'')
        {
          break;
        }
        else if (ch == '/' && i > 0)
        {
          if (document.GetCharAt(i - 1) == '/') 
            break;
          if (document.GetCharAt(i - 1) == '*') 
            break;
        }
      }
      return -1;
    }


    /// <summary>
    /// Finds the offset of the closing bracket in the block defined by offset skipping
    /// brackets in strings and comments.
    /// </summary>
    /// <param name="document">The document to search in.</param>
    /// <param name="offset">The offset of an position in the block or the offset of the opening bracket.</param>
    /// <param name="openBracket">The character for the opening bracket.</param>
    /// <param name="closingBracket">The character for the closing bracket.</param>
    /// <returns>
    /// Returns the offset of the closing bracket or -1 if no matching bracket was found.
    /// </returns>
    public virtual int SearchBracketForward(IDocument document, int offset, char openBracket, char closingBracket)
    {
      int brackets = 1;
      // try "quick find" - find the matching bracket if there is no string/comment in the way
      for (int i = offset; i < document.TextLength; ++i)
      {
        char ch = document.GetCharAt(i);
        if (ch == openBracket)
        {
          ++brackets;
        }
        else if (ch == closingBracket)
        {
          --brackets;
          if (brackets == 0) 
            return i;
        }
        else if (ch == '"')
        {
          break;
        }
        else if (ch == '\'')
        {
          break;
        }
        else if (ch == '/' && i > 0)
        {
          if (document.GetCharAt(i - 1) == '/') 
            break;
        }
        else if (ch == '*' && i > 0)
        {
          if (document.GetCharAt(i - 1) == '/') 
            break;
        }
      }
      return -1;
    }
  }
}
