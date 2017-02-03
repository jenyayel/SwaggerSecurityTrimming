# Demo for Swagger security trimming 

For details see [blog post](https://blog.jenyay.com/swagger-security-trimming-of-end-points-in-asp-net-core-application).

`V1` contains Swagger UI client where to pass Bearer token we had to overide `index.html` file. `V2` contains example with latest version of Swagger UI, where there is a configuration
for passing token in header and no `index.html` overide required.

## Running

To run the demo:

* Have [.NET Core](https://www.microsoft.com/net/core) installed 
* Clone the repo
* Execute:
```
dotnet restore
cd src/V1 # or cd src/V2
dotnet run
```

Browse to [http://localhost:5000](http://localhost:5000). Using tokens below different endpoint will be showen in Swagger.

## Tokens

Authenticated user:
```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.e30.lMQZ-L2fcyy9tgM_RyOt7BCyHnnryQweBmhgVUC9Qc4
```

With scope `can-update`:
```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzY29wZXMiOlsiYXBpOnVwZGF0ZSJdfQ.ok6saWx1101ygDqz-GrhHBJMyINUB2NqpE4k6BYc47s
```

With scope `can-delete`:
```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzY29wZXMiOlsiYXBpOmRlbGV0ZSJdfQ.6YAU8_DLiyixE2xxoGuZnPTOo6Dzoz4cQ3QzM69p5o4
```

With both scopes `can-update` and `can-delete`:
```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzY29wZXMiOlsiYXBpOnVwZGF0ZSIsImFwaTpkZWxldGUiXX0.DXynNpRlNLUWevAazv4vEOLYDGzkEfI8OAnP2qihJr8
```
