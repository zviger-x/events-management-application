param (
    [switch]$v
)

$ErrorActionPreference = "Stop"

$services = @("UsersMicroservice", "EventsMicroservice", "PaymentMicroservice", "NotificationsMicroservice")

foreach ($service in $services) {
    Write-Host "Stopping $service..."
    Push-Location $service

    $args = @("down")
    if ($v) {
        $args += "-v"
    }

    docker-compose @args

    Pop-Location
}

Write-Host "Removing shared Docker network (if exists)..."

$networkExists = docker network ls --filter name=backend -q
if ($networkExists) {
    docker network rm backend
    Write-Host "Network 'backend' removed."
} else {
    Write-Host "Network 'backend' does not exist or already removed."
}

Write-Host "All services have been stopped."