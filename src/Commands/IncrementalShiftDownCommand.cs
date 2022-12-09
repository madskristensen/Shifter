using Shifter.Providers;

namespace Shifter
{
    [Command(PackageIds.IncrementalShiftDown)]
    internal sealed class IncrementalShiftDownCommand : BaseCommand<IncrementalShiftDownCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            DocumentView docView = await VS.Documents.GetActiveDocumentViewAsync();

            if (docView?.TextView == null)
            {
                return;
            }

            ShiftDownCommand.Shift(docView, ShiftDirection.Down, true);
        }
    }
}