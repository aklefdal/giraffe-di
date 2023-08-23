module ToWebApp.Storage

open Azure
open Azure.Data.Tables

let createTableClientFraConnectionString (connectionString: string) : TableClient =
    let tableServiceClient = TableServiceClient(connectionString)
    let tableClient = tableServiceClient.GetTableClient("DiDemo")
    tableClient.CreateIfNotExists() |> ignore
    tableClient

let partitionKey = "sum"
let rowKey = "sum"
let sumProperty = "sum"

let getSum (tableClient: TableClient)  =
    task {
        let! currentSum = tableClient.GetEntityIfExistsAsync<TableEntity>(partitionKey, rowKey)
        if currentSum.HasValue then
            return currentSum.Value.GetInt32(sumProperty).Value, currentSum.Value.ETag
        else
            let entity = TableEntity(partitionKey, rowKey)
            entity.Add(sumProperty, 0)
            let! _ = tableClient.AddEntityAsync(entity)
            return 0, ETag.All
    }

let add (tableClient: TableClient) (i: int) =
    task {
        let! currentSum, etag = getSum tableClient
        let newSum = currentSum + i
        let entity = TableEntity(partitionKey, rowKey)
        entity.Add(sumProperty, newSum)
        let! _ = tableClient.UpdateEntityAsync(entity, etag, TableUpdateMode.Replace)
        return newSum
    }
