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

				EventLog.WriteEntry("DDUSafeBootHandler", "BCDEDIT completed. Stopping the service...", EventLogEntryType.Information)
				Dim stopThread As New Thread(AddressOf StopService)
				stopThread.Start()
			Catch ex As Exception
				EventLog.WriteEntry("DDUSafeBootHandler", ex.Message, EventLogEntryType.Error)
			End Try
		End Sub

		Private Sub StopService()
			' Wait for a short period before stopping the service, just to ensure everything is settled
			Thread.Sleep(500)

			' Stop the service
			Me.Stop()
		End Sub

		Protected Overrides Sub OnStop()
			' Clean up and uninstall the service
			Try
				' Uninstall the service
				UninstallService()
				EventLog.WriteEntry("DDUSafeBootHandler", "Service Uninstalled", EventLogEntryType.Information)
			Catch ex As Exception
				EventLog.WriteEntry("DDUSafeBootHandler", "Error uninstalling the service: " & ex.Message, EventLogEntryType.Error)
			End Try
		End Sub

		Private Sub UninstallService()
			Try
				Dim processInfo As New ProcessStartInfo("sc.exe", "delete DDUSafeBootHandler") With {
				.UseShellExecute = True,
				.CreateNoWindow = True,
				.Verb = "runas"
			}
				Using process As New Process With {
				.StartInfo = processInfo
			}
					process.Start()
					process.WaitForExit()
				End Using
			Catch ex As Exception
				EventLog.WriteEntry("DDUSafeBootHandler", "Error uninstalling the service: " & ex.Message, EventLogEntryType.Error)
			End Try
		End Sub
	End Class
End Namespace