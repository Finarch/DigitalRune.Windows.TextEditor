using System;
using System.Diagnostics;
using System.Text;
using DigitalRune.Windows.TextEditor.Document;

namespace DigitalRune.Windows.TextEditor.Utilities
{
  /// <summary>
  /// Helper functions for manipulating text.
  /// </summary>
  public static class TextUtilities
  {
    /// <summary>
    /// Converts leading whitespaces to tabs.
    /// </summary>
    /// <param name="line">The line.</param>
    /// <param name="tabIndent">The indentation size.</param>
    /// <returns>The converted line.</returns>
    /// <remarks>
    /// This function takes a string and converts the whitespace in front of
    /// it to tabs. If the length of the whitespace at the start of the string
    /// was not a whole number of tabs then there will still be some spaces just
    /// before the text starts.
    /// the output string will be of the form:
    /// 1. zero or more tabs
    /// 2. zero or more spaces (less than tabIndent)
    /// 3. the rest of the line
    /// </remarks>
    public static string LeadingWhiteSpaceToTabs(string line, int tabIndent)
    {
      StringBuilder sb = new StringBuilder(line.Length);
      int consecutiveSpaces = 0;
      int i = 0;
      for (i = 0; i < line.Length; i++)
      {
        if (line[i] == ' ')
        {
          consecutiveSpaces++;
          if (consecutiveSpaces == tabIndent)
          {
            sb.Append('\t');
            consecutiveSpaces = 0;
          }
        }
        else if (line[i] == '\t')
        {
          sb.Append('\t');
          // if we had say 3 spaces then a tab and tabIndent was 4 then
          // we would want to simply replace all of that with 1 tab
          consecutiveSpaces = 0;
        }
        else
        {
          break;
        }
      }

      if (i < line.Length)
      {
        sb.Append(line.Substring(i - consecutiveSpaces));
      }
      return sb.ToString();
    }


    /// <summary>
    /// Determines whether a letter is a digit or an underscore.
    /// </summary>
    /// <param name="c">The letter.</param>
    /// <returns>
    /// 	<c>true</c> if the letter is a digit or underscore; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsLetterDigitOrUnderscore(char c)
    {
      if (!Char.IsLetterOrDigit(c))
        return c == '_';
      else
        return true;
    }


    /// <summary>
    /// Types of characters
    /// </summary>
    public enum CharacterType
    {
      /// <summary>
      /// A letter, digit or underscore.
      /// </summary>
      LetterDigitOrUnderscore,
      /// <summary>
      /// A whitespace character.
      /// </summary>
      WhiteSpace,
      /// <summary>
      /// Any other character (no letter, digit, or whitespace).
      /// </summary>
      Other
    }


    /// <summary>
    /// Gets the expression before a given offset.
    /// </summary>
    /// <param name="textArea">The text area.</param>
    /// <param name="initialOffset">The initial offset.</param>
    /// <returns>The expression.</returns>
    /// <remarks>
    /// This method returns the expression before a specified offset.
    /// That method is used in code completion to determine the expression before
    /// the caret. The expression can be passed to a parser to resolve the type
    /// or similar.
    /// </remarks>
    public static string GetExpressionBeforeOffset(TextArea textArea, int initialOffset)
    {
      IDocument document = textArea.Document;
      int offset = initialOffset;
      while (offset - 1 > 0)
      {
        switch (document.GetCharAt(offset - 1))
        {
          case '\n':
          case '\r':
          case '}':
            goto done;
          //						offset = SearchBracketBackward(document, offset - 2, '{','}');
          //						break;
          case ']':
            offset = SearchBracketBackward(document, offset - 2, '[', ']');
            break;
          case ')':
            offset = SearchBracketBackward(document, offset - 2, '(', ')');
            break;
          case '.':
            --offset;
            break;
          case '"':
            if (offset < initialOffset - 1)
            {
              return null;
            }
            return "\"\"";
          case '\'':
            if (offset < initialOffset - 1)
            {
              return null;
            }
            return "'a'";
          case '>':
            if (document.GetCharAt(offset - 2) == '-')
            {
              offset -= 2;
              break;
            }
            goto done;
          default:
            if (Char.IsWhiteSpace(document.GetCharAt(offset - 1)))
            {
              --offset;
              break;
            }
            int start = offset - 1;
            if (!IsLetterDigitOrUnderscore(document.GetCharAt(start)))
            {
              goto done;
            }

            while (start > 0 && IsLetterDigitOrUnderscore(document.GetCharAt(start - 1)))
            {
              --start;
            }
            string word = document.GetText(start, offset - start).Trim();
            switch (word)
            {
              case "ref":
              case "out":
              case "in":
              case "return":
              case "throw":
              case "case":
                goto done;
            }

            if (word.Length > 0 && !IsLetterDigitOrUnderscore(word[0]))
            {
              goto done;
            }
            offset = start;
            break;
        }
      }
    done:
      //// simple exit fails when : is inside comment line or any other character
      //// we have to check if we got several ids in resulting line, which usually happens when
      //// id. is typed on next line after comment one
      //// Would be better if lexer would parse properly such expressions. However this will cause
      //// modifications in this area too - to get full comment line and remove it afterwards
      if (offset < 0)
        return string.Empty;

      string resText = document.GetText(offset, textArea.Caret.Offset - offset).Trim();
      int pos = resText.LastIndexOf('\n');
      if (pos >= 0)
      {
        offset += pos + 1;
        //// whitespaces and tabs, which might be inside, will be skipped by trim below
      }
      string expression = document.GetText(offset, textArea.Caret.Offset - offset).Trim();
      return expression;
    }


