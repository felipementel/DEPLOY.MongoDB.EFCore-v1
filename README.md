# DEPLOY.MongoDB.EFCore-v1

Projeto criado para testar a conexão com o MongoDB utilizando o Entity Framework Core.

![banner](./docs/banner.png)

## Referência
https://devblogs.microsoft.com/dotnet/mongodb-ef-core-provider-whats-new/

## Para atualizar os pacotes do projeto
```powershell
dotnet tool install --global dotnet-outdated-tool
````

````powershell
dotnet list src/DEPLOY.MongoBDEFCore.sln package --outdated
````

```powershell
dotnet outdated src/DEPLOY.MongoBDEFCore.sln --upgrade
````

## Criar ambiente local

### Variáveis de ambiente
Para executar o projeto, é necessário definir as seguintes variáveis de ambiente:
```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
````

## Docker

Configure o arquivo `docker/mongodb.env` com as credenciais do MongoDB:
```env
ADMIN_MONGODB_USER='USER'
ADMIN_MONGODB_PASSWORD='PASSWORD'
BASICAUTH_USERNAME='BROWSER_USER'
BASICAUTH_PASSWORD='BROWSER_PASSWORD'
```


Para subir o ambiente com MongoDB, OpenTelemetry e Jeager, execute o seguinte comando:
```powershell
docker compose -f docker/infra-docker.yml --env-file ./docker/mongodb.env down
```

Para desmontar o ambiente, execute:
```powershell
docker compose -f docker/infra-docker.yml --env-file ./docker/mongodb.env down
```


Execute o comando no diretório raiz do projeto:
````
dotnet run --project ./src/DEPLOY.MongoBDEFCore.API --launch-profile "https"
````
