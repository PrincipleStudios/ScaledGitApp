# GitHub Actions setup

GitHub Actions use secrets  generated from [a PowerShell script stored in the
PrincipleStudiosDotCom repository.][gen-credentials]

	./generate-credentials.ps1 -appName ScaledGitApp-GitHub-Actions -k8sNamespace scaledgitapp

These credentials need to be re-run on a regular basis (once per year) or the
actions will begin to fail. (Updating the secrets in the app and re-running will
fix the deployment.)

[gen-credentials]: https://github.com/PrincipleStudios/PrincipleStudiosDotCom/blob/main/deployment/generate-credentials.ps1