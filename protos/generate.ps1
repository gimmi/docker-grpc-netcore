$ErrorActionPreference = "Stop"

function Install-NuGetTool($Id, $Version) {
    $OutputDirectory = "$PSScriptRoot\packages"
    $NuGetApp = (Get-Command -Name nuget -CommandType Application).Source
    $NuGetArgs = @(
        "install", $Id,
        "-Version", $Version,
        "-Verbosity", "quiet",
        "-ForceEnglishOutput",
        "-OutputDirectory", $OutputDirectory
    )
    & $NuGetApp $NuGetArgs
    if ($LASTEXITCODE -ne 0) {
        throw "NuGet invocation returned $LASTEXITCODE"
    }

    return "$OutputDirectory\$Id.$Version"
}

$GrpcTools = Install-NuGetTool -Id "Grpc.Tools" -Version "1.14.0"
$ProtoApp = "$GrpcTools\tools\windows_x64\protoc.exe"
$GrpcPluginApp = "$GrpcTools\tools\windows_x64\grpc_csharp_plugin.exe"

$ProtoFiles = Get-ChildItem -Path $PSScriptRoot -Filter "*.proto" -Recurse
foreach ($ProtoFile in $ProtoFiles) {
    $ProtoDir = $ProtoFile.Directory.FullName
    $ProtoFile = $ProtoFile.FullName

    Write-Output "Generating: $ProtoFile"

    $ProtoArgs = @(
        "--proto_path=$ProtoDir",
        "--csharp_out=$ProtoDir",
        "--plugin=protoc-gen-grpc=$GrpcPluginApp",
        # Some additional params can be specified, see https://groups.google.com/d/msg/grpc-io/rM6q0WYXp-w/Qy5R91g7DwAJ
        "--grpc_out=$ProtoDir",
        $ProtoFile
    )

    & $ProtoApp $ProtoArgs
    if ($LASTEXITCODE -ne 0) {
        throw "Proto invocation returned $LASTEXITCODE"
    }
}
