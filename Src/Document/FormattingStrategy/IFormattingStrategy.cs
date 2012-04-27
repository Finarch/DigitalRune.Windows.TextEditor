using DigitalRune.Windows.TextEditor.Document;


namespace DigitalRune.Windows.TextEditor.Formatting
{
	/// <summary>
	/// This interface handles the auto and smart indenting and formatting
	/// in the document while you type. Language bindings could overwrite this 
	/// interface and define their own indentation/formatting.
	/// </summary>
	public interface IFormattingStrategy
	{
    /// <summary>
    /// This function formats a specific line after a character is typed.
    /// </summary>
    /// <param name="textArea">The text area.</param>
    /// <param name="line">The line.</param>
    /// <param name="caretOffset">The caret offset.</param>
    /// <param name="charTyped">The character typed.</param>
		void FormatLine(TextArea textArea, int line, int caretOffset, char charTyped);

    /// <summary>
    /// This function sets the indentation level in a specific line
    /// </summary>
    /// <param name="textArea">The text area.</param>
    /// <param name="line">The line.</param>
    /// <returns>The target caret position (length of new indentation).</returns>
    int IndentLine(TextArea textArea, int line);

    /// <summary>
    /// This function sets the indent level in of range of lines.
    /// </summary>
    /// <param name="textArea">The text area.</param>
    /// <param name="begin">The begin.</param>
    /// <param name="end">The end.</param>
		void IndentLines(TextArea textArea, int begin, int end);

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
		int SearchBracketBackward(IDocument document, int offset, char openBracket, char closingBracket);
		
		/// <summary>
		/// Finds the offset of the closing bracket in the block defined by offset skipping
		/// brackets in strings and comments.
		/// </summary>
		/// <param name="document">The document to search in.</param>
		/// <param name="offset">The offset of an position in the block or the offset of the opening bracket.</param>
		/// <param name="openBracket">The character for the opening bracket.</param>
		/// <param name="closingBracket">The character for the closing bracket.</param>
		/// <returns>Returns the offset of the closing bracket or -1 if no matching bracket was found.</returns>
		int SearchBracketForward(IDocument document, int offset, char openBracket, char closingBracket);
	}
}
