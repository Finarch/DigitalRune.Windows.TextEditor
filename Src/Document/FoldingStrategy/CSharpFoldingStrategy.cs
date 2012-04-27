using System;
using System.Collections.Generic;
using DigitalRune.Windows.TextEditor.Document;
using DigitalRune.Windows.TextEditor.Utilities;


namespace DigitalRune.Windows.TextEditor.Folding
{
  /// <summary>
  /// A simple folding strategy for C# files.
  /// </summary>
  public class CSharpFoldingStrategy : IFoldingStrategy
  {
    /// <summary>
    /// Generates the fold markers.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="parseInformation">The parse information.</param>
    /// <returns>A list containing all foldings.</returns>
    public List<FoldMarker> GenerateFoldMarkers(IDocument document, string fileName, object parseInformation)
    {
      List<FoldMarker> foldMarkers = new List<FoldMarker>();
      MarkCodeBlocks(document, foldMarkers);
      MarkRegions(document, foldMarkers);
      return foldMarkers;
    }


    /// <summary>
    /// Marks all code blocks (namespaces, classes, methods, etc.) in the document.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="foldMarkers">The fold markers.</param>
    private void MarkCodeBlocks(IDocument document, List<FoldMarker> foldMarkers)
    {
      int offset = 0;
      while (offset < document.TextLength)
      {
        switch (document.GetCharAt(offset))
        {
          case '/':
            offset = SkipComment(document, offset);
            break;
          case 'c':
            offset = MarkBlock("class", document, offset, foldMarkers);
            break;
          case 'e':
            offset = MarkBlock("enum", document, offset, foldMarkers);
            break;
          case 'i':
            offset = MarkBlock("interface", document, offset, foldMarkers);
            break;
          case 'n':
            offset = MarkBlock("namespace", document, offset, foldMarkers);
            break;
          case 's':
            offset = MarkBlock("struct", document, offset, foldMarkers);
            break;
          case '{':
            offset = MarkMethod(document, offset, foldMarkers);
            break;
          default:
            offset = TextUtilities.FindWordEnd(document, offset) + 1;
            break;
        }
      }
    }


    /// <summary>
    /// Skips any comments that start at the current offset.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="offset">The offset.</param>
    /// <returns>The index of the next character after the comments.</returns>
    private int SkipComment(IDocument document, int offset)
    {
      if (offset >= document.TextLength - 1)
        return offset + 1;

      char current = document.GetCharAt(offset);
      char next = document.GetCharAt(offset + 1);

      if (current == '/' && next == '/')
      {
        // Skip line comment "//"
        LineSegment line = document.GetLineSegmentForOffset(offset);
        int offsetOfNextLine = line.Offset + line.TotalLength;
        return offsetOfNextLine;
      }
      else if (current == '/' && next == '*')
      {
        // Skip block comment "/* ... */"
        offset += 2;
        while (offset + 1 < document.TextLength)
        {
          if (document.GetCharAt(offset) == '*' && document.GetCharAt(offset + 1) == '/')
          {
            offset = offset + 2;
            break;
          }
          offset++;
        }
        return offset;
      }
      else
      {
        return offset + 1;
      }
    }


    /// <summary>
    /// Marks the block that starts at the current offset.
    /// </summary>
    /// <param name="name">The identifier of the block (e.g. "class", "struct").</param>
    /// <param name="document">The document.</param>
    /// <param name="offset">The offset of the identifier.</param>
    /// <param name="foldMarkers">The fold markers.</param>
    /// <returns>The index of the next character after the block.</returns>
    private int MarkBlock(string name, IDocument document, int offset, List<FoldMarker> foldMarkers)
    {
      if (offset >= document.TextLength)
        return offset;

      char c;
      int offsetOfClosingBracket;

      string word = TextUtilities.GetWordAt(document, offset);
      if (word == name)
      {
        offset += word.Length;
        while (offset < document.TextLength)
        {
          c = document.GetCharAt(offset);
          if (c == '}' || c == ';')
          {
            offset++;
            break;
          }
          if (c == '{')
          {
            int startOffset = offset;
            while (Char.IsWhiteSpace(document.GetCharAt(startOffset - 1)))
              startOffset--;

            offsetOfClosingBracket = TextUtilities.SearchBracketForward(document, offset + 1, '{', '}');
            if (offsetOfClosingBracket > 0)
            {
              int length = offsetOfClosingBracket - startOffset + 1;
              foldMarkers.Add(new FoldMarker(document, startOffset, length, "{...}", false));

              // Skip to offset after '{'.
              offset++;
              break;
            }
          }
          offset++;
        }
      }
      else
      {
        // Skip to next word
        offset += word.Length;
      }
      return offset;
    }


