using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
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

            await ShiftAsync(docView, ShiftDirection.Down, false);
        }

        public static async Task ShiftAsync(DocumentView docView, ShiftDirection direction, bool incremental)
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
                        // We need ascending order to preserve selections after shift transformations, we'll calculate cumulative difference
                        .OrderBy(static pair => pair.Selection.Start.Position.Position)
                        .ToArray();

                    ITextBuffer buffer = textView.TextBuffer;
                    string undoText = (direction, incremental) switch
                    {
                        (ShiftDirection.Up, false)   => "Shift up",
                        (ShiftDirection.Down, false) => "Shift down",
                        (ShiftDirection.Up, true)    => "Incremental Shift up",
                        (ShiftDirection.Down, true)  => "Incremental Shift down",
                        _                            => "Shifter"
                    };

                    using ITextUndoTransaction undo = await buffer.OpenUndoContextAsync(undoText);
                    Selection[] newSelections;
                    using (ITextEdit edit = buffer.CreateEdit())
                    {
                        int diff = 0;

                        // Collect positions, we have to create new snapshot after all changes are applied with single edit
                        List<(int InsertionPoint, Span Span)> newSelectionPositions = shifts
                            .Select(
                                pair =>
                                {
                                    (Selection selection, ShiftResult shiftResult) = pair;

                                    // Update text buffer
                                    Span span = new(shiftResult.Start + selection.Start.Position, shiftResult.Length);
                                    edit.Replace(span, shiftResult.ShiftedText);

                                    Span newSpan = new(diff + shiftResult.Start + selection.Start.Position, shiftResult.ShiftedText.Length);

                                    // Cumulate total difference so far so spans will match the new ones
                                    diff += shiftResult.ShiftedText.Length - span.Length;

                                    // Try to remain insertion point
                                    int insertionPoint = newSpan.Contains(selection.InsertionPoint.Position.Position + diff)
                                        ? selection.InsertionPoint.Position.Position
                                        : newSpan.End;
                                    return (insertionPoint, newSpan);
                                })
                            .ToList();

                        ITextSnapshot updatedSnapshot = edit.Apply();

                        // Create new selections from calculated new positions
                        newSelections = new Selection[newSelectionPositions.Count];
                        for (int i = 0; i < newSelectionPositions.Count; i++)
                        {
                            (int insertionPoint, Span span) = newSelectionPositions[i];
                            SnapshotSpan newSpan = new(updatedSnapshot, span);
                            SnapshotPoint newEndPosition = new(updatedSnapshot, insertionPoint);
                            Selection newSelection = new(newEndPosition, newSpan.Start, newSpan.End);
                            newSelections[i] = newSelection;
                        }
                    }

                    using (multiSelectionBroker.BeginBatchOperation())
                    {
                        // Refresh selections
                        selections = multiSelectionBroker.AllSelections
                        // New selections are also ordered by position in ascending order
                        .OrderBy(static selection => selection.Start.Position.Position)
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

                    // Safe to history as single named edit
                    undo.Complete();
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
