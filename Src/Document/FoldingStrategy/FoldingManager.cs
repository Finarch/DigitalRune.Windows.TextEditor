using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DigitalRune.Windows.TextEditor.Document;


namespace DigitalRune.Windows.TextEditor.Folding
{
  /// <summary>
  /// Manages the folding (<see cref="FoldMarker"/>) of a text buffer.
  /// </summary>
  public class FoldingManager
  {
    List<FoldMarker> foldMarkers = new List<FoldMarker>();
    List<FoldMarker> foldMarkerByEnd = new List<FoldMarker>();
    IFoldingStrategy foldingStrategy = null;
    IDocument document;
    private List<FoldMarker> topLevelFoldings;


    /// <summary>
    /// Gets a list of all <see cref="FoldMarker"/>.
    /// </summary>
    /// <value>The fold marker.</value>
    public IList<FoldMarker> FoldMarker
    {
      get { return foldMarkers.AsReadOnly(); }
    }


    /// <summary>
    /// Gets or sets the folding strategy.
    /// </summary>
    /// <value>The folding strategy.</value>
    public IFoldingStrategy FoldingStrategy
    {
      get { return foldingStrategy; }
      set { foldingStrategy = value; }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="FoldingManager"/> class.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="lineTracker">The line tracker.</param>
    internal FoldingManager(IDocument document, LineManager lineTracker)
    {
      this.document = document;
      document.DocumentChanged += DocumentChanged;
    }


    void DocumentChanged(object sender, DocumentEventArgs e)
    {
      int oldCount = foldMarkers.Count;
      document.UpdateSegmentListOnDocumentChange(foldMarkers, e);
      if (oldCount != foldMarkers.Count)
        document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
    }


    /// <summary>
    /// Gets the foldings from position.
    /// </summary>
    /// <param name="line">The line.</param>
    /// <param name="column">The column.</param>
    /// <returns></returns>
    public List<FoldMarker> GetFoldingsFromPosition(int line, int column)
    {
      List<FoldMarker> foldings = new List<FoldMarker>();
      if (foldMarkers != null)
      {
        for (int i = 0; i < foldMarkers.Count; ++i)
        {
          FoldMarker fm = foldMarkers[i];
          if ((fm.StartLine == line && column > fm.StartColumn && !(fm.EndLine == line && column >= fm.EndColumn)) 
              || (fm.EndLine == line && column < fm.EndColumn && !(fm.StartLine == line && column <= fm.StartColumn)) 
              || (line > fm.StartLine && line < fm.EndLine))
          {
            foldings.Add(fm);
          }
        }
      }
      return foldings;
    }


    class StartComparer : IComparer<FoldMarker>
    {
      public readonly static StartComparer Instance = new StartComparer();

      public int Compare(FoldMarker x, FoldMarker y)
      {
        if (x.StartLine < y.StartLine)
          return -1;
        else if (x.StartLine == y.StartLine)
          return x.StartColumn.CompareTo(y.StartColumn);
        else
          return 1;
      }
    }


    class EndComparer : IComparer<FoldMarker>
    {
      public readonly static EndComparer Instance = new EndComparer();

      public int Compare(FoldMarker x, FoldMarker y)
      {
        if (x.EndLine < y.EndLine)
          return -1;
        else if (x.EndLine == y.EndLine)
          return x.EndColumn.CompareTo(y.EndColumn);
        else
          return 1;
      }
    }


    List<FoldMarker> GetFoldingsByStartAfterColumn(int lineNumber, int column, bool forceFolded)
    {
      List<FoldMarker> foldings = new List<FoldMarker>();

      if (foldMarkers != null)
      {
        FoldMarker reference = new FoldMarker(document, lineNumber, column, lineNumber, column);
        int index = foldMarkers.BinarySearch(reference, StartComparer.Instance);

        if (index < 0) 
          index = ~index;

        for (; index < foldMarkers.Count; index++)
        {
          FoldMarker fm = foldMarkers[index];

          if (fm.StartLine < lineNumber)
            continue;
          else if (fm.StartLine > lineNumber)
            break;

          if (fm.StartColumn <= column)
            continue;

          if (!forceFolded || fm.IsFolded)
            foldings.Add(fm);
        }
      }
      return foldings;
    }


    /// <summary>
    /// Gets the foldings that start at certain line.
    /// </summary>
    /// <param name="lineNumber">The line number.</param>
    /// <returns>The foldings that start <paramref name="lineNumber"/>.</returns>
    public List<FoldMarker> GetFoldingsWithStart(int lineNumber)
    {
      return GetFoldingsByStartAfterColumn(lineNumber, -1, false);
    }


    /// <summary>
    /// Gets the folded foldings that start at certain line.
    /// </summary>
    /// <param name="lineNumber">The line number.</param>
    /// <returns>The folded foldings that start at <paramref name="lineNumber"/>.</returns>
    public List<FoldMarker> GetFoldedFoldingsWithStart(int lineNumber)
    {
      return GetFoldingsByStartAfterColumn(lineNumber, -1, true);
    }


    /// <summary>
    /// Gets the folded foldings that start after a certain column.
    /// </summary>
    /// <param name="lineNumber">The line number.</param>
    /// <param name="column">The column.</param>
    /// <returns>The folded foldings that start in <paramref name="lineNumber"/> somewhere after <paramref name="column"/>.</returns>
    public List<FoldMarker> GetFoldedFoldingsWithStartAfterColumn(int lineNumber, int column)
    {
      return GetFoldingsByStartAfterColumn(lineNumber, column, true);
    }


    List<FoldMarker> GetFoldingsByEndAfterColumn(int lineNumber, int column, bool forceFolded)
    {
      List<FoldMarker> foldings = new List<FoldMarker>();

      if (foldMarkers != null)
      {
        FoldMarker reference = new FoldMarker(document, lineNumber, column, lineNumber, column);
        int index = foldMarkerByEnd.BinarySearch(reference, EndComparer.Instance);
        if (index < 0) index = ~index;

        for (; index < foldMarkerByEnd.Count; index++)
        {
          FoldMarker fm = foldMarkerByEnd[index];

          if (fm.EndLine < lineNumber)
            continue;
          else if (fm.EndLine > lineNumber)
            break;

          if (fm.EndColumn <= column)
            continue;

          if (!forceFolded || fm.IsFolded)
            foldings.Add(fm);
        }
      }
      return foldings;
    }


    /// <summary>
    /// Gets the foldings that ends in a certain line.
    /// </summary>
    /// <param name="lineNumber">The line number.</param>
    /// <returns>The foldings that end at <paramref name="lineNumber"/>.</returns>
    public List<FoldMarker> GetFoldingsWithEnd(int lineNumber)
    {
      return GetFoldingsByEndAfterColumn(lineNumber, -1, false);
    }


    /// <summary>
    /// Gets the folded foldings that end at certain line.
    /// </summary>
    /// <param name="lineNumber">The line number.</param>
    /// <returns>The folded foldings that end at <paramref name="lineNumber"/>.</returns>
    public List<FoldMarker> GetFoldedFoldingsWithEnd(int lineNumber)
    {
      return GetFoldingsByEndAfterColumn(lineNumber, -1, true);
    }


    /// <summary>
    /// Determines whether any foldings start at the line.
    /// </summary>
    /// <param name="lineNumber">The line number.</param>
    /// <returns>
    /// 	<c>true</c> if any foldings start at <paramref name="lineNumber"/>; otherwise, <c>false</c>.
    /// </returns>
    public bool IsFoldStart(int lineNumber)
    {
      return GetFoldingsWithStart(lineNumber).Count > 0;
    }


    /// <summary>
    /// Determines whether any foldings end at the line.
    /// </summary>
    /// <param name="lineNumber">The line number.</param>
    /// <returns>
    /// 	<c>true</c> if any foldings end at <paramref name="lineNumber"/>; otherwise, <c>false</c>.
    /// </returns>
    public bool IsFoldEnd(int lineNumber)
    {
      return GetFoldingsWithEnd(lineNumber).Count > 0;
    }


    /// <summary>
    /// Gets the foldings that contain a certain line number (excluding start and end line).
    /// </summary>
    /// <param name="lineNumber">The line number.</param>
    /// <returns>The foldings that contain a certain line number.</returns>
    public List<FoldMarker> GetFoldingsContainsLineNumber(int lineNumber)
    {
      List<FoldMarker> foldings = new List<FoldMarker>();
      if (foldMarkers != null)
      {
        foreach (FoldMarker fm in foldMarkers)
        {
          if (fm.StartLine < lineNumber)
          {
            if (lineNumber < fm.EndLine)
              foldings.Add(fm);
          }
          else
          {
            break;
          }
        }
      }
      return foldings;
    }


    /// <summary>
    /// Determines whether a line belongs to any folding.
    /// </summary>
    /// <param name="lineNumber">The line number.</param>
    /// <returns>
    /// 	<c>true</c> if <paramref name="lineNumber"/> belongs to any folding; otherwise, <c>false</c>.
    /// </returns>
    public bool IsBetweenFolding(int lineNumber)
    {
      if (foldMarkers != null)
      {
        foreach (FoldMarker fm in foldMarkers)
          if (fm.StartLine < lineNumber && lineNumber < fm.EndLine)
            return true;
      }
      return false;
    }


    /// <summary>
    /// Determines whether a line is visible (not folded).
    /// </summary>
    /// <param name="lineNumber">The line number.</param>
    /// <returns>
    /// 	<c>true</c> if <paramref name="lineNumber"/> is visible; otherwise, <c>false</c>.
    /// </returns>
    public bool IsLineVisible(int lineNumber)
    {
      foreach (FoldMarker fm in GetTopLevelFoldedFoldings())
        if (fm.StartLine < lineNumber && lineNumber < fm.EndLine && fm.IsFolded)
          return false;

      return true;
    }


    /// <summary>
    /// Gets the top level folded foldings.
    /// </summary>
    /// <returns>The top level folded foldings.</returns>
    public ReadOnlyCollection<FoldMarker> GetTopLevelFoldedFoldings()
    {
      if (topLevelFoldings != null)
        return topLevelFoldings.AsReadOnly();

      topLevelFoldings = new List<FoldMarker>();
      if (foldMarkers != null)
      {
        TextLocation end = new TextLocation(0, 0);
        foreach (FoldMarker fm in foldMarkers)
        {
          if (fm.IsFolded && (fm.StartLine > end.Line || fm.StartLine == end.Line && fm.StartColumn >= end.Column))
          {
            topLevelFoldings.Add(fm);
            end = new TextLocation(fm.EndColumn, fm.EndLine);
          }
        }
      }
      return topLevelFoldings.AsReadOnly();
    }


    internal void ClearCache()
    {
      topLevelFoldings = null; 
    }


    /// <summary>
    /// Updates the foldings.
    /// </summary>
    public void UpdateFoldings()
    {
      UpdateFoldings(null, null);
    }


    /// <summary>
    /// Updates the foldings.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="parseInfo">The parse info.</param>
    public void UpdateFoldings(string fileName, object parseInfo)
    {
      if (foldingStrategy == null || !document.TextEditorProperties.EnableFolding)
        return;

      UpdateFoldings(foldingStrategy.GenerateFoldMarkers(document, fileName, parseInfo));
    }


    /// <summary>
    /// Updates the foldings.
    /// </summary>
    /// <param name="newFoldings">The new foldings.</param>
    public void UpdateFoldings(List<FoldMarker> newFoldings)
    {
      int oldFoldingsCount = foldMarkers.Count;
      lock (this)
      {
        ClearCache();
        if (newFoldings != null && newFoldings.Count != 0)
        {
          newFoldings.Sort();
          if (foldMarkers.Count == newFoldings.Count)
          {
            for (int i = 0; i < foldMarkers.Count; ++i)
            {
              newFoldings[i].IsFolded = foldMarkers[i].IsFolded;
            }
            foldMarkers = newFoldings;
          }
          else
          {
            for (int i = 0, j = 0; i < foldMarkers.Count && j < newFoldings.Count; )
            {
              int n = newFoldings[j].CompareTo(foldMarkers[i]);
              if (n > 0)
              {
                ++i;
              }
              else
              {
                if (n == 0)
                  newFoldings[j].IsFolded = foldMarkers[i].IsFolded;

                ++j;
              }
            }
          }
        }
        if (newFoldings != null)
        {
          foldMarkers = newFoldings;
          foldMarkerByEnd = new List<FoldMarker>(newFoldings);
          foldMarkerByEnd.Sort(EndComparer.Instance);
        }
        else
        {
          foldMarkers.Clear();
          foldMarkerByEnd.Clear();
        }
      }
      if (oldFoldingsCount != foldMarkers.Count)
      {
        document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
        document.CommitUpdate();
      }
    }


    //public string SerializeToString()
    //{
    //  StringBuilder sb = new StringBuilder();
    //  foreach (FoldMarker marker in this.foldMarkers)
    //  {
    //    sb.Append(marker.Offset); sb.Append("\n");
    //    sb.Append(marker.Length); sb.Append("\n");
    //    sb.Append(marker.FoldText); sb.Append("\n");
    //    sb.Append(marker.IsFolded); sb.Append("\n");
    //  }
    //  return sb.ToString();
    //}


    //public void DeserializeFromString(string str)
    //{
    //  try
    //  {
    //    string[] lines = str.Split('\n');
    //    for (int i = 0; i < lines.Length && lines[i].Length > 0; i += 4)
    //    {
    //      int offset = Int32.Parse(lines[i]);
    //      int length = Int32.Parse(lines[i + 1]);
    //      string text = lines[i + 2];
    //      bool isFolded = Boolean.Parse(lines[i + 3]);
    //      bool found = false;
    //      foreach (FoldMarker marker in foldMarkers)
    //      {
    //        if (marker.Offset == offset && marker.Length == length)
    //        {
    //          marker.IsFolded = isFolded;
    //          found = true;
    //          break;
    //        }
    //      }
    //      if (!found)
    //      {
    //        foldMarkers.Add(new FoldMarker(document, offset, length, text, isFolded));
    //      }
    //    }
    //    if (lines.Length > 0)
    //    {
    //      NotifyFoldingsChanged(EventArgs.Empty);
    //    }
    //  }
    //  catch (Exception)
    //  {
    //  }
    //}


    /// <summary>
    /// Raises the <see cref="FoldingsChanged"/> event.
    /// </summary>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    public void NotifyFoldingsChanged(EventArgs e)
    {
      ClearCache();

      if (FoldingsChanged != null)
        FoldingsChanged(this, e);
    }


    /// <summary>
    /// Occurs when the foldings have been changed.
    /// </summary>
    public event EventHandler FoldingsChanged;
  }
}
