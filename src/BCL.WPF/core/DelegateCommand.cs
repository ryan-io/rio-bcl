// using System.Windows.Input;
//
// namespace BCL.WPF {
//     /// <summary>
//     ///  Wrapper for application commands
//     ///  CanExecute is dependent on a Func<object?, bool>, specified in ctor
//     ///  Execute is dependent on a Action<object?>, specified in ctor
//     /// </summary>
//     public class DelegateCommand : ICommand {
//         /// <summary>
//         ///  Wrapper for application commands
//         ///  CanExecute is dependent on a Func<object?, bool>, specified in ctor
//         ///  Execute is dependent on a Action<object?>, specified in ctor
//         /// </summary>
//         /// <summary>
//         ///  Ties into the CommandManager to verify if this command implementation can be raised
//         /// </summary>
//         public event EventHandler? CanExecuteChanged {
//             add { CommandManager.RequerySuggested += value; }
//             remove { CommandManager.RequerySuggested -= value; }
//         }
//
//         /// <summary>
//         ///  Raised after the command is 
//         /// </summary>
//         public event Action? OnExecutedCallback;
//
//         public void RaiseCanExecuteChanged () {
//             CommandManager.InvalidateRequerySuggested();
//         }
//
//         /// <summary>
//         /// Checks if _canExecute is null -> returns false
//         /// Used generically to check if this instance can actually invoke its context
//         /// </summary>
//         /// <param name="parameter">object to pass to _canExecute</param>
//         /// <returns>False if _canExecute is null, else result from _canExecute</returns>
//         public bool CanExecute(object? parameter) {
//             if (CanExecuteFunc == null)
//                 return true;
//
//             return CanExecuteFunc.Invoke(parameter);
//         }
//
//         /// <summary>
//         ///  Invokes the context action
//         /// </summary>
//         /// <param name="parameter">object to pass to action _context</param>
//         public virtual void Execute (object? parameter) {
//             ContextAction?.Invoke(parameter);
//             OnExecutedCallback?.Invoke();
//         }
//
//         public DelegateCommand(Action<object?>? context, Func<object?, bool>? canExecute = null) {
//             ContextAction = context;
//             CanExecuteFunc = canExecute;
//         }
//
//         protected readonly Action<object?>? ContextAction;
//         protected readonly Func<object?, bool>? CanExecuteFunc;
//     }
// }