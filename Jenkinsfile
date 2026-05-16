pipeline {
    agent any

    stages {

        stage('Build Docker Image') {
            steps {
                sh 'docker build -t orderservice .'
            }
        }

        stage('Run Docker Container') {
            steps {
                sh 'docker run -d -p 8085:8080 orderservice'
            }
        }
    }
}