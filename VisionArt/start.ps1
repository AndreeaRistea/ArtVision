$apiJob = Start-Job -ScriptBlock {
    Set-Location "$using:PSScriptRoot\VisionArt.Api"
    dotnet run --launch-profile https
}

$webJob = Start-Job -ScriptBlock {
    Set-Location "$using:PSScriptRoot\VisionArt.Web"
    dotnet run --launch-profile https
}

Write-Host "Started VisionArt.Api (PID: $($apiJob.Id)) and VisionArt.Web (PID: $($webJob.Id))" -ForegroundColor Green
Write-Host "Press Ctrl+C to stop both projects." -ForegroundColor Yellow

try {
    Receive-Job -Job $apiJob, $webJob -Wait -AutoRemoveJob
} finally {
    Stop-Job -Job $apiJob, $webJob -ErrorAction SilentlyContinue
    Remove-Job -Job $apiJob, $webJob -ErrorAction SilentlyContinue
}
