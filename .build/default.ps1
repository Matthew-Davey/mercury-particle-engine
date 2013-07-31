properties {
  $solutionPath  = "..\Mercury.ParticleEngine.sln"
  $xunitConsole  = ".\xunit.console.clr4.x86.exe"
  $unitTests     = "..\Mercury.ParticleEngine.Core.Tests\bin\Debug\Mercury.ParticleEngine.Core.Tests.dll"
}

task default -depends Clean, Compile, Test

task Test -depends Compile {
  Exec {
    & $xunitConsole $unitTests
  }
}

task Compile -depends Clean {
  Exec {
    msbuild $solutionPath /t:Rebuild /m /tv:4.0 /p:Configuration=Debug
  }
}

task Clean {
  Exec {
    msbuild $solutionPath /t:Clean /m /tv:4.0 /p:Configuration=Debug
  }
}

task ? -Description "Helper to display task info" {
  Write-Documentation
}