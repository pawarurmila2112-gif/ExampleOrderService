pipeline {
    agent any

    stages {

        stage('Build Docker Image') {
            steps {
                bat 'docker build -t orderservice .'
            }
        }

        stage('Run Docker Container') {
            steps {
                bat 'docker run -d -p 8085:8080 orderservice'
            }
        }
    }
}