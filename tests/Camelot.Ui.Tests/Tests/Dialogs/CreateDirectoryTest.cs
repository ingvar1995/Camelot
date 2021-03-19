using System;
using System.Linq;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.VisualTree;
using Camelot.Views.Dialogs;
using Xunit;

namespace Camelot.Ui.Tests.Tests.Dialogs
{
    public class CreateDirectoryTest : IUiTest, IDisposable
    {
        private CreateDirectoryDialog _dialog;

        public void Execute(IClassicDesktopStyleApplicationLifetime app)
        {
            var window = app.MainWindow;

            Keyboard.PressKey(window, Key.Tab);
            Keyboard.PressKey(window, Key.Down);
            Keyboard.PressKey(window, Key.F7);

            _dialog = app.Windows.OfType<CreateDirectoryDialog>().SingleOrDefault();
            Assert.NotNull(_dialog);

            var createButton = _dialog
                .GetVisualDescendants()
                .OfType<Button>()
                .SingleOrDefault(b => !b.Classes.Contains("transparentDialogButton"));

            Assert.NotNull(createButton);
            Assert.False(createButton.Command.CanExecute(null));
            // Keyboard.PressKey(_dialog, Key.K);
            // Assert.True(createButton.Command.CanExecute(null));
            // Keyboard.PressKey(_dialog, Key.Enter);
        }

        public void Dispose() => _dialog?.Close();
    }
}