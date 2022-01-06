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

            if (ShiftEngine.TryShift(text, caretPosition - selection.Start, direction, out ShiftResult result))
            {
                Span span = new(result.Start + selection.Start, result.Length);
                ITextSnapshot snapshot = docView.TextBuffer.Replace(span, result.ShiftedText);

                SnapshotPoint point = new(snapshot, caretPosition.Position);
                docView.TextView.Caret.MoveTo(point);

                docView.TextView.Selection.Select(new SnapshotSpan(snapshot, span.Start, result.ShiftedText.Length), false);
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
