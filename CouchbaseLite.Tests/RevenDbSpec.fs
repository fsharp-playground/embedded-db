module RevenDbSpec

open Raven.Client.Embedded
open FsUnit
open NUnit.Framework
open System.Linq

type Property = {
    Name: string
    Value: obj
}

type Info = {
    Name: string
    Path: string
    Properties: Property list
}

let db() = 
    let db = "test"
    let store = new EmbeddableDocumentStore()
    store.DataDirectory <- "Database"
    store.DefaultDatabase <- db
    store.Initialize()

[<Test>]
let ShouldQueryDocument() =
    let store = db()
    let session = store.OpenSession()
    let doc = session.Load<Info>("10")
    doc |> should equal null

    let docs = session.Query<Info>().Where( fun x -> x.Path = "/root")

    docs.Count() |> should equal 1
    docs.FirstOrDefault().Name |> should equal "Test.pdf"
   

[<Test>]
let ShouldCreateEmptyDatabase() =
    let store = db()
    let info = {
        Info.Name = "Test.pdf"
        Path = "/root"
        Properties = [
                        { Name = "a"; Value = 1}
                        { Name = "b"; Value = 2} ]
    }

    let session = store.OpenSession()
    session.Store(info)
    session.SaveChanges()


