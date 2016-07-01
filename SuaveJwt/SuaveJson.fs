﻿module SuaveJson

open Suave
open Newtonsoft.Json
open Newtonsoft.Json.Serialization

let JSON v =
    let settings = new JsonSerializerSettings()
    settings.ContractResolver <- CamelCasePropertyNamesContractResolver()
    JsonConvert.SerializeObject(v, settings)
    |> OK
    >=> Writers.setMimeType "application/json; charset=utf-8"

let mapJsonPayload<'a> (req : HttpRequest) =
    let fromJson json =
        try
            JsonConvert.DeserializeObject(json, typeof<'a>)
            :?> 'a
            |> Some
        with
        | _ -> None

    let getString =
        System.Text.Encoding.UTF8.GetString

    req.rawForm |> getString |> fromJson