    /// <summary>
    /// Gets the type of the character.
    /// </summary>
    /// <param name="c">The character.</param>
    /// <returns>The character type.</returns>
    public static CharacterType GetCharacterType(char c)
    {
      if (IsLetterDigitOrUnderscore(c))
        return CharacterType.LetterDigitOrUnderscore;
      if (Char.IsWhiteSpace(c))
        return CharacterType.WhiteSpace;
      return CharacterType.Other;
    }


    /// <summary>
    /// Gets the first non-whitespace character.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="offset">The offset.</param>
    /// <returns>The offset of the first non-whitespace after <paramref name="offset"/>.</returns>
    public static int GetFirstNonWSChar(IDocument document, int offset)
    {
      while (offset < document.TextLength && Char.IsWhiteSpace(document.GetCharAt(offset)))
        ++offset;

      return offset;
    }


    /// <summary>
    /// Finds the word end.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="offset">The offset.</param>
    /// <returns>The offset of the end of the word.</returns>
    public static int FindWordEnd(IDocument document, int offset)
    {
      LineSegment line = document.GetLineSegmentForOffset(offset);
      int endPos = line.Offset + line.Length;
      while (offset < endPos && IsLetterDigitOrUnderscore(document.GetCharAt(offset)))
        ++offset;

      return offset;
    }


    /// <summary>
    /// Finds the word start.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="offset">The offset.</param>
    /// <returns>The offset of the start of the word.</returns>
    public static int FindWordStart(IDocument document, int offset)
    {
      LineSegment line = document.GetLineSegmentForOffset(offset);
      while (offset > line.Offset && IsLetterDigitOrUnderscore(document.GetCharAt(offset - 1)))
        --offset;

      return offset;
    }


    /// <summary>
    /// Finds the offset where the next word starts.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="offset">The offset.</param>
    /// <returns>The start of the next word after <paramref name="offset"/>.</returns>
    public static int FindNextWordStart(IDocument document, int offset)
    {
      // go forward to the start of the next word
      // if the cursor is at the start or in the middle of a word we move to the end of the word
      // and then past any whitespace that follows it
      // if the cursor is at the start or in the middle of some whitespace we move to the start of the
      // next word

      int originalOffset = offset;
      LineSegment line = document.GetLineSegmentForOffset(offset);
      int endPos = line.Offset + line.Length;
      // lets go to the end of the word, whitespace or operator
      CharacterType t = GetCharacterType(document.GetCharAt(offset));
      while (offset < endPos && GetCharacterType(document.GetCharAt(offset)) == t)
      {
        ++offset;
      }

      // now we're at the end of the word, lets find the start of the next one by skipping whitespace
      while (offset < endPos && GetCharacterType(document.GetCharAt(offset)) == CharacterType.WhiteSpace)
      {
        ++offset;
      }

      return offset;
    }


    /// <summary>
    /// Finds the offset where the previous word starts.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="offset">The offset.</param>
    /// <returns>The start of the word before <paramref name="offset"/>.</returns>
    public static int FindPrevWordStart(IDocument document, int offset)
    {
      // go back to the start of the word we are on
      // if we are already at the start of a word or if we are in whitespace, then go back
      // to the start of the previous word

      int originalOffset = offset;
      if (offset > 0)
      {
        LineSegment line = document.GetLineSegmentForOffset(offset);
        CharacterType t = GetCharacterType(document.GetCharAt(offset - 1));
        while (offset > line.Offset && GetCharacterType(document.GetCharAt(offset - 1)) == t)
        {
          --offset;
        }

        // if we were in whitespace, and now we're at the end of a word or operator, go back to the beginning of it
        if (t == CharacterType.WhiteSpace && offset > line.Offset)
        {
          t = GetCharacterType(document.GetCharAt(offset - 1));
          while (offset > line.Offset && GetCharacterType(document.GetCharAt(offset - 1)) == t)
          {
            --offset;
          }
        }
      }

      return offset;
    }


    /// <summary>
    /// Gets the line as string.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="lineNumber">The line number.</param>
    /// <returns>The line as string.</returns>
    public static string GetLineAsString(IDocument document, int lineNumber)
    {
      LineSegment line = document.GetLineSegment(lineNumber);
      return document.GetText(line.Offset, line.Length);
    }


    /// <summary>
    /// Searches backwards for a matching bracket..
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="openBracket">The open bracket.</param>
    /// <param name="closingBracket">The closing bracket.</param>
    /// <returns>The offset of the matching bracket.</returns>
    public static int SearchBracketBackward(IDocument document, int offset, char openBracket, char closingBracket)
    {
      return document.FormattingStrategy.SearchBracketBackward(document, offset, openBracket, closingBracket);
    }


