Imports System.ServiceProcess
Imports System.Threading

Namespace Display_Driver_Uninstaller
	Public Class DDUSafeBootService
		Inherits ServiceBase

		Public Sub New()
			ServiceName = "DDUSafeBootHandler"
			CanStop = True
			CanPauseAndContinue = False
			AutoLog = True
		End Sub

		Protected Overrides Sub OnStart(ByVal args() As String)
			Try
				' Attendre un peu pour s'assurer que tous les services nécessaires sont démarrés
				Thread.Sleep(2000)

				' Exécuter bcdedit pour supprimer safeboot
				Dim processInfo As New ProcessStartInfo(Application.Paths.System32 & "BCDEDIT", "/deletevalue safeboot") With {
				.UseShellExecute = False,
				.CreateNoWindow = True,
				.Verb = "runas"
			}

				Using process As New Process With {
				.StartInfo = processInfo
			}
					process.Start()
					process.WaitForExit()
				End Using
				' Auto-suppression du service après exécution
				UninstallService()
				StopService()
				EventLog.WriteEntry("DDUSafeBootHandler", "Service Uninstalled", EventLogEntryType.Information)
			Catch ex As Exception
				EventLog.WriteEntry("DDUSafeBootHandler", ex.Message, EventLogEntryType.Error)
			End Try
		End Sub

		Private Sub UninstallService()
			Try
				Dim processInfo As New ProcessStartInfo("sc.exe", "delete DDUSafeBootHandler") With {
				.UseShellExecute = True,
				.CreateNoWindow = True,
				.Verb = "runas"
			}
				Process.Start(processInfo)
			Catch ex As Exception
				EventLog.WriteEntry("DDUSafeBootHandler", "Erreur lors de la désinstallation: " & ex.Message, EventLogEntryType.Error)
			End Try
		End Sub

		Private Sub StopService()
			Try
				Dim processInfo As New ProcessStartInfo("sc.exe", "stop DDUSafeBootHandler") With {
				.UseShellExecute = True,
				.CreateNoWindow = True,
				.Verb = "runas"
			}
				Process.Start(processInfo)
			Catch ex As Exception
				EventLog.WriteEntry("DDUSafeBootHandler", "Erreur lors de la désinstallation: " & ex.Message, EventLogEntryType.Error)
			End Try
		End Sub
	End Class
End Namespace