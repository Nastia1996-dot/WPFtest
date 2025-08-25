using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Shell;

namespace WpfTest
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			if (e.Args.Length > 0 && e.Args[0] == "--open-main-window")
			{
				MainWindowLogged main = new MainWindowLogged();
				main.Show();
			}
			else
			{

				//crea una jumplist e la associa alla mia applicazione
				JumpList jumplist = new JumpList();

				//abilita la categoria Recenti e Frequenti
				jumplist.ShowRecentCategory = true;
				jumplist.ShowFrequentCategory = true;

				JumpList.SetJumpList(Application.Current, jumplist);

				//aggiungo una scorciatoia per MainWindow
				JumpTask openMainLoggedTask = new JumpTask
				{
					Title = "Apri MainWindow",
					Arguments = "--open-main-window",
					Description = "Apri la finestra riservata dopo il login",
					CustomCategory = "Azioni veloci",
					ApplicationPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName
				};

				JumpTask calculator = new JumpTask
				{
					Title = "Calcolatrice",
					Arguments = "",
					Description = "Apri la calcolatrice",
					CustomCategory = "Azioni veloci",
					ApplicationPath = System.IO.Path.Combine(
		Environment.GetFolderPath(Environment.SpecialFolder.Windows),
		"System32",
		"calc.exe")
				};

				//aggiungo i task alla JumpList
				jumplist.JumpItems.Add(openMainLoggedTask);
				jumplist.JumpItems.Add(calculator);
				//applico la lista
				jumplist.Apply();

				//aggiungo un file ai Recenti
				JumpList.AddToRecentCategory(new JumpPath { Path = @"C:\Users\anastasia.sacca\Desktop\Appunti NovaServiceMonitor.docx" });
				JumpList.AddToRecentCategory(new JumpPath { Path = @"C:\Users\anastasia.sacca\Desktop\ProgrammiInstallati.txt" });

			}
		}
	}

}
