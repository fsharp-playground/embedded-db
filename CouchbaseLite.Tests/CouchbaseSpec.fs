module CouchbaseSpec 

open FsUnit
open NUnit.Framework
open Couchbase.Lite
open System
open System.Linq

type Property = {
    Name: string
    Value: obj
}

type Info = {
   Path: string
   FielName: string
   Properties: Property list
}

module Db = 
    let private name = "test"
    let private manager = Manager.SharedInstance
    let test = manager.GetDatabase(name)

[<Test>]
let ShouldQueryDocument() =
    let view = Db.test.GetView("info")
    let query = view.CreateQuery()
    ()

[<Test>]
let ShouldRetreiveDocument() =
    let doc = Db.test.GetDocument("___")
    doc.Properties |> should equal null

[<Test>]
let ShouldCreateEmptyDatabaseFile() =
    let doc = Db.test.CreateDocument()

    let dict = new System.Collections.Generic.Dictionary<String, Object>()
    dict.Add("name", "wk")
    dict.Add("age", 20)

    let info = {
        Path = "/root"
        FielName = "Hello.txt"
        Properties = [
                        { Name = "A"; Value = 100 }
                        { Name = "B"; Value = 100 }
                        { Name = "C"; Value = 100 } ]
    }

    dict.Add("info", info)

    let revision = doc.PutProperties dict

    revision.Database.Name  |> should equal "test"