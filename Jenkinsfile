pipeline {
    agent any

    stages {
        stage('Checkout') {
            steps {
                git branch: 'main', url: 'https://github.com/100RBH99/ICS_Tickets_Tools.git'
            }
        }

        stage('Build') {
            steps {
                bat 'dotnet build ICS_Tickets_Tools.sln -c Release'
            }
        }

        stage('Publish') {
            steps {
                bat 'dotnet publish .\\ICS_Tickets_Tools\\ICS_Tickets_Tools.csproj -c Release -o D:\\Deploy\\ICS_Tickets_Tools'
            }
        }

        stage('Deploy to IIS') {
            steps {
                bat '''
                powershell -Command "Import-Module WebAdministration;
                Stop-WebSite -Name 'Tickets_Tools';
                Copy-Item -Path D:\\Deploy\\ICS_Tickets_Tools\\* -Destination 'C:\\inetpub\\wwwroot\\pub' -Recurse -Force;
                Start-WebSite -Name 'Tickets_Tools';"
                '''
            }
        }
    }
}
