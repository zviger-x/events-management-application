param (
    [switch]$b
)

$ErrorActionPreference = "Stop"

Write-Host "Creating shared Docker network (if not exists)..."

$networkExists = docker network ls --filter name=backend -q
if (-not $networkExists) {
    docker network create backend
    Write-Host "Network 'backend' created."
} else {
    Write-Host "Network 'backend' already exists."
}

$services = @("UsersMicroservice", "EventsMicroservice", "PaymentMicroservice", "NotificationsMicroservice")

foreach ($service in $services) {
    Write-Host "Starting $service..."
    Push-Location $service

    $args = @("up", "-d")
    if ($b) {
        $args += "--build"
    }

    docker-compose @args

    Pop-Location
}

Write-Host "All services are up and running!"