    /// <summary>
    /// Marks the method whose block starts at the given offset.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="offset">The offset of the method body ('{').</param>
    /// <param name="foldMarkers">The fold markers.</param>
    /// <returns>The index of the next character after the method.</returns>
    private int MarkMethod(IDocument document, int offset, List<FoldMarker> foldMarkers)
    {
      if (offset >= document.TextLength)
        return offset;

      int startOffset = offset;
      while (startOffset - 1 > 0 && Char.IsWhiteSpace(document.GetCharAt(startOffset - 1)))
        startOffset--;

      int offsetOfClosingBracket = TextUtilities.SearchBracketForward(document, offset + 1, '{', '}');
      if (offsetOfClosingBracket > 0)
      {
        // Check whether next character is ';'
        int offsetOfNextCharacter = TextUtilities.GetFirstNonWSChar(document, offsetOfClosingBracket + 1);
        if (offsetOfNextCharacter < document.TextLength && document.GetCharAt(offsetOfNextCharacter) == ';')
          return offset + 1;

        int length = offsetOfClosingBracket - startOffset + 1;
        foldMarkers.Add(new FoldMarker(document, startOffset, length, "{...}", false));

        // Skip to offset after '}'. (Ignore nested blocks.)
        offset = offsetOfClosingBracket + 1;
        return offset;
      }
      else
      {
        return offset + 1;
      }
    }


    /// <summary>
    /// Marks all regions ("#region") in the document.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="foldMarkers">The fold markers.</param>
    private void MarkRegions(IDocument document, List<FoldMarker> foldMarkers)
    {
      FindAndMarkRegions(document, 0, foldMarkers);
    }


    /// <summary>
    /// Finds and marks all regions.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="offset">The offset where the search starts.</param>
    /// <param name="foldMarkers">The fold markers.</param>
    /// <returns>The index of the next character after the all regions.</returns>
    /// <remarks>
    /// This method returns when it finds a "#endregion" string that does not have
    /// a "#region" statement after <paramref name="offset"/>. In this case it 
    /// returns the index of the next character after the "#endregion" statement.
    /// </remarks>
    private int FindAndMarkRegions(IDocument document, int offset, List<FoldMarker> foldMarkers)
    {
      if (offset >= document.TextLength)
        return offset;

      while (offset < document.TextLength)
      {
        char c = document.GetCharAt(offset);
        switch (c)
        {
          case '/':
            // Skip comments
            offset = SkipComment(document, offset);
            break;
          case '#':
            string word = TextUtilities.GetWordAt(document, offset + 1);
            if (word == "region")
            {
              offset = MarkRegion(document, offset, foldMarkers);
            }
            else if (word == "endregion")
            {
              return offset + "endregion".Length + 1;
            }
            else
            {
              offset++;
            }
            break;
          default:
            // Skip to next word
            offset = TextUtilities.FindWordEnd(document, offset) + 1;
            break;
        }
      }
      return offset;
    }


    /// <summary>
    /// Marks the region that starts at the given offset.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="foldMarkers">The fold markers.</param>
    /// <returns>The index of the next character after the region.</returns>
    private int MarkRegion(IDocument document, int offset, List<FoldMarker> foldMarkers)
    {
      if (offset >= document.TextLength)
        return offset;

      if (document.GetCharAt(offset) == '#')
      {
        int startOffset = offset;
        offset++;
        string word = TextUtilities.GetWordAt(document, offset);
        if (word == "region")
        {
          offset += "region".Length;

          // Find label
          LineSegment line = document.GetLineSegmentForOffset(offset);
          int lineEnd = line.Offset + line.Length;
          int labelLength = lineEnd - offset;
          string label = document.GetText(offset, labelLength);
          label = label.Trim();
          if (label.Length == 0)
            label = "#region";

          // Find and mark subregions
          offset = FindAndMarkRegions(document, lineEnd, foldMarkers);

          if (offset <= document.TextLength)          
          {
            int length = offset - startOffset;
            foldMarkers.Add(new FoldMarker(document, startOffset, length, label, true));
            offset++;
          }
        }
      }
      else
      {
        offset++;
      }
      return offset;
    }
  }
}
