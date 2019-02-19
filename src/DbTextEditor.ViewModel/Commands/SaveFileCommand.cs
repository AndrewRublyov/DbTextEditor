using DbTextEditor.Shared;
using DbTextEditor.Shared.DataBinding;
using DbTextEditor.Shared.DataBinding.Interfaces;
using DbTextEditor.Shared.Exceptions;
using DbTextEditor.Shared.Storage;
using DbTextEditor.ViewModel.Interfaces;

namespace DbTextEditor.ViewModel.Commands
{
    public class SaveFileCommand : ICommand
    {
        private readonly IEditorViewModel _editorViewModel;

        public SaveFileCommand(IEditorViewModel editorViewModel)
        {
            _editorViewModel = editorViewModel;
        }

        public void Execute()
        {
            if (_editorViewModel.IsNewFile)
            {
                throw new BusinessLogicException("Can't save new file without path");
            }

            var model = new FileDto
            {
                FileName = _editorViewModel.Path,
                Contents = _editorViewModel.Contents
            };

            _editorViewModel.Save(model);
            _editorViewModel.IsModified.Value = false;

            CommandLogger.LogExecuted<IEditorViewModel, SaveFileCommand>();
        }
    }
}