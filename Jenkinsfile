properties([pipelineTriggers([githubPush()])])


pipeline {
    agent {
        node {
            label 'draupnir'
        }
    }

    stages {
        stage('Clean workspace') {
            steps [
                cleanWs()
            ]
        }

        stage('Git Checkout') {
            steps {
                git branch: 'master', credentialsId: '', url: 'git@github.com:Frodenkvist/erik-portfolio-api.git'
            }
        }
    }
}