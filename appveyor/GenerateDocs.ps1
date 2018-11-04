if($env:APPVEYOR_REPO_TAG -eq 'True' -And $env:framework -eq 'netcoreapp2.0') {
    git config --global credential.helper store
    Add-Content "$env:USERPROFILE\.git-credentials" "https://$($env:git_access_token):x-oauth-basic@github.com`n"

    git config --global user.email "dariokondratiuk@gmail.com"
    git config --global user.name "Dario Kondratiuk"
    git remote add pages https://github.com/kblok/puppeteer-sharp.git
    git fetch pages
    git checkout master
    git subtree add --prefix docs pages/gh-pages
    docfx metadata docfx_project/docfx.json
    docfx build docfx_project/docfx.json -o docs
    git add docs/* -f
    git commit -m "Docs version $($env:APPVEYOR_REPO_TAG_NAME)"
    git subtree push --prefix docs pages gh-pages
}