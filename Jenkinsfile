properties([pipelineTriggers([githubPush()])])


pipeline {
    agent {
        node {
            label 'draupnir'
        }
    }

    stages {
        stage('Replace tokens') {
            steps {
                script {
                    DB_CONNECTION_STRING=credentials('erikportfolio-db-connection-string')
                    JWT_SECRET=credentials('erikportfolio-jwt-secret')

                    sed -i "s/\${dbConnectionString}/$DB_CONNECTION_STRING/g" ErikPortfolioApi/appsettings.json
                    sed -i "s/\${jwtSecret}/$JWT_SECRET/g" ErikPortfolioApi/appsettings.json
                }
            }
        }

        stage('Build') {
            steps {
                script {
                    docker.build("erik-portfolio-api")
                }
            }
        }

        stage('Deploy') {
            steps {
                script {
                    sh 'docker-compose up -d'
                }
            }
        }
    }
}