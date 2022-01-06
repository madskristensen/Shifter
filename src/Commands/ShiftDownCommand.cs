using Microsoft.VisualStudio.Text;
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
            SnapshotPoint position = docView.TextView.Caret.Position.BufferPosition;
            ITextSnapshotLine line = docView.TextBuffer.CurrentSnapshot.GetLineFromPosition(position);

            string text = line.GetText();

            if (ShiftEngine.Parse(text, position - line.Start, direction, out ShiftResult result))
            {
                Span span = new(result.Start + line.Start, result.Length);
                ITextSnapshot snapshot = docView.TextBuffer.Replace(span, result.ShiftedText);

                SnapshotPoint point = new(snapshot, position.Position);
                docView.TextView.Caret.MoveTo(point);

                docView.TextView.Selection.Select(new SnapshotSpan(snapshot, span.Start, result.ShiftedText.Length), false);
            }
        }
    }
}
