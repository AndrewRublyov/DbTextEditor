﻿using System.Collections.ObjectModel;
using DbTextEditor.Shared;
using DbTextEditor.ViewModel.Commands;

namespace DbTextEditor.ViewModel
{
    public class MainViewModel
    {
        public ICommand NewFileCommand { get; }
        public ICommand<string> OpenFileCommand { get; }
        public ObservableCollection<EditorViewModel> OpenedEditors { get; } = new ObservableCollection<EditorViewModel>();

        public MainViewModel()
        {
            NewFileCommand = new NewFileCommand(this);
            OpenFileCommand = new OpenFileCommand(this);
        }
    }
}