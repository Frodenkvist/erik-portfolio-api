properties([pipelineTriggers([githubPush()])])


pipeline {
    agent {
        node {
            label 'draupnir'
        }
    }

    stages {
        stage('Replace tokens') {
            environment {
                DB_CONNECTION_STRING=credentials('erikportfolio-db-connection-string')
                JWT_SECRET=credentials('erikportfolio-jwt-secret')
            }
            steps {
                script {
                    sh '''
                    REPLACE_DB='${dbConnectionString}'
                    REPLACE_JWT='${jwtSecret}'

                    sed -i "s/$REPLACE_DB/$DB_CONNECTION_STRING/g" ErikPortfolioApi/appsettings.json
                    sed -i "s/$REPLACE_JWT/$JWT_SECRET/g" ErikPortfolioApi/appsettings.json
                    '''
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