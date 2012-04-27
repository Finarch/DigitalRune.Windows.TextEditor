using System;
using System.IO;
using System.Text;

namespace DigitalRune.Windows.TextEditor.TextBuffer
{
  /// <summary>
  /// Simple implementation of the ITextBuffer interface implemented using a
  /// string. (Only for fall-back purposes.)
  /// </summary>
  public class StringTextBufferStrategy : ITextBufferStrategy
  {
    string storedText = "";


    /// <summary>
    /// Gets the length of the sequence of characters.
    /// </summary>
    /// <value>
    /// The current length of the sequence of characters that can be edited.
    /// </value>
    public int Length
    {
      get { return storedText.Length; }
    }


    /// <summary>
    /// Inserts a string of characters into the sequence.
    /// </summary>
    /// <param name="offset">Offset where to insert the string.</param>
    /// <param name="text">Text to be inserted.</param>
    public void Insert(int offset, string text)
    {
      if (text != null)
        storedText = storedText.Insert(offset, text);
    }


    /// <summary>
    /// Removes some portion of the sequence.
    /// </summary>
    /// <param name="offset">Offset of the remove.</param>
    /// <param name="length">Number of characters to remove.</param>
    public void Remove(int offset, int length)
    {
      storedText = storedText.Remove(offset, length);
    }


    /// <summary>
    /// Replace some portion of the sequence.
    /// </summary>
    /// <param name="offset">Offset.</param>
    /// <param name="length">Number of characters to replace.</param>
    /// <param name="text">Text to be replaced with.</param>
    public void Replace(int offset, int length, string text)
    {
      Remove(offset, length);
      Insert(offset, text);
    }


    /// <summary>
    /// Fetches a string of characters contained in the sequence.
    /// </summary>
    /// <param name="offset">Offset into the sequence to fetch</param>
    /// <param name="length">Number of characters to copy.</param>
    /// <returns>The string at the specified offset.</returns>
    public string GetText(int offset, int length)
    {
      if (length == 0)
        return "";

      if (offset == 0 && length >= storedText.Length)
        return storedText;

      return storedText.Substring(offset, Math.Min(length, storedText.Length - offset));
    }


    /// <summary>
    /// Returns a specific character of the sequence.
    /// </summary>
    /// <param name="offset">Offset of the char to get.</param>
    /// <returns>The character at the specified offset.</returns>
    public char GetCharAt(int offset)
    {
      if (offset == Length)
        return '\0';

      return storedText[offset];
    }


    /// <summary>
    /// This method sets the stored content.
    /// </summary>
    /// <param name="text">The string that represents the character sequence.</param>
    public void SetContent(string text)
    {
      storedText = text;
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="StringTextBufferStrategy"/> class.
    /// </summary>
    public StringTextBufferStrategy()
    {
    }


    /// <summary>
    /// Creates an text buffer from the given file.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <returns>The text buffer.</returns>
    public static ITextBufferStrategy CreateTextBufferFromFile(string fileName)
    {
      if (!File.Exists(fileName))
        throw new FileNotFoundException(fileName);

      StringTextBufferStrategy s = new StringTextBufferStrategy();
      s.SetContent(Utilities.FileReader.ReadFileContent(fileName, Encoding.Default));
      return s;
    }
  }
}
