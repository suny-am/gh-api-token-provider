# Github API Token Provider

## Description

A small Azure Function C# application for providing tokens
to be used with the Github API

# Requirements
- Azure Keyvault storage (Stores Github Access token)
- Azure Function App (Runs the application on HttpTrigger on endpoint)
- Have the KEY_VAULT_URI and SECRET_NAME values added to the application settings.

# Security
It is recommended you restrict access and apply CORS for the function app to only the origin that is to request
the token. 
