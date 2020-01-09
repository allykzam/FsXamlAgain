/// Starting to implement some helpers on top of ProvidedTypes API
module internal FsXaml.TypeProviders.Helper
open System
open System.IO
open ProviderImplementation.ProvidedTypes

let findConfigFile resolutionFolder (configFileName : string) =
    if Path.IsPathRooted configFileName then
        configFileName
    else
        Path.Combine(resolutionFolder, configFileName)

let seqType ty = typedefof<seq<_>>.MakeGenericType[| ty |]
let listType ty = typedefof<list<_>>.MakeGenericType[| ty |]
let optionType ty = typedefof<option<_>>.MakeGenericType[| ty |]

/// Implements invalidation of schema when the file changes
let watchForChanges (ownerType:TypeProviderForNamespaces) (fileName:string) =
    if fileName.StartsWith("http", System.StringComparison.InvariantCultureIgnoreCase) then None else

    let path = Path.GetDirectoryName(fileName)
    let name = Path.GetFileName(fileName)
    let watcher = new FileSystemWatcher(Filter = name, Path = path)
    watcher.Changed.Add(fun _ -> ownerType.Invalidate())
    watcher.EnableRaisingEvents <- true
    Some (watcher :> IDisposable)

// Get the assembly and namespace used to house the provided types
let thisAssembly = System.Reflection.Assembly.GetExecutingAssembly()
let missingValue = "@@@missingValue###"
