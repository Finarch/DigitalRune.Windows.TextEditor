using System;
using System.Collections.Generic;
using DigitalRune.Windows.TextEditor.Document;

namespace DigitalRune.Windows.TextEditor.Bookmarks
{
  /// <summary>
  /// A factory object for creating bookmarks.
  /// </summary>
  public interface IBookmarkFactory
  {
    /// <summary>
    /// Creates a bookmark.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="lineNumber">The line number.</param>
    /// <returns>The bookmark.</returns>
    Bookmark CreateBookmark(IDocument document, int lineNumber);
  }


  /// <summary>
  /// This class handles the bookmarks for a buffer.
  /// </summary>
  public class BookmarkManager
  {
    IDocument document;
    List<Bookmark> bookmark = new List<Bookmark>();


    /// <summary>
    /// Gets the bookmarks.
    /// </summary>
    /// <value>The bookmarks.</value>
    public List<Bookmark> Marks
    {
      get { return bookmark; }
    }


    /// <summary>
    /// Gets the document.
    /// </summary>
    /// <value>The document.</value>
    public IDocument Document
    {
      get { return document; }
    }


    /// <summary>
    /// Creates a new instance of <see cref="BookmarkManager"/>
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="lineTracker">The line tracker.</param>
    internal BookmarkManager(IDocument document, LineManager lineTracker)
    {
      this.document = document;
      lineTracker.LineDeleted += delegate(object sender, LineEventArgs e)
      {
        for (int i = 0; i < bookmark.Count; i++)
        {
          Bookmark b = bookmark[i];
          if (b.Line == e.LineSegment)
          {
            bookmark.RemoveAt(i--);
            OnRemoved(new BookmarkEventArgs(b));
          }
        }
      };
    }


    IBookmarkFactory factory;


    /// <summary>
    /// Gets or sets the bookmark factory.
    /// </summary>
    /// <value>The bookmark factory.</value>
    public IBookmarkFactory Factory
    {
      get { return factory; }
      set { factory = value; }
    }


    /// <summary>
    /// Sets the mark at the line <c>lineNr</c> if it is not set, if the
    /// line is already marked the mark is cleared.
    /// </summary>
    /// <param name="lineNr">The line number.</param>
    public void ToggleMarkAt(int lineNr)
    {
      Bookmark newMark;
      if (factory != null)
        newMark = factory.CreateBookmark(document, lineNr);
      else
        newMark = new Bookmark(document, lineNr);

      Type newMarkType = newMark.GetType();

      for (int i = 0; i < bookmark.Count; ++i)
      {
        Bookmark mark = bookmark[i];

        if (mark.LineNumber == lineNr && mark.CanToggle && mark.GetType() == newMarkType)
        {
          bookmark.RemoveAt(i);
          OnRemoved(new BookmarkEventArgs(mark));
          return;
        }
      }

      bookmark.Add(newMark);
      OnAdded(new BookmarkEventArgs(newMark));
    }


    /// <summary>
    /// Adds a bookmark.
    /// </summary>
    /// <param name="mark">The bookmark.</param>
    public void AddMark(Bookmark mark)
    {
      bookmark.Add(mark);
      OnAdded(new BookmarkEventArgs(mark));
    }


    /// <summary>
    /// Removes the bookmark.
    /// </summary>
    /// <param name="mark">The bookmark.</param>
    public void RemoveMark(Bookmark mark)
    {
      bookmark.Remove(mark);
      OnRemoved(new BookmarkEventArgs(mark));
    }


    /// <summary>
    /// Removes the bookmarks that match a certain criteria.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    public void RemoveMarks(Predicate<Bookmark> predicate)
    {
      for (int i = 0; i < bookmark.Count; ++i)
      {
        Bookmark bm = bookmark[i];
        if (predicate(bm))
        {
          bookmark.RemoveAt(i--);
          OnRemoved(new BookmarkEventArgs(bm));
        }
      }
    }


    /// <summary>
    /// Determines whether the specified line has a bookmark.
    /// </summary>
    /// <param name="lineNr">The line number.</param>
    /// <returns>
    /// true, if a mark at <paramref name="lineNr"/> exists, otherwise false.
    /// </returns>
    public bool IsMarked(int lineNr)
    {
      for (int i = 0; i < bookmark.Count; ++i)
        if (bookmark[i].LineNumber == lineNr)
          return true;

      return false;
    }


    /// <summary>
    /// Clears all bookmarks.
    /// </summary>
    public void Clear()
    {
      foreach (Bookmark mark in bookmark)
        OnRemoved(new BookmarkEventArgs(mark));

      bookmark.Clear();
    }


