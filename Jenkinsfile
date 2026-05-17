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

        stage('Run Docker Container') {
            steps {
                sh 'docker run -d -p 8085:8080 orderservice'
            }
        }
    }
}