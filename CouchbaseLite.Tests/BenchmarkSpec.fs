module BenchmarkSpec

open BenchmarkDotNet
open Raven.Client.Embedded
open FSharp.Core.Fluent
open NUnit.Framework
open FsUnit
open BenchmarkDotNet.Tasks

type File = 
    { Name : string
      Path : string
      Extension : string
      Length : int }

[<BenchmarkTask(platform = BenchmarkPlatform.X86, jitVersion = BenchmarkJitVersion.LegacyJit)>]
//[<BenchmarkTask(platform = BenchmarkPlatform.X64, jitVersion = BenchmarkJitVersion.LegacyJit)>]
//[<BenchmarkTask(platform = BenchmarkPlatform.X64, jitVersion = BenchmarkJitVersion.RyuJit)>]
type Db() = 
    
    let createStore() = 
        let db = "test"
        let store = new EmbeddableDocumentStore()
        store.DataDirectory <- "Database"
        store.DefaultDatabase <- db
        store.Initialize()
    
    let createDoc name = 
        { Name = name
          Path = name
          Extension = name
          Length = name.Length }
    
    [<Benchmark>]
    member this.Insert() = 
        use store = createStore()
        use session = store.OpenSession()
        [ 1..100000].map(fun x -> x.ToString() |> createDoc).map(session.Store) |> ignore
        session.SaveChanges()
    
    [<Benchmark>]
    member this.Query() = 
        use store = createStore()
        use session = store.OpenSession()
        let docs = session.Query<File>().where(fun x -> x.Name.StartsWith("1"))
        docs.toList()

[<Test>]
let ShouldInsertAndQuery() = 
    let db = Db()
    db.Insert()
    db.Query().length |> should greaterThan 0

[<Test>]
let ShouldExecuteBenchmark() = 
    let reports = BenchmarkRunner().RunCompetition(Db())
    ()
