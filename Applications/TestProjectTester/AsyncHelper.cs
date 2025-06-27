using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestProjectTester.Localization;

namespace TestProjectTester
{
	/// <summary>
	/// Classe helper per il lancio di metodi asincroni in modo sincrono
	/// </summary>
	/// <remarks>https://cpratt.co/async-tips-tricks/</remarks>
	public static class AsyncHelper
	{

		#region Proprietà

		private static readonly TaskFactory _taskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

		#endregion

		#region Metodi

		/// <summary>
		/// Esegue in maniera sincrona una funzione asincrona e ne ritorna il risultato.
		/// <para/>L'eventuale eccezione viene unwrappata
		/// </summary>
		/// <typeparam name="TResult">Tipo di risultato</typeparam>
		/// <param name="func">Funzione asincrona</param>
		/// <returns></returns>
		public static TResult RunSync<TResult>(Func<Task<TResult>> func)
		{
			var t1 = _taskFactory.StartNew(func);
			var t2 = t1.Unwrap();
			var t3 = t2.GetAwaiter();
			return t3.GetResult();
		}

		/// <summary>
		/// Esegue in maniera sincrona un metodo asincrono.
		/// <para/>L'eventuale eccezione viene unwrappata
		/// </summary>
		/// <param name="func">Metodo asincrono</param>
		public static void RunSync(Func<Task> func) => _taskFactory.StartNew(func).Unwrap().GetAwaiter().GetResult();

		/// <summary>
		/// Avvia un nuovo task per eseguire l'azione specificata e, se l'azione non si conclude nel tempo previsto, lancia un'eccezione
		/// </summary>
		/// <param name="action"></param>
		/// <param name="timeout">Tempo massimo di attesa dopo il quale verrà lanciato un errore</param>
		/// <exception cref="TimeoutException">Se il task non ha ritornato un risultato prima del <paramref name="timeout"/></exception>
		public static void RunInTaskWithTimeout(this Action action, TimeSpan timeout)
		{
			RunSync(() => Task.Run(action).WaitWithTimeoutAsync(timeout));
		}

		/// <summary>
		/// Avvia un nuovo task per eseguire la funzione specificata e ritornare il valore, se la funzione non ritorna un valore nel tempo previsto, lancia un'eccezione
		/// </summary>
		/// <typeparam name="TResult">Tipo del valore di ritorno</typeparam>
		/// <param name="function"></param>
		/// <param name="timeout">Tempo massimo di attesa dopo il quale verrà lanciato un errore</param>
		/// <returns></returns>
		/// <exception cref="TimeoutException">Se il task non ha ritornato un risultato prima del <paramref name="timeout"/></exception>
		public static TResult RunInTaskWithTimeout<TResult>(this Func<TResult> function, TimeSpan timeout)
		{
			return RunSync(() => Task.Run(function).WaitForResultWithTimeoutAsync(timeout));
		}

		/// <summary>
		/// Attende la conclusione del task per un tempo prestabilito, dopodiché viene lanciato un errore
		/// </summary>
		/// <param name="task"></param>
		/// <param name="timeout">Tempo massimo di attesa dopo il quale verrà lanciato un errore</param>
		/// <returns></returns>
		/// <exception cref="TimeoutException">Se il task non ha ritornato un risultato prima del <paramref name="timeout"/></exception>
		public static async Task WaitWithTimeoutAsync(this Task task, TimeSpan timeout)
		{
			using (var timeoutCancellationTokenSource = new CancellationTokenSource())
			{
				var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
				if (completedTask == task)
				{
					timeoutCancellationTokenSource.Cancel();
					await task;  // Very important in order to propagate exceptions
				}
				else
				{
					throw new TimeoutException(string.Format(AsyncHelperLoc.WaitWithTimeoutAsyncErrorFormat, timeout));
				}
			}
		}

