# Demo for Swagger security trimming 

For details see [blog post](https://blog.jenyay.com/swagger-security-trimming-of-end-points-in-asp-net-core-application).

There are demos for multiple versions of Swagger and framework versions, see project files for version information.

## Running

To run the demo:

* Have [.NET Core](https://www.microsoft.com/net/core) installed 
* Clone the repo
* Execute:
```
cd src/V3 # or previous versions
dotnet run
```

Using tokens below different endpoint will be showen in Swagger.

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
