pipeline {
    agent any

    stages {

        stage('Build Docker Image') {
            steps {
                sh 'docker build -t orderservice .'
            }
        }

        stage('Stop Old Container') {
            steps {
                sh 'docker stop orderservice-container || true'
                sh 'docker rm orderservice-container || true'
            }
        }

        stage('Remove Old Random Containers') {
            steps {
                sh 'docker ps -aq --filter "ancestor=orderservice" | xargs -r docker rm -f || true'
            }
        }

        stage('Run Docker Container') {
            steps {
                sh 'docker run -d --name orderservice-container -p 8085:8080 orderservice'
            }
        }
    }
}