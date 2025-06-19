param (
    [switch]$b,
    [switch]$l
)

$buildOption = if ($b) { "" } else { "--no-build" }

$loggerOption = if ($l) { '--logger "console;verbosity=detailed"' } else { "" }

Get-ChildItem -Recurse -Filter *.sln | ForEach-Object {
    $solutionPath = $_.FullName
    Write-Host "Running tests for solution: $solutionPath"

    $command = "dotnet test `"$solutionPath`" $buildOption $loggerOption"
    Invoke-Expression $command
}