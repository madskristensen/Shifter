using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Shifter.Providers;
using Selection = Microsoft.VisualStudio.Text.Selection;

namespace Shifter
{
    [Command(PackageIds.ShiftDown)]
    internal sealed class ShiftDownCommand : BaseCommand<ShiftDownCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            DocumentView docView = await VS.Documents.GetActiveDocumentViewAsync();

            if (docView?.TextView == null)
            {
                return;
            }

            Shift(docView, ShiftDirection.Down, false);
        }

        public static void Shift(DocumentView docView, ShiftDirection direction, bool incremental)
        {
            IWpfTextView textView = docView.TextView!;
            IMultiSelectionBroker multiSelectionBroker = textView.GetMultiSelectionBroker();
            if (multiSelectionBroker.HasMultipleSelections)
            {
                // Multiple caret support
                IReadOnlyList<Selection> selections = multiSelectionBroker.AllSelections;
                if (selections.Count == 0)
                {
                    return;
                }

                ITextSnapshot snapshot = docView.TextBuffer!.CurrentSnapshot;
                ShiftResult[] shiftResults = new ShiftResult[selections.Count];
                for (int i = 0; i < selections.Count; i++)
                {
                    Selection selection = selections[i];
                    VirtualSnapshotPoint selectionStart;
                    VirtualSnapshotPoint selectionEnd;
                    if (selection.IsEmpty)
                    {
                        selectionStart = selection.Extent.Start;
                        selectionEnd = selection.Extent.End;
                    }
                    else
                    {
                        selectionStart = selection.Start;
                        selectionEnd = selection.End;
                    }


                    int start = selectionStart.Position.Position;
                    int end = selectionEnd.Position.Position;
                    int length = end - start;
                    if (length == 0) continue;

                    string text = snapshot.GetText(start, length);
                    int selectionCaret = end - selection.InsertionPoint.Position.Position;

                    // Try to shift with each selection
                    if (ShiftEngine.TryShift(text, selectionCaret, direction, incremental ? i : null, out ShiftResult shiftResult))
                    {
                        shiftResults[i] = shiftResult;
                    }
                }

                // Continue only if any shift was made
                if (shiftResults.Any(static shiftResult => shiftResult != null))
                {
                    (Selection, ShiftResult)[] shifts = shiftResults
                        .Select<ShiftResult, (Selection Selection, ShiftResult ShiftResult)>((shiftResult, i) => (selections[i], shiftResult))
                        .Where(static pair => pair.ShiftResult != null)
                        // We need descending order to preserve positions after shift transformations
                        .OrderByDescending(static pair => pair.Selection.Start.Position.Position)
                        .ToArray();

                    ITextSnapshot updatedSnapshot = snapshot;
                    Selection[] newSelections = shifts
                        .Select(
                            pair =>
                            {
                                (Selection selection, ShiftResult shiftResult) = pair;
                                // Update text buffer
                                Span span = new(shiftResult.Start + selection.Start.Position, shiftResult.Length);
                                updatedSnapshot = updatedSnapshot.TextBuffer.Replace(span, shiftResult.ShiftedText);

                                // Save new caret position and selection
                                SnapshotSpan newSpan = new(updatedSnapshot, shiftResult.Start + selection.Start.Position, shiftResult.ShiftedText.Length);
                                SnapshotPoint newEndPosition = new(updatedSnapshot, newSpan.Contains(selection.InsertionPoint.Position.Position) ? selection.InsertionPoint.Position.Position : newSpan.End.Position);
                                Selection newSelection = new(newEndPosition, newSpan.Start, newSpan.End);
                                return newSelection;
                            })
                        .ToArray();

                    // Refresh selections
                    selections = multiSelectionBroker.AllSelections
                        // New selections are also ordered by position in descending order
                        .OrderByDescending(static selection => selection.Start.Position.Position)
                        .ToArray();
                    for (int i = 0; i < selections.Count; i++)
                    {
                        Selection selection = selections[i];
                        Selection newSelection = newSelections[i];
                        multiSelectionBroker.TryPerformActionOnSelection(
                            selection,
                            transformer =>
                            {
                                // Move selection to new position
                                transformer.MoveTo(newSelection.AnchorPoint, newSelection.ActivePoint, newSelection.InsertionPoint, newSelection.InsertionPointAffinity);
                            }, out _);
                    }
                }
            }
            else
            {
                SnapshotPoint caretPosition = textView.Caret.Position.BufferPosition;
                SnapshotSpan selection = GetSelectedSpan(textView, caretPosition);

                string text = selection.GetText();
                int start = caretPosition - selection.Start;

                if (ShiftEngine.TryShift(text, start, direction, null, out ShiftResult result))
                {
                    // Update text buffer
                    Span span = new(result.Start + selection.Start, result.Length);
                    ITextSnapshot snapshot = docView.TextBuffer!.Replace(span, result.ShiftedText);

                    // Maintain caret position
                    Span newSpan = new(result.Start + selection.Start, result.ShiftedText.Length);
                    int endPosition = newSpan.Contains(caretPosition) ? caretPosition : newSpan.End;
                    SnapshotPoint point = new(snapshot, endPosition);
                    textView.Caret.MoveTo(point);

                    // Select the new text
                    textView.Selection.Select(new SnapshotSpan(snapshot, span.Start, result.ShiftedText.Length), false);
                }
            }
        }

        private static SnapshotSpan GetSelectedSpan(ITextView view, int caretPosition)
        {
            if (view.Selection.IsEmpty)
            {
                return view.TextBuffer.CurrentSnapshot.GetLineFromPosition(caretPosition).Extent;
            }

            return view.Selection.SelectedSpans[0];
        }
    }
}
