pipeline {
    agent any

    stages {
        stage('Check .NET SDK') {
            steps {
                sh 'dotnet --info'
            }
        }

        stage('Build') {
            steps {
                dir('FolderSync') {
                    sh 'dotnet build --configuration Debug'
                }
            }
        }

        stage('Test') {
            steps {
                dir('FolderSync.Tests') {
                    sh 'dotnet test --configuration Debug --logger junit'
                }
            }
        }

        stage('Publish Linux') {
            steps {
                dir('FolderSync') {
                    sh 'dotnet publish -c Release -r linux-x64 --self-contained true -o ../publish/linux'
                }
            }
        }

        stage('Publish Windows') {
            steps {
                dir('FolderSync') {
                    sh 'dotnet publish -c Release -r win-x64 --self-contained true -o ../publish/windows'
                }
            }
        }
    }

    post {
        always {
            junit '**/TestResults/*.xml'
            archiveArtifacts artifacts: 'publish/**/*', fingerprint: true
        }
    }
}
