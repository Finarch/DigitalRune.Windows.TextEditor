using System;
using DigitalRune.Windows.TextEditor.Document;
using DigitalRune.Windows.TextEditor.Utilities;

namespace DigitalRune.Windows.TextEditor.Highlighting
{
  /// <summary>
  /// This class implements a keyword map. It implements a digital search tree (tries) to find
  /// a word.
  /// </summary>
  public class LookupTable
  {
    Node root = new Node(null, null);
    bool casesensitive;
    int length;


    /// <value>
    /// The number of elements in the table
    /// </value>
    public int Count
    {
      get { return length; }
    }


    /// <summary>
    /// Get the object that was inserted under the keyword.
    /// </summary>
    /// <value>The object that was inserted under the keyword.</value>
    /// <remarks>
    /// <para>
    /// The keyword is taken from an <see cref="IDocument"/> at a given location
    /// (line, offset in line, length).
    /// </para>
    /// <para>
    /// Returns null, if no such keyword exists.
    /// </para>
    /// </remarks>
    public object this[IDocument document, LineSegment line, int offsetInLine, int lengthOfWord]
    {
      get
      {
        if (lengthOfWord == 0)
          return null;

        Node next = root;

        int wordOffset = line.Offset + offsetInLine;
        if (casesensitive)
        {
          for (int i = 0; i < lengthOfWord; ++i)
          {
            int index = ((int) document.GetCharAt(wordOffset + i)) % 256;
            next = next.leaf[index];

            if (next == null)
              return null;

            if (next.color != null && TextUtilities.RegionMatches(document, wordOffset, lengthOfWord, next.word))
              return next.color;
          }
        }
        else
        {
          for (int i = 0; i < lengthOfWord; ++i)
          {
            int index = ((int) Char.ToUpper(document.GetCharAt(wordOffset + i))) % 256;

            next = next.leaf[index];

            if (next == null)
              return null;

            if (next.color != null && TextUtilities.RegionMatches(document, casesensitive, wordOffset, lengthOfWord, next.word))
              return next.color;
          }
        }
        return null;
      }
    }


    /// <summary>
    /// Gets or sets an object in the tree under the keyword.
    /// </summary>
    public object this[string keyword]
    {
      get
      {
        if (String.IsNullOrEmpty(keyword))
          return null;

        Node next = root;
        int length = keyword.Length;

        if (casesensitive)
        {
          for (int i = 0; i < length; ++i)
          {
            int index = (int) keyword[i] % 256;
            next = next.leaf[index];

            if (next == null)
              return null;

            if (next.color != null && String.Compare(keyword, next.word, false) == 0)
              return next.color;
          }
        }
        else
        {
          for (int i = 0; i < length; ++i)
          {
            int index = (int) Char.ToUpper(keyword[i]) % 256;

            next = next.leaf[index];

            if (next == null)
              return null;

            if (next.color != null && String.Compare(keyword, next.word, true) == 0)
              return next.color;
          }
        }
        return null;
      }

      set
      {
        Node node = root;
        Node next = root;
        if (!casesensitive)
        {
          keyword = keyword.ToUpper();
        }
        ++length;

        // insert word into the tree
        for (int i = 0; i < keyword.Length; ++i)
        {
          int index = ((int) keyword[i]) % 256; // index of current char
          bool d = keyword[i] == '\\';

          next = next.leaf[index];             // get node to this index

          if (next == null)
          { // no node created -> insert word here
            node.leaf[index] = new Node(value, keyword);
            break;
          }

          if (next.word != null && next.word.Length != i)
          { // node there, take node content and insert them again
            string tmpword = next.word;                  // this word will be inserted 1 level deeper (better, don't need too much 
            object tmpcolor = next.color;                 // string comparisons for finding.)
            next.color = next.word = null;
            this[tmpword] = tmpcolor;
          }

          if (i == keyword.Length - 1)
          { // end of keyword reached, insert node there, if a node was here it was
            next.word = keyword;       // reinserted, if it has the same length (keyword EQUALS this word) it will be overwritten
            next.color = value;
            break;
          }

          node = next;
        }
      }
    }


    /// <summary>
    /// Creates a new instance of <see cref="LookupTable"/>
    /// </summary>
    public LookupTable(bool casesensitive)
    {
      this.casesensitive = casesensitive;
    }

    private class Node
    {
      public Node(object color, string word)
      {
        this.word = word;
        this.color = color;
      }

      public string word;
      public object color;

      public Node[] leaf = new Node[256];
    }
  }
}
