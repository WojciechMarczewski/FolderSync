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
                dir('FolderSync') {
                    sh 'dotnet test --configuration Debug --no-build'
                }
            }
        }
    }
}
