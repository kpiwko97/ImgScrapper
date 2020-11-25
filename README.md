# ImgScrapper

## General information about API
A microservice designed to download images from selected websites.<br/>
You may connect to ImgScrapper API through REST interface using HTTP or HTTPS protocol. No authorisation is needed. Main endpoint adress:
https://imgscrapper.azurewebsites.net/api/ImgScrapper

![Architecture1](https://user-images.githubusercontent.com/38471368/100264102-a9f08980-2f4e-11eb-8565-bd1ac2e6fffd.png)

Azure functions offers integration with a number of Azure services, include Azure Blob Storage, which is a great technology to store images as blobs. It's triggered  by an HTTP request.

## Request body schema: application/json

```
{
    "url": "https://semantive.com"
}
```

## Returned value

Responses: <br/> <br/>
`200 OK` - Response returned by the request returns the number of successfully saved images. <br/>
`400 Bad Request` - If an error has occurred, the user will be informed about the problem in response. <br/>

## Run project locally

1. Run the unpacked project in Visual Studio Code. It is highly recommended to install the extension: Azure Functions
2. Restore Packages or run in terminal:
```sh
dotnet add package Microsoft.Azure.Storage.Blob
```
3. Change line in: `local.settings.json"AzureWebJobsStorage": "Change Me "`. This is Application settings dynamic generated after deployed, or pass to existing BlobStorageAccount
4. Run
