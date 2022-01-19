using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Shifter.Providers;

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

            Shift(docView, ShiftDirection.Down);
        }

        public static void Shift(DocumentView docView, ShiftDirection direction)
        {
            SnapshotPoint caretPosition = docView.TextView.Caret.Position.BufferPosition;
            SnapshotSpan selection = GetSelectedSpan(docView.TextView, caretPosition);

            string text = selection.GetText();
            int start = caretPosition - selection.Start;

            if (ShiftEngine.TryShift(text, start, direction, out ShiftResult result))
            {
                // Update text buffer
                Span span = new(result.Start + selection.Start, result.Length);
                ITextSnapshot snapshot = docView.TextBuffer.Replace(span, result.ShiftedText);

                // Maintain caret position
                Span newSpan = new(result.Start + selection.Start, result.ShiftedText.Length);
                int endPosition = newSpan.Contains(caretPosition) ? caretPosition : newSpan.End;
                SnapshotPoint point = new(snapshot, endPosition);
                docView.TextView.Caret.MoveTo(point);
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
