using Shifter.Providers;

namespace Shifter
{
    [Command(PackageIds.ShiftUp)]
    internal sealed class ShiftUpCommand : BaseCommand<ShiftUpCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            DocumentView docView = await VS.Documents.GetActiveDocumentViewAsync();

            if (docView?.TextView == null)
            {
                return;
            }

            ShiftDownCommand.Shift(docView, ShiftDirection.Up);
        }
    }
}
