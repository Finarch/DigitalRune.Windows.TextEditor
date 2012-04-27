using System;
using System.Collections.Generic;
using System.Drawing;
using DigitalRune.Windows.TextEditor.Document;

namespace DigitalRune.Windows.TextEditor.Markers
{
  /// <summary>
  /// Manages the list of markers and provides ways to retrieve markers for 
  /// specific positions.
  /// </summary>
  public sealed class MarkerStrategy
  {
    List<TextMarker> textMarker = new List<TextMarker>();
    IDocument document;


    /// <summary>
    /// Gets the document.
    /// </summary>
    /// <value>The document.</value>
    public IDocument Document
    {
      get { return document; }
    }


    /// <summary>
    /// Gets the text markers.
    /// </summary>
    /// <value>The text markers.</value>
    public IEnumerable<TextMarker> TextMarker
    {
      get { return textMarker.AsReadOnly(); }
    }


    /// <summary>
    /// Adds a text marker.
    /// </summary>
    /// <param name="item">The text marker.</param>
    public void AddMarker(TextMarker item)
    {
      markersTable.Clear();
      textMarker.Add(item);
    }


    /// <summary>
    /// Inserts a text marker.
    /// </summary>
    /// <param name="index">The index at which to insert the marker.</param>
    /// <param name="item">The text marker.</param>
    public void InsertMarker(int index, TextMarker item)
    {
      markersTable.Clear();
      textMarker.Insert(index, item);
    }


    /// <summary>
    /// Removes a text marker.
    /// </summary>
    /// <param name="item">The text marker.</param>
    public void RemoveMarker(TextMarker item)
    {
      markersTable.Clear();
      textMarker.Remove(item);
    }


    /// <summary>
    /// Removes all text markers.
    /// </summary>
    public void Clear()
    {
      markersTable.Clear();
      textMarker.Clear();
    }


    /// <summary>
    /// Removes all text markers that match a given criteria.
    /// </summary>
    /// <param name="match">The match.</param>
    public void RemoveAll(Predicate<TextMarker> match)
    {
      markersTable.Clear();
      textMarker.RemoveAll(match);
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="MarkerStrategy"/> class.
    /// </summary>
    /// <param name="document">The document.</param>
    public MarkerStrategy(IDocument document)
    {
      this.document = document;
      document.DocumentChanged += DocumentChanged;
    }


    // Cache that stores: (key, value) = (offset, lift of markers at offset)
    Dictionary<int, List<TextMarker>> markersTable = new Dictionary<int, List<TextMarker>>();


    /// <summary>
    /// Retrieves the text markers that contain a specific offset.
    /// </summary>
    /// <param name="offset">The offset in the document.</param>
    /// <returns>
    /// The text marker at <paramref name="offset"/>. Returns an empty list if
    /// no marker contains the offset.
    /// </returns>
    public List<TextMarker> GetMarkers(int offset)
    {
      if (!markersTable.ContainsKey(offset))
      {
        List<TextMarker> markers = new List<TextMarker>();
        for (int i = 0; i < textMarker.Count; ++i)
        {
          TextMarker marker = (TextMarker) textMarker[i];
          if (marker.Offset <= offset && offset <= marker.EndOffset)
          {
            markers.Add(marker);
          }
        }
        markersTable[offset] = markers;
      }
      return markersTable[offset];
    }


    /// <summary>
    /// Retrieves all text markers in a given region.
    /// </summary>
    /// <param name="offset">The offset.</param>
    /// <param name="length">The length.</param>
    /// <returns>A list of all text markers in this region.</returns>
    public List<TextMarker> GetMarkers(int offset, int length)
    {
      int endOffset = offset + length - 1;
      List<TextMarker> markers = new List<TextMarker>();
      for (int i = 0; i < textMarker.Count; ++i)
      {
        TextMarker marker = (TextMarker) textMarker[i];

        if (marker.Offset <= offset && offset <= marker.EndOffset           // start in marker region
            || marker.Offset <= endOffset && endOffset <= marker.EndOffset  // end in marker region
            || offset <= marker.Offset && marker.Offset <= endOffset        // marker start in region
            || offset <= marker.EndOffset && marker.EndOffset <= endOffset) // marker end in region
        {
          markers.Add(marker);
        }
      }
      return markers;
    }


    /// <summary>
    /// Retrieves a list of all text markers at a given position.
    /// </summary>
    /// <param name="position">The position in the document.</param>
    /// <returns>A list of all text markers at <paramref name="position"/>.</returns>
    public List<TextMarker> GetMarkers(TextLocation position)
    {
      if (position.Y >= document.TotalNumberOfLines || position.Y < 0)
        return new List<TextMarker>();

      LineSegment segment = document.GetLineSegment(position.Y);
      return GetMarkers(segment.Offset + position.X);
    }


    void DocumentChanged(object sender, DocumentEventArgs e)
    {
      // reset markers table
      markersTable.Clear();
      document.UpdateSegmentListOnDocumentChange(textMarker, e);
    }
  }
}
