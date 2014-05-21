properties {
  $solutionPath  = "..\Mercury.ParticleEngine.proj"
  $configuration = "Debug"
  $xunitConsole  = ".\xunit.console.clr4.x86.exe"
  $unitTests     = "..\Mercury.ParticleEngine.Core.Tests\bin\$($configuration)\Mercury.ParticleEngine.Core.Tests.dll"
}

Import-Module ".\teamcity.psm1"

TaskSetup {
  TeamCity-ReportBuildProgress "Running task $($psake.context.Peek().currentTaskName)"
}

task default -depends Clean, Compile, Test

task Test -depends Compile {
  Exec {
    & $xunitConsole $unitTests
  }
}

task Compile -depends Clean {
  Exec {
    msbuild $solutionPath /t:Build /m /tv:4.0 /p:Configuration=$($configuration)
  }
}

task Clean {
  Exec {
    msbuild $solutionPath /t:Clean /m /tv:4.0 /p:Configuration=$($configuration)
  }
}

task ? -Description "Helper to display task info" {
  Write-Documentation
}