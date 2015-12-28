require 'rubygems'
require 'bundler/setup'
require 'albacore'
require 'albacore/tasks/versionizer'

task :default => [:build]

desc 'Clean up the working folder'
build :clean do |build|
  build.nologo
  build.sln = 'Mercury.ParticleEngine.sln'
  build.target = [:Clean]
  build.prop 'configuration', build_configuration
  build.logging = 'detailed'
end

desc 'Extract version information from .semver'
Albacore::Tasks::Versionizer.new :read_semver

desc 'Writes out the AssemblyVersion file'
asmver :version => [:read_semver] do |file|
  file.file_path = 'AssemblyVersion.cs'
  file.attributes assembly_version: ENV['FORMAL_VERSION'],
    assembly_file_version: ENV['BUILD_VERSION'],
    assembly_informational_version: ENV['NUGET_VERSION']
end

desc 'Restores missing nuget packages'
nugets_restore :package_restore do |nuget|
    nuget.out = 'packages'
    nuget.nuget_gem_exe
end

desc 'Executes msbuild/xbuild against the project file'
build :build => [:clean, :version, :package_restore] do |build|
  build.sln = 'Mercury.ParticleEngine.sln'
  build.target = [:Build]
  build.prop 'configuration', build_configuration
  build.logging = 'detailed'
  build.add_parameter '/consoleloggerparameters:PerformanceSummary;Summary;ShowTimestamp'
end

desc 'Executes unit tests'
task :test => [:build] do
  Dir.mkdir(artifacts_path) unless Dir.exist?(artifacts_path)
  xunit = File.join('packages', 'xunit.runner.console.2.1.0', 'tools', 'xunit.console.exe')
  tests = File.join(Dir.pwd, 'Mercury.ParticleEngine.Core.Tests', 'bin', build_configuration, 'Mercury.ParticleEngine.Core.Tests.dll')
  system("#{xunit} #{tests} -Verbose -html #{File.join(artifacts_path, 'test-results.html')} -nunit #{File.join(artifacts_path, 'test-results.xml')}")
end

desc 'Writes out the nuget package for the current version'
nugets_pack :package => [:test] do |nuget|
  Dir.mkdir(artifacts_path) unless Dir.exist?(artifacts_path)
  nuget.configuration = build_configuration
  nuget.files = FileList[File.join('Mercury.ParticleEngine.Core', 'Mercury.ParticleEngine.Core.csproj')]
  nuget.out = 'artifacts'
  nuget.nuget_gem_exe
  nuget.gen_symbols
  nuget.with_metadata do |meta|
    meta.version = ENV['NUGET_VERSION']
    meta.authors = 'Matt Davey'
    meta.description = 'Mercury Particle Engine core assembly'
    meta.release_notes = ''
  end
end

task :publish => [:package] do
  package = File.join(artifacts_path, "Mercury.ParticleEngine.#{ENV['NUGET_VERSION']}.nupkg")
  nuget = Albacore::Nugets::find_nuget_gem_exe
  system(nuget, "push #{package} #{ENV['NUGET_API_KEY']} -Source #{ENV['NUGET_FEED_URL']} -NonInteractive -Verbosity detailed")
end

def artifacts_path
  return File.join(Dir.pwd, 'artifacts')
end

def build_configuration
  return ENV['configuration'] || 'Debug'
end