		/// <summary>
		/// Attende il risultato del task per un tempo prestabilito, dopodiché viene lanciato un errore
		/// </summary>
		/// <typeparam name="TResult">Tipo di risultato</typeparam>
		/// <param name="task"></param>
		/// <param name="timeout">Tempo massimo di attesa dopo il quale verrà lanciato un errore</param>
		/// <returns></returns>
		/// <exception cref="TimeoutException">Se il task non ha ritornato un risultato prima del <paramref name="timeout"/></exception>
		public static async Task<TResult> WaitForResultWithTimeoutAsync<TResult>(this Task<TResult> task, TimeSpan timeout)
		{
			using (var timeoutCancellationTokenSource = new CancellationTokenSource())
			{
				var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
				if (completedTask == task)
				{
					timeoutCancellationTokenSource.Cancel();
					return await task;  // Very important in order to propagate exceptions
				}
				else
				{
					throw new TimeoutException(string.Format(AsyncHelperLoc.WaitWithTimeoutAsyncErrorFormat, timeout));
				}
			}
		}

		/// <summary>
		/// Avvia un nuovo task per eseguire l'azione specificata e, se l'azione non si conclude nel tempo previsto, lancia un'eccezione
		/// </summary>
		/// <param name="action"></param>
		/// <param name="process">Processo da tenere sotto controllo</param>
		/// <param name="timeout">Tempo massimo di attesa dopo il quale verrà lanciato un errore</param>
		/// <exception cref="TimeoutException">Se il task non ha ritornato un risultato prima del <paramref name="timeout"/></exception>
		public static void RunInTaskWithTimeoutOrProcessExited(this Action action, Process process, TimeSpan timeout)
		{
			RunSync(() => Task.Run(action).WaitWithTimeoutOrProcessExitedAsync(process, timeout));
		}

		/// <summary>
		/// Avvia un nuovo task per eseguire la funzione specificata e ritornare il valore, se la funzione non ritorna un valore nel tempo previsto, lancia un'eccezione
		/// </summary>
		/// <typeparam name="TResult">Tipo del valore di ritorno</typeparam>
		/// <param name="function"></param>
		/// <param name="process">Processo da tenere sotto controllo</param>
		/// <param name="timeout">Tempo massimo di attesa dopo il quale verrà lanciato un errore</param>
		/// <returns></returns>
		/// <exception cref="TimeoutException">Se il task non ha ritornato un risultato prima del <paramref name="timeout"/></exception>
		public static TResult RunInTaskWithTimeoutOrProcessExited<TResult>(this Func<TResult> function, Process process, TimeSpan timeout)
		{
			return RunSync(() => Task.Run(function).WaitForResultWithTimeoutOrProcessExitedAsync(process, timeout));
		}

