node {
  stage('SCM') {
    checkout scm
  }
  stage('SonarQube Analysis') {
    def scannerHome = tool 'SonarScanner for MSBuild'
    withSonarQubeEnv() {
      sh "dotnet ${scannerHome}/SonarScanner.MSBuild.dll begin /k:\"scan_dotnet_connections\""
      sh "dotnet build"
      sh "dotnet ${scannerHome}/SonarScanner.MSBuild.dll end"
    }
  }
}
