<System.ComponentModel.RunInstaller(True)> Partial Class projectInstaller
    Inherits System.Configuration.Install.Installer

    'Installer overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Component Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Component Designer
    'It can be modified using the Component Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.ServiceInstaller1 = New System.ServiceProcess.ServiceInstaller
        Me.ServiceProcessInstaller1 = New System.ServiceProcess.ServiceProcessInstaller
        '
        'ServiceInstaller1
        '
        Me.ServiceInstaller1.DisplayName = "RunBat Service"
        Me.ServiceInstaller1.ServiceName = "RUNBAT"
        Me.ServiceInstaller1.StartType = System.ServiceProcess.ServiceStartMode.Automatic
        '
        'ServiceProcessInstaller1
        '
        Me.ServiceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalService
        Me.ServiceProcessInstaller1.Password = Nothing
        Me.ServiceProcessInstaller1.Username = Nothing
        '
        'projectInstaller
        '
        Me.Installers.AddRange(New System.Configuration.Install.Installer() {Me.ServiceInstaller1, Me.ServiceProcessInstaller1})

    End Sub
    Friend WithEvents ServiceInstaller1 As System.ServiceProcess.ServiceInstaller
    Public WithEvents ServiceProcessInstaller1 As System.ServiceProcess.ServiceProcessInstaller

End Class