		/// <summary>
		/// Attende il risultato del task per un tempo prestabilito o per la chiusura di un processo, dopodiché viene lanciato un errore
		/// </summary>
		/// <param name="task"></param>
		/// <param name="process">Processo da tenere sotto controllo</param>
		/// <param name="timeout">Tempo massimo di attesa dopo il quale verrà lanciato un errore</param>
		/// <returns></returns>
		/// <exception cref="TimeoutException">Se il task non ha ritornato un risultato prima del <paramref name="timeout"/></exception>
		/// <exception cref="IOException">Se il task non ha ritornato un risultato prima che il <paramref name="process"/> si sia concluso</exception>
		public static async Task<TResult> WaitForResultWithTimeoutOrProcessExitedAsync<TResult>(this Task<TResult> task, Process process, TimeSpan timeout)
		{
			using (var timeoutCancellationTokenSource = new CancellationTokenSource())
			{
				var processClosedTask = process.WaitForExitAsync(timeoutCancellationTokenSource.Token);
				var completedTask = await Task.WhenAny(task, processClosedTask, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
				if (completedTask == task)
				{
					timeoutCancellationTokenSource.Cancel();
					return await task;  // Very important in order to propagate exceptions
				}
				else if (completedTask == processClosedTask)
				{
					//se il task che si è concluso è quello della chiusura del processo allora è un altro errore
					throw new IOException(string.Format(AsyncHelperLoc.WaitWithProcessExitedAsyncErrorFormat, process?.Id));
				}
				else
				{
					throw new TimeoutException(string.Format(AsyncHelperLoc.WaitWithTimeoutAsyncErrorFormat, timeout));
				}
			}
		}

		/// <summary>
		/// Attende la conclusione del task per un tempo prestabilito o per la chiusura di un processo, dopodiché viene lanciato un errore
		/// </summary>
		/// <param name="task"></param>
		/// <param name="process">Processo da tenere sotto controllo</param>
		/// <param name="timeout">Tempo massimo di attesa dopo il quale verrà lanciato un errore</param>
		/// <returns></returns>
		/// <exception cref="TimeoutException">Se il task non ha ritornato un risultato prima del <paramref name="timeout"/></exception>
		/// <exception cref="IOException">Se il task non ha ritornato un risultato prima che il <paramref name="process"/> si sia concluso</exception>
		public static async Task WaitWithTimeoutOrProcessExitedAsync(this Task task, Process process, TimeSpan timeout)
		{
			using (var timeoutCancellationTokenSource = new CancellationTokenSource())
			{
				var processClosedTask = process.WaitForExitAsync(timeoutCancellationTokenSource.Token);
				var completedTask = await Task.WhenAny(task, processClosedTask, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
				if (completedTask == task)
				{
					timeoutCancellationTokenSource.Cancel();
					await task;  // Very important in order to propagate exceptions
				}
				else if (completedTask == processClosedTask)
				{
					//se il task che si è concluso è quello della chiusura del processo allora è un altro errore
					throw new IOException(string.Format(AsyncHelperLoc.WaitWithProcessExitedAsyncErrorFormat, process?.Id));
				}
				else
				{
					throw new TimeoutException(string.Format(AsyncHelperLoc.WaitWithTimeoutAsyncErrorFormat, timeout));
				}
			}
		}

		/// <summary>
		/// Safely execute the Task without waiting for it to complete before moving to the next line of code; commonly known as "Fire And Forget". Inspired by John Thiriet's blog post, "Removing Async Void": https://johnthiriet.com/removing-async-void/.
		/// </summary>
		/// <param name="task">Task.</param>
		/// <param name="onException">If an exception is thrown in the Task, <c>onException</c> will execute. If onException is null, the exception will be re-thrown</param>
		/// <param name="continueOnCapturedContext">If set to <c>true</c>, continue on captured context; this will ensure that the Synchronization Context returns to the calling thread. If set to <c>false</c>, continue on a different context; this will allow the Synchronization Context to continue on a different thread</param>
		public static void SafeFireAndForget(this Task task, Action<Exception> onException = null, bool continueOnCapturedContext = false) => HandleSafeFireAndForget(task, continueOnCapturedContext, onException);

		/// <summary>
		/// Safely execute the Task without waiting for it to complete before moving to the next line of code; commonly known as "Fire And Forget". Inspired by John Thiriet's blog post, "Removing Async Void": https://johnthiriet.com/removing-async-void/.
		/// </summary>
		/// <param name="task">Task.</param>
		/// <param name="onException">If an exception is thrown in the Task, <c>onException</c> will execute. If onException is null, the exception will be re-thrown</param>
		/// <param name="continueOnCapturedContext">If set to <c>true</c>, continue on captured context; this will ensure that the Synchronization Context returns to the calling thread. If set to <c>false</c>, continue on a different context; this will allow the Synchronization Context to continue on a different thread</param>
		/// <typeparam name="TException">Exception type. If an exception is thrown of a different type, it will not be handled</typeparam>
		public static void SafeFireAndForget<TException>(this Task task, Action<TException> onException = null, bool continueOnCapturedContext = false) where TException : Exception => HandleSafeFireAndForget(task, continueOnCapturedContext, onException);

#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void

		static async void HandleSafeFireAndForget<TException>(Task task, bool continueOnCapturedContext, Action<TException> onException)
			where TException : Exception
		{
			try
			{
				await task.ConfigureAwait(continueOnCapturedContext);
			}
			catch (TException ex) when (onException != null)
			{
				onException?.Invoke(ex);
			}
		}

#pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void

		#endregion

	}

}
