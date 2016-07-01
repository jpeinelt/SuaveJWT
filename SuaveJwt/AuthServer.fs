﻿module AuthServer

open JwtToken
open Suave.Operators
open Suave.RequestErrors
open Suave.Http

type AudienceCreateRequest = {
    Name : string
}

type AudienceCreateResponse = {
    ClientId : string
    Base64Secret : string
    Name : string
}

type Config = {
    AddAudienceUrlPath : string
    SaveAudience : Audience -> Async<Audience>
}

let audienceWebPart config =
    let toAudienceCreateResponse (audience : Audience) = {
        Base64Secret = audience.Secret.ToString()
        ClientId = audience.ClientId
        Name = audience.Name
    }
    let tryCreateAudience (ctx : HttpContext) =
        match mapJsonPayload<AudienceCreateRequest> ctx.request with
        | Some audienceCreateRequest ->
            async {
                let! audience =
                    audienceCreateRequest.Name
                    |> createAudience
                    |> config.SaveAudience
                let audienceCreateResponse =
                    toAudienceCreateResponse audience
                return! JSON audienceCreateResponse ctx
            }
        | None -> BAD_REQUEST "Ïnvalid Audience Create Request" ctx
    path config.AddAudienceUrlPath >=> POST >=> tryCreateAudience
        