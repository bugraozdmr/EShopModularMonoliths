<p align=center>
  <br>
  <span>EShopMicroservices is modular monolith based asp web api</span>
  <br>
</p>

## Clone the Repo
```console
# clone the repo
$ git clone https://github.com/bugraozdmr/EShopModularMonoliths.git
```

## Dotnet Package Installation
```console
# change the working directory EShopModularMonoliths/src
$ cd EShopMicroservices/src

# install packages
$ dotnet restore
```

## Docker Compose Up
```console
# change the working directory EShopModularMonoliths/src
$ cd EShopMicroservices/src

# run the command
$ sudo docker-compose down -v && sudo docker-compose -p modulith up --build
```

## Getting Migrations For Each Modules
```console
# change the working directory EShopModularMonoliths/src
$ cd EShopMicroservices/src

# run the fallowing commands
$ dotnet ef migrations add InitialCreate --output-dir Data/Migrations --project Modules/Catalog --startup-project Bootstrapper/Api
$ dotnet ef migrations add InitialCreate --output-dir Data/Migrations --project Modules/Basket --startup-project Bootstrapper/Api --context BasketDbContext
$ dotnet ef migrations add InitialCreate --output-dir Data/Migrations --project Modules/Ordering --startup-project Bootstrapper/Api --context OrderingDbContext
```

Modular Monolith based project 🎉
