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

            SnapshotPoint position = docView.TextView.Caret.Position.BufferPosition;
            ITextSnapshotLine line = docView.TextBuffer.CurrentSnapshot.GetLineFromPosition(position);

            string text = line.GetText();

            if (ShiftEngine.Parse(text, position - line.Start, ShiftDirection.Down, out ShiftResult result))
            {
                Span span = new(result.Start + line.Start, result.Length);
                docView.TextBuffer.Replace(span, result.ShiftedText);
                //docView.TextView.Caret.MoveTo(position);
            }
        }
    }
}
