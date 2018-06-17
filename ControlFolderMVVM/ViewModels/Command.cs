using System;
using System.Windows.Input;

namespace ControlFolderMVVM.ViewModels
{
    class Command : ICommand
    {
        public event EventHandler CanExecuteChanged; 
        private readonly Action<object> execute; 
        private Func<object, bool> canExecute; 

        public void OneExecuteChaneged() 
        {
            CanExecuteChanged.Invoke(this, new EventArgs());
        }

        public bool CanExecute(object parameter)  
        {
            return this.canExecute == null || this.canExecute(parameter); 
        }


        public Command(Action<object> execute, Func<object, bool> canExecute = null) 
        {
            this.execute = execute; 
            this.canExecute = canExecute; 
        }

        public void Execute(object parameter) 
        {
            this.execute(parameter);
        }
    }
}
