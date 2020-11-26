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

# Img Scrapper 2

![image](https://user-images.githubusercontent.com/38471368/100287662-55acd000-2f75-11eb-8a83-3522dc67eeb2.png)

The most time it takes to download images. The infrastructure was designed to maximally reduce the time of downloading multiple files simultaneously.
At the beginning all tags which contain valid links are scrapped from website and then put on Azure Queue.
If there are a number of links to be processed in the queue is greater than 100, Azure Fumction will automatically scale-out. 

## Authorization

![image](https://user-images.githubusercontent.com/38471368/100326260-05139200-2fca-11eb-85f5-3340229d76df.png)

Each function checks the request headers where the user must provide: username and password.
The question in how to secure credentials and store them in the right place - in this case Azure Key Vault.
Azure Key Vault with Azure Functions can store secrets from the source code in the secure way. 
Add Key Vault secrets reference in the Function App configuration. 
