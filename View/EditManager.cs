using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace WpfPlotDigitizer.NetFramework
{
	public class EditManager<TObject> : INotifyPropertyChanged
	{
		private EditManager()
		{
			UndoCommand = new RelayCommand(Undo, CanUndo);
			RedoCommand = new RelayCommand(Redo, CanRedo);
			GoToCommand = new RelayCommand<int>(GoTo, CanGoTo);
			EditCommand = new RelayCommand<(TObject, string)>(Edit, CanEdit);
		}

		public EditManager(TObject _object) : this()
		{
			ObjectList = new List<TObject>
			{
				_object,
			};
			TagList = new List<string>
			{
				"initialise",
			};
		}

		public int Index { get; private set; } = 0;
		public List<TObject> ObjectList { get; private set; } 
		public TObject CurrentObject => ObjectList[Index];
		public List<string> TagList { get; private set; }
		public string CurrentTag => TagList[Index];

		public RelayCommand UndoCommand { get; private set; }
		public RelayCommand RedoCommand { get; private set; }
		public RelayCommand<int> GoToCommand { get; private set; }
		public RelayCommand<(TObject obj, string tag)> EditCommand { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		
		private void Undo()
		{
			Index--;
			UndoCommand.RaiseCanExecuteChanged();
			RedoCommand.RaiseCanExecuteChanged();
		}
		private bool CanUndo()
		{
			return Index > 0;
		}
		private void Redo()
		{
			Index++;
			UndoCommand.RaiseCanExecuteChanged();
			RedoCommand.RaiseCanExecuteChanged();
		}
		private bool CanRedo()
		{
			return Index < ObjectList.Count - 1;
		}
		private void GoTo(int targetIndex)
		{
			Index = targetIndex;
			UndoCommand.RaiseCanExecuteChanged();
			RedoCommand.RaiseCanExecuteChanged();
		}
		private bool CanGoTo(int targetIndex)
		{
			return targetIndex >= 0 && targetIndex < TagList.Count;
		}

		private void Edit((TObject obj, string tag) edit)
		{
			ObjectList.RemoveRange(Index + 1, ObjectList.Count - Index - 1);
			ObjectList.Add(edit.obj);
			OnPropertyChanged(nameof(ObjectList));

			TagList.RemoveRange(Index + 1, TagList.Count - Index - 1);
			TagList.Add(edit.tag);
			OnPropertyChanged(nameof(TagList));

			Index++;
		}
		protected virtual bool CanEdit((TObject obj, string tag) arg)
		{
			return true;
		}
	}
}

