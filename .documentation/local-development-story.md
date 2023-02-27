# Local Development Story

To save some money and simplify development and testing we can use some software to emulate some Azure Services;

## Azure Cosmos DB Emulator

**Documentation**: [Install and use the Azure Cosmos DB Emulator for local development and testing](https://learn.microsoft.com/en-us/azure/cosmos-db/local-emulator?tabs=ssl-netstd21)

**Notes**:

- We can set up a local instance of the emulator or use dockerized instance;
- Since this is an emulator no additional changes are required for code and we just need to replace the connection string in `appsettings.json` file;

## Azurite - Azure Blob Store Emulator

**Documentation**: [Use the Azurite emulator for local Azure Storage development](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite?tabs=visual-studio)

**Notes**:

- We can set up a local instance of the emulator or use dockerized instance;
- Since this is an emulator no additional changes are required for code and we just need to replace the connection string in `appsettings.json` file;
- To explore what inside of emulated Blob Store we can use [Azure Storage Explorer](https://azure.microsoft.com/en-us/products/storage/storage-explorer);