    /// <summary>
    /// Gets the first bookmark that matches a certain criteria.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <returns>The first matching bookmark.</returns>
    public Bookmark GetFirstMark(Predicate<Bookmark> predicate)
    {
      if (bookmark.Count < 1)
        return null;

      Bookmark first = null;
      for (int i = 0; i < bookmark.Count; ++i)
        if (predicate(bookmark[i]) && bookmark[i].IsEnabled && (first == null || bookmark[i].LineNumber < first.LineNumber))
          first = bookmark[i];

      return first;
    }


    /// <summary>
    /// Gets the last bookmark that matches a certain criteria.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <returns>The last matching bookmark.</returns>
    public Bookmark GetLastMark(Predicate<Bookmark> predicate)
    {
      if (bookmark.Count < 1)
        return null;

      Bookmark last = null;
      for (int i = 0; i < bookmark.Count; ++i)
        if (predicate(bookmark[i]) && bookmark[i].IsEnabled && (last == null || bookmark[i].LineNumber > last.LineNumber))
          last = bookmark[i];

      return last;
    }


    /// <summary>
    /// Accepts any mark.
    /// </summary>
    /// <param name="mark">The mark.</param>
    /// <returns>Always <c>true</c>.</returns>
    public static bool AcceptAnyMarkPredicate(Bookmark mark)
    {
      return true;
    }


    /// <summary>
    /// Gets the next bookmark for a given line.
    /// </summary>
    /// <param name="curLineNr">The line number.</param>
    /// <returns>The next bookmark afer the given line.</returns>
    public Bookmark GetNextMark(int curLineNr)
    {
      return GetNextMark(curLineNr, AcceptAnyMarkPredicate);
    }


    /// <summary>
    /// Gets the next bookmark for a given line.
    /// </summary>
    /// <param name="curLineNr">The line number.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns>
    /// The next bookmark for the given line; if it does not exist it returns <see cref="GetFirstMark"/>.
    /// </returns>
    public Bookmark GetNextMark(int curLineNr, Predicate<Bookmark> predicate)
    {
      if (bookmark.Count == 0)
        return null;

      Bookmark next = GetFirstMark(predicate);
      foreach (Bookmark mark in bookmark)
        if (predicate(mark) && mark.IsEnabled && mark.LineNumber > curLineNr)
          if (mark.LineNumber < next.LineNumber || next.LineNumber <= curLineNr)
            next = mark;

      return next;
    }


    /// <summary>
    /// Gets the bookmark before a given line.
    /// </summary>
    /// <param name="curLineNr">The line number.</param>
    /// <returns>The previous bookmark.</returns>
    public Bookmark GetPrevMark(int curLineNr)
    {
      return GetPrevMark(curLineNr, AcceptAnyMarkPredicate);
    }


    /// <summary>
    /// Gets the bookmark before a given line.
    /// </summary>
    /// <param name="curLineNr">The line number.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns>
    /// The previous bookmark for the given line; if it does not exist it returns <see cref="GetLastMark"/>.
    /// </returns>
    public Bookmark GetPrevMark(int curLineNr, Predicate<Bookmark> predicate)
    {
      if (bookmark.Count == 0)
        return null;

      Bookmark prev = GetLastMark(predicate);

      foreach (Bookmark mark in bookmark)
        if (predicate(mark) && mark.IsEnabled && mark.LineNumber < curLineNr)
          if (mark.LineNumber > prev.LineNumber || prev.LineNumber >= curLineNr)
            prev = mark;

      return prev;
    }




    /// <summary>
    /// Raises the <see cref="Removed"/> event.
    /// </summary>
    /// <param name="e">The <see cref="DigitalRune.Windows.TextEditor.Bookmarks.BookmarkEventArgs"/> instance containing the event data.</param>
    protected virtual void OnRemoved(BookmarkEventArgs e)
    {
      if (Removed != null)
        Removed(this, e);
    }


    /// <summary>
    /// Raises the <see cref="Added"/> event.
    /// </summary>
    /// <param name="e">The <see cref="DigitalRune.Windows.TextEditor.Bookmarks.BookmarkEventArgs"/> instance containing the event data.</param>
    protected virtual void OnAdded(BookmarkEventArgs e)
    {
      if (Added != null)
        Added(this, e);
    }


    /// <summary>
    /// Occurs when bookmark is removed.
    /// </summary>
    public event EventHandler<BookmarkEventArgs> Removed;

    /// <summary>
    /// Occurs when bookmark is added.
    /// </summary>
    public event EventHandler<BookmarkEventArgs> Added;
  }
}
