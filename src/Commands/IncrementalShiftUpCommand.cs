using Shifter.Providers;

namespace Shifter
{
    [Command(PackageIds.IncrementalShiftUp)]
    internal sealed class IncrementalShiftUpCommand : BaseCommand<IncrementalShiftUpCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            DocumentView docView = await VS.Documents.GetActiveDocumentViewAsync();

            if (docView?.TextView == null)
            {
                return;
            }

            ShiftDownCommand.Shift(docView, ShiftDirection.Up, true);
        }
    }
}