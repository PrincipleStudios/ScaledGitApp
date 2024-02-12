#!/usr/bin/env pwsh

Param(
    [Parameter(Mandatory)][String] $required,
    [Parameter()][String] $one,
    [Parameter()][String] $two,
    [Parameter()] $anyType,
    [switch] $switch
)

$data = @{
    PSScriptRoot = $PSScriptRoot
    WorkingDirectory = Get-Location
    PSBoundParameters = $PSBoundParameters
}

Write-Host (ConvertTo-Json $data)

$data
