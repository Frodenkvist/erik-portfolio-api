properties([pipelineTriggers([githubPush()])])


pipeline {
    agent {
        node {
            label 'draupnir'
        }
    }

    stages {
        stage('Build') {
            steps {
                script {
                    docker.build("erik-portfolio-api")
                }
            }
        }
    }
}