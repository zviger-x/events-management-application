param (
    [Parameter(Position=0, Mandatory=$true)]
    [ValidateSet("up", "down")]
    [string]$command,

    [switch]$b,  # для build
    [switch]$v   # для volumes
)

$ErrorActionPreference = "Stop"

$networkName = "internal-docker-network"
$services = @("UsersMicroservice", "EventsMicroservice", "PaymentMicroservice", "NotificationsMicroservice")

switch ($command) {
    "up" {
        Write-Host "Creating shared Docker network if it doesn't exist..."
        $networkExists = docker network ls --filter name=$networkName -q
        if (-not $networkExists) {
            docker network create $networkName
            Write-Host "Network '$networkName' created."
        } else {
            Write-Host "Network '$networkName' already exists."
        }

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

        Write-Host "All services are up and running."
    }

    "down" {
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

        Write-Host "Removing shared Docker network if it exists..."
        $networkExists = docker network ls --filter name=$networkName -q
        if ($networkExists) {
            docker network rm $networkName
            Write-Host "Network '$networkName' removed."
        } else {
            Write-Host "Network '$networkName' does not exist or was already removed."
        }

        Write-Host "All services have been stopped."
    }
}