    /// <summary>
    /// Searches forwards for a matching bracket.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="openBracket">The open bracket.</param>
    /// <param name="closingBracket">The closing bracket.</param>
    /// <returns>The offset of the matching bracket.</returns>
    public static int SearchBracketForward(IDocument document, int offset, char openBracket, char closingBracket)
    {
      return document.FormattingStrategy.SearchBracketForward(document, offset, openBracket, closingBracket);
    }


    /// <summary>
    /// Determines whether a line is empty or filled with whitespaces.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="lineNumber">The line number.</param>
    /// <returns>
    /// 	<c>true</c> if line is empty of filled with whitespaces; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// Returns true, if the line lineNumber is empty or filled with whitespaces.
    /// </remarks>
    public static bool IsEmptyLine(IDocument document, int lineNumber)
    {
      return IsEmptyLine(document, document.GetLineSegment(lineNumber));
    }


    /// <summary>
    /// Determines whether a line is empty or filled with whitespaces.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="line">The line.</param>
    /// <returns>
    /// 	<c>true</c> if line is empty of filled with whitespaces; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// Returns true, if the line lineNumber is empty or filled with whitespaces.
    /// </remarks>
    public static bool IsEmptyLine(IDocument document, LineSegment line)
    {
      for (int i = line.Offset; i < line.Offset + line.Length; ++i)
      {
        char ch = document.GetCharAt(i);
        if (!Char.IsWhiteSpace(ch))
          return false;
      }
      return true;
    }


    static bool IsWordPart(char ch)
    {
      return IsLetterDigitOrUnderscore(ch) || ch == '.';
    }


    /// <summary>
    /// Gets the word at a given offset.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="offset">The offset.</param>
    /// <returns>The word.</returns>
    public static string GetWordAt(IDocument document, int offset)
    {
      if (offset < 0 || offset >= document.TextLength || !IsWordPart(document.GetCharAt(offset)))
        return String.Empty;

      int startOffset = offset;
      int endOffset = offset;
      while (startOffset > 0 && IsWordPart(document.GetCharAt(startOffset - 1)))
        --startOffset;

      while (endOffset < document.TextLength - 1 && IsWordPart(document.GetCharAt(endOffset + 1)))
        ++endOffset;

      Debug.Assert(endOffset >= startOffset);
      return document.GetText(startOffset, endOffset - startOffset + 1);
    }


    /// <summary>
    /// Checks whether a region (offset + length) matches a given word.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="length">The length.</param>
    /// <param name="word">The word.</param>
    /// <returns><c>true</c> if region matches word.</returns>
    /// <remarks>
    /// The comparison is case-sensitive.
    /// </remarks>
    public static bool RegionMatches(IDocument document, int offset, int length, string word)
    {
      if (length != word.Length || document.TextLength < offset + length)
        return false;

      for (int i = 0; i < length; ++i)
        if (document.GetCharAt(offset + i) != word[i])
          return false;

      return true;
    }


    /// <summary>
    /// Checks whether a region (offset + length) matches a given word.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="casesensitive">If set to <c>true</c> the comparison is case-sensitive.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="length">The length.</param>
    /// <param name="word">The word.</param>
    /// <returns><c>true</c> if region matches word.</returns>
    public static bool RegionMatches(IDocument document, bool casesensitive, int offset, int length, string word)
    {
      if (casesensitive)
        return RegionMatches(document, offset, length, word);

      if (length != word.Length || document.TextLength < offset + length)
        return false;

      for (int i = 0; i < length; ++i)
        if (Char.ToUpper(document.GetCharAt(offset + i)) != Char.ToUpper(word[i]))
          return false;

      return true;
    }


    /// <summary>
    /// Checks whether a region (offset + length) matches a given word.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="length">The length.</param>
    /// <param name="word">The word.</param>
    /// <returns><c>true</c> if region matches word.</returns>
    /// <remarks>
    /// The comparison is case-sensitive.
    /// </remarks>
    public static bool RegionMatches(IDocument document, int offset, int length, char[] word)
    {
      if (length != word.Length || document.TextLength < offset + length)
        return false;

      for (int i = 0; i < length; ++i)
        if (document.GetCharAt(offset + i) != word[i])
          return false;

      return true;
    }

    
    /// <summary>
    /// Checks whether a region (offset + length) matches a given word.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="casesensitive">If set to <c>true</c> the comparison is case-sensitive.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="length">The length.</param>
    /// <param name="word">The word.</param>
    /// <returns><c>true</c> if region matches word.</returns>
    public static bool RegionMatches(IDocument document, bool casesensitive, int offset, int length, char[] word)
    {
      if (casesensitive)
        return RegionMatches(document, offset, length, word);

      if (length != word.Length || document.TextLength < offset + length)
        return false;

      for (int i = 0; i < length; ++i)
        if (Char.ToUpper(document.GetCharAt(offset + i)) != Char.ToUpper(word[i]))
          return false;

      return true;
    }
  }
}
