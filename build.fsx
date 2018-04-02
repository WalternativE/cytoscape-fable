#r @"packages/build/FAKE/tools/FakeLib.dll"

open System

open Fake

let platformTool tool winTool =
  let tool = if isUnix then tool else winTool
  tool
  |> ProcessHelper.tryFindFileOnPath
  |> function Some t -> t | _ -> failwithf "%s not found" tool

let dotnetcliVersion = DotNetCli.GetDotNetSDKVersionFromGlobalJson()
let mutable dotnetCli = "dotnet"

let run cmd args workingDir =
  let result =
    ExecProcess (fun info ->
      info.FileName <- cmd
      info.WorkingDirectory <- workingDir
      info.Arguments <- args) TimeSpan.MaxValue
  if result <> 0 then failwithf "'%s %s' failed" cmd args

let projPath = "src/Cytoscape/"

Target "Clean" (fun _ -> 
  run dotnetCli "clean" projPath
)

Target "InstallDotNetCore" (fun _ ->
  dotnetCli <- DotNetCli.InstallDotNetSDK dotnetcliVersion
)

Target "Restore" (fun () ->
  run dotnetCli "restore" projPath
)

Target "Build" (fun () ->
  run dotnetCli "build" projPath
)

"Clean"
  ==> "InstallDotNetCore"
  ==> "Restore"
  ==> "Build"

RunTargetOrDefault "Build